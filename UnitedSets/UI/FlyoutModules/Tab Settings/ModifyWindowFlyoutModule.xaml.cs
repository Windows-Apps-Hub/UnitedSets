using Get.EasyCSharp;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using Process = System.Diagnostics.Process;
using WindowHoster;

namespace UnitedSets.UI.FlyoutModules;

public sealed partial class ModifyWindowFlyoutModule
{
    public ModifyWindowFlyoutModule(RegisteredWindow window)
    {
        RegisteredWindow = window;
        InitializeComponent();
        string CompatablityString = string.Join(", ",
            new string?[]
            {
                window.CompatablityMode.NoOwner ? "No Owner" : null,
                window.CompatablityMode.NoMoving ? "No Move" : null
            }.Where(x => x is not null)
        );
        if (string.IsNullOrEmpty(CompatablityString)) CompatablityString = "None";
        CompatabilityModeTB.Text = CompatablityString;
        
        BorderlessWindowSettings.Visibility = window.CompatablityMode.NoMoving ? Visibility.Collapsed : Visibility.Visible;
    }
    readonly RegisteredWindow RegisteredWindow;

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
			RegisteredWindow.Properties.CropRegion = default;
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
        string? FileName = GetOwnerProcessModuleFilename();
        if (FileName is null) return;
    
        Process.Start("explorer.exe", $"/select,\"{FileName}\"");
    }
    string? GetOwnerProcessModuleFilename()
    {
        var host = RegisteredWindow;
        var FileName = host.Window.OwnerProcess.GetDotNetProcess.MainModule?.FileName;
        if (FileName == @"C:\WINDOWS\system32\ApplicationFrameHost.exe")
        {
            var child = host.Window.Children.AsEnumerable().FirstOrDefault(x =>
                x.Class.Name is "Windows.UI.Core.CoreWindow", host.Window);
            FileName = child.OwnerProcess.GetDotNetProcess.MainModule?.FileName;
        }
        return FileName;
    }

    [Event(typeof(RoutedEventHandler))]
    async void CloseWindow()
    {
        await RegisteredWindow.Window.TryCloseAsync();
    }
    [Event(typeof(RoutedEventHandler))]
    void DetachWindow()
    {
        RegisteredWindow.Detach();
    }
    double ToDouble(int x) => x;
    void CropLeftBindBack(double x)
    {
        RegisteredWindow.Properties.CropRegion = RegisteredWindow.Properties.CropRegion with { Left = (int)x };
    }
    void CropRightBindBack(double x)
    {
        RegisteredWindow.Properties.CropRegion = RegisteredWindow.Properties.CropRegion with { Right = (int)x };
    }
    void CropTopBindBack(double x)
    {
        RegisteredWindow.Properties.CropRegion = RegisteredWindow.Properties.CropRegion with { Top = (int)x };
    }
    void CropBottomBindBack(double x)
    {
        RegisteredWindow.Properties.CropRegion = RegisteredWindow.Properties.CropRegion with { Bottom = (int)x };
    }
}
