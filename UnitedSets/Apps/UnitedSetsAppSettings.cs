using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Composition;
using EnumsNET;
using System.ComponentModel.DataAnnotations;
using UnitedSets.Apps;
using UnitedSets.Configurations;
using UnitedSets.Settings;

namespace UnitedSets.Mvvm.Services;

public partial class UnitedSetsAppSettings
{
    private static USConfig Configuration => UnitedSetsApp.Current.Configuration.PersistantService.MainConfiguration;
    public UnitedSetsAppSettings()
    {
        AllSettings = (Setting[])[
            Autosave = new(
                () => Configuration.Autosave ?? true, x => Configuration.Autosave = x
            ) {
                Title = "Autosave Settings",
                Description = "Automatically saves the settings as you edit them. Turn this off if you only want the current session to have this setting.",
                Icon = SymbolEx.Save,
                UserInterfaceVisibility = Visibility.Visible
            },
            Layout = new(
                () => Configuration.UnitedSetsLayout, x => Configuration.UnitedSetsLayout = x,
                Enum.GetValues<UnitedSetsLayouts>()
            )
            {
                Title = "Layout",
                Description = "Layout of the set windows",
                Icon = SymbolEx.Move,
                UserInterfaceVisibility = Visibility.Visible,
                OOBEUserInterfaceVisibility = Visibility.Visible,
                RequiresRestart = true
            },
            CloseTabBehavior = new(
                () => Configuration.CloseTabBehaviors, x => Configuration.CloseTabBehaviors = x,
                Enum.GetValues<CloseTabBehaviors>()
            )
            {
                Title = "Closing tab behavior",
                Description = "What happens when you close a tab?",
                Icon = SymbolEx.ChromeClose,
                UserInterfaceVisibility = Visibility.Visible,
                OOBEUserInterfaceVisibility = Visibility.Visible
            },
            UserMoveWindowBehavior = new(
                () => Configuration.UserMoveWindowBehavior, x => Configuration.UserMoveWindowBehavior = x,
                Enum.GetValues<UserMoveWindowBehaviors>()
            )
            {
                Title = "Moving added window behavior",
                Description = "What happens when you drag a tabbed window by its title bar?",
                Icon = SymbolEx.Move,
                UserInterfaceVisibility = Visibility.Visible,
                OOBEUserInterfaceVisibility = Visibility.Visible
            },
            DragToGroupMode = new(
                () => Configuration.DragToGroupMode ?? DragToGroupModes.HoldShift, x => Configuration.DragToGroupMode = x,
                Enum.GetValues<DragToGroupModes>()
            )
            {
                Title = "Grouping windows by dragging",
                Description = "Drag a window by its title bar onto another window to group them into a set, or onto a set to add it as a tab",
                Icon = SymbolEx.Add,
                UserInterfaceVisibility = Visibility.Visible,
                OOBEUserInterfaceVisibility = Visibility.Visible
            },
            GroupWithAnyWindow = new(
                () => Configuration.GroupWithAnyWindow ?? true, x => Configuration.GroupWithAnyWindow = x
            )
            {
                Title = "Group with any window",
                Description = "Dropping a window onto another window (not only onto a set) creates a new set with both windows, like Groupy",
                Icon = SymbolEx.NewWindow,
                UserInterfaceVisibility = Visibility.Visible
            },
            TaskbarTabThumbnails = new(
                () => Configuration.TaskbarTabThumbnails ?? true, x => Configuration.TaskbarTabThumbnails = x
            )
            {
                Title = "Taskbar thumbnail for each tab",
                Description = "Hovering a set on the taskbar shows every tab (like Groupy); the set uses the icon of its selected app",
                Icon = SymbolEx.ViewAll,
                UserInterfaceVisibility = Visibility.Visible
            },
            StartWithWindows = new(
                () => Helpers.StartupHelper.IsEnabled, x => Helpers.StartupHelper.IsEnabled = x
            )
            {
                Title = "Start with Windows",
                Description = "Start United Sets X in the notification area when you sign in. It keeps running in the background until you choose Exit from its tray icon.",
                Icon = SymbolEx.Home,
                UserInterfaceVisibility = Visibility.Visible
            },
            TabDroppedOutsideBehavior = new(
                () => Configuration.TabDroppedOutsideBehavior ?? TabDroppedOutsideBehaviors.ReleaseWindow, x => Configuration.TabDroppedOutsideBehavior = x,
                Enum.GetValues<TabDroppedOutsideBehaviors>()
            )
            {
                Title = "Dragging a tab out",
                Description = "What happens when a tab is dropped outside of a tab bar",
                Icon = SymbolEx.NewWindow,
                UserInterfaceVisibility = Visibility.Visible
            },
            CloseSetWithTabsBehavior = new(
                () => Configuration.CloseSetWithTabsBehavior ?? CloseSetWithTabsBehaviors.Ask, x => Configuration.CloseSetWithTabsBehavior = x,
                Enum.GetValues<CloseSetWithTabsBehaviors>()
            )
            {
                Title = "Closing a set window",
                Description = "What happens to the windows of a set when you close it. United Sets X itself keeps running in the notification area (use Exit in its tray menu to quit).",
                Icon = SymbolEx.ChromeClose,
                UserInterfaceVisibility = Visibility.Visible
            },
            TabSwitchHotkeys = new(
                () => Configuration.TabSwitchHotkeys ?? true, x => Configuration.TabSwitchHotkeys = x
            )
            {
                Title = "Tab switching hotkeys",
                Description = "Win + ` switches to the next tab, Win + Shift + ` to the previous tab of the active set",
                Icon = SymbolEx.KeyboardClassic,
                UserInterfaceVisibility = Visibility.Visible
            },
            //TransculentWindow = new(
            //    () => cfg.Design.UseTranslucentWindow ?? false, x => cfg.Design.UseTranslucentWindow = x
            //)
            //{
            //    Title = "Use Translucent Bordering/Background",
            //    Description = "Allow the opacity of our background/borders (not the pinned apps) to be translucent",
            //    Icon = SymbolEx.PPSOneLandscape,
            //    RequiresRestart = true
            //},
            HomePageInfo = new(
                () => Configuration.HomePageBackground ?? true, x => Configuration.HomePageBackground = x
            )
            {
                Title = "Home Page Information",
                Description = "Shows or hide the hotkeys, tips, and other information",
                Icon = SymbolEx.Home,
                UserInterfaceVisibility = Visibility.Visible,
            },
            BypassMinimumSize = new(() => Configuration.BypassMinSize, x => Configuration.BypassMinSize = x) {
                Title = "Bypass Minimum Size",
                Description = $"Allows resizing the window down to {
                    Constants.BypassMinWidth}x{Constants.BypassMinHeight
                    } (Normal minimum size is {
                    Constants.MinWidth}x{Constants.MinHeight})",
                Icon = SymbolEx.ResizeMouseSmall,
                UserInterfaceVisibility = Visibility.Visible
            },
            BackdropMode = new(
                () => Configuration.Design!.Backdrop, x => Configuration.Design!.Backdrop = x, Enums.GetValues<USBackdrop>()
            ) {
                Title = "Window Backdrop",
                Icon = SymbolEx.Color,
                UserInterfaceVisibility = Visibility.Visible
            },
            Theme = new(
                () => Configuration.Design!.Theme ?? ElementTheme.Default, x => Configuration.Design!.Theme = x,
                Enums.GetValues<ElementTheme>()
            )
            {
                Title = "Theme Override",
                Description = "Override the windows theme",
                Icon = SymbolEx.Light,
                RequiresRestart = true,
                UserInterfaceVisibility = Visibility.Visible
            },
            Background = new(
                () => PreservedHelpers.ConvertToColor(Configuration.Design!.PrimaryBackgroundNonTranslucent ?? ""),
                x => Configuration.Design!.PrimaryBackgroundNonTranslucent = PreservedHelpers.ColorToStr(x)
            )
            {
                Title = "Background Color",
                Description = "Background color to be applied after the selected backdrop",
                Icon = SymbolEx.MailFill,
                UserInterfaceVisibility = Visibility.Visible
            },
            BorderColor = new(
                () => PreservedHelpers.ConvertToColor(Configuration.Design!.BorderColor ?? ""),
                x => Configuration.Design!.BorderColor = PreservedHelpers.ColorToStr(x)
            )
            {
                Title = "Border Color",
                Icon = SymbolEx.DeviceMonitorNoPic,
                UserInterfaceVisibility = Visibility.Visible
            },
            BorderThickness = new(
                () => PreservedHelpers.RectToThick(Configuration.Design!.BorderThickness),
                x => Configuration.Design!.BorderThickness = PreservedHelpers.ThickToRect(x)
            )
            {
                Title = "Thickness",
                Description = "Sets the thickness (useful with border color)",
                Icon = SymbolEx.ChromeMaximize,
                UserInterfaceVisibility = Visibility.Visible
            },
            CornerRadius = new(
                () => PreservedHelpers.RectToCornerRadius(Configuration.Design!.BorderCorner),
                x => Configuration.Design!.BorderCorner = PreservedHelpers.RectToCornerRadius(x)
            )
            {
                Title = "Corner Radius",
                Description = "Sets the corner radius (useful with border color)",
                Icon = SymbolEx.Checkbox,
                UserInterfaceVisibility = Visibility.Visible
            },
            WindowTitlePrefix = new(
                () => Configuration.TitlePrefix ?? "", x => Configuration.TitlePrefix = x
            )
            {
                Title = "Window Title Prefix",
                Description = "Prefix that shows up before the normal UnitedSets title",
                Icon = SymbolEx.AlignLeft,
                PlaceholderText = "None - Normal Title Mode",
                UserInterfaceVisibility = Visibility.Visible
            },
            TaskbarIcon = new(
                () => Configuration.TaskbarIco ?? "", x => Configuration.TaskbarIco = x
            )
            {
                Title = "Taskbar Icon",
                Description = "Override the taskbar icon",
                Icon = SymbolEx.Picture
            },
            //BorderGraident1 = new(
            //    () => PreservedHelpers.ConvertToColor(Configuration.Design!.BorderGradiant1 ?? ""),
            //    x => Configuration.Design!.BorderGradiant1 = PreservedHelpers.ColorToStr(x)
            //)
            //{
            //    Title = "[Transparent Window] Border Gradient (1)",
            //    Description = "Sets the border gradient if the window is transparent",
            //    Icon = SymbolEx.DeviceMonitorNoPic
            //},
            //BorderGraident2 = new(
            //    () => PreservedHelpers.ConvertToColor(Configuration.Design!.BorderGradiant2 ?? ""),
            //    x => Configuration.Design!.BorderGradiant2 = PreservedHelpers.ColorToStr(x)
            //)
            //{
            //    Title = "[Transparent Window] Border Gradient (2)",
            //    Description = "Sets the border gradient if the window is transparent",
            //    Icon = SymbolEx.DeviceMonitorNoPic
            //},
            MainMargin = new(
                () => PreservedHelpers.RectToThick(Configuration.Design!.BorderThickness),
                x => Configuration.Design!.BorderThickness = PreservedHelpers.ThickToRect(x)
            )
            {
                Title = "Main Margin",
                Description = "Sets the main margin",
                Icon = SymbolEx.ChromeMaximize,
                UserInterfaceVisibility = Visibility.Collapsed
            },
            InitialWindowSize = new(
                () => Configuration.Design!.WindowSize,
                x => Configuration.Design!.WindowSize = x
            )
            {
                Title = "Initial Window Size",
                Description = "Sets the startup window size",
                Icon = SymbolEx.WindowsInsider,
                RequiresRestart = true,
                UserInterfaceVisibility = Visibility.Collapsed
            },
        ];
        foreach (var setting in AllSettings)
            setting.PropertyChanged += delegate
            {
                if (Autosave.Value)
                {
                    UnitedSetsApp.Current.Configuration.SaveCurSettingsAsDefault();
                }
            };
    }
    public IReadOnlyList<Setting> AllSettings { get; }

    public SelectSetting<CloseTabBehaviors> CloseTabBehavior { get; }
    public OnOffSetting Autosave { get; }
    public SelectSetting<UnitedSetsLayouts> Layout { get; }
    public SelectSetting<UserMoveWindowBehaviors> UserMoveWindowBehavior { get; }
    public SelectSetting<DragToGroupModes> DragToGroupMode { get; }
    public OnOffSetting GroupWithAnyWindow { get; }
    public OnOffSetting StartWithWindows { get; }
    public OnOffSetting TaskbarTabThumbnails { get; }
    public SelectSetting<TabDroppedOutsideBehaviors> TabDroppedOutsideBehavior { get; }
    public OnOffSetting TabSwitchHotkeys { get; }
    public SelectSetting<CloseSetWithTabsBehaviors> CloseSetWithTabsBehavior { get; }
    public OnOffSetting BypassMinimumSize { get; }
    public OnOffSetting HomePageInfo { get; }
    //public OnOffSetting TransculentWindow { get; }
    public TextSetting WindowTitlePrefix { get; }
    public TempLinkSetting<string> TaskbarIcon { get; }
    public ColorPickerSetting BorderColor { get; }
    //public TempLinkSetting<Color> BorderGraident1 { get; }
    //public TempLinkSetting<Color> BorderGraident2 { get; }
    public ThicknessSetting BorderThickness { get; }
    public CornerRadiusSetting CornerRadius { get; }
    public TempLinkSetting<Thickness> MainMargin { get; }
    public TempLinkSetting<SizeInt?> InitialWindowSize { get; }
    public SelectSetting<ElementTheme> Theme { get; }

    public SelectSetting<USBackdrop> BackdropMode { get; }
    public ColorPickerSetting Background { get; }

    SettingsWindow? s_window;
    [RelayCommand]
    public void LaunchSettings(MainWindow mainWindow)
    {
        try
        {
            if (s_window is not null)
            {
                s_window.Activate();
                return;
            }
        }
        catch (COMException)
        {
        }
        CreateWindow(mainWindow);
        s_window.Activate();
    }

    [MemberNotNull(nameof(s_window))]
    private void CreateWindow(MainWindow mainWindow)
    {
        s_window = new(this, mainWindow);
        s_window.Closed += (_, _) => s_window = new(this, mainWindow);
    }
}
public enum USBackdrop
{
    Mica,
    Acrylic,
    Tabbed,
    Transparent
}
public enum CloseTabBehaviors
{
    [Display(Name = "Detach Window")]
    DetachWindow,
    [Display(Name = "Close Window")]
    CloseWindow
}
public enum UserMoveWindowBehaviors
{
    [Display(Name = "Detach Window")]
    DetachWindow,
    Ignore
}
public enum DragToGroupModes
{
    [Display(Name = "Hold Shift while dragging (like Groupy)")]
    HoldShift,
    [Display(Name = "Hold Ctrl while dragging")]
    HoldCtrl,
    [Display(Name = "Hold Alt while dragging")]
    HoldAlt,
    [Display(Name = "Hold Tab while dragging (press it after you start dragging)")]
    HoldTab,
    [Display(Name = "No key: drop on the title bar or tab bar")]
    WithoutKey,
    [Display(Name = "Disabled")]
    Disabled
}
public enum CloseSetWithTabsBehaviors
{
    [Display(Name = "Ask every time")]
    Ask,
    [Display(Name = "Release the windows")]
    ReleaseWindows,
    [Display(Name = "Close the windows")]
    CloseWindows
}
public enum TabDroppedOutsideBehaviors
{
    [Display(Name = "Release the window")]
    ReleaseWindow,
    [Display(Name = "Open in a new set window")]
    NewSetWindow
}
public enum UnitedSetsLayouts
{
    [Display(Name = "Regular")]
    HorizontalTabs,
    [Display(Name = "Vertical tabs")]
    VerticalTabs,
    [Display(Name = "(Experimental) Vertical tabs (Full Height)")]
    VerticalTabsFull
}
static class BackdropHelper
{
    public static Microsoft.UI.Xaml.Media.SystemBackdrop GetSystemBackdrop(this USBackdrop backdrop)
        => backdrop switch
        {
            USBackdrop.Acrylic => new InfiniteSystemBackdrop<DesktopAcrylicController>(),
            USBackdrop.Mica => new InfiniteSystemBackdrop<MicaController>(),
            USBackdrop.Tabbed => new InfiniteSystemBackdrop<MicaController>(x => x.Kind = MicaKind.BaseAlt),
            USBackdrop.Transparent => new WinUIEx.TransparentTintBackdrop(),
            _ => throw new ArgumentOutOfRangeException(nameof(backdrop))
        };
}


public class InfiniteSystemBackdrop<T> : Microsoft.UI.Xaml.Media.SystemBackdrop where T : ISystemBackdropControllerWithTargets, new()
{
    public InfiniteSystemBackdrop() { }
    public InfiniteSystemBackdrop(Action<T> act) { action = act; }
    Action<T>? action;
    public bool IsInfinite { get; set; } = true;
    T? controller;

    protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop connectedTarget, XamlRoot xamlRoot)
    {
        // Call the base method to initialize the default configuration object.
        base.OnTargetConnected(connectedTarget, xamlRoot);

        // This example does not support sharing MicaSystemBackdrop instances.
        if (controller is not null)
        {
            throw new Exception("This controller cannot be shared");
        }

        controller = new T();
        action?.Invoke(controller);
        // Set configuration.
        SystemBackdropConfiguration defaultConfig = GetDefaultSystemBackdropConfiguration(connectedTarget, xamlRoot);

        controller.SetSystemBackdropConfiguration(GetConfig(defaultConfig));
        // Add target.
        controller.AddSystemBackdropTarget(connectedTarget);
    }
    SystemBackdropConfiguration GetConfig(SystemBackdropConfiguration a)
        => IsInfinite ? new()
        {
            IsInputActive = IsInfinite,
            IsHighContrast = a.IsHighContrast,
            HighContrastBackgroundColor = a.HighContrastBackgroundColor,
            Theme = a.Theme
        } : a;
    protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop disconnectedTarget)
    {
        base.OnTargetDisconnected(disconnectedTarget);

        controller?.RemoveSystemBackdropTarget(disconnectedTarget);
        controller = default;
    }
    protected override void OnDefaultSystemBackdropConfigurationChanged(ICompositionSupportsSystemBackdrop target, XamlRoot xamlRoot)
    {
        if (target == null)
            return;
        try
        {
            SystemBackdropConfiguration defaultConfig = GetDefaultSystemBackdropConfiguration(target, xamlRoot);
            controller?.SetSystemBackdropConfiguration(GetConfig(defaultConfig));
        }
        catch { }
    }
}
