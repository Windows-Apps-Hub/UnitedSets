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
    /// <summary>
    /// Sets the global compatability mode check for No Moving mode
    /// </summary>
    public static Func<Window, bool>
        NoMovingModeChecker
    { get; set; } = (window) => window.Class.Name is "RAIL_WINDOW";
    /// <summary>
    /// Sets the global blacklist checker
    /// </summary>
    public static Func<Window, bool> BlacklistChecker { get; set; } = (window)
        => window.Class.Name is
            "Shell_TrayWnd" // Taskbar
            or "Progman" or "WorkerW" // Desktop
            or "WindowsDashboard" // I forget
            or "Windows.UI.Core.CoreWindow" // Quick Settings and Notification Center (other uwp apps should already be ApplicationFrameHost)
        ;
    /// <summary>
    /// Checks if the window should be detach
    /// </summary>
    public static Func<Window, bool>
        ShouldWindowBeDetachOnUserMove
    { get; set; } = (window) => true;
    internal readonly WindowStylingState InitalStylingState;
    public Window Window { get; }
    [AutoNotifyProperty(SetVisibility = GeneratorVisibility.Private, OnChanged = nameof(CompatablityModeChanged))]
    CompatablityMode _CompatablityMode;

    private RegisteredWindow(Window WindowToHost, bool shouldBeHidden = false)
    {
        if (WindowToHost.IsMaximized)
            WindowToHost.SendMessage(WindowMessages.SysCommand, /* SC_RESTORE */ 0xF120, 0);

        InitalStylingState = WindowStylingState.GetCurrentState(WindowToHost);

        Window = WindowToHost;

        CompatablityMode = CompatablityMode with { NoMoving = NoMovingModeChecker(WindowToHost) };

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
                async delegate
                {
                    if (CurrentController is not { } controller) return;
                    if (controller.Updating)
                        // current controller is doing the update
                        return;
                    var cachedLatestRequestedRect = controller.LatestRequestedRect;
                    if (Window.Bounds == cachedLatestRequestedRect)
                        // same position
                        return;
                    if (ShouldWindowBeDetachOnUserMove(Window))
                        // wait for 200 millisecond, this is basically to ensure that the user
                        // has actually initiated the action and not some automated program
                        // that wants to take the window and ended up returning it back to the same positon
                        await Task.Delay(200).ContinueWith(x =>
                        {
                            if (Window.Bounds == cachedLatestRequestedRect)
                                // same position or current controller is doing the update
                                return;
                            if (ShouldWindowBeDetachOnUserMove(Window))
                                controller.DispatcherQueue.TryEnqueue(delegate
                                {
                                    Detach();
                                });
                        });
                }
            );
        registeredWindowShownEvent = WinEvents.Register(
            Window.Handle,
            WinEventTypes.WindowShown,
            Window.OwnerProcess.Id == Process.Current.Id,
            delegate
            {
                if (CurrentController is null || !CurrentController.Updating)
                {
                    ShownByUser?.Invoke();
                }
            }
        );
        if (shouldBeHidden && IsValid)
            WindowToHost.IsVisible = false;
        Closed += delegate { BecomesInvalid?.Invoke(); };
        Detached += delegate { BecomesInvalid?.Invoke(); };
        Properties = new(this);
    }
    WinEventsRegistrationParameters registeredWindowClosedEvent, registeredPosSizeChangedEvent, registeredWindowShownEvent;
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
    internal void InternalUpdateParent(Window parent)
    {
        if (parent == default)
            // replace with some window
            parent = default;
        var window = Window;
        window.Owner = parent;
        CompatablityMode = CompatablityMode with { NoOwner = Window.Owner != parent };
    }
    private void Dispose()
    {
        registeredWindowClosedEvent.Unregister();
        registeredPosSizeChangedEvent.Unregister();
        IsValid = false;
    }
    /// <summary>
    /// Gets whether <see cref="RegisteredWindow"/> is still valid. <see cref="RegisteredWindow"/> is valid if the window
    /// is not detached or closed.
    /// </summary>
    public bool IsValid { get; internal set; } = true;
    /// <summary>
    /// Raises when the window becomes detached. This may cause by <see cref="Detach"/> or <see cref="DetachAsync"/> call.
    /// It may also caused by the user moving the window out of position or if the host moved too fast.
    /// </summary>
    public event Action? Detached;
    /// <summary>
    /// Raises when the window is closed.
    /// </summary>
    public event Action? Closed;
    /// <summary>
    /// Raises when the window becomes invalid.
    /// </summary>
    public event Action? BecomesInvalid;
    /// <summary>
    /// Raises when the window becomes visible either by user or programmatic action outside
    /// RegisteredWindow control.
    /// </summary>
    public event Action? ShownByUser;
    /// <inheritdoc cref="DetachAsync"/>
    public async void Detach() => await DetachAsync();
    /// <summary>
    /// Release control of the current window and reset all customizations. Do not close the window.
    /// </summary>
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

    /// <summary>
    /// Creates a new <see cref="RegisteredWindowController"/>. Only one <see cref="RegisteredWindowController"/> may exists at a time per window.
    /// </summary>
    /// <param name="parentWindow">The parent window.</param>
    /// <param name="dispatcherQueue">The dispatcher queue. Used for defining event callback thread.</param>
    /// <returns>A new registered window controller</returns>
    /// <exception cref="InvalidOperationException">If <see cref="RegisteredWindowController"/> for the current window already exists and has not been unregistered.</exception>
    public RegisteredWindowController? GetController(Window parentWindow, DispatcherQueue dispatcherQueue)
    {
        if (CurrentController is not null)
            throw new InvalidOperationException("You already have a controller. Please unregister that one to get a new controller");
        CurrentController = new(parentWindow, this, dispatcherQueue);
        TimeWhenGettingController = DateTime.UtcNow;
        return CurrentController;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    /// <summary>
    /// Registers a new window.
    /// </summary>
    /// <returns>A <see cref="RegisteredWindow"/> if the window is not part of the blacklist. Returns <c>null</c> otherwise.</returns>
    public static RegisteredWindow? Register(Window window, bool shouldBeHidden = false)
    {
        if (BlacklistChecker(window)) return null;
        return new(window, shouldBeHidden);
    }
}
