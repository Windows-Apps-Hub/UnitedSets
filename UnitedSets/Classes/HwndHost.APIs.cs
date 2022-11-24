using Microsoft.UI.Xaml;
using System;
using Microsoft.UI.Windowing;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using Microsoft.UI.Dispatching;
using Window = Microsoft.UI.Xaml.Window;
using WindowEx = WinWrapper.Window;
using WinWrapper;
using Windows.Win32.UI.WindowsAndMessaging;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Windows.Win32;
using Windows.Win32.Graphics.Dwm;
using Windows.Foundation;
using EasyCSharp;
using System.Runtime.CompilerServices;

namespace UnitedSets.Classes;

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
