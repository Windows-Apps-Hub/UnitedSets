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
namespace WinUI3HwndHostPlus;
partial class HwndHost
{
    [Property(SetVisibility = GeneratorVisibility.DoNotGenerate)]
    float _CacheXFromWindow, _CacheYFromWindow, _CacheWidth, _CacheHeight;

    [Property(SetVisibility = GeneratorVisibility.DoNotGenerate)]
    readonly WindowEx _ParentWindow;

    [Property(SetVisibility = GeneratorVisibility.DoNotGenerate)]
    readonly WindowEx _HostedWindow;

    [Property(OnChanged = nameof(IsWindowVisibleChanged))]
    bool _IsWindowVisible;
    void IsWindowVisibleChanged()
        => Task.Run(ForceUpdateWindow);

    bool ActivateCropAllowed => !_NoMovingMode;
    [AutoNotifyProperty(OnChanged = nameof(OnActivateCropChanged))]
    bool _ActivateCrop = false;
    void OnActivateCropChanged()
    {
        if (!ActivateCropAllowed && _ActivateCrop) throw new InvalidOperationException($"{nameof(ActivateCropAllowed)} is false");
        if (_ActivateCrop)
        {
            if (IsDwmBackdropSupported && !IsDisposed)
                SetBackdrop = true;
        }
        else
            if (SetBackdrop)
                SetBackdrop = false;
    }
    [Property(Visibility = GeneratorVisibility.Private, OnChanged = nameof(OnSetBackdropChange))]
    bool _SetBackdrop = false;
    void OnSetBackdropChange()
    {
        var WindowToHost = this._HostedWindow;
        if (SetBackdrop)
        {
            WindowToHost.DwmSetWindowAttribute((DWMWINDOWATTRIBUTE)38, DWM_SYSTEMBACKDROP_TYPE.DWMSBT_NONE);
            WindowToHost.ExStyle |= WINDOW_EX_STYLE.WS_EX_TRANSPARENT;
        } else
        {
            WindowToHost.DwmSetWindowAttribute((DWMWINDOWATTRIBUTE)38, InitialBackdropType);
            WindowToHost.ExStyle = InitialExStyle;
            WindowToHost.Region = null;
        }
    }

    [AutoNotifyProperty(OnChanged = nameof(OnBorderlessWindowChanged))]
    bool _BorderlessWindow = false;
    void OnBorderlessWindowChanged()
    {
        var WindowToHost = this._HostedWindow;
        if (_BorderlessWindow && !IsDisposed)
        {
            WindowToHost.Style = WINDOW_STYLE.WS_POPUP;
        }
        else
        {
            WindowToHost.Style = InitialStyle;
        }
    }

    [Property(Visibility = GeneratorVisibility.Private)]
    bool _ForceInvalidateCrop = false;

    [AutoNotifyProperty(OnChanged = nameof(SetForceInvalidateCropToTrue))]
    int _CropTop = 0, _CropBottom = 0, _CropLeft = 0, _CropRight = 0;

    void SetForceInvalidateCropToTrue() => ForceInvalidateCrop = true;

    public bool IsDisposed { get; private set; }
}
