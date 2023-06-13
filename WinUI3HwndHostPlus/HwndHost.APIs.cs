using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace WinUI3HwndHostPlus;

partial class HwndHost
{
    public partial Task DetachAndDispose(bool Focus = true);
    public partial void FocusWindow();
    public partial void Dispose();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public partial void ForceUpdateWindow();

    public event Action? Closed;
    public event Action? Updating;

    public void FixSizeBug() => _HostedWindow.Restore();
    public bool MayBeSizeBug =>
        _CacheWidth == 0 &&
        _CacheHeight == 0 &&
        !IsDisposed &&
        _HostedWindow.IsValid &&
        _HostedWindow.IsNormalSize &&
        Visibility == Microsoft.UI.Xaml.Visibility.Visible;
}
