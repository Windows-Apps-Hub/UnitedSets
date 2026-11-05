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
    private void OnUIThreadTimerLoop()
    {
        CacheValue();
        // titles and icons of the taskbar thumbnails (every 2 seconds)
        if (++uiTicks % 4 == 0)
            taskbarTabs.UpdateTitlesAndIcons();
    }
    private async void OnDifferentThreadLoop()
    {
        // (the taskbar icon is now handled by TaskbarTabs)
        HasOwnerUpdate();

        BringUWPWindowUp();

        await RemoveDisposedTab();
        // Dragging windows into a set is handled by WindowDragToGroupService
    }

}
