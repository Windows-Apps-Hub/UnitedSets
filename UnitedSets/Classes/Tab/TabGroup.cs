using EasyCSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitedSets.Classes;

public partial class TabGroup
{
    public TabGroup(string Name) { _Name = Name; }
    [AutoNotifyProperty]
    string _Name;
    public ObservableCollection<TabBase> Tabs { get; } = new();
}
