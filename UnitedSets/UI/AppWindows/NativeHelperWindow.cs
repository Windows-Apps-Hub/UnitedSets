using WinRT.Interop;
using WinUIEx.Messaging;

namespace UnitedSets.UI.AppWindows;

public abstract class NativeHelperWindow : WinUIEx.WindowEx
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
