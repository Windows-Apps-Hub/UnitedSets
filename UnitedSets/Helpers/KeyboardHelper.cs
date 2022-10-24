using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
namespace UnitedSets.Helpers
{
    public class KeyboardHelper : IDisposable
    {
        public event EventHandler<KeyboardHelperEventArgs>? KeyboardPressed;

        public KeyboardHelper()
        {
            _hookProc = LowLevelKeyboardProc; // we must keep alive _hookProc, because GC is not aware about SetWindowsHookEx behaviour.

            _user32LibraryHandle = PInvoke.LoadLibrary("User32");
            if (_user32LibraryHandle.IsInvalid)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode, $"Failed to load library 'User32.dll'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
            }



            _windowsHookHandle = PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_KEYBOARD_LL, _hookProc, _user32LibraryHandle, 0);
            if (_windowsHookHandle.IsInvalid)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode, $"Failed to adjust keyboard hooks for '{Process.GetCurrentProcess().ProcessName}'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // because we can unhook only in the same thread, not in garbage collector thread
                if (!_windowsHookHandle.IsInvalid)
                {
                    _windowsHookHandle.Dispose();
                    //if (Error From Disposing)
                    //{
                    //    int errorCode = Marshal.GetLastWin32Error();
                    //    throw new Win32Exception(errorCode, $"Failed to remove keyboard hooks for '{Process.GetCurrentProcess().ProcessName}'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
                    //}

                    // ReSharper disable once DelegateSubtraction
#pragma warning disable CS8601
                    _hookProc -= LowLevelKeyboardProc;
#pragma warning restore CS8601
                }
            }

            if (_user32LibraryHandle.IsInvalid) return;
            {
                _user32LibraryHandle.Dispose(); // reduces reference to library by 1.
                //if (Error From Disposing)
                //{
                //    int errorCode = Marshal.GetLastWin32Error();
                //    throw new Win32Exception(errorCode, $"Failed to unload library 'User32.dll'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
                //}
            }
        }

        ~KeyboardHelper()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private UnhookWindowsHookExSafeHandle _windowsHookHandle;
        private FreeLibrarySafeHandle _user32LibraryHandle;
        private HOOKPROC _hookProc;

        [StructLayout(LayoutKind.Sequential)]
        public struct LowLevelKeyboardInputEvent
        {
            /// <summary>
            /// A virtual-key code. The code must be a value in the range 1 to 254.
            /// </summary>
            public int VirtualCode;

            /// <summary>
            /// A hardware scan code for the key. 
            /// </summary>
            public int HardwareScanCode;

            /// <summary>
            /// The extended-key flag, event-injected Flags, context code, and transition-state flag. This member is specified as follows. An application can use the following values to test the keystroke Flags. Testing LLKHF_INJECTED (bit 4) will tell you whether the event was injected. If it was, then testing LLKHF_LOWER_IL_INJECTED (bit 1) will tell you whether or not the event was injected from a process running at lower integrity level.
            /// </summary>
            public int Flags;

            /// <summary>
            /// The time stamp stamp for this message, equivalent to what GetMessageTime would return for this message.
            /// </summary>
            public int TimeStamp;

            /// <summary>
            /// Additional information associated with the message. 
            /// </summary>
            public IntPtr AdditionalInformation;
        }

        public const int WH_KEYBOARD_LL = 13;
        //const int HC_ACTION = 0;

        public enum KeyboardState
        {
            KeyDown = 0x0100,
            KeyUp = 0x0101,
            SysKeyDown = 0x0104,
            SysKeyUp = 0x0105
        }

        public const int VkSnapshot = 0x5b;
        //const int VkLwin = 0x5b;
        //const int VkRwin = 0x5c;
        //const int VkTab = 0x09;
        //const int VkEscape = 0x18;
        //const int VkControl = 0x11;
        const int KfAltdown = 0x2000;
        public const int LlkhfAltdown = (KfAltdown >> 8);

        public LRESULT LowLevelKeyboardProc(int nCode, WPARAM wParam, LPARAM lParam)
        {
            bool fEatKeyStroke = false;

            var wparamTyped = (int)wParam.Value;
            if (Enum.IsDefined(typeof(KeyboardState), wparamTyped))
            {
                object o = Marshal.PtrToStructure(lParam, typeof(LowLevelKeyboardInputEvent))!;
                LowLevelKeyboardInputEvent p = (LowLevelKeyboardInputEvent)o;

                var eventArguments = new KeyboardHelperEventArgs(p, (KeyboardState)wparamTyped);

                KeyboardPressed?.Invoke(this, eventArguments);

                fEatKeyStroke = eventArguments.Handled;
            }

            return fEatKeyStroke ? (LRESULT)1 : PInvoke.CallNextHookEx(null, nCode, wParam, lParam);
        }
    }
    public class KeyboardHelperEventArgs : HandledEventArgs
    {
        public KeyboardHelper.KeyboardState KeyboardState { get; private set; }
        public KeyboardHelper.LowLevelKeyboardInputEvent KeyboardData { get; private set; }

        public KeyboardHelperEventArgs(
            KeyboardHelper.LowLevelKeyboardInputEvent keyboardData,
            KeyboardHelper.KeyboardState keyboardState)
        {
            KeyboardData = keyboardData;
            KeyboardState = keyboardState;
        }
    }
}