using Windows.Win32;

namespace UnitedSets;

class Constants
{
    public const string UnitedSetsTabWindowDragProperty = "UnitedSetsTabWindow";
    public static readonly bool IsAltTabVisible = false;
    public static readonly uint UnitedSetCommunicationChangeWindowOwnership
        = PInvoke.RegisterWindowMessage(nameof(UnitedSetCommunicationChangeWindowOwnership));
}
