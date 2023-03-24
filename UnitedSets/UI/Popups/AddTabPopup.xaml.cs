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
using EasyCSharp;
using System;
namespace UnitedSets.UI.Popups;

public sealed partial class AddTabPopup
{
    public WindowEx Result;
    readonly KeyboardHelper KeyboardHook = new();
    const uint VK_TAB = 0x09;

    public AddTabPopup() : base(IsMicaInfinite: true)
    {
        InitializeComponent();
        KeyboardHook.KeyboardPressed += OnKeyPressed;
        this.SetForegroundWindow();
        this.CenterOnScreen();
        AppWindow.Move(new PointInt32(AppWindow.Position.X, 80));
        this.Hide();
    }

    [Event(typeof(EventHandler<KeyboardHelperEventArgs>))]
    private void OnKeyPressed(KeyboardHelperEventArgs e)
    {
        if (e.KeyboardState == KeyboardHelper.KeyboardState.KeyDown)
        {
            if (e.KeyboardData.VirtualCode == VK_TAB && AppWindow.IsVisible)
            {
				e.Handled = true;//don't pass the tab through
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
        AppWindow.Move(new PointInt32(AppWindow.Position.X, 80));
        AppWindow.Show();
        while (AppWindow.IsVisible) 
            await Task.Delay(1000);
    }

    [Event(typeof(RoutedEventHandler))]
    private void CancelClick() => this.Hide();
}
