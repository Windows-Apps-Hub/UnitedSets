using UnitedSets.Windows.Pages;

namespace UnitedSets.Windows;


public sealed partial class OOBEWindow : MicaWindow
{
    public OOBEWindow()
    {
        this.InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        OOBE.Navigate(typeof(OOBEPage));
    }
}
