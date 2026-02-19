namespace UnitedSets.Settings;

public partial class TextSetting(Func<string> Getter, Action<string> Setter) : Setting<string>(Getter, Setter) {
    public string PlaceholderText { get; init; } = "";
}
