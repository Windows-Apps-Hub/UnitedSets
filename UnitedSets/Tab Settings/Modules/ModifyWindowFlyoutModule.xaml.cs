using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UnitedSets.Classes;

namespace UnitedSets;

public sealed partial class ModifyWindowFlyoutModule
{
    public ModifyWindowFlyoutModule(HwndHost hwndHost)
    {
        HwndHost = hwndHost;
        InitializeComponent();
        WindowCropMarginToggleSwitch_Toggled(null, null);
        BorderlessToggleSwitch_Toggled(null, null);
    }
    readonly HwndHost HwndHost;

    private void TopMarginShortcutClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            TopCropMargin.Value = double.Parse(btn.Content.ToString() ?? "0");
        }
    }

    private void TopCropMargin_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        HwndHost.CropTop = (int)TopCropMargin.Value;
    }

    private void LeftCropMargin_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        HwndHost.CropLeft = (int)LeftCropMargin.Value;
    }

    private void RightCropMargin_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        HwndHost.CropRight = (int)RightCropMargin.Value;
    }

    private void BottomCropMargin_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        HwndHost.CropBottom = (int)BottomCropMargin.Value;
    }

    private void WindowCropMarginToggleSwitch_Toggled(object? sender, RoutedEventArgs? e)
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

    private void BorderlessToggleSwitch_Toggled(object? sender, RoutedEventArgs? e)
    {
        HwndHost.BorderlessWindow = BorderlessToggleSwitch.IsOn;
        if (!BorderlessToggleSwitch.IsOn)
            WindowCropMarginToggleSwitch.IsOn = false;
        BorderlessSettingsStackPanel.Visibility = BorderlessToggleSwitch.IsOn ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ResetClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is NumberBox nbb)
        {
            nbb.Value = 0;
        }
    }

    private void DrawClick(object sender, RoutedEventArgs e)
    {
        HwndHost.HostedWindow.Redraw();
    }
}
