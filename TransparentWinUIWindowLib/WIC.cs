using System;
using System.Runtime.InteropServices;
using GlobalStructures;

namespace WIC
{
    internal class WICTools
    {
        public static Guid CLSID_WICImagingFactory = new Guid("{cacaf262-9370-4615-a13b-9f5539da4c0a}");
        public static Guid GUID_WICPixelFormat32bppBGR = new Guid("6fddc324-4e03-4bfe-b185-3d77768dc90e");
        public static Guid GUID_WICPixelFormat32bppPBGRA = new Guid("6fddc324-4e03-4bfe-b185-3d77768dc910");
    } 

    [StructLayout(LayoutKind.Sequential)]
    public struct WICRect
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        private int v1;
        private int v2;
        private int v3;
        private int v4;

        public WICRect(int v1, int v2, int v3, int v4) : this()
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            this.v4 = v4;
        }
    }   

    [ComImport]
    [Guid("ec5ec8a9-c395-4314-9c77-54d7a935ff70")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICImagingFactory
    {
        HRESULT CreateDecoderFromFilename(string wzFilename, ref Guid pguidVendor, int dwDesiredAccess, WICDecodeOptions metadataOptions, out IWICBitmapDecoder ppIDecoder);
        HRESULT CreateDecoderFromStream(System.Runtime.InteropServices.ComTypes.IStream pIStream, ref Guid pguidVendor, WICDecodeOptions metadataOptions, out IWICBitmapDecoder ppIDecoder);
        HRESULT CreateDecoderFromFileHandle(IntPtr hFile, ref Guid pguidVendor, WICDecodeOptions metadataOptions, out IWICBitmapDecoder ppIDecoder);
        HRESULT CreateComponentInfo(ref Guid clsidComponent, out IWICComponentInfo ppIInfo);
        HRESULT CreateDecoder(ref Guid guidContainerFormat, ref Guid pguidVendor, out IWICBitmapDecoder ppIDecoder);
        HRESULT CreateEncoder(ref Guid guidContainerFormat, ref Guid pguidVendor, out IWICBitmapEncoder ppIEncoder);
        HRESULT CreatePalette(out IWICPalette ppIPalette);
        HRESULT CreateFormatConverter(out IWICFormatConverter ppIFormatConverter);
        HRESULT CreateBitmapScaler(out IWICBitmapScaler ppIBitmapScaler);
        HRESULT CreateBitmapClipper(out IWICBitmapClipper ppIBitmapClipper);
        HRESULT CreateBitmapFlipRotator(out IWICBitmapFlipRotator ppIBitmapFlipRotator);
        HRESULT CreateStream(out IWICStream ppIWICStream);
        HRESULT CreateColorContext(out IWICColorContext ppIWICColorContext);
        HRESULT CreateColorTransformer(out IWICColorTransform ppIWICColorTransform);
        HRESULT CreateBitmap(uint uiWidth, uint uiHeight, ref Guid pixelFormat, WICBitmapCreateCacheOption option, out IWICBitmap ppIBitmap);
        HRESULT CreateBitmapFromSource(IWICBitmapSource pIBitmapSource, WICBitmapCreateCacheOption option, out IWICBitmap ppIBitmap);
        HRESULT CreateBitmapFromSourceRect(IWICBitmapSource pIBitmapSource, uint x, uint y, uint width, uint height, out IWICBitmap ppIBitmap);
        HRESULT CreateBitmapFromMemory(uint uiWidth, uint uiHeight, ref Guid pixelFormat, uint cbStride, uint cbBufferSize, IntPtr pbBuffer, out IWICBitmap ppIBitmap);
        HRESULT CreateBitmapFromHBITMAP(IntPtr hBitmap, IntPtr hPalette, WICBitmapAlphaChannelOption options, out IWICBitmap ppIBitmap);
        HRESULT CreateBitmapFromHICON(IntPtr hIcon, out IWICBitmap ppIBitmap);
        //HRESULT CreateComponentEnumerator(int componentTypes, int options, out IEnumUnknown ppIEnumUnknown);
        HRESULT CreateComponentEnumerator(int componentTypes, int options, out IntPtr ppIEnumUnknown);
        HRESULT CreateFastMetadataEncoderFromDecoder(IWICBitmapDecoder pIDecoder, out IWICFastMetadataEncoder ppIFastEncoder);
        HRESULT CreateFastMetadataEncoderFromFrameDecode(IWICBitmapFrameDecode pIFrameDecoder, out IWICFastMetadataEncoder ppIFastEncoder);
        HRESULT CreateQueryWriter(ref Guid guidMetadataFormat, ref Guid pguidVendor, out IWICMetadataQueryWriter ppIQueryWriter);
        HRESULT CreateQueryWriterFromReader(IWICMetadataQueryReader pIQueryReader, ref Guid pguidVendor, out IWICMetadataQueryWriter ppIQueryWriter);
    }

    public enum WICDecodeOptions
    {
        WICDecodeMetadataCacheOnDemand = 0,
        WICDecodeMetadataCacheOnLoad = 1,
        WICMETADATACACHEOPTION_FORCE_DWORD = 0x7FFFFFFF
    }

    [ComImport]
    [Guid("23BC3F0A-698B-4357-886B-F24D50671334")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICComponentInfo
    {
        HRESULT GetComponentType(out WICComponentType pType);
        HRESULT GetCLSID(out Guid pclsid);
        HRESULT GetSigningStatus(out int pStatus);
        HRESULT GetAuthor(uint cchAuthor, [Out, In] string wzAuthor, out uint pcchActual);
        HRESULT GetVendorGUID(out Guid pguidVendor);
        HRESULT GetVersion(uint cchVersion, [Out, In] string wzVersion, out uint pcchActual);
        HRESULT GetSpecVersion(uint cchSpecVersion, [Out, In] string wzSpecVersion, out uint pcchActual);
        HRESULT GetFriendlyName(uint cchFriendlyName, [Out, In] string wzFriendlyName, out uint pcchActual);
    }

    [ComImport]
    [Guid("9EDDE9E7-8DEE-47ea-99DF-E6FAF2ED44BF")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICBitmapDecoder
    {
        HRESULT QueryCapability(System.Runtime.InteropServices.ComTypes.IStream pIStream, out int pdwCapability);
        HRESULT Initialize(System.Runtime.InteropServices.ComTypes.IStream pIStream, WICDecodeOptions cacheOptions);
        HRESULT GetContainerFormat(out Guid pguidContainerFormat);
        HRESULT GetDecoderInfo(out IWICBitmapDecoderInfo ppIDecoderInfo);
        HRESULT CopyPalette(IWICPalette pIPalette);
        HRESULT GetMetadataQueryReader(out IWICMetadataQueryReader ppIMetadataQueryReader);
        HRESULT GetPreview(out IWICBitmapSource ppIBitmapSource);
        HRESULT GetColorContexts(uint cCount, [Out, In] IWICColorContext ppIColorContexts, out uint pcActualCount);
        HRESULT GetThumbnail(out IWICBitmapSource ppIThumbnail);
        HRESULT GetFrameCount(out uint pCount);
        HRESULT GetFrame(uint index, out IWICBitmapFrameDecode ppIBitmapFrame);
    }

    public enum WICComponentType
    {
        WICDecoder = 0x1,
        WICEncoder = 0x2,
        WICPixelFormatConverter = 0x4,
        WICMetadataReader = 0x8,
        WICMetadataWriter = 0x10,
        WICPixelFormat = 0x20,
        WICAllComponents = 0x3F,
        WICCOMPONENTTYPE_FORCE_DWORD = 0x7FFFFFFF
    }

    [ComImport]
    [Guid("D8CD007F-D08F-4191-9BFC-236EA7F0E4B5")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICBitmapDecoderInfo : IWICBitmapCodecInfo
    {
        #region IWICBitmapCodecInfo
        #region IWICComponentInfo
        new HRESULT GetComponentType(out WICComponentType pType);
        new HRESULT GetCLSID(out Guid pclsid);
        new HRESULT GetSigningStatus(out int pStatus);
        new HRESULT GetAuthor(uint cchAuthor, [Out, In] string wzAuthor, out uint pcchActual);
        new HRESULT GetVendorGUID(out Guid pguidVendor);
        new HRESULT GetVersion(uint cchVersion, [Out, In] string wzVersion, out uint pcchActual);
        new HRESULT GetSpecVersion(uint cchSpecVersion, [Out, In] string wzSpecVersion, out uint pcchActual);
        new HRESULT GetFriendlyName(uint cchFriendlyName, [Out, In] string wzFriendlyName, out uint pcchActual);
        #endregion

        new HRESULT GetContainerFormat(out Guid pguidContainerFormat);
        new HRESULT GetPixelFormats(uint cFormats, ref Guid pguidPixelFormats, out uint pcActual);
        new HRESULT GetColorManagementVersion(uint cchColorManagementVersion, string wzColorManagementVersion, out uint pcchActual);
        new HRESULT GetDeviceManufacturer(uint cchDeviceManufacturer, string wzDeviceManufacturer, out uint pcchActual);
        new HRESULT GetDeviceModels(uint cchDeviceModels, string wzDeviceModels, out uint pcchActual);
        new HRESULT GetMimeTypes(uint cchMimeTypes, string wzMimeTypes, out uint pcchActual);
        new HRESULT GetFileExtensions(uint cchFileExtensions, string wzFileExtensions, out uint pcchActual);
        new HRESULT DoesSupportAnimation(out bool pfSupportAnimation);
        new HRESULT DoesSupportChromakey(out bool pfSupportChromakey);
        new HRESULT DoesSupportLossless(out bool pfSupportLossless);
        new HRESULT DoesSupportMultiframe(out bool pfSupportMultiframe);
        new HRESULT MatchesMimeType(string wzMimeType, out bool pfMatches);
        #endregion

        HRESULT GetPatterns(uint cbSizePatterns, out WICBitmapPattern pPatterns, out uint pcPatterns, out uint pcbPatternsActual);
        HRESULT MatchesPattern(System.Runtime.InteropServices.ComTypes.IStream pIStream, out bool pfMatches);
        HRESULT CreateInstance(out IWICBitmapDecoder ppIBitmapDecoder);
    }

    [ComImport]
    [Guid("E87A44C4-B76E-4c47-8B09-298EB12A2714")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICBitmapCodecInfo : IWICComponentInfo
    {
        #region IWICComponentInfo
        new HRESULT GetComponentType(out WICComponentType pType);
        new HRESULT GetCLSID(out Guid pclsid);
        new HRESULT GetSigningStatus(out int pStatus);
        new HRESULT GetAuthor(uint cchAuthor, [Out, In] string wzAuthor, out uint pcchActual);
        new HRESULT GetVendorGUID(out Guid pguidVendor);
        new HRESULT GetVersion(uint cchVersion, [Out, In] string wzVersion, out uint pcchActual);
        new HRESULT GetSpecVersion(uint cchSpecVersion, [Out, In] string wzSpecVersion, out uint pcchActual);
        new HRESULT GetFriendlyName(uint cchFriendlyName, [Out, In] string wzFriendlyName, out uint pcchActual);
        #endregion

        HRESULT GetContainerFormat(out Guid pguidContainerFormat);
        HRESULT GetPixelFormats(uint cFormats, ref Guid pguidPixelFormats, out uint pcActual);
        HRESULT GetColorManagementVersion(uint cchColorManagementVersion, string wzColorManagementVersion, out uint pcchActual);
        HRESULT GetDeviceManufacturer(uint cchDeviceManufacturer, string wzDeviceManufacturer, out uint pcchActual);
        HRESULT GetDeviceModels(uint cchDeviceModels, string wzDeviceModels, out uint pcchActual);
        HRESULT GetMimeTypes(uint cchMimeTypes, string wzMimeTypes, out uint pcchActual);
        HRESULT GetFileExtensions(uint cchFileExtensions, string wzFileExtensions, out uint pcchActual);        
        HRESULT DoesSupportAnimation(out bool pfSupportAnimation);
        HRESULT DoesSupportChromakey(out bool pfSupportChromakey);        
        HRESULT DoesSupportLossless(out bool pfSupportLossless);        
        HRESULT DoesSupportMultiframe(out bool pfSupportMultiframe);        
        HRESULT MatchesMimeType(string wzMimeType, out bool pfMatches);
    }

    [ComImport]
    [Guid("00000120-a8f2-4877-ba0a-fd2b6645fb94")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICBitmapSource
    {
        HRESULT GetSize(out uint puiWidth, out uint puiHeight);
        HRESULT GetPixelFormat(out Guid pPixelFormat);
        HRESULT GetResolution(out double pDpiX, out double pDpiY);
        HRESULT CopyPalette(IWICPalette pIPalette);
        //HRESULT CopyPixels(ref WICRect prc, uint cbStride, uint cbBufferSize, [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] byte[] pbBuffer);
        HRESULT CopyPixels(ref WICRect prc, uint cbStride, uint cbBufferSize, IntPtr pbBuffer);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WICBitmapPattern
    {
        public LARGE_INTEGER Position;
        public uint Length;
        // public byte* Pattern;
        // public byte* Mask;
        public IntPtr Pattern;
        public IntPtr Mask;
        private bool EndOfStream;
    }

    public enum WICBitmapEncoderCacheOption
    {
        WICBitmapEncoderCacheInMemory = 0x0,
        WICBitmapEncoderCacheTempFile = 0x1,
        WICBitmapEncoderNoCache = 0x2,
        WICBITMAPENCODERCACHEOPTION_FORCE_DWORD = 0x7FFFFFFF
    }

    [ComImport]
    [Guid("94C9B4EE-A09F-4f92-8A1E-4A9BCE7E76FB")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //IWICBitmapEncoderInfo : public IWICBitmapCodecInfo
    public interface IWICBitmapEncoderInfo : IWICBitmapCodecInfo
    {
        #region <IWICBitmapCodecInfo>
        #region IWICComponentInfo
        new HRESULT GetComponentType(out WICComponentType pType);
        new HRESULT GetCLSID(out Guid pclsid);
        new HRESULT GetSigningStatus(out int pStatus);
        new HRESULT GetAuthor(uint cchAuthor, [Out, In] string wzAuthor, out uint pcchActual);
        new HRESULT GetVendorGUID(out Guid pguidVendor);
        new HRESULT GetVersion(uint cchVersion, [Out, In] string wzVersion, out uint pcchActual);
        new HRESULT GetSpecVersion(uint cchSpecVersion, [Out, In] string wzSpecVersion, out uint pcchActual);
        new HRESULT GetFriendlyName(uint cchFriendlyName, [Out, In] string wzFriendlyName, out uint pcchActual);
        #endregion

        new HRESULT GetContainerFormat(out Guid pguidContainerFormat);
        new HRESULT GetPixelFormats(uint cFormats, ref Guid pguidPixelFormats, out uint pcActual);
        new HRESULT GetColorManagementVersion(uint cchColorManagementVersion, string wzColorManagementVersion, out uint pcchActual);
        new HRESULT GetDeviceManufacturer(uint cchDeviceManufacturer, string wzDeviceManufacturer, out uint pcchActual);
        new HRESULT GetDeviceModels(uint cchDeviceModels, string wzDeviceModels, out uint pcchActual);
        new HRESULT GetMimeTypes(uint cchMimeTypes, string wzMimeTypes, out uint pcchActual);
        new HRESULT GetFileExtensions(uint cchFileExtensions, string wzFileExtensions, out uint pcchActual);
        new HRESULT DoesSupportAnimation(out bool pfSupportAnimation);
        new HRESULT DoesSupportChromakey(out bool pfSupportChromakey);
        new HRESULT DoesSupportLossless(out bool pfSupportLossless);
        new HRESULT DoesSupportMultiframe(out bool pfSupportMultiframe);
        new HRESULT MatchesMimeType(string wzMimeType, out bool pfMatches);

        #endregion
        HRESULT CreateInstance(out IWICBitmapEncoder ppIBitmapEncoder);
    }
    [ComImport]
    [Guid("00000040-a8f2-4877-ba0a-fd2b6645fb94")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICPalette
    {
        HRESULT InitializePredefined(WICBitmapPaletteType ePaletteType, bool fAddTransparentColor);
        HRESULT InitializeCustom(uint pColors, uint cCount);
        HRESULT InitializeFromBitmap(IWICBitmapSource pISurface, uint cCount, bool fAddTransparentColor);
        HRESULT InitializeFromPalette(IWICPalette pIPalette);
        HRESULT GetType(out WICBitmapPaletteType pePaletteType);
        HRESULT GetColorCount(out uint pcCount);
        HRESULT GetColors(uint cCount, out uint pColors, out uint pcActualColors);
        HRESULT IsBlackWhite(out bool pfIsBlackWhite);
        HRESULT IsGrayscale(out bool pfIsGrayscale);
        HRESULT HasAlpha(out bool pfHasAlpha);
    }

    [ComImport]
    [Guid("00000103-a8f2-4877-ba0a-fd2b6645fb94")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICBitmapEncoder
    {
        HRESULT Initialize(System.Runtime.InteropServices.ComTypes.IStream pIStream, WICBitmapEncoderCacheOption cacheOption);
        HRESULT GetContainerFormat(out Guid pguidContainerFormat);
        HRESULT GetEncoderInfo(out IWICBitmapEncoderInfo ppIEncoderInfo);
        HRESULT SetColorContexts(uint cCount, IWICColorContext ppIColorContext);
        HRESULT SetPalette(IWICPalette pIPalette);
        HRESULT SetThumbnail(IWICBitmapSource pIThumbnail);
        HRESULT SetPreview(IWICBitmapSource pIPreview);
        HRESULT CreateNewFrame(out IWICBitmapFrameEncode ppIFrameEncode, [Out, In] IPropertyBag2 ppIEncoderOptions);
        HRESULT Commit();
        HRESULT GetMetadataQueryWriter(out IWICMetadataQueryWriter ppIMetadataQueryWriter);
    }

    public enum WICBitmapDitherType
    {
        WICBitmapDitherTypeNone = 0,
        WICBitmapDitherTypeSolid = 0,
        WICBitmapDitherTypeOrdered4x4 = 0x1,
        WICBitmapDitherTypeOrdered8x8 = 0x2,
        WICBitmapDitherTypeOrdered16x16 = 0x3,
        WICBitmapDitherTypeSpiral4x4 = 0x4,
        WICBitmapDitherTypeSpiral8x8 = 0x5,
        WICBitmapDitherTypeDualSpiral4x4 = 0x6,
        WICBitmapDitherTypeDualSpiral8x8 = 0x7,
        WICBitmapDitherTypeErrorDiffusion = 0x8,
        WICBITMAPDITHERTYPE_FORCE_DWORD = 0x7FFFFFFF
    }

    [ComImport]
    [Guid("00000301-a8f2-4877-ba0a-fd2b6645fb94")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICFormatConverter : IWICBitmapSource
    {
        #region <IWICBitmapSource>
        new HRESULT GetSize(out uint puiWidth, out uint puiHeight);
        new HRESULT GetPixelFormat(out Guid pPixelFormat);
        new HRESULT GetResolution(out double pDpiX, out double pDpiY);
        new HRESULT CopyPalette(IWICPalette pIPalette);
        //new HRESULT CopyPixels(ref WICRect prc, uint cbStride, uint cbBufferSize, [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] byte[] pbBuffer);
        new HRESULT CopyPixels(ref WICRect prc, uint cbStride, uint cbBufferSize, IntPtr pbBuffer);
        #endregion
        HRESULT Initialize(IWICBitmapSource pISource, ref Guid dstFormat, WICBitmapDitherType dither, IWICPalette pIPalette, double alphaThresholdPercent, WICBitmapPaletteType paletteTranslate);
        HRESULT CanConvert(ref Guid srcPixelFormat, ref Guid dstPixelFormat, out bool pfCanConvert);
    }

    [ComImport]
    [Guid("135FF860-22B7-4ddf-B0F6-218F4F299A43")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICStream : System.Runtime.InteropServices.ComTypes.IStream
    {
        #region IStream      
        new void Clone(out System.Runtime.InteropServices.ComTypes.IStream ppstm);
        new void Commit(int grfCommitFlags);
        new void CopyTo(System.Runtime.InteropServices.ComTypes.IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten);
        new void LockRegion(long libOffset, long cb, int dwLockType);
        new void Read(byte[] pv, int cb, IntPtr pcbRead);
        new void Revert();
        new void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition);
        new void SetSize(long libNewSize);
        new void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag);
        new void UnlockRegion(long libOffset, long cb, int dwLockType);
        new void Write(byte[] pv, int cb, IntPtr pcbWritten);
        #endregion

        HRESULT InitializeFromIStream(System.Runtime.InteropServices.ComTypes.IStream pIStream);
        HRESULT InitializeFromFilename(string wzFileName, int dwDesiredAccess);
        HRESULT InitializeFromMemory([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] byte[] pbBuffer, int cbBufferSize);
        HRESULT InitializeFromIStreamRegion(System.Runtime.InteropServices.ComTypes.IStream pIStream, LARGE_INTEGER ulOffset, LARGE_INTEGER ulMaxSize);
    }

    public enum WICBitmapInterpolationMode
    {
        WICBitmapInterpolationModeNearestNeighbor = 0,
        WICBitmapInterpolationModeLinear = 0x1,
        WICBitmapInterpolationModeCubic = 0x2,
        WICBitmapInterpolationModeFant = 0x3,
        WICBITMAPINTERPOLATIONMODE_FORCE_DWORD = 0x7FFFFFFF
    }

    [ComImport]
    [Guid("00000302-a8f2-4877-ba0a-fd2b6645fb94")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICBitmapScaler : IWICBitmapSource
    {
        #region <IWICBitmapSource>
        new HRESULT GetSize(out uint puiWidth, out uint puiHeight);
        new HRESULT GetPixelFormat(out Guid pPixelFormat);
        new HRESULT GetResolution(out double pDpiX, out double pDpiY);
        new HRESULT CopyPalette(IWICPalette pIPalette);
        new HRESULT CopyPixels(ref WICRect prc, uint cbStride, uint cbBufferSize, IntPtr pbBuffer);
        #endregion
        HRESULT Initialize(IWICBitmapSource pISource, uint uiWidth, uint uiHeight, WICBitmapInterpolationMode mode);
    }

    [ComImport()]
    [Guid("E4FBCF03-223D-4e81-9333-D635556DD1B5")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICBitmapClipper : IWICBitmapSource
    {
        #region IWICBitmapSource
        new HRESULT GetSize(out uint puiWidth, out uint puiHeight);
        new HRESULT GetPixelFormat(out Guid pPixelFormat);
        new HRESULT GetResolution(out double pDpiX, out double pDpiY);
        new HRESULT CopyPalette(IWICPalette pIPalette);
        //HRESULT CopyPixels(ref WICRect prc, uint cbStride, uint cbBufferSize, [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] byte[] pbBuffer);
        new HRESULT CopyPixels(ref WICRect prc, uint cbStride, uint cbBufferSize, IntPtr pbBuffer);
        #endregion

        HRESULT Initialize(IWICBitmapSource pISource, WICRect prc);
    }

    public enum WICBitmapTransformOptions
    {
        WICBitmapTransformRotate0 = 0,
        WICBitmapTransformRotate90 = 0x1,
        WICBitmapTransformRotate180 = 0x2,
        WICBitmapTransformRotate270 = 0x3,
        WICBitmapTransformFlipHorizontal = 0x8,
        WICBitmapTransformFlipVertical = 0x10,
        WICBITMAPTRANSFORMOPTIONS_FORCE_DWORD = 0x7FFFFFFF
    }

    [ComImport]
    [Guid("5009834F-2D6A-41ce-9E1B-17C5AFF7A782")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICBitmapFlipRotator : IWICBitmapSource
    {
        #region IWICBitmapSource
        new HRESULT GetSize(out uint puiWidth, out uint puiHeight);
        new HRESULT GetPixelFormat(out Guid pPixelFormat);
        new HRESULT GetResolution(out double pDpiX, out double pDpiY);
        new HRESULT CopyPalette(IWICPalette pIPalette);
        //HRESULT CopyPixels(ref WICRect prc, uint cbStride, uint cbBufferSize, [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] byte[] pbBuffer);
        new HRESULT CopyPixels(ref WICRect prc, uint cbStride, uint cbBufferSize, IntPtr pbBuffer);
        #endregion

        HRESULT Initialize(IWICBitmapSource pISource, WICBitmapTransformOptions options);
    }

    [ComImport]
    [Guid("B66F034F-D0E2-40ab-B436-6DE39E321A94")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICColorTransform : IWICBitmapSource
    {
        #region IWICBitmapSource
        new HRESULT GetSize(out uint puiWidth, out uint puiHeight);
        new HRESULT GetPixelFormat(out Guid pPixelFormat);
        new HRESULT GetResolution(out double pDpiX, out double pDpiY);
        new HRESULT CopyPalette(IWICPalette pIPalette);
        //HRESULT CopyPixels(ref WICRect prc, uint cbStride, uint cbBufferSize, [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] byte[] pbBuffer);
        new HRESULT CopyPixels(ref WICRect prc, uint cbStride, uint cbBufferSize, IntPtr pbBuffer);
        #endregion

        HRESULT Initialize(IWICBitmapSource pIBitmapSource, IWICColorContext pIContextSource, IWICColorContext pIContextDest, ref Guid pixelFmtDest);
    }

    public enum WICBitmapCreateCacheOption
    {
        WICBitmapNoCache = 0,
        WICBitmapCacheOnDemand = 0x1,
        WICBitmapCacheOnLoad = 0x2,
        WICBITMAPCREATECACHEOPTION_FORCE_DWORD = 0x7FFFFFFF
    }

    public enum WICBitmapAlphaChannelOption
    {
        WICBitmapUseAlpha = 0,
        WICBitmapUsePremultipliedAlpha = 0x1,
        WICBitmapIgnoreAlpha = 0x2,
        WICBITMAPALPHACHANNELOPTIONS_FORCE_DWORD = 0x7FFFFFFF
    }

    [ComImport]
    [Guid("B84E2C09-78C9-4AC4-8BD3-524AE1663A2F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICFastMetadataEncoder
    {
        HRESULT Commit();
        HRESULT GetMetadataQueryWriter(out IWICMetadataQueryWriter ppIMetadataQueryWriter);
    }

    public enum WICColorContextType
    {
        WICColorContextUninitialized = 0,
        WICColorContextProfile = 1,
        WICColorContextExifColorSpace = 2
    }
 
    [ComImport]
    [Guid("3C613A02-34B2-44ea-9A7C-45AEA9C6FD6D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICColorContext
    {
        HRESULT InitializeFromFilename(string wzFilename);
        HRESULT InitializeFromMemory([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] byte[] pbBuffer, int cbBufferSize);
        HRESULT InitializeFromExifColorSpace(uint value);
        HRESULT GetType(out WICColorContextType pType);
        HRESULT GetProfileBytes(uint cbBuffer, [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] byte[] pbBuffer, out uint pcbActual);
        HRESULT GetExifColorSpace(out uint pValue);
    }

    [ComImport]
    [Guid("00000121-a8f2-4877-ba0a-fd2b6645fb94")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICBitmap : IWICBitmapSource
    {
        #region IWICBitmapSource
        new HRESULT GetSize(out uint puiWidth, out uint puiHeight);
        new HRESULT GetPixelFormat(out Guid pPixelFormat);
        new HRESULT GetResolution(out double pDpiX, out double pDpiY);
        new HRESULT CopyPalette(IWICPalette pIPalette);
        //HRESULT CopyPixels(ref WICRect prc, uint cbStride, uint cbBufferSize, [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] byte[] pbBuffer);
        new HRESULT CopyPixels(ref WICRect prc, uint cbStride, uint cbBufferSize, IntPtr pbBuffer);
        #endregion

        HRESULT Lock(ref WICRect prcLock, WICBitmapLockFlags flags, out IWICBitmapLock ppILock);
        HRESULT SetPalette(IWICPalette pIPalette);
        HRESULT SetResolution(double dpiX, double dpiY);
    }

    [ComImport]
    [Guid("00000123-a8f2-4877-ba0a-fd2b6645fb94")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICBitmapLock
    {
        HRESULT GetSize(out uint puiWidth, out uint puiHeight);
        HRESULT GetStride(out uint pcbStride);
        //HRESULT GetDataPointer(out uint pcbBufferSize, out WICInProcPointer ppbData);
        HRESULT GetDataPointer(out uint pcbBufferSize, out IntPtr ppbData);
        HRESULT GetPixelFormat(out Guid pPixelFormat);
    }

    public enum WICBitmapLockFlags
    {
        WICBitmapLockRead = 1,
        WICBitmapLockWrite = 2,
        WICBITMAPLOCKFLAGS_FORCE_DWORD = 0x7FFFFFFF
    }     

    [ComImport]
    [Guid("3B16811B-6A43-4ec9-A813-3D930C13B940")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICBitmapFrameDecode : IWICBitmapSource
    {
        #region <IWICBitmapSource>
        new HRESULT GetSize(out uint puiWidth, out uint puiHeight);
        new HRESULT GetPixelFormat(out Guid pPixelFormat);
        new HRESULT GetResolution(out double pDpiX, out double pDpiY);
        new HRESULT CopyPalette(IWICPalette pIPalette);
        //HRESULT CopyPixels(ref WICRect prc, uint cbStride, uint cbBufferSize, [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] byte[] pbBuffer);
        new HRESULT CopyPixels(ref WICRect prc, uint cbStride, uint cbBufferSize, IntPtr pbBuffer);
        #endregion

        HRESULT GetMetadataQueryReader(out IWICMetadataQueryReader ppIMetadataQueryReader);
        HRESULT GetColorContexts(uint cCount, [Out, In] IWICColorContext ppIColorContexts, out uint pcActualCount);
        HRESULT GetThumbnail(out IWICBitmapSource ppIThumbnail);
    }

    [ComImport]
    [Guid("A721791A-0DEF-4d06-BD91-2118BF1DB10B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICMetadataQueryWriter : IWICMetadataQueryReader
    {
        #region IWICMetadataQueryReader
        new HRESULT GetContainerFormat(out Guid pguidContainerFormat);
        new HRESULT GetLocation(uint cchMaxLength, [Out, In] string wzNamespace, out uint pcchActualLength);
        new HRESULT GetMetadataByName(string wzName, [Out, In] PROPVARIANT pvarValue);
        // new HRESULT GetEnumerator(out IEnumString ppIEnumString);
        new HRESULT GetEnumerator(out IntPtr ppIEnumString);
        #endregion

        HRESULT SetMetadataByName(string wzName, PROPVARIANT pvarValue);
        HRESULT RemoveMetadataByName(string wzName);
    }

    [ComImport]
    [Guid("30989668-E1C9-4597-B395-458EEDB808DF")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICMetadataQueryReader
    {
        HRESULT GetContainerFormat(out Guid pguidContainerFormat);
        HRESULT GetLocation(uint cchMaxLength, [Out, In] string wzNamespace, out uint pcchActualLength);
        HRESULT GetMetadataByName(string wzName, [Out, In] PROPVARIANT pvarValue);
        //HRESULT GetEnumerator(out IEnumString ppIEnumString);
        HRESULT GetEnumerator(out IntPtr ppIEnumString);
    }
 
    [ComImport]
    [Guid("00000105-a8f2-4877-ba0a-fd2b6645fb94")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICBitmapFrameEncode
    {
        HRESULT Initialize(IPropertyBag2 pIEncoderOptions);
        HRESULT SetSize(uint uiWidth, uint uiHeight);
        HRESULT SetResolution(double dpiX, double dpiY);
        HRESULT SetPixelFormat([Out, In] Guid pPixelFormat);
        HRESULT SetColorContexts(uint cCount, IWICColorContext ppIColorContext);
        HRESULT SetPalette(IWICPalette pIPalette);
        HRESULT SetThumbnail(IWICBitmapSource pIThumbnail);
        //HRESULT WritePixels(uint lineCount, uint cbStride, uint cbBufferSize, BYTE* pbPixels);
        HRESULT WritePixels(uint lineCount, uint cbStride, uint cbBufferSize, IntPtr pbPixels);
        HRESULT WriteSource(IWICBitmapSource pIBitmapSource, ref WICRect prc);
        HRESULT Commit();
        HRESULT GetMetadataQueryWriter(out IWICMetadataQueryWriter ppIMetadataQueryWriter);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PROPBAG2
    {
        public int dwType;
        public ushort vt;
        public ushort cfType;
        public int dwHint;
        // public LPOLESTR pstrName;
        public string pstrName;
        public Guid clsid;
    }

    [ComImport]
    [Guid("22F55882-280B-11d0-A8A9-00A0C90C2004")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPropertyBag2
    {
        //HRESULT Read(uint cProperties, IErrorLog pErrLog, out VARIANT pvarValue,  [Out, In] HRESULT phrError);
        HRESULT Read(uint cProperties, IErrorLog pErrLog, out PROPVARIANT pvarValue, [Out, In] HRESULT phrError);
        //HRESULT Write(uint cProperties, PROPBAG2 pPropBag, VARIANT pvarValue);
        HRESULT Write(uint cProperties, PROPBAG2 pPropBag, PROPVARIANT pvarValue);
        HRESULT CountProperties(out uint pcProperties);
        HRESULT GetPropertyInfo(uint iProperty, uint cProperties, out PROPBAG2 pPropBag, out uint pcProperties);
        //HRESULT LoadObject(LPCOLESTR pstrName, int dwHint, IUnknown pUnkObject,  IErrorLog pErrLog);
        HRESULT LoadObject([In, MarshalAs(UnmanagedType.LPWStr)] string pstrName, int dwHint, IntPtr pUnkObject, IErrorLog pErrLog);
    }

    [ComImport]
    [Guid("3127CA40-446E-11CE-8135-00AA004BB851")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IErrorLog
    {
        //HRESULT AddError(LPCOLESTR pszPropName, System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo);
        HRESULT AddError([In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName, System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo);
    }

    public enum WICBitmapPaletteType
    {
        WICBitmapPaletteTypeCustom = 0,
        WICBitmapPaletteTypeMedianCut = 1,
        WICBitmapPaletteTypeFixedBW = 2,
        WICBitmapPaletteTypeFixedHalftone8 = 3,
        WICBitmapPaletteTypeFixedHalftone27 = 4,
        WICBitmapPaletteTypeFixedHalftone64 = 5,
        WICBitmapPaletteTypeFixedHalftone125 = 6,
        WICBitmapPaletteTypeFixedHalftone216 = 7,
        WICBitmapPaletteTypeFixedWebPalette = WICBitmapPaletteTypeFixedHalftone216,
        WICBitmapPaletteTypeFixedHalftone252 = 8,
        WICBitmapPaletteTypeFixedHalftone256 = 9,
        WICBitmapPaletteTypeFixedGray4 = 10,
        WICBitmapPaletteTypeFixedGray16 = 11,
        WICBitmapPaletteTypeFixedGray256 = 12,
        WICBITMAPPALETTETYPE_FORCE_DWORD = 0x7FFFFFFF
    }
}
