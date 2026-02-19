using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using UnitedSets.Mvvm.Services;
using Windows.Graphics;
using WinUIEx;
using WinWrapper;
using WinWrapper.Input;

namespace UnitedSets.UI.Popups;

public sealed partial class AddTabPopup
{
    public WindowEx Result;

    public AddTabPopup()
    {
        UnitedSetsApp.Current.RegisterUnitedSetsWindow(WindowEx.FromWindowHandle((nint)AppWindow.Id.Value));
        InitializeComponent();
        LowLevelKeyboard.KeyPressed += OnKeyPressed;
        this.SetForegroundWindow();
        this.CenterOnScreen();
        AppWindow.Move(new PointInt32(AppWindow.Position.X, 80));
        AppWindow.Closing += (_, _) => LowLevelKeyboard.KeyPressed -= OnKeyPressed;
        SystemBackdrop = new InfiniteSystemBackdrop<MicaController>();
        this.Hide();
    }

    private void OnKeyPressed(KeyboardHookInfo eventDetails, KeyboardState state, ref bool Handled)
    {
        if (state == KeyboardState.KeyDown)
        {
            if (eventDetails.KeyCode is VirtualKey.Tab or VirtualKey.ESCAPE && AppWindow.IsVisible)
            {
				Handled = true; //don't pass the tab through
                Result = WindowEx.GetWindowFromPoint(Cursor.Position);
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
