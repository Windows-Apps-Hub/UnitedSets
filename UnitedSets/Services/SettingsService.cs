using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyCSharp;
using Microsoft.UI.Xaml;
using System.Threading;
using Windows.Storage;
using UnitedSets.Windows;
using System.Collections.Generic;
using System.Reflection;

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
				if (tempState != TempState)
					SetProperty(ref tempState, TempState);
				Thread.Sleep(2000);
            }
        })
        {
            Name = "United Sets Settings Update Loop"
        }.Start();
        s_window = new(this);
        s_window.Closed += (_, _) => s_window = new(this);
    }
#if !UNPKG
private static readonly ApplicationDataContainer Settings = ApplicationData.Current.LocalSettings;
#else
	public static FauxSettings Settings = new();

	public class FauxSettings {
		public FauxSettings Current => this;
		public FauxSettings InstalledLocation => this;
		public string Path => System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

		public class FauxValues {
			public Dictionary<string, object> dict = new();
			public object this[string key] {
				get {
					if (dict.TryGetValue(key, out var value))
						return value;
					dict.Add(key, null);
					return null;
				}
				set { dict[key] = value; }
			}
		}
		public FauxValues Values = new();
	}

#endif

	[Property(CustomGetExpression = "(bool)(Settings.Values[\"ExitOnClose\"] ?? true)", OnChanged = nameof(ExitOnCloseChanged))]
    private bool exitOnClose = (bool)(Settings.Values["ExitOnClose"] ?? true);
    private void ExitOnCloseChanged() => Settings.Values["ExitOnClose"] = exitOnClose;

	[Property(CustomGetExpression = "(bool)(Settings.Values[" + nameof(TEMP_STATE_SETTING_NAME) + "] ?? false)", OnChanged = nameof(TempStateChanged))]
	private bool tempState = (bool)(Settings.Values[TEMP_STATE_SETTING_NAME] ?? false);
	private void TempStateChanged() => Settings.Values[TEMP_STATE_SETTING_NAME] = exitOnClose;
	private const string TEMP_STATE_SETTING_NAME = "TempState";

	SettingsWindow s_window;
    [RelayCommand]
    public void LaunchSettings()
    {
        s_window?.Activate();
    }
}
