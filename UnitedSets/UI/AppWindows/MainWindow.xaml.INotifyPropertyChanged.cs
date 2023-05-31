using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UnitedSets.UI.AppWindows;

public sealed partial class MainWindow : INotifyPropertyChanged
{
    PropertyChangedEventHandler? _PropertyChanged;
    event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
    {
        add => _PropertyChanged += value;
        remove => _PropertyChanged -= value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void NotifyPropertyChanged(string propertyName)
        => _PropertyChanged?.Invoke(this, new(propertyName));
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void NotifyPropertyChangedOnUIThread(string propertyName)
        => UIRun(() => NotifyPropertyChanged(propertyName));
}