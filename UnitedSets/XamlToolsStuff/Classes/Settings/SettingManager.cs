using System;
using System.Diagnostics.Tracing;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace Get.XAMLTools.Classes.Settings.Manager;

public abstract class SettingManager
{
    public event MapChangedEventHandler<string, object>? SettingsChanged;
    protected void InvokeSettingsChanged(IObservableMap<string, object> sender, IMapChangedEventArgs<string> args) => SettingsChanged?.Invoke(sender, args);

    public abstract object this[string Key] { get; set; }
}

public class CurrentApplicationDataContainerSettingManager : SettingManager
{
    protected static readonly ApplicationDataContainer Settings = ApplicationData.Current.LocalSettings;
    public CurrentApplicationDataContainerSettingManager()
    {
        Settings.Values.MapChanged += InvokeSettingsChanged;
    }

    public override object this[string Key] { get => Settings.Values[Key]; set => Settings.Values[Key] = value; }
}