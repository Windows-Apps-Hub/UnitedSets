using Get.EasyCSharp;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnitedSets.UI.Controls;
using UnitedSets.UI.FlyoutModules;

namespace UnitedSets.Tabs;

public abstract partial class TabBase : INotifyPropertyChanged
{
	public const bool DefaultIsSwitcherVisible = true;
	[AutoNotifyProperty]
	bool _IsSelected;
    [OptionalParameter(nameof(IsSwitcherVisible), DefaultIsSwitcherVisible)]
    public TabBase(bool IsSwitcherVisible)
    {
		AllTabs.Add(this);
        this.IsSwitcherVisible = IsSwitcherVisible;
        InitSwitcher();
    }
	protected virtual void DoShowTab() {
		UnitedSetsApp.Current.SelectedTab = this;
    }
    protected void ShowFlyout(UIElement Element, UIElement RelativeTo)
    {
        ShowFlyout([Element], RelativeTo);
    }

    protected async void ShowFlyout(UIElement[] Elements, UIElement RelativeTo) {
		await Task.Delay(300);
        StackPanel sp;
        var flyout = new BackdropedFlyout
        {
            Content = sp = new StackPanel
            {
                Width = 350,
                Spacing = 8,
                Children =
                {
                    new BasicTabFlyoutModule(this)
                }
            },
            ShouldConstrainToRootBounds = false,
            Placement = Microsoft.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Bottom
        };
        foreach (var ele in Elements)
            sp.Children.Add(ele);
        flyout.ShowAt((FrameworkElement)RelativeTo);
    }
	protected virtual void DoRemoveTab()
    {
        UnitedSetsApp.Current.DispatcherQueue.TryEnqueue(() => UnitedSetsApp.Current.Tabs.Remove(this));
    }
}
