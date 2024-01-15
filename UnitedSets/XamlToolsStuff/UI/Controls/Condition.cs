using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Get.XAMLTools;

/// <summary>
/// Creates a Condition Element, to aply template on True or False
/// </summary>
[DependencyProperty<bool>("Value",
    GenerateLocalOnPropertyChangedMethod = true,
    LocalOnPropertyChangedMethodName = "UpdateTemplate",
    LocalOnPropertyChangedMethodWithParameter = false,
    Documentation =
    """
    /// <summary>
    /// Gets or Set the Value of the condition and update the template
    /// </summary>
    """
)]
[DependencyProperty<DataTemplate>("OnTrue",
    GenerateLocalOnPropertyChangedMethod = true,
    LocalOnPropertyChangedMethodName = "UpdateTemplate",
    LocalOnPropertyChangedMethodWithParameter = false,
    Documentation =
    """
    /// <summary>
    /// Gets or Set the <see cref="DataTemplate"/> to use when <see cref="Value"/> is true.
    /// </summary>
    """
)]
[DependencyProperty<DataTemplate>("OnFalse",
    GenerateLocalOnPropertyChangedMethod = true,
    LocalOnPropertyChangedMethodName = "UpdateTemplate",
    LocalOnPropertyChangedMethodWithParameter = false,
    Documentation =
    """
    /// <summary>
    /// Gets or Set the <see cref="DataTemplate"/> to use when <see cref="Value"/> is false.
    /// </summary>
    """
)]
public partial class Condition : ContentControl
{
    void UpdateTemplate()
    {
        var NewTempalte = Value ? OnTrue : OnFalse;
        if (ContentTemplate != NewTempalte)
            ContentTemplate = NewTempalte;
    }
}