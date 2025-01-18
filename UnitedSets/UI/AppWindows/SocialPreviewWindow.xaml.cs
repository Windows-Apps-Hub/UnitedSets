using Microsoft.UI.Windowing;
using WinUIEx;

namespace UnitedSets.UI.AppWindows;


public sealed partial class SocialPreviewWindow : WindowEx
{
    public SocialPreviewWindow()
    {
        InitializeComponent();
        var w = WinWrapper.Windowing.Window.FromWindowHandle((nint)AppWindow.Id.Value);
        var dwmWindowCornerPreference = (WinWrapper.Windowing.DwmWindowAttribute)33;
        var DWMWCP_DONOTROUND = 1;
        // disable rounded corner so we can take screenshot
        w.DwmAttribute.Set(dwmWindowCornerPreference, DWMWCP_DONOTROUND);
        var presenter = ((OverlappedPresenter)AppWindow.Presenter);
        ExtendsContentIntoTitleBar = true;
        presenter.IsMinimizable = false;
        presenter.IsMaximizable = false;
        presenter.SetBorderAndTitleBar(true, false);
    }
}
