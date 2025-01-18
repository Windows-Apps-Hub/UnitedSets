using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UnitedSets.Tabs;
using Get.EasyCSharp;
namespace UnitedSets.UI.FlyoutModules;

public sealed partial class CellTabFlyoutModule
{
    public CellTabFlyoutModule(CellTab CellTab)
    {
        this.CellTab = CellTab;
        InitializeComponent();
        CellMarginNB.Value = CellTab.CellMargin;
    }
    readonly CellTab CellTab;
    private void CellMarginNB_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        CellTab.CellMargin = CellMarginNB.Value;
    }

    private void CellMarginReset(object sender, RoutedEventArgs e)
    {
        CellMarginNB.Value = 10;
    }
}
