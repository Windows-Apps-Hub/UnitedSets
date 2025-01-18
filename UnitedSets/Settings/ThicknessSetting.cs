using System;
using Microsoft.UI.Xaml;
using Windows.UI;

namespace UnitedSets.Settings;

public partial class ThicknessSetting(Func<Thickness> Getter, Action<Thickness> Setter) : Setting<Thickness>(Getter, Setter);
