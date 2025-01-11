using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using Get.EasyCSharp;
using Microsoft.UI.Xaml;
using UnitedSets.Tabs;
using UnitedSets.Mvvm.Services;
using WindowHoster;
using UnitedSets.Cells;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace UnitedSets.Configurations;
//putting in its own namespace as most things wont need to access these classes
public class StartingResults
{
    public class StartItem
    {
        public ContainerCell? rootContainerCell;
        public Cell? cell;
        public TabBase? tab;
        public List<Action<TabBase>> OnTabCreated = new();
        /// <summary>
        /// null no tab at all, false = use currenty tab, true = new tab
        /// </summary>
        public bool? NeedNewTab;
        public ProcessStartInfo startInfo;
        public int? ExtraWaitMS;
        public SavedCellData loadData;
        public Process? running;
        public RegisteredWindow hwndHost;

    }
    public StartItem CurBuildItem = new();
    public void AddCurBuildItemCreateNext()
    {
        items.Add(CurBuildItem);
        CurBuildItem = new();
    }
    public List<StartItem> items = new();
}
public partial class SavedWindowDesign : CloneableBase, INotifyPropertyChanged
{
    public bool? ShouldAutoSave { get; set; }
    public OurRect? BorderThickness { get; set; }
    public OurRect? BorderCorner { get; set; }
    public OurRect? MainMargin { get; set; }
    public string? PrimaryBackgroundLightTheme { get; set; }
    public string? PrimaryBackgroundDarkTheme { get; set; }
    public string? PrimaryBackgroundNonTranslucent { get; set; }

    public Size? WindowSize { get; set; }
    public bool? UseTranslucentWindow { get; set; }
    public string? BorderGradiant1 { get; set; }
    public string? BorderGradiant2 { get; set; }
    public bool? UseDXBorderTransparency { get; set; }
    [AutoNotifyProperty]
    public ElementTheme? _Theme;
    [AutoNotifyProperty]
    public USBackdrop _Backdrop;

    public event PropertyChangedEventHandler? PropertyChanged;
}
public partial class SavedInstanceData : CloneableBase, INotifyPropertyChanged
{
    protected SavedInstanceData _CloneWithoutTabs()
    {
        var ret = (SavedInstanceData)DeepClone();
        ret.Tabs = null;
        return ret;
    }
    public bool? DragAnywhere { get; set; }
    public SavedWindowDesign? Design { get; set; }
    public string? TaskbarIco { get; set; }

    [AutoNotifyProperty]
    public string? _TitlePrefix;

    [AutoNotifyProperty]
    public bool? _Autosave;

    [AutoNotifyProperty]
    public CloseTabBehaviors _CloseTabBehaviors;

    [AutoNotifyProperty]
    public UserMoveWindowBehaviors _UserMoveWindowBehavior;


    [AutoNotifyProperty]
    public bool? _HomePageBackground;

    [AutoNotifyProperty]
    public bool _BypassMinSize;

    public event PropertyChangedEventHandler? PropertyChanged;

    public SavedTabData[]? Tabs { get; set; }
    public Dictionary<string, SavedCellData> DefaultWindowStylesData { get; set; } = [];
    public SavedTabData? DefaultTabData { get; set; }
    public SavedCellData? DefaultCellData { get; set; }
}
public enum KEY_BIND_TYPES { WINDOW_HOVER_ADD, TAB_SWITCH_NEXT, TAB_SWITCH_PREV, SPLIT_CUR_PANE } //not yet used
public class SavedTabData : CloneableBase
{
    public string? CustomTitle { get; set; }
    public string? TabHeaderBackground { get; set; }
    public string? TabHeaderForeground { get; set; }
    public SavedSplitData? Split { get; set; } //can only have one of Split or CellOnly
    public SavedCellData? CellOnly { get; set; }//can only have one of Split or CellOnly, 
    public class SavedSplitData
    {//so the 
        public enum SplitDirection { Vertical, Horizontal }
        public SplitDirection? Direction { get; set; }//this is how we know if we are saving it or just the children
        public int Count { get; set; }
        public SavedSplitData[]? Children { get; set; }//can only have one of Child cell data or Children
        public SavedCellData? Child { get; set; }//can only have one of Child cell data or Children
    }
}
public class SavedCellData : CloneableBase
{
    public decimal? ParentCellPercent { get; set; }
    public int? Row { get; set; }
    public int? Column { get; set; }
    public CropRegion? CropRect { get; set; }
    public string? CustomTitle { get; set; }
    public bool? Borderless { get; set; }
    public bool? CropEnabled { get; set; }
    public SavedCellData TypedClone() => (SavedCellData)MemberwiseClone();

    public class ProcessInfo : CloneableBase
    {
        protected override void PostClone()
        {
            ExecutableArguments = (string[]?)ExecutableArguments?.Clone();
        }
        public string Executable { get; set; }
        public string[]? ExecutableArguments { get; set; }
        public string? ExecutableArgString { get; set; }
        public string? WorkingDirectory { get; set; }
        public int? ExtraWaitMSOnLaunch { get; set; }
    }
    public ProcessInfo? process { get; set; }
}
public class InMemoryCellData
{
    public InMemoryCellData(IntPtr HwndID, uint pid)
    {
        this.HwndID = HwndID;
        this.pid = pid;
    }
    public IntPtr HwndID;
    public uint pid;
    public DateTime LastDetached = DateTime.Now;
    public string CustomTitle;
    public bool Borderless;
    public bool CropEnabled;
    public CropRegion CropRect;

}
public struct OurRect
{
    public int? Left { get; set; }
    public int? Top { get; set; }
    public int? Right { get; set; }
    public int? Bottom { get; set; }
    public OurRect(int all) : this(all, all, all, all) { }
    public OurRect(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }
    public OurRect() { }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
