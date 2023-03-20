using System;
using EasyCSharp;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using UnitedSets.Classes;
using UnitedSets.Classes.Tabs;
using UnitedSets.Helpers;
using static UnitedSets.Services.PreservedHelpers;
using WinUIEx;
using UnitedSets.Windows;
using UnitedSets.Classes.PreservedDataClasses;
#pragma warning disable CS8625
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8605 // Unboxing a possibly null value.
#pragma warning disable CS8602 // 
#pragma warning disable CS8601 // Possible null reference assignment.
namespace UnitedSets.Services {

	public class PreservedTabDataService {
		public void init(MainWindow wind, MainWindow.CfgElements main_ui_elems) {
			this.MainWind = wind;
			this.main_ui_elems = main_ui_elems;
		}
		private MainWindow MainWind;
		private MainWindow.CfgElements main_ui_elems;


		public void SetPrimaryDesignProperties() {
			if (main_ui_elems.swapChain == null)
				return;

			
			var mainBg = cfg.Design.PrimaryBackgroundNonTranslucent;
			if (cfg.Design.UseTranslucentWindow == true) {
				if (App.Current.RequestedTheme== ApplicationTheme.Dark)
					mainBg = cfg.Design.PrimaryBackgroundDarkTheme;
				else
					mainBg = cfg.Design.PrimaryBackgroundLightTheme;
			}
			//main_ui_elems.MainAreaBorder.Background = ColorStrToBrush(mainBg);
			main_ui_elems.WindowBorder.BorderThickness = RectToThick(cfg.Design.BorderThickness);
			main_ui_elems.WindowBorder.Background = ColorStrToBrush(mainBg);;
			main_ui_elems.WindowBorder.CornerRadius = RectToCornerRadius(cfg.Design.BorderCorner);
			main_ui_elems.WindowBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
			main_ui_elems.WindowBorder.VerticalAlignment = VerticalAlignment.Stretch;
			var stops = (main_ui_elems.WindowBorder.BorderBrush as LinearGradientBrush)!.GradientStops;
			stops.First().Color = ConvertToColor(cfg.Design.BorderGradiant1);
			stops.Last().Color = ConvertToColor(cfg.Design.BorderGradiant2);
			main_ui_elems.MainAreaBorder.Margin = RectToThick(cfg.Design.MainMargin);
			//may want to look into a way to use _ImportSettings here but stop it from doing any tab loads etc just the design data.
		} //Right now almost all settings are applied in real time as changed.  This function is called at startup (for initial settings) and in theory might be called after something major like a theme change.

		private USConfig def_config => USConfig.def_config;
		public void LoadInitialSettingsAndTheme() {
			USConfig.LoadDefaultConfig();

			if (File.Exists(USConfig.DefaultConfigFile)) {
				var text = File.ReadAllText(USConfig.DefaultConfigFile);
				if (String.IsNullOrWhiteSpace(text) == false) {
					try {
						var loaded = Deserialize<SavedInstanceData>(text);
						//PropHelper.CopyNotNullPropertiesTo(loaded.Design, def_config.Design);
						//loaded.Design = null;
						loaded.Tabs = null;
						PropHelper.CopyNotNullPropertiesTo(loaded, def_config, true);
					} catch (Exception e) {
						Debug.WriteLine($"Should have better error handling for config load failtures err was: {e.ToString()}");
					}
				}
			}

			var cfg = def_config.CloneWithoutTabs();
			SettingsService.Settings = cfg;
			cfg.PropertyChanged += Cfg_PropertyChanged;
			if (cfg.Design.Theme != ElementTheme.Default && USConfig.FLAGS_THEME_CHOICE_ENABLED)
				Application.Current.RequestedTheme = cfg.Design.Theme == ElementTheme.Dark ? ApplicationTheme.Dark : ApplicationTheme.Light;
		}


		private void Cfg_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == nameof(USConfig.TitlePrefix))
				main_ui_elems?.TitleUpdate();
			
		}

		private void LogCfgError(String? msg, Exception? ex = null, [System.Runtime.CompilerServices.CallerFilePath] string source_file_path = "", [System.Runtime.CompilerServices.CallerMemberName] string member_name = "") {
			string GetCallerIdent([System.Runtime.CompilerServices.CallerMemberName] string member_name = "", [System.Runtime.CompilerServices.CallerFilePath] string source_file_path = "") {
				return GetFileNameFromFullPath(source_file_path) + "::" + member_name;

			}
			String GetFileNameFromFullPath(String source_file_path) {
				var file = source_file_path.Substring(source_file_path.LastIndexOf('\\') + 1);
				return file;
			}
			var str = $"{DateTime.Now.ToString("mm:ss.ffff")} {GetCallerIdent(member_name, source_file_path)}: {msg ?? ""}";
			if (ex != null)
				str += $" Exception {ex.GetType()}: {ex.ToString}";
			Debug.WriteLine(str);
		}
		/*
			We never override existing layouts or cells.   If only a cell is loaded and there is currently a free cell on the page it is placed there,  if there is a split layout loaded it will always load in a new tab.

	*/
		public async Task ImportSettings(String from_file) {
			var text = File.ReadAllText(from_file);
			if (String.IsNullOrWhiteSpace(text))
				return;

			var data = Deserialize<USConfig>(text);
			await _ImportSettings(data);
		}
		private async Task _ImportSettings(USConfig data) {
			try {
				PropHelper.CopyNotNullPropertiesTo(data.Design, cfg.Design);
				var tabs = data.Tabs;
				var design = data.Design;
				data.Design = null;
				data.Tabs = null;
				PropHelper.CopyNotNullPropertiesTo(data, cfg, true);
				data.Design = design;
				data.Tabs = tabs;
				StartingResults starts = new();



				DoOrThrow(data.TitlePrefix, (_) => main_ui_elems.TitleUpdate());
				DoOrThrow(data.TaskbarIco, (val) => MainWind.SetTaskBarIcon(Icon.FromFile(val)), File.Exists, () => new FileNotFoundException(data.TaskbarIco));

				if (data.Design != null) {
					var desdata = data.Design;
					var stops = (main_ui_elems.WindowBorder.BorderBrush as LinearGradientBrush)!.GradientStops;

					DoOrThrow(desdata.BorderGradiant1, ConvertToColor, (_, val) => stops.First().Color = val);
					DoOrThrow(desdata.BorderGradiant2, ConvertToColor, (_, val) => stops.Last().Color = val);
					DoOrThrow(desdata.BorderThickness, RectToThick, (_, val) => main_ui_elems.WindowBorder.BorderThickness = val);
					DoOrThrow(desdata.MainMargin, RectToThick, (_, val) => main_ui_elems.MainAreaBorder.Margin = val);
					DoOrThrow(desdata.WindowSize, (val) => MainWind.SetWindowSize(val.Width, val.Height), (val) => val.Width > 0 && val.Height > 0);
					//DoOrThrow(desdata.UseTranslucentWindow, (val) => (val ? TranslucentEnable : (Action)TranslucentDisable).Invoke());
				}
				if (tabs != null) {
					foreach (var tab in tabs)
						ApplySavedTab(starts, tab);
				}
				foreach (var start_arg in starts.items) {
					if (String.IsNullOrWhiteSpace(start_arg.startInfo?.FileName))
						continue;
					start_arg.running = Process.Start(start_arg.startInfo);
				}
				var maxExtra = starts.items.Max(a => a.ExtraWaitMS) ?? 0;
				await Task.Delay(3000 + maxExtra);//right now we wait for everything to start to do things in order, may change later
				foreach (var start_arg in starts.items) {
					var isCell = start_arg.cell != null;
					var hwnd = start_arg.running?.MainWindowHandle;//may need to make sure not yexited
					if (hwnd == null)
						hwnd = IntPtr.Zero;
					WinWrapper.Window? wind = null;
					if (hwnd != IntPtr.Zero)
						wind = WinWrapper.Window.FromWindowHandle((nint)hwnd);
					if (start_arg.NeedNewTab) {
						if (isCell == false) {
							if (wind != null) {
								start_arg.tab = MainWind.JustCreateTab((WinWrapper.Window)wind);
								start_arg.hwndHost = (start_arg.tab as HwndHostTab).HwndHost;
							} else {
								LogCfgError($"Window was not found for cmd {start_arg.startInfo.FileName} may need to wait longer? Running: {!start_arg.running.HasExited} its exit code: {start_arg.running.ExitCode}");
								continue;
							}
						} else {
							var cTab = new CellTab(start_arg.cell, true);
							start_arg.tab = cTab;
							var all_kids = start_arg.cell.AllSubCells.ToArray();
							foreach (var item in starts.items)
								if (item.cell != null && item.tab == null && all_kids.Contains(item.cell))
									item.tab = cTab;

						}
						MainWind.AddTab(start_arg.tab);
						//await Task.Delay(300);
						MainWind.TabView.SelectedItem = start_arg.tab;
						if (isCell) { //while selected need to do this to fix
							var cTab = start_arg.tab as CellTab;
							var subs = cTab._MainCell.SubCells;
							cTab._MainCell.SubCells = null;
							await Task.Delay(100);
							cTab._MainCell.SubCells = subs;
						}

					}
					if (isCell && wind != null) {
						start_arg.hwndHost = new OurHwndHost((CellTab)start_arg.tab, MainWind, (WinWrapper.Window)wind);
						start_arg.cell.RegisterWindow(start_arg.hwndHost);
					}

					ApplySavedCellData(start_arg.loadData, start_arg.hwndHost);
				}

			} catch (Exception e) {
				_ = MainWind.ShowMessageDialogAsync($"Unable to apply config due to exception: {e}");
				LogCfgError("Outer exception catch for config load", e);
			}
		}
#region Import Functions
		private void ApplySavedCellData(SavedCellData data, OurHwndHost hwndHost) {
			if (data == null || hwndHost == null)
				return;
			DoOrThrow(data.CustomTitle, (val) => hwndHost.parent.Tab.CustomTitle = val);
			DoOrThrow(data.Borderless, (val) => hwndHost.BorderlessWindow = val);
			DoOrThrow(data.CropEnabled, (val) => hwndHost.ActivateCrop = val);
			DoOrThrow(data.CropRect?.Top, (val) => hwndHost.CropTop = val);
			DoOrThrow(data.CropRect?.Left, (val) => hwndHost.CropLeft = val);
			DoOrThrow(data.CropRect?.Bottom, (val) => hwndHost.CropBottom = val);
			DoOrThrow(data.CropRect?.Right, (val) => hwndHost.CropRight = val);

		}

		private Classes.Cell? FindFreeCell(StartingResults starts, Classes.Cell cur) {
			if (cur.Empty)
				return cur;
			if (!cur.HasSubCells || cur.SubCells.Count() < 1)
				return null;

			foreach (var sub in cur.SubCells.ToArray()) {
				var c = FindFreeCell(starts, sub);
				if (c != null) return c;
			}
			return null;
		}
		private void ApplySavedTab(StartingResults starts, SavedTabData data) {
			bool? NeedNewTab = null;//null no tab at all, false = use currenty tab, true = new tab
			TabBase? tab = null;
			Cell? freeCell = null;
			if (MainWind.TabView.SelectedItem != null && MainWind.TabView.SelectedItem is CellTab cTab) {
				freeCell = FindFreeCell(starts, cTab._MainCell);
				if (freeCell != null)
					tab = cTab;
			} else
				NeedNewTab = true;

			DoOrThrow(data.CustomTitle, (val) => tab.CustomTitle = val);
			DoOrThrow(data.TabHeaderForeground, ColorStrToBrush, (_, val) => tab.HeaderForegroundBrush = val);
			DoOrThrow(data.TabHeaderBackground, ColorStrToBrush, (_, val) => tab.HeaderBackgroundBrush = val);
			if (data.CellOnly != null) {//cell only means it wasn't actually in a celltab but a normal hwndtab.
				NeedNewTab = true;
				freeCell = null;
				ApplyNewProc(starts, data.CellOnly, NeedNewTab, tab, freeCell);
			} else if (data.Split != null) {
				NeedNewTab = true;//always need new one for a the fist cellTab
				ApplyNewSplit(starts, data.Split, NeedNewTab, tab, null);

			}


		}

		private void ApplyNewSplit(StartingResults starts, SavedTabData.SavedSplitData data, bool? needNewTab, TabBase? tab, Cell? freeCell) {
			if (freeCell == null)
				needNewTab = true;
			if (freeCell == null)
				freeCell = new Cell(null, null, default);//ok to create doesnt register anything so safe if we dont use it

			if (data.Child != null) {
				ApplyNewProc(starts, data.Child, needNewTab, tab, freeCell);
				return;
			}
			if (data.Direction == null)
				return;

			freeCell ??= new Cell(null, null, default);//orientation wil lbe reset
			if (data.Count == 0)
				data.Count = 2;
			//Rather than just add extra cells to a new tab we will just assume they didn't update their size, still need count so that you can have free cells
			if (data.Children.Length > data.Count)
				data.Count = data.Children.Length;
			if (data.Direction == SavedTabData.SavedSplitData.SplitDirection.Horizontal)
				freeCell.SplitHorizontally(data.Count);
			else
				freeCell.SplitVertically(data.Count);
			if (data.Children?.Length > 0 == false)
				return;
			needNewTab ??= false;//not sure this is right
			starts.items.Add(new StartingResults.StartItem { cell = freeCell, NeedNewTab = needNewTab.Value, tab = tab });


			for (var x = 0; x < data.Children.Length; x++)
				ApplyNewSplit(starts, data.Children[x], false, tab, freeCell.SubCells.ElementAt(x));


		}

		private void ApplyNewProc(StartingResults starts, SavedCellData data, bool? NeedNewTab, TabBase? tab, Cell? freeCell) {
			if (data.process == null || String.IsNullOrWhiteSpace(data.process.Executable))
				return;
			var startInfo = new ProcessStartInfo(data.process.Executable);
			if (data.process.ExecutableArguments?.Length > 0) {
				foreach (var arg in data.process.ExecutableArguments)
					startInfo.ArgumentList.Add(arg);
			} else
				DoOrThrow(data.process.ExecutableArgString, (val) => startInfo.Arguments = val);
			DoOrThrow(data.process.WorkingDirectory, (val) => startInfo.WorkingDirectory = val);
			if (NeedNewTab != true && freeCell != null)
				NeedNewTab = false;
			else
				NeedNewTab = true;
			var add = new StartingResults.StartItem { startInfo = startInfo, cell = freeCell, tab = tab!, NeedNewTab = (bool)NeedNewTab, loadData = data };
			DoOrThrow(data.process.ExtraWaitMSOnLaunch, (val) => add.ExtraWaitMS = val);


			starts.items.Add(add);
		}

		#endregion
		private USConfig cfg => SettingsService.Settings;
		public async Task ResetSettings() {
			await _ImportSettings(def_config.CloneWithoutTabs());
			//def_config
		}
		public void ExportSettings(String filename, bool OnlyNonDefault, bool ExcludeTabs = false) {
			try {
				var exported = cfg.CloneWithoutTabs();
				var allTabs = MainWind.Tabs.ToArray();
				if (ExcludeTabs)
					allTabs = new TabBase[0];

				var tabsExported = new List<SavedTabData>();
				var allDataCells = new List<SavedCellData>();
				foreach (var tab in allTabs) {
					if (tab.IsDisposed)
						continue;
					var exp = new SavedTabData();
					exp.CustomTitle = tab.CustomTitle;
					exp.TabHeaderForeground = BrushToStr(tab.HeaderForegroundBrush);
					exp.TabHeaderBackground = BrushToStr(tab.HeaderBackgroundBrush);
					if (tab is HwndHostTab hTab) {
						exp.CellOnly = ExportCellDataFromHwndHost(hTab.HwndHost);
						allDataCells.Add(exp.CellOnly);
					} else if (tab is CellTab cTab) {
						exp.Split = ExportSplit(allDataCells, cTab._MainCell);
					}
					if (OnlyNonDefault)
						PropHelper.UnsetDstPropertiesEqualToSrcOrEmptyCollections(def_config.DefaultTabData, exp, true);
					tabsExported.Add(exp);//should probably check some property is actually set here could have it count and do allDataCells before hand to take care of them as well
				}

				foreach (var cell in allDataCells)
					PropHelper.UnsetDstPropertiesEqualToSrcOrEmptyCollections(def_config.DefaultCellData, cell, true);
				exported.Tabs = tabsExported.ToArray();

				if (OnlyNonDefault)
					PropHelper.UnsetDstPropertiesEqualToSrcOrEmptyCollections(def_config, exported, true);



				var serialized = Serialize(exported);

				File.WriteAllText(filename, serialized);
			} catch (Exception ex) {
				LogCfgError("Exporting data errror", ex);
			}

		}
		#region Export Functions
		private SavedTabData.SavedSplitData ExportSplit(List<SavedCellData> allDataCells, Cell cell) {
			var ret = new SavedTabData.SavedSplitData();
			var kids = new List<SavedTabData.SavedSplitData>();
			if (cell.SubCells?.Length > 0) {
				ret.Direction = cell.Orientation == Orientation.Horizontal ? SavedTabData.SavedSplitData.SplitDirection.Horizontal : SavedTabData.SavedSplitData.SplitDirection.Vertical;
				ret.Count = cell.SubCells.Count();
				foreach (var child in cell.SubCells)
					kids.Add(ExportSplit(allDataCells, child));
				ret.Children = kids.ToArray();
			} else if (cell.HasWindow && cell.CurrentCell is OurHwndHost host) {
				ret.Child = ExportCellDataFromHwndHost(host);
				allDataCells.Add(ret.Child);
			} else if (!cell.Empty)
				throw new Exception("Unknown cell type or maybe just no kids");
			return ret;

		}

		private SavedCellData? ExportCellDataFromHwndHost(OurHwndHost hwndHost) {
			var ret = new SavedCellData();
			ret.Borderless = hwndHost.BorderlessWindow;
			ret.CropEnabled = hwndHost.ActivateCrop;
			ret.CropRect = new(hwndHost.CropLeft, hwndHost.CropTop, hwndHost.CropRight, hwndHost.CropBottom);
			ret.CustomTitle = hwndHost.parent.Tab.CustomTitle;
			ret.process = new();
			var proc = ret.process;
			var info = hwndHost.GetOwnerProcessInfo();
			proc.ExecutableArgString = info.args;
			proc.Executable = info.cmd;
			proc.WorkingDirectory = "";
			return ret;
		}
		#endregion


	}
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS8603 //
#pragma warning restore CS8601 //
#pragma warning restore CS8604 //
#pragma warning restore CS8605
#pragma warning restore CS8602
#pragma warning restore CS8625
