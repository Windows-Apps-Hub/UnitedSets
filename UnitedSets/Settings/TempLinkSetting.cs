using System;

namespace UnitedSets.Settings;
interface ITempLinkSetting
{
    string Display { get; }
}
public class TempLinkSetting<T>(Func<T> Getter, Action<T> Setter) : Setting<T>(Getter, Setter), ITempLinkSetting
{
    public string Display => Value?.ToString() ?? "<null>";
}
