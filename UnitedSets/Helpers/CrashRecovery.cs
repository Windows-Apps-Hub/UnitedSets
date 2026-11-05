using System.IO;
using System.Runtime.InteropServices;
using UnitedSets.Configurations;
using UnitedSets.Tabs;

namespace UnitedSets.Helpers;

/// <summary>
/// Windows in unselected tabs are hidden. If United Sets crashes or is killed, they would stay hidden forever
/// (the user would "lose" them). This keeps a list of hosted windows on disk and shows the ones that are
/// still hidden on the next start, and tries to release every window when a crash happens.
/// </summary>
static class CrashRecovery
{
    static string FilePath => Path.Combine(USConfig.AppDataPath, "hosted-windows.txt");
    static string lastWritten = "";

    /// <summary>
    /// Saves the list of currently hosted windows (called periodically).
    /// </summary>
    public static void Save(IEnumerable<TabBase> tabs)
    {
        try
        {
            var lines = tabs
                .Where(t => !t.IsDisposed)
                .SelectMany(t => t.Windows)
                .Where(w => w.IsValid)
                .Select(w => $"{(long)w.Handle},{w.OwnerProcess.Id}")
                .Distinct()
                .Order();
            var text = string.Join(Environment.NewLine, lines);
            if (text == lastWritten) return;
            Directory.CreateDirectory(USConfig.AppDataPath);
            File.WriteAllText(FilePath, text);
            lastWritten = text;
        }
        catch (Exception ex)
        {
            CrashLog.Write("Saving hosted windows", ex);
        }
    }

    /// <summary>
    /// Shows windows that were hosted when United Sets exited unexpectedly and are still hidden.
    /// </summary>
    /// <returns>The number of windows recovered</returns>
    public static int RecoverOrphanedWindows()
    {
        var recovered = 0;
        try
        {
            if (!File.Exists(FilePath)) return 0;
            foreach (var line in File.ReadAllLines(FilePath))
            {
                var parts = line.Split(',');
                if (parts.Length != 2 || !long.TryParse(parts[0], out var hwndValue) || !uint.TryParse(parts[1], out var pid))
                    continue;
                var hwnd = (nint)hwndValue;
                if (!IsWindow(hwnd)) continue;
                // window handles can be reused: make sure it's still the same process
                GetWindowThreadProcessId(hwnd, out var currentPid);
                if (currentPid != pid) continue;
                if (IsWindowVisible(hwnd)) continue;
                ShowWindow(hwnd, SW_SHOW);
                recovered++;
            }
            File.Delete(FilePath);
        }
        catch (Exception ex)
        {
            CrashLog.Write("Recovering hosted windows", ex);
        }
        return recovered;
    }

    /// <summary>
    /// Best effort: shows and releases every hosted window (used when crashing).
    /// </summary>
    public static void ReleaseAllWindowsNow()
    {
        try
        {
            foreach (var tab in UnitedSetsApp.Current.AllTabs)
                foreach (var w in tab.Windows)
                {
                    var window = w;
                    if (!window.IsValid) continue;
                    window.Owner = default;
                    window.IsVisible = true;
                }
        }
        catch { }
    }

    const int SW_SHOW = 5;
    [DllImport("user32.dll")] static extern bool IsWindow(nint hwnd);
    [DllImport("user32.dll")] static extern bool IsWindowVisible(nint hwnd);
    [DllImport("user32.dll")] static extern bool ShowWindow(nint hwnd, int cmd);
    [DllImport("user32.dll")] static extern uint GetWindowThreadProcessId(nint hwnd, out uint pid);
}
