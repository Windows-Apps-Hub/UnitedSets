using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UnitedSets.Classes;
using EasyCSharp;
namespace UnitedSets;

public sealed partial class BasicTabFlyoutModule
{
    public BasicTabFlyoutModule(TabBase TabBase)
    {
        this.TabBase = TabBase;
        InitializeComponent();
    }
    readonly TabBase TabBase;

    [Event(typeof(TextChangedEventHandler))]
    private void TabNameTextBoxChanged()
    {
        TabBase.CustomTitle = TabNameTextBox.Text;
    }

    [Event(typeof(RoutedEventHandler))]
    private void TabNameReset()
    {
        TabNameTextBox.Text = "";
    }
}
