using Microsoft.UI.Xaml.Markup;
using Windows.Foundation;
using WinUIEx;
using UnitedSets.UI.Controls;
using Win32Window = WinWrapper.Windowing.Window;
using Microsoft.UI.Xaml;
using System;

namespace Get.XAMLTools.UI.AppWindows;

[ContentProperty(Name = nameof(SizeToContentContent))]
[DependencyProperty<UIElement>("SizeToContentContent", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<Point>("AnchorPoint", GenerateLocalOnPropertyChangedMethod = true, LocalOnPropertyChangedMethodName = nameof(UpdateBounds), LocalOnPropertyChangedMethodWithParameter = false)]
[DependencyProperty<Point>("WindowScaledOffsetPoint", GenerateLocalOnPropertyChangedMethod = true, LocalOnPropertyChangedMethodName = nameof(UpdateBounds), LocalOnPropertyChangedMethodWithParameter = false)]
[DependencyProperty<Point>("WindowOffsetPoint", GenerateLocalOnPropertyChangedMethod = true, LocalOnPropertyChangedMethodName = nameof(UpdateBounds), LocalOnPropertyChangedMethodWithParameter = false)]
[DependencyProperty<HorizontalAnchorModes>("HorizontalAnchorMode", GenerateLocalOnPropertyChangedMethod = true, LocalOnPropertyChangedMethodName = nameof(UpdateBounds), LocalOnPropertyChangedMethodWithParameter = false)]
[DependencyProperty<VerticalAnchorModes>("VerticalAnchorMode", GenerateLocalOnPropertyChangedMethod = true, LocalOnPropertyChangedMethodName = nameof(UpdateBounds), LocalOnPropertyChangedMethodWithParameter = false)]
public partial class SizeToContentWindow : DependencyObject
{
    readonly WindowEx Window = new();
    public WindowEx UnsafeGetWindow() => Window;
    readonly SizeChangedDetectorPanel Panel = new();
    readonly Win32Window Win32Window;
    public SizeToContentWindow()
    {
        Window.WindowContent = Panel;
        Panel.SizeUpdated += SizeUpdate;
        Win32Window = Win32Window.FromWindowHandle(Window.GetWindowHandle());
    }
    Size cacheSize;
    void SizeUpdate(Size size)
    {
        cacheSize = size;
        UpdateBounds();
    }
    partial void OnSizeToContentContentChanged(UIElement oldValue, UIElement newValue)
        => Panel.Content = newValue;
    void UpdateBounds()
    {
        if (cacheSize == default)
        {
            Panel.InvalidateMeasure();
        }
        var AnchorPoint = this.AnchorPoint;
        var scale = HwndExtensions.GetDpiForWindow(Win32Window) / 96f;
        var OffsetPoint = new Point(
            x: WindowOffsetPoint.X + WindowScaledOffsetPoint.X * scale,
            y: WindowOffsetPoint.Y + WindowScaledOffsetPoint.Y * scale
        );
        Win32Window.Bounds = new(
            x: HorizontalAnchorMode switch {
                HorizontalAnchorModes.Left => (int)(AnchorPoint.X + OffsetPoint.X),
                HorizontalAnchorModes.Center => (int)(AnchorPoint.X - cacheSize.Width / 2 * scale + OffsetPoint.X),
                HorizontalAnchorModes.Right => (int)(AnchorPoint.X - cacheSize.Width * scale + OffsetPoint.X),
                _ => throw new ArgumentException($"{nameof(HorizontalAnchorMode)} has an invalid input")
            },
            y: VerticalAnchorMode switch
            {
                VerticalAnchorModes.Top => (int)(AnchorPoint.Y + OffsetPoint.Y),
                VerticalAnchorModes.Center => (int)(AnchorPoint.Y - cacheSize.Height * scale / 2 + OffsetPoint.Y),
                VerticalAnchorModes.Bottom => (int)(AnchorPoint.Y - cacheSize.Height * scale + OffsetPoint.Y),
                _ => throw new ArgumentException($"{nameof(VerticalAnchorMode)} has an invalid input")
            },
            width: (int)(cacheSize.Width * scale),
            height: (int)(cacheSize.Height * scale)
        );
    }
}
public enum HorizontalAnchorModes : byte
{
    Left,
    Center,
    Right
}
public enum VerticalAnchorModes : byte
{
    Top,
    Center,
    Bottom
}