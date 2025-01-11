using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using UnitedSets.Apps;
using UnitedSets.Configurations;
using UnitedSets.UI.AppWindows;

namespace UnitedSets;

public partial class App : Application
{
    public App()
    {
        Directory.CreateDirectory(USConfig.AppDataPath);
        InitializeComponent();
        // Ensure initialized
        var app = UnitedSetsApp.Current;
#if DEBUG
        RequestAttachDebugger();
#else
        UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedException;
        AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
#endif
    }

    async static void RequestAttachDebugger()
    {
        await Task.Delay(2000);
        if (!Debugger.IsAttached)
            Debugger.Launch();
    }
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        //DebugRedir.Listen();
        //if (Constants.IsFirstRun)
        //    LaunchNewOOBE();
        //else
        //    LaunchNewMain();
        LaunchNewMain();
    }


    void LaunchNewOOBE()
    {
        var oobeWindow = new OOBEWindow();
        oobeWindow.Activate();
    }

    public void LaunchNewMain()
    {
        if (UnitedSetsApp.Current.MainWindow is null)
        {
            var window = new MainWindow();
            window.Activate();
        } else
        {
            UnitedSetsApp.Current.MainWindow.Activate();
        }
    }

    private static void OnUnobservedException(object? sender, UnobservedTaskExceptionEventArgs e) => e.SetObserved();

    private static void OnUnhandledException(object? sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e) => e.Handled = true;

    private void CurrentDomain_FirstChanceException(object? sender, FirstChanceExceptionEventArgs e)
    {
    }
}

public class DebugRedir : StringWriter
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
