using System.Diagnostics;
using System.IO;
using UnitedSets.Configurations;

namespace UnitedSets.Helpers;

/// <summary>
/// Writes unexpected exceptions to <c>%AppData%\UnitedSets\error.log</c> so bugs can be reported
/// (Release builds keep running after unhandled exceptions, which used to hide them completely).
/// </summary>
static class CrashLog
{
    static readonly object Lock = new();
    const long MaxLogSize = 1024 * 1024;
    public static string LogFile => Path.Combine(USConfig.AppDataPath, "error.log");

    public static void Write(string source, Exception? exception)
    {
        var text = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {source}: {exception}{Environment.NewLine}";
        Debug.WriteLine(text);
        try
        {
            lock (Lock)
            {
                Directory.CreateDirectory(USConfig.AppDataPath);
                var file = new FileInfo(LogFile);
                if (file.Exists && file.Length > MaxLogSize)
                    // keep the log small: start over
                    File.Move(LogFile, LogFile + ".old", overwrite: true);
                File.AppendAllText(LogFile, text);
            }
        }
        catch
        {
            // never throw from the logger
        }
    }
}
