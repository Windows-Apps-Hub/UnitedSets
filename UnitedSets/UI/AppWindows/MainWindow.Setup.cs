using System.Runtime.CompilerServices;
using Cube.UI.Icons;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using UnitedSets.Apps;
using UnitedSets.Cells.Data;
using UnitedSets.Configurations;
using UnitedSets.Mvvm.Services;
using UnitedSets.Tabs;
using WindowHoster;
using WinWrapper.Windowing;
using Icon = WinWrapper.Icon;
using Keyboard = WinWrapper.Input.Keyboard;

namespace UnitedSets.UI.AppWindows;

partial class MainWindow : NativeHelperWindow
{
    public MainWindow()
    {
        UnitedSetsApp.Current.RegisterUnitedSetsWindow(this);

        Init();

        SetupBasicWindow();
        
        ((OverlappedPresenter)AppWindow.Presenter).SetBorderAndTitleBar(true, true);


        SetupCustomization();

        SetupEvent();
        SetupUIThreadLoopTimer(out timer);

        // SetupTaskbarMode();

        // Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SetupBasicWindow()
        {
            Title = "UnitedSets";
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.PreferredHeightOption = 
                UnitedSetsApp.Current.Settings.Layout.Value is UnitedSetsLayouts.HorizontalTabs ?
                    TitleBarHeightOption.Tall : TitleBarHeightOption.Standard;
            //SetTitleBar(CustomDragRegion);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SetupEvent()
        {
            ((FrameworkElement)Content).FirstLoadedEv(() =>
            {
                LeftInset = GridLengthFromPixelInt(AppWindow.TitleBar.LeftInset);
                RightInset = GridLengthFromPixelInt(AppWindow.TitleBar.RightInset);
            });
            AppWindow.Closing += OnWindowClosing;
            ClosingFlyout.CloseRequest += RequestCloseAsync;
            Activated += FirstRun;
            TabBase.OnUpdateStatusLoopComplete += OnDifferentThreadLoop;
            EmptyCell.ValidDrop += OnDropOverCell;
        }
        //ShellHookMessage = (WindowMessages)PInvoke.RegisterWindowMessage("SHELLHOOK");
        //PInvoke.RegisterShellHookWindow(new(Win32Window.Handle));
        RegisteredWindow.ShouldWindowBeDetachOnUserMove =
            _ => UnitedSetsApp.Current.Settings.UserMoveWindowBehavior.Value is UserMoveWindowBehaviors.DetachWindow;
    }

    WindowMessages ShellHookMessage;


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
        UnitedSetsApp.Current.HandleCLICmds();
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
