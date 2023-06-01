using EasyXAMLTools;
using UnitedSets.Classes;
using UnitedSets.Classes.Settings;

namespace UnitedSets.UI.Controls;
[DependencyProperty(
    typeof(Setting),
    "Setting",
    UseNullableReferenceType = true,
    GenerateLocalOnPropertyChangedMethod = true
)]
public partial class SettingController
{
	public SettingController()
	{
		InitializeComponent();
	}

    partial void OnSettingChanged(Setting? oldValue, Setting? newValue)
    {
        ContentTemplate = newValue switch
        {
            OnOffSetting => OnOffSettingTemplate,
            ISelectSetting => SelectSettingTemplate,
            _ => null
        };
    }
}