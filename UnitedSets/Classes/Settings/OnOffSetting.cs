namespace UnitedSets.Classes.Settings;

public partial class OnOffSetting : Setting<bool>
{
    public OnOffSetting(string Key) : base(Key)
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