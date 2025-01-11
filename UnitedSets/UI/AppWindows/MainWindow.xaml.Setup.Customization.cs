using System;
using System.IO;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.XamlTypeInfo;
using UnitedSets.Apps;
using UnitedSets.Configurations;
using UnitedSets.Mvvm.Services;
using UnitedSets.Settings;
using WinUIEx;

namespace UnitedSets.UI.AppWindows;

partial class MainWindow
{
    void SetupCustomization()
    {
        UnitedSetsApp.Current.Configuration.PersistantService.FinalizeLoadAsync();

        void SetupSetting<T>(Setting<T> setting, Action<T> onUpdate)
        {
            setting.PropertyChanged += (_, _) => onUpdate(setting.Value);
            onUpdate(setting.Value);
        }
        void SetupStartupSetting<T>(Setting<T> setting, Action<T> onUpdate)
        {
            onUpdate(setting.Value);
        }
        var settings = UnitedSetsApp.Current.Settings;
        var gradStops = (WindowBorderOnTransparent.BorderBrush as LinearGradientBrush)!.GradientStops;
        SetupSetting(settings.TaskbarIcon, x =>
        {
            if (x is not null)
            {
                var y = Path.IsPathRooted(x) ? x : Path.Combine(USConfig.RootLocation, x);
                if (File.Exists(y))
                {
                    var icon = Icon.FromFile(y);
                    this.SetTaskBarIcon(icon);
                }
                else throw new FileNotFoundException($"File not found: {y}");
            }
        });
        SetupSetting(settings.BackdropMode, x => SystemBackdrop = x.GetSystemBackdrop());
        SetupSetting(settings.BypassMinimumSize, bypass => {
            MinWidth = bypass ? Constants.BypassMinWidth : Constants.MinWidth;
            MinHeight = bypass ? Constants.BypassMinHeight : Constants.MinHeight;
        });
        SetupSetting(settings.BorderGraident1, x => gradStops[0].Color = x);
        SetupSetting(settings.BorderGraident2, x => gradStops[^1].Color = x);
        SetupSetting(settings.BorderThickness, x => WindowBorderOnTransparent.BorderThickness = x);
        SetupSetting(settings.MainMargin, x => WindowBorderOnTransparent.Margin = x);
        SetupSetting(settings.CornerRadius, x => WindowBorderOnTransparent.CornerRadius = x);
        SetupSetting(settings.WindowTitlePrefix, x => UpdateTitle());
        SetupStartupSetting(settings.InitialWindowSize, size =>
        {
            if (size.HasValue) this.SetWindowSize(size.Value.Width, size.Value.Height);
        });

    }
}
