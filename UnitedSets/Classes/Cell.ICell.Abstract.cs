using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using Window = WinWrapper.Window;
using WinUI3HwndHostPlus;
using UnitedSets.Windows;

namespace UnitedSets.Classes;
public abstract partial class ICell : INotifyPropertyChanged
{
    public abstract MainWindow MainWindow { get; }
    public abstract HwndHost? CurrentCell { get; set; }
    public abstract Cell[]? SubCells { get; set; }
    public abstract bool IsVisible { get; set; }

    public abstract Orientation Orientation { get; set; }

    public abstract bool HasWindow { get; }
    public abstract bool HasSubCells { get; }
    public abstract bool Empty { get; }
    public abstract bool HoverEffect { get; set; }

    public abstract void SplitHorizontally(int Amount);
    public abstract void SplitVertically(int Amount);
    public abstract void RegisterWindow(Window Window);
}