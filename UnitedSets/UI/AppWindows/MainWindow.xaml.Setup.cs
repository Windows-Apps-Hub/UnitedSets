using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Get.OutOfBoundsFlyout;
using System;
using System.Linq;
using UnitedSets.Classes;
using UnitedSets.Classes.Tabs;
using UnitedSets.Helpers;
using WinRT.Interop;
using WinUIEx.Messaging;
using Window = WinWrapper.Windowing.Window;
using EasyCSharp;
using Microsoft.UI.Windowing;
using Keyboard = WinWrapper.Input.Keyboard;
using Windows.ApplicationModel;
using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Foundation;
using System.Runtime.CompilerServices;
using System.Drawing;
using UnitedSets.Classes.Settings;
using UnitedSets.Mvvm.Services;
using WinWrapper;
using WinWrapper.Windowing;
using Icon = WinWrapper.Icon;

namespace UnitedSets.UI.AppWindows;

partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        SetupBasicWindow();
        
        //TransparentMode = FeatureFlags.UseTransparentWindow;
        //if (TransparentMode)
        //    SetupTransparent(out trans_mgr);

        void UpdateBackdrop(USBackdrop x)
        {
            var IsTransparent = x is USBackdrop.Transparent;
            WindowBorderOnTransparent.Visibility = IsTransparent ? Visibility.Visible : Visibility.Collapsed;
            TransparentMode = IsTransparent;
            SystemBackdrop = x.GetSystemBackdrop();
        }
        Settings.BackdropMode.Updated += UpdateBackdrop;
        UpdateBackdrop(Settings.BackdropMode.Value);
        ((OverlappedPresenter)AppWindow.Presenter).SetBorderAndTitleBar(true, true);

        SetupNative(out Win32Window, out WindowMessageMonitor);

        RegisterWindow();

        SetupEvent();

        SetupUIThreadLoopTimer(out timer);

        ApplyFlags();

        //SetupTaskbarMode();

        // Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void RegisterWindow()
        {
            AttachedOutOfBoundsFlyout.RegisterWindow(this);
            TabBase.MainWindows.Add(Win32Window);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SetupBasicWindow()
        {
            Title = "UnitedSets";
            MinWidth = 100;
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(CustomDragRegion);
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //void SetupTransparent(out TransparentWindowManager trans_mgr)
        //{
        //    WindowBorderOnTransparent.Visibility = Visibility.Visible;
        //    MainAreaBorder.Margin = new(8, 0, 8, 8);
        //    var presenter = (OverlappedPresenter)AppWindow.Presenter;
        //    //RootGrid.Children.Insert(0, border);
        //    //trans_mgr = new(this, swapChainPanel, FeatureFlags.EntireWindowDraggable);
        //    //trans_mgr.AfterInitialize();
        //    trans_mgr = null!;
        //}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SetupEvent()
        {
            AppWindow.Closing += OnWindowClosing;
            // Activated += FirstRun;
            SizeChanged += OnMainWindowResize;
            CustomDragRegionUpdator.EffectiveViewportChanged += OnCustomDragRegionUpdatorCalled;
            WindowMessageMonitor.WindowMessageReceived += OnWindowMessageReceived;
            TabBase.OnUpdateStatusLoopComplete += OnDifferentThreadLoop;
            Cell.ValidDrop += CellWindowDropped;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SetupNative(out Window WindowEx, out WindowMessageMonitor WindowMessageMonitor)
        {
            WindowEx = Window.FromWindowHandle(WindowNative.GetWindowHandle(this));
            WindowMessageMonitor = new WindowMessageMonitor(WindowEx);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ApplyFlags()
        {
            static bool IsProcessNotExited(System.Diagnostics.Process proc)
            {
                //try
                //{
                //    return !proc.HasExited;
                //} catch
                //{
                //    return false;
                //}
                return false;
            }
            // --add-window-by-exe
            var toAdd = CLI.GetArrVal("add-window-by-exe");
            var editLastAddedWindow = CLI.GetFlag("edit-last-added");
            //LeftFlyout.NoAutoClose = CLI.GetFlag("edit-no-autoclose");
            foreach (var window in
                from proc in System.Diagnostics.Process.GetProcesses()
                where IsProcessNotExited(proc)
                from itm in toAdd
                where proc.ProcessName.Equals(itm, StringComparison.OrdinalIgnoreCase)
                select Window.FromWindowHandle(proc.MainWindowHandle)
            )
                AddTab(window);

            if (editLastAddedWindow && Tabs.Count > 0)
                Tabs[^1].TabDoubleTapped(
                    this,
                    new DoubleTappedRoutedEventArgs()
                );
        }

        
    }
    [Event(typeof(TypedEventHandler<object, WindowActivatedEventArgs>))]
    void FirstRun()
    {
#if UNPKG
		    var Package = SettingsService.Settings;
#endif
        Activated -= FirstRun;
        var icon = Icon.Load($@"{Package.Current.InstalledLocation.Path}\Assets\UnitedSets.ico");
        Win32Window.SmallIcon = Win32Window.LargeIcon = icon;

        if (Keyboard.IsShiftDown)
            Win32Window.SetAppId($"UnitedSets {Win32Window.Handle}");
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
