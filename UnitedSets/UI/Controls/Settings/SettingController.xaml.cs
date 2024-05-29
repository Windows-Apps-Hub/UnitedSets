using Get.XAMLTools;
using UnitedSets.Settings;

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
            TextSetting => TextSettingTemplate,
            ISelectSetting => SelectSettingTemplate,
            ITempLinkSetting => TempLinkSettingTemplate,
            _ => null
        };
    }
}