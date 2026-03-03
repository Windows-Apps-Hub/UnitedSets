global using System;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.ComponentModel;
global using System.Diagnostics.CodeAnalysis; // nullable attributes
global using System.Linq;
global using System.Threading.Tasks;

global using PointInt = System.Drawing.Point;
global using SizeInt = System.Drawing.Size;
global using Path = System.IO.Path;

global using COMException = System.Runtime.InteropServices.COMException;

global using Windows.Foundation;

global using Size = Windows.Foundation.Size;
global using Color = Windows.UI.Color;

global using Microsoft.UI;
global using Microsoft.UI.Input;
global using Microsoft.UI.Xaml;
global using Microsoft.UI.Xaml.Controls;
global using Microsoft.UI.Xaml.Media;
global using Microsoft.UI.Xaml.Shapes;
global using Microsoft.UI.Xaml.Input;

global using CommunityToolkit.WinUI;
global using RelayCommandAttribute = CommunityToolkit.Mvvm.Input.RelayCommandAttribute;

global using Get.Data.Bindings.Linq;
global using Get.Data.Collections;
global using Get.Data.Collections.Linq;
global using Get.Data.Collections.Update;
global using Get.Data.Helpers;
global using Get.Data.Properties;
global using Get.EasyCSharp;
global using Get.Symbols;
global using Get.UI.Data;
global using Get.XAMLTools;


global using static Get.Data.Properties.AutoTyper;
global using static Get.Data.XACL.QuickBindingExtension;
global using static Get.UI.Data.QuickCreate;
global using WindowEx = WinWrapper.Windowing.Window;

global using QuickMarkup.SourceGen;
global using QuickMarkup.Infra;

global using static QuickMarkup.Infra.QuickRefs;

// Usage in QuickMarkup
global using Cube.UI.Icons;
global using UnitedSets.UI.AppWindows;
global using UnitedSets.UI.Controls;
global using UnitedSets.UI.FlyoutModules;
global using UnitedSets.QuickMarkup;
