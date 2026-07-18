using UnitedSets.UI.FlyoutModules;
using WinWrapper.Input;

namespace UnitedSets.Tabs;

partial class WindowHostTab
{
    public override async Task TryCloseAsync()
        => await Window.TryCloseAsync();

    public override async void DetachAndDispose(bool JumpToCursor)
        => await DetachAndDisposeAsync(JumpToCursor);

    public override async Task DetachAndDisposeAsync(bool JumpToCursor)
    {
        var Window = this.Window;
		var NoMovingMode = RegisteredWindow.CompatablityMode.NoMoving;
		await RegisteredWindow.DetachAsync();
        var CursorPos = Cursor.Position;
        if (JumpToCursor && !NoMovingMode)
            Window.Location = new PointInt(CursorPos.X - 100, CursorPos.Y - 30);
        _IsDisposed = true;
    }
    public override void Focus()
    {
        DoShowTab();
    }
    protected override void OnDoubleClick(UIElement sender, DoubleTappedRoutedEventArgs args)
    {
		ShowFlyout(new ModifyWindowFlyoutModule(RegisteredWindow), sender);
    }
    protected override void OnRightClick(UIElement sender, RightTappedRoutedEventArgs args)
        => OnDoubleClick(sender, null!);
}
