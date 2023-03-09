using System;
using Microsoft.UI.Dispatching;
using System.Runtime.CompilerServices;
using Windows.UI.Core;
using Microsoft.Toolkit.Uwp;
using System.Threading.Tasks;

namespace WinUI3HwndHostPlus;

partial class HwndHost
{
    public async Task DetachAndDispose(bool Focus = true)
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

    public void FocusWindow() => _HostedWindow.Focus();

    public event Action? Closed;
    public event Action? Updating;

    public void Dispose()
    {
        IsDisposed = true;

		DispatcherQueue.EnqueueAsync(()=> {
			SizeChanged -= WinUIAppWindowChanged;
			WinUIAppWindow.Changed -= WinUIAppWindowChanged;
			UnregisterPropertyChangedCallback(VisibilityProperty, VisiblePropertyChangedToken);
			Closed?.Invoke();
			GC.SuppressFinalize(this);
		});
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ForceUpdateWindow() => OnWindowUpdate();
}
