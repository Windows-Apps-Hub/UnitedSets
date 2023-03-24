using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Threading.Tasks;
using TransparentWinUIWindowLib;
using WinUIEx;
using Window = WinWrapper.Window;
using WinWrapper;
using WinUIPoint = Windows.Foundation.Point;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Foundation;
using Point = System.Drawing.Point;

namespace OutOfBoundsFlyout;

class OutOfBoundsFlyoutHost : WindowEx, IDisposable
{
    readonly SwapChainPanel swapChainPanel;
    readonly TransparentWindowManager trans_mgr;
    readonly Window Window;

    public OutOfBoundsFlyoutHost(bool addBorder)
    {
        swapChainPanel = new();
        trans_mgr = new(this, swapChainPanel, false);
        if (addBorder)
            swapChainPanel.Children.Add(new Border { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Margin = new(5) });

        IsResizable = false;
        IsTitleBarVisible = false;
        IsAlwaysOnTop = true;
        WindowContent = swapChainPanel;
        Window = Window.FromWindowHandle(this.GetWindowHandle());

        trans_mgr.AfterInitialize();
        Activate();
        this.Hide();
    }

    public void Dispose()
    {
        trans_mgr.Cleanup();
    }
    FlyoutBase? CurrentFlyout;
    public async Task ShowFlyoutAsync(FlyoutBase Flyout, Point pt, bool TouchPenCompatibilityMode, FlyoutPlacementMode placementMode = FlyoutPlacementMode.Auto, Rect? ExclusionRect = default)
    {
        CurrentFlyout = Flyout;
        var window = Window;
        var displayBounds = Display.FromPoint(pt).WorkingAreaBounds;
        window.Bounds = new()
        {
            X = displayBounds.X,
            Y = displayBounds.Y,
            Width = displayBounds.Width,
            Height = displayBounds.Height
        };
        Activate();

        await Task.Delay(50);//critical for sizing to be right, could cache scale info per monitor and watch for dpi changes

        var widthScale = window.ClientBounds.Width / swapChainPanel.ActualWidth;
        var heightScale = window.ClientBounds.Height / swapChainPanel.ActualHeight;
        var loc = new WinUIPoint(pt.X - displayBounds.X, pt.Y - displayBounds.Y);
        var scaled = loc;
        scaled.X /= widthScale;
        scaled.Y /= heightScale;
        window.ToString();

        Flyout.ShowAt(swapChainPanel, new()
        {
            ShowMode = FlyoutShowMode.Standard,
            Placement = placementMode,
            Position = scaled,
            ExclusionRect = ExclusionRect.HasValue ? new(
                (ExclusionRect.Value._x - displayBounds.X) / widthScale,
                (ExclusionRect.Value._y - displayBounds.Y) / heightScale,
                ExclusionRect.Value._width / widthScale,
                ExclusionRect.Value._height / heightScale
            ) : default
        });


        bool Opening = true;
        void closedEv(object? o, object e) => Opening = false;
        Flyout.Closed += closedEv;

        bool weVisible = true;

        if (TouchPenCompatibilityMode)
        {
            while (Opening) { await Task.Delay(100); }
            goto EndLoop;
        }
        while (Opening)
        {
            var startTick = Environment.TickCount;

            try
            {
                if (!weVisible)
                    window.SetExStyleFlag(WINDOW_EX_STYLE.WS_EX_TRANSPARENT, false);
                var elem = ElementFromCursor();
                var bounding = elem.CurrentBoundingRectangle;
                var width = bounding.right - bounding.left;
                var notOverMenu = width > 700 && elem.CurrentName == "Close";
                if (notOverMenu)
                {
                    window.SetExStyleFlag(WINDOW_EX_STYLE.WS_EX_TRANSPARENT, true);
                    weVisible = false;
                }
                else
                {
                    if (!weVisible)
                    {
                        weVisible = true;
                        window.SetExStyleFlag(WINDOW_EX_STYLE.WS_EX_TRANSPARENT, false);
                    }
                }
            }
            catch
            {
                await Task.Delay(100);
            }
            await Task.Delay(50);
        }
    EndLoop:
        this.Hide();
        CurrentFlyout = null;
    }
    public void CloseFlyout() => CurrentFlyout?.Hide();


    static Interop.UIAutomationClient.IUIAutomationElement ElementFromCursor()
    {
        // Convert mouse position from System.Drawing.Point to System.Windows.Point.
        var auto = new Interop.UIAutomationClient.CUIAutomation();
        //var desktop = auto.GetRootElement();
        var element = auto.ElementFromPoint(new Interop.UIAutomationClient.tagPOINT { x = Cursor.Position.X, y = Cursor.Position.Y });


        return element;
    }
}