using System;

namespace UnitedSets.Settings;

public class OnOffSetting(Func<bool> Getter, Action<bool> Setter) : Setting<bool>(Getter, Setter) { }