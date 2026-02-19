namespace UnitedSets.Settings;
interface ITempLinkSetting
{
    string Display { get; }
}
public partial class TempLinkSetting<T>(Func<T> Getter, Action<T> Setter) : Setting<T>(Getter, Setter), ITempLinkSetting
{
    public string Display => Value?.ToString() ?? "<null>";
}
