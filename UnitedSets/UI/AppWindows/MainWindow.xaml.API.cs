using Microsoft.UI.Xaml.Controls;
using System.Linq;
using WinUIEx;
using System;
using WindowEx = WinWrapper.Windowing.Window;
using UnitedSets.Classes;
using System.ComponentModel;
using WinUI3HwndHostPlus;
using UnitedSets.Classes.Tabs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitedSets.UI.AppWindows;

public sealed partial class MainWindow : INotifyPropertyChanged
{
    // Public APIs
    public partial void AddTab(WindowEx newWindow, int? index = null);
    public partial void AddTab(TabBase tab, int? index = null);
    public partial void RemoveTab(TabBase tab);
    public partial IEnumerable<TabBase> GetTabsAndClear();
    public partial TabBase? FindTabByWindow(WindowEx window);
    public partial (TabGroup? group, TabBase? tab) FindHiddenTabByWindow(WindowEx window);
    public partial HwndHostTab? CreateHwndHostTab(WindowEx newWindow);

    // Private APIs
    private partial Task TimerStop();
    private partial Task Suicide();
    private partial void WireTabEvents(TabBase tab);
    private partial void UnwireTabEvents(TabBase tab);
}
