using System.ComponentModel;
using System.Threading.Tasks;

namespace UnitedSets.UI.AppWindows;

public sealed partial class MainWindow : INotifyPropertyChanged
{

    // Private APIs
    private partial Task TimerStop();
}
