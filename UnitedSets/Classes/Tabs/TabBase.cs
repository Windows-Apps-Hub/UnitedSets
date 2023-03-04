using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace UnitedSets.Classes.Tabs;

public abstract partial class TabBase : INotifyPropertyChanged
{
    public TabBase(TabView Parent, bool IsSwitcherVisible)
    {
        AllTabs.Add(this);
        ParentTabView = Parent;
        this.IsSwitcherVisible = IsSwitcherVisible;
        InitSwitcher();
    }
}
