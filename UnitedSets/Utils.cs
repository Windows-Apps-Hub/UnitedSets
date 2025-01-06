using System.IO;
using System;
using UnitedSets.Helpers;
using Microsoft.Win32;
using WindowHoster;
using WinWrapper.Windowing;
using System.Linq;

namespace UnitedSets;

static class Utils
{
    public static string? GetOwnerProcessModuleFilename(Window window) => GetOwnerWindow(window).OwnerProcess.GetDotNetProcess.MainModule?.FileName;
    /// <summary>
    /// Work around WinUI/UWP as AppFrameHost is normally the owner but we want the actual app
    /// </summary>
    /// <returns></returns>
    static Window GetOwnerWindow(Window window, out bool wasUwp)
    {
        var owner = window;
        wasUwp = false;
        var mainModulePath = owner.OwnerProcess.GetDotNetProcess.MainModule?.FileName ?? "";
        if (mainModulePath?.Equals(System.IO.Path.Combine(Environment.SystemDirectory, "ApplicationFrameHost.exe"), StringComparison.CurrentCultureIgnoreCase) != true)
        {
            wasUwp = mainModulePath!.Contains(WindowsAppFolder ?? LoadWindowsAppFolder(), StringComparison.CurrentCultureIgnoreCase);//some windows apps dont use appframehost, IE windows terminal

            return owner;
        }
        wasUwp = true;
        var child = GetCoreWindowFromAppHostWindow(window);
        return child;
    }
    public static Window GetCoreWindowFromAppHostWindow(Window appFrameHostMainWindow)
        => appFrameHostMainWindow.Children
        .FirstOrDefault(x => x.Class.Name is "Windows.UI.Core.CoreWindow", appFrameHostMainWindow);
    private static string? WindowsAppFolder = null;
    private static string LoadWindowsAppFolder()
    {
        if (WindowsAppFolder == null)
        {
            using var appx = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Appx");
            WindowsAppFolder = appx?.GetValue("PackageRoot")?.ToString() ?? "invalid";
        }
        return WindowsAppFolder;
    }
    public const string OUR_WINDOWS_STORE_APP_EXEC_PREFIX = "#WindowsApp!";
    static Window GetOwnerWindow(Window window) => GetOwnerWindow(window, out _);

    public static (string cmd, string args) GetOwnerProcessInfo(Window window)
    {
        var owner = GetOwnerWindow(window, out var wasUWP);
        var toParse = ExternalProcessHelper.GetProcessCommandLineByPID(owner.OwnerProcess.Id.ToString());
        var parsed = ExternalProcessHelper.ParseCmdLine(toParse!);
        // So to get theofficial UWP executable we should use mainModulePath, as the command line can be different for example "wt" will launch windows terminal and will show that as its process. Now we could always use mainModulePath but as we can relaunch terminal with the same command we simply can honor what it says it is.  We must disable UWP mode though in this case so if we are not under the App Folder we can assume not uwp.
        if (wasUWP && parsed.filename.Replace("\\", "/").Contains((WindowsAppFolder ?? LoadWindowsAppFolder()).Replace("\\", "/")) == false)
            wasUWP = false;
        if (wasUWP)
        {
            var fileInfo = new FileInfo(parsed.filename);
            var dir = fileInfo?.Directory?.Name ?? "";
            parsed.filename = $"{OUR_WINDOWS_STORE_APP_EXEC_PREFIX}{dir}";
            var serverNamePos = parsed.args.IndexOf("-ServerName:", StringComparison.CurrentCultureIgnoreCase);
            if (serverNamePos != -1)
            {
                var spaceAfter = parsed.args.IndexOf(" ", serverNamePos + 1);
                var part1 = parsed.args.Substring(0, serverNamePos);
                var part2 = "";
                if (spaceAfter != -1)
                    part2 = parsed.args.Substring(spaceAfter + 1);//should have a space before servername anyway
                parsed.args = part1 + part2;
            }
        }
        return parsed;
    }
}
