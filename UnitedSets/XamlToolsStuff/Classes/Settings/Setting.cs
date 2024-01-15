using Get.EasyCSharp;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Foundation.Collections;
using System.Collections.Generic;
using System;
using Get.XAMLTools.UI;
using Get.XAMLTools.Classes.Settings.Manager;

namespace Get.XAMLTools.Classes.Settings;

public abstract partial class Setting : ObservableObject
{
    public string Title { get; set; }
    public string Description { get; set; } = "";
    public SymbolEx Icon { get; set; } = default;
    public bool RequiresRestart { get; set; } = false;

    protected readonly string Key;
    public Setting(string Key)
    {
        this.Key = Title = Key;
        SettingManager.SettingsChanged += SettingsChanged;
    }
    static SettingManager? _SettingManager;
    public static SettingManager SettingManager {
        get => _SettingManager ??= new CurrentApplicationDataContainerSettingManager();
        set => _SettingManager = value;
    }
    [Event(typeof(MapChangedEventHandler<string, object>))]
    private void SettingsChanged(IMapChangedEventArgs<string> args)
    {
        if (args.CollectionChange is CollectionChange.ItemChanged && args.Key == Key)
            SettingsChanged();
    }

    protected abstract void SettingsChanged();
}
public abstract class Setting<T> : Setting
{
    public T? DefaultValue { get; set; }
    public Setting(string Key) : base(Key) { }
    static readonly EqualityComparer<T> Comparer = EqualityComparer<T>.Default;
    public T Value
    {
        get
        {
            var value = SettingManager[Key];
            if (value is null && !Comparer.Equals(DefaultValue, default))
                return DefaultValue!;
            return TransformValue(value);
        }
        set
        {
            var oldValue = Value;
            if (Comparer.Equals(x: oldValue, y: value)) return;
            OnPropertyChanging(nameof(Value));
            SettingManager[Key] = TransformValue(value);
            OnPropertyChanged(nameof(Value));
            Updated?.Invoke(value);
        }
    }
    protected abstract T TransformValue(object? savedObj);
    protected abstract object TransformValue(T input);
    public event Action<T>? Updated;
    public static implicit operator T(Setting<T> setting)
        => setting.Value;

    protected override void SettingsChanged()
        => OnPropertyChanged(nameof(Value));
}