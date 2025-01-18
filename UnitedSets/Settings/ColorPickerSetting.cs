using System;
using Windows.UI;

namespace UnitedSets.Settings;

public class ColorPickerSetting(Func<Color> Getter, Action<Color> Setter) : Setting<Color>(Getter, Setter) { }
