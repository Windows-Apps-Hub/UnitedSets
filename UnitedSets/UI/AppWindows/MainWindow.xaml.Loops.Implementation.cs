using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using WindowRelative = WinWrapper.Windowing.WindowRelative;
using WindowEx = WinWrapper.Windowing.Window;
using Cursor = WinWrapper.Input.Cursor;
using UnitedSets.Classes;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.UI.Dispatching;
using System.Threading;
using Windows.Foundation;
using UnitedSets.Tabs;
using CommunityToolkit.WinUI;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using WinWrapper.Taskbar;
using WindowHoster;
using WinWrapper;
using Thread = System.Threading.Thread;

namespace UnitedSets.UI.AppWindows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow
{
    Icon lastIcon = default;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void UpdateWindowIcon()
    {
        var icon = SelectedTabCache?.Windows.FirstOrDefault().LargeIcon ?? default;
        if (icon != lastIcon)
        {

            Taskbar.SetOverlayIcon(
                Win32Window,
                icon,
                SelectedTabCache?.Title ?? ""
            );
            lastIcon = icon;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    async Task RemoveDisposedTab()
    {
        foreach (var Tab in UnitedSetsApp.Current.Tabs.CacheEnumerable())
        {
            if (Tab.IsDisposed)
                await UIRemoveFromCollectionAsync(UnitedSetsApp.Current.Tabs, Tab);
        }
        foreach (var TabGroup in UnitedSetsApp.Current.HiddenTabs.CacheEnumerable())
        {
            foreach (var Tab in TabGroup.Tabs.CacheEnumerable())
            {
                if (Tab.IsDisposed)
                    await UIRemoveFromCollectionAsync(TabGroup.Tabs, Tab);
            }
            if (TabGroup.Tabs.Count == 0)
                await UIRemoveFromCollectionAsync(UnitedSetsApp.Current.HiddenTabs, TabGroup);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Task UIRunAsync(Action action) => Task.Run(() => DispatcherQueue.TryEnqueue(() => action()));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Task UIRemoveFromCollectionAsync<T>(Collection<T> collection, T item) => UIRunAsync(() => collection.Remove(item));
}
static partial class Extension
{
    // Make the name explicit
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] CacheEnumerable<T>(this IEnumerable<T> values)
        => values.ToArray();
}
