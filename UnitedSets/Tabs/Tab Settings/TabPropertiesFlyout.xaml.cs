using EasyCSharp;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using UnitedSets.Helpers;
using WindowEx = WinWrapper.Window;
using static WinUIEx.WindowExtensions;
using Windows.Foundation;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UnitedSets;

public sealed partial class TabPropertiesFlyout
{
    readonly WindowEx CurrentWindowEx;
    public TabPropertiesFlyout(WindowEx ParentWindow, params UIElement[] Modules)
    {
        InitializeComponent();
        foreach (var Module in Modules)
            ModuleContainer.Children.Add(Module);
        CurrentWindowEx = WindowEx.FromWindowHandle(
            WinRT.Interop.WindowNative.GetWindowHandle(this)
        );
        this.SetForegroundWindow();
        var parentbounds = ParentWindow.Bounds;
        CurrentWindowEx.Bounds = CurrentWindowEx.Bounds with
        {
            X = Math.Max(10, parentbounds.X - 455),
            Y = parentbounds.Y
        };
        Activated += OnActivatedChanged;
    }
    TaskCompletionSource? ShowTaskCompletion;

    [Event(typeof(TypedEventHandler<object, WindowActivatedEventArgs>))]
    void OnActivatedChanged(WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState == WindowActivationState.Deactivated)
        {
            ShowTaskCompletion?.SetResult();
            ShowTaskCompletion = null;
        }
    }

    public async Task ShowAsync()
    {
        AppWindow.Show();
        await Task.Delay(100);
        Activate();
        ShowTaskCompletion ??= new();
        await ShowTaskCompletion.Task;
    }

    [Event(typeof(RoutedEventHandler))]
    void CloseClick() => Close();
}
