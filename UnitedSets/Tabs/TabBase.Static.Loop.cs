using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace UnitedSets.Tabs;

partial class TabBase
{

	static void StaticUpdateStatusThreadLoop()
    {
        while (true)
        {
            do
                Thread.Sleep(500);
            while (!(UnitedSetsApp.Current.MainWindow?.Win32Window.IsVisible ?? false));

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
