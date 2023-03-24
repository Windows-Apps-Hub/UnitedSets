using System;
using Window = Microsoft.UI.Xaml.Window;
using WindowEx = WinWrapper.Window;
using UnitedSets.Helpers;
using WinWrapper;
using Microsoft.UI.Dispatching;

namespace UnitedSets.Classes.Tabs;

public partial class HwndHostTab : TabBase, IHwndHostParent
{
    IntPtr _Icon = IntPtr.Zero;
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
        var icon = Window.LargeIcon ?? Window.SmallIcon;
        if (icon is not null)
        {
            var oldIcon = _BitmapIcon;
            _IconBmpImg = await ImageHelper.ImageFromBitmap(icon);
            _BitmapIcon = icon;
            OnIconChanged();
            oldIcon?.Dispose();
        }
    }
}
