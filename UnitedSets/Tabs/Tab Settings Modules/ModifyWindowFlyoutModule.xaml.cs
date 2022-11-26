using EasyCSharp;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using System.Linq;
using UnitedSets.Classes;
using Windows.Foundation;
using WinWrapper;
using Process = System.Diagnostics.Process;
using WinUI3HwndHostPlus;
namespace UnitedSets;

public sealed partial class ModifyWindowFlyoutModule
{
    public ModifyWindowFlyoutModule(HwndHost hwndHost)
    {
        HwndHost = hwndHost;
        InitializeComponent();
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
        WindowCropMarginSettingsStackPanel.Visibility = WindowCropMarginToggleSwitch.IsOn ? Visibility.Visible : Visibility.Collapsed;
    }

    [Event(typeof(RoutedEventHandler))]
    void OnBorderlessToggleSwitchToggled()
    {
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
    void OpenWindowLocation()
    {
        string? FileName = HwndHost.HostedWindow.OwnerProcess.GetDotNetProcess.MainModule?.FileName;
        if (FileName is null) return;
        if (FileName is @"C:\WINDOWS\system32\ApplicationFrameHost.exe")
        {
            var child = HwndHost.HostedWindow.Children.FirstOrDefault(x =>
                x.ClassName is "Windows.UI.Core.CoreWindow", HwndHost.HostedWindow);
            FileName = child.OwnerProcess.GetDotNetProcess.MainModule?.FileName;
            if (FileName is null) return;
        }
        Process.Start("explorer.exe", $"/select,\"{FileName}\"");
    }
}
