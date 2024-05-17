using EasyCSharp;
using Microsoft.UI.Windowing;
using System.Collections.ObjectModel;
using WinRT.Interop;
using Microsoft.UI.Xaml;
using System;
using WindowEx = WinWrapper.Window;
using Windows.Win32;
using UnitedSets.Services;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using WinUIEx.Messaging;
using Microsoft.UI.Dispatching;
using UnitedSets.Classes.Tabs;
using UnitedSets.Windows.Flyout;
using UnitedSets.Classes;
using System.Linq;
using System.Collections.Generic;
using Microsoft.UI;
using TransparentWinUIWindowLib;
using System.Threading.Tasks;
using IT = Windows.Foundation;
using System.Text.Json;
using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

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

	//protected volatile bool TabCollectionChanged=true;
	//private TabBase[]? GetUpdatedTabCollection() {
	//	if (!TabCollectionChanged)
	//		return null;

	//	TabCollectionChanged = false;
	//	return Tabs.ToArray();
	//}
	public void AddTab(TabBase tab, int? index=null) {
		WireTabEvents(tab);
		if (index != null)
			Tabs.Insert(index.Value, tab);
		else
			Tabs.Add(tab);
		//TabCollectionChanged = true;
	}
	public void RemoveTab(TabBase tab) {
		Tabs.Remove(tab);
		UnwireTabEvents(tab);
		//TabCollectionChanged = true;
	}
	public IEnumerable<TabBase> GetTabsAndClear() {
		var ret = Tabs.ToArray();
		foreach (var tab in ret)
			RemoveTab(tab);
		return ret;
	}
	
	public TabBase? FindTabByWindow(WinWrapper.Window window) {
		return Tabs.ToArray().FirstOrDefault(tab=>tab.Windows.Contains(window));
	}
	public (TabGroup? group, TabBase? tab) FindHiddenTabByWindow(WinWrapper.Window window) {
		foreach (var tabg in HiddenTabs.ToArray()) {
			var tab = tabg.Tabs.ToArray().FirstOrDefault(tab => tab.Windows.Contains(window));
			if (tab != null)
				return (tabg, tab);
		}

		return (null, null);
	}
	//private Func<TabBase?> GetUpdatedTabCollectionDelegate() {
	//	var info = typeof(MainWindow).GetMethod(nameof(GetUpdatedTabCollection), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
	//	var args = new ParameterExpression[0];
	//	var lambdaExpression = Expression.Call(Expression.Constant(this), info!, args);
	//	return Expression.Lambda<Func<TabBase?>>(lambdaExpression, args).Compile();//compiled expressions have a one time setup cost but should be near equivalent of bare metal code for each call
	//}
	TransparentWindowManager? trans_mgr;
	    public MainWindow(PreservedTabDataService persistantService)
    {
		this.persistantService = persistantService;
		
		InitializeComponent();
		if (cfg.Design.Theme != null && cfg.Design.Theme != ElementTheme.Default && USConfig.FLAGS_THEME_CHOICE_ENABLED) {
			(this.Content as FrameworkElement).RequestedTheme = cfg.Design.Theme.Value;
			///not sure why settng the requested theme doesnt seem to work
			//MainAreaBorder.RequestedTheme = this.RootGrid.RequestedTheme = swapChainPanel.RequestedTheme = cfg.Design.Theme.Value;
			
		}
		ui_configs = new() { TitleUpdate = UpdateTitle, swapChain=this.swapChainPanel};
		persistantService.init(this,ui_configs);

		
		
        Title = "UnitedSets";
		if (cfg.Design.UseDXBorderTransparency == true)
			TransparentSetup();

		MinWidth = 100;
        
        WindowEx = WindowEx.FromWindowHandle(WindowNative.GetWindowHandle(this));
        WindowMessageMonitor = new WindowMessageMonitor(WindowEx);
		ExtendsContentIntoTitleBar = true;
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
	}
		private void TransparentSetup() {
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
		ui_configs.WindowBorder = new Border() {HorizontalAlignment = HorizontalAlignment.Stretch,VerticalAlignment = VerticalAlignment.Stretch  };
		ui_configs.WindowBorder.BorderBrush = new LinearGradientBrush(new GradientStopCollection { new GradientStop { Offset = 1 }, new GradientStop { Offset = 0 } }, 45);
		//ui_configs.WindowBorder.RequestedTheme = MainAreaBorder.RequestedTheme;
		Grid.SetColumnSpan(ui_configs.WindowBorder, 50);
		Grid.SetRowSpan(ui_configs.WindowBorder, 50);
		Canvas.SetZIndex(ui_configs.WindowBorder, -5);
		ui_configs.MainAreaBorder = MainAreaBorder;
		RootGrid.Children.Insert(0, ui_configs.WindowBorder);
		persistantService.SetPrimaryDesignProperties();
		trans_mgr = new(this, swapChainPanel, cfg.DragAnywhere == true);
		trans_mgr.AfterInitialize();
		
#pragma warning restore CS8604 // Possible null reference argument.
		#pragma warning restore CS8602 // Dereference of a possibly null reference.
	}
	public class CfgElements {
		public Border? WindowBorder;
		public Border? MainAreaBorder;
		public SwapChainPanel swapChain;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public Action TitleUpdate { init; get; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	}
	
	private CfgElements ui_configs;

	private void UpdateTitle() {
		var prefix = cfg.TitlePrefix + " ";
		if (TabView.SelectedIndex != -1)
			prefix += $"{Tabs[TabView.SelectedIndex].Title} (+{Tabs.Count - 1} Tabs) - ";

		Title = $"{prefix.TrimStart()}United Sets";

	}

	public USConfig cfg => Settings.cfg;

	private PreservedTabDataService persistantService;
	
	private void WireTabEvents(TabBase tab) {
		tab.RemoveTab += TabRemoveRequest;
		tab.ShowFlyout += TabShowFlyoutRequest;
		tab.ShowTab += TabShowRequest;
	}



	private void UnwireTabEvents(TabBase tab) {
		tab.RemoveTab -= TabRemoveRequest;
		tab.ShowFlyout -= TabShowFlyoutRequest;
		tab.ShowTab -= TabShowRequest;
	}

}
