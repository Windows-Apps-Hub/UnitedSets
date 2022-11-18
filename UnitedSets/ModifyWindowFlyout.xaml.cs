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

namespace UnitedSets
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModifyWindowFlyout : WinUIEx.WindowEx
    {
        readonly HwndHost HwndHost;
        WindowEx CurrentWindowEx;
        public ModifyWindowFlyout(HwndHost hwndHost)
        {
            HwndHost = hwndHost;
            InitializeComponent();
            CurrentWindowEx = WindowEx.FromWindowHandle(
                WinRT.Interop.WindowNative.GetWindowHandle(this)
            );
            MicaHelper Mica = new();
            Mica.TrySetMicaBackdrop(this);
            this.SetForegroundWindow();
            var ParentWindow = HwndHost.ParentWindow;
            var parentbounds = ParentWindow.Bounds;
            CurrentWindowEx.Bounds = CurrentWindowEx.Bounds with
            {
                X = Math.Max(10, parentbounds.X - 405),
                Y = parentbounds.Y
            };
            WindowCropMarginToggleSwitch_Toggled(null, null);
            BorderlessToggleSwitch_Toggled(null, null);
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

        private void TopMarginShortcutClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                TopCropMargin.Value = double.Parse(btn.Content.ToString() ?? "0");
            }
        }

        private void TopCropMargin_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            HwndHost.CropTop = (int)TopCropMargin.Value;
        }

        private void LeftCropMargin_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            HwndHost.CropLeft = (int)LeftCropMargin.Value;
        }

        private void RightCropMargin_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            HwndHost.CropRight = (int)RightCropMargin.Value;
        }

        private void BottomCropMargin_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            HwndHost.CropBottom = (int)BottomCropMargin.Value;
        }

        private void WindowCropMarginToggleSwitch_Toggled(object? sender, RoutedEventArgs? e)
        {
            HwndHost.ActivateCrop = WindowCropMarginToggleSwitch.IsOn;
            WindowCropMarginSettingsStackPanel.Visibility = WindowCropMarginToggleSwitch.IsOn ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BorderlessToggleSwitch_Toggled(object? sender, RoutedEventArgs? e)
        {
            HwndHost.BorderlessWindow = BorderlessToggleSwitch.IsOn;
            if (!BorderlessToggleSwitch.IsOn)
                WindowCropMarginToggleSwitch.IsOn = false;
            BorderlessSettingsStackPanel.Visibility = BorderlessToggleSwitch.IsOn ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ResetClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is NumberBox nbb)
            {
                nbb.Value = 0;
            }
        }

        private void DrawClick(object sender, RoutedEventArgs e)
        {
            HwndHost.HostedWindow.Redraw();
        }
    }
}
