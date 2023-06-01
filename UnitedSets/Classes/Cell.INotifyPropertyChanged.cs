using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
namespace UnitedSets.Classes;
public partial class Cell : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected partial void NotifyPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new(PropertyName));
    }
}