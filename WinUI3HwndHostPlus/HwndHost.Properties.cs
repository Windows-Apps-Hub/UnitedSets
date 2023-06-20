using System;
using WindowEx = WinWrapper.Windowing.Window;
using System.Threading.Tasks;
using Windows.Win32;
using EasyCSharp;
using EasyXAMLTools;
using WinWrapper.Windowing;
using WinWrapper;
using System.Drawing;
using WinWrapper.Windowing.Dwm;

namespace WinUI3HwndHostPlus;
[DependencyProperty(typeof(bool), "ActivateCrop", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(bool), "BorderlessWindow", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(int), "CropTop",
    GenerateLocalOnPropertyChangedMethod = true,
    LocalOnPropertyChangedMethodWithParameter = false,
    LocalOnPropertyChangedMethodName = "CropParamChanged"
)]
[DependencyProperty(typeof(int), "CropBottom",
    GenerateLocalOnPropertyChangedMethod = true,
    LocalOnPropertyChangedMethodWithParameter = false,
    LocalOnPropertyChangedMethodName = "CropParamChanged"
)]
[DependencyProperty(typeof(int), "CropLeft",
    GenerateLocalOnPropertyChangedMethod = true,
    LocalOnPropertyChangedMethodWithParameter = false,
    LocalOnPropertyChangedMethodName = "CropParamChanged"
)]
[DependencyProperty(typeof(int), "CropRight",
    GenerateLocalOnPropertyChangedMethod = true,
    LocalOnPropertyChangedMethodWithParameter = false,
    LocalOnPropertyChangedMethodName = "CropParamChanged"
)]
partial class HwndHost
{
    [Property(SetVisibility = GeneratorVisibility.DoNotGenerate)]
    RectangleF _CacheWindowRect;

    //[Property(OnChanged = nameof(IsWindowVisibleChanged))]
    bool IsWindowVisible;
    void IsWindowVisibleChanged()
        => Task.Run(ForceUpdateWindow);

    partial void OnActivateCropChanged(bool oldValue, bool newValue)
    {
        if (!CompatabilityMode.CanActivateCrop
            && newValue
        ) throw new InvalidOperationException($"Cannot activate crop due to compatability mode");
        if (newValue)
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
        var HostedWindow = WindowInfo.HostedWindow;
        if (SetBackdrop)
        {
            HostedWindow.DwmAttribute.SystemBackdrop = SystemBackdropTypes.None;
            HostedWindow.ExStyle |= WindowExStyles.Transparent;
        } else
        {
            HostedWindow.DwmAttribute.SystemBackdrop = WindowInitialCondition.SystemBackdrop;
            HostedWindow.ExStyle = WindowInitialCondition.ExStyle;
            HostedWindow.Region = null;
        }
    }

    partial void OnBorderlessWindowChanged(bool oldValue, bool newValue)
    {
        var HostedWindow = WindowInfo.HostedWindow;
        if (newValue && !IsDisposed)
        {
            HostedWindow.Style = WindowStyles.Popup;
        }
        else
        {
            HostedWindow.Style = WindowInitialCondition.Style;
        }
    }

    void CropParamChanged() => SetForceInvalidateCropToTrue();



    bool ForceInvalidateCrop = false;

    void SetForceInvalidateCropToTrue() => ForceInvalidateCrop = true;

    public bool IsDisposed { get; private set; }
}
