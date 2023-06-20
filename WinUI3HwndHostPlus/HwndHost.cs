using Microsoft.UI.Xaml;
using System;
using Microsoft.UI.Windowing;
using System.Drawing;
using WindowEx = WinWrapper.Windowing.Window;
using Microsoft.UI.Dispatching;
using System.ComponentModel;
using WinWrapper;
using WinWrapper.Windowing;
using Window = Microsoft.UI.Xaml.Window;
using WinWrapper.Windowing.Dwm;

namespace WinUI3HwndHostPlus;

public partial class HwndHost : FrameworkElement, IDisposable, INotifyPropertyChanged
{
    readonly WindowInformation WindowInfo;

    public HwndHost(Window XAMLWindow, WindowEx WindowToHost)
    {
        WindowInfo = new(
            XAMLWindow: XAMLWindow,
            XAMLAppWindow: XAMLWindow.AppWindow,
            XAMLWindowDispatcherQueue: XAMLWindow.DispatcherQueue,
            XAMLWin32Window: WindowEx.FromWindowHandle((nint)XAMLWindow.AppWindow.Id.Value),
            HostedWindow: WindowToHost
        );
        WindowInitialCondition = new(
            Style: WindowToHost.Style,
            ExStyle: WindowToHost.ExStyle,
            Region: WindowToHost.Region,
            IsResizable: WindowToHost.IsResizable,
            SystemBackdrop: WindowToHost.DwmAttribute.SystemBackdrop
        );
        
        WindowToHost.Owner = WindowInfo.XAMLWin32Window;

        CompatabilityMode = new(
            NoMovingMode: WindowToHost.Class.Name is "RAIL_WINDOW",
            NoOwnerMode: WindowToHost.Owner != WindowInfo.XAMLWin32Window
        );

        WindowInfo.XAMLAppWindow.Changed += WinUIAppWindowChanged;
        SizeChanged += WinUIAppWindowChanged;
        VisiblePropertyChangedToken = RegisterPropertyChangedCallback(VisibilityProperty, OnPropChanged);
        AddHwndHost(this);
    }

    // Initial State
    readonly WindowInitialCondition WindowInitialCondition;
    // Compatability Mode
    public readonly HwndHostCompatability CompatabilityMode;
    // For Disposing Logic
    readonly long VisiblePropertyChangedToken;

}
public record class HwndHostCompatability(bool NoMovingMode, bool NoOwnerMode)
{
    public bool IsInCompatabilityMode { get; init; } = NoOwnerMode || NoMovingMode;
    public bool CanActivateCrop => !NoMovingMode;
}
record class WindowInitialCondition(
    WindowStyles Style,
    WindowExStyles ExStyle,
    Rectangle? Region,
    SystemBackdropTypes SystemBackdrop,
    bool IsResizable);
record class WindowInformation(
    Window XAMLWindow,
    AppWindow XAMLAppWindow,
    DispatcherQueue XAMLWindowDispatcherQueue,
    WindowEx XAMLWin32Window,
    WindowEx HostedWindow
);