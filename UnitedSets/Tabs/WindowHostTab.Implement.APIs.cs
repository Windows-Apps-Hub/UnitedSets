using UnitedSets.UI.FlyoutModules;
using WinWrapper.Input;

namespace UnitedSets.Tabs;

partial class WindowHostTab
{
    public override async Task TryCloseAsync()
        => await Window.TryCloseAsync();

    public override async void DetachAndDispose(bool JumpToCursor)
        => await DetachAndDisposeAsync(JumpToCursor);
    public override async Task DetachAndDisposeAsync(bool JumpToCursor = false)
    {
        var Window = this.Window;
		var NoMovingMode = RegisteredWindow.CompatablityMode.NoMoving;
        _IsDisposed = true;
		await RegisteredWindow.DetachAsync();
        var CursorPos = Cursor.Position;
        if (JumpToCursor && !NoMovingMode && Window.IsValid)
            Window.Location = new PointInt(CursorPos.X - 100, CursorPos.Y - 30);
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
