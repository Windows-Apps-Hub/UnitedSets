using CommunityToolkit.WinUI.Helpers;
using Visibility = Microsoft.UI.Xaml.Visibility;
using System;
using WinWrapper.Windowing;

namespace UnitedSets.Apps;

static class Constants
{
#if false // Dev Version
    public const Visibility ExperimentalFeedback = Visibility.Collapsed;
    public const Visibility VisibleOnExperimental = Visibility.Visible;
    public const string AppVersionTag = "- Development";
#elif false // Experimental Version
    public const Visibility ExperimentalFeedback = Visibility.Visible;
    public const Visibility VisibleOnExperimental = Visibility.Visible;
    public const string AppVersionTag = "- Experimental";
#else // Release Version
    public const Visibility ExperimentalFeedback = Visibility.Collapsed;
    public const Visibility VisibleOnExperimental = Visibility.Collapsed;
    public const string AppVersionTag = "- Preview Beta";
#endif

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
