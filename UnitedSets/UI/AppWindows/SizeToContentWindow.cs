using Get.EasyCSharp;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Windows.Foundation;
using WinUIEx;
using System.ComponentModel;
using UnitedSets.UI.Controls;
using WinWrapper;
using Win32Window = WinWrapper.Windowing.Window;

namespace UnitedSets.UI.AppWindows;

[ContentProperty(Name = nameof(SizeToWindowContent))]
public partial class SizeToContentWindow : WindowEx
{
    [Property(OnChanged = nameof(SizeToWindowContentChanged))]
    UIElement? _SizeToWindowContent;
    void SizeToWindowContentChanged() => Panel.Content = _SizeToWindowContent;
    SizeChangedDetectorPanel Panel = new();
    readonly Win32Window Win32Window;
    public SizeToContentWindow()
    {
        Content = Panel;
        Panel.SizeUpdated += SizeUpdate;
        Win32Window = Win32Window.FromWindowHandle(this.GetWindowHandle());
    }
    Size cacheSize;
    void SizeUpdate(Size size)
    {
        //size.Width += 30;
        //size.Height += 10;
        cacheSize = size;
        UpdateBounds();
    }
    [Property(OnChanged = nameof(UpdateBounds))]
    Point _AnchorPoint;

    void UpdateBounds()
    {
        if (cacheSize == default)
        {
            Panel.InvalidateMeasure();
        }
        var scale = HwndExtensions.GetDpiForWindow(this.GetWindowHandle()) / 96f;
        Win32Window.Bounds = new(
            x: (int)(_AnchorPoint.X - (cacheSize.Width + 6) * scale),
            y: (int)_AnchorPoint.Y,
            width: (int)(cacheSize.Width * scale),
            height: (int)(cacheSize.Height * scale)
        );
    }
}
