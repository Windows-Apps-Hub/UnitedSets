using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Drawing;
using Colors = System.Drawing.Color;
using System.Threading.Tasks;
using TransparentWinUIWindowLib;
//using UnitedSets.Classes;
using WinUIEx;
using WinWrapper;
using SysDiaProcess = System.Diagnostics.Process;//grrrrrrrrrrrrrr
using Window = WinWrapper.Window;
using WinUIPoint = Windows.Foundation.Point;
using Microsoft.UI.Xaml.Media;
using Windows.Win32;
using CWP_FLAGS = Windows.Win32.UI.WindowsAndMessaging.CWP_FLAGS;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Diagnostics;
using UnitedSets.Windows;
using Windows.Win32.UI.WindowsAndMessaging;
using WinUIEx;
using Windows.Foundation;
using Point = System.Drawing.Point;

namespace UnitedSets.Windows.Flyout.OutOfBoundsFlyout
{
    static class OutOfBoundsFlyoutSystem
    {
        public static void Initialize(bool addBorder)
        {
            Instance = new(addBorder);
        }
        public static void Dispose()
        {
            Instance.Dispose();
        }
        static OutOfBoundsFlyoutHost Instance;
        static private Window appWindow;
        private class OutOfBoundsFlyoutHost : WindowEx, IDisposable
        {
            public readonly SwapChainPanel swapChainPanel;
            readonly TransparentWindowManager trans_mgr;
            public readonly Window Window;

            public OutOfBoundsFlyoutHost(bool addBorder)
            {
                swapChainPanel = new();
                trans_mgr = new(this, swapChainPanel, false);
                if (addBorder)
                    swapChainPanel.Children.Add(new Border { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Margin = new(5) });
                //IsMinimizable = false;
                //IsMaximizable = false;
                appWindow = Window.FromWindowHandle(SysDiaProcess.GetCurrentProcess().MainWindowHandle);

                IsResizable = false;
                IsTitleBarVisible = false;
                //IsAlwaysOnTop = true;
                WindowContent = swapChainPanel;
                Window = Window.FromWindowHandle(this.GetWindowHandle());
                //Window.SetTopMost();
                trans_mgr.AfterInitialize();
                Activate();
                this.Hide();
            }

            public void Dispose()
            {
                Instance.Close();
                trans_mgr.Cleanup();
            }
        }
        public static double widthScale;
        public static double heightScale;
        private enum CUR_OVER { Us, Caller, External }
        private static bool firstRun = true;
        private static Interop.UIAutomationClient.IUIAutomationElement ElementFromCursor()
        {
            // Convert mouse position from System.Drawing.Point to System.Windows.Point.
            var auto = new Interop.UIAutomationClient.CUIAutomation();
            //var desktop = auto.GetRootElement();
            var element = auto.ElementFromPoint(new Interop.UIAutomationClient.tagPOINT { x = Cursor.Position.X, y = Cursor.Position.Y });


            return element;
        }
        public static async Task ShowAsync(FlyoutBase Flyout, Point pt, bool TouchPenCompatibilityMode, FlyoutPlacementMode placementMode = FlyoutPlacementMode.Auto, Rect? ExclusionRect = default)
        {
            if (firstRun)
            {
                //appWindow = Window.FromWindowHandle(SysDiaProcess.GetCurrentProcess().MainWindowHandle);
            }

            firstRun = false;
            var instance = Instance;
            var window = instance.Window;
            var displayBounds = Display.FromPoint(pt).WorkingAreaBounds;
            window.Bounds = new()
            {
                X = displayBounds.X,
                Y = displayBounds.Y,
                Width = displayBounds.Width,
                Height = displayBounds.Height
            };
            instance.Activate();
            await Task.Delay(50);//critical for sizing to be right, could cache scale info per monitor and watch for dpi changes
            widthScale = window.ClientBounds.Width / instance.swapChainPanel.ActualWidth;
            heightScale = window.ClientBounds.Height / instance.swapChainPanel.ActualHeight;
            var loc = new WinUIPoint(pt.X - displayBounds.X, pt.Y - displayBounds.Y);
            var scaled = loc;
            scaled.X /= widthScale;
            scaled.Y /= heightScale;
            window.ToString();

            //Flyout = new MenuFlyout
            //{
            //    Items =
            //    {
            //        new MenuFlyoutItem { Text = "This" },
            //        new MenuFlyoutItem { Text = "Is" },
            //        new MenuFlyoutItem { Text = "Just" },
            //        new MenuFlyoutItem { Text = "A" },
            //        new MenuFlyoutItem { Text = "Test" }
            //    }
            //};


            Flyout.ShowAt(instance.swapChainPanel, new()
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
            Point lastPos = new(-1, -1);
            var uiThread = PInvoke.GetCurrentThreadId();
            bool weVisible = true;
            var ourColor = Colors.LightGreen;
            var notUsColor = Colors.LightYellow;
            var appColor = Colors.LightPink;
            CUR_OVER over;
            var usHandle = window.Handle.Value;
            var callerHandle = appWindow.Handle.Value;
            long totalTicks = 0;
            var totalLoop = 0;
            if (TouchPenCompatibilityMode)
            {
                while (Opening) { await Task.Delay(100); }
                goto EndLoop;
            }
            while (Opening)
            {
                var startTick = Environment.TickCount;

                
                {
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

                }
                totalLoop++;

                totalTicks += Environment.TickCount - startTick;
                await Task.Delay(50);
            }
        EndLoop:
            instance.Hide();
        }
    }
    static partial class Extension
    {
        public static float GetScale(this Microsoft.UI.Xaml.Window elementOwnerWindow)
            => Window.FromWindowHandle(elementOwnerWindow.GetWindowHandle()).GetScale();
        public static float GetScale(this Window elementOwnerWindowEx)
            => elementOwnerWindowEx.CurrentDisplay.ScaleFactor / 100.0f;
        public static RectangleF GetBoundsRelativeToScreen(this UIElement Element, Microsoft.UI.Xaml.Window elementOwnerWindow)
            => Element.GetBoundsRelativeToScreen(elementOwnerWindow, Window.FromWindowHandle(elementOwnerWindow.GetWindowHandle()));
        public static RectangleF GetBoundsRelativeToScreen(this UIElement Element, Microsoft.UI.Xaml.Window elementOwnerWindow, Window elementOwnerWindowEX)
        {
            var bounds = Element.GetBoundsRelativeToWindow(elementOwnerWindow, elementOwnerWindowEX);
            var windowBounds = elementOwnerWindowEX.Bounds;
            bounds.X += windowBounds.X;
            bounds.Y += windowBounds.Y;
            return bounds;
        }
        public static RectangleF GetBoundsRelativeToWindow(this UIElement Element, Microsoft.UI.Xaml.Window elementOwnerWindow)
            => Element.GetBoundsRelativeToWindow(elementOwnerWindow, Window.FromWindowHandle(elementOwnerWindow.GetWindowHandle()));
        public static RectangleF GetBoundsRelativeToWindow(this UIElement Element, Microsoft.UI.Xaml.Window elementOwnerWindow, Window elementOwnerWindowEX)
        {
            var Pt = Element.TransformToVisual(elementOwnerWindow.Content).TransformPoint(
                new WinUIPoint(0, 0)
            );

            var scale = elementOwnerWindowEX.CurrentDisplay.ScaleFactor / 100.0f;
            var size = Element.ActualSize;
            return new(Pt._x * scale, Pt._y * scale, size.X * scale, size.Y * scale);
        }

    }
}
