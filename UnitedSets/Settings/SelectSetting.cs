using EnumsNET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnitedSets.Settings;

public interface IEnumSelectSetting : ISelectSetting
{
    new IReadOnlyList<Enum> ValidOptions { get; }
    new Enum Value { get; set; }
    string? GetDisplayName(Enum value);
    string? GetDescription(Enum value);
}
public interface ISelectSetting
{
    IReadOnlyList<string> ValidOptionsAsString { get; }
    IReadOnlyList<object> ValidOptions { get; }
    object Value { get; set; }
    int ValueIndex { get; set; }
    string? GetDisplayName(object value);
    string? GetDescription(object value);
}
public class SelectSetting<TEnum>(Func<TEnum> Getter, Action<TEnum> Setter, IReadOnlyList<TEnum> ValidOptions) : Setting<TEnum>(Getter, Setter), IEnumSelectSetting where TEnum : struct, Enum
{
    public IReadOnlyList<TEnum> ValidOptions { get; } = ValidOptions;

#pragma warning disable CA2021 // Do not call Enumerable.Cast<T> or Enumerable.OfType<T> with incompatible types
    IReadOnlyList<Enum> IEnumSelectSetting.ValidOptions => ValidOptions.Cast<Enum>().ToArray();
#pragma warning restore CA2021 // Do not call Enumerable.Cast<T> or Enumerable.OfType<T> with incompatible types
    IReadOnlyList<object> ISelectSetting.ValidOptions => ValidOptions.Cast<object>().ToArray();

    Enum IEnumSelectSetting.Value { get => Value; set => Value = (TEnum)value; }
    object ISelectSetting.Value { get => Value; set => Value = (TEnum)value; }
    public int ValueIndex
    {
        get => ValidOptions.IndexOf(Value);
        set
        {
            if (value >= 0)
                Value = ValidOptions[value];
        }
    }

    public IReadOnlyList<string> ValidOptionsAsString => new StrWrapper(this);

    readonly struct StrWrapper(SelectSetting<TEnum> self) : IReadOnlyList<string>
    {
        public string this[int index] => self.GetDisplayName(self.ValidOptions[index]) ?? "";

        public int Count => self.ValidOptions.Count;

        public IEnumerator<string> GetEnumerator()
        {
            for (int i = 0; i < Count; i++) yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

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