using Get.EasyCSharp;
using WinWrapper.Windowing;
using System;
using System.ComponentModel;
using WinWrapper;
using System.Threading.Tasks;
namespace WindowHoster;

partial class RegisteredWindow
{
    public RegisteredWindowProperties Properties { get; }
}

public partial class RegisteredWindowProperties : INotifyPropertyChanged
{
    RegisteredWindow self;
    internal RegisteredWindowProperties(RegisteredWindow self)
    {
        this.self = self;
    }
    [AutoNotifyProperty(OnChanged = nameof(CropRegionChanged))]
    CropRegion _CropRegion;
    void CropRegionChanged()
    {
        ForceInvalidateCrop = true;
        self.CurrentController?.UpdatePosition();
    }
    internal bool ForceInvalidateCrop { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;


    [AutoNotifyProperty(OnChanged = nameof(OnActivateCropChanged))]
    bool _ActivateCrop = false;
    void OnActivateCropChanged()
    {
        if (!self.CompatablityMode.IsActivateCropAllowed && _ActivateCrop)
            throw new InvalidOperationException($"The current window has a compatability mode that does not allow cropping window");
        if (_ActivateCrop)
        {
            if (CompatablityMode.IsDwmBackdropSupported && self.IsValid)
                SetBackdrop = true;
        }
        else
            if (SetBackdrop)
                SetBackdrop = false;
    }

    [AutoNotifyProperty(OnChanged = nameof(OnBorderlessWindowChanged))]
    bool _BorderlessWindow = false;
    void OnBorderlessWindowChanged()
    {
        var window = self.Window;
        if (_BorderlessWindow && self.IsValid)
        {
            window.Hide();
            window.Style = WindowStyles.Popup;
            self.CurrentController?.UpdatePosition();
            window.Show();
        }
        else
        {
            window.Style = self.InitalStylingState.Style;
        }
    }
    // private
    [Property(Visibility = GeneratorVisibility.Private, OnChanged = nameof(OnSetBackdropChange))]
    bool _SetBackdrop = false;
    void OnSetBackdropChange()
    {
        var window = self.Window;
        if (SetBackdrop)
        {
            window.DwmAttribute.Set(DwmWindowAttribute.SystemBackdropTypes, DwmSystemBackdropType.None);
            window.ExStyle |= WindowExStyles.Transparent;
        }
        else
        {
            window.DwmAttribute.Set(DwmWindowAttribute.SystemBackdropTypes, self.InitalStylingState.Backdrop);
            window.ExStyle = self.InitalStylingState.ExStyles;
            window.Region = null;
        }
    }
}
public record struct CropRegion(int Top, int Left, int Right, int Bottom);
