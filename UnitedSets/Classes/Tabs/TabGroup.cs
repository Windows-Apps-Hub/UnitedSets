using System.Collections.ObjectModel;
using System.ComponentModel;
using EasyCSharp;

namespace UnitedSets.Classes.Tabs;

public partial class TabGroup : INotifyPropertyChanged
{
    public TabGroup(string Name) { _Name = Name; }
    [AutoNotifyProperty]
    string _Name;

    public ObservableCollection<TabBase> Tabs { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

}
