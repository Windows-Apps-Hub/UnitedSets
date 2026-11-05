using Get.EasyCSharp;
using WinWrapper.Windowing;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;
using System;
using System.Drawing;
using Microsoft.UI.Dispatching;
using System.Threading;
using WinWrapper.Input;
namespace WindowHoster;

/// <summary>
/// Keeps a <see cref="RegisteredWindow"/> positioned over a rectangle of its host window.
/// </summary>
/// <remarks>
/// Positioning happens on a dedicated background thread so that a hung hosted application
/// (cross-process <c>SetWindowPos</c> may block) never freezes the host UI.
/// </remarks>
public partial class RegisteredWindowController
{
    /// <summary>
    /// How many times in a row the controller puts back a window that moves/resizes itself
    /// before giving up (until the host area changes). Prevents fighting with applications
    /// that snap their size (e.g. console windows) or enforce a minimum size.
    /// </summary>
    const int MaxConsecutiveCorrections = 3;
    internal bool Updating { get; private set; } = false;
    volatile bool isRegistered = true;
    readonly Window Parent;
    readonly RegisteredWindow self;
    readonly AutoResetEvent updateSignal = new(false);
    readonly WinEventsRegistrationParameters parentPositionChangedRegistration;
    [Property(OnChanged = nameof(CointainerRectangleChanged))]
    Windows.Foundation.Rect _ContainerRectangle;
    /// <summary>
    /// The scale used to convert <see cref="ContainerRectangle"/> (in DIPs) into physical pixels.
    /// Should be the host's <c>XamlRoot.RasterizationScale</c>. If 0, the scale of the display
    /// the host window is on is used.
    /// </summary>
    public double RasterizationScale { get; set; }
    internal Rectangle LatestRequestedRect { get; private set; }
    int consecutiveCorrections;
    /// <summary>
    /// <see cref="Environment.TickCount64"/> of the last time the host window moved/resized
    /// </summary>
    long lastParentMove;
    /// <summary>
    /// When to do a final "settle" update after the host stopped moving (0 = none)
    /// </summary>
    long settleAt;
    /// <summary>
    /// The host window is being moved/resized. While this is the case, a hosted window that resizes itself
    /// (e.g. because it moved to a monitor with a different DPI) is not corrected immediately:
    /// fighting with it at every frame is what made the window look "jelly". A final update is done
    /// once the host stops moving.
    /// </summary>
    internal bool ParentMovedRecently => Environment.TickCount64 - Interlocked.Read(ref lastParentMove) < 250;

    static readonly System.Collections.Concurrent.ConcurrentDictionary<RegisteredWindowController, byte> Active = new();

    /// <summary>
    /// Position of the hosted window relative to the host window rectangle, as of the last full update
    /// </summary>
    System.Drawing.Point relativeToHost;
    bool hasRelativeToHost;

    /// <summary>
    /// Call from the host window procedure when it moves or is about to move (<c>WM_MOVING</c>,
    /// <c>WM_WINDOWPOSCHANGED</c>) with its new window position (screen pixels, like <c>GetWindowRect</c>).
    /// The hosted windows are moved right away, together with the host. They used to follow only after a
    /// WinEvent round trip, which made them trail behind (the "jelly" effect), especially across monitors.
    /// Calling it several times with the same position does nothing.
    /// </summary>
    public static void OnHostMoved(nint hostHwnd, int hostX, int hostY)
    {
        foreach (var (controller, _) in Active)
        {
            if (controller.Parent.Handle != hostHwnd || !controller.isRegistered || !controller.hasRelativeToHost) continue;
            var self = controller.self;
            if (!self.IsValid || self.CompatablityMode.NoMoving) continue;
            var rect = controller.LatestRequestedRect;
            if (rect.Width <= 0 || rect.Height <= 0) continue;
            var target = new System.Drawing.Point(hostX + controller.relativeToHost.X, hostY + controller.relativeToHost.Y);
            if (rect.Location == target) continue;
            rect.Location = target;
            controller.LatestRequestedRect = rect;
            Interlocked.Exchange(ref controller.lastParentMove, Environment.TickCount64);
            // Moved by the positioning thread right away (without waiting for the WinEvent round trip).
            // Not with SWP_ASYNCWINDOWPOS: that runs SetWindowPos on the hosted application's thread, in its DPI
            // awareness context, and DPI unaware windows were then placed ~1/1.5 off on a 150% monitor.
            lock (controller.moveLock)
                controller.moveTarget = target;
            Interlocked.Exchange(ref controller.moveRequested, 1);
            controller.updateSignal.Set();
        }
    }

    readonly object moveLock = new();
    System.Drawing.Point moveTarget;
    int moveRequested, fullUpdateRequested;

    void DoMove()
    {
        if (!isRegistered || !self.IsValid || self.CompatablityMode.NoMoving) return;
        System.Drawing.Point target;
        lock (moveLock) target = moveTarget;
        Updating = true;
        try
        {
            PInvoke.SetWindowPos((HWND)self.Window.Handle, HWND.Null, target.X, target.Y, 0, 0,
                Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOSIZE |
                Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOZORDER |
                Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);
        }
        finally
        {
            Updating = false;
        }
    }

    /// <summary>
    /// The hosted window moved or resized itself: put it back, unless the host is moving (see <see cref="ParentMovedRecently"/>).
    /// </summary>
    internal void OnHostedWindowChangedItself()
    {
        if (ParentMovedRecently)
        {
            ScheduleSettle();
            return;
        }
        QueueUpdatePosition(resetCorrections: false);
    }

    void ScheduleSettle()
    {
        Interlocked.Exchange(ref settleAt, Environment.TickCount64 + 300);
        // wake the thread so it starts waiting with the new timeout
        updateSignal.Set();
    }

    /// <summary>
    /// Requests the window position to be updated (asynchronously).
    /// </summary>
    internal void UpdatePosition() => QueueUpdatePosition(resetCorrections: true);

    /// <param name="resetCorrections">
    /// <c>true</c> if this update is caused by the host (container moved/resized, user action),
    /// <c>false</c> if it is a correction because the hosted window moved itself.
    /// </param>
    internal void QueueUpdatePosition(bool resetCorrections)
    {
        if (!isRegistered) return;
        if (resetCorrections)
            consecutiveCorrections = 0;
        else if (Interlocked.Increment(ref consecutiveCorrections) > MaxConsecutiveCorrections)
            return;
        Interlocked.Exchange(ref fullUpdateRequested, 1);
        updateSignal.Set();
    }

    void DoUpdatePosition()
    {
        if (!isRegistered || !self.IsValid) return;
        if (self.CompatablityMode.NoMoving) return;
        Updating = true;
        try
        {
            var window = self.Window;
            if (queueSetVisible)
            {
                queueSetVisible = false;
                // compute the position first so the window doesn't flash at its old location
                SetBounds(window);
                window.IsVisible = true;
                // the invisible borders can only be measured reliably once the window is visible
                SetBounds(window);
                // do not steal focus if we are doing things with other window
                Thread.Sleep(100);
                if (isRegistered && !Cursor.IsLeftButtonDown)
                    window.SetAsForegroundWindow();
            }
            else
            {
                SetBounds(window);
            }
        }
        finally
        {
            Updating = false;
        }
    }

    void SetBounds(Window window)
    {
        System.Drawing.Point windowOffset = default;
        unsafe
        {
            _ = PInvoke.MapWindowPoints(
                (HWND)Parent.Handle,
                default,
                &windowOffset,
                1
            );
        }
        var scale = RasterizationScale > 0 ? RasterizationScale : Parent.CurrentDisplay.ScaleFactor / 100d;
        var requestedRect = RectToScreen(ContainerRectangle, windowOffset, scale);
        if (requestedRect.Width <= 0 || requestedRect.Height <= 0)
            // the host is not laid out yet
            return;
        if (self.Properties.ActivateCrop)
        {
            var cropRegion = self.Properties.CropRegion;
            requestedRect = requestedRect with
            {
                X = requestedRect.X - cropRegion.Left,
                Y = requestedRect.Y - cropRegion.Top,
                Width = requestedRect.Width + cropRegion.Left + cropRegion.Right,
                Height = requestedRect.Height + cropRegion.Top + cropRegion.Bottom
            };
            var currentBounds = window.Bounds;
            if (self.Properties.ForceInvalidateCrop || currentBounds != requestedRect)
            {
                self.Properties.ForceInvalidateCrop = false;
                _ = window.SetRegionAsync(new(
                    cropRegion.Left,
                    cropRegion.Top,
                    requestedRect.Width - cropRegion.Left - cropRegion.Right,
                    requestedRect.Height - cropRegion.Top - cropRegion.Bottom
                ));
            }
        }
        else
        {
            var insets = GetInvisibleBorders(window);
            var compensated = Compensate(requestedRect, insets);
            LatestRequestedRect = compensated;
            RememberRelativePosition(compensated);
            if (window.Bounds != compensated)
                window.Bounds = compensated;
            // The borders can change with the size/style (and DWM may report stale values right after
            // the window is shown): measure again and correct once if needed.
            if (MeasureInvisibleBorders(window) is { } measured && measured != insets)
            {
                self.CachedInvisibleBorders = measured;
                compensated = Compensate(requestedRect, measured);
                LatestRequestedRect = compensated;
                RememberRelativePosition(compensated);
                window.Bounds = compensated;
            }
            return;
        }
        LatestRequestedRect = requestedRect;
        RememberRelativePosition(requestedRect);
        if (window.Bounds != requestedRect)
            window.Bounds = requestedRect;
    }

    void RememberRelativePosition(Rectangle hostedRect)
    {
        if (!PInvoke.GetWindowRect((HWND)Parent.Handle, out var host)) return;
        relativeToHost = new(hostedRect.X - host.left, hostedRect.Y - host.top);
        hasRelativeToHost = true;
    }

    static Rectangle Compensate(Rectangle target, InvisibleBorders b) => new(
        target.X - b.Left,
        target.Y - b.Top,
        target.Width + b.Left + b.Right,
        target.Height + b.Top + b.Bottom
    );

    /// <summary>
    /// Since Windows 10, normal windows have invisible resize borders (usually ~7px on the left, right and bottom)
    /// that are part of the window rectangle. Without compensating, hosted windows have visible gaps.
    /// </summary>
    InvisibleBorders GetInvisibleBorders(Window window)
    {
        if (MeasureInvisibleBorders(window) is { } measured)
            self.CachedInvisibleBorders = measured;
        return self.CachedInvisibleBorders ?? default;
    }

    /// <summary>
    /// Measures the invisible borders. Returns <c>null</c> when the window is hidden or minimized
    /// (DWM reports stale values then).
    /// </summary>
    static unsafe InvisibleBorders? MeasureInvisibleBorders(Window window)
    {
        var hwnd = (HWND)window.Handle;
        if (!PInvoke.IsWindowVisible(hwnd) || PInvoke.IsIconic(hwnd)) return null;
        RECT frame;
        if (PInvoke.DwmGetWindowAttribute(
                hwnd,
                DWMWINDOWATTRIBUTE.DWMWA_EXTENDED_FRAME_BOUNDS,
                &frame,
                (uint)sizeof(RECT)
            ).Failed)
            return null;
        if (!PInvoke.GetWindowRect(hwnd, out var rect))
            return null;
        var borders = new InvisibleBorders(
            frame.left - rect.left,
            frame.top - rect.top,
            rect.right - frame.right,
            rect.bottom - frame.bottom
        );
        // sanity check: the frame must be inside the window rectangle and the borders must be small
        const int MaxBorder = 64;
        if (borders.Left is < 0 or > MaxBorder || borders.Top is < 0 or > MaxBorder ||
            borders.Right is < 0 or > MaxBorder || borders.Bottom is < 0 or > MaxBorder)
            return null;
        return borders;
    }

    static Rectangle RectToScreen(Windows.Foundation.Rect rect, System.Drawing.Point WindowPosOffset, double RasterizationScale)
    {
        return new(
            (int)Math.Round(rect.X * RasterizationScale + WindowPosOffset.X),
            (int)Math.Round(rect.Y * RasterizationScale + WindowPosOffset.Y),
            (int)Math.Round(rect.Width * RasterizationScale),
            (int)Math.Round(rect.Height * RasterizationScale)
        );
    }
    volatile bool queueSetVisible;
    internal readonly DispatcherQueue DispatcherQueue;
    internal RegisteredWindowController(Window Parent, RegisteredWindow self, DispatcherQueue dispatcherQueue)
    {
        this.Parent = Parent;
        this.self = self;
        DispatcherQueue = dispatcherQueue;
        queueSetVisible = true;
        self.InternalUpdateParent(Parent);
        parentPositionChangedRegistration = WinEvents.Register(Parent.Handle, WinEventTypes.PositionSizeChanged, false, ParentPositionChangedHandler);
        thread = new(ThreadLoop)
        {
            IsBackground = true,
            Name = "United Sets Window Positioning"
        };
        Active[this] = 0;
        thread.Start();
    }
    readonly Thread thread;
    void ParentPositionChangedHandler(WinEventTypes eventType, nint hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
    {
        if (!WinEvents.IsWindowItself(idObject, idChild)) return;
        Interlocked.Exchange(ref lastParentMove, Environment.TickCount64);
        // one last update once the host stops moving (DPI changes, windows that resized themselves...)
        Interlocked.Exchange(ref settleAt, Environment.TickCount64 + 300);
        // we can't update immedietly when the handler is called
        // so we do it in another thread
        QueueUpdatePosition(resetCorrections: true);
    }
    void CointainerRectangleChanged()
    {
        QueueUpdatePosition(resetCorrections: true);
    }
    void ThreadLoop()
    {
        while (true)
        {
            var settle = Interlocked.Read(ref settleAt);
            var timeout = settle == 0 ? Timeout.Infinite : (int)Math.Max(0, settle - Environment.TickCount64);
            var signaled = updateSignal.WaitOne(timeout);
            if (!isRegistered) break;
            var full = Interlocked.Exchange(ref fullUpdateRequested, 0) == 1;
            if (!signaled)
            {
                // the host stopped moving: final update
                Interlocked.Exchange(ref settleAt, 0);
                consecutiveCorrections = 0;
                full = true;
            }
            try
            {
                if (Interlocked.Exchange(ref moveRequested, 0) == 1)
                    DoMove();
                if (full)
                    DoUpdatePosition();
            }
            catch (Exception ex)
            {
                // an exception here would otherwise crash the whole process
                System.Diagnostics.Debug.WriteLine($"[RegisteredWindowController] update failed: {ex}");
            }
        }
    }
    /// <summary>
    /// Stops controlling the window. Safe to call multiple times.
    /// </summary>
    /// <param name="hideWindow">Whether to hide the window (when switching away from the tab)</param>
    internal void Unregister(bool hideWindow = true)
    {
        if (!isRegistered) return;
        isRegistered = false;
        Active.TryRemove(this, out _);
        // wake the thread so it can exit
        updateSignal.Set();
        parentPositionChangedRegistration.Unregister();
        if (hideWindow && self.IsValid)
        {
            var window = self.Window;
            window.IsVisible = false;
        }
        if (self.CurrentController == this)
            self.CurrentController = null;
    }
}
