using System.ComponentModel;
using Windows.Win32;
using Windows.Win32.Graphics.Gdi;
using WinWrapper.Windowing;
namespace UnitedSets.WinForms;

public class FormWndProc : Form
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public nint HBitmap { get; set; }
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
