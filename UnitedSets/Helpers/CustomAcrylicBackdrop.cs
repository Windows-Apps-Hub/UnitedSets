using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using Windows.UI;
using Microsoft.UI.Composition;

namespace UnitedSets.Helpers;
public partial class CustomAcrylicBackdrop : SystemBackdrop
{
    DesktopAcrylicController Controller = new();
    // double because XAML Failed to create a 'Windows.Foundation.Single' from the text '0.6'.
    SystemBackdropTheme _theme;
    public ElementTheme Theme
    {
        set
        {
            _theme = (SystemBackdropTheme)(int)value;
            Controller.SetSystemBackdropConfiguration(new()
            {
                IsInputActive = true,
                Theme = _theme,
                IsHighContrast = false
            });
        }
    }
    public double LuminosityOpacity
    {
        get => Controller.LuminosityOpacity;
        set => Controller.LuminosityOpacity = (float)value;
    }
    public double TintOpacity
    {
        get => Controller.TintOpacity;
        set => Controller.TintOpacity = (float)value;
    }
    public Color TintColor
    {
        get => Controller.TintColor;
        set => Controller.TintColor = value;
    }
    public DesktopAcrylicKind Kind
    {
        get => Controller.Kind;
        set => Controller.Kind = value;
    }
    public Color FallbackColor
    {
        get => Controller.FallbackColor;
        set => Controller.FallbackColor = value;
    }
    protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop connectedTarget, XamlRoot xamlRoot)
    {
        Controller.SetSystemBackdropConfiguration(new()
        {
            IsInputActive = true,
            Theme = _theme,
            IsHighContrast = false
        });
        Controller.AddSystemBackdropTarget(connectedTarget);
    }
    protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop disconnectedTarget)
        => Controller.RemoveSystemBackdropTarget(disconnectedTarget);
    protected override void OnDefaultSystemBackdropConfigurationChanged(ICompositionSupportsSystemBackdrop target, XamlRoot xamlRoot)
    {
        Controller.SetSystemBackdropConfiguration(GetDefaultSystemBackdropConfiguration(target, xamlRoot));
    }
}
