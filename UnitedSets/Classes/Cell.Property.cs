using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using EasyCSharp;
using WinUI3HwndHostPlus;
using UnitedSets.Windows;

namespace UnitedSets.Classes;
partial class Cell
{

    [AutoNotifyProperty(OnChanged = nameof(OnCurrentCellChanged))]
    OurHwndHost? _CurrentCell;

    [AutoNotifyProperty(OnChanged = nameof(OnSubCellsUpdate))]
    Cell[]? _SubCells;

    [Property(OnChanged = nameof(OnVisibleChanged))]
    bool _IsParentVisible = true;

    [Property(CustomGetExpression = "IsParentVisible && _IsVisible", OnChanged = nameof(OnVisibleChanged))]
    bool _IsVisible;

    [AutoNotifyProperty]
    bool _HoverEffect;

    [AutoNotifyProperty]
    Orientation _Orientation;

    [AutoNotifyProperty]
    double _RelativeSize = 1;
}