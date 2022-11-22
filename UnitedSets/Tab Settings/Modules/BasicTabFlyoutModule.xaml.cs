using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UnitedSets.Classes;

namespace UnitedSets;

public sealed partial class BasicTabFlyoutModule
{
    public BasicTabFlyoutModule(TabBase TabBase)
    {
        this.TabBase = TabBase;
        InitializeComponent();
    }
    readonly TabBase TabBase;


    private void TabNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        TabBase.CustomTitle = TabNameTextBox.Text;
    }

    private void TabNameReset(object sender, RoutedEventArgs e)
    {
        TabNameTextBox.Text = "";
    }
}
