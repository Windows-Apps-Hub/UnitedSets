using Get.EasyCSharp;
using System.Runtime.CompilerServices;
using UnitedSets.Tabs;

namespace UnitedSets.UI.AppWindows;

public sealed partial class MainWindow
{
    public bool HasOwner => Win32Window.Owner.IsValid;
    bool cacheHasOwner = false;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ThreadLoopDetectAndUpdateHasOwnerChange()
    {
        var _new = HasOwner;
        if (cacheHasOwner == _new) return;

        cacheHasOwner = _new;
        NotifyPropertyChangedOnUIThread(nameof(HasOwner));
    }
}