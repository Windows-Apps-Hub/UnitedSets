using Get.Symbols;
using Get.XAMLTools.UI;
using System;
using System.ComponentModel;

namespace UnitedSets.Settings;

public abstract class Setting<T>(Func<T> Getter, Action<T> Setter) : Setting
{
    public T Value
    {
        get => Getter();
        set
        {
            Setter(value);
            PropertyChanged?.Invoke(this, new(nameof(Value)));
        }
    }

    public override event PropertyChangedEventHandler? PropertyChanged;
}
public abstract class Setting : INotifyPropertyChanged
{
    public required string Title { get; set; }
    public string Description { get; set; } = "";
    public SymbolEx Icon { get; set; } = default;
    public bool RequiresRestart { get; set; } = false;

    public abstract event PropertyChangedEventHandler? PropertyChanged;
}
