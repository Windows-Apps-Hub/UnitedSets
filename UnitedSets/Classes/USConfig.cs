using System;
using System.IO;
using Microsoft.UI.Xaml;
using UnitedSets.Classes.PreservedDataClasses;
using WindowsOG = Windows;
#pragma warning disable CS8625
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8605 // Unboxing a possibly null value.
#pragma warning disable CS8602 // 
#pragma warning disable CS8601 // Possible null reference assignment.
namespace UnitedSets.Classes {
	public class USConfig : SavedInstanceData {
		public USConfig CloneWithoutTabs() {
			return (USConfig)_CloneWithoutTabs();
		}
		public static bool FLAGS_THEME_CHOICE_ENABLED = false;// ughz https://github.com/microsoft/WindowsAppSDK/issues/3487 https://github.com/microsoft/microsoft-ui-xaml/issues/8249 although even setting it at the app level doesnt work
		public static string BaseProfileFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UnitedSets");
		public static string DefaultConfigFile => Path.Combine(BaseProfileFolder, "default.json");
		public static string RootLocation =>
#if UNPKG
			System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
#else
            WindowsOG.ApplicationModel.Package.Current.InstalledLocation.Path;
#endif

		public static USConfig def_config { get; private set; }
		internal static void LoadDefaultConfig() {
			if (!Directory.Exists(BaseProfileFolder))
				Directory.CreateDirectory(BaseProfileFolder);
			def_config = new() {
				TitlePrefix = "", TaskbarIco = "Assets/UnitedSets.ico", Tabs = new SavedTabData[0], Design = new() {
					BorderCorner = new(15, 5, 15, 5), BorderGradiant1 = "#9987C7FF", BorderGradiant2 = "#9900008B", BorderThickness = new(10),
					MainMargin = new(8, 0, 8, 8), PrimaryBackgroundLightTheme = "#DDFFFFFF", PrimaryBackgroundDarkTheme = "#DD000000", PrimaryBackgroundNonTranslucent = "White",
					WindowSize = new(1000, 800), UseTranslucentWindow = true, Theme = ElementTheme.Default, UseDXBorderTransparency = true,
				},
				DragAnywhere = true,
				DefaultCellData = new() { Borderless = false, CropEnabled = false, CropRect = new(0), CustomTitle = "" },
				DefaultTabData = new() { CustomTitle = "" }
			};
		}
	}
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS8603 //
#pragma warning restore CS8601 //
#pragma warning restore CS8604 //
#pragma warning restore CS8605
#pragma warning restore CS8602
#pragma warning restore CS8625
