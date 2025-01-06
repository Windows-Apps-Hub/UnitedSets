using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace UnitedSets.UI.AppWindows;

public sealed partial class MainWindow
{

    // Private APIs
    private partial Task TimerStop();
    // Binding APIs
    private GridLength GridLengthFromPixelInt(int i) => new(i * Win32Window.CurrentDisplay.ScaleFactor / 100);
}
