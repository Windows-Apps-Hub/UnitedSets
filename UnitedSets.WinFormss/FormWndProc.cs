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
using WinWrapper.Windowing;
namespace UnitedSets.WinForms
{
    public class FormWndProc : Form
    {
        public nint HBitmap { get; set; }
        public Window WindowLink { get; set; }
        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg is (int)WindowMessages.DwmSendIconICLivePreviewBitmap)
            {
                SetIconicLivePreviewBitmap();
                m.Result = IntPtr.Zero;
            } else if (m.Msg is (int)WindowMessages.DwmSendIconICThumbnail)
            {
                SetIconicThumbnail();
                m.Result = IntPtr.Zero;
            }
            else
                base.WndProc(ref m);
        }
        public unsafe void SetIconicLivePreviewBitmap()
        {
            PInvoke.DwmSetIconicLivePreviewBitmap(new(Handle), new(HBitmap), default(Point*), 0).ThrowOnFailure();
        }
        public unsafe void SetIconicThumbnail()
        {
            PInvoke.DwmSetIconicThumbnail(new(Handle), new HBITMAP(HBitmap), 0).ThrowOnFailure();
        }
    }
}
