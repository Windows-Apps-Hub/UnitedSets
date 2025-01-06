using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Text;

namespace UnitedSets.UI.FlyoutModules.TabSettings;
public sealed partial class SettingsItem : ContentControl
{
    public SettingsItem()
    {
        this.DefaultStyleKey = typeof(SettingsItem);
    }
    public string Label
    {
        get { return (string)GetValue(LabelProperty); }
        set { SetValue(LabelProperty, value); }
    }

    public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(SettingsItem), new PropertyMetadata(""));
    public FontWeight LabelWeight
    {
        get { return (FontWeight)GetValue(LabelWeightProperty); }
        set { SetValue(LabelWeightProperty, value); }
    }

    public static readonly DependencyProperty LabelWeightProperty = DependencyProperty.Register("LabelWeight", typeof(FontWeight), typeof(SettingsItem), new PropertyMetadata(FontWeights.Normal));

    public double LabelSize
    {
        get { return (double)GetValue(LabelSizeProperty); }
        set { SetValue(LabelSizeProperty, value); }
    }

    public static readonly DependencyProperty LabelSizeProperty = DependencyProperty.Register("LabelSize", typeof(double), typeof(SettingsItem), new PropertyMetadata(14));
}
