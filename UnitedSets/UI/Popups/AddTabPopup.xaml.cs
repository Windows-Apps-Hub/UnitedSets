using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using System.Threading.Tasks;
using WindowEx = WinWrapper.Window;
using Windows.Win32;
using WinUIEx;
using Windows.Graphics;
using WinWrapper;
using EasyCSharp;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace UnitedSets.UI.Popups;

public sealed partial class AddTabPopup
{
    public WindowEx Result;

    public AddTabPopup() : base(IsMicaInfinite: true)
    {
        InitializeComponent();
        LowLevelKeyboard.KeyPressed += OnKeyPressed;
        this.SetForegroundWindow();
        this.CenterOnScreen();
        AppWindow.Move(new PointInt32(AppWindow.Position.X, 80));
        AppWindow.Closing += (_, _) => LowLevelKeyboard.KeyPressed -= OnKeyPressed;
        this.Hide();
    }

    private void OnKeyPressed(KBDLLHOOKSTRUCT eventDetails, KeyboardState state, ref bool Handled)
    {
        if (state == KeyboardState.KeyDown)
        {
            if (eventDetails.vkCode is (uint)VIRTUAL_KEY.VK_TAB && AppWindow.IsVisible)
            {
				Handled = true; //don't pass the tab through
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
