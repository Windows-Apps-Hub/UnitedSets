using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using UnitedSets.Mvvm.Services;
using WinUIEx;

namespace UnitedSets.UI.AppWindows;


public sealed partial class SettingsWindow : WindowEx
{
    public UnitedSetsAppSettings Settings;

    public SettingsWindow(UnitedSetsAppSettings Settings, MainWindow mainWindow)
    {
        this.Settings = Settings;
        this.mainWindow = mainWindow;
        this.InitializeComponent();
        //themeCntrl.Visibility = USConfig.FLAGS_THEME_CHOICE_ENABLED ? Visibility.Visible : Visibility.Collapsed;
        gridMain.DataContext = this;//we have to use normal bindings for anything with a converter as winui is broke af https://github.com/microsoft/microsoft-ui-xaml/issues/4966
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        SystemBackdrop = new MicaBackdrop();
    }
    private MainWindow mainWindow;
    [RelayCommand]
    public void SaveDefaultSettings()
    {
        mainWindow.SaveCurSettingsAsDefault();
    }
}
