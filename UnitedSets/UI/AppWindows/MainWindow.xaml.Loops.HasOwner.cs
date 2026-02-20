using System.Runtime.CompilerServices;

namespace UnitedSets.UI.AppWindows;

public sealed partial class MainWindow
{
    bool cacheHasOwner = false;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void HasOwnerUpdate()
    {
        var HasOwner = Win32Window.Owner.IsValid;
        var _new = HasOwner;
        if (cacheHasOwner == _new) return;
        cacheHasOwner = _new;
        HasOwnerChanged?.Invoke(_new);
        IsResizable = !_new;
        IsTitleBarVisible = !_new;
        IsMinimizable = !_new;
        IsMaximizable = !_new;
    }
    public event Action<bool>? HasOwnerChanged;
}
