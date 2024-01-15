using System.Collections.ObjectModel;
using System;
using WindowEx = WinWrapper.Windowing.Window;
using UnitedSets.Mvvm.Services;
using Microsoft.Extensions.DependencyInjection;
using WinUIEx.Messaging;
using Microsoft.UI.Dispatching;
using UnitedSets.Classes.Tabs;

namespace UnitedSets.UI.AppWindows;


sealed partial class NewMainWindow
{
    
}
class AppData
{
    ObservableCollection<TabBase> tabs;
}