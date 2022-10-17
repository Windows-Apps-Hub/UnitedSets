using Windows.System;
using WinRT.Interop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using System.Threading.Tasks;
using WindowEx = WinWrapper.Window;
using Windows.Win32;

namespace UnitedSets;

public sealed partial class AddTabFlyout : MicaWindow
{
    public WindowEx Result;
    public AddTabFlyout()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(BorderTitleBar);
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
        PInvoke.GetCursorPos(out var pt);
        AppWindow.Move(new Windows.Graphics.PointInt32(pt.X, pt.Y));
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
            PInvoke.GetCursorPos(out var pt);
            Result = WindowEx.GetWindowFromPoint(pt);
            Hide();
        }
    }
}
