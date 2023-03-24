using EasyCSharp;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation;

namespace UnitedSets.Classes.Tabs;

public abstract partial class TabBase : INotifyPropertyChanged
{
	public const bool DefaultIsSwitcherVisible = true;
    [OptionalParameter(nameof(IsSwitcherVisible), DefaultIsSwitcherVisible)]
    public TabBase(bool IsSwitcherVisible)
    {
		AllTabs.Add(this);
        this.IsSwitcherVisible = IsSwitcherVisible;
        InitSwitcher();
    }
	public event EventHandler? RemoveTab;
	public event EventHandler? ShowTab;
    public event EventHandler<ShowFlyoutEventArgs>? ShowFlyout;
    
	protected virtual void DoShowTab() {
		this.ShowTab?.Invoke(this, EventArgs.Empty);
	}
	public record class ShowFlyoutEventArgs(UIElement Element, Point CursorPosition, UIElement RelativeTo, PointerDeviceType PointerDeviceType) {
		
	}
	protected virtual void DoShowFlyout(UIElement Element, Point CursorPosition, UIElement RelativeTo, PointerDeviceType PointerDeviceType) {
		var args = new ShowFlyoutEventArgs(Element, CursorPosition, RelativeTo, PointerDeviceType);
		ShowFlyout?.Invoke(this, args);
	}
	protected virtual void DoRemoveTab() => RemoveTab?.Invoke(this, EventArgs.Empty);
}
