using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnitedSets.Mvvm.Services;
using WinWrapper.Windowing;

namespace UnitedSets.Tabs;

partial class TabBase
{
    public static event Action? OnUpdateStatusLoopComplete;
    static readonly SynchronizedCollection<TabBase> AllTabs = [];
    //static readonly WindowClass UnitedSetsSwitcherWindowClass;

    static readonly UnitedSetsAppSettings Settings
        = UnitedSetsApp.Current.Settings;

    static TabBase()
    {
        //UnitedSetsSwitcherWindowClass = new WindowClass(nameof(UnitedSetsSwitcherWindowClass),
        //    (hwnd, msg, wparam, lparam) =>
        //    {
        //        if (msg == PInvoke.WM_ACTIVATE)
        //        {
        //            var tab = AllTabs.FirstOrDefault(x => x.Windows.FirstOrDefault(x => x.Handle == hwnd, default) != default);
        //            tab?.SwitcherWindowFocusCallback();
        //        }
        //        return PInvoke.DefWindowProc(hwnd, msg, wparam, lparam);
        //    },
        //    ClassStyle: WNDCLASS_STYLES.CS_VREDRAW | WNDCLASS_STYLES.CS_HREDRAW,
        //    BackgroundBrush: new(PInvoke.GetStockObject(GET_STOCK_OBJECT_FLAGS.BLACK_BRUSH).Value));
        Thread UpdateStatusLoop = new(StaticUpdateStatusThreadLoop)
        {
            Name = "United Sets Update Status Loop"
        };
        UpdateStatusLoop.Start();
    }
    

}
