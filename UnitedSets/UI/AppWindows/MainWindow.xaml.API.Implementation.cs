using System.Threading.Tasks;

namespace UnitedSets.UI.AppWindows;

public sealed partial class MainWindow
{
    private partial async Task TimerStop()
    {
        timer.Stop();
        OnUIThreadTimerLoop();
        await Task.Delay(100);
    }
}
