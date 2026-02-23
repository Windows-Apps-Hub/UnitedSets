using UnitedSets.Mvvm.Services;
using Microsoft.UI.Dispatching;

namespace UnitedSets.UI.AppWindows;


public sealed partial class MainWindow : NativeHelperWindow
{
    // Singleton
    readonly UnitedSetsAppSettings Settings = UnitedSetsApp.Current.Settings;

    // Readonly
    readonly DispatcherQueueTimer timer;

    readonly CloseAppFlyout ClosingFlyout = new();
}
