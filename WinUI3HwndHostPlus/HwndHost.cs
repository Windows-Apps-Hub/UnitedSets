using Microsoft.UI.Xaml;
using System;
using Microsoft.UI.Windowing;
using System.Drawing;
using WindowEx = WinWrapper.Windowing.Window;
using EasyCSharp;
using Microsoft.UI.Dispatching;
using System.ComponentModel;
using WinWrapper;
using WinWrapper.Windowing;
using Window = Microsoft.UI.Xaml.Window;

namespace WinUI3HwndHostPlus;

public partial class HwndHost : FrameworkElement, IDisposable, INotifyPropertyChanged
{
    readonly Window XAMLWindow;

	private DispatcherQueue UIDispatcher;

	readonly AppWindow WinUIAppWindow;
    
    public HwndHost(Window XAMLWindow, WindowEx WindowToHost)
    {
        InitialStyle = WindowToHost.Style;
        InitialExStyle = WindowToHost.ExStyle;
        InitialRegion = WindowToHost.Region;
        
        this.XAMLWindow = XAMLWindow;
		
		this.UIDispatcher = XAMLWindow.DispatcherQueue;//caching incase our window dies


		var WinUIHandle = WinRT.Interop.WindowNative.GetWindowHandle(XAMLWindow);
        _ParentWindow = WindowEx.FromWindowHandle(WinUIHandle);
        WinUIAppWindow = AppWindow.GetFromWindowId(
            Microsoft.UI.Win32Interop.GetWindowIdFromWindow(
                WinUIHandle
            )
        );
        
        this._HostedWindow = WindowToHost;

        _NoMovingMode = WindowToHost.Class.Name is "RAIL_WINDOW";

        InitialIsResizable = WindowToHost.IsResizable;
        
        WindowToHost.Owner = _ParentWindow;
        _IsOwnerSetSuccessful = WindowToHost.Owner == _ParentWindow;
        if (IsDwmBackdropSupported)
            InitialBackdropType = WindowToHost.DwmAttribute.Get<DwmSystemBackdropType>(
                DwmWindowAttribute.SystemBackdropTypes
            );

        //if (!IsOwnerSetSuccessful) WindowToHost.ExStyle |= WINDOW_EX_STYLE.WS_EX_TOOLWINDOW;
        WinUIAppWindow.Changed += WinUIAppWindowChanged;
        SizeChanged += WinUIAppWindowChanged;

        VisiblePropertyChangedToken = RegisterPropertyChangedCallback(VisibilityProperty, OnPropChanged);
        AddHwndHost(this);
    }

    // Initial State
    readonly WindowStyles InitialStyle;
    readonly Rectangle? InitialRegion;
    readonly DwmSystemBackdropType InitialBackdropType;
    readonly WindowExStyles InitialExStyle;
    readonly bool InitialIsResizable;
    // Compatability Mode
    [Property(SetVisibility = GeneratorVisibility.DoNotGenerate)]
    readonly bool _NoMovingMode;
    [Property(SetVisibility = GeneratorVisibility.DoNotGenerate)]
    readonly bool _IsOwnerSetSuccessful;
    bool IsInCompatabilityMode => _NoMovingMode || !_IsOwnerSetSuccessful;
    // For Disposing Logic
    readonly long VisiblePropertyChangedToken;

    public event PropertyChangedEventHandler? PropertyChanged;
}
