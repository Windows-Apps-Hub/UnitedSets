using CommunityToolkit.WinUI;
using Get.XAMLTools;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UnitedSets.Settings;

namespace UnitedSets.UI.Controls;
[DependencyProperty(
    typeof(Thickness),
    "Value",
    UseNullableReferenceType = true,
    GenerateLocalOnPropertyChangedMethod = true
)]
public partial class ThicknessEditor
{
    public ThicknessEditor()
    {
        suppressUpdate = true;
        InitializeComponent();
        suppressUpdate = false;
        //SetAlign(left, TextAlignment.Left);
        //SetAlign(right, TextAlignment.Right);
        //SetAlign(top, TextAlignment.Center);
        //SetAlign(bottom, TextAlignment.Center);
    }
    bool suppressUpdate = false;
    partial void OnValueChanged(Thickness oldValue, Thickness newValue)
    {
        suppressUpdate = true;
        top.Value = newValue.Top;
        left.Value = newValue.Left;
        right.Value = newValue.Right;
        bottom.Value = newValue.Bottom;
        suppressUpdate = false;
    }

    private void NumberBox_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
    {
        if (suppressUpdate) return;
        Value = new(left.Value, top.Value, right.Value, bottom.Value);
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
        } else
        {
            nb.Loaded += delegate
            {
                SetAlign(nb, align);
            };
        }
    }
}
