using EnumsNET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Get.XAMLTools.Classes.Settings;

public interface IEnumSelectSetting : ISelectSetting
{
    new IReadOnlyList<Enum> ValidOptions { get; }
    new Enum Value { get; set; }
    string? GetDisplayName(Enum value);
    string? GetDescription(Enum value);
}
public interface ISelectSetting
{
    IReadOnlyList<object> ValidOptions { get; }
    object Value { get; set; }
    int ValueIndex { get; set; }
    string? GetDisplayName(object value);
    string? GetDescription(object value);
}
public partial class SelectSetting<TEnum> : Setting<TEnum>, IEnumSelectSetting where TEnum : struct, Enum
{
    public IReadOnlyList<TEnum> ValidOptions { get; }

    IReadOnlyList<Enum> IEnumSelectSetting.ValidOptions => ValidOptions.Cast<Enum>().ToArray();
    IReadOnlyList<object> ISelectSetting.ValidOptions => ValidOptions.Cast<object>().ToArray();

    Enum IEnumSelectSetting.Value { get => Value; set => Value = (TEnum)value; }
    object ISelectSetting.Value { get => Value; set => Value = (TEnum)(value ?? DefaultValue); }
    public int ValueIndex
    {
        get => ValidOptions.IndexOf(Value);
        set => Value = ValidOptions[value];
    }

    public SelectSetting(string Key, TEnum[] ValidOptions) : base(Key)
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

    string? IEnumSelectSetting.GetDisplayName(Enum value) => GetDisplayName((TEnum)value);
    string? ISelectSetting.GetDisplayName(object value) => GetDisplayName((TEnum)value);
    public string? GetDisplayName(TEnum value)
        => value.AsString(EnumFormat.DisplayName) ?? value.AsString(EnumFormat.Name);
    string? ISelectSetting.GetDescription(object value) => GetDescription((TEnum)value);
    string? IEnumSelectSetting.GetDescription(Enum value) => GetDescription((TEnum)value);
    public string? GetDescription(TEnum value)
        => value.AsString(EnumFormat.Description);
}

static class Extension
{
    public static int IndexOf<T>(this IEnumerable<T> source, T value, EqualityComparer<T>? comparer = null)
    {
        int index = 0;
        comparer ??= EqualityComparer<T>.Default;
        foreach (T item in source)
        {
            if (comparer.Equals(item, value)) return index;
            index++;
        }
        return -1;
    }
}