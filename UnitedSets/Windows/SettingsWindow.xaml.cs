using CommunityToolkit.Mvvm.Input;
using Cube.UI.Controls.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnitedSets.Classes;
using UnitedSets.Controls;
using UnitedSets.Helpers;
using UnitedSets.Services;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UnitedSets.Windows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SettingsWindow : WindowEx {
	public USConfig cfg { get; set; }
	public SettingsService Settings;
	private MainWindow mainWindow;
	public Type ThemeOptionEnumType => typeof(ElementTheme);

	public SettingsWindow(SettingsService Settings, MainWindow mainWindow) {
		this.Settings = Settings;
		this.mainWindow = mainWindow;
		cfg = Settings.cfg;
		this.InitializeComponent();
		themeCntrl.Visibility = USConfig.FLAGS_THEME_CHOICE_ENABLED ? Visibility.Visible : Visibility.Collapsed;
		gridMain.DataContext = this;//we have to use normal bindings for anything with a converter as winui is broke af https://github.com/microsoft/microsoft-ui-xaml/issues/4966
		ExtendsContentIntoTitleBar = true;
		MicaHelper Mica = new();
		Mica.TrySetMicaBackdrop(this);
		SetTitleBar(AppTitleBar);
	}
	public List<string> theme_options { get; set; } = Enum.GetValues<ElementTheme>().Select(a=>a.ToString()).ToList();
	[RelayCommand]
	public void SaveDefaultSettings() {
		mainWindow.SaveCurSettingsAsDefault();
	}

}
