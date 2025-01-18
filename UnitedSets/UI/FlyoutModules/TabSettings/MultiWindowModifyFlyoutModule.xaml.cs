using System.Linq;
using Get.EasyCSharp;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using WindowHoster;
namespace UnitedSets.UI.FlyoutModules;

public sealed partial class MultiWindowModifyFlyoutModule
{
    public MultiWindowModifyFlyoutModule(RegisteredWindow[] Windows)
    {
        this.RegisteredWindows = Windows;
        InitializeComponent();
        foreach (var registeredWindow in Windows)
        {
            RegisteredWindowSelector.Items.Add(registeredWindow.Window.TitleText);
        }
        Loaded += delegate
        {
            if (Windows.Any(x => x.CompatablityMode.NoOwner))
            {
                var w = WinWrapper.Windowing.Window.FromWindowHandle((nint)XamlRoot.ContentIslandEnvironment.AppWindowId.Value);
                var allPopupWindows = WinWrapper.Windowing.Window.GetWindowsInThread(w.Thread)
                    .Where(x => x.Class.Name is "Microsoft.UI.Content.PopupWindowSiteBridge");
                foreach (var wind in allPopupWindows)
                    wind[WinWrapper.Windowing.WindowExStyles.TOPMOST] = true;
            }
        };
        Unloaded += delegate
        {

            var w = WinWrapper.Windowing.Window.FromWindowHandle((nint)XamlRoot.ContentIslandEnvironment.AppWindowId.Value);
            var allPopupWindows = WinWrapper.Windowing.Window.GetWindowsInThread(w.Thread)
                .Where(x => x.Class.Name is "Microsoft.UI.Content.PopupWindowSiteBridge");
            foreach (var wind in allPopupWindows)
                wind[WinWrapper.Windowing.WindowExStyles.TOPMOST] = false;
        };
    }

    readonly RegisteredWindow[] RegisteredWindows;

    [Event(typeof(SelectionChangedEventHandler))]
    void RegisteredWindowSelector_SelectionChanged()
    {
        if (RegisteredWindowSelector.SelectedIndex != -1)
            ModifyWindowFlyoutModulePlace.Child = new ModifyWindowFlyoutModule(
                RegisteredWindows[RegisteredWindowSelector.SelectedIndex]
            );
        else
            ModifyWindowFlyoutModulePlace.Child = null;
    }
}
