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
    private KeyboardHelper KeyboardHook;
    private const uint VK_TAB = 0x09;

    public AddTabFlyout()
    {
        InitializeComponent();
        KeyboardHook = new KeyboardHelper();
        KeyboardHook.KeyboardPressed += OnKeyPressed;
        MicaHelper Mica = new();
        Mica.TrySetMicaBackdrop(this);
        this.SetForegroundWindow();
        this.CenterOnScreen();
        AppWindow.Move(new PointInt32(this.AppWindow.Position.X, 80));
        this.Hide();
    }

    private void OnKeyPressed(object? sender, KeyboardHelperEventArgs e)
    {
        if (e.KeyboardState == KeyboardHelper.KeyboardState.KeyDown)
        {
            if (e.KeyboardData.VirtualCode == VK_TAB && AppWindow.IsVisible)
            {
                PInvoke.GetCursorPos(out var pt);
                Result = WindowEx.GetWindowFromPoint(pt);
                this.Hide();
            }
        }
    }

    public async ValueTask ShowAsync()
    {
        Result = default;
        this.CenterOnScreen();
        AppWindow.Move(new PointInt32(this.AppWindow.Position.X, 80));
        AppWindow.Show();
        while (AppWindow.IsVisible) 
            await Task.Delay(1000);
    }
    private void CancelClick(object sender, RoutedEventArgs e) => this.Hide();
}
