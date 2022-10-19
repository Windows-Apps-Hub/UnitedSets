using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace UnitedSets.Services
{
    public partial class SettingsService : ObservableObject
    {
        private static ApplicationDataContainer Settings = ApplicationData.Current.LocalSettings;

        private bool exitOnClose = (bool)(Settings.Values["ExitOnClose"] ?? true);
        public bool ExitOnClose
        {
            get => exitOnClose;
            set
            {
                Settings.Values["ExitOnClose"] = value;
                SetProperty(ref exitOnClose, value);
            }
        }

        [RelayCommand]
        public void LaunchSettings()
        {
            SettingsWindow s_window = new SettingsWindow();
            s_window.Activate();
        }
    }
}
