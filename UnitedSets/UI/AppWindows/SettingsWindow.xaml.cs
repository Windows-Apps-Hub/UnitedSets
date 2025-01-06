using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using UnitedSets.Configurations;
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
    public void SaveDefaultSettings() => SaveCurSettingsAsDefault();
    public void SaveCurSettingsAsDefault() => UnitedSetsApp.Current.Configuration.PersistantService.ExportSettings(USConfig.DefaultConfigFile, true, true);//don't give user any choice as to what for now so will exclude current tabs
    public async Task ResetSettingsToDefault()
    {
        await UnitedSetsApp.Current.Configuration.PersistantService.ResetSettings();
        SaveCurSettingsAsDefault();
    }
}
