using System.Threading;
using UnitedSets.WinForms;
using WinWrapper;
using Windows.Win32.UI.WindowsAndMessaging;
using Application = System.Windows.Forms.Application;
using EasyCSharp;
using System;
using System.Windows.Forms;
using System.Drawing;
using Windows.Win32;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.Graphics.Dwm;
using Windows.Win32.Foundation;
using UnitedSets.Helpers;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using System.Linq;
using System.Diagnostics;
using System.Reflection.Metadata;
using Windows.Win32.Storage.Xps;

namespace UnitedSets.Tabs;

partial class TabBase
{
    // Code is not yet ready for real use
#if false
    FormWndProc? SwitcherForm;
    Window SwitcherWindow;
    KeyboardHelper KeyboardHelper = new();
    void InitSwitcher()
    {
        KeyboardHelper.KeyboardPressed += KeyPressed;
        var thread = new Thread(() =>
        {
            SwitcherForm = new FormWndProc()
            {
                StartPosition = FormStartPosition.Manual,
                Location = new(0, 0),
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None,
                BackgroundImageLayout = ImageLayout.Zoom,
                BackColor = Color.Black,
                TransparencyKey = Color.Red,
                ShowInTaskbar = false,
                WindowLink = Windows.First()
            };
            SwitcherWindow = Window.FromWindowHandle(SwitcherForm.Handle);
            SwitcherForm.GotFocus += SwitcherWindowFocusCallback;
            Application.Run(SwitcherForm);
        })
        { Name = "Window Thread" };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        new Thread(() =>
        {
            while (true)
            {
                var WindowLink = Windows.First();
                HDC hdcScreen = PInvoke.GetDC(default);
                CreatedHDC hdc = PInvoke.CreateCompatibleDC(hdcScreen);
                var rc = WindowLink.Bounds;
                SwitcherForm?.Invoke(() => SwitcherForm.Size = rc.Size);
                HBITMAP hbmp = PInvoke.CreateCompatibleBitmap(hdcScreen,
                    rc.Width, rc.Height);
                PInvoke.SelectObject(new HDC(hdc.Value), new(hbmp.Value));

                //Print to memory hdc
                PInvoke.PrintWindow(WindowLink, new HDC(hdc.Value), PRINT_WINDOW_FLAGS.PW_CLIENTONLY);
                if (SwitcherForm is not null)
                    SwitcherForm.Bitmap = hbmp;
                PInvoke.DeleteDC(hdc);
                //PInvoke.DeleteObject(new(hbmp.Value));
                PInvoke.ReleaseDC(default, hdcScreen);
                PInvoke.DwmInvalidateIconicBitmaps(SwitcherWindow);
                Thread.Sleep(1000);
            }
        }).Start();
        //if (!IsSwitcherVisible) SwitcherWindow.ExStyle |= WINDOW_EX_STYLE.WS_EX_TOOLWINDOW;
    }

    private void KeyPressed(object? sender, KeyboardHelperEventArgs e)
    {
        if (
            e.KeyboardData.VirtualCode == (int)VIRTUAL_KEY.VK_TAB &&
            (e.KeyboardData.Flags & (int)KBDLLHOOKSTRUCT_FLAGS.LLKHF_ALTDOWN) != 0
        )
            PInvoke.DwmInvalidateIconicBitmaps(SwitcherWindow);
    }

    void ChangeSwitcherTitle()
    {
        SwitcherForm.Invoke(() =>
            SwitcherForm.Text = $"{Title} - United Sets"
        );
    }
    void ChangeSwitcherIcon()
    {
        if (BitmapIcon is null) return;
        //var newicon = Bitmap.FromHicon(icon.Value);
        //SwitcherWindow.SmallIconPtr = SwitcherWindow.LargeIconPtr = newicon.GetHicon();
        BitmapIcon.Save("D:\\file.png");
        SwitcherForm.Invoke(() =>
        {
            SwitcherWindow = Window.FromWindowHandle(SwitcherForm.Handle);
            SwitcherWindow.DwmSetWindowAttribute(DWMWINDOWATTRIBUTE.DWMWA_FORCE_ICONIC_REPRESENTATION, new BOOL(true));
            SwitcherWindow.DwmSetWindowAttribute(DWMWINDOWATTRIBUTE.DWMWA_HAS_ICONIC_BITMAP, new BOOL(true));
            var oldIcon = SwitcherForm.Icon;
            SwitcherForm.Icon = System.Drawing.Icon.FromHandle(BitmapIcon.GetHicon());
            //SwitcherForm.BackgroundImage = BitmapIcon;
            //SwitcherForm.Bitmap = BitmapIcon;
            SwitcherForm.ShowInTaskbar = true;
            // PInvoke.DwmInvalidateIconicBitmaps(SwitcherWindow);
            //unsafe
            //{
            //    PInvoke.DwmSetIconicLivePreviewBitmap(SwitcherWindow, new HBITMAP(BitmapIcon.GetHicon()), default, 0);
            //}
            SwitcherForm.SetIconicLivePreviewBitmap();
            SwitcherForm.SetIconicThumbnail();
            SwitcherForm.Width = 192;
            SwitcherForm.Height = 108;
            oldIcon?.Dispose();
        });
    }
    [Event(typeof(EventHandler))]
    void SwitcherWindowFocusCallback()
    {

    }
#else
    void InitSwitcher() { }
    void ChangeSwitcherTitle() { }
    void ChangeSwitcherIcon() { }
#endif
}
