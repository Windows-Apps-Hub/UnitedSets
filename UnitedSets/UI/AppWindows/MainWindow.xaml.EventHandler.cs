using WinUIEx.Messaging;
using Microsoft.UI.Windowing;
using UnitedSets.Cells.Data;

namespace UnitedSets.UI.AppWindows;

public sealed partial class MainWindow
{
    #region Tabs
    private partial void TabSelectionChanged();

    #region Tabs Dragging
    private partial void OnDropOverCell(EmptyCell cell, nint hwnd);
    #endregion

    #endregion


    #region Window
    [Event(typeof(EventHandler<WindowMessageEventArgs>))]
    private partial void OnWindowMessageReceived(WindowMessageEventArgs e);

    [Event(typeof(TypedEventHandler<AppWindow, AppWindowClosingEventArgs>))]
    private partial void OnWindowClosing(AppWindowClosingEventArgs e);
    #endregion
}
