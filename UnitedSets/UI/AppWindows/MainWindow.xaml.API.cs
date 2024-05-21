using WindowEx = WinWrapper.Windowing.Window;
using System.ComponentModel;
using UnitedSets.Tabs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitedSets.UI.AppWindows;

public sealed partial class MainWindow : INotifyPropertyChanged
{

    // Private APIs
    private partial Task TimerStop();
}
