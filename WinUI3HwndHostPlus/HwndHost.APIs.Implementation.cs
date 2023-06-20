using System;
using Microsoft.UI.Dispatching;
using Microsoft.Toolkit.Uwp;
using System.Threading.Tasks;

namespace WinUI3HwndHostPlus;

partial class HwndHost
{
    public partial async Task DetachAndDispose(bool Focus)
    {

        var HostedWindow = WindowInfo.HostedWindow;
        await WindowInfo.XAMLWindowDispatcherQueue.EnqueueAsync(async () => {

            HostedWindow.Region = WindowInitialCondition.InitialRegion;
            ActivateCrop = false;
            BorderlessWindow = false;

            HostedWindow.Owner = default;
            HostedWindow.IsResizable = WindowInitialCondition.InitialIsResizable;
            HostedWindow.IsVisible = true;
            Dispose();

            HostedWindow.Focus();
            HostedWindow.Redraw();
            HostedWindow.SetAsForegroundWindow();
            await Task.Delay(100).ContinueWith(_ => HostedWindow.Redraw());
        });
    }

    public partial void FocusWindow() => WindowInfo.HostedWindow.Focus();
    public partial void Dispose()
    {
        IsDisposed = true;

        DispatcherQueue.EnqueueAsync(() => {
            SizeChanged -= WinUIAppWindowChanged;
            WindowInfo.XAMLAppWindow.Changed -= WinUIAppWindowChanged;
            UnregisterPropertyChangedCallback(VisibilityProperty, VisiblePropertyChangedToken);
            Closed?.Invoke();
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        });
    }
    public partial void ForceUpdateWindow() => OnWindowUpdate();
}
