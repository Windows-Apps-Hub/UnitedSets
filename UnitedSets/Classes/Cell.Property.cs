using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using Get.EasyCSharp;
using UnitedSets.UI.AppWindows;
using CommunityToolkit.Mvvm.ComponentModel;
using WindowHoster;

namespace UnitedSets.Classes;
partial class Cell
{

    [AutoNotifyProperty(OnChanged = nameof(OnCurrentCellChanged))]
    RegisteredWindow? _CurrentCell;
    
    [AutoNotifyProperty(OnChanged = nameof(OnSubCellsUpdate))]
    Cell[]? _SubCells;

    [AutoNotifyProperty]
    bool _HoverEffect;

    [AutoNotifyProperty]
    Orientation _Orientation;

    [AutoNotifyProperty]
    double _RelativeSize = 1;
}