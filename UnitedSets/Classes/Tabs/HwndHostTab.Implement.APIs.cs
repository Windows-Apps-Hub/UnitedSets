using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Drawing;
using System.Threading.Tasks;
using Windows.Win32;
using Window = Microsoft.UI.Xaml.Window;
using WindowEx = WinWrapper.Window;
using System.Collections.Generic;
using System.Linq;
using UnitedSets.Helpers;
using static WinUIEx.WindowExtensions;
using WinWrapper;
using WinUIEx;
using WinUI3HwndHostPlus;
using UnitedSets.Windows;
using UnitedSets.Windows.Flyout;
using UnitedSets.Windows.Flyout.Modules;
using Windows.Win32.Graphics.Gdi;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml;

namespace UnitedSets.Classes.Tabs;

partial class HwndHostTab
{
    public override async Task TryCloseAsync()
        => await Window.TryCloseAsync();

    public override async void DetachAndDispose(bool JumpToCursor)
    {
        var Window = this.Window;
		var NoMovingMode = HwndHost.NoMoving;
		await HwndHost.DetachAndDispose();
        PInvoke.GetCursorPos(out var CursorPos);
        if (JumpToCursor && !NoMovingMode)
            Window.Location = new Point(CursorPos.X - 100, CursorPos.Y - 30);
        _IsDisposed = true;
    }
    public override void Focus()
    {
		HwndHost.SetVisible(true);
    }
    protected override void OnDoubleClick(UIElement sender, DoubleTappedRoutedEventArgs args)
    {
		DoShowFlyout(new ModifyWindowFlyoutModule(HwndHost), args.GetPosition(sender), sender, args.PointerDeviceType);
    }
}
