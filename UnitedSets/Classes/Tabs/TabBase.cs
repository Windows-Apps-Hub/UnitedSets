using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace UnitedSets.Classes.Tabs;

public abstract partial class TabBase : INotifyPropertyChanged
{
    public TabBase(bool IsSwitcherVisible)
    {
		AllTabs.Add(this);
        this.IsSwitcherVisible = IsSwitcherVisible;
        InitSwitcher();
    }
	public event EventHandler? RemoveTab;
	public event EventHandler? ShowTab;
	protected virtual void DoShowTab() {
		this.ShowTab?.Invoke(this, EventArgs.Empty);
	}
	public event EventHandler<ShowFlyoutEventArgs>? ShowFlyout;
	public class ShowFlyoutEventArgs : EventArgs {
		public ShowFlyoutEventArgs(UIElement element) {
			this.element = element;
		}
		public UIElement element;
	}
	protected virtual void DoShowFlyout(UIElement element) {
		var args = new ShowFlyoutEventArgs(element);
		ShowFlyout?.Invoke(this, args);
	}
	protected virtual void DoRemoveTab() => RemoveTab?.Invoke(this, EventArgs.Empty);
}
