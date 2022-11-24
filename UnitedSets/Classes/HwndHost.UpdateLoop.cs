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

    int CountDown = 5;
    async void OnWindowUpdate()
    {
        if (_CacheWidth == 0 || _CacheHeight == 0) return; // wait for update
        if (IsDisposed) return;

        var WindowToHost = this._HostedWindow;

        if (!IsWindowVisible)
        {
            WindowToHost.IsVisible = false;
            return;
        }

        bool Check = false;
        if (CountDown > 0)
        {
            CountDown--;
            if (CountDown == 0) WindowToHost.Redraw();
        }
        else Check = true;
        var windowbounds = _ParentWindow.Bounds;

        var scale = GetScale(_ParentWindow);
        var Pt = new System.Drawing.Point
        {
            X = (int)(windowbounds.X + _CacheXFromWindow * scale),
            Y = (int)(windowbounds.Y + _CacheYFromWindow * scale)
        };


        try
        {
            WindowToHost.IsResizable = false;
        }
        catch
        {

        }
        if (!WindowToHost.IsValid)
        {
            Dispose();
            return;
        }
        Updating?.Invoke();
        var YShift = _ParentWindow.IsMaximized ? 8 : 0;
        var oldBounds = WindowToHost.Bounds;
        var newBounds = new Rectangle(
        Pt.X + 8 - _CropLeft,
        Pt.Y + YShift - _CropTop,
        (int)(_CacheWidth * scale) + _CropLeft + _CropRight,
        (int)(_CacheHeight * scale) + _CropTop + _CropBottom
        );
        if (oldBounds != newBounds)
        {
            if (Check && WindowEx.ForegroundWindow == WindowToHost)
            {
                DetachAndDispose();
                return;
            }
            else WindowToHost.Bounds = newBounds;
            if (ActivateCrop)
                if (ForceInvalidateCrop || oldBounds.Size != newBounds.Size)
                {
                    WindowToHost.Region = new(_CropLeft, _CropTop, WindowToHost.Bounds.Width - _CropLeft - _CropRight, WindowToHost.Bounds.Height - _CropTop - _CropBottom);
                }
        }
        if (!IsOwnerSetSuccessful)
        {
            if (new WindowRelative(WindowToHost).GetAboves().Take(10).Any(x => x == _ParentWindow))
            {
                await Task.Delay(500);
                if (oldBounds == WindowToHost.Bounds && IsWindowVisible)
                {
                    WindowToHost.ActivateTopMost();
                    WindowToHost.Focus();
                }
            }
        }
        WindowToHost.IsVisible = true;
    }
}
