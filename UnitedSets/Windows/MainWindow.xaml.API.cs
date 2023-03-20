using EasyCSharp;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using WinRT.Interop;
using WinUIEx;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using System;
using WindowRelative = WinWrapper.WindowRelative;
using WindowEx = WinWrapper.Window;
using Cursor = WinWrapper.Cursor;
using Keyboard = WinWrapper.Keyboard;
using UnitedSets.Classes;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Diagnostics;
using WinUIEx.Messaging;
using Microsoft.UI.Dispatching;
using System.Threading;
using System.IO;
using WinWrapper;
using System.Text.RegularExpressions;
using Windows.Foundation;
using WinUI3HwndHostPlus;
using UnitedSets.Classes.Tabs;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using UnitedSets.Windows.Flyout.Modules;
using UnitedSets.Windows.Flyout;

namespace UnitedSets.Windows;

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

	public void SaveCurSettingsAsDefault() => persistantService.ExportSettings(USConfig.DefaultConfigFile, true, true);//don't give user any choice as to what for now so will exclude current tabs
	public async Task ResetSettingsToDefault() {
		await persistantService.ResetSettings();
		SaveCurSettingsAsDefault();
	}
	public HwndHostTab? JustCreateTab(WindowEx newWindow) {
		if (!newWindow.IsValid)
			return null;
		newWindow = newWindow.Root;
		if (newWindow.Handle == IntPtr.Zero)
			return null;
		if (newWindow.Handle == AddTabFlyout.GetWindowHandle())
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
