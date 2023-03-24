using Microsoft.UI.Xaml.Controls;
using System.Linq;
using WinUIEx;
using System;
using WindowEx = WinWrapper.Window;
using UnitedSets.Classes;
using System.ComponentModel;
using WinUI3HwndHostPlus;
using UnitedSets.Classes.Tabs;

namespace UnitedSets.UI.AppWindows;

public sealed partial class MainWindow : INotifyPropertyChanged
{
    public void AddTab(WindowEx newWindow, int? index = null)
    {
		var newTab = JustCreateTab(newWindow);
		if (newTab == null)
			return;
		AddTab(newTab, index);
		TabView.SelectedItem = newTab;
    }

	
	public HwndHostTab? JustCreateTab(WindowEx newWindow) {
		if (!newWindow.IsValid)
			return null;
		newWindow = newWindow.Root;
		if (newWindow.Handle == IntPtr.Zero)
			return null;
		if (newWindow.Handle == AddTabPopup.GetWindowHandle())
			return null;
		if (newWindow.Handle == WindowEx.Handle)
			return null;
		if (HwndHost.ShouldBeBlacklisted(newWindow))
			return null;
		// Check if United Sets has owner (United Sets in United Sets)
		if (WindowEx.Root.Children.Any(x => x == newWindow))
			return null;
		if (Tabs.ToArray().Any(x => x.Windows.Any(y => y == newWindow)))
			return null;
		return new HwndHostTab((IHwndHostParent tab) => new OurHwndHost(tab, this, newWindow),DispatcherQueue, newWindow, IsAltTabVisible);
	}
}
