using Window = Microsoft.UI.Xaml.Window;
using WindowEx = WinWrapper.Windowing.Window;
using UnitedSets.Helpers;
using WinWrapper.Input;
using Microsoft.UI.Dispatching;
using WinWrapper;
using WindowHoster;
using System.Linq;
using System;
using UnitedSets.PostProcessing;
using System.Threading.Tasks;
using Windows.Win32;
using UnitedSets.Apps;

namespace UnitedSets.Tabs;

public partial class WindowHostTab : TabBase
{
    Icon _Icon = default;
    string _Title;
	private DispatcherQueue UIDispatcher;
	private WindowHostTab(RegisteredWindow Window, DispatcherQueue UIDispatcher, WindowEx WindowEx, bool IsTabSwitcherVisibile) : base(IsTabSwitcherVisibile)
    {
		this.UIDispatcher = UIDispatcher;

		this.Window = WindowEx;
		RegisteredWindow = Window;
		//RegisteredWindow.SetVisible(false);


		RegisteredWindow.BecomesInvalid += DoRemoveTab;
        RegisteredWindow.ShownByUser += AttemptToSelectTab;
        _Title = DefaultTitle;
        UpdateAppIcon();
    }

    private async void AttemptToSelectTab()
    {
        await Task.Delay(100);
        UnitedSetsApp.Current.SelectedTab = this;
    }

    public static WindowHostTab? Create(WindowEx newWindow)
    {
        if (!newWindow.IsValid)
            return null;
        newWindow = newWindow.Root;
        if (newWindow.Handle == IntPtr.Zero)
            return null;
        if (UnitedSetsApp.Current.AllUnitedSetsWindows.Contains(newWindow))
            return null;
        if (Constants.ShouldBeBlacklisted(newWindow))
            return null;
        // Check if United Sets has owner (United Sets in United Sets)
        if (UnitedSetsApp.Current.MainWindow.Win32Window.Root.Children.Any(x => x == newWindow))
            return null;
        if (UnitedSetsApp.Current.Tabs.ToArray().Any(x => x.Windows.Any(y => y == newWindow)))
            return null;
        var registeredWindow = PostProcessingRegisteredWindow.Register(newWindow);
        if (registeredWindow is null)
            return null;
        return new WindowHostTab(registeredWindow, UnitedSetsApp.Current.DispatcherQueue, newWindow, Constants.IsAltTabVisible);
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
