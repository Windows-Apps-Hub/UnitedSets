using Microsoft.UI.Xaml;
using System.Drawing;
using WinUIEx;
using Window = WinWrapper.Windowing.Window;
using WinUIPoint = Windows.Foundation.Point;


namespace OutOfBoundsFlyout.ScreenHelper;

public static partial class Extension
{
    public static float GetScale(this Microsoft.UI.Xaml.Window elementOwnerWindow)
        => Window.FromWindowHandle(elementOwnerWindow.GetWindowHandle()).GetScale();
    public static float GetScale(this Window elementOwnerWindowEx)
        => elementOwnerWindowEx.CurrentDisplay.ScaleFactor / 100.0f;
    public static RectangleF GetBoundsRelativeToScreen(this UIElement Element, Microsoft.UI.Xaml.Window elementOwnerWindow)
        => Element.GetBoundsRelativeToScreen(elementOwnerWindow, Window.FromWindowHandle(elementOwnerWindow.GetWindowHandle()));
    public static RectangleF GetBoundsRelativeToScreen(this UIElement Element, Microsoft.UI.Xaml.Window elementOwnerWindow, Window elementOwnerWindowEX)
    {
        var bounds = Element.GetBoundsRelativeToWindow(elementOwnerWindow, elementOwnerWindowEX);
        var windowBounds = elementOwnerWindowEX.Bounds;
        bounds.X += windowBounds.X;
        bounds.Y += windowBounds.Y;
        return bounds;
    }
    public static RectangleF GetBoundsRelativeToWindow(this UIElement Element, Microsoft.UI.Xaml.Window elementOwnerWindow)
        => Element.GetBoundsRelativeToWindow(elementOwnerWindow, Window.FromWindowHandle(elementOwnerWindow.GetWindowHandle()));
    public static RectangleF GetBoundsRelativeToWindow(this UIElement Element, Microsoft.UI.Xaml.Window elementOwnerWindow, Window elementOwnerWindowEX)
    {
        var Pt = Element.TransformToVisual(elementOwnerWindow.Content).TransformPoint(
            new WinUIPoint(0, 0)
        );

        var scale = elementOwnerWindowEX.CurrentDisplay.ScaleFactor / 100.0f;
        var size = Element.ActualSize;
        return new(Pt._x * scale, Pt._y * scale, size.X * scale, size.Y * scale);
    }
}
