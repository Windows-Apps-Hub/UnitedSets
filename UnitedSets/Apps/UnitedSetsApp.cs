using Microsoft.UI.Dispatching;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnitedSets.Tabs;
using UnitedSets.UI.AppWindows;
using Window = WinWrapper.Windowing.Window;
using System;
using Get.EasyCSharp;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Threading.Tasks;
using UnitedSets.Mvvm.Services;
using UnitedSets.Helpers;
using System.IO;
using UnitedSets.Configurations;

namespace UnitedSets;

partial class UnitedSetsApp : INotifyPropertyChanged
{
    // singleton setup
    private UnitedSetsApp()
    {
        Settings = new();
        Configuration = new();
    }
    public UnitedSetsAppSettings Settings { get; } = new();
    public UnitedSetsAppConfiguration Configuration { get; } = new();
    public static UnitedSetsApp Current { get; } = new();
    readonly List<Window> _allWindows = [];

    public IReadOnlyList<Window> AllUnitedSetsWindows => _allWindows;
    public MainWindow MainWindow { get; private set; } = null!;
    public DispatcherQueue DispatcherQueue { get; private set; } = null!;
    public ObservableCollection<TabBase> Tabs { get; } = [];
    public ObservableCollection<TabGroup> HiddenTabs { get; } = [];
    [AutoNotifyProperty(OnChanged = nameof(OnSelectedTabChanged))]
    TabBase? _SelectedTab;
    public event PropertyChangedEventHandler? PropertyChanged;
    void OnSelectedTabChanged()
    {
        if (_SelectedTab is { } tab)
            tab.IsFlashing = false;
    }
    public void RegisterUnitedSetsWindow(Window window) => _allWindows.Add(window);
    public void RegisterUnitedSetsWindow(MainWindow window)
    {
        MainWindow = window;
        DispatcherQueue = window.DispatcherQueue;
        RegisterUnitedSetsWindow(Window.FromWindowHandle((nint)window.AppWindow.Id.Value));
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
                if (!proc.HasExited && WindowHostTab.Create(Window.FromWindowHandle(proc.MainWindowHandle)) is { } tab)
                    UnitedSetsApp.Current.Tabs.Add(tab);
            }
        }
        if (editLastAddedWindow && UnitedSetsApp.Current.Tabs.Count > 0)
            UnitedSetsApp.Current.Tabs.Last().TabDoubleTapped(this, new Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs());
    }
}
