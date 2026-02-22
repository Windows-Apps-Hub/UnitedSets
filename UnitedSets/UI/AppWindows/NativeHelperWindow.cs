using WinRT.Interop;
using WinUIEx.Messaging;

namespace UnitedSets.UI.AppWindows;

public abstract class NativeHelperWindow : WinUIEx.WindowEx
{
    // Readonly
    public readonly WindowEx Win32Window;
    protected readonly WindowMessageMonitor WindowMessageMonitor;
    protected NativeHelperWindow()
    {
        Win32Window = WindowEx.FromWindowHandle(WindowNative.GetWindowHandle(this));
        WindowMessageMonitor = new WindowMessageMonitor(Win32Window);
    }
}
