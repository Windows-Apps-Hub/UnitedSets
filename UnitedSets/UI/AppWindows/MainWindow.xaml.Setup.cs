using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Linq;
using UnitedSets.Classes;
using UnitedSets.Tabs;
using UnitedSets.Helpers;
using WinRT.Interop;
using WinUIEx.Messaging;
using Window = WinWrapper.Windowing.Window;
using Get.EasyCSharp;
using Microsoft.UI.Windowing;
using Keyboard = WinWrapper.Input.Keyboard;
using Windows.Foundation;
using System.Runtime.CompilerServices;
using Icon = WinWrapper.Icon;
using Microsoft.UI.Xaml.Controls;
using System.IO;
using System.Threading.Tasks;
using UnitedSets.Mvvm.Services;

namespace UnitedSets.UI.AppWindows;

partial class MainWindow
{
    public MainWindow()
    {
        Win32Window = Window.FromWindowHandle(WindowNative.GetWindowHandle(this));
        UnitedSetsApp.Current.RegisterUnitedSetsWindow(this);

        InitializeComponent();

        ui_configs = new() { TitleUpdate = UpdateTitle, WindowBorder = WindowBorderOnTransparent, MainAreaBorder = MainAreaBorder }; //, swapChain = this.swapChainPanel
        UnitedSetsApp.Current.Configuration.PersistantService.init(ui_configs);

        SetupBasicWindow();
        
        void UpdateBackdrop(USBackdrop x)
        {
            SystemBackdrop = x.GetSystemBackdrop();
        }
        Settings.BackdropMode.PropertyChanged += (_, _) => UpdateBackdrop(Settings.BackdropMode.Value);
        UpdateBackdrop(Settings.BackdropMode.Value);

        void UpdateMinSize(bool bypass)
        {
            MinWidth = bypass ? Constants.BypassMinWidth : Constants.MinWidth;
            MinHeight = bypass ? Constants.BypassMinHeight : Constants.MinHeight;
        }
        Settings.BypassMinimumSize.PropertyChanged += (_, _) => UpdateMinSize(Settings.BypassMinimumSize.Value);
        UpdateMinSize(Settings.BypassMinimumSize.Value);

        ((OverlappedPresenter)AppWindow.Presenter).SetBorderAndTitleBar(true, true);

        SetupNative(out WindowMessageMonitor);

        SetupEvent();

        SetupUIThreadLoopTimer(out timer);

        // SetupTaskbarMode();

        // Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SetupBasicWindow()
        {
            Title = "UnitedSets";
            ExtendsContentIntoTitleBar = true;
            //SetTitleBar(CustomDragRegion);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SetupEvent()
        {
            AppWindow.Closing += OnWindowClosing;
            Activated += FirstRun;
            WindowMessageMonitor.WindowMessageReceived += OnWindowMessageReceived;
            TabBase.OnUpdateStatusLoopComplete += OnDifferentThreadLoop;
            Cell.ValidDrop += CellWindowDropped;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SetupNative(out WindowMessageMonitor WindowMessageMonitor)
        {
            WindowMessageMonitor = new WindowMessageMonitor(Win32Window);
        }
    }

    public class CfgElements
    {
        public Border? WindowBorder;
        public Border? MainAreaBorder;
        public SwapChainPanel? swapChain;
        public required Action TitleUpdate { init; get; }
    }

    private CfgElements ui_configs;

    private void UpdateTitle()
    {
        var prefix = Configuration.TitlePrefix;
        if (UnitedSetsApp.Current.SelectedTab is { } tab)
            prefix = $"{prefix} {tab.Title} (+{UnitedSetsApp.Current.Tabs.Count - 1} Tabs) - ";

        Title = $"{prefix}United Sets";
    }

    public USConfig Configuration => UnitedSetsApp.Current.Configuration.MainConfiguration;

    [Event(typeof(TypedEventHandler<object, WindowActivatedEventArgs>))]
    void FirstRun()
    {
        Activated -= FirstRun;
        var icoFile = Path.IsPathRooted(Configuration.TaskbarIco) ? Configuration.TaskbarIco : Path.Combine(USConfig.RootLocation, Configuration.TaskbarIco!);
        var icon = Icon.Load(icoFile);
        Win32Window.SmallIcon = Win32Window.LargeIcon = icon;

        if (Keyboard.IsShiftDown)
            Win32Window.SetAppId($"UnitedSets {Win32Window.Handle}");
        HandleCLICmds();
        UnitedSetsApp.Current.Configuration.PersistantService.FinalizeLoadAsync();
    }
    async void HandleCLICmds()
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private partial void SetupUIThreadLoopTimer(out DispatcherQueueTimer timer)
    {
        timer = DispatcherQueue.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(500);
        timer.Tick += OnUIThreadTimerLoop;
        timer.Start();
    }
}
