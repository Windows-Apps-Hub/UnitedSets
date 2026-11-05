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
    /// <summary>
    /// Raised (on the hook thread) when a hosted window gets minimized or maximized by itself,
    /// e.g. using its own minimize/maximize button, Win+Down or Win+Up. The hosted window is
    /// restored automatically; the handler typically minimizes/maximizes the host window instead.
    /// </summary>
    public static event Action<RegisteredWindow, HostedWindowStateRequest>? HostedWindowStateRequested;
    internal readonly WindowStylingState InitalStylingState;
    public Window Window { get; }
    [AutoNotifyProperty(SetVisibility = GeneratorVisibility.Private, OnChanged = nameof(CompatablityModeChanged))]
    CompatablityMode _CompatablityMode;

    private RegisteredWindow(Window WindowToHost, bool shouldBeHidden = false)
    {
        if (WindowToHost.IsMaximizedReliable() || WindowToHost.IsMinimizedReliable())
            WindowToHost.SendMessage(WindowMessages.SysCommand, /* SC_RESTORE */ 0xF120, 0);

        InitalStylingState = WindowStylingState.GetCurrentState(WindowToHost);

        Window = WindowToHost;

        CompatablityMode = CompatablityMode with { NoMoving = NoMovingModeChecker(WindowToHost) };

        var skipOwnProcess = Window.OwnerProcess.Id == Process.Current.Id;
        registeredWindowClosedEvent = WinEvents.Register(
            Window.Handle,
            WinEventTypes.ObjectDestroyed,
            skipOwnProcess,
            (_, _, idObject, idChild, _, _) =>
            {
                // EVENT_OBJECT_DESTROY is also raised when the caret/cursor/scrollbar of the window
                // is destroyed. Treating that as the window being closed made tabs disappear while
                // the (hidden) window was still alive.
                if (!WinEvents.IsWindowItself(idObject, idChild)) return;
                if (!IsValid) return;
                IsValid = false;
                Dispose();
                Closed?.Invoke();
            }
        );
        if (!CompatablityMode.NoMoving)
        {
            registeredPosSizeChangedEvent = WinEvents.Register(
                Window.Handle,
                WinEventTypes.PositionSizeChanged,
                skipOwnProcess,
                (_, _, idObject, idChild, _, _) =>
                {
                    if (!WinEvents.IsWindowItself(idObject, idChild)) return;
                    OnPositionChangedExternally();
                }
            );
            registeredMoveSizeStartEvent = WinEvents.Register(
                Window.Handle,
                WinEventTypes.WindowMovedStart,
                skipOwnProcess,
                (_, _, idObject, idChild, _, _) =>
                {
                    if (!WinEvents.IsWindowItself(idObject, idChild)) return;
                    if (CurrentController is null) return;
                    userMoveSizeStartBounds = Window.Bounds;
                    IsUserMovingOrResizing = true;
                }
            );
            registeredMoveSizeEndEvent = WinEvents.Register(
                Window.Handle,
                WinEventTypes.WindowMovedEnd,
                skipOwnProcess,
                (_, _, idObject, idChild, _, _) =>
                {
                    if (!WinEvents.IsWindowItself(idObject, idChild)) return;
                    if (!IsUserMovingOrResizing) return;
                    IsUserMovingOrResizing = false;
                    // the user resized the window by its border (or dragged it while detaching is disabled):
                    // put it back to where it belongs
                    CurrentController?.QueueUpdatePosition(resetCorrections: true);
                }
            );
        }
        registeredWindowShownEvent = WinEvents.Register(
            Window.Handle,
            WinEventTypes.WindowShown,
            skipOwnProcess,
            (_, _, idObject, idChild, _, _) =>
            {
                // also raised when the caret is shown
                if (!WinEvents.IsWindowItself(idObject, idChild)) return;
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
    WinEventsRegistrationParameters registeredWindowClosedEvent, registeredPosSizeChangedEvent,
        registeredWindowShownEvent, registeredMoveSizeStartEvent, registeredMoveSizeEndEvent;
    System.Drawing.Rectangle userMoveSizeStartBounds;
    /// <summary>
    /// Whether the user is currently in the move/size modal loop of the window (dragging its title bar or border)
    /// </summary>
    public bool IsUserMovingOrResizing { get; private set; }

    void OnPositionChangedExternally()
    {
        if (!IsValid) return;
        if (CurrentController is not { } controller) return;
        if (controller.Updating)
            // current controller is doing the update
            return;
        var bounds = Window.Bounds;
        if (bounds == controller.LatestRequestedRect)
            // same position
            return;
        if (Window.IsMinimizedReliable() || Window.IsMaximizedReliable())
        {
            // The hosted window was minimized/maximized directly (e.g. its own buttons, Win+Down/Up).
            // A hosted window can't do that on its own, so restore it and let the host decide.
            var request = Window.IsMinimizedReliable() ? HostedWindowStateRequest.Minimize : HostedWindowStateRequest.Maximize;
            Window.SendMessage(WindowMessages.SysCommand, /* SC_RESTORE */ 0xF120, 0);
            controller.QueueUpdatePosition(resetCorrections: true);
            HostedWindowStateRequested?.Invoke(this, request);
            return;
        }
        if (IsUserMovingOrResizing)
        {
            // Only detach when the user drags the window (title bar), not when they resize it by the border
            var start = userMoveSizeStartBounds;
            var isMove = bounds.Size == start.Size && bounds.Location != start.Location;
            if (isMove && ShouldWindowBeDetachOnUserMove(Window))
            {
                IsUserMovingOrResizing = false;
                controller.DispatcherQueue.TryEnqueue(Detach);
            }
            // otherwise wait for the user to finish (move size end will put it back)
            return;
        }
        // The window moved or resized itself (or another program moved it): put it back.
        controller.OnHostedWindowChangedItself();
    }

    void CompatablityModeChanged()
    {
        if (CompatablityMode.NoOwner)
        {
            var window = Window;
            window[WindowExStyles.TOOLWINDOW] = true;
        }
    }
    internal RegisteredWindowController? CurrentController;
    /// <summary>
    /// The last measured invisible resize borders (see <see cref="RegisteredWindowController"/>)
    /// </summary>
    internal InvisibleBorders? CachedInvisibleBorders;
    internal void InternalUpdateParent(Window parent)
    {
        var window = Window;
        // A tabbed window has no taskbar button of its own: the host represents it.
        // (WS_EX_APPWINDOW forces a button even for owned windows. The original extended styles are
        // restored when the window is detached.)
        if (window[WindowExStyles.APPWINDOW])
            window[WindowExStyles.APPWINDOW] = false;
        window.Owner = parent;
        CompatablityMode = CompatablityMode with { NoOwner = Window.Owner != parent };
    }
    bool disposed;
    private void Dispose()
    {
        if (disposed) return;
        disposed = true;
        registeredWindowClosedEvent.Unregister();
        registeredPosSizeChangedEvent.Unregister();
        registeredWindowShownEvent.Unregister();
        registeredMoveSizeStartEvent.Unregister();
        registeredMoveSizeEndEvent.Unregister();
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
        if (!IsValid) return;
        var WindowToHost = Window;
        // stop following the host before restoring anything so the controller
        // doesn't move the window back while we are detaching it
        CurrentController?.Unregister(hideWindow: false);
        Dispose();
        try
        {
            WindowToHost.Region = InitalStylingState.Region;
            Properties.ActivateCrop = false;
            Properties.BorderlessWindow = false;
            WindowToHost.Owner = default;
            WindowToHost.Style = InitalStylingState.Style;
            // NoOwner compatibility mode adds WS_EX_TOOLWINDOW which hides the window from the taskbar
            // and Alt+Tab. Always restore the original extended styles.
            WindowToHost.ExStyle = InitalStylingState.ExStyles;
            WindowToHost.IsResizable = InitalStylingState.IsResizable;
            WindowToHost.IsVisible = true;

            WindowToHost.Focus();
            WindowToHost.Redraw();
            WindowToHost.SetAsForegroundWindow();
        }
        catch (Exception ex)
        {
            // the window may have been closed in the mean time
            Debug.WriteLine($"[RegisteredWindow] Detach failed: {ex}");
        }
        await Task.Delay(100);
        try
        {
            WindowToHost.Redraw();
            WindowToHost.IsVisible = true;
        }
        catch { }
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
        if (!IsValid) return null;
        if (CurrentController is not null)
            throw new InvalidOperationException("You already have a controller. Please unregister that one to get a new controller");
        CurrentController = new(parentWindow, this, dispatcherQueue);
        return CurrentController;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    /// <summary>
    /// Registers a new window.
    /// </summary>
    /// <returns>A <see cref="RegisteredWindow"/> if the window is not part of the blacklist. Returns <c>null</c> otherwise.</returns>
    public static RegisteredWindow? Register(Window window, bool shouldBeHidden = false)
    {
        if (!window.IsValid) return null;
        if (BlacklistChecker(window)) return null;
        return new(window, shouldBeHidden);
    }
}
internal readonly record struct InvisibleBorders(int Left, int Top, int Right, int Bottom);
public enum HostedWindowStateRequest
{
    Minimize,
    Maximize
}
