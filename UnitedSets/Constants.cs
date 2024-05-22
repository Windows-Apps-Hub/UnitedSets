using CommunityToolkit.WinUI.Helpers;
using System;
using Windows.Win32;
using WinWrapper.Windowing;

namespace UnitedSets;

static class Constants
{
    public const string UnitedSetsLifeCycleKey = "UnitedSetsLifeCycle";
    public const string UnitedSetsTabWindowDragProperty = "UnitedSetsTabWindow";
    public static readonly bool IsAltTabVisible = false;
    public static readonly WindowMessages UnitedSetCommunicationChangeWindowOwnership
        = WindowMessagesHelper.Register(nameof(UnitedSetCommunicationChangeWindowOwnership));

    public const int MinWidth = 600, MinHeight = 500;
    public const int BypassMinWidth = 100, BypassMinHeight = 100;

    static readonly Lazy<bool> _IsFirstRun = new(delegate
    {
        var isFirstRun = false;
#if !UNPKG
        try
        {
            isFirstRun = SystemInformation.Instance.IsFirstRun;
        }
        catch { }
#endif
        return isFirstRun;
    });
    public static bool IsFirstRun => _IsFirstRun.Value;
    public static bool ShouldBeBlacklisted(Window Window)
        => Window.Class.Name is
            "Shell_TrayWnd" // Taskbar
            or "Progman" or "WorkerW" // Desktop
            or "WindowsDashboard" // I forget
            or "Windows.UI.Core.CoreWindow" // Quick Settings and Notification Center (other uwp apps should already be ApplicationFrameHost)
        ;
}
