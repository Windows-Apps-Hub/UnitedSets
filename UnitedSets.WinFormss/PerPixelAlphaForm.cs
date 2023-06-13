// This class PerPixelAlphaForm were taken and modified from https://www.codeproject.com/Articles/1822/Per-Pixel-Alpha-Blend-in-C

using WinWrapper.Windowing;
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
        NativeWindow.ExStyle = WindowExStyles.Layered | WindowExStyles.Transparent;
    }

    public unsafe void SetBitmap(Bitmap bitmap, byte opacity = 255) {
        NativeWindow.SetLayeredWindowBitmap(bitmap.GetHbitmap(), opacity);
    }

    public unsafe void SetBitmap(nint hBitmap, byte opacity = 255)
    {
        NativeWindow.SetLayeredWindowBitmap(hBitmap, opacity);
    }


    protected override CreateParams CreateParams {
        get {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= (int)WindowExStyles.Layered;
            return cp;
        }
    }
}