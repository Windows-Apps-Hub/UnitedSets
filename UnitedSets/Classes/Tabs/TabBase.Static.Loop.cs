using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace UnitedSets.Classes.Tabs;

partial class TabBase
{

	static void StaticUpdateStatusThreadLoop()
    {
        while (true)
        {
            do
                Thread.Sleep(500);
            while (!MainWindows.Any(x => x.IsVisible));

            try
            {
				foreach (var tab in AllTabs.ToArray())
                {
					if (tab.IsDisposed)
						AllTabs.Remove(tab);
					else
						tab.UpdateStatusLoop();
                }
                OnUpdateStatusLoopComplete?.Invoke();
            }
            catch
            {
                Debug.WriteLine("[United Sets Update Status Loop] Exception Occured");
            }
        }
    }

}
