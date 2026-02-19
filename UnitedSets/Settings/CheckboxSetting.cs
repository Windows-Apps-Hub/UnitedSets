namespace UnitedSets.Settings;

public partial class CheckboxSetting(Func<bool> Getter, Action<bool> Setter) : Setting<bool>(Getter, Setter);
