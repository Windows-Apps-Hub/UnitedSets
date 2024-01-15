using CommunityToolkit.Mvvm.Input;
using UnitedSets.UI.AppWindows;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using Get.XAMLTools.Classes.Settings;
using Get.XAMLTools.Classes.Settings.Boolean;
using Cube.UI.Icons;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Composition;
using Get.XAMLTools.UI;

namespace UnitedSets.Mvvm.Services;

public partial class SettingsService
{
    public SettingsService()
    {
        AllSettings = new Setting[] {
            CloseWindowOnCloseTab,
            BackdropMode
        };
    }
    public IReadOnlyList<Setting> AllSettings { get; }

    public OnOffSetting CloseWindowOnCloseTab { get; } = new(nameof(CloseWindowOnCloseTab))
    {
        Title = "Closing tab closes window",
        Description = "If on, close the window when closing a tab. If off, the window will be detach from United Sets.",
        Icon = SymbolEx.Delete,
        DefaultValue = true
    };

    public SelectSetting<USBackdrop> BackdropMode { get; } = new(nameof(BackdropMode), Enum.GetValues<USBackdrop>())
    {
        Title = "Window Background",
        Description = "Select the Window Background (NOTE: Changing to Transparent requires restart)",
        Icon = SymbolEx.PPSOneLandscape,
        DefaultValue = USBackdrop.Mica
    };

    SettingsWindow? s_window;
    [RelayCommand]
    public void LaunchSettings()
    {
		try {
			s_window?.Activate();
		} catch (COMException) {
			CreateWindow();
			s_window.Activate();
		}

	}

    [MemberNotNull(nameof(s_window))]
	private void CreateWindow() {
		s_window = new(this);
		s_window.Closed += (_, _) => s_window = new(this);
	}
}
public enum USBackdrop
{
    Acrylic,
    Mica,
    Tabbed,
    Transparent
}
static class BackdropHelper
{
    public static SystemBackdrop GetSystemBackdrop(this USBackdrop backdrop)
        => backdrop switch
        {
            USBackdrop.Acrylic => new InfiniteSystemBackdrop<DesktopAcrylicController>(),
            USBackdrop.Mica => new InfiniteSystemBackdrop<MicaController>(),
            USBackdrop.Tabbed => new InfiniteSystemBackdrop<MicaController>(x => x.Kind = MicaKind.BaseAlt),
            //USBackdrop.Transparent => new TransparentBackdrop(),
            _ => throw new ArgumentOutOfRangeException(nameof(backdrop))
        };
}
public class InfiniteSystemBackdrop<T> : SystemBackdrop where T : ISystemBackdropControllerWithTargets, new()
{
    public InfiniteSystemBackdrop() { }
    public InfiniteSystemBackdrop(Action<T> act) { action = act; }
    Action<T>? action;
    public bool IsInfinite { get; set; } = true;
    T? controller;

    protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop connectedTarget, XamlRoot xamlRoot)
    {
        // Call the base method to initialize the default configuration object.
        base.OnTargetConnected(connectedTarget, xamlRoot);

        // This example does not support sharing MicaSystemBackdrop instances.
        if (controller is not null)
        {
            throw new Exception("This controller cannot be shared");
        }

        controller = new T();
        action?.Invoke(controller);
        // Set configuration.
        SystemBackdropConfiguration defaultConfig = GetDefaultSystemBackdropConfiguration(connectedTarget, xamlRoot);
        
        controller.SetSystemBackdropConfiguration(GetConfig(defaultConfig));
        // Add target.
        controller.AddSystemBackdropTarget(connectedTarget);
    }
    SystemBackdropConfiguration GetConfig(SystemBackdropConfiguration a)
        => IsInfinite ? new()
        {
            IsInputActive = IsInfinite,
            IsHighContrast = a.IsHighContrast,
            HighContrastBackgroundColor = a.HighContrastBackgroundColor,
            Theme = a.Theme
        } : a;
    protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop disconnectedTarget)
    {
        base.OnTargetDisconnected(disconnectedTarget);

        controller?.RemoveSystemBackdropTarget(disconnectedTarget);
        controller = default;
    }
    protected override void OnDefaultSystemBackdropConfigurationChanged(ICompositionSupportsSystemBackdrop target, XamlRoot xamlRoot)
    {
        try
        {
            SystemBackdropConfiguration defaultConfig = GetDefaultSystemBackdropConfiguration(target, xamlRoot);
            controller?.SetSystemBackdropConfiguration(GetConfig(defaultConfig));
        } catch { }
    }
}