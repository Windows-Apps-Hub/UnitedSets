using CommunityToolkit.WinUI.Helpers;
using System;
using Windows.Win32;

namespace UnitedSets;

static class Constants
{
    public const string UnitedSetsLifeCycleKey = "UnitedSetsLifeCycle";
    public const string UnitedSetsTabWindowDragProperty = "UnitedSetsTabWindow";
    public static readonly bool IsAltTabVisible = false;
    public static readonly uint UnitedSetCommunicationChangeWindowOwnership
        = PInvoke.RegisterWindowMessage(nameof(UnitedSetCommunicationChangeWindowOwnership));

    static Lazy<bool> _IsFirstRun = new(delegate
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
}
