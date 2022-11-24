using System.ComponentModel;

namespace UnitedSets.Classes;
public abstract partial class ICell : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add
        {
            _PropertyChanged += value;
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
        remove
        {
            _PropertyChanged -= value;
        }
    }
    protected PropertyChangedEventHandler? _PropertyChanged;
}