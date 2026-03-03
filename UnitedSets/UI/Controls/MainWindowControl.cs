using UnitedSets.Apps;

namespace UnitedSets.UI.Controls;

[QuickMarkup("""
    MainWindow MainWindow;
    <root Padding=5
        Flyout=
            <BackdropedFlyout !ShouldConstrainToRootBounds>
                <MainWindowMenuFlyoutModule MainWindow=`MainWindow` />
            </BackdropedFlyout>
    >
        <FluentIconElement Symbol=`SettingsMode ? FluentSymbol.Settings20 : FluentSymbol.Navigation20` Margin=`new(-2,-6,0,0)` />
    </root>
    """)]
partial class MainWindowControlButton : Button
{
    static bool SettingsMode => !Constants.IsExperimentalVersion;
    public MainWindowControlButton()
    {
        Init();
        if (SettingsMode) Flyout = null;
    }
}
