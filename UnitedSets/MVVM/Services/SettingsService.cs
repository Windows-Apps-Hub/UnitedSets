using CommunityToolkit.Mvvm.Input;
using UnitedSets.UI.AppWindows;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using UnitedSets.Classes.Settings;
using Cube.UI.Icons;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UnitedSets.Mvvm.Services;

public partial class SettingsService
{
    public SettingsService()
    {
        CreateWindow();
        AllSettings = new Setting[] {
            CloseWindowOnCloseTab,
            TransparentWindowMode
        };
    }
    public IReadOnlyList<Setting> AllSettings { get; }

    public OnOffSetting CloseWindowOnCloseTab { get; } = new(nameof(CloseWindowOnCloseTab))
    {
        Title = "Closing tab closes window",
        Description = "If on, close the window when closing a tab. If off, the window will be detach from United Sets.",
        Icon = FluentSymbol.Delete24,
        DefaultValue = true
    };

    public OnOffSetting TransparentWindowMode { get; } = new(nameof(TransparentWindowMode))
    {
        Title = "Transparent Window Mode",
        Description = "Make the window transparent",
        RequiresRestart = true,
        Icon = FluentSymbol.Window20
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