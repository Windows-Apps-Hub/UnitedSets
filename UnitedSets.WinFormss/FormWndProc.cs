using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.Storage.Xps;
using WinWrapper;
namespace UnitedSets.WinForms
{
    public class FormWndProc : Form
    {
        public HBITMAP Bitmap { get; set; }
        public Window WindowLink { get; set; }
        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg == PInvoke.WM_DWMSENDICONICLIVEPREVIEWBITMAP)
            {
                SetIconicLivePreviewBitmap();
                m.Result = IntPtr.Zero;
            } else if (m.Msg == PInvoke.WM_DWMSENDICONICTHUMBNAIL)
            {
                SetIconicThumbnail();
                m.Result = IntPtr.Zero;
            }
            else
                base.WndProc(ref m);
        }
        public unsafe HRESULT SetIconicLivePreviewBitmap()
        {
            return PInvoke.DwmSetIconicLivePreviewBitmap(new(Handle), Bitmap, (Point*)0, 0);
        }
        public unsafe HRESULT SetIconicThumbnail()
        {
            return PInvoke.DwmSetIconicThumbnail(new(Handle), Bitmap, 0);
        }
    }
}
