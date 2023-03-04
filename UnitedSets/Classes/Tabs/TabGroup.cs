using System.Collections.ObjectModel;
using EasyCSharp;

namespace UnitedSets.Classes.Tabs;

public partial class TabGroup
{
    public TabGroup(string Name) { _Name = Name; }
    [AutoNotifyProperty]
    string _Name;
    public ObservableCollection<TabBase> Tabs { get; } = new();
}
