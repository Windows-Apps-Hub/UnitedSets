using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UnitedSets.Classes;

namespace UnitedSets;

public sealed partial class MultiWindowModifyFlyoutModule
{
    public MultiWindowModifyFlyoutModule(HwndHost[] HwndHosts)
    {
        this.HwndHosts = HwndHosts;
        InitializeComponent();
        foreach (var hwndhost in HwndHosts)
        {
            HwndHostSelector.Items.Add(hwndhost.HostedWindow.TitleText);
        }
    }
    readonly HwndHost[] HwndHosts;

    private void HwndHostSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (HwndHostSelector.SelectedIndex != -1)
            ModifyWindowFlyoutModulePlace.Child = new ModifyWindowFlyoutModule(HwndHosts[HwndHostSelector.SelectedIndex]);
        else
            ModifyWindowFlyoutModulePlace.Child = null;
    }
}
