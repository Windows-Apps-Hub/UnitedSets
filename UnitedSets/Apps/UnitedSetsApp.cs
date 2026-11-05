using System.Diagnostics;
using System.IO;
using Microsoft.UI.Dispatching;
using UnitedSets.Apps;
using UnitedSets.Configurations;
using UnitedSets.Helpers;
using UnitedSets.Mvvm.Services;
using UnitedSets.Services;
using UnitedSets.Tabs;
using UnitedSets.UI.Popups;
using WindowHoster;

namespace UnitedSets;

/// <summary>
/// App-wide state. Every <see cref="MainWindow"/> is an independent "set" with its own tabs
/// (like browser windows); this class keeps track of all of them.
/// </summary>
partial class UnitedSetsApp : INotifyPropertyChanged
{
    // singleton setup
    private UnitedSetsApp()
    {
        // NOTE: these used to be created twice (property initializer + constructor). Loading the configuration
        // reads *and deletes* the saved session file, so the second instance never saw the saved tabs
        // and "Save and Close" could never restore them.
        Settings = new();
        Configuration = new();
    }
    public UnitedSetsAppSettings Settings { get; }
    public UnitedSetsAppConfiguration Configuration { get; }
    public static UnitedSetsApp Current { get; } = new();
    readonly List<WindowEx> _allWindows = [];

    /// <summary>
    /// All windows created by United Sets (set windows, settings, popups...). These must never be added as tabs.
    /// </summary>
    public IReadOnlyList<WindowEx> AllUnitedSetsWindows => _allWindows;
    /// <summary>
    /// All open set windows.
    /// </summary>
    public ObservableCollection<MainWindow> SetWindows { get; } = [];
    /// <summary>
    /// The set window that was active most recently. Used as the target for actions that
    /// don't come from a specific window (command line, restoring a session, dialogs...).
    /// </summary>
    public MainWindow MainWindow => LastActiveWindow ?? SetWindows.FirstOrDefault()!;
    public MainWindow? LastActiveWindow { get; private set; }
    public DispatcherQueue DispatcherQueue { get; private set; } = null!;
    /// <summary>
    /// Tabs set aside (see the main menu). Shared by all set windows.
    /// </summary>
    public ObservableCollection<TabGroup> HiddenTabs { get; } = [];
    /// <summary>
    /// Every tab of every set window.
    /// </summary>
    public IEnumerable<TabBase> AllTabs => SetWindows.ToArray().SelectMany(w => w.Tabs.ToArray());
    public event PropertyChangedEventHandler? PropertyChanged;
    public void RegisterUnitedSetsWindow(WindowEx window)
    {
        if (!_allWindows.Contains(window))
            _allWindows.Add(window);
    }
    public void UnregisterUnitedSetsWindow(WindowEx window) => _allWindows.Remove(window);
    bool initialized;
    public void RegisterUnitedSetsWindow(MainWindow window)
    {
        RegisterUnitedSetsWindow(window.Win32Window);
        SetWindows.Add(window);
        LastActiveWindow ??= window;
        window.Activated += (_, e) =>
        {
            if (e.WindowActivationState is not WindowActivationState.Deactivated)
                LastActiveWindow = window;
        };
        InitializeServices(window.DispatcherQueue);
    }

    /// <summary>
    /// Starts the app-wide services (window hooks, drag to group, hotkeys, tray icon).
    /// Called once, on the UI thread, before or with the first set window.
    /// </summary>
    public void InitializeServices(DispatcherQueue dispatcherQueue)
    {
        if (initialized) return;
        initialized = true;
        DispatcherQueue = dispatcherQueue;
        // WinEvent hooks must live on a thread with a message loop
        WinEvents.HookDispatcherQueue = DispatcherQueue;

        ReactiveScheduler.AddTickCallbackForCurrentThread(() =>
            DispatcherQueue.TryEnqueue(DispatcherQueuePriority.High, ReactiveScheduler.Tick)
        );
        RegisteredWindow.HostedWindowStateRequested += OnHostedWindowStateRequested;
        WindowDragToGroupService.Start();
        HotkeyService.Start();
        TrayIconService.Start();
    }

    /// <summary>
    /// Whether the app keeps running (in the notification area) when the last set window is closed.
    /// </summary>
    /// <remarks>Always: United Sets only exits from the tray icon menu.</remarks>
    public bool RunsInBackground => true;

    /// <summary>
    /// Releases every window of every set and exits.
    /// </summary>
    public async void ExitReleasingAllWindows()
    {
        foreach (var window in SetWindows.ToArray())
            foreach (var tab in window.Tabs.ToArray())
            {
                window.Tabs.Remove(tab);
                await tab.DetachAndDisposeAsync();
            }
        foreach (var group in HiddenTabs.ToArray())
            foreach (var tab in group.Tabs.ToArray())
                await tab.DetachAndDisposeAsync();
        await Suicide();
    }
    public void UnregisterSetWindow(MainWindow window)
    {
        SetWindows.Remove(window);
        UnregisterUnitedSetsWindow(window.Win32Window);
        if (LastActiveWindow == window)
            LastActiveWindow = SetWindows.LastOrDefault();
    }

    /// <summary>
    /// Opens a new, empty set window.
    /// </summary>
    /// <param name="bounds">The window rectangle (screen pixels, like <c>GetWindowRect</c>), or null for the default</param>
    public MainWindow CreateNewSetWindow(System.Drawing.Rectangle? bounds = null)
    {
        var window = new MainWindow();
        if (bounds is { } b && b.Width > 0 && b.Height > 0)
            // before activating, so it doesn't show up somewhere else first
            window.AppWindow.MoveAndResize(new(b.X, b.Y, b.Width, b.Height));
        window.Activate();
        return window;
    }

    /// <summary>
    /// Finds the tab (in any set window) that hosts the given window.
    /// </summary>
    public (MainWindow Window, TabBase Tab)? FindTab(nint hwnd)
    {
        foreach (var window in SetWindows.ToArray())
            foreach (var tab in window.Tabs.ToArray())
                if (tab.ContainsWindow(hwnd))
                    return (window, tab);
        return null;
    }

    /// <summary>
    /// Finds the set window that the given window belongs to: either the set window itself,
    /// or the set window hosting it.
    /// </summary>
    public MainWindow? FindSetWindowOf(WindowEx window)
    {
        if (!window.IsValid) return null;
        var root = window.Root;
        foreach (var setWindow in SetWindows.ToArray())
            if (setWindow.Win32Window == root || setWindow.Win32Window == window)
                return setWindow;
        return FindTab(root.Handle)?.Window;
    }

    /// <summary>
    /// Moves a tab to another set window, keeping the hosted window and all its settings.
    /// </summary>
    public void MoveTab(TabBase tab, MainWindow target, int index = -1)
    {
        var source = tab.ParentWindow;
        if (source is not null && source != target)
            source.Tabs.Remove(tab);
        else if (source == target)
        {
            var oldIndex = target.Tabs.IndexOf(tab);
            if (oldIndex >= 0)
            {
                if (index < 0 || index >= target.Tabs.Count) index = target.Tabs.Count - 1;
                if (oldIndex != index) target.Tabs.Move(oldIndex, index);
                target.SelectedTab = tab;
                return;
            }
        }
        if (index < 0 || index > target.Tabs.Count)
            target.Tabs.Add(tab);
        else
            target.Tabs.Insert(index, tab);
        target.SelectedTab = tab;
        target.Activate();
        source?.CloseIfEmptyAfterTabMovedOut();
    }

    void OnHostedWindowStateRequested(RegisteredWindow window, HostedWindowStateRequest request)
    {
        // Groupy-like: minimizing/maximizing a tabbed window minimizes/maximizes the whole set
        var setWindow = FindTab(window.Window.Handle)?.Window;
        if (setWindow is null)
        {
            // cell tabs
            foreach (var w in SetWindows.ToArray())
                if (w.Tabs.ToArray().Any(t => t.ContainsRegisteredWindow(window)))
                    setWindow = w;
        }
        if (setWindow is null) return;
        if (request is HostedWindowStateRequest.Minimize)
            setWindow.Win32Window.Minimize();
        else
            setWindow.ToggleMaximize();
    }

    [DoesNotReturn]
    public async Task Suicide()
    {
        HotkeyService.Stop();
        TrayIconService.Stop();
        await Task.Delay(300);
        Debug.WriteLine("Cleanish exit");
        Environment.Exit(0);
    }

    public async void HandleCLICmds(MainWindow? target = null)
    {
        var toAdd = CLI.GetArrVal("add-window-by-exe");
        var editLastAddedWindow = CLI.GetFlag("edit-last-added");
        //LeftFlyout.NoAutoClose = CLI.GetFlag("edit-no-autoclose");
        var profile = CLI.GetVal("profile");
        if (!string.IsNullOrWhiteSpace(profile))
        {
            if (Path.HasExtension(profile) == false)
                profile += ".json";
            if (!File.Exists(profile) && !Path.IsPathRooted(profile))
                profile = Path.Combine(USConfig.BaseProfileFolder, profile);
            if (File.Exists(profile))
            {
                await Task.Delay(1500);
                await UnitedSetsApp.Current.Configuration.PersistantService.ImportSettings(profile);
            }
        }
        target ??= MainWindow;
        TabBase? lastAdded = null;
        foreach (var itm in toAdd)
        {
            var procs = System.Diagnostics.Process.GetProcesses().Where(p => p.ProcessName.Equals(itm, StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var proc in procs)
            {
                if (proc.HasExited || proc.MainWindowHandle == 0) continue;
                // hide the window until its tab is selected (it used to stay floating on the desktop)
                if (WindowHostTab.Create(WindowEx.FromWindowHandle(proc.MainWindowHandle), shouldBeHidden: true) is { } tab)
                {
                    target.Tabs.Add(tab);
                    lastAdded = tab;
                }
            }
        }
        if (lastAdded is not null)
            target.SelectedTab = lastAdded;
        if (editLastAddedWindow && lastAdded is not null)
            lastAdded.TabDoubleTapped(this, new Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs());
    }

    public AddTabPopup AddTabPopup => field ??= new();
    public async void OpenAddTabDialog(MainWindow? target = null)
    {
        target ??= MainWindow;
        target.Win32Window.Minimize();
        await AddTabPopup.ShowAsync();
        target.Win32Window.Restore();
        var result = AddTabPopup.Result;
        if (WindowHostTab.Create(result) is { } tab)
        {
            target.Tabs.Add(tab);
            target.SelectedTab = tab;
        }
    }
}
