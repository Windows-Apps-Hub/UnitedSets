using Microsoft.UI.Xaml.Controls;
using System.Linq;
using WinUIEx;
using System;
using WindowEx = WinWrapper.Windowing.Window;
using UnitedSets.Classes;
using System.ComponentModel;
using WinUI3HwndHostPlus;
using UnitedSets.Classes.Tabs;
using System.Collections.Generic;
using Get.OutOfBoundsFlyout;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Threading.Tasks;

namespace UnitedSets.UI.AppWindows;

public sealed partial class MainWindow : INotifyPropertyChanged
{
    public partial void AddTab(WindowEx newWindow, int? index)
    {
		var newTab = CreateHwndHostTab(newWindow);
		if (newTab == null)
			return;
		AddTab(newTab, index);
		TabView.SelectedItem = newTab;
    }
    public partial void AddTab(TabBase tab, int? index)
    {
        WireTabEvents(tab);
        if (index != null)
            Tabs.Insert(index.Value, tab);
        else
            Tabs.Add(tab);
    }
    public partial void RemoveTab(TabBase tab)
    {
        Tabs.Remove(tab);
        UnwireTabEvents(tab);
    }
    public partial IEnumerable<TabBase> GetTabsAndClear()
    {
        var ret = Tabs.ToArray();
        foreach (var tab in ret)
            RemoveTab(tab);
        return ret;
    }
    public partial TabBase? FindTabByWindow(WindowEx window)
    {
        return Tabs.ToArray().FirstOrDefault(tab => tab.Windows.Contains(window));
    }
    public partial (TabGroup? group, TabBase? tab) FindHiddenTabByWindow(WindowEx window)
    {
        foreach (var tabg in HiddenTabs.ToArray())
        {
            var tab = tabg.Tabs.ToArray().FirstOrDefault(tab => tab.Windows.Contains(window));
            if (tab != null)
                return (tabg, tab);
        }

        return (null, null);
    }
    public partial HwndHostTab? CreateHwndHostTab(WindowEx newWindow) {
		if (!newWindow.IsValid)
			return null;
		newWindow = newWindow.Root;
		if (newWindow.Handle == IntPtr.Zero)
			return null;
		if (newWindow.Handle == AddTabPopup.GetWindowHandle())
			return null;
		if (newWindow.Handle == Win32Window.Handle)
			return null;
		if (HwndHost.ShouldBeBlacklisted(newWindow))
			return null;
		// Check if United Sets has owner (United Sets in United Sets)
		if (Win32Window.Root.Children.AsEnumerable().Any(x => x == newWindow))
			return null;
		if (Tabs.ToArray().Any(x => x.Windows.Any(y => y == newWindow)))
			return null;
		return new HwndHostTab((IHwndHostParent tab) => new OurHwndHost(tab, this, newWindow),DispatcherQueue, newWindow, Constants.IsAltTabVisible);
	}

    private partial async Task TimerStop()
    {
        timer.Stop();
        OnUIThreadTimerLoop();
        await Task.Delay(100);
    }

    [DoesNotReturn]
    private partial async Task Suicide()
    {
        //trans_mgr?.Cleanup();
        OutOfBoundsFlyoutSystem.Dispose();
        await Task.Delay(300);
        Debug.WriteLine("Cleanish exit");
        Environment.Exit(0);
    }

    private partial void WireTabEvents(TabBase tab)
    {
        tab.RemoveTab += TabRemoveRequest;
        tab.ShowFlyout += TabShowFlyoutRequest;
        tab.ShowTab += TabShowRequest;
    }

    private partial void UnwireTabEvents(TabBase tab)
    {
        tab.RemoveTab -= TabRemoveRequest;
        tab.ShowFlyout -= TabShowFlyoutRequest;
        tab.ShowTab -= TabShowRequest;
    }
}
