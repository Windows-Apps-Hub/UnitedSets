using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UnitedSets.Tabs;
using static UnitedSets.Configurations.PreservedHelpers;
using WindowsOG = Windows;
using Windows.Win32;
using WindowHoster;
using System.Diagnostics.CodeAnalysis;
using Get.Data.Collections.Linq;
using UnitedSets.Cells;
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // 
#pragma warning disable CS8601 // Possible null reference assignment.
namespace UnitedSets.Configurations;


public class PreservedTabDataService
{
    //public void SetPrimaryDesignProperties() {
    //	var mainBg = MainConfiguration.Design.PrimaryBackgroundNonTranslucent;
    //	if (MainConfiguration.Design.UseTranslucentWindow == true) {
    //		if (App.Current.RequestedTheme == ApplicationTheme.Dark)
    //			mainBg = MainConfiguration.Design.PrimaryBackgroundDarkTheme;
    //		else
    //			mainBg = MainConfiguration.Design.PrimaryBackgroundLightTheme;
    //	}
    //}

    private static USConfig DefaultConfiguration => USConfig.DefaultConfiguration;
    public bool LoadPreviousSessionData([NotNullWhen(true)] out USConfig? MainConfig)
    {
        if (File.Exists(USConfig.SessionSaveConfigFile))
        {
            USConfig.LoadDefaultConfig();
            LoadInitialSettingsAndTheme(out MainConfig);
            var text = File.ReadAllText(USConfig.SessionSaveConfigFile);
            if (!string.IsNullOrWhiteSpace(text))
            {
                try
                {
                    var loaded = Deserialize<SavedInstanceData>(text);
                    PropHelper.CopyNotNullPropertiesTo(loaded, MainConfig, true);
                    File.Delete(USConfig.SessionSaveConfigFile);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Should have better error handling for config load failtures err was: {e}");
                }
            }
            return true;
        }
        MainConfig = null;
        return false;
    }
    public Task FinalizeLoadAsync()
    {
        USConfig.LoadDefaultConfig();
        return _ImportSettings(MainConfiguration);
    }
    public void LoadInitialSettingsAndTheme(out USConfig MainConfig)
    {
        USConfig.LoadDefaultConfig();

        if (File.Exists(USConfig.DefaultConfigFile))
        {
            var text = File.ReadAllText(USConfig.DefaultConfigFile);
            if (String.IsNullOrWhiteSpace(text) == false)
            {
                try
                {
                    var loaded = Deserialize<SavedInstanceData>(text);
                    //PropHelper.CopyNotNullPropertiesTo(loaded.Design, def_config.Design);
                    //loaded.Design = null;
                    loaded.Tabs = null;
                    PropHelper.CopyNotNullPropertiesTo(loaded, DefaultConfiguration, true);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Should have better error handling for config load failtures err was: {e}");
                }
            }
        }

        var cfg = DefaultConfiguration.CloneWithoutTabs();
        MainConfig = cfg;
        if (cfg.Design.Theme != ElementTheme.Default && USConfig.FLAGS_THEME_CHOICE_ENABLED)
            Application.Current.RequestedTheme = cfg.Design.Theme == ElementTheme.Dark ? ApplicationTheme.Dark : ApplicationTheme.Light;
    }

    public USConfig MainConfiguration => UnitedSetsApp.Current.Configuration.MainConfiguration;

    private void LogCfgError(String? msg, Exception? ex = null, [System.Runtime.CompilerServices.CallerFilePath] string source_file_path = "", [System.Runtime.CompilerServices.CallerMemberName] string member_name = "")
    {
        string GetCallerIdent([System.Runtime.CompilerServices.CallerMemberName] string member_name = "", [System.Runtime.CompilerServices.CallerFilePath] string source_file_path = "")
        {
            return GetFileNameFromFullPath(source_file_path) + "::" + member_name;

        }
        String GetFileNameFromFullPath(String source_file_path)
        {
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
    public async Task ImportSettings(String from_file)
    {
        var text = File.ReadAllText(from_file);
        if (String.IsNullOrWhiteSpace(text))
            return;

        var data = Deserialize<USConfig>(text);
        await _ImportSettings(data);
    }
    public async Task ApplyFinalStartData(StartingResults starts, StartingResults.StartItem start_arg)
    {
        var isCell = start_arg.cell is not null;
        start_arg.running?.Refresh();//make sure we have latest
        var hwnd = start_arg.running?.MainWindowHandle;//may need to make sure not yexited
        if (hwnd == null)
            hwnd = IntPtr.Zero;
        WinWrapper.Windowing.Window? wind = null;
        if (hwnd != IntPtr.Zero)
            wind = WinWrapper.Windowing.Window.FromWindowHandle((nint)hwnd);
        if (start_arg?.NeedNewTab != false)
        {
            if (isCell == false)
            {
                if (wind == null && start_arg.startInfo.FileName.StartsWith(Utils.OUR_WINDOWS_STORE_APP_EXEC_PREFIX))
                {//so two ways we could do this, Main WindowHandle isnt set butthe process sdoes own some windows.  Our options are to get the windows for each thread and hopefully guess the right one in the proc, or we can go through all the ApplicationFrameHost.exe get the coreWIndow from them and find the one matching our pid
                    var hostProcs = Process.GetProcessesByName("ApplicationFrameHost");
                    foreach (var hostProc in hostProcs)
                    {
                        if (start_arg.running?.HasExited ?? true)
                            break;
                        try
                        {
                            var testWind = Utils.GetCoreWindowFromAppHostWindow(WinWrapper.Windowing.Window.FromWindowHandle(hostProc.MainWindowHandle));
                            if (testWind.OwnerProcess.Id == start_arg.running.Id)
                            {
                                wind = testWind;
                                break;
                            }
                        }
                        catch { }
                    }
                }
                if (wind != null)
                {
                    start_arg.tab = WindowHostTab.Create(wind.Value);
                    start_arg.hwndHost = (start_arg.tab as WindowHostTab).RegisteredWindow;
                }
                else
                {
                    LogCfgError($"Window was not found for cmd {start_arg.startInfo.FileName} may need to wait longer? Exited: {(start_arg.running?.HasExited)?.ToString() ?? "<unable to lunch>"} its exit code: {(start_arg.running?.HasExited ?? false ? start_arg.running.ExitCode.ToString() : "")}");
                    return;
                }
            }
            else
            {
                var cTab = new CellTab(start_arg.rootContainerCell, true);
                start_arg.tab = cTab;
                var all_kids = start_arg.rootContainerCell.AllSubCells.ToArray();
                foreach (var item in starts.items)
                    if (item.cell != null && item.tab == null && all_kids.Contains(item.cell))
                        item.tab = cTab;

            }
            if (start_arg.OnTabCreated != null)
            {
                foreach (var onCreated in start_arg.OnTabCreated)
                    onCreated(start_arg.tab);
            }
            UnitedSetsApp.Current.Tabs.Add(start_arg.tab);
            //await Task.Delay(300);
            UnitedSetsApp.Current.SelectedTab = start_arg.tab;
            //if (isCell) { //while selected need to do this to fix
            //	var cTab = start_arg.tab as CellTab;
            //	var subs = cTab.MainCell.SubCells;
            //	cTab.MainCell.SubCells = null;
            //	await Task.Delay(100);
            //	cTab.MainCell.SubCells = subs;
            //}

        }
        if (isCell && wind != null)
        {
            var win = wind.Value;
            start_arg.hwndHost = RegisteredWindow.Register(win, shouldBeHidden: true);
            if (start_arg.cell is EmptyCell ec)
            {
                ec.RegisterWindow(start_arg.hwndHost);
            }
            else
            {
                throw new InvalidCastException("start_arg.cell is not EmptyCell");
            }
        }

        ApplySavedCellData(start_arg.loadData, start_arg.hwndHost, start_arg.tab);
    }
    private async Task _ImportSettings(USConfig data)
    {
        try
        {
            SavedTabData[]? tabs;
            StartingResults starts;
            ApplyGlobalWindowData(data, out tabs, out starts);
            if (tabs != null)
            {
                foreach (var tab in tabs)
                    ApplySavedTab(starts, tab);
            }
            await LaunchAndWaitForProccesses(starts);

            foreach (var start_arg in starts.items)
            {
                await ApplyFinalStartData(starts, start_arg);
            }

        }
        catch (Exception e)
        {
            _ = UnitedSetsApp.Current.MainWindow.ShowMessageDialogAsync($"Unable to apply config due to exception: {e}");
            LogCfgError("Outer exception catch for config load", e);
        }
    }


    private static async Task LaunchAndWaitForProccesses(StartingResults starts)
    {
        foreach (var start_arg in starts.items)
        {
            if (String.IsNullOrWhiteSpace(start_arg.startInfo?.FileName))
                continue;
            if (start_arg.startInfo.FileName.StartsWith(Utils.OUR_WINDOWS_STORE_APP_EXEC_PREFIX))
            {
                var pkgId = start_arg.startInfo.FileName[Utils.OUR_WINDOWS_STORE_APP_EXEC_PREFIX.Length..];
                var manager = new WindowsOG.Management.Deployment.PackageManager();
                var pkg = manager.FindPackageForUser(string.Empty, pkgId);
                if (pkg is null)
                    // To do
                    continue;
                var allEntries = await pkg.GetAppListEntriesAsync();
                var entry = allEntries.First();
                //allEntries.First().LaunchForUserAsync(
                var manager2 = (WindowsOG.Win32.UI.Shell.IApplicationActivationManager)new WindowsOG.Win32.UI.Shell.ApplicationActivationManager();
                var args = start_arg.startInfo.Arguments;

                UI_Shell_IApplicationActivationManager_Extensions.ActivateApplication(manager2, entry.AppUserModelId, args, WindowsOG.Win32.UI.Shell.ACTIVATEOPTIONS.AO_NONE, out var pid);//sadly we can only launch without splash screen (AO_NOSPLASHSCREEN) if we enable debug on the app
                start_arg.running = Process.GetProcessById((int)pid);
            }
            else
                start_arg.running = Process.Start(start_arg.startInfo);
        }
        var maxExtra = starts.items.Max(a => a.ExtraWaitMS) ?? 0;
        await Task.Delay(3000 + maxExtra);//right now we wait for everything to start to do things in order, may change later
    }

    private void ApplyGlobalWindowData(USConfig data, out SavedTabData[]? tabs, out StartingResults starts)
    {
        PropHelper.CopyNotNullPropertiesTo(data.Design, MainConfiguration.Design);
        tabs = data.Tabs;
        var design = data.Design;
        data.Design = null;
        data.Tabs = null;
        PropHelper.CopyNotNullPropertiesTo(data, MainConfiguration, true);
        data.Design = design;
        data.Tabs = tabs;
        starts = new();
    }
    #region Import Functions

    private void ApplySavedCellData(SavedCellData data, RegisteredWindow hwndHost, TabBase tab)
    {
        if (data == null || hwndHost == null || tab == null)
            return;
        OnNotNull.Get(data.CustomTitle)?.Action(val => tab.CustomTitle = val);
        OnNotNull.Get(data.Borderless)?.Action(val => hwndHost.Properties.BorderlessWindow = val);
        OnNotNull.Get(data.CropEnabled)?.Action(val => hwndHost.Properties.ActivateCrop = val);
        OnNotNull.Get(data.CropRect)?.Action(val => hwndHost.Properties.CropRegion = val);
    }

    private EmptyCell? FindFreeCell(StartingResults starts, Cell cur)
    {
        if (cur is EmptyCell empty)
            return empty;
        if (cur is not ContainerCell container)
            return null;
        foreach (var sub in container.SubCells.AsEnumerable().ToArray())
        {
            var c = FindFreeCell(starts, sub);
            if (c != null) return c;
        }
        return null;
    }
    private void ApplySavedTab(StartingResults starts, SavedTabData data)
    {

        if (UnitedSetsApp.Current.SelectedTab is CellTab cTab)
        {
            starts.CurBuildItem.cell = FindFreeCell(starts, cTab.MainCell);
            if (starts.CurBuildItem.cell != null)
                starts.CurBuildItem.tab = cTab;
        }
        else
            starts.CurBuildItem.NeedNewTab = true;

        //starts.CurBuildItem.OnTabCreated
        OnNotNull.Get(data.CustomTitle)?.DelayAction(starts.CurBuildItem.OnTabCreated, (tab, val) => tab.CustomTitle = val);
        OnNotNull.Get(data.TabHeaderForeground)?.Convert(ColorStrToBrush)?.DelayAction(starts.CurBuildItem.OnTabCreated, (tab, val) => tab.HeaderForegroundBrush = val.result);
        OnNotNull.Get(data.TabHeaderBackground)?.Convert(ColorStrToBrush)?.DelayAction(starts.CurBuildItem.OnTabCreated, (tab, val) => tab.HeaderBackgroundBrush = val.result);


        if (data.CellOnly != null)
        {//cell only means it wasn't actually in a celltab but a normal hwndtab.
            starts.CurBuildItem.NeedNewTab = true;
            starts.CurBuildItem.cell = null;
            ApplyNewProc(starts, data.CellOnly);
        }
        else if (data.Split != null)
        {
            starts.CurBuildItem.NeedNewTab = true;//always need new one for a the fist cellTab
            ApplyNewSplit(starts, data.Split);

        }


    }
    private void ApplyNewSplit(StartingResults starts, SavedTabData.SavedSplitData data)
    {

        if (starts.CurBuildItem.cell == null)
        {
            starts.CurBuildItem.NeedNewTab = true;
            //ok to create doesnt register anything so safe if we dont use it
            starts.CurBuildItem.rootContainerCell = new ContainerCell(null, Orientation.Horizontal);
            starts.CurBuildItem.cell = new EmptyCell(starts.CurBuildItem.rootContainerCell);
        }

        if (data.Child != null)
        {
            ApplyNewProc(starts, data.Child);
            return;
        }
        if (data.Direction == null)
            return;

        if (data.Count == 0)
            data.Count = 2;
        //Rather than just add extra cells to a new tab we will just assume they didn't update their size, still need count so that you can have free cells
        if (data.Children.Length > data.Count)
            data.Count = data.Children.Length;
        if (data.Direction == SavedTabData.SavedSplitData.SplitDirection.Horizontal)
            ((EmptyCell)starts.CurBuildItem.cell).Split(data.Count, Orientation.Horizontal);
        else
            ((EmptyCell)starts.CurBuildItem.cell).Split(data.Count, Orientation.Vertical);
        if (data.Children?.Length > 0 == false)
            return;
        starts.CurBuildItem.NeedNewTab ??= false;//not sure this is right
        var ourCell = starts.CurBuildItem.cell;
        starts.AddCurBuildItemCreateNext();

        if (ourCell is ContainerCell containerCell)
        {
            for (var x = 0; x < data.Children.Length; x++)
            {
                starts.CurBuildItem.cell = containerCell.SubCells[x];
                starts.CurBuildItem.NeedNewTab = false;
                ApplyNewSplit(starts, data.Children[x]);
            }
        }
        else
        {
            throw new InvalidCastException("outCell is not ContainerCell");
        }
    }

    private void ApplyNewProc(StartingResults starts, SavedCellData data)
    {
        if (data.process == null || String.IsNullOrWhiteSpace(data.process.Executable))
            return;
        var startInfo = starts.CurBuildItem.startInfo = new ProcessStartInfo(data.process.Executable);
        if (data.process.ExecutableArguments?.Length > 0)
        {
            foreach (var arg in data.process.ExecutableArguments)
                startInfo.ArgumentList.Add(arg);
        }
        else
            OnNotNull.Get(data.process.ExecutableArgString)?.Action(val => startInfo.Arguments = val);
        OnNotNull.Get(data.process.WorkingDirectory)?.Action(val => startInfo.WorkingDirectory = val);
        if (starts.CurBuildItem.NeedNewTab != true && starts.CurBuildItem.cell != null)
            starts.CurBuildItem.NeedNewTab = false;
        else
            starts.CurBuildItem.NeedNewTab = true;
        starts.CurBuildItem.loadData = data;

        OnNotNull.Get(data.process.ExtraWaitMSOnLaunch)?.Action(val => starts.CurBuildItem.ExtraWaitMS = val);
        starts.AddCurBuildItemCreateNext();
    }

    #endregion
    public async Task ResetSettings()
    {
        await _ImportSettings(DefaultConfiguration.CloneWithoutTabs());
        //def_config
    }
    public void ExportSettings(string filename, bool OnlyNonDefault, bool ExcludeTabs = false)
    {
        try
        {
            var exported = MainConfiguration.CloneWithoutTabs();
            var allTabs = UnitedSetsApp.Current.Tabs.ToArray();
            if (ExcludeTabs)
                allTabs = [];

            var tabsExported = new List<SavedTabData>();
            var allDataCells = new List<SavedCellData>();
            foreach (var tab in allTabs)
            {
                if (tab.IsDisposed)
                    continue;
                var exp = new SavedTabData();
                exp.CustomTitle = tab.CustomTitle;
                exp.TabHeaderForeground = BrushToStr(tab.HeaderForegroundBrush);
                exp.TabHeaderBackground = BrushToStr(tab.HeaderBackgroundBrush);
                if (tab is WindowHostTab hTab)
                {
                    exp.CellOnly = ExportCellDataFromHwndHost(hTab.RegisteredWindow, hTab.CustomTitle);
                    allDataCells.Add(exp.CellOnly);
                }
                else if (tab is CellTab cTab)
                {
                    exp.Split = ExportSplit(allDataCells, cTab.MainCell, cTab.CustomTitle);
                }
                if (OnlyNonDefault)
                    PropHelper.UnsetDstPropertiesEqualToSrcOrEmptyCollections(DefaultConfiguration.DefaultTabData, exp, true);
                tabsExported.Add(exp);//should probably check some property is actually set here could have it count and do allDataCells before hand to take care of them as well
            }

            foreach (var cell in allDataCells)
                PropHelper.UnsetDstPropertiesEqualToSrcOrEmptyCollections(DefaultConfiguration.DefaultCellData, cell, true);
            exported.Tabs = tabsExported.ToArray();

            if (OnlyNonDefault)
                PropHelper.UnsetDstPropertiesEqualToSrcOrEmptyCollections(DefaultConfiguration, exported, true);



            var serialized = Serialize(exported);

            File.WriteAllText(filename, serialized);
        }
        catch (Exception ex)
        {
            LogCfgError("Exporting data errror", ex);
        }

    }
    #region Export Functions
    private SavedTabData.SavedSplitData ExportSplit(List<SavedCellData> allDataCells, Cell cell, string? customTitle)
    {
        var ret = new SavedTabData.SavedSplitData();
        var kids = new List<SavedTabData.SavedSplitData>();
        if (cell is ContainerCell cc && cc.SubCells.Count > 0)
        {
            ret.Direction = cc.Orientation == Orientation.Horizontal ? SavedTabData.SavedSplitData.SplitDirection.Horizontal : SavedTabData.SavedSplitData.SplitDirection.Vertical;
            ret.Count = cc.SubCells.Count;
            foreach (var child in cc.SubCells.AsEnumerable())
                kids.Add(ExportSplit(allDataCells, child, customTitle));
            ret.Children = kids.ToArray();
        }
        else if (cell is WindowCell wc)
        {
            ret.Child = ExportCellDataFromHwndHost(wc.Window, customTitle);
            allDataCells.Add(ret.Child);
        }
        else if (cell is not EmptyCell)
            throw new Exception("Unknown cell type or maybe just no kids");
        return ret;

    }

    private SavedCellData? ExportCellDataFromHwndHost(RegisteredWindow hwndHost, string? customTitle)
    {
        var ret = new SavedCellData();
        ret.Borderless = hwndHost.Properties.BorderlessWindow;
        ret.CropEnabled = hwndHost.Properties.ActivateCrop;
        ret.CropRect = hwndHost.Properties.CropRegion;
        ret.CustomTitle = customTitle;
        ret.process = new();
        var proc = ret.process;
        var info = Utils.GetOwnerProcessInfo(hwndHost.Window);
        proc.ExecutableArgString = info.args;
        proc.Executable = info.cmd;
        proc.WorkingDirectory = "";
        return ret;
    }
    #endregion


}

#pragma warning restore CS8601 //
#pragma warning restore CS8604 //
#pragma warning restore CS8602
