using System;
using Window = Microsoft.UI.Xaml.Window;
using WindowEx = WinWrapper.Windowing.Window;
using UnitedSets.Helpers;
using WinWrapper.Input;
using Microsoft.UI.Dispatching;
using UnitedSets.UI.Popups;
using WinWrapper;

namespace UnitedSets.Classes.Tabs;

public partial class HwndHostTab : TabBase, IHwndHostParent
{
    Icon _Icon = default;
    string _Title;
	private DispatcherQueue UIDispatcher;
	public HwndHostTab(Func<IHwndHostParent,OurHwndHost> GetHwndHost, DispatcherQueue UIDispatcher, WindowEx WindowEx, bool IsTabSwitcherVisibile) : base(IsTabSwitcherVisibile)
    {
		this.UIDispatcher = UIDispatcher;

		this.Window = WindowEx;
		HwndHost = GetHwndHost(this);
		HwndHost.SetVisible(false);
		HwndHost.SetBorderless(Keyboard.IsAltDown);


		HwndHost.Closed += (_,_) => DoRemoveTab();
        _Title = DefaultTitle;
        UpdateAppIcon();
    }

	TabBase IHwndHostParent.Tab => this;
    async void UpdateAppIcon()
    {
        var icon = Window.LargeIconAsBitmap ?? Window.SmallIconAsBitmap;
        if (icon is not null)
        {
            var oldIcon = _BitmapIcon;
            _IconBmpImg = await icon.ToXAMLBitmapImageAsync();
            _BitmapIcon = icon;
            OnIconChanged();
            oldIcon?.Dispose();
        }
    }
}
