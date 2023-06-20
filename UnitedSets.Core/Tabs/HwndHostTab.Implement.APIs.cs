using System;
using System.Drawing;
using System.Threading.Tasks;
using Windows.Win32;
using Window = Microsoft.UI.Xaml.Window;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml;
using UnitedSets.UI.FlyoutModules;
using WinWrapper.Input;

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
        var CursorPos= Cursor.Position;
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
