using Get.EasyCSharp;
using WinWrapper.Input;
using System.ComponentModel;
using Microsoft.UI.Dispatching;
using Windows.Foundation;

namespace UnitedSets.UI.AppWindows;

public sealed partial class MainWindow
{
    private partial void SetupUIThreadLoopTimer(out DispatcherQueueTimer timer);

    [Event(typeof(TypedEventHandler<DispatcherQueueTimer, object>))]
    private void OnUIThreadTimerLoop() => CacheValue();
    private async void OnDifferentThreadLoop()
    {
        UpdateWindowIcon();

        HasOwnerUpdate();

        await RemoveDisposedTab();

        if (Cursor.IsLeftButtonDown && Keyboard.IsControlDown)
            WindowDragLogic();
    }

}
