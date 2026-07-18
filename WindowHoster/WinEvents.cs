using Windows.Win32;
using Windows.Win32.UI.Accessibility;
using Windows.Win32.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
namespace WindowHoster;

public static class WinEvents
{
    static readonly object Sync = new();
    readonly static Dictionary<WinEventTypes, IDisposable> UnhookFuncs = [];
    readonly static Dictionary<WinEventTypes, IDisposable> UnhookFuncsSkipOwnProcess = [];
    public static WinEventsRegistrationParameters Register(nint hwnd, WinEventTypes type, bool skipOwnProcess, WinEventHandler handler)
    {
        lock (Sync)
        {
            EnsureWinHookRegistered(type, skipOwnProcess);
            var registrations = skipOwnProcess ? EventRegistrationsSkipOwnProcess : EventRegistrations;
            if (!registrations.TryGetValue((HWND)hwnd, out var registeredWindow))
                registrations[(HWND)hwnd] = registeredWindow = [];
            if (registeredWindow.TryGetValue(type, out var oldHandler))
                registeredWindow[type] = oldHandler + handler;
            else
                registeredWindow[type] = handler;
        }
        return new(hwnd, type, skipOwnProcess, handler);
    }
    public static void Unregister(WinEventsRegistrationParameters param)
        => Unregister(param.Hwnd, param.Type, param.SkipOwnProcess, param.Handler);
    public static void Unregister(nint hwnd, WinEventTypes type, bool skipOwnProcess, WinEventHandler handler)
    {
        lock (Sync)
        {
            var registrations = skipOwnProcess ? EventRegistrationsSkipOwnProcess : EventRegistrations;
            if (!registrations.TryGetValue((HWND)hwnd, out var registeredWindow) ||
                !registeredWindow.TryGetValue(type, out var oldHandler)) return;
            var newHandler = oldHandler - handler;
            if (newHandler is null)
            {
                registeredWindow.Remove(type);
                if (registeredWindow.Count == 0)
                    registrations.Remove((HWND)hwnd);

                if (!registrations.Values.Any(x => x.ContainsKey(type)))
                {
                    var unhookFunctions = skipOwnProcess ? UnhookFuncsSkipOwnProcess : UnhookFuncs;
                    if (!unhookFunctions.Remove(type, out var disposable)) return;
                    disposable.Dispose();
                }
            }
            else
                registeredWindow[type] = newHandler;
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
        WinEventHandler? handler;
        lock (Sync)
            handler = EventRegistrations.TryGetValue(hwnd, out var registeredWindow) &&
                registeredWindow.TryGetValue((WinEventTypes)@event, out var found) ? found : null;
        handler?.Invoke((WinEventTypes)@event, hwnd, idObject, idChild, idEventThread, dwmsEventTime);
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
        WinEventHandler? handler;
        lock (Sync)
            handler = EventRegistrationsSkipOwnProcess.TryGetValue(hwnd, out var registeredWindow) &&
                registeredWindow.TryGetValue((WinEventTypes)@event, out var found) ? found : null;
        handler?.Invoke((WinEventTypes)@event, hwnd, idObject, idChild, idEventThread, dwmsEventTime);
    }
}
public delegate void WinEventHandler(WinEventTypes eventType, nint hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime);
public enum WinEventTypes : uint
{
    PositionSizeChanged = PInvoke.EVENT_OBJECT_LOCATIONCHANGE,
    WindowMovedStart = PInvoke.EVENT_SYSTEM_MOVESIZESTART,
    NameChanged = PInvoke.EVENT_OBJECT_NAMECHANGE,
    ObjectDestroyed = PInvoke.EVENT_OBJECT_DESTROY,
    WindowShown = PInvoke.EVENT_OBJECT_SHOW,
    Foreground = 0x0003
}
public readonly record struct WinEventsRegistrationParameters(nint Hwnd, WinEventTypes Type, bool SkipOwnProcess, WinEventHandler Handler)
{
    public void Unregister() => WinEvents.Unregister(this);
}
