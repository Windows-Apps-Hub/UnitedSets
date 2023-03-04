// This class PerPixelAlphaForm were taken and modified from https://www.codeproject.com/Articles/1822/Per-Pixel-Alpha-Blend-in-C

using WinWrapper;
using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.Foundation;

namespace UnitedSets.WinForms;


public class PerPixelAlphaForm : Form {
    readonly Window NativeWindow;
    public PerPixelAlphaForm() {
        FormBorderStyle = FormBorderStyle.None;
        Load += new EventHandler(PerPixelAlphaFormLoad);
        NativeWindow = Window.FromWindowHandle(Handle);
    }
    public void PerPixelAlphaFormLoad(object? sender, EventArgs e) {
        NativeWindow.ExStyle = WINDOW_EX_STYLE.WS_EX_LAYERED | WINDOW_EX_STYLE.WS_EX_TRANSPARENT;
    }

    public unsafe void SetBitmap(Bitmap bitmap, byte opacity = 255) {
        HDC screenDc = PInvoke.GetDC(default);
        CreatedHDC memDc = PInvoke.CreateCompatibleDC(screenDc);
        HBITMAP hBitmap = default;
        HBITMAP oldBitmap = default;

        try {
            hBitmap = (HBITMAP)bitmap.GetHbitmap(Color.FromArgb(0));  // grab a GDI handle from this GDI+ bitmap
            oldBitmap = (HBITMAP)PInvoke.SelectObject((HDC)memDc.Value, (HGDIOBJ)hBitmap.Value).Value;

            SIZE size = new(Width, Height);
            Point pointSource = new(0, 0);
            Point topPos = new(Left, Top);
            BLENDFUNCTION blend = new() {
                BlendOp = (byte)PInvoke.AC_SRC_OVER,
                BlendFlags = 0,
                SourceConstantAlpha = opacity,
                AlphaFormat = (byte)PInvoke.AC_SRC_ALPHA
            };

            PInvoke.UpdateLayeredWindow(
                NativeWindow.Handle,
                screenDc,
                &topPos,
                &size,
                (HDC)memDc.Value,
                &pointSource,
                new(0),
                &blend,
                UPDATE_LAYERED_WINDOW_FLAGS.ULW_ALPHA
            );
        } finally {
            PInvoke.ReleaseDC(default, screenDc);
            if (hBitmap != IntPtr.Zero) {
                PInvoke.SelectObject((HDC)memDc.Value, (HGDIOBJ)oldBitmap.Value);
                //Windows.DeleteObject(hBitmap); // The documentation says that we have to use the Windows.DeleteObject... but since there is no such method I use the normal DeleteObject from Win32 GDI and it's working fine without any resource leak.
                PInvoke.DeleteObject((HGDIOBJ)hBitmap.Value);
            }
            PInvoke.DeleteDC(memDc);
        }
    }

    public unsafe void SetBitmap(HBITMAP hBitmap, byte opacity = 255)
    {
        HDC screenDc = PInvoke.GetDC(default);
        CreatedHDC memDc = PInvoke.CreateCompatibleDC(screenDc);
        HBITMAP oldBitmap = default;

        try
        {
            oldBitmap = (HBITMAP)PInvoke.SelectObject((HDC)memDc.Value, (HGDIOBJ)hBitmap.Value).Value;

            SIZE size = new(Width, Height);
            Point pointSource = new(0, 0);
            Point topPos = new(Left, Top);
            BLENDFUNCTION blend = new()
            {
                BlendOp = (byte)PInvoke.AC_SRC_OVER,
                BlendFlags = 0,
                SourceConstantAlpha = opacity,
                AlphaFormat = (byte)PInvoke.AC_SRC_ALPHA
            };

            PInvoke.UpdateLayeredWindow(
                NativeWindow.Handle,
                screenDc,
                &topPos,
                &size,
                (HDC)memDc.Value,
                &pointSource,
                new(0),
                &blend,
                UPDATE_LAYERED_WINDOW_FLAGS.ULW_ALPHA
            );
        }
        finally
        {
            PInvoke.ReleaseDC(default, screenDc);
            if (hBitmap != IntPtr.Zero)
            {
                PInvoke.SelectObject((HDC)memDc.Value, (HGDIOBJ)oldBitmap.Value);
                //Windows.DeleteObject(hBitmap); // The documentation says that we have to use the Windows.DeleteObject... but since there is no such method I use the normal DeleteObject from Win32 GDI and it's working fine without any resource leak.
                PInvoke.DeleteObject((HGDIOBJ)hBitmap.Value);
            }
            PInvoke.DeleteDC(memDc);
        }
    }


    protected override CreateParams CreateParams {
        get {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_LAYERED;
            return cp;
        }
    }
}