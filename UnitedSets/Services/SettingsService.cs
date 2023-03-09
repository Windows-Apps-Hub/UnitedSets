using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyCSharp;
using System.Threading;
using Windows.Storage;
using UnitedSets.Windows;
using System.Runtime.InteropServices;

namespace UnitedSets.Services;

public partial class SettingsService : ObservableObject
{
    public SettingsService()
    {
        new Thread(() =>
        {
            while (true)
            {
                if (exitOnClose != ExitOnClose)
                    SetProperty(ref exitOnClose, ExitOnClose);
                Thread.Sleep(2000);
            }
        })
        {
            Name = "United Sets Settings Update Loop"
        }.Start();
		CreateWindow();
    }
#if !UNPKG
private static readonly ApplicationDataContainer Settings = ApplicationData.Current.LocalSettings;
#else
	internal static Classes.FauxSettings Settings = new();

#endif

	[Property(CustomGetExpression = "(bool)(Settings.Values[\"ExitOnClose\"] ?? true)", OnChanged = nameof(ExitOnCloseChanged))]
    private bool exitOnClose = (bool)(Settings.Values["ExitOnClose"] ?? true);
    private void ExitOnCloseChanged() => Settings.Values["ExitOnClose"] = exitOnClose;
    SettingsWindow s_window;
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
	

	private void CreateWindow() {
		s_window = new(this);
		s_window.Closed += (_, _) => s_window = new(this);
	}
}
