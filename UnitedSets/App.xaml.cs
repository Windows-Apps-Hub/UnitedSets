using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using UnitedSets.Helpers;
using UnitedSets.Mvvm.Services;
using UnitedSets.UI.AppWindows;
using WinUIEx;

namespace UnitedSets;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Gets the current <see cref="App"/> instance in use
    /// </summary>
    public new static App Current => (App)Application.Current;

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
    /// </summary>
    public IServiceProvider Services { get; } = ConfigureServices();

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedException;
        AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
    }

    static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<SettingsService>();

        return services.BuildServiceProvider();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        DebugRedir.Listen();

        //WindowsCompositionHelper.EnsureCompositor();

        //new TransparentWindow()
        //{
        //    Content = new Microsoft.UI.Xaml.Controls.Button
        //    {
        //        Content = "Hello World!"
        //    }
        //}.Activate();
        if (Constants.IsFirstRun)
            LaunchNewOOBE();
        else
            LaunchNewMain();
        //ObservableCollection<string> strs = new();
        //int i = 0;
        //var panel = new UI.Controls.SizeChangedDetectorPanel
        //{
        //    Content = new StackPanel
        //    {
        //        Children =
        //        {
        //            new ItemsControl {
        //                ItemsSource = strs
        //            },
        //            new Button
        //            {
        //                Content = "+",
        //                Command = new RelayCommand(() => strs.Add((++i).ToString()))
        //            }
        //        }
        //    }
        //};
        //var window = new Window
        //{
        //    Content = panel,
        //    ExtendsContentIntoTitleBar = true
        //};
        //panel.SizeUpdated += x =>
        //{
        //    var w = window;
        //    //.SetWindowSize(x.Width, x.Height+20);
        //};
        //window.Show();
    }

    private Window? window;

    void LaunchNewOOBE()
    {
        var oobeWindow = new OOBEWindow();
        oobeWindow.Activate();
    }

    public void LaunchNewMain()
    {
        window = new MainWindow();
        window.Activate();
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