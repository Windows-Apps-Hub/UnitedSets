using Window = Microsoft.UI.Xaml.Window;
using WindowEx = WinWrapper.Windowing.Window;
using UnitedSets.Helpers;
using WinWrapper.Input;
using Microsoft.UI.Dispatching;
using WinWrapper;
using WindowHoster;

namespace UnitedSets.Classes.Tabs;

public partial class WindowHostTab : TabBase
{
    Icon _Icon = default;
    string _Title;
	private DispatcherQueue UIDispatcher;
	public WindowHostTab(RegisteredWindow Window, DispatcherQueue UIDispatcher, WindowEx WindowEx, bool IsTabSwitcherVisibile) : base(IsTabSwitcherVisibile)
    {
		this.UIDispatcher = UIDispatcher;

		this.Window = WindowEx;
		RegisteredWindow = Window;
		//RegisteredWindow.SetVisible(false);
		RegisteredWindow.Properties.BorderlessWindow = Keyboard.IsAltDown;


		RegisteredWindow.BecomesInvalid += DoRemoveTab;
        _Title = DefaultTitle;
        UpdateAppIcon();
    }

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
