using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UnitedSets.Classes;
using UnitedSets.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.Win32;
using WindowEx = WinWrapper.Window;
using static WinUIEx.WindowExtensions;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UnitedSets;

public sealed partial class TabPropertiesFlyout : WinUIEx.WindowEx
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
        MicaHelper Mica = new();
        Mica.TrySetMicaBackdrop(this);
        this.SetForegroundWindow();
        var parentbounds = ParentWindow.Bounds;
        CurrentWindowEx.Bounds = CurrentWindowEx.Bounds with
        {
            X = Math.Max(10, parentbounds.X - 405),
            Y = parentbounds.Y
        };
        Activated += ThisActivated;
    }
    TaskCompletionSource? ShowTaskCompletion;
    private void ThisActivated(object sender, WindowActivatedEventArgs args)
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
    private void CloseClick(object sender, RoutedEventArgs e) => Close();
}
