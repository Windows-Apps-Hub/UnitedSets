using Get.EasyCSharp;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using Process = System.Diagnostics.Process;
using WindowHoster;
using System;
using WinUIEx;
using Microsoft.Win32;
using System.IO;
using UnitedSets.Helpers;
using WindowEx = WinWrapper.Windowing.Window;
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
    string? GetOwnerProcessModuleFilename() => GetOwnerWindow().OwnerProcess.GetDotNetProcess.MainModule?.FileName;
    /// <summary>
    /// Work around WinUI/UWP as AppFrameHost is normally the owner but we want the actual app
    /// </summary>
    /// <returns></returns>
    WindowEx GetOwnerWindow(out bool wasUwp)
    {
        var owner = RegisteredWindow.Window;
        wasUwp = false;
        var mainModulePath = owner.OwnerProcess.GetDotNetProcess.MainModule?.FileName ?? "";
        if (mainModulePath?.Equals(System.IO.Path.Combine(Environment.SystemDirectory, "ApplicationFrameHost.exe"), StringComparison.CurrentCultureIgnoreCase) != true)
        {
            wasUwp = mainModulePath!.Contains(WindowsAppFolder ?? LoadWindowsAppFolder(), StringComparison.CurrentCultureIgnoreCase);//some windows apps dont use appframehost, IE windows terminal

            return owner;
        }
        wasUwp = true;
        var child = GetCoreWindowFromAppHostWindow(RegisteredWindow.Window);
        return child;
    }
    public static WindowEx GetCoreWindowFromAppHostWindow(WindowEx appFrameHostMainWindow) => appFrameHostMainWindow.Children.FirstOrDefault(x => x.Class.Name is "Windows.UI.Core.CoreWindow", appFrameHostMainWindow);
    private static string? WindowsAppFolder = null;
    private static string LoadWindowsAppFolder()
    {
        if (WindowsAppFolder == null)
        {
            using var appx = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Appx");
            WindowsAppFolder = appx?.GetValue("PackageRoot")?.ToString() ?? "invalid";
        }
        return WindowsAppFolder;
    }
    public const string OUR_WINDOWS_STORE_APP_EXEC_PREFIX = "#WindowsApp!";
    WindowEx GetOwnerWindow() => GetOwnerWindow(out _);
    public (string cmd, string args) GetOwnerProcessInfo()
    {
        var owner = GetOwnerWindow(out var wasUWP);
        var toParse = ExternalProcessHelper.GetProcessCommandLineByPID(owner.OwnerProcess.Id.ToString());
        var parsed = ExternalProcessHelper.ParseCmdLine(toParse!);
        // So to get theofficial UWP executable we should use mainModulePath, as the command line can be different for example "wt" will launch windows terminal and will show that as its process. Now we could always use mainModulePath but as we can relaunch terminal with the same command we simply can honor what it says it is.  We must disable UWP mode though in this case so if we are not under the App Folder we can assume not uwp.
        if (wasUWP && parsed.filename.Replace("\\", "/").Contains((WindowsAppFolder ?? LoadWindowsAppFolder()).Replace("\\", "/")) == false)
            wasUWP = false;
        if (wasUWP)
        {
            var fileInfo = new FileInfo(parsed.filename);
            var dir = fileInfo?.Directory?.Name ?? "";
            parsed.filename = $"{OUR_WINDOWS_STORE_APP_EXEC_PREFIX}{dir}";
            var serverNamePos = parsed.args.IndexOf("-ServerName:", StringComparison.CurrentCultureIgnoreCase);
            if (serverNamePos != -1)
            {
                var spaceAfter = parsed.args.IndexOf(" ", serverNamePos + 1);
                var part1 = parsed.args.Substring(0, serverNamePos);
                var part2 = "";
                if (spaceAfter != -1)
                    part2 = parsed.args.Substring(spaceAfter + 1);//should have a space before servername anyway
                parsed.args = part1 + part2;
            }
        }
        return parsed;
    }
    [Event(typeof(RoutedEventHandler))]
    async void CloseWindow()
    {
        await RegisteredWindow.Window.TryCloseAsync();
    }
    [Event(typeof(RoutedEventHandler))]
    async void DetachWindow()
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
