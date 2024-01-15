namespace Get.XAMLTools.Classes.Settings.Boolean;

public abstract partial class BooleanSetting : Setting<bool>
{
    public BooleanSetting(string Key) : base(Key)
    {
    }

    protected override bool TransformValue(object? savedObj)
    {
        if (savedObj is bool val) return val;
        return DefaultValue;
    }
    protected override object TransformValue(bool input)
        => input;
}