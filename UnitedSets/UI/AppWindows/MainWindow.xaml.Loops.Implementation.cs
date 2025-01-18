using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using UnitedSets.Tabs;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using WinWrapper.Taskbar;
using WinWrapper;
using Win32Window = WinWrapper.Windowing.Window;
using UnitedSets.Cells;
using System.Diagnostics;
using Microsoft.UI.Xaml.Media;

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
    void BringUWPWindowUp()
    {
        // prevent case of united sets in united sets update
        if (!Win32Window.IsVisible) return;
        var fg = Win32Window.ForegroundWindow;
        if (Win32Window.Root == fg.Root)
        {
            if (UnitedSetsApp.Current.SelectedTab is { } tab)
            {
                if (tab is WindowHostTab wht)
                {
                    if (wht.RegisteredWindow.CompatablityMode.NoOwner)
                    {
                        // uwp window is probably behind
                        // don't activate the logic if we have any popups
                        DispatcherQueue.TryEnqueue(delegate
                        {
                            var popups = VisualTreeHelper.GetOpenPopupsForXamlRoot(Content.XamlRoot);
                            if (popups.Count is 0)
                                // attempt to activate it
                                wht.RegisteredWindow.Window.Activate(WinWrapper.Windowing.ActivationTechnique.SetWindowPosTopMost);
                        });
                    }
                }
                else if (tab is CellTab ct)
                {
                    foreach (var cell in ct.MainCell.AllSubCells)
                    {
                        if (cell is not WindowCell wc) continue;
                        if (wc.Window.CompatablityMode.NoOwner)
                        {
                            // uwp window is probably behind
                            // don't activate the logic if we have any popups
                            DispatcherQueue.TryEnqueue(delegate
                            {
                                var popups = VisualTreeHelper.GetOpenPopupsForXamlRoot(Content.XamlRoot);
                                if (popups.Count is 0)
                                    // attempt to activate it
                                    wc.Window.Window.Activate(WinWrapper.Windowing.ActivationTechnique.SetWindowPosTopMost);
                            });
                        }
                    }
                }
            }
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
