using Microsoft.UI.Dispatching;
using UnitedSets.Apps;
using UnitedSets.Helpers;
using UnitedSets.PostProcessing;
using WindowHoster;
using WinWrapper;

namespace UnitedSets.Tabs;

public partial class WindowHostTab : TabBase
{
    Icon _Icon = default;
    string _Title;
	private readonly DispatcherQueue UIDispatcher;
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
        ParentWindow?.SelectedTab = this;
    }

    public override bool ContainsRegisteredWindow(RegisteredWindow window) => RegisteredWindow == window;

    /// <summary>
    /// Creates a tab for the given window, or returns <c>null</c> if the window can't (or shouldn't) be added.
    /// </summary>
    /// <param name="shouldBeHidden">Hide the window until the tab is selected</param>
    public static WindowHostTab? Create(WindowEx newWindow, bool shouldBeHidden = false)
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
        foreach (var setWindow in UnitedSetsApp.Current.SetWindows.ToArray())
            if (setWindow.Win32Window.Root.Children.Any(x => x == newWindow))
                return null;
        // already in a tab
        if (UnitedSetsApp.Current.FindTab(newWindow.Handle) is not null)
            return null;
        var registeredWindow = PostProcessingRegisteredWindow.Register(newWindow, shouldBeHidden);
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
