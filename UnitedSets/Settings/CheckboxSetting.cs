using System;

namespace UnitedSets.Settings;

public class CheckboxSetting(Func<bool> Getter, Action<bool> Setter) : Setting<bool>(Getter, Setter) { }