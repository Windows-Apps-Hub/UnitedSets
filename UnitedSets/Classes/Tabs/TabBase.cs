using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnitedSets.Services;
using WinUI3HwndHostPlus;

namespace UnitedSets.Classes.Tabs;

public abstract partial class TabBase : INotifyPropertyChanged
{
    public TabBase(TabView Parent, bool IsSwitcherVisible)
    {
        AllTabs.Add(this);
        ParentTabView = Parent;
        this.IsSwitcherVisible = IsSwitcherVisible;
        InitSwitcher();
    }
	protected void SaveTabData(HwndHost host) {
		if (!Settings.TempState)
			return;
		GetWindowThreadProcessId(host.GetRawHWND(), out var pid);
		if (pid == 0)
			return;
		var data = new PreservedTabData(host.GetRawHWND().Value, pid) { CustomTitle = CustomTitle, Borderless = host.BorderlessWindow, CropEnabled = host.ActivateCrop, CropRect = new CropRect(host.CropLeft, host.CropTop, host.CropRight, host.CropBottom) };
		PreservedTabService.SaveTab(data);
	}
	protected virtual void LoadTabData(HwndHost host) {
		if (!Settings.TempState)
			return;
		GetWindowThreadProcessId(host.GetRawHWND(), out var pid);
		if (pid == 0)
			return;
		var data = PreservedTabService.LookupTab(pid, host.GetRawHWND().Value);
		if (data == null)
			return;
		CustomTitle = data.CustomTitle;
		host.BorderlessWindow = data.Borderless;
		host.ActivateCrop = data.CropEnabled;
		host.CropLeft = data.CropRect.Left;
		host.CropTop = data.CropRect.Top;
		host.CropRight = data.CropRect.Right;
		host.CropBottom = data.CropRect.Bottom;
	}
	[DllImport("user32.dll", SetLastError = true)]
	static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
}
