using Windows.Win32;
using Windows.Win32.UI.Accessibility;
using Windows.Win32.Foundation;
using System;
using System.Collections.Generic;
using Microsoft.UI.Dispatching;
namespace WindowHoster;

/// <summary>
/// Shared, ref-counted wrapper around <c>SetWinEventHook</c>.
/// </summary>
/// <remarks>
/// <para>
/// One out-of-context hook is installed per (event type, skip-own-process) pair and shared by every
/// registration. Notes on the fixes compared to the original implementation:
/// </para>
/// <list type="bullet">
/// <item>The callback delegates are stored in static fields. Passing a method group directly to
/// <c>SetWinEventHook</c> creates a temporary delegate that the GC may collect while native code still
/// calls it, which crashes the process at random.</item>
/// <item>A hook is only removed once no window is registered for that event type any more. Previously,
/// unregistering one window removed the hook for every window, so e.g. close detection stopped working
/// for all other tabs.</item>
/// <item>Registrations made with <c>skipOwnProcess</c> are stored in the dictionary the matching callback
/// actually reads.</item>
/// <item>Out-of-context hooks deliver events through the message loop of the installing thread, so hooks are
/// always installed on <see cref="HookDispatcherQueue"/> (the UI thread) when it is set.</item>
/// </list>
/// </remarks>
public static class WinEvents
{
    /// <summary>
    /// The dispatcher queue whose thread owns the hooks. Must be a thread with a message loop.
    /// If not set, hooks are installed on the calling thread.
    /// </summary>
    public static DispatcherQueue? HookDispatcherQueue { get; set; }

    static readonly object Lock = new();
    // Keep the delegates alive for the lifetime of the process (see remarks).
    static readonly WINEVENTPROC EventCallbackDelegate = EventCallback;
    static readonly WINEVENTPROC EventCallbackSkipOwnProcessDelegate = EventCallbackSkipOwnProcess;

    static readonly Dictionary<WinEventTypes, IDisposable> UnhookFuncs = [];
    static readonly Dictionary<WinEventTypes, IDisposable> UnhookFuncsSkipOwnProcess = [];
    static readonly Dictionary<HWND, Dictionary<WinEventTypes, WinEventHandler>> EventRegistrations = [];
    static readonly Dictionary<HWND, Dictionary<WinEventTypes, WinEventHandler>> EventRegistrationsSkipOwnProcess = [];
    /// <summary>
    /// Handlers that receive the event for every window of other processes
    /// </summary>
    static readonly Dictionary<WinEventTypes, WinEventHandler> GlobalRegistrations = [];

    /// <summary>
    /// Registers a handler that receives <paramref name="type"/> for all windows of other processes.
    /// </summary>
    public static WinEventsRegistrationParameters RegisterGlobal(WinEventTypes type, WinEventHandler handler)
    {
        lock (Lock)
        {
            GlobalRegistrations[type] = GlobalRegistrations.TryGetValue(type, out var oldHandler)
                ? oldHandler + handler
                : handler;
        }
        EnsureWinHookRegistered(type, skipOwnProcess: true);
        return new(GlobalHwnd, type, true, handler);
    }
    /// <summary>
    /// The <see cref="WinEventsRegistrationParameters.Hwnd"/> of a global registration
    /// </summary>
    public const nint GlobalHwnd = 0;

    public static WinEventsRegistrationParameters Register(nint hwnd, WinEventTypes type, bool skipOwnProcess, WinEventHandler handler)
    {
        lock (Lock)
        {
            var registrations = skipOwnProcess ? EventRegistrationsSkipOwnProcess : EventRegistrations;
            if (!registrations.TryGetValue((HWND)hwnd, out var registeredWindow))
                registrations[(HWND)hwnd] = registeredWindow = [];
            registeredWindow[type] = registeredWindow.TryGetValue(type, out var oldHandler)
                ? oldHandler + handler
                : handler;
        }
        EnsureWinHookRegistered(type, skipOwnProcess);
        return new(hwnd, type, skipOwnProcess, handler);
    }

    public static void Unregister(WinEventsRegistrationParameters param)
        => Unregister(param.Hwnd, param.Type, param.SkipOwnProcess, param.Handler);

    public static void Unregister(nint hwnd, WinEventTypes type, bool skipOwnProcess, WinEventHandler? handler)
    {
        if (handler is null) return;
        bool shouldUnhook;
        if (hwnd == GlobalHwnd)
        {
            lock (Lock)
            {
                if (!GlobalRegistrations.TryGetValue(type, out var oldGlobal)) return;
                var newGlobal = oldGlobal - handler;
                if (newGlobal is null) GlobalRegistrations.Remove(type);
                else GlobalRegistrations[type] = newGlobal;
                shouldUnhook = !AnyRegistrationFor(EventRegistrationsSkipOwnProcess, type) && !GlobalRegistrations.ContainsKey(type);
            }
            if (shouldUnhook)
                RemoveHook(type, skipOwnProcess: true);
            return;
        }
        lock (Lock)
        {
            var registrations = skipOwnProcess ? EventRegistrationsSkipOwnProcess : EventRegistrations;
            if (!registrations.TryGetValue((HWND)hwnd, out var registeredWindow)) return;
            if (!registeredWindow.TryGetValue(type, out var oldHandler)) return;
            var newHandler = oldHandler - handler;
            if (newHandler is null)
            {
                registeredWindow.Remove(type);
                if (registeredWindow.Count is 0)
                    registrations.Remove((HWND)hwnd);
            }
            else
                registeredWindow[type] = newHandler;
            // Only unhook when nobody is listening for this event type anymore
            shouldUnhook = !AnyRegistrationFor(registrations, type) &&
                !(skipOwnProcess && GlobalRegistrations.ContainsKey(type));
        }
        if (shouldUnhook)
            RemoveHook(type, skipOwnProcess);
    }

    static bool AnyRegistrationFor(Dictionary<HWND, Dictionary<WinEventTypes, WinEventHandler>> registrations, WinEventTypes type)
    {
        foreach (var (_, dict) in registrations)
            if (dict.ContainsKey(type))
                return true;
        return false;
    }

    static void RunOnHookThread(Action action)
    {
        var dq = HookDispatcherQueue;
        if (dq is null || dq.HasThreadAccess)
            action();
        else
            dq.TryEnqueue(DispatcherQueuePriority.High, () => action());
    }

    static void RemoveHook(WinEventTypes type, bool skipOwnProcess)
    {
        RunOnHookThread(() =>
        {
            IDisposable? toDispose = null;
            lock (Lock)
            {
                var registrations = skipOwnProcess ? EventRegistrationsSkipOwnProcess : EventRegistrations;
                // someone may have registered again in the mean time
                if (AnyRegistrationFor(registrations, type)) return;
                if (skipOwnProcess && GlobalRegistrations.ContainsKey(type)) return;
                var hooks = skipOwnProcess ? UnhookFuncsSkipOwnProcess : UnhookFuncs;
                if (hooks.Remove(type, out var disposable))
                    toDispose = disposable;
            }
            toDispose?.Dispose();
        });
    }

    static void EnsureWinHookRegistered(WinEventTypes type, bool skipOwnProcess)
    {
        RunOnHookThread(() =>
        {
            lock (Lock)
            {
                var hooks = skipOwnProcess ? UnhookFuncsSkipOwnProcess : UnhookFuncs;
                if (hooks.ContainsKey(type)) return;
                hooks[type] = PInvoke.SetWinEventHook(
                    (uint)type,
                    (uint)type,
                    null,
                    skipOwnProcess ? EventCallbackSkipOwnProcessDelegate : EventCallbackDelegate,
                    0,
                    0,
                    skipOwnProcess
                        ? PInvoke.WINEVENT_OUTOFCONTEXT | PInvoke.WINEVENT_SKIPOWNPROCESS
                        : PInvoke.WINEVENT_OUTOFCONTEXT
                );
            }
        });
    }

    static void Dispatch(
        Dictionary<HWND, Dictionary<WinEventTypes, WinEventHandler>> registrations,
        bool includeGlobal,
        uint @event, HWND hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
    {
        WinEventHandler? handler = null, globalHandler = null;
        lock (Lock)
        {
            if (registrations.TryGetValue(hwnd, out var registeredWindow))
                registeredWindow.TryGetValue((WinEventTypes)@event, out handler);
            if (includeGlobal)
                GlobalRegistrations.TryGetValue((WinEventTypes)@event, out globalHandler);
        }
        Invoke(handler, @event, hwnd, idObject, idChild, idEventThread, dwmsEventTime);
        Invoke(globalHandler, @event, hwnd, idObject, idChild, idEventThread, dwmsEventTime);
    }
    static void Invoke(WinEventHandler? handler, uint @event, HWND hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
    {
        if (handler is null) return;
        try
        {
            handler((WinEventTypes)@event, hwnd, idObject, idChild, idEventThread, dwmsEventTime);
        }
        catch (Exception ex)
        {
            // never let an exception escape into native code
            System.Diagnostics.Debug.WriteLine($"[WinEvents] handler threw: {ex}");
        }
    }

    static void EventCallback(
        HWINEVENTHOOK hWinEventHook,
        uint @event,
        HWND hwnd,
        int idObject,
        int idChild,
        uint idEventThread,
        uint dwmsEventTime
    ) => Dispatch(EventRegistrations, false, @event, hwnd, idObject, idChild, idEventThread, dwmsEventTime);

    static void EventCallbackSkipOwnProcess(
        HWINEVENTHOOK hWinEventHook,
        uint @event,
        HWND hwnd,
        int idObject,
        int idChild,
        uint idEventThread,
        uint dwmsEventTime
    ) => Dispatch(EventRegistrationsSkipOwnProcess, true, @event, hwnd, idObject, idChild, idEventThread, dwmsEventTime);

    /// <summary>
    /// <c>OBJID_WINDOW</c>. WinEvents are also raised for the caret, cursor, scroll bars, etc. of a window.
    /// Only events with this object id are about the window itself.
    /// </summary>
    public const int OBJID_WINDOW = 0;
    /// <summary><c>CHILDID_SELF</c></summary>
    public const int CHILDID_SELF = 0;
    /// <summary>
    /// Returns whether a WinEvent is about the window itself (and not its caret, cursor, scroll bars, etc.)
    /// </summary>
    public static bool IsWindowItself(int idObject, int idChild) => idObject == OBJID_WINDOW && idChild == CHILDID_SELF;
}
public delegate void WinEventHandler(WinEventTypes eventType, nint hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime);
public enum WinEventTypes : uint
{
    PositionSizeChanged = PInvoke.EVENT_OBJECT_LOCATIONCHANGE,
    WindowMovedStart = PInvoke.EVENT_SYSTEM_MOVESIZESTART,
    WindowMovedEnd = PInvoke.EVENT_SYSTEM_MOVESIZEEND,
    NameChanged = PInvoke.EVENT_OBJECT_NAMECHANGE,
    ObjectDestroyed = PInvoke.EVENT_OBJECT_DESTROY,
    WindowShown = PInvoke.EVENT_OBJECT_SHOW,
    WindowHidden = PInvoke.EVENT_OBJECT_HIDE,
    ForegroundChanged = PInvoke.EVENT_SYSTEM_FOREGROUND,
    Minimized = PInvoke.EVENT_SYSTEM_MINIMIZESTART,
}
public readonly record struct WinEventsRegistrationParameters(nint Hwnd, WinEventTypes Type, bool SkipOwnProcess, WinEventHandler Handler)
{
    public void Unregister() => WinEvents.Unregister(this);
}
