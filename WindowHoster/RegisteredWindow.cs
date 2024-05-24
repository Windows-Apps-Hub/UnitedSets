using Get.EasyCSharp;
using WinWrapper.Windowing;
using System;
using System.ComponentModel;
using Microsoft.UI.Dispatching;
using WinWrapper;
using System.Threading.Tasks;
using System.Diagnostics;
using Process = WinWrapper.Process;
namespace WindowHoster;

public partial class RegisteredWindow : INotifyPropertyChanged
{
    public static Func<Window, bool>
        NoMovingModeChecker = (window) => window.Class.Name is "RAIL_WINDOW";
    public static Func<Window, bool> BlacklistChecker = (window)
        => window.Class.Name is
            "Shell_TrayWnd" // Taskbar
            or "Progman" or "WorkerW" // Desktop
            or "WindowsDashboard" // I forget
            or "Windows.UI.Core.CoreWindow" // Quick Settings and Notification Center (other uwp apps should already be ApplicationFrameHost)
        ;
    internal readonly WindowStylingState InitalStylingState;
    public Window Window { get; }
    [AutoNotifyProperty(SetVisibility = GeneratorVisibility.Private, OnChanged = nameof(CompatablityModeChanged))]
    CompatablityMode _CompatablityMode;

    private RegisteredWindow(Window WindowToHost, bool shouldBeHidden = false)
    {
        if (WindowToHost.IsMaximized)
            WindowToHost.SendMessage(WindowMessages.SysCommand, /* SC_RESTORE */ 0xF120, 0);

        InitalStylingState = WindowStylingState.GetCurrentState(WindowToHost);

        //this.XAMLWindow = XAMLWindow;

        //this.UIDispatcher = XAMLWindow.DispatcherQueue;//caching incase our window dies


        //var WinUIHandle = WinRT.Interop.WindowNative.GetWindowHandle(XAMLWindow);
        //_ParentWindow = WindowEx.FromWindowHandle(WinUIHandle);
        //WinUIAppWindow = AppWindow.GetFromWindowId(
        //    Microsoft.UI.Win32Interop.GetWindowIdFromWindow(
        //        WinUIHandle
        //    )
        //);

        Window = WindowToHost;

        CompatablityMode = CompatablityMode with { NoMoving = NoMovingModeChecker(WindowToHost) };


        //WinUIAppWindow.Changed += WinUIAppWindowChanged;
        //SizeChanged += WinUIAppWindowChanged;

        //VisiblePropertyChangedToken = RegisterPropertyChangedCallback(VisibilityProperty, OnPropChanged);
        //AddHwndHost(this);

        registeredWindowClosedEvent = WinEvents.Register(
            Window.Handle,
            WinEventTypes.ObjectDestroyed,
            Window.OwnerProcess.Id == Process.Current.Id,
            delegate { Closed?.Invoke(); }
        );
        if (!CompatablityMode.NoMoving)
            registeredPosSizeChangedEvent = WinEvents.Register(
                Window.Handle,
                WinEventTypes.PositionSizeChanged,
                Window.OwnerProcess.Id == Process.Current.Id,
                delegate
                {
                    if (CurrentController is null) return;
                    if (Window.Bounds != CurrentController.LatestRequestedRect)
                    {
                        Detach();
                    }
                }
            );
        if (shouldBeHidden && IsValid)
            WindowToHost.IsVisible = false;
        Closed += delegate { BecomesInvalid?.Invoke(); };
        Detached += delegate { BecomesInvalid?.Invoke(); };
        Properties = new(this);
    }
    WinEventsRegistrationParameters registeredWindowClosedEvent, registeredPosSizeChangedEvent;
    void CompatablityModeChanged()
    {
        if (CompatablityMode.NoOwner)
        {
            var window = Window;
            window[WindowExStyles.TOOLWINDOW] = true;
        }
    }
    internal RegisteredWindowController? CurrentController;
    DateTime TimeWhenGettingController = default;
    public RegisteredWindowController? GetController(Window parentWindow, DispatcherQueue dispatcherQueue)
    {
        if (CurrentController is not null)
            throw new InvalidOperationException("You already have a controller. Please unregister that one to get a new controller");
        CurrentController = new(parentWindow, this, dispatcherQueue);
        TimeWhenGettingController = DateTime.UtcNow;
        return CurrentController;
    }
    public static RegisteredWindow? Register(Window window, bool shouldBeHidden = false)
    {
        if (BlacklistChecker(window)) return null;
        return new(window, shouldBeHidden);
    }

    public bool IsValid { get; set; } = true;

    public event PropertyChangedEventHandler? PropertyChanged;
    internal void InternalUpdateParent(Window parent)
    {
        if (parent == default)
            // replace with some window
            parent = default;
        var window = Window;
        window.Owner = parent;
        CompatablityMode = CompatablityMode with { NoOwner = Window.Owner != parent };
    }
    public event Action? Detached;
    public event Action? Closed;
    public event Action? BecomesInvalid;
    public async void Detach() => await DetachAsync();
    public async Task DetachAsync()
    {
        
        var WindowToHost = Window;
        WindowToHost.Region = InitalStylingState.Region;
        Properties.ActivateCrop = false;
        Properties.BorderlessWindow = false;
        IsValid = false;
        WindowToHost.Owner = default;
        WindowToHost.IsResizable = InitalStylingState.IsResizable;
        WindowToHost.IsVisible = true;
        Dispose();

        WindowToHost.Focus();
        WindowToHost.Redraw();
        WindowToHost.SetAsForegroundWindow();
        await Task.Delay(100).ContinueWith(_ =>
        {
            WindowToHost.Redraw();
            WindowToHost.IsVisible = true;
        });
        Detached?.Invoke();
    }
    private void Dispose()
    {
        registeredWindowClosedEvent.Unregister();
        registeredPosSizeChangedEvent.Unregister();
        IsValid = false;
    }
}
