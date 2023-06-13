using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Input;
using EasyXAMLTools;
using OutOfBoundsFlyout.ScreenHelper;
using WinUIEx;

namespace OutOfBoundsFlyout;

[AttachedProperty(typeof(FlyoutContainer), "ContextFlyout", typeof(UIElement), UseNullableReferenceType = true, GenerateLocalOnPropertyChangedMethod = true)]
[AttachedProperty(typeof(FlyoutContainer), "Flyout", typeof(UIElement), UseNullableReferenceType = true, GenerateLocalOnPropertyChangedMethod = true)]
[AttachedProperty(typeof(FlyoutContainer), "DoubleTappedFlyout", typeof(UIElement), UseNullableReferenceType = true, GenerateLocalOnPropertyChangedMethod = true)]
public sealed partial class AttachedOutOfBoundsFlyout : DependencyObject
{
    private AttachedOutOfBoundsFlyout() { }

    readonly static LinkedList<Window> Windows = new();

    public static void RegisterWindow(Window window)
    {
        Windows.AddLast(window);
    }

    static partial void OnFlyoutChanged(UIElement obj, FlyoutContainer? oldValue, FlyoutContainer? newValue)
    {
        if (newValue is null)
            obj.Tapped -= FlyoutShow;
        else if (oldValue is null)
            obj.Tapped += FlyoutShow;
    }

    static partial void OnDoubleTappedFlyoutChanged(UIElement obj, FlyoutContainer? oldValue, FlyoutContainer? newValue)
    {
        if (newValue is null)
            obj.DoubleTapped -= FlyoutShowDoubleTapped;
        else if (oldValue is null)
            obj.DoubleTapped += FlyoutShowDoubleTapped;
    }

    static partial void OnContextFlyoutChanged(UIElement obj, FlyoutContainer? oldValue, FlyoutContainer? newValue)
    {
        if (newValue is null)
            obj.RightTapped -= ContextFlyoutShow;
        else if (oldValue is null)
            obj.RightTapped += ContextFlyoutShow;
    }

    private static async void ContextFlyoutShow(object sender, RightTappedRoutedEventArgs e)
    {
        if (sender is not UIElement element) return;
        var flyout = GetContextFlyout(element);
        if (flyout is null) return;
        {
            foreach (var window in Windows)
            {
                if (window.Content?.XamlRoot != element.XamlRoot) continue;
                var bounds = element.GetBoundsRelativeToScreen(window);
                var scale = window.GetScale();
                var cursorPos = e.GetPosition(element);

                bounds.X += 8;
                bounds.Y += WinWrapper.Windowing.Window
                        .FromWindowHandle(window.GetWindowHandle()).IsMaximized ? 8 : 0;

                await OutOfBoundsFlyoutSystem.ShowAsync(
                    flyout.Flyout!,
                    new((int)(bounds.X + cursorPos._x * scale), (int)(bounds.Y + cursorPos._y * scale)),
                    e.PointerDeviceType is not (PointerDeviceType.Touchpad or PointerDeviceType.Mouse)
                );
                break;
            }
        }
    }

    private static void FlyoutShow(object sender, TappedRoutedEventArgs e)
    {
        if (sender is not UIElement element) return;
        var flyout = GetFlyout(element);
        if (flyout is null) return;
        foreach (var window in Windows)
        {
            if (window.Content?.XamlRoot != element.XamlRoot) continue;
            var bounds = element.GetBoundsRelativeToScreen(window);
            var scale = window.GetScale();
            var cursorPos = e.GetPosition(element);

            bounds.X += 8;
            bounds.Y += WinWrapper.Windowing.Window
                    .FromWindowHandle(window.GetWindowHandle()).IsResizable ? 8 : 0;

            _ = OutOfBoundsFlyoutSystem.ShowAsync(
                flyout.Flyout!,
                new((int)(bounds.X + cursorPos._x * scale), (int)(bounds.Y + cursorPos._y * scale)),
                e.PointerDeviceType is not (PointerDeviceType.Touchpad or PointerDeviceType.Mouse),
                FlyoutPlacementMode.Bottom,
                ExclusionRect: new(bounds.X, bounds.Y, bounds.Width, bounds.Height)
            );
            break;
        }
    }

    private static void FlyoutShowDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (sender is not UIElement element) return;
        var flyout = GetFlyout(element);
        if (flyout is null) return;
        foreach (var window in Windows)
        {
            if (window.Content?.XamlRoot != element.XamlRoot) continue;
            var bounds = element.GetBoundsRelativeToScreen(window);
            var scale = window.GetScale();
            var cursorPos = e.GetPosition(element);

            bounds.X += 8;
            bounds.Y += WinWrapper.Windowing.Window
                    .FromWindowHandle(window.GetWindowHandle()).IsResizable ? 8 : 0;

            _ = OutOfBoundsFlyoutSystem.ShowAsync(
                flyout.Flyout!,
                new((int)(bounds.X + cursorPos._x * scale), (int)(bounds.Y + cursorPos._y * scale)),
                e.PointerDeviceType is not (PointerDeviceType.Touchpad or PointerDeviceType.Mouse),
                FlyoutPlacementMode.Bottom,
                ExclusionRect: new(bounds.X, bounds.Y, bounds.Width, bounds.Height)
            );
            break;
        }
    }

    public static void ShowFlyout(UIElement element, FlyoutBase flyout, global::Windows.Foundation.Point pt, bool TouchPenCompatable)
    {
        foreach (var window in Windows)
        {
            if (window.Content?.XamlRoot != element.XamlRoot) continue;
            var bounds = element.GetBoundsRelativeToScreen(window);
            var scale = window.GetScale();

            bounds.X += 8;
            bounds.Y += WinWrapper.Windowing.Window
                    .FromWindowHandle(window.GetWindowHandle()).IsResizable ? 8 : 0;

            _ = OutOfBoundsFlyoutSystem.ShowAsync(
                flyout,
                new((int)(bounds.X + bounds.Width / 2), (int)(bounds.Y + bounds.Height / 2)),
                TouchPenCompatable,
                FlyoutPlacementMode.Bottom,
                ExclusionRect: new(bounds.X, bounds.Y, bounds.Width, bounds.Height)
            );
            break;
        }
    }
}
