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
using WinUIEx;
using Windows.Graphics;
using UnitedSets.Helpers;

namespace UnitedSets;

public sealed partial class AddTabFlyout : WinUIEx.WindowEx
{
    public WindowEx Result;
    public AddTabFlyout()
    {
        InitializeComponent();
        MicaHelper Mica = new();
        Mica.TrySetMicaBackdrop(this);
        this.SetForegroundWindow();
        this.CenterOnScreen();
        AppWindow.Move(new PointInt32(this.AppWindow.Position.X, 80));
        this.Hide();
    }

    public async ValueTask ShowAtCursorAsync()
    {
        /* PInvoke.GetCursorPos(out var pt);
         AppWindow.Move(new Windows.Graphics.PointInt32(pt.X, pt.Y));
         AppWindow.Show();
         btn.Focus(FocusState.Keyboard);
         while (AppWindow.IsVisible)
             await Task.Delay(1000);*/ // bad UX
        //btn.Focus(FocusState.Keyboard);
        AppWindow.Show();
        while (AppWindow.IsVisible) 
            await Task.Delay(1000);
    }
    private void CancelClick(object sender, RoutedEventArgs e) => this.Hide();

    private void KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key.HasFlag(VirtualKey.Shift))
        {
            PInvoke.GetCursorPos(out var pt);
            Result = WindowEx.GetWindowFromPoint(pt);
            this.Hide();
        }
    }
}
