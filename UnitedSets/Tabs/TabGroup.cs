using System.Collections.ObjectModel;
using System.ComponentModel;
using Get.EasyCSharp;

namespace UnitedSets.Tabs;

public partial class TabGroup : INotifyPropertyChanged
{
    public TabGroup(string Name) { _Name = Name; }
    [AutoNotifyProperty]
    string _Name;

    public ObservableCollection<TabBase> Tabs { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

}
