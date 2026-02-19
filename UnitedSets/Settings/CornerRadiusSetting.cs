namespace UnitedSets.Settings;

public partial class CornerRadiusSetting(Func<CornerRadius> Getter, Action<CornerRadius> Setter) : Setting<CornerRadius>(Getter, Setter);
