using EasyCSharp;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UnitedSets.Classes;
using WinUI3HwndHostPlus;
namespace UnitedSets.UI.FlyoutModules;

public sealed partial class MultiWindowModifyFlyoutModule
{
    public MultiWindowModifyFlyoutModule(OurHwndHost[] HwndHosts)
    {
        this.HwndHosts = HwndHosts;
        InitializeComponent();
        foreach (var hwndhost in HwndHosts)
        {
            HwndHostSelector.Items.Add(hwndhost.GetTitle());
        }
    }
    readonly OurHwndHost[] HwndHosts;

    [Event(typeof(SelectionChangedEventHandler))]
    void HwndHostSelector_SelectionChanged()
    {
        if (HwndHostSelector.SelectedIndex != -1)
            ModifyWindowFlyoutModulePlace.Child = new ModifyWindowFlyoutModule(
                HwndHosts[HwndHostSelector.SelectedIndex]
            );
        else
            ModifyWindowFlyoutModulePlace.Child = null;
    }
}
