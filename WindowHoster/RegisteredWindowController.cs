using Get.EasyCSharp;
using WinWrapper.Windowing;
using Windows.Win32;
using Windows.Win32.Foundation;
using System.Drawing;
using Microsoft.UI.Dispatching;
using System.Threading.Tasks;
using System.Threading;
using WinWrapper.Input;
namespace WindowHoster;

public partial class RegisteredWindowController
{
    internal bool Updating { get; private set; } = false;
    bool isRegistered = true;
    Window Parent;
    RegisteredWindow self;
    [Property(OnChanged = nameof(CointainerRectangleChanged))]
    Windows.Foundation.Rect _ContainerRectangle;
    internal Rectangle LatestRequestedRect { get; private set; }
    internal async void UpdatePosition()
    {
        if (!isRegistered) return;
        if (self.CompatablityMode.NoMoving) return;
        Updating = true;
        var window = self.Window;
        Point windowOffset;
        unsafe
        {
            _ = PInvoke.MapWindowPoints(
                (HWND)Parent.Handle,
                default,
                &windowOffset,
                1
            );
        }
        var rasterizationScale = self.Window.CurrentDisplay.ScaleFactor / 100d;
        var requestedRect = RectToScreen(ContainerRectangle, windowOffset, rasterizationScale);
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
                    currentBounds.Width - cropRegion.Left - cropRegion.Right,
                    currentBounds.Height - cropRegion.Top - cropRegion.Bottom
                ));
            }
        }
        LatestRequestedRect = requestedRect;
        window.Bounds = requestedRect;
        if (queueSetVisible)
        {
            queueSetVisible = false;
            window.IsVisible = true;
            // do not steal focus if we are doing things with other window
            await Task.Delay(100);
            if (!Cursor.IsLeftButtonDown)
                window.SetAsForegroundWindow();
        }
        Updating = false;
    }
    static Rectangle RectToScreen(Windows.Foundation.Rect rect, Point WindowPosOffset, double RasterizationScale)
    {
        return new(
            (int)(rect.X * RasterizationScale + WindowPosOffset.X),
            (int)(rect.Y * RasterizationScale + WindowPosOffset.Y),
            (int)(rect.Width * RasterizationScale),
            (int)(rect.Height * RasterizationScale)
        );
    }
    bool queueSetVisible;
    internal readonly DispatcherQueue DispatcherQueue;
    internal RegisteredWindowController(Window Parent, RegisteredWindow self, DispatcherQueue dispatcherQueue)
    {
        this.Parent = Parent;
        
        this.self = self;
        DispatcherQueue = dispatcherQueue;
        var window = self.Window;
        //window.DwmAttribute.Set(DwmWindowAttribute.DWMWA_CLOAK, 0);
        queueSetVisible = true;
        self.InternalUpdateParent(Parent);
        WinEvents.Register(Parent.Handle, WinEventTypes.PositionSizeChanged, false, ParentPositionChangedHandler);
        thread = new(ThreadLoop);
        thread.Start();
    }
    readonly Thread thread;
    void ParentPositionChangedHandler(WinEventTypes eventType, nint hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
    {
        // we can't update immedietly when the handler is called
        // so we do it in another thread
        queueUpdatePosition = true;
    }
    void CointainerRectangleChanged()
    {
        queueUpdatePosition = true;
    }
    bool queueUpdatePosition = false;
    void ThreadLoop()
    {
        while (isRegistered)
        {
            if (queueUpdatePosition)
            {
                queueUpdatePosition = false;
                UpdatePosition();
            }
            Thread.Sleep(1000 / 60);
        }
    }
    internal void Unregister()
    {
        isRegistered = false;
        var window = self.Window;
        WinEvents.Unregister(Parent.Handle, WinEventTypes.PositionSizeChanged, false, ParentPositionChangedHandler);
        //window.DwmAttribute.Set(DwmWindowAttribute.DWMWA_CLOAK, 1);
        if (self.IsValid)
            window.IsVisible = false;
        if (self.CurrentController == this)
            self.CurrentController = null;
        Parent = default;
        self = null!;
    }
}
