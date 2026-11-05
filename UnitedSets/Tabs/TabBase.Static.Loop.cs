using System.Diagnostics;
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
            while (!UnitedSetsApp.Current.SetWindows.ToArray().Any(w => w.Win32Window.IsVisible));

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
                Helpers.CrashRecovery.Save(UnitedSetsApp.Current.AllTabs);
            }
            catch
            {
                Debug.WriteLine("[United Sets Update Status Loop] Exception Occured");
            }
        }
    }

}
