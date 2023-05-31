using System;
using Microsoft.UI.Dispatching;
using Microsoft.Toolkit.Uwp;
using System.Threading.Tasks;

namespace WinUI3HwndHostPlus;

partial class HwndHost
{
    public partial async Task DetachAndDispose(bool Focus)
    {

        var WindowToHost = _HostedWindow;
        await UIDispatcher.EnqueueAsync(async () => {

            WindowToHost.Region = InitialRegion;
            ActivateCrop = false;
            BorderlessWindow = false;

            WindowToHost.Owner = default;
            WindowToHost.IsResizable = InitialIsResizable;
            WindowToHost.IsVisible = true;
            Dispose();

            WindowToHost.Focus();
            WindowToHost.Redraw();
            WindowToHost.SetAsForegroundWindow();
            await Task.Delay(100).ContinueWith(_ => WindowToHost.Redraw());
        });
    }

    public partial void FocusWindow() => _HostedWindow.Focus();
    public partial void Dispose()
    {
        IsDisposed = true;

        DispatcherQueue.EnqueueAsync(() => {
            SizeChanged -= WinUIAppWindowChanged;
            WinUIAppWindow.Changed -= WinUIAppWindowChanged;
            UnregisterPropertyChangedCallback(VisibilityProperty, VisiblePropertyChangedToken);
            Closed?.Invoke();
            GC.SuppressFinalize(this);
        });
    }
    public partial void ForceUpdateWindow() => OnWindowUpdate();
}
