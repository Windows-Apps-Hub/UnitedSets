using Microsoft.UI.Windowing;
using WinRT.Interop;
using WinUIEx;
using WinUIEx.Messaging;

namespace UnitedSets.UI.AppWindows;
public abstract class NativeHelperWindow : WindowEx
{
    // Readonly
    public readonly WinWrapper.Windowing.Window Win32Window;
    protected readonly WindowMessageMonitor WindowMessageMonitor;
    protected NativeHelperWindow()
    {
        Win32Window = WinWrapper.Windowing.Window.FromWindowHandle(WindowNative.GetWindowHandle(this));
        WindowMessageMonitor = new WindowMessageMonitor(Win32Window);
    }
}
