//using System.Drawing;
//using System.Runtime.InteropServices;
//using Windows.Win32;
//using WinRT.Interop;
//using WinUIEx;
//using WinUIEx.Messaging;
//using WinWrapper.Windowing;
//using Windows.Win32.UI.WindowsAndMessaging;
//using UnitedSets.Helpers;
//using Microsoft.UI.Xaml.Media;
//using SystemBackdrop = Microsoft.UI.Xaml.Media.SystemBackdrop;
//using Windows.UI.Composition;
//using ICompositionSupportsSystemBackdrop = Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop;
//using System;

//namespace UnitedSets.UI.AppWindows;

//public class TransparentSupportedWindow : WindowEx
//{
//    WindowMessageMonitor m;
//    Window Win32Window;
//    public TransparentSupportedWindow()
//    {
//        m = new(this);
//        m.WindowMessageReceived += WindowMessageReceived;
//        Win32Window = Window.FromWindowHandle(WindowNative.GetWindowHandle(this));
//        Activated += FirstRun;
//    }
//    bool _TransparentMode;
//    protected bool TransparentMode
//    {
//        set
//        {
//            if (_TransparentMode != value)
//            {
//                _TransparentMode = value;
//                Win32Window[WindowExStyles.Layered] = value;
//                if (value)
//                {
//                    Win32Window.Hide();
//                    Win32Window.Show();
//                }
//            }
//        }
//    }
//    private void FirstRun(object sender, Microsoft.UI.Xaml.WindowActivatedEventArgs args)
//    {
//        Activated -= FirstRun;
//    }

//    //SafeHandle TransparentBrush = PInvoke.CreateSolidBrush_SafeHandle(
//    //    new(
//    //        (uint)ColorTranslator.ToWin32(Color.FromArgb(0, 0, 0, 0))));
//    private void WindowMessageReceived(object? sender, WindowMessageEventArgs e)
//    {
//        if (e.Message.MessageId is (uint)WindowMessages.EarseBackground)
//        {
//            var g = Graphics.FromHdc((nint)e.Message.WParam);
//            g.Clear(Color.Transparent);
//            //PInvoke.GetClientRect(Win32Window, out var rect);
//            //PInvoke.FillRect(new((nint)e.Message.WParam), rect, TransparentBrush);
            
//            e.Handled = true;
//            e.Result = 1;
//        }
//    }
//}
//class TransparentBackdrop : SystemBackdrop
//{
//    static readonly Lazy<Compositor> _Compositor = new(() =>
//    {
//        WindowsSystemDispatcherQueueHelper.EnsureWindowsSystemDispatcherQueueController();
//        return new();
//    });
//    static Compositor Compositor => _Compositor.Value;
//    protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop connectedTarget, Microsoft.UI.Xaml.XamlRoot xamlRoot)
//    {
//        connectedTarget.SystemBackdrop = Compositor.CreateColorBrush(
//            Windows.UI.Color.FromArgb(0, 255, 255, 255)
//        );
//    }
//    protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop disconnectedTarget)
//    {
//        disconnectedTarget.SystemBackdrop = null;
//    }
//}
