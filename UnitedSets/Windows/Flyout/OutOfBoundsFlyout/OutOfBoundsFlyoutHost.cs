using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Drawing;
using System.Threading.Tasks;
using TransparentWinUIWindowLib;
using WinUIEx;
using WinWrapper;
using Window = WinWrapper.Window;
using WinUIPoint = Windows.Foundation.Point;
using System.Diagnostics;

namespace UnitedSets.Windows.Flyout.OutOfBoundsFlyout
{
    static class OutOfBoundsFlyoutSystem
    {
        public static void Initialize()
        {
            _ = LazyInstance.Value;
        }
        public static void Dispose()
        {
            Instance.Dispose();
        }
        readonly static Lazy<OutOfBoundsFlyoutHost> LazyInstance = new(() => new OutOfBoundsFlyoutHost());
        static OutOfBoundsFlyoutHost Instance => LazyInstance.Value;
        private class OutOfBoundsFlyoutHost : WindowEx, IDisposable
        {
            public readonly SwapChainPanel swapChainPanel;
            readonly TransparentWindowManager trans_mgr;
            public readonly Window Window;
            public OutOfBoundsFlyoutHost()
            {
                swapChainPanel = new();
                trans_mgr = new(this, swapChainPanel, false);
                MinWidth = 0;
                MinHeight = 0;
                Width = 1;
                Height = 1;
                IsMinimizable = false;
                IsMaximizable = false;
                IsResizable = false;
                IsTitleBarVisible = false;
                //var appwindow = this.GetAppWindow();
                //var presenter = Microsoft.UI.Windowing.OverlappedPresenter.Create();
                //presenter.SetBorderAndTitleBar(false, false);
                //presenter.IsResizable = false;
                //appwindow.SetPresenter(presenter);
                Content = swapChainPanel;
                Window = Window.FromWindowHandle(this.GetWindowHandle());
                //Window.SetTopMost();
                trans_mgr.AfterInitialize();
                Activate();
            }

            public void Dispose()
            {
                trans_mgr.Cleanup();
            }
        }
        public static async Task ShowAsync(FlyoutBase Flyout, Point pt, FlyoutPlacementMode placementMode = FlyoutPlacementMode.Auto)
        {
            var instance = Instance;
            var Window = instance.Window;
            var displayBounds = Display.FromPoint(pt).WorkingAreaBounds;
            Window.Bounds = new()
            {
                X = displayBounds.X,
                Y = displayBounds.Y,
                Width = displayBounds.Width,
                Height = displayBounds.Height
            };
            instance.Activate();
            Flyout.ShowAt(instance.swapChainPanel, new()
            {
                ShowMode = FlyoutShowMode.Standard,
                Placement = placementMode,
                Position = new WinUIPoint(pt.X - displayBounds.X, pt.Y - displayBounds.Y)
            });
            bool Opening = true;
            void closedEv(object? o, object e) => Opening = false;
            Flyout.Closed += closedEv;
            Point lastPos = new(-1, -1);
            var ourProcId = Window.ThreadProcessId;
            bool weVisible = true;
            while (Opening)
            {
                var pos = Cursor.Position;
                
                //Window.SetExStyleFlag(WINDOW_EX_STYLE.WS_EX_TRANSPARENT, false);
                weVisible = true;
                var under = Window.FromLocation(pos);
                Debug.WriteLine(under.Root);
                if (under.Root == Window)
                {
                    
                } else
                {
                    //Window.SetExStyleFlag(WINDOW_EX_STYLE.WS_EX_TRANSPARENT, true);
                    weVisible = false;
                    //Debug.WriteLine("Transparent");
                }
                await Task.Delay(100);
            }
            instance.Hide();
            //{

            //    var hwnd1 = PInvoke.ChildWindowFromPointEx(,pt,CWP_FLAGS.);
            //    if (hwnd1 == instance.Window)
            //        instance.Window.SetExStyleFlag(WINDOW_EX_STYLE.WS_EX_TRANSPARENT, false); //assuming we were transparent
            //    var hwnd2 = Window.FromLocation(Cursor.Position);
            //    if (hwnd2 != instance.Window) //ok we are not actually over us lets go transparent
            //        instance.Window.SetExStyleFlag(WINDOW_EX_STYLE.WS_EX_TRANSPARENT, true);
            //    await Task.Delay(100);
            //}
        }
    }
}
