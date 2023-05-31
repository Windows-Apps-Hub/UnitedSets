using UnitedSets.Mvvm.Services;

namespace UnitedSets.UI.AppWindows;


public sealed partial class SettingsWindow : MicaWindow
{
    public SettingsService Settings;

    public SettingsWindow(SettingsService Settings) : base(IsMicaInfinite: false)
    {
        this.Settings = Settings;
        this.InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
    }
}
