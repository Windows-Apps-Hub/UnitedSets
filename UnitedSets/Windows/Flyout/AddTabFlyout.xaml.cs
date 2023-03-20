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

namespace UnitedSets.Windows.Flyout;

public sealed partial class AddTabFlyout
{
    public WindowEx Result;
    readonly KeyboardHelper KeyboardHook = new();
    const uint VK_TAB = 0x09;
	const uint VK_ESCAPE = 0x1B;

    public AddTabFlyout()
    {
        InitializeComponent();
        KeyboardHook.KeyboardPressed += OnKeyPressed;
        this.SetForegroundWindow();
        this.CenterOnScreen();
        AppWindow.Move(new PointInt32(AppWindow.Position.X, 80));
        this.Hide();
		Closed +=  FlyoutClosed;

	}

	private void FlyoutClosed(object sender, WindowEventArgs args) {
		KeyboardHook.KeyboardPressed -= OnKeyPressed;
	}

	[Event(typeof(EventHandler<KeyboardHelperEventArgs>))]
    private void OnKeyPressed(KeyboardHelperEventArgs e)
    {
        if (e.KeyboardState == KeyboardHelper.KeyboardState.KeyDown)
        {
            if ( (e.KeyboardData.VirtualCode == VK_TAB || e.KeyboardData.VirtualCode == VK_ESCAPE) && AppWindow.IsVisible)
            {
				
				e.Handled = true;//don't pass the tab through
				if (e.KeyboardData.VirtualCode != VK_ESCAPE) {
					PInvoke.GetCursorPos(out var pt);
					Result = WindowEx.GetWindowFromPoint(pt);
				} else
					Result = default;
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
