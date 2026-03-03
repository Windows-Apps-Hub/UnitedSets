using System.Diagnostics;
using System.IO;
using Microsoft.UI.Dispatching;
using UnitedSets.Apps;
using UnitedSets.Configurations;
using UnitedSets.Helpers;
using UnitedSets.Mvvm.Services;
using UnitedSets.Tabs;
using UnitedSets.UI.Popups;

namespace UnitedSets;

[QuickMarkup("""
    using UnitedSets.Tabs;
    TabBase? SelectedTab;
    """)]
partial class UnitedSetsApp : INotifyPropertyChanged
{
    // singleton setup
    private UnitedSetsApp()
    {
        Init();
        Settings = new();
        Configuration = new();
        SelectedTabProp!.Watch(tab => tab?.IsFlashing = false);
    }
    public UnitedSetsAppSettings Settings { get; } = new();
    public UnitedSetsAppConfiguration Configuration { get; } = new();
    public static UnitedSetsApp Current { get; } = new();
    readonly List<WindowEx> _allWindows = [];

    public IReadOnlyList<WindowEx> AllUnitedSetsWindows => _allWindows;
    public MainWindow MainWindow { get; private set; } = null!;
    public DispatcherQueue DispatcherQueue { get; private set; } = null!;
    public ObservableCollection<TabBase> Tabs { get; } = [];
    public ObservableCollection<TabGroup> HiddenTabs { get; } = [];
    public event PropertyChangedEventHandler? PropertyChanged;
    public void RegisterUnitedSetsWindow(WindowEx window) => _allWindows.Add(window);
    public void RegisterUnitedSetsWindow(MainWindow window)
    {
        MainWindow = window;
        DispatcherQueue = window.DispatcherQueue;
        RegisterUnitedSetsWindow(WindowEx.FromWindowHandle((nint)window.AppWindow.Id.Value));

        ReactiveScheduler.AddTickCallbackForCurrentThread(() =>
            DispatcherQueue.TryEnqueue(DispatcherQueuePriority.High, ReactiveScheduler.Tick)
        );
    }

    [DoesNotReturn]
    public async Task Suicide()
    {
        //trans_mgr?.Cleanup();
        await Task.Delay(300);
        Debug.WriteLine("Cleanish exit");
        Environment.Exit(0);
    }

    public async void HandleCLICmds()
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

        foreach (var itm in toAdd)
        {
            var procs = System.Diagnostics.Process.GetProcesses().Where(p => p.ProcessName.Equals(itm, StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var proc in procs)
            {
                if (!proc.HasExited && WindowHostTab.Create(WindowEx.FromWindowHandle(proc.MainWindowHandle)) is { } tab)
                    UnitedSetsApp.Current.Tabs.Add(tab);
            }
        }
        if (editLastAddedWindow && UnitedSetsApp.Current.Tabs.Count > 0)
            UnitedSetsApp.Current.Tabs.Last().TabDoubleTapped(this, new Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs());
    }

    public AddTabPopup AddTabPopup => field ??= new();
    public async void OpenAddTabDialog()
    {
        MainWindow.Win32Window.Minimize();
        await AddTabPopup.ShowAsync();
        MainWindow.Win32Window.Restore();
        var result = AddTabPopup.Result;
        if (WindowHostTab.Create(result) is { } tab)
        {
            UnitedSetsApp.Current.Tabs.Add(tab);
            UnitedSetsApp.Current.SelectedTab = tab;
        }
    }
}
