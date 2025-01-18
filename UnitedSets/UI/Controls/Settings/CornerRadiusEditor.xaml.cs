using CommunityToolkit.WinUI;
using Get.XAMLTools;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using UnitedSets.Settings;

namespace UnitedSets.UI.Controls;
[DependencyProperty(
    typeof(CornerRadius),
    "Value",
    UseNullableReferenceType = true,
    GenerateLocalOnPropertyChangedMethod = true
)]
public partial class CornerRadiusEditor
{
    public CornerRadiusEditor()
    {
        suppressUpdate = true;
        InitializeComponent();
        suppressUpdate = false;
        //SetAlign(tl, TextAlignment.Left);
        //SetAlign(bl, TextAlignment.Left);
        //SetAlign(tr, TextAlignment.Right);
        //SetAlign(br, TextAlignment.Right);
    }
    bool suppressUpdate = false;
    partial void OnValueChanged(CornerRadius oldValue, CornerRadius newValue)
    {
        suppressUpdate = true;
        tl.Value = newValue.TopLeft;
        tr.Value = newValue.TopRight;
        bl.Value = newValue.BottomLeft;
        br.Value = newValue.BottomRight;
        suppressUpdate = false;
    }

    private void NumberBox_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
    {
        if (suppressUpdate) return;
        // sounds like a typo but this one works
        Value = new(tl.Value, tr.Value, bl.Value, br.Value);
    }

    private void confirmBtn_Click(object sender, RoutedEventArgs e)
    {
        flyout.Hide();
    }
    static void SetAlign(NumberBox nb, TextAlignment align)
    {
        if (nb.IsLoaded)
        {
            var InputBox = nb.FindDescendant<TextBox>(tb => tb.Name is "InputBox")
                ?? throw new System.InvalidOperationException("Wait for the component to be initialized first");
            InputBox.TextAlignment = align;
        }
        else
        {
            nb.Loaded += delegate
            {
                SetAlign(nb, align);
            };
        }
    }
}
