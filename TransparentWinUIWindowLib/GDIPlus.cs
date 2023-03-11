using System;
using System.Runtime.InteropServices;
using GlobalStructures;

namespace GDIPlus
{
    internal class GDIPlusTools
    {
        //public static Guid CLSID_WICImagingFactory = new Guid("{cacaf262-9370-4615-a13b-9f5539da4c0a}");
        //public static Guid GUID_WICPixelFormat32bppBGR = new Guid("6fddc324-4e03-4bfe-b185-3d77768dc90e");
        //public static Guid GUID_WICPixelFormat32bppPBGRA = new Guid("6fddc324-4e03-4bfe-b185-3d77768dc910");

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdiplusStartup(out IntPtr token, ref StartupInput input, out StartupOutput output);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern void GdiplusShutdown(IntPtr token);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipCreateBitmapFromFile(string filename, out IntPtr bitmap);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipCreateBitmapFromStream(System.Runtime.InteropServices.ComTypes.IStream Stream, out IntPtr bitmap);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipCreateHBITMAPFromBitmap(IntPtr nativeBitmap, out IntPtr hbitmap, int argbBackground);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipDisposeImage(IntPtr image);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipCreateFromHDC(IntPtr hdc, out IntPtr graphics);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipDeleteGraphics(IntPtr graphics);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipDrawImageRect(IntPtr graphics, IntPtr image, float x, float y, float width, float height);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipDrawImagePointRect(IntPtr graphics, IntPtr image, float x, float y,
            float srcx, float srcy, float srcwidth, float srcheight, GpUnit srcUnit);
  
        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipCreateBitmapFromScan0(int width, int height, int stride, PixelFormat format, IntPtr scan0, out IntPtr bitmap);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipSaveImageToFile(IntPtr image, string filename, ref Guid clsidEncoder, IntPtr /*EncoderParameters**/ encoderParams);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipGetImageDimension(IntPtr image, ref float width, ref float height);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipGetImagePixelFormat(IntPtr image, out PixelFormat format);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipBitmapLockBits(IntPtr bitmap, ref GpRect rect, uint flags, PixelFormat format, [In, Out] BitmapData lockedBitmapData);

        [DllImport("GdiPlus.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipBitmapUnlockBits(IntPtr bitmap, BitmapData lockedBitmapData);

        public static int RGB(byte r, byte g, byte b)
        {
            return (r) | ((g) << 8) | ((b) << 16);
        }
    }
 
    public enum GpStatus : int
    {
        Ok = 0,
        GenericError = 1,
        InvalidParameter = 2,
        OutOfMemory = 3,
        ObjectBusy = 4,
        InsufficientBuffer = 5,
        NotImplemented = 6,
        Win32Error = 7,
        WrongState = 8,
        Aborted = 9,
        FileNotFound = 10,
        ValueOverflow = 11,
        AccessDenied = 12,
        UnknownImageFormat = 13,
        FontFamilyNotFound = 14,
        FontStyleNotFound = 15,
        NotTrueTypeFont = 16,
        UnsupportedGdiplusVersion = 17,
        GdiplusNotInitialized = 18,
        PropertyNotFound = 19,
        PropertyNotSupported = 20,
        ProfileNotFound = 21,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct StartupInput
    {
        public int GdiplusVersion;             // Must be 1
                                               // public DebugEventProc DebugEventCallback; // Ignored on free builds
        public IntPtr DebugEventCallback;
        public bool SuppressBackgroundThread;     // FALSE unless you're prepared to call 
                                                  // the hook/unhook functions properly
        public bool SuppressExternalCodecs;       // FALSE unless you want GDI+ only to use
                                                  // its internal image codecs.
        public static StartupInput GetDefault()
        {
            StartupInput result = new StartupInput();
            result.GdiplusVersion = 1;
            // result.DebugEventCallback = null;
            result.SuppressBackgroundThread = false;
            result.SuppressExternalCodecs = false;
            return result;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct StartupOutput
    {
        public IntPtr hook;//not used
        public IntPtr unhook;//not used.
    }

    public enum GpUnit
    {
        UnitWorld,      // 0 -- World coordinate (non-physical unit)
        UnitDisplay,    // 1 -- Variable -- for PageTransform only
        UnitPixel,      // 2 -- Each unit is one device pixel.
        UnitPoint,      // 3 -- Each unit is a printer's point, or 1/72 inch.
        UnitInch,       // 4 -- Each unit is 1 inch.
        UnitDocument,   // 5 -- Each unit is 1/300 inch.
        UnitMillimeter  // 6 -- Each unit is 1 millimeter.
    };

    public enum PixelFormat : int
    {
        PixelFormatIndexed = 0x00010000, // Indexes into a palette
        PixelFormatGDI = 0x00020000, // Is a GDI-supported format
        PixelFormatAlpha = 0x00040000, // Has an alpha component
        PixelFormatPAlpha = 0x00080000, // Pre-multiplied alpha
        PixelFormatExtended = 0x00100000, // Extended color 16 bits/channel
        PixelFormatCanonical = 0x00200000,
        PixelFormatUndefined = 0,
        PixelFormatDontCare = 0,
        PixelFormat1bppIndexed = (1 | (1 << 8) | PixelFormatIndexed | PixelFormatGDI),
        PixelFormat4bppIndexed = (2 | (4 << 8) | PixelFormatIndexed | PixelFormatGDI),
        PixelFormat8bppIndexed = (3 | (8 << 8) | PixelFormatIndexed | PixelFormatGDI),
        PixelFormat16bppGrayScale = (4 | (16 << 8) | PixelFormatExtended),
        PixelFormat16bppRGB555 = (5 | (16 << 8) | PixelFormatGDI),
        PixelFormat16bppRGB565 = (6 | (16 << 8) | PixelFormatGDI),
        PixelFormat16bppARGB1555 = (7 | (16 << 8) | PixelFormatAlpha | PixelFormatGDI),
        PixelFormat24bppRGB = (8 | (24 << 8) | PixelFormatGDI),
        PixelFormat32bppRGB = (9 | (32 << 8) | PixelFormatGDI),
        PixelFormat32bppARGB = (10 | (32 << 8) | PixelFormatAlpha | PixelFormatGDI | PixelFormatCanonical),
        PixelFormat32bppPARGB = (11 | (32 << 8) | PixelFormatAlpha | PixelFormatPAlpha | PixelFormatGDI),
        PixelFormat48bppRGB = (12 | (48 << 8) | PixelFormatExtended),
        PixelFormat64bppARGB = (13 | (64 << 8) | PixelFormatAlpha | PixelFormatCanonical | PixelFormatExtended),
        PixelFormat64bppPARGB = (14 | (64 << 8) | PixelFormatAlpha | PixelFormatPAlpha | PixelFormatExtended),
        PixelFormat32bppCMYK = (15 | (32 << 8)),
        PixelFormatMax = 16
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public sealed class BitmapData
    {
        public uint Width;
        public uint Height;
        public int Stride;
        public int PixelFormat;
        public IntPtr Scan0;
        public int Reserved;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct GpRect
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public GpRect()
        {
            X = Y = Width = Height = 0;
        }

        public GpRect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    };
}
