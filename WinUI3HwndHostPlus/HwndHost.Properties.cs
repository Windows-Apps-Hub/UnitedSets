using System;
using WindowEx = WinWrapper.Windowing.Window;
using System.Threading.Tasks;
using Windows.Win32;
using EasyCSharp;
using WinWrapper.Windowing;
using WinWrapper;

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
            WindowToHost.DwmAttribute.Set(DwmWindowAttribute.SystemBackdropTypes, DwmSystemBackdropType.None);
            WindowToHost.ExStyle |= WindowExStyles.Transparent;
        } else
        {
            WindowToHost.DwmAttribute.Set(DwmWindowAttribute.SystemBackdropTypes, InitialBackdropType);
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
            WindowToHost.Style = WindowStyles.Popup;
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
