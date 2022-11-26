using System;
using Microsoft.UI.Dispatching;
using System.Runtime.CompilerServices;

namespace WinUI3HwndHostPlus;

partial class HwndHost
{
    public void DetachAndDispose()
    {
        Dispose();
        var WindowToHost = this._HostedWindow;
        BorderlessWindow = false;
        WindowToHost.Region = InitialRegion;
        ActivateCrop = false;
        WindowToHost.Owner = default;
        WindowToHost.IsResizable = InitialIsResizable;
        WindowToHost.IsVisible = true;
    }

    public void FocusWindow() => _HostedWindow.Focus();

    public event Action? Closed;
    public event Action? Updating;

    public void Dispose()
    {
        IsDisposed = true;
        DispatcherQueue.TryEnqueue(delegate
        {
            SizeChanged -= WinUIAppWindowChanged;
            WinUIAppWindow.Changed -= WinUIAppWindowChanged;
            UnregisterPropertyChangedCallback(VisibilityProperty, VisiblePropertyChangedToken);
            Closed?.Invoke();
            GC.SuppressFinalize(this);
            return;
        });
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ForceUpdateWindow() => OnWindowUpdate();
}
