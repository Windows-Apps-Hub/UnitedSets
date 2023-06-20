using Microsoft.UI.Xaml;
using Window = WinWrapper.Windowing.Window;
using Microsoft.UI.Windowing;
using WinUIEx;
using Microsoft.UI.Xaml.Controls;

namespace UnitedSets.UI.AppWindows;

partial class MainWindow
{
    void SetupTaskbarMode()
    {
        TabViewBorder.Visibility = Visibility.Collapsed;
        //CustomDragRegionUpdator
        IsMinimizable = false;
        IsMaximizable = false;
        Grid.SetRow(MainAreaBorder, 0);
        Grid.SetRowSpan(MainAreaBorder, 2);
        MainAreaBorder.Margin = new(0, top: 8, 0, 0);
        var window = new FloatingTaskbar(this)
        {
            IsShownInSwitchers = false,
            IsTitleBarVisible = false
        };
        var windownative = Window.FromWindowHandle(window.GetWindowHandle());
        windownative.Owner = Win32Window;
        var presenter = (OverlappedPresenter)AppWindow.Presenter;

        AppWindow.Changed += PositionAppWindowChanged;
        void PositionAppWindowChanged(AppWindow sender, AppWindowChangedEventArgs args)
        {
            if (args.DidPositionChange || args.DidSizeChange)
            {
                var bounds = Win32Window.Bounds;
                window.AnchorPoint = new(bounds.Right, bounds.Bottom + 10);
            }
        }
        window.Activate();
    }
}
