using Windows.Win32;
using Windows.Win32.UI.Accessibility;
using Windows.Win32.Foundation;
using System;
using System.Collections.Generic;
namespace WindowHoster;

public static class WinEvents
{
    readonly static Dictionary<WinEventTypes, IDisposable> UnhookFuncs = [];
    readonly static Dictionary<WinEventTypes, IDisposable> UnhookFuncsSkipOwnProcess = [];
    public static WinEventsRegistrationParameters Register(nint hwnd, WinEventTypes type, bool skipOwnProcess, WinEventHandler handler)
    {
        EnsureWinHookRegistered(type, skipOwnProcess);
        if (!EventRegistrations.TryGetValue((HWND)hwnd, out var registeredWindow))
        {
            EventRegistrations[(HWND)hwnd] = registeredWindow = [];
        }
        if (registeredWindow.TryGetValue(type, out var oldHandler)) {
            registeredWindow[type] = oldHandler + handler;
        } else
        {
            registeredWindow[type] = handler;
        }
        return new(hwnd, type, skipOwnProcess, handler);
    }
    public static void Unregister(WinEventsRegistrationParameters param)
        => Unregister(param.Hwnd, param.Type, param.SkipOwnProcess, param.Handler);
    public static void Unregister(nint hwnd, WinEventTypes type, bool skipOwnProcess, WinEventHandler handler)
    {
        if (!EventRegistrations.TryGetValue((HWND)hwnd, out var registeredWindow)) return;
        if (registeredWindow.TryGetValue(type, out var oldHandler))
        {
            var newHandler = oldHandler - handler;
            if (newHandler is null)
            {
                registeredWindow.Remove(type);
                
                if (UnhookFuncs.TryGetValue(type, out var disposable)) {
                    UnhookFuncs.Remove(type);
                    disposable.Dispose();
                }
            }
            else
                registeredWindow[type] = newHandler;
        }
        else
        {
            registeredWindow[type] = handler;
        }
    }
    static void EnsureWinHookRegistered(WinEventTypes type, bool skipOwnProcess)
    {
        if (skipOwnProcess)
        {
            if (!UnhookFuncsSkipOwnProcess.ContainsKey(type))
                UnhookFuncsSkipOwnProcess[type] = PInvoke.SetWinEventHook(
                    (uint)type,
                    (uint)type,
                    null,
                    EventCallbackSkipOwnProcess,
                    0,
                    0,
                    PInvoke.WINEVENT_OUTOFCONTEXT | PInvoke.WINEVENT_SKIPOWNPROCESS
                );
        } else
        {
            if (!UnhookFuncs.ContainsKey(type))
                UnhookFuncs[type] = PInvoke.SetWinEventHook(
                    (uint)type,
                    (uint)type,
                    null,
                    EventCallback,
                    0,
                    0,
                    PInvoke.WINEVENT_OUTOFCONTEXT
                );
        }
    }
    readonly static Dictionary<HWND, Dictionary<WinEventTypes, WinEventHandler>> EventRegistrations = [];
    readonly static Dictionary<HWND, Dictionary<WinEventTypes, WinEventHandler>> EventRegistrationsSkipOwnProcess = [];
    static void EventCallback(
        HWINEVENTHOOK hWinEventHook,
        uint @event,
        HWND hwnd,
        int idObject,
        int idChild,
        uint idEventThread,
        uint dwmsEventTime
    )
    {
        if (
            EventRegistrations.TryGetValue(hwnd, out var registeredWindow) &&
            registeredWindow.TryGetValue((WinEventTypes)@event, out var handler))
        {
            handler((WinEventTypes)@event, hwnd, idObject, idChild, idEventThread, dwmsEventTime);
        }
    }
    static void EventCallbackSkipOwnProcess(
        HWINEVENTHOOK hWinEventHook,
        uint @event,
        HWND hwnd,
        int idObject,
        int idChild,
        uint idEventThread,
        uint dwmsEventTime
    )
    {
        if (
            EventRegistrationsSkipOwnProcess.TryGetValue(hwnd, out var registeredWindow) &&
            registeredWindow.TryGetValue((WinEventTypes)@event, out var handler))
        {
            handler((WinEventTypes)@event, hwnd, idObject, idChild, idEventThread, dwmsEventTime);
        }
    }
}
public delegate void WinEventHandler(WinEventTypes eventType, nint hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime);
public enum WinEventTypes : uint
{
    PositionSizeChanged = PInvoke.EVENT_OBJECT_LOCATIONCHANGE,
    WindowMovedStart = PInvoke.EVENT_SYSTEM_MOVESIZESTART,
    NameChanged = PInvoke.EVENT_OBJECT_NAMECHANGE,
    ObjectDestroyed = PInvoke.EVENT_OBJECT_DESTROY,
    WindowShown = PInvoke.EVENT_OBJECT_SHOW
}
public readonly record struct WinEventsRegistrationParameters(nint Hwnd, WinEventTypes Type, bool SkipOwnProcess, WinEventHandler Handler)
{
    public void Unregister() => WinEvents.Unregister(this);
}
