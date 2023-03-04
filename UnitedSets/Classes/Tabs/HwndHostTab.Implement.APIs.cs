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

namespace UnitedSets.Classes.Tabs;

partial class HwndHostTab
{
    public override async Task TryCloseAsync()
        => await Window.TryCloseAsync();

    public override async void DetachAndDispose(bool JumpToCursor)
    {
        var Window = this.Window;
        HwndHost.DetachAndDispose();
        PInvoke.GetCursorPos(out var CursorPos);
        if (JumpToCursor && !HwndHost.NoMovingMode)
            Window.Location = new Point(CursorPos.X - 100, CursorPos.Y - 30);
        Window.Focus();
        Window.Redraw();
        await Task.Delay(1000).ContinueWith(_ => Window.Redraw());
        _IsDisposed = true;
    }
    
    public override void Focus()
    {
        HwndHost.IsWindowVisible = true;
        HwndHost.FocusWindow();
    }
    
    protected override async void OnDoubleClick()
    {
        var flyout = new LeftFlyout(
            WindowEx.FromWindowHandle(MainWindow.GetWindowHandle()),
            new BasicTabFlyoutModule(this),
            new ModifyWindowFlyoutModule(HwndHost)
        );
        await flyout.ShowAsync();
        flyout.Close();
    }
}
