using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnitedSets.Classes.Settings;

public interface ISelectSetting
{
    IEnumerable<(object obj, string Description)> ValidOptions { get; }
    object Value { get; set; }
}
public partial class SelectSetting<TEnum> : Setting<TEnum>, ISelectSetting where TEnum : struct, Enum
{
    public IEnumerable<(TEnum value, string Description)> ValidOptions { get; }

    IEnumerable<(object obj, string Description)> ISelectSetting.ValidOptions =>
        (IEnumerable<(object obj, string Description)>)ValidOptions;

    object ISelectSetting.Value { get => Value; set => Value = (TEnum)value; }

    public SelectSetting(string Key, IEnumerable<TEnum> ValidOptions) : this(Key, ValidOptions.Select(x => (x, x.ToString())))
    {
        
    }
    public SelectSetting(string Key, IEnumerable<(TEnum Value, string Description)> ValidOptions) : base(Key)
    {
        this.ValidOptions = ValidOptions;
    }

    protected override TEnum TransformValue(object? savedObj)
    {
        if (savedObj is string s && Enum.TryParse<TEnum>(s, true, out var @enum))
            return @enum;
        return DefaultValue;
    }
    protected override object TransformValue(TEnum input)
        => Enum.GetName(input) ?? "";
}