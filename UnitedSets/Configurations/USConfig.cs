using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.UI.Xaml;
using UnitedSets.Mvvm.Services;
using WindowsOG = Windows;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace UnitedSets.Configurations;

public partial class USConfig : SavedInstanceData
{
    public USConfig CloneWithoutTabs()
    {
        return (USConfig)_CloneWithoutTabs();
    }
    public static bool FLAGS_THEME_CHOICE_ENABLED { get; } = true;// ughz https://github.com/microsoft/WindowsAppSDK/issues/3487 https://github.com/microsoft/microsoft-ui-xaml/issues/8249 although even setting it at the app level doesnt work
    public static string BaseProfileFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UnitedSets");
    public static string DefaultConfigFile => Path.Combine(BaseProfileFolder, "default.json");
    public static string AppDataPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "UnitedSets"
    );
    public static string SessionSaveConfigFile => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "UnitedSets",
            "prevSession.json"
        );
    public static string RootLocation =>
#if UNPKG
			System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
#else
        WindowsOG.ApplicationModel.Package.Current.InstalledLocation.Path;
#endif

    static USConfig? _DefaultConfiguration;
    public static USConfig DefaultConfiguration
    {
        get
        {
            if (_DefaultConfiguration is null)
                LoadDefaultConfig();
            return _DefaultConfiguration;
        }
    }
    [MemberNotNull(nameof(_DefaultConfiguration))]
    internal static void LoadDefaultConfig()
    {
        if (!Directory.Exists(BaseProfileFolder))
            Directory.CreateDirectory(BaseProfileFolder);
        _DefaultConfiguration = new()
        {
            TitlePrefix = "",
            TaskbarIco = "Assets/UnitedSets.ico",
            Tabs = [],
            Design = new()
            {
                BorderCorner = new(15, 5, 15, 5),
                BorderGradiant1 = "#9987C7FF",
                BorderGradiant2 = "#9900008B",
                BorderThickness = new(10),
                MainMargin = new(8, 0, 8, 8),
                PrimaryBackgroundLightTheme = "#DDFFFFFF",
                PrimaryBackgroundDarkTheme = "#DD000000",
                PrimaryBackgroundNonTranslucent = "White",
                WindowSize = new(1000, 800),
                UseTranslucentWindow = true,
                Theme = ElementTheme.Default,
                UseDXBorderTransparency = true,
                Backdrop = USBackdrop.Mica
            },
            DragAnywhere = true,
            CloseTabBehaviors = CloseTabBehaviors.DetachWindow,
            UserMoveWindowBehavior = UserMoveWindowBehaviors.DetachWindow,
            DefaultWindowStylesData = [],
            DefaultCellData = new() { Borderless = false, CropEnabled = false, CropRect = default, CustomTitle = "" },
            DefaultTabData = new() { CustomTitle = "" }
        };
    }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
