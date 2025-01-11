using System;
using System.Drawing;
using System.Threading.Tasks;
using Windows.Win32;
using Window = Microsoft.UI.Xaml.Window;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml;
using UnitedSets.UI.FlyoutModules;
using WinWrapper.Input;

namespace UnitedSets.Tabs;

partial class WindowHostTab
{
    public override async Task TryCloseAsync()
        => await Window.TryCloseAsync();

    public override async void DetachAndDispose(bool JumpToCursor)
    {
        var Window = this.Window;
		var NoMovingMode = RegisteredWindow.CompatablityMode.NoMoving;
		await RegisteredWindow.DetachAsync();
        var CursorPos = Cursor.Position;
        if (JumpToCursor && !NoMovingMode)
            Window.Location = new Point(CursorPos.X - 100, CursorPos.Y - 30);
        _IsDisposed = true;
    }
    public override void Focus()
    {
        //RegisteredWindow.SetVisible(true);
        RegisteredWindow.Window.Focus();
    }
    protected override void OnDoubleClick(UIElement sender, DoubleTappedRoutedEventArgs args)
    {
		ShowFlyout(new ModifyWindowFlyoutModule(RegisteredWindow), sender);
    }
    protected override void OnRightClick(UIElement sender, RightTappedRoutedEventArgs args)
        => OnDoubleClick(sender, null!);
}
