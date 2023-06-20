using System;
using System.Collections.Generic;
using System.Threading;
using WinWrapper.Windowing;

namespace UnitedSets.Classes.Tabs;

partial class TabBase
{
    public readonly static List<Window> MainWindows = new();
    public static event Action? OnUpdateStatusLoopComplete;
    static readonly SynchronizedCollection<TabBase> AllTabs = new();
    private static partial void StaticUpdateStatusThreadLoop();

    static TabBase()
    {
        Thread UpdateStatusLoop = new(StaticUpdateStatusThreadLoop)
        {
            Name = "United Sets Update Status Loop"
        };
        UpdateStatusLoop.Start();
    }
    

}
