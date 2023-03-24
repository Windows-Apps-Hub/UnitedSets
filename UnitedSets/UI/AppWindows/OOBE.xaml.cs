using UnitedSets.UI.AppWindows.Pages;

namespace UnitedSets.UI.AppWindows;


public sealed partial class OOBEWindow : MicaWindow
{
    public OOBEWindow() : base(IsMicaInfinite: false)
    {
        this.InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        OOBE.Navigate(typeof(OOBEPage));
    }
}
