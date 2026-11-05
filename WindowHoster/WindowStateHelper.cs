using Windows.Win32;
using Windows.Win32.Foundation;
using WinWrapper.Windowing;
namespace WindowHoster;

/// <summary>
/// Reliable window state checks. <c>WinWrapper</c>'s <c>Window.IsMinimized</c> compares the placement
/// against <c>SW_MINIMIZE</c> while Windows reports <c>SW_SHOWMINIMIZED</c>, so it is always <c>false</c>.
/// </summary>
public static class WindowStateHelper
{
    public static bool IsMinimizedReliable(this Window window) => PInvoke.IsIconic((HWND)window.Handle);
    public static bool IsMaximizedReliable(this Window window) => PInvoke.IsZoomed((HWND)window.Handle);
    public static bool IsAlive(this Window window) => window.Handle != 0 && PInvoke.IsWindow((HWND)window.Handle);
}
