using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Drawing;

namespace WinUI3HwndHostPlus;

partial class HwndHost
{
    public partial Task DetachAndDispose(bool Focus = true);
    public partial void FocusWindow();
    public partial void Dispose();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public partial void ForceUpdateWindow();


    public void FixSizeBug() => HostedWindow.Restore();
    public bool MayBeSizeBug =>
        !IsDisposed &&
        _CacheWindowRect.Size == SizeF.Empty &&
        HostedWindow.IsValid &&
        HostedWindow.IsNormalSize &&
        Visibility == Microsoft.UI.Xaml.Visibility.Visible;
}
