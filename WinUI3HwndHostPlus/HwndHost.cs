using Microsoft.UI.Xaml;
using System;
using Microsoft.UI.Windowing;
using System.Drawing;
using WindowEx = WinWrapper.Window;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32;
using Windows.Win32.Graphics.Dwm;

namespace WinUI3HwndHostPlus;

public partial class HwndHost : FrameworkElement, IDisposable
{
    readonly Window XAMLWindow;
    readonly AppWindow WinUIAppWindow;
    
    public HwndHost(Window XAMLWindow, WindowEx WindowToHost)
    {
        InitialStyle = WindowToHost.Style;
        InitialExStyle = WindowToHost.ExStyle;
        InitialRegion = WindowToHost.Region;
        
        this.XAMLWindow = XAMLWindow;

        var WinUIHandle = WinRT.Interop.WindowNative.GetWindowHandle(XAMLWindow);
        _ParentWindow = WindowEx.FromWindowHandle(WinUIHandle);
        WinUIAppWindow = AppWindow.GetFromWindowId(
            Microsoft.UI.Win32Interop.GetWindowIdFromWindow(
                WinUIHandle
            )
        );
        
        this._HostedWindow = WindowToHost;
        InitialIsResizable = WindowToHost.IsResizable;
        
        WindowToHost.Owner = _ParentWindow;
        IsOwnerSetSuccessful = WindowToHost.Owner == _ParentWindow;
        if (IsDwmBackdropSupported)
            InitialBackdropType = WindowToHost.DwmGetWindowAttribute<DWM_SYSTEMBACKDROP_TYPE>((DWMWINDOWATTRIBUTE)38);

        //if (!IsOwnerSetSuccessful) WindowToHost.ExStyle |= WINDOW_EX_STYLE.WS_EX_TOOLWINDOW;
        WinUIAppWindow.Changed += WinUIAppWindowChanged;
        SizeChanged += WinUIAppWindowChanged;

        VisiblePropertyChangedToken = RegisterPropertyChangedCallback(VisibilityProperty, OnPropChanged);
        AddHwndHost(this);
    }

    // Initial State
    readonly WINDOW_STYLE InitialStyle;
    readonly Rectangle? InitialRegion;
    readonly DWM_SYSTEMBACKDROP_TYPE InitialBackdropType;
    readonly WINDOW_EX_STYLE InitialExStyle;
    readonly bool InitialIsResizable;

    readonly bool IsOwnerSetSuccessful;
    
    // For Disposing Logic
    readonly long VisiblePropertyChangedToken;
}