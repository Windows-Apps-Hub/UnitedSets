using Get.XAMLTools.Classes.Settings;
using Get.XAMLTools.Classes.Settings.Boolean;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Get.XAMLTools.UI.Controls;
[DependencyProperty(
    typeof(Setting),
    "Setting",
    UseNullableReferenceType = true,
    GenerateLocalOnPropertyChangedMethod = true,
    LocalOnPropertyChangedMethodWithParameter = false,
    LocalOnPropertyChangedMethodName = "UpdateTemplate"
)]
[DependencyProperty(
    typeof(bool?),
    "UseBuiltInTemplate",
    GenerateLocalOnPropertyChangedMethod = true,
    LocalOnPropertyChangedMethodWithParameter = false,
    LocalOnPropertyChangedMethodName = "UpdateTemplate"
)]
[DependencyProperty(
    typeof(DataTemplateSelector),
    "AdditionalSettingTemplate",
    UseNullableReferenceType = true,
    GenerateLocalOnPropertyChangedMethod = true,
    LocalOnPropertyChangedMethodWithParameter = false,
    LocalOnPropertyChangedMethodName = "UpdateTemplate"
)]
public partial class SettingController
{
    public Style SettingCheckBoxStyle
    {
        get => (Style)Resources[nameof(SettingCheckBoxStyle)];
        set => Resources[nameof(SettingCheckBoxStyle)] = value;
    }
    public Style SettingToggleSwitchStyle
    {
        get => (Style)Resources[nameof(SettingToggleSwitchStyle)];
        set => Resources[nameof(SettingToggleSwitchStyle)] = value;
    }
    public Style SettingComboBoxStyle
    {
        get => (Style)Resources[nameof(SettingComboBoxStyle)];
        set => Resources[nameof(SettingComboBoxStyle)] = value;
    }
    public Style OnOffSettingToggleSwitchStyle
    {
        get => (Style)Resources[nameof(OnOffSettingToggleSwitchStyle)];
        set => Resources[nameof(OnOffSettingToggleSwitchStyle)] = value;
    }
    public Style CheckBoxSettingCheckBoxStyle
    {
        get => (Style)Resources[nameof(CheckBoxSettingCheckBoxStyle)];
        set => Resources[nameof(CheckBoxSettingCheckBoxStyle)] = value;
    }
    public Style SelectSettingComboBoxStyle
    {
        get => (Style)Resources[nameof(SelectSettingComboBoxStyle)];
        set => Resources[nameof(SelectSettingComboBoxStyle)] = value;
    }

    public static DataTemplateSelector? DefaultAdditionalSettingTemplate;
    public static bool DefaultUseBuiltInTempalte = true;
    public SettingController()
    {
        Resources["ToggleSwitchPreContentMargin"] = Resources["ToggleSwitchPostContentMargin"] = new GridLength(0);
        InitializeComponent();
    }

    void UpdateTemplate()
    {
        ContentTemplate = (UseBuiltInTemplate ?? DefaultUseBuiltInTempalte ?
            Setting switch
            {
                OnOffSetting => OnOffSettingTemplate,
                CheckboxSetting => CheckBoxSettingTemplate,
                ISelectSetting => SelectSettingTemplate,
                _ => null
            } :
            null
        )
        ?? AdditionalSettingTemplate?.SelectTemplate(Setting)
        ?? DefaultAdditionalSettingTemplate?.SelectTemplate(Setting)
        ?? NoTemplateSettingTemplate;
    }

    internal static ObjectSetting[] GetObjectSettings(ISelectSetting setting)
        => _GetObjectSettings(setting).ToArray();
    static IEnumerable<ObjectSetting> _GetObjectSettings(ISelectSetting setting)
    {
        foreach (var obj in setting.ValidOptions)
        {
            yield return new ObjectSetting(obj, setting);
        }
    }
    internal static int IndexOfValue(ISelectSetting setting)
    {
        return setting.ValidOptions.IndexOf(setting.Value);
    }
    internal static Action<int> SetValueFromIndex(ISelectSetting setting)
    {
        return i => setting.Value = setting.ValidOptions[i]!;
    }
}
record struct ObjectSetting(object Object, ISelectSetting Setting);