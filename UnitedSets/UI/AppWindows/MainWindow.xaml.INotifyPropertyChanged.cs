using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UnitedSets.UI.AppWindows;

public sealed partial class MainWindow : INotifyPropertyChanged
{
    PropertyChangedEventHandler? PropertyChanged;
    event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
    {
        add => PropertyChanged += value;
        remove => PropertyChanged -= value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void NotifyPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new(propertyName));
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void NotifyPropertyChangedOnUIThread(string propertyName)
        => UIRun(() => NotifyPropertyChanged(propertyName));
}