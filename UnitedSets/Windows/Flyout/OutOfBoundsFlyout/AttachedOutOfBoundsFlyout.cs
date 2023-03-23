using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyXAMLTools;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Input;
using System.Windows.Forms;
using UnitedSets.Windows.Flyout.OutOfBoundsFlyout;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace UnitedSets.OutOfBoundsFlyout;
[ContentProperty(Name = nameof(Flyout))]
public class FlyoutContainer
{
    public FlyoutBase? Flyout { get; set; }
}
[AttachedProperty(typeof(FlyoutContainer), "ContextFlyout", typeof(DependencyObject), UseNullableReferenceType = true, GenerateLocalOnPropertyChangedMethod = true)]
[AttachedProperty(typeof(FlyoutContainer), "Flyout", typeof(UIElement), UseNullableReferenceType = true, GenerateLocalOnPropertyChangedMethod = true)]
public partial class AttachedOutOfBoundsFlyout : DependencyObject
{
    static LinkedList<Window> Windows = new();

    public static void RegisterWindow(Window window)
    {
        Windows.AddLast(window);
    }

    static partial void OnContextFlyoutChanged(DependencyObject obj, FlyoutContainer? oldValue, FlyoutContainer? newValue)
    {
        if (obj is not UIElement obj2) return;
        //if (newValue is null)
        //    obj2.PointerReleased -= ContextFlyoutPointerReleased;
        //else if (oldValue is null)
        //    obj2.PointerReleased += ContextFlyoutPointerReleased;
        obj2.RightTapped += ContextFlyoutShow;
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
                await OutOfBoundsFlyoutSystem.ShowAsync(
                    flyout.Flyout!,
                    new((int)(bounds.X + cursorPos._x * scale), (int)(bounds.Y + cursorPos._y * scale)),
                    e.PointerDeviceType is not (PointerDeviceType.Touchpad or PointerDeviceType.Mouse)
                );
                break;
            }
        }
    }

    //private static async void ContextFlyoutPointerReleased(object sender, PointerRoutedEventArgs e)
    //{
    //    if (sender is not UIElement element) return;
    //    var flyout = GetContextFlyout(element);
    //    if (flyout is null) return;
    //    if (e.GetCurrentPoint(element).Properties.PointerUpdateKind is PointerUpdateKind.RightButtonReleased)
    //    {
    //        foreach (var window in Windows)
    //        {
    //            if (window.Content?.XamlRoot != element.XamlRoot) continue;
    //            var bounds = element.GetBoundsRelativeToScreen(window);
    //            var scale = window.GetScale();
    //            var cursorPos = e.GetCurrentPoint(element).Position;
    //            await OutOfBoundsFlyoutSystem.ShowAsync(
    //                flyout.Flyout!,
    //                new((int)(bounds.X + cursorPos._x * scale), (int)(bounds.Y + cursorPos._y * scale)),
    //                e.Pointer.PointerDeviceType is not PointerDeviceType.Touchpad or PointerDeviceType.Mouse
    //            );
    //            break;
    //        }
    //    }
    //}

    static partial void OnFlyoutChanged(UIElement obj, FlyoutContainer? oldValue, FlyoutContainer? newValue)
    {
        if (newValue is null)
            obj.Tapped -= FlyoutShow;
        else if (oldValue is null)
            obj.Tapped += FlyoutShow;
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
}
