using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace UnitedSets.Services
{
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
            }).Start();
            s_window.Closed += delegate
            {
                s_window = new();
            };
        }
        private static ApplicationDataContainer Settings = ApplicationData.Current.LocalSettings;

        private bool exitOnClose = (bool)(Settings.Values["ExitOnClose"] ?? true);
        public bool ExitOnClose
        {
            get => (bool)(Settings.Values["ExitOnClose"] ?? true);
            set
            {
                Settings.Values["ExitOnClose"] = value;
            }
        }
        SettingsWindow s_window = new();
        [RelayCommand]
        public void LaunchSettings()
        {
            s_window.Activate();
        }
    }
}
