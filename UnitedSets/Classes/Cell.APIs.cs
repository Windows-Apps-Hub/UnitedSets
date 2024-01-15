using Microsoft.UI.Xaml.Controls;
using System.Linq;
using System;
using EasyCSharp;
using UnitedSets.UI.AppWindows;
using CommunityToolkit.Mvvm.Input;
using WindowHoster;

namespace UnitedSets.Classes;
partial class Cell
{
    /// <summary>
    /// Registers the <see cref="RegisteredWindow"/> as the current cell
    /// </summary>
    /// <param name="host">The <see cref="RegisteredWindow"/> to register</param>
    /// <exception cref="InvalidOperationException">Throws if the cell is not empty</exception>
    public partial void RegisterWindow(RegisteredWindow host);

    /// <summary>
    /// Splits the cell horizontally
    /// </summary>
    /// <param name="Amount">The amount of children cells</param>
    /// <exception cref="InvalidOperationException">Throws if the cell is not empty</exception>
    [RelayCommand]
    public partial void SplitHorizontally(int Amount);

    /// <summary>
    /// Splits the cell vertically
    /// </summary>
    /// <param name="Amount">The amount of children cells</param>
    /// <exception cref="InvalidOperationException">Throws if the cell is not empty</exception>
    [RelayCommand]
    public partial void SplitVertically(int Amount);

    public partial (Cell?, double renamining) GetChildFromPosition(double normalizedPosition);

    public partial (double In1, double In2) TranslatePositionFromChild((double In1, double In2) a, Cell childCell);

    /// <summary>
    /// Clones the current cell for the specified window
    /// </summary>
    /// <param name="NewWindow">The window new cell is refering to</param>
    /// <returns>The cloned cells</returns>
    /// <exception cref="NotImplementedException">Always throw</exception>
    [Obsolete("Not implemented", error:  true)]
    public partial Cell DeepClone(MainWindow NewWindow);

    private static partial Cell[] CraeteNCells(int Amount);
    /// <summary>
    /// Invokes PropertyChanged Event with specific property name
    /// </summary>
    /// <param name="PropertyName">Property name</param>
    protected partial void NotifyPropertyChanged(string PropertyName);
}