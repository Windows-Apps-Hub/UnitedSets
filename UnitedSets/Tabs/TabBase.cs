using Get.EasyCSharp;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using UnitedSets.UI.FlyoutModules;
using Windows.Foundation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
	protected async void ShowFlyout(UIElement Element, UIElement RelativeTo) {
		await Task.Delay(300);

        var flyout = new Flyout
        {
            Content = new StackPanel
            {
                Width = 350,
                Spacing = 8,
                Children =
                {
                    new BasicTabFlyoutModule(this),
                    Element
                }
            },
            ShouldConstrainToRootBounds = false,
            Placement = Microsoft.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Bottom
        };
        flyout.ShowAt((FrameworkElement)RelativeTo);
    }
	protected virtual void DoRemoveTab()
    {
        UnitedSetsApp.Current.DispatcherQueue.TryEnqueue(() => UnitedSetsApp.Current.Tabs.Remove(this));
    }
}
