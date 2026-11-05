using WindowRelative = WinWrapper.Windowing.WindowRelative;
using Cursor = WinWrapper.Input.Cursor;
using System.Diagnostics;
using Microsoft.UI.Dispatching;
using UnitedSets.Tabs;
using System.Runtime.CompilerServices;
using Thread = System.Threading.Thread;
using UnitedSets.Cells.Data;
using UnitedSets.PostProcessing;
using WinWrapper.Input;

namespace UnitedSets.UI.AppWindows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow
{
    TabBase? SelectedTabCache;
    System.Drawing.Rectangle CacheMiddleAreaBounds;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void CacheValue()
    {
        CacheMiddleAreaBounds = MainAreaPanel.GetMainAreaRectangle(Content);
        SelectedTabCache = SelectedTab;
    }

    /// <summary>
    /// Finds the empty cell at the given normalized position (0..1) inside <paramref name="cell"/>,
    /// taking the (user resizable) relative sizes of the cells into account.
    /// </summary>
    static EmptyCell? GetCellAtCursor((double X, double Y) pos, Cell cell)
    {
        switch (cell)
        {
            case EmptyCell ec:
                return ec;
            case ContainerCell cc when cc.SubCells.Count > 0:
                var horizontal = cc.Orientation is Orientation.Horizontal;
                var along = horizontal ? pos.X : pos.Y;
                var subCells = cc.SubCells.AsEnumerable().ToArray();
                var total = subCells.Sum(x => x.RelativeSize);
                if (total <= 0) return null;
                var start = 0d;
                foreach (var sub in subCells)
                {
                    var size = sub.RelativeSize / total;
                    if (along <= start + size || sub == subCells[^1])
                    {
                        var local = size <= 0 ? 0 : Math.Clamp((along - start) / size, 0, 1);
                        return GetCellAtCursor(horizontal ? (local, pos.Y) : (pos.X, local), sub);
                    }
                    start += size;
                }
                return null;
            default:
                return null;
        }
    }
    /// <summary>
    /// Finds the empty cell of the selected splitable tab that is under the cursor.
    /// </summary>
    internal (CellTab? tab, EmptyCell? cell) DetectCell()
    {
        if (SelectedTabCache is not CellTab cellTab) return (null, null);
        var cursorPos = Cursor.Position;
        // the main area (where cells are shown) in screen pixels
        var area = ToScreen(CacheMiddleAreaBounds);
        if (area.Width <= 0 || area.Height <= 0 || !area.Contains(cursorPos)) return (null, null);
        var normPos = (
            (double)(cursorPos.X - area.X) / area.Width,
            (double)(cursorPos.Y - area.Y) / area.Height
        );
        var cell = GetCellAtCursor(normPos, cellTab.MainCell);
        return cell is null ? (null, null) : (cellTab, cell);
    }
}
