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
using System;
using UnitedSets.Windows.Flyout;

namespace UnitedSets.Windows.Flyout.Modules;

public sealed partial class ModifyWindowFlyoutModule : IWindowFlyoutModule
{
    public ModifyWindowFlyoutModule(OurHwndHost hwndHost)
    {
        HwndHost = hwndHost;
        InitializeComponent();
        string CompatablityString = string.Join(", ",
            new string?[]
            {
                hwndHost.IsOwnerSetSuccessful ? null : "No Owner",
                hwndHost.NoMoving ? "No Move" : null
            }.Where(x => x is not null)
        );
        if (string.IsNullOrEmpty(CompatablityString)) CompatablityString = "None";
        CompatabilityModeTB.Text = CompatablityString;
        
        BorderlessWindowSettings.Visibility = hwndHost.NoMoving ? Visibility.Collapsed : Visibility.Visible;
    }
    readonly OurHwndHost HwndHost;

    public event Action? RequestClose;

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
			HwndHost.ClearCrop();
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
        string? FileName = HwndHost.GetOwnerProcessModuleFilename();
        if (FileName is null) return;
    
        Process.Start("explorer.exe", $"/select,\"{FileName}\"");
    }
    [Event(typeof(RoutedEventHandler))]
    async void CloseWindow()
    {
        await HwndHost.Close();
        RequestClose?.Invoke();
    }
    [Event(typeof(RoutedEventHandler))]
    void DetachWindow()
    {
        HwndHost.DetachAndDispose();
        RequestClose?.Invoke();
    }

    public void OnActivated()
    {
        
    }
}
