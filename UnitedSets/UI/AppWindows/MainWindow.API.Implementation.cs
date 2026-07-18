namespace UnitedSets.UI.AppWindows;

public sealed partial class MainWindow
{
    private partial async Task TimerStop()
    {
        timer.Stop();
        await Task.CompletedTask;
    }
}
