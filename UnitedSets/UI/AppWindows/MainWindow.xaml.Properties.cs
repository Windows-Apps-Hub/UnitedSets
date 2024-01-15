using Get.EasyCSharp;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnitedSets.Classes.Tabs;

namespace UnitedSets.UI.AppWindows;

public sealed partial class MainWindow
{
    public bool HasOwner => Win32Window.Owner.IsValid;
    [AutoNotifyProperty]
    TabBase _SelectedTab;
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