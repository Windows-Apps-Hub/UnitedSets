using Windows.System;
using WinRT.Interop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PInvoke;
using Microsoft.UI.Xaml.Input;
using WindowStyles = PInvoke.User32.WindowStyles;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using System.Threading.Tasks;

namespace UnitedSets;

public sealed partial class AddTabFlyout : MicaWindow
{
    public WindowEx Result;
    public AddTabFlyout()
    {
        InitializeComponent();
        SetTitleBar(TitleBar);
        AppWindow.Closing += AppWindow_Closing;
        Activated += delegate
        {
            btn.Focus(FocusState.Keyboard);
        };
    }

    private void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        if (AppWindow.IsVisible)
        {
            args.Cancel = true;
            Hide();
        }
    }

    public void Hide()
    {
        AppWindow.Hide();
    }
    public async ValueTask ShowAtCursorAsync()
    {
        var pt = User32.GetCursorPos();
        AppWindow.Move(new Windows.Graphics.PointInt32(pt.x, pt.y));
        AppWindow.Show();
        btn.Focus(FocusState.Keyboard);
        while (AppWindow.IsVisible)
            await Task.Delay(1000);
    }
    private void CancelClick(object sender, RoutedEventArgs e)
    {
        Hide();
    }

    private void KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key.HasFlag(VirtualKey.Shift))
        {
            Result = WindowEx.GetWindowFromPoint(User32.GetCursorPos());
            Hide();
        }
    }
}
