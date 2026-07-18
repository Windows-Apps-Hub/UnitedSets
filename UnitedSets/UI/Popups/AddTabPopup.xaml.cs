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
    TaskCompletionSource<WindowEx>? _completion;

    public AddTabPopup()
    {
        UnitedSetsApp.Current.RegisterUnitedSetsWindow(WindowEx.FromWindowHandle((nint)AppWindow.Id.Value));
        InitializeComponent();
        this.CenterOnScreen();
        AppWindow.Move(new PointInt32(AppWindow.Position.X, 80));
        AppWindow.Closing += (_, _) => Complete(default);
        SystemBackdrop = new InfiniteSystemBackdrop<MicaController>();
        this.Hide();
    }

    private void OnKeyPressed(KeyboardHookInfo eventDetails, KeyboardState state, ref bool Handled)
    {
        if (state == KeyboardState.KeyDown)
        {
            if (eventDetails.KeyCode is VirtualKey.Tab or VirtualKey.ESCAPE && _completion is not null)
            {
                Handled = true;
                var result = eventDetails.KeyCode == VirtualKey.ESCAPE
                    ? default
                    : WindowEx.GetWindowFromPoint(Cursor.Position);
                DispatcherQueue.TryEnqueue(() => Complete(result));
            }
        }
    }

    public Task<WindowEx> ShowAsync()
    {
        if (_completion is not null)
            return _completion.Task;

        Result = default;
        _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        LowLevelKeyboard.KeyPressed += OnKeyPressed;
        this.CenterOnScreen();
        AppWindow.Move(new PointInt32(AppWindow.Position.X, 80));
        AppWindow.Show();
        this.SetForegroundWindow();
        return _completion.Task;
    }

    void Complete(WindowEx result)
    {
        var completion = _completion;
        if (completion is null) return;

        _completion = null;
        Result = result;
        LowLevelKeyboard.KeyPressed -= OnKeyPressed;
        this.Hide();
        completion.TrySetResult(result);
    }

    [Event(typeof(RoutedEventHandler))]
    private void CancelClick() => Complete(default);
}
