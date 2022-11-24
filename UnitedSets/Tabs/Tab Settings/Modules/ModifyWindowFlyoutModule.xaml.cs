using EasyCSharp;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UnitedSets.Classes;
using Windows.Foundation;

namespace UnitedSets;

public sealed partial class ModifyWindowFlyoutModule
{
    public ModifyWindowFlyoutModule(HwndHost hwndHost)
    {
        HwndHost = hwndHost;
        InitializeComponent();
        OnWindowCropMarginToggleSwitchToggled(null, null);
        OnBorderlessToggleSwitchToggled(null, null);
    }
    readonly HwndHost HwndHost;

    [Event(typeof(RoutedEventHandler))]
    void TopMarginShortcutClick(object sender)
    {
        if (sender is Button btn)
        {
            TopCropMargin.Value = double.Parse(btn.Content.ToString() ?? "0");
        }
    }

    [Event(typeof(TypedEventHandler<NumberBox, NumberBoxValueChangedEventArgs>))]
    void OnTopCropMarginChanged()
        => HwndHost.CropTop = (int)TopCropMargin.Value;

    [Event(typeof(TypedEventHandler<NumberBox, NumberBoxValueChangedEventArgs>))]
    void OnLeftCropMarginChanged()
        => HwndHost.CropLeft = (int)LeftCropMargin.Value;

    [Event(typeof(TypedEventHandler<NumberBox, NumberBoxValueChangedEventArgs>))]
    void OnRightCropMarginChanged()
        => HwndHost.CropRight = (int)RightCropMargin.Value;

    [Event(typeof(TypedEventHandler<NumberBox, NumberBoxValueChangedEventArgs>))]
    void OnBottomCropMarginChanged()
        => HwndHost.CropBottom = (int)BottomCropMargin.Value;

    [Event(typeof(RoutedEventHandler))]
    void OnWindowCropMarginToggleSwitchToggled()
    {
        if (!WindowCropMarginToggleSwitch.IsOn)
        {
            TopCropMargin.Value =
            BottomCropMargin.Value =
            LeftCropMargin.Value =
            RightCropMargin.Value = 0;
        }
        HwndHost.ActivateCrop = WindowCropMarginToggleSwitch.IsOn;
        WindowCropMarginSettingsStackPanel.Visibility = WindowCropMarginToggleSwitch.IsOn ? Visibility.Visible : Visibility.Collapsed;
    }

    [Event(typeof(RoutedEventHandler))]
    void OnBorderlessToggleSwitchToggled()
    {
        HwndHost.BorderlessWindow = BorderlessToggleSwitch.IsOn;
        if (!BorderlessToggleSwitch.IsOn)
            WindowCropMarginToggleSwitch.IsOn = false;
        BorderlessSettingsStackPanel.Visibility = BorderlessToggleSwitch.IsOn ? Visibility.Visible : Visibility.Collapsed;
    }

#pragma warning disable CA1822 // Mark members as static
    [Event(typeof(RoutedEventHandler))]
    void OnResetClick(object sender)
    {
        if (sender is Button btn && btn.Tag is NumberBox nbb)
        {
            nbb.Value = 0;
        }
    }
#pragma warning restore CA1822 // Mark members as static

    [Event(typeof(RoutedEventHandler))]
    void OnRedrawClick()
        => HwndHost.HostedWindow.Redraw();
}
