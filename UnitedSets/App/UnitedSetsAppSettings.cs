using CommunityToolkit.Mvvm.Input;
using UnitedSets.UI.AppWindows;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using UnitedSets.Settings;
using System.Collections.Generic;
using System;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Composition;
using Get.XAMLTools.UI;
using UnitedSets.Classes;
using EnumsNET;

namespace UnitedSets.Mvvm.Services;

public partial class UnitedSetsAppSettings
{
    private static USConfig Configuration => UnitedSetsApp.Current.Configuration.PersistantService.MainConfiguration;
    public UnitedSetsAppSettings()
    {
        AllSettings = [
            Autosave = new(
                () => Configuration.Autosave ?? true, x => Configuration.Autosave = x
            ) {
                Title = "Autosave Settings",
                Description = "Automatically saves the settings as you edit them. Turn this off if you only want the current session to have this setting.",
                Icon = SymbolEx.Save
            },
            CloseWindowOnCloseTab = new(
                () => Configuration.CloseWindowOnCloseTab, x => Configuration.CloseWindowOnCloseTab = x
            )
            {
                Title = "Closing tab closes window",
                Description = "If on, close the window when closing a tab. If off, the window will be detach from United Sets.",
                Icon = SymbolEx.Delete
            },
            //TransculentWindow = new(
            //    () => cfg.Design.UseTranslucentWindow ?? false, x => cfg.Design.UseTranslucentWindow = x
            //)
            //{
            //    Title = "Use Translucent Bordering/Background",
            //    Description = "Allow the opacity of our background/borders (not the pinned apps) to be translucent",
            //    Icon = SymbolEx.PPSOneLandscape,
            //    RequiresRestart = true
            //},
            BypassMinimumSize = new(() => Configuration.BypassMinSize, x => Configuration.BypassMinSize = x) {
                Title = "Bypass Minimum Size",
                Description = $"Allows resizing the window down to {
                    Constants.BypassMinWidth}x{Constants.BypassMinHeight
                    } (Normal minimum size is {
                    Constants.MinWidth}x{Constants.MinHeight})",
                Icon = SymbolEx.ResizeMouseSmall
            },
            BackdropMode = new(
                () => Configuration.Design!.Backdrop, x => Configuration.Design!.Backdrop = x, Enums.GetValues<USBackdrop>()
            ) {
                Title = "Window Background",
                Description = "Select the Window Background (NOTE: Changing to Transparent requires restart)",
                Icon = SymbolEx.Color
            },
            WindowTitlePrefix = new(
                () => Configuration.TitlePrefix ?? "", x => Configuration.TitlePrefix = x
            )
            {
                Title = "Window Title Prefix",
                Description = "Prefix that shows up before the normal UnitedSets title",
                Icon = SymbolEx.AlignLeft,
                PlaceholderText = "None - Normal Title Mode"
            },
            Theme = new(
                () => Configuration.Design!.Theme ?? ElementTheme.Default, x => Configuration.Design!.Theme = x,
                Enums.GetValues<ElementTheme>()
            )
            {
                Title = "Theme Override",
                Description = "Override the windows theme",
                Icon = SymbolEx.Light,
                RequiresRestart = true
            }
        ];
        foreach (var setting in AllSettings)
            setting.PropertyChanged += delegate
            {
                if (Autosave.Value)
                {
                    UnitedSetsApp.Current.Configuration.SaveCurSettingsAsDefault();
                }
            };
    }
    public IReadOnlyList<Setting> AllSettings { get; }

    public OnOffSetting CloseWindowOnCloseTab { get; }
    public OnOffSetting Autosave { get; }
    public OnOffSetting BypassMinimumSize { get; }
    //public OnOffSetting TransculentWindow { get; }
    public TextSetting WindowTitlePrefix { get; }
    public SelectSetting<ElementTheme> Theme { get; }

    public SelectSetting<USBackdrop> BackdropMode { get; }

    SettingsWindow? s_window;
    [RelayCommand]
    public void LaunchSettings(MainWindow mainWindow)
    {
        try
        {
			if (s_window is not null)
            {
                s_window.Activate();
                return;
            }
        } catch (COMException) {
		}
        CreateWindow(mainWindow);
        s_window.Activate();
	}

    [MemberNotNull(nameof(s_window))]
	private void CreateWindow(MainWindow mainWindow) {
		s_window = new(this, mainWindow);
		s_window.Closed += (_, _) => s_window = new(this, mainWindow);
	}
}
public enum USBackdrop
{
    Mica,
    Acrylic,
    Tabbed,
    //Transparent
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
        if (target == null)
        	return;
        try
        {
            SystemBackdropConfiguration defaultConfig = GetDefaultSystemBackdropConfiguration(target, xamlRoot);
            controller?.SetSystemBackdropConfiguration(GetConfig(defaultConfig));
        } catch { }
    }
}
