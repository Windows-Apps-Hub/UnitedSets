using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System;
using UnitedSets.Classes;
using UnitedSets.Mvvm.Services;
using System.Linq;

namespace UnitedSets.UI.AppWindows;


public sealed partial class SettingsWindow : MicaWindow
{
    public SettingsService Settings;

    public SettingsWindow(SettingsService Settings, MainWindow mainWindow) : base(IsMicaInfinite: false)
    {
        this.Settings = Settings;
        this.mainWindow = mainWindow;
        cfg = Settings.cfg;
        this.InitializeComponent();
        themeCntrl.Visibility = USConfig.FLAGS_THEME_CHOICE_ENABLED ? Visibility.Visible : Visibility.Collapsed;
        gridMain.DataContext = this;//we have to use normal bindings for anything with a converter as winui is broke af https://github.com/microsoft/microsoft-ui-xaml/issues/4966
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
    }
    public USConfig cfg { get; set; }
    private MainWindow mainWindow;
    public Type ThemeOptionEnumType => typeof(ElementTheme);

    public List<string> theme_options { get; set; } = Enum.GetValues<ElementTheme>().Select(a => a.ToString()).ToList();
    [RelayCommand]
    public void SaveDefaultSettings()
    {
        mainWindow.SaveCurSettingsAsDefault();
    }
}
