using Get.Symbols;
using Microsoft.UI.Xaml;
using System;
using System.ComponentModel;
using UnitedSets.Apps;

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
    public Visibility OOBEUserInterfaceVisibility { get; set; } = Visibility.Collapsed;
    public Visibility UserInterfaceVisibility { get; set; } = Constants.VisibleOnExperimental;

    public abstract event PropertyChangedEventHandler? PropertyChanged;
}
