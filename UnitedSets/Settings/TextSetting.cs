using EnumsNET;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitedSets.Settings;
public class TextSetting(Func<string> Getter, Action<string> Setter) : Setting<string>(Getter, Setter) {
    public string PlaceholderText { get; init; } = "";
}