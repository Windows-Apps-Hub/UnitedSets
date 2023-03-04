using EasyCSharp;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using WinRT.Interop;
using WinUIEx;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using System;
using WindowRelative = WinWrapper.WindowRelative;
using WindowEx = WinWrapper.Window;
using Cursor = WinWrapper.Cursor;
using Keyboard = WinWrapper.Keyboard;
using UnitedSets.Classes;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;
using UnitedSets.Services;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Diagnostics;
using WinUIEx.Messaging;
using Microsoft.UI.Dispatching;
using System.Threading;
using System.IO;
using WinWrapper;
using System.Text.RegularExpressions;
using Windows.Foundation;
using UnitedSets.Classes.Tabs;
using UnitedSets.Windows.Flyout;

namespace UnitedSets.Windows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : INotifyPropertyChanged
{
    public const string UnitedSetsTabWindowDragProperty = "UnitedSetsTabWindow";
    public static readonly uint UnitedSetCommunicationChangeWindowOwnership
        = PInvoke.RegisterWindowMessage(nameof(UnitedSetCommunicationChangeWindowOwnership));

    // Implement INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    // Singleton
    public SettingsService Settings = App.Current.Services.GetService<SettingsService>() ?? throw new InvalidOperationException();
    
    // Readonly
    public ObservableCollection<TabBase> Tabs { get; } = new();
    public ObservableCollection<TabGroup> HiddenTabs { get; } = new();

    public readonly WindowEx WindowEx;
    
    // Property
    [Property(CustomGetExpression = $"{nameof(_HasOwner)} = {nameof(WindowEx)}.Owner.IsValid", SetVisibility = GeneratorVisibility.DoNotGenerate)]
    bool _HasOwner = false;
    
    Visibility SettingsButtonVisibility => HasOwner ? Visibility.Collapsed : Visibility.Visible;
    readonly DispatcherQueueTimer timer;
    readonly WindowMessageMonitor WindowMessageMonitor;
    public bool IsAltTabVisible;
    public MainWindow()
    {
        Title = "UnitedSets";
        InitializeComponent();
        MinWidth = 100;
        
        WindowEx = WindowEx.FromWindowHandle(WindowNative.GetWindowHandle(this));
        WindowMessageMonitor = new WindowMessageMonitor(WindowEx);
#if !UNPKG
		ExtendsContentIntoTitleBar = true;
#endif
        SetTitleBar(CustomDragRegion);

        timer = DispatcherQueue.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(500);

        TabBase.MainWindows.Add(WindowEx);

        AppWindow.Closing += OnWindowClosing;
        Activated += FirstRun;
        SizeChanged += OnMainWindowResize;
        CustomDragRegionUpdator.EffectiveViewportChanged += OnCustomDragRegionUpdatorCalled;
        WindowMessageMonitor.WindowMessageReceived += OnWindowMessageReceived;
        TabBase.OnUpdateStatusLoopComplete += OnLoopCalled;
        timer.Tick += OnTimerLoopTick;
        timer.Start();
		// --add-window-by-exe
		var toAdd = CLI.GetArrVal("add-window-by-exe");
		var editLastAddedWindow = CLI.GetFlag("edit-last-added");
		LeftFlyout.NoAutoClose = CLI.GetFlag("edit-no-autoclose");
		foreach (var itm in toAdd) {
			var procs = System.Diagnostics.Process.GetProcesses().Where(p=>p.ProcessName.Equals(itm, StringComparison.OrdinalIgnoreCase)).ToList();
			foreach (var proc in procs) {
				if (!proc.HasExited) {
					AddTab( WindowEx.FromWindowHandle(proc.MainWindowHandle));
				}
			}
		}
		if (editLastAddedWindow && Tabs.Count > 0)
			Tabs.Last().TabDoubleTapped(this, new Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs());
	}
}
