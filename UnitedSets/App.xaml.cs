using System.Diagnostics;
using System.IO;
using UnitedSets.Configurations;
using UnitedSets.Helpers;
using Microsoft.Windows.AppLifecycle;
using UnitedSets.Services;
namespace UnitedSets;

public partial class App : Application
{
    /// <summary>
    /// The instance that owns all set windows. Launching United Sets again opens a new set window in it
    /// (instead of a second process that would fight over hotkeys and window dragging).
    /// </summary>
    readonly AppInstance MainInstance;
    readonly bool IsRedirecting;
    public App()
    {
        MainInstance = AppInstance.FindOrRegisterForKey("UnitedSets.Main");
        IsRedirecting = !MainInstance.IsCurrent;
        InitializeComponent();
        if (IsRedirecting)
            // don't load anything: loading the configuration consumes the saved session
            return;
        Directory.CreateDirectory(USConfig.AppDataPath);
        // windows that were hidden in tabs when United Sets crashed last time
        CrashRecovery.RecoverOrphanedWindows();
        // Ensure initialized
        var app = UnitedSetsApp.Current;
#if DEBUG
        // Opt-in: launching the JIT debugger dialog on every debug run outside Visual Studio is annoying
        if (Environment.GetEnvironmentVariable("UNITEDSETS_ATTACH_DEBUGGER") is "1")
            RequestAttachDebugger();
#endif
        UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedException;
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            CrashLog.Write("AppDomain", e.ExceptionObject as Exception);
            if (e.IsTerminating)
                // don't leave hidden windows behind
                CrashRecovery.ReleaseAllWindowsNow();
        };
    }

    async static void RequestAttachDebugger()
    {
        await Task.Delay(2000);
        if (!Debugger.IsAttached)
            Debugger.Launch();
    }
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        if (IsRedirecting)
        {
            try
            {
                await MainInstance.RedirectActivationToAsync(AppInstance.GetCurrent().GetActivatedEventArgs());
            }
            catch (Exception ex)
            {
                CrashLog.Write("Redirecting activation", ex);
            }
            Environment.Exit(0);
            return;
        }
        AppInstance.GetCurrent().Activated += OnActivatedByAnotherLaunch;
        // keep running (in the notification area) when the last set window is closed
        DispatcherShutdownMode = DispatcherShutdownMode.OnExplicitShutdown;
        UnitedSetsApp.Current.InitializeServices(Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread());
        // United Sets lives in the notification area: windows are grouped by dragging them (Shift + drag).
        // A set window is only opened when there is something to show.
        var needsWindow = CLI.GetFlag("show") || CLI.GetArrVal("add-window-by-exe").Any() ||
            !string.IsNullOrWhiteSpace(CLI.GetVal("profile")) ||
            UnitedSetsApp.Current.Configuration.PersistantService.HasSessionToRestore;
        if (!needsWindow)
        {
            if (!CLI.GetFlag("background"))
                // started by the user (not at sign in): say where it went
                TrayIconService.ShowBalloon($"{UnitedSets.Apps.Constants.AppName} is running",
                    "Hold Shift and drag a window onto another window to group them. Use the tray icon to open a set or exit.");
            return;
        }
        //DebugRedir.Listen();
        //if (Constants.IsFirstRun)
        //    LaunchNewOOBE();
        //else
            //LaunchNewMain();
        LaunchNewMain();
        //LaunchNewSocialPreview();
    }

    /// <summary>
    /// United Sets was launched again: open a new set window (like browsers do)
    /// </summary>
    void OnActivatedByAnotherLaunch(object? sender, AppActivationArguments e)
    {
        string? arguments = null;
        if (e.Kind is ExtendedActivationKind.Launch && e.Data is Windows.ApplicationModel.Activation.ILaunchActivatedEventArgs launch)
            arguments = launch.Arguments;
        UnitedSetsApp.Current.DispatcherQueue.TryEnqueue(() =>
        {
            try
            {
                var window = UnitedSetsApp.Current.CreateNewSetWindow();
                if (!string.IsNullOrWhiteSpace(arguments))
                {
                    var newArgs = CLI.Split(arguments);
                    // unpackaged apps get the whole command line, including the executable
                    if (newArgs.Length > 0 && newArgs[0].EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                        newArgs = newArgs[1..];
                    CLI.SetArgs(["UnitedSets.exe", .. newArgs]);
                    UnitedSetsApp.Current.HandleCLICmds(window);
                }
            }
            catch (Exception ex)
            {
                CrashLog.Write("Activation from another launch", ex);
            }
        });
    }


    void LaunchNewOOBE()
    {
        var oobeWindow = new OOBEWindow();
        oobeWindow.Activate();
    }

    public void LaunchNewMain()
    {
        if (UnitedSetsApp.Current.SetWindows.Count is 0)
        {
            var window = new MainWindow();
            window.Activate();
        } else
        {
            UnitedSetsApp.Current.MainWindow.Activate();
        }
    }

    void LaunchNewSocialPreview()
    {
        var window = new SocialPreviewWindow();
        window.Activate();
    }


    private static void OnUnobservedException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        CrashLog.Write("UnobservedTask", e.Exception);
        e.SetObserved();
    }

    private static void OnUnhandledException(object? sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        CrashLog.Write("XAML", e.Exception);
#if !DEBUG
        // keep running: losing every hosted window because of one bug is worse
        e.Handled = true;
#endif
    }
}

public partial class DebugRedir : StringWriter
{
    static DebugRedir? instance = null;
    public static void Listen()
    {
        instance ??= new DebugRedir(Console.Out);
    }
    private readonly TextWriter OldWriter;

    private DebugRedir(TextWriter oldWriter)
    {
        OldWriter = oldWriter;
    }
    public override void Write(string? x)
    {
        OldWriter.Write(x);
        Debug.Write(x);
        base.Write(x);
    }
    public override void WriteLine(string? x)
    {
        OldWriter.Write(x);
        Debug.WriteLine(x);
        base.WriteLine(x);
    }
}
