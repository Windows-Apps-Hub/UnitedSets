using Microsoft.Win32;

namespace UnitedSets.Helpers;

/// <summary>
/// "Start with Windows": a value in HKCU\Software\Microsoft\Windows\CurrentVersion\Run that starts
/// United Sets in the background (tray only) at sign in.
/// </summary>
static class StartupHelper
{
    const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    const string ValueName = "UnitedSets";
    static string Command => $"\"{Environment.ProcessPath}\" --background";

    public static bool IsEnabled
    {
        get
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RunKey);
                return key?.GetValue(ValueName) is string;
            }
            catch { return false; }
        }
        set
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(RunKey);
                if (value) key.SetValue(ValueName, Command);
                else key.DeleteValue(ValueName, throwOnMissingValue: false);
            }
            catch (Exception ex)
            {
                CrashLog.Write("Start with Windows", ex);
            }
        }
    }
}
