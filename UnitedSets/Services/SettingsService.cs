using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyCSharp;
using System.Threading;
using Windows.Storage;
using UnitedSets.Windows;
using System.Runtime.InteropServices;
using UnitedSets.Classes;

namespace UnitedSets.Services;

public partial class SettingsService : ObservableObject
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public static USConfig Settings;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public USConfig cfg => Settings;

    SettingsWindow? s_window;
    [RelayCommand]
    public void LaunchSettings(MainWindow mainWindow)
    {
		try {
			if (s_window == null)
				CreateWindow(mainWindow);
			s_window?.Activate();
		} catch (COMException) {
			CreateWindow(mainWindow);
			s_window?.Activate();
		}

	}
	

	private void CreateWindow(MainWindow mainWindow) {
		s_window = new(this,mainWindow) { };
		s_window.Closed += (_, _) => s_window = new(this, mainWindow) { };
	}
}
