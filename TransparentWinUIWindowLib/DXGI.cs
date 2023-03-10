using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using GlobalStructures;

namespace DXGI
{  
    [ComImport]
    [Guid("aec22fb8-76f3-4639-9be0-28eb43a67a2e")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDXGIObject
    {
        HRESULT SetPrivateData(ref Guid Name, uint DataSize, IntPtr pData);
        HRESULT SetPrivateDataInterface(ref Guid Name, IntPtr pUnknown);
        HRESULT GetPrivateData(ref Guid Name, ref uint pDataSize, out IntPtr pData);
        HRESULT GetParent(ref Guid riid, out IntPtr ppParent);
    }

    [ComImport]
    [Guid("3d3e0379-f9de-4d58-bb6c-18d62992f1a6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDXGIDeviceSubObject : IDXGIObject
    {
        #region <IDXGIObject>
        new HRESULT SetPrivateData(ref Guid Name, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid Name, IntPtr pUnknown);
        new HRESULT GetPrivateData(ref Guid Name, ref uint pDataSize, out IntPtr pData);
        new HRESULT GetParent(ref Guid riid, out IntPtr ppParent);
        #endregion

        HRESULT GetDevice(ref Guid riid, out IntPtr ppDevice);
    }

    [ComImport]
    [Guid("2411e7e1-12ac-4ccf-bd14-9798e8534dc0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDXGIAdapter : IDXGIObject
    {
        #region IDXGIObject
        new HRESULT SetPrivateData(ref Guid Name, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid Name, IntPtr pUnknown);
        new HRESULT GetPrivateData(ref Guid Name, ref uint pDataSize, out IntPtr pData);
        new HRESULT GetParent(ref Guid riid, out IntPtr ppParent);
        #endregion

        [PreserveSig]
        HRESULT EnumOutputs(uint Output, ref IDXGIOutput ppOutput);
        HRESULT GetDesc(out DXGI_ADAPTER_DESC pDesc);
        HRESULT CheckInterfaceSupport(ref Guid InterfaceName, out LARGE_INTEGER pUMDVersion);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DXGI_ADAPTER_DESC
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Description;
        public uint VendorId;
        public uint DeviceId;
        public uint SubSysId;
        public uint Revision;
        public uint DedicatedVideoMemory;
        public uint DedicatedSystemMemory;
        public uint SharedSystemMemory;
        public LUID AdapterLuid;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LUID
    {
        uint LowPart;
        int HighPart;
    }

    [ComImport]
    [Guid("ae02eedb-c735-4690-8d52-5a8dc20213aa")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDXGIOutput : IDXGIObject
    {
        #region IDXGIObject
        new HRESULT SetPrivateData(ref Guid Name, uint DataSize, IntPtr pData);
        //HRESULT SetPrivateDataInterface(ref Guid Name, IUnknown* pUnknown);
        new HRESULT SetPrivateDataInterface(ref Guid Name, IntPtr pUnknown);
        new HRESULT GetPrivateData(ref Guid Name, ref uint pDataSize, out IntPtr pData);
        new HRESULT GetParent(ref Guid riid, out IntPtr ppParent);
        #endregion

        HRESULT GetDesc(out DXGI_OUTPUT_DESC pDesc);
        HRESULT GetDisplayModeList(DXGI_FORMAT EnumFormat, uint Flags, ref uint pNumModes, DXGI_MODE_DESC pDesc);
        //HRESULT FindClosestMatchingMode(DXGI_MODE_DESC pModeToMatch, out  DXGI_MODE_DESC pClosestMatch, IUnknown pConcernedDevice);
        HRESULT FindClosestMatchingMode(DXGI_MODE_DESC pModeToMatch, out DXGI_MODE_DESC pClosestMatch, IntPtr pConcernedDevice);
        HRESULT WaitForVBlank();
        //HRESULT TakeOwnership(IUnknown pDevice, bool Exclusive);
        HRESULT TakeOwnership(IntPtr pDevice, bool Exclusive);
        void ReleaseOwnership();
        HRESULT GetGammaControlCapabilities(out DXGI_GAMMA_CONTROL_CAPABILITIES pGammaCaps);
        HRESULT SetGammaControl(DXGI_GAMMA_CONTROL pArray);
        HRESULT GetGammaControl(out DXGI_GAMMA_CONTROL pArray);
        HRESULT SetDisplaySurface(IDXGISurface pScanoutSurface);
        HRESULT GetDisplaySurfaceData(IDXGISurface pDestination);
        HRESULT GetFrameStatistics(out DXGI_FRAME_STATISTICS pStats);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DXGI_OUTPUT_DESC
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        string DeviceName;
        public RECT DesktopCoordinates;
        public bool AttachedToDesktop;
        public DXGI_MODE_ROTATION Rotation;
        public IntPtr Monitor;
    }

    public enum DXGI_MODE_ROTATION
    {
        DXGI_MODE_ROTATION_UNSPECIFIED = 0,
        DXGI_MODE_ROTATION_IDENTITY = 1,
        DXGI_MODE_ROTATION_ROTATE90 = 2,
        DXGI_MODE_ROTATION_ROTATE180 = 3,
        DXGI_MODE_ROTATION_ROTATE270 = 4
    }

    public enum DXGI_FORMAT
    {
        DXGI_FORMAT_UNKNOWN = 0,
        DXGI_FORMAT_R32G32B32A32_TYPELESS = 1,
        DXGI_FORMAT_R32G32B32A32_FLOAT = 2,
        DXGI_FORMAT_R32G32B32A32_UINT = 3,
        DXGI_FORMAT_R32G32B32A32_SINT = 4,
        DXGI_FORMAT_R32G32B32_TYPELESS = 5,
        DXGI_FORMAT_R32G32B32_FLOAT = 6,
        DXGI_FORMAT_R32G32B32_UINT = 7,
        DXGI_FORMAT_R32G32B32_SINT = 8,
        DXGI_FORMAT_R16G16B16A16_TYPELESS = 9,
        DXGI_FORMAT_R16G16B16A16_FLOAT = 10,
        DXGI_FORMAT_R16G16B16A16_UNORM = 11,
        DXGI_FORMAT_R16G16B16A16_UINT = 12,
        DXGI_FORMAT_R16G16B16A16_SNORM = 13,
        DXGI_FORMAT_R16G16B16A16_SINT = 14,
        DXGI_FORMAT_R32G32_TYPELESS = 15,
        DXGI_FORMAT_R32G32_FLOAT = 16,
        DXGI_FORMAT_R32G32_UINT = 17,
        DXGI_FORMAT_R32G32_SINT = 18,
        DXGI_FORMAT_R32G8X24_TYPELESS = 19,
        DXGI_FORMAT_D32_FLOAT_S8X24_UINT = 20,
        DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS = 21,
        DXGI_FORMAT_X32_TYPELESS_G8X24_UINT = 22,
        DXGI_FORMAT_R10G10B10A2_TYPELESS = 23,
        DXGI_FORMAT_R10G10B10A2_UNORM = 24,
        DXGI_FORMAT_R10G10B10A2_UINT = 25,
        DXGI_FORMAT_R11G11B10_FLOAT = 26,
        DXGI_FORMAT_R8G8B8A8_TYPELESS = 27,
        DXGI_FORMAT_R8G8B8A8_UNORM = 28,
        DXGI_FORMAT_R8G8B8A8_UNORM_SRGB = 29,
        DXGI_FORMAT_R8G8B8A8_UINT = 30,
        DXGI_FORMAT_R8G8B8A8_SNORM = 31,
        DXGI_FORMAT_R8G8B8A8_SINT = 32,
        DXGI_FORMAT_R16G16_TYPELESS = 33,
        DXGI_FORMAT_R16G16_FLOAT = 34,
        DXGI_FORMAT_R16G16_UNORM = 35,
        DXGI_FORMAT_R16G16_UINT = 36,
        DXGI_FORMAT_R16G16_SNORM = 37,
        DXGI_FORMAT_R16G16_SINT = 38,
        DXGI_FORMAT_R32_TYPELESS = 39,
        DXGI_FORMAT_D32_FLOAT = 40,
        DXGI_FORMAT_R32_FLOAT = 41,
        DXGI_FORMAT_R32_UINT = 42,
        DXGI_FORMAT_R32_SINT = 43,
        DXGI_FORMAT_R24G8_TYPELESS = 44,
        DXGI_FORMAT_D24_UNORM_S8_UINT = 45,
        DXGI_FORMAT_R24_UNORM_X8_TYPELESS = 46,
        DXGI_FORMAT_X24_TYPELESS_G8_UINT = 47,
        DXGI_FORMAT_R8G8_TYPELESS = 48,
        DXGI_FORMAT_R8G8_UNORM = 49,
        DXGI_FORMAT_R8G8_UINT = 50,
        DXGI_FORMAT_R8G8_SNORM = 51,
        DXGI_FORMAT_R8G8_SINT = 52,
        DXGI_FORMAT_R16_TYPELESS = 53,
        DXGI_FORMAT_R16_FLOAT = 54,
        DXGI_FORMAT_D16_UNORM = 55,
        DXGI_FORMAT_R16_UNORM = 56,
        DXGI_FORMAT_R16_UINT = 57,
        DXGI_FORMAT_R16_SNORM = 58,
        DXGI_FORMAT_R16_SINT = 59,
        DXGI_FORMAT_R8_TYPELESS = 60,
        DXGI_FORMAT_R8_UNORM = 61,
        DXGI_FORMAT_R8_UINT = 62,
        DXGI_FORMAT_R8_SNORM = 63,
        DXGI_FORMAT_R8_SINT = 64,
        DXGI_FORMAT_A8_UNORM = 65,
        DXGI_FORMAT_R1_UNORM = 66,
        DXGI_FORMAT_R9G9B9E5_SHAREDEXP = 67,
        DXGI_FORMAT_R8G8_B8G8_UNORM = 68,
        DXGI_FORMAT_G8R8_G8B8_UNORM = 69,
        DXGI_FORMAT_BC1_TYPELESS = 70,
        DXGI_FORMAT_BC1_UNORM = 71,
        DXGI_FORMAT_BC1_UNORM_SRGB = 72,
        DXGI_FORMAT_BC2_TYPELESS = 73,
        DXGI_FORMAT_BC2_UNORM = 74,
        DXGI_FORMAT_BC2_UNORM_SRGB = 75,
        DXGI_FORMAT_BC3_TYPELESS = 76,
        DXGI_FORMAT_BC3_UNORM = 77,
        DXGI_FORMAT_BC3_UNORM_SRGB = 78,
        DXGI_FORMAT_BC4_TYPELESS = 79,
        DXGI_FORMAT_BC4_UNORM = 80,
        DXGI_FORMAT_BC4_SNORM = 81,
        DXGI_FORMAT_BC5_TYPELESS = 82,
        DXGI_FORMAT_BC5_UNORM = 83,
        DXGI_FORMAT_BC5_SNORM = 84,
        DXGI_FORMAT_B5G6R5_UNORM = 85,
        DXGI_FORMAT_B5G5R5A1_UNORM = 86,
        DXGI_FORMAT_B8G8R8A8_UNORM = 87,
        DXGI_FORMAT_B8G8R8X8_UNORM = 88,
        DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM = 89,
        DXGI_FORMAT_B8G8R8A8_TYPELESS = 90,
        DXGI_FORMAT_B8G8R8A8_UNORM_SRGB = 91,
        DXGI_FORMAT_B8G8R8X8_TYPELESS = 92,
        DXGI_FORMAT_B8G8R8X8_UNORM_SRGB = 93,
        DXGI_FORMAT_BC6H_TYPELESS = 94,
        DXGI_FORMAT_BC6H_UF16 = 95,
        DXGI_FORMAT_BC6H_SF16 = 96,
        DXGI_FORMAT_BC7_TYPELESS = 97,
        DXGI_FORMAT_BC7_UNORM = 98,
        DXGI_FORMAT_BC7_UNORM_SRGB = 99,
        DXGI_FORMAT_AYUV = 100,
        DXGI_FORMAT_Y410 = 101,
        DXGI_FORMAT_Y416 = 102,
        DXGI_FORMAT_NV12 = 103,
        DXGI_FORMAT_P010 = 104,
        DXGI_FORMAT_P016 = 105,
        DXGI_FORMAT_420_OPAQUE = 106,
        DXGI_FORMAT_YUY2 = 107,
        DXGI_FORMAT_Y210 = 108,
        DXGI_FORMAT_Y216 = 109,
        DXGI_FORMAT_NV11 = 110,
        DXGI_FORMAT_AI44 = 111,
        DXGI_FORMAT_IA44 = 112,
        DXGI_FORMAT_P8 = 113,
        DXGI_FORMAT_A8P8 = 114,
        DXGI_FORMAT_B4G4R4A4_UNORM = 115,
        DXGI_FORMAT_FORCE_UINT = unchecked((int)0xffffffff)
    }

    public enum DXGI_COLOR_SPACE_TYPE
    {
        DXGI_COLOR_SPACE_RGB_FULL_G22_NONE_P709 = 0,
        DXGI_COLOR_SPACE_RGB_FULL_G10_NONE_P709 = 1,
        DXGI_COLOR_SPACE_RGB_STUDIO_G22_NONE_P709 = 2,
        DXGI_COLOR_SPACE_RGB_STUDIO_G22_NONE_P2020 = 3,
        DXGI_COLOR_SPACE_RESERVED = 4,
        DXGI_COLOR_SPACE_YCBCR_FULL_G22_NONE_P709_X601 = 5,
        DXGI_COLOR_SPACE_YCBCR_STUDIO_G22_LEFT_P601 = 6,
        DXGI_COLOR_SPACE_YCBCR_FULL_G22_LEFT_P601 = 7,
        DXGI_COLOR_SPACE_YCBCR_STUDIO_G22_LEFT_P709 = 8,
        DXGI_COLOR_SPACE_YCBCR_FULL_G22_LEFT_P709 = 9,
        DXGI_COLOR_SPACE_YCBCR_STUDIO_G22_LEFT_P2020 = 10,
        DXGI_COLOR_SPACE_YCBCR_FULL_G22_LEFT_P2020 = 11,
        DXGI_COLOR_SPACE_RGB_FULL_G2084_NONE_P2020 = 12,
        DXGI_COLOR_SPACE_YCBCR_STUDIO_G2084_LEFT_P2020 = 13,
        DXGI_COLOR_SPACE_RGB_STUDIO_G2084_NONE_P2020 = 14,
        DXGI_COLOR_SPACE_YCBCR_STUDIO_G22_TOPLEFT_P2020 = 15,
        DXGI_COLOR_SPACE_YCBCR_STUDIO_G2084_TOPLEFT_P2020 = 16,
        DXGI_COLOR_SPACE_RGB_FULL_G22_NONE_P2020 = 17,
        DXGI_COLOR_SPACE_YCBCR_STUDIO_GHLG_TOPLEFT_P2020 = 18,
        DXGI_COLOR_SPACE_YCBCR_FULL_GHLG_TOPLEFT_P2020 = 19,
        DXGI_COLOR_SPACE_RGB_STUDIO_G24_NONE_P709 = 20,
        DXGI_COLOR_SPACE_RGB_STUDIO_G24_NONE_P2020 = 21,
        DXGI_COLOR_SPACE_YCBCR_STUDIO_G24_LEFT_P709 = 22,
        DXGI_COLOR_SPACE_YCBCR_STUDIO_G24_LEFT_P2020 = 23,
        DXGI_COLOR_SPACE_YCBCR_STUDIO_G24_TOPLEFT_P2020 = 24,
        DXGI_COLOR_SPACE_CUSTOM = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DXGI_MODE_DESC
    {
        public uint Width;
        public uint Height;
        public DXGI_RATIONAL RefreshRate;
        public DXGI_FORMAT Format;
        public DXGI_MODE_SCANLINE_ORDER ScanlineOrdering;
        public DXGI_MODE_SCALING Scaling;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DXGI_RATIONAL
    {
        public uint Numerator;
        public uint Denominator;
    }

    public enum DXGI_MODE_SCANLINE_ORDER
    {
        DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED = 0,
        DXGI_MODE_SCANLINE_ORDER_PROGRESSIVE = 1,
        DXGI_MODE_SCANLINE_ORDER_UPPER_FIELD_FIRST = 2,
        DXGI_MODE_SCANLINE_ORDER_LOWER_FIELD_FIRST = 3
    }

    public enum DXGI_MODE_SCALING
    {
        DXGI_MODE_SCALING_UNSPECIFIED = 0,
        DXGI_MODE_SCALING_CENTERED = 1,
        DXGI_MODE_SCALING_STRETCHED = 2
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DXGI_GAMMA_CONTROL_CAPABILITIES
    {
        public bool ScaleAndOffsetSupported;
        public float MaxConvertedValue;
        public float MinConvertedValue;
        public uint NumGammaControlPoints;
        [MarshalAs(UnmanagedType.R4, SizeConst = 1025)]
        float ControlPointPositions;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DXGI_GAMMA_CONTROL
    {
        public DXGI_RGB Scale;
        public DXGI_RGB Offset;
        [MarshalAs(UnmanagedType.Struct, SizeConst = 1025)]
        DXGI_RGB GammaCurve;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DXGI_RGB
    {
        public float Red;
        public float Green;
        public float Blue;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DXGI_FRAME_STATISTICS
    {
        public uint PresentCount;
        public uint PresentRefreshCount;
        public uint SyncRefreshCount;
        public LARGE_INTEGER SyncQPCTime;
        public LARGE_INTEGER SyncGPUTime;
    }

    [ComImport]
    [Guid("cafcb56c-6ac3-4889-bf47-9e23bbd260ec")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDXGISurface : IDXGIDeviceSubObject
    {
        #region <IDXGIDeviceSubObject>

        #region <IDXGIObject>
        new HRESULT SetPrivateData(ref Guid Name, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid Name, IntPtr pUnknown);
        new HRESULT GetPrivateData(ref Guid Name, ref uint pDataSize, out IntPtr pData);
        new HRESULT GetParent(ref Guid riid, out IntPtr ppParent);
        #endregion

        new HRESULT GetDevice(ref Guid riid, out IntPtr ppDevice);
        #endregion

        HRESULT GetDesc(out DXGI_SURFACE_DESC pDesc);
        [PreserveSig]
        HRESULT Map(out DXGI_MAPPED_RECT pLockedRect, uint MapFlags);
        [PreserveSig]
        HRESULT Unmap();
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DXGI_SURFACE_DESC
    {
        public uint Width;
        public uint Height;
        public DXGI_FORMAT Format;
        public DXGI_SAMPLE_DESC SampleDesc;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct DXGI_MAPPED_RECT
    {
        public int Pitch;
        public IntPtr pBits;
    };

    [ComImport]
    [Guid("7b7166ec-21c7-44ae-b21a-c9ae321ae369")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDXGIFactory : IDXGIObject
    {
        #region <IDXGIObject>
        new HRESULT SetPrivateData(ref Guid Name, uint DataSize, IntPtr pData);
        //HRESULT SetPrivateDataInterface(ref Guid Name, IUnknown* pUnknown);
        new HRESULT SetPrivateDataInterface(ref Guid Name, IntPtr pUnknown);
        new HRESULT GetPrivateData(ref Guid Name, ref uint pDataSize, out IntPtr pData);
        new HRESULT GetParent(ref Guid riid, out IntPtr ppParent);
        #endregion

        HRESULT EnumAdapters(uint Adapter, out IDXGIAdapter ppAdapter);
        HRESULT MakeWindowAssociation(IntPtr WindowHandle, uint Flags);
        HRESULT GetWindowAssociation(out IntPtr pWindowHandle);
        [PreserveSig]
        HRESULT CreateSwapChain(IntPtr pDevice, DXGI_SWAP_CHAIN_DESC pDesc, out IDXGISwapChain ppSwapChain);
        HRESULT CreateSoftwareAdapter(IntPtr Module, out IDXGIAdapter ppAdapter);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DXGI_SWAP_CHAIN_DESC
    {
        public DXGI_MODE_DESC BufferDesc;
        public DXGI_SAMPLE_DESC SampleDesc;
        public uint BufferUsage;
        public uint BufferCount;
        public IntPtr OutputWindow;
        public bool Windowed;
        public DXGI_SWAP_EFFECT SwapEffect;
        public uint Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DXGI_SAMPLE_DESC
    {
        public uint Count;
        public uint Quality;
    };

    public enum DXGI_SWAP_EFFECT
    {
        DXGI_SWAP_EFFECT_DISCARD = 0,
        DXGI_SWAP_EFFECT_SEQUENTIAL = 1,
        DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL = 3,
        DXGI_SWAP_EFFECT_FLIP_DISCARD = 4
    }

    [ComImport]
    [Guid("310d36a0-d2e7-4c0a-aa04-6a9d23b8886a")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDXGISwapChain : IDXGIDeviceSubObject
    {
        #region IDXGIDeviceSubObject
        #region IDXGIObject
        new HRESULT SetPrivateData(ref Guid Name, uint DataSize, IntPtr pData);
        //HRESULT SetPrivateDataInterface(ref Guid Name, IUnknown* pUnknown);
        new HRESULT SetPrivateDataInterface(ref Guid Name, IntPtr pUnknown);
        new HRESULT GetPrivateData(ref Guid Name, ref uint pDataSize, out IntPtr pData);
        new HRESULT GetParent(ref Guid riid, out IntPtr ppParent);
        #endregion

        new HRESULT GetDevice(ref Guid riid, out IntPtr ppDevice);
        #endregion

        [PreserveSig]
        HRESULT Present(uint SyncInterval, uint Flags);
        [PreserveSig]
        HRESULT GetBuffer(uint Buffer, ref Guid riid, out IntPtr ppSurface);
        HRESULT SetFullscreenState(bool Fullscreen, IDXGIOutput pTarget);
        HRESULT GetFullscreenState(out bool pFullscreen, out IDXGIOutput ppTarget);
        HRESULT GetDesc(out DXGI_SWAP_CHAIN_DESC pDesc);
        HRESULT ResizeBuffers(uint BufferCount, uint Width, uint Height, DXGI_FORMAT NewFormat, uint SwapChainFlags);
        HRESULT ResizeTarget(DXGI_MODE_DESC pNewTargetParameters);
        HRESULT GetContainingOutput(out IDXGIOutput ppOutput);
        HRESULT GetFrameStatistics(out DXGI_FRAME_STATISTICS pStats);
        HRESULT GetLastPresentCount(out uint pLastPresentCount);
    }

    [ComImport]
    [Guid("770aae78-f26f-4dba-a829-253c83d1b387")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDXGIFactory1 : IDXGIFactory
    {
        #region <IDXGIObject>
        new HRESULT SetPrivateData(ref Guid Name, uint DataSize, IntPtr pData);
        //HRESULT SetPrivateDataInterface(ref Guid Name, IUnknown* pUnknown);
        new HRESULT SetPrivateDataInterface(ref Guid Name, IntPtr pUnknown);
        new HRESULT GetPrivateData(ref Guid Name, ref uint pDataSize, out IntPtr pData);
        new HRESULT GetParent(ref Guid riid, out IntPtr ppParent);
        #endregion

        #region <IDXGIFactory>
        new HRESULT EnumAdapters(uint Adapter, out IDXGIAdapter ppAdapter);
        new HRESULT MakeWindowAssociation(IntPtr WindowHandle, uint Flags);
        new HRESULT GetWindowAssociation(out IntPtr pWindowHandle);
        [PreserveSig]
        new HRESULT CreateSwapChain(IntPtr pDevice, DXGI_SWAP_CHAIN_DESC pDesc, out IDXGISwapChain ppSwapChain);
        new HRESULT CreateSoftwareAdapter(IntPtr Module, out IDXGIAdapter ppAdapter);
        #endregion

        HRESULT EnumAdapters1(uint Adapter, out IDXGIAdapter1 ppAdapter);
        bool IsCurrent();
    }

    [ComImport]
    [Guid("29038f61-3839-4626-91fd-086879011a05")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDXGIAdapter1 : IDXGIAdapter
    {
        #region IDXGIAdapter
        #region IDXGIObject
        new HRESULT SetPrivateData(ref Guid Name, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid Name, IntPtr pUnknown);
        new HRESULT GetPrivateData(ref Guid Name, ref uint pDataSize, out IntPtr pData);
        new HRESULT GetParent(ref Guid riid, out IntPtr ppParent);
        #endregion

        [PreserveSig]
        new HRESULT EnumOutputs(uint Output, ref IDXGIOutput ppOutput);
        new HRESULT GetDesc(out DXGI_ADAPTER_DESC pDesc);
        new HRESULT CheckInterfaceSupport(ref Guid InterfaceName, out LARGE_INTEGER pUMDVersion);
        #endregion

        HRESULT GetDesc1(DXGI_ADAPTER_DESC1 pDesc);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DXGI_ADAPTER_DESC1
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Description;
        public uint VendorId;
        public uint DeviceId;
        public uint SubSysId;
        public uint Revision;
        public uint DedicatedVideoMemory;
        public uint DedicatedSystemMemory;
        public uint SharedSystemMemory;
        public LUID AdapterLuid;
        public uint Flags;
    }

    [ComImport]
    [Guid("50c83a1c-e072-4c48-87b0-3630fa36a6d0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDXGIFactory2 : IDXGIFactory1
    {
        #region <IDXGIFactory1>
        #region <IDXGIObject>
        new HRESULT SetPrivateData(ref Guid Name, uint DataSize, IntPtr pData);
        //HRESULT SetPrivateDataInterface(ref Guid Name, IUnknown* pUnknown);
        new HRESULT SetPrivateDataInterface(ref Guid Name, IntPtr pUnknown);
        new HRESULT GetPrivateData(ref Guid Name, ref uint pDataSize, out IntPtr pData);
        new HRESULT GetParent(ref Guid riid, out IntPtr ppParent);
        #endregion

        #region <IDXGIFactory>
        new HRESULT EnumAdapters(uint Adapter, out IDXGIAdapter ppAdapter);
        new HRESULT MakeWindowAssociation(IntPtr WindowHandle, uint Flags);
        new HRESULT GetWindowAssociation(out IntPtr pWindowHandle);
        [PreserveSig]
        new HRESULT CreateSwapChain(IntPtr pDevice, DXGI_SWAP_CHAIN_DESC pDesc, out IDXGISwapChain ppSwapChain);
        new HRESULT CreateSoftwareAdapter(IntPtr Module, out IDXGIAdapter ppAdapter);
        #endregion

        new HRESULT EnumAdapters1(uint Adapter, out IDXGIAdapter1 ppAdapter);
        new bool IsCurrent();
        #endregion

        bool IsWindowedStereoEnabled();
        //HRESULT CreateSwapChainForHwnd(IntPtr pDevice, IntPtr hWnd, DXGI_SWAP_CHAIN_DESC1 pDesc, DXGI_SWAP_CHAIN_FULLSCREEN_DESC pFullscreenDesc, IDXGIOutput pRestrictToOutput, out IDXGISwapChain1 ppSwapChain);
        [PreserveSig]
        HRESULT CreateSwapChainForHwnd(IntPtr pDevice, IntPtr hWnd, ref DXGI_SWAP_CHAIN_DESC1 pDesc, IntPtr pFullscreenDesc, IDXGIOutput pRestrictToOutput, out IDXGISwapChain1 ppSwapChain);
        [PreserveSig]
        HRESULT CreateSwapChainForCoreWindow(IntPtr pDevice, IntPtr pWindow, ref DXGI_SWAP_CHAIN_DESC1 pDesc, IDXGIOutput pRestrictToOutput, out IDXGISwapChain1 ppSwapChain);
        HRESULT GetSharedResourceAdapterLuid(IntPtr hResource, out LUID pLuid);
        HRESULT RegisterStereoStatusWindow(IntPtr WindowHandle, uint wMsg, out uint pdwCookie);
        HRESULT RegisterStereoStatusEvent(IntPtr hEvent, out uint pdwCookie);
        void UnregisterStereoStatus(uint dwCookie);
        HRESULT RegisterOcclusionStatusWindow(IntPtr WindowHandle, uint wMsg, out uint pdwCookie);
        HRESULT RegisterOcclusionStatusEvent(IntPtr hEvent, out uint pdwCookie);
        void UnregisterOcclusionStatus(uint dwCookie);
        [PreserveSig]
        HRESULT CreateSwapChainForComposition(IntPtr pDevice, ref DXGI_SWAP_CHAIN_DESC1 pDesc, IDXGIOutput pRestrictToOutput, out IDXGISwapChain1 ppSwapChain);
    }

    [ComImport]
    [Guid("790a45f7-0d42-4876-983a-0a55cfe6f4aa")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDXGISwapChain1 : IDXGISwapChain
    {
        #region IDXGISwapChain
        #region IDXGIDeviceSubObject
        #region IDXGIObject
        new HRESULT SetPrivateData(ref Guid Name, uint DataSize, IntPtr pData);
        //HRESULT SetPrivateDataInterface(ref Guid Name, IUnknown* pUnknown);
        new HRESULT SetPrivateDataInterface(ref Guid Name, IntPtr pUnknown);
        new HRESULT GetPrivateData(ref Guid Name, ref uint pDataSize, out IntPtr pData);
        new HRESULT GetParent(ref Guid riid, out IntPtr ppParent);
        #endregion

        new HRESULT GetDevice(ref Guid riid, out IntPtr ppDevice);
        #endregion

        [PreserveSig]
        new HRESULT Present(uint SyncInterval, uint Flags);
        [PreserveSig]
        new HRESULT GetBuffer(uint Buffer, ref Guid riid, out IntPtr ppSurface);
        new HRESULT SetFullscreenState(bool Fullscreen, IDXGIOutput pTarget);
        new HRESULT GetFullscreenState(out bool pFullscreen, out IDXGIOutput ppTarget);
        new HRESULT GetDesc(out DXGI_SWAP_CHAIN_DESC pDesc);

        [PreserveSig]
        new HRESULT ResizeBuffers(uint BufferCount, uint Width, uint Height, DXGI_FORMAT NewFormat, uint SwapChainFlags);
        new HRESULT ResizeTarget(DXGI_MODE_DESC pNewTargetParameters);
        new HRESULT GetContainingOutput(out IDXGIOutput ppOutput);
        [PreserveSig]
        new HRESULT GetFrameStatistics(out DXGI_FRAME_STATISTICS pStats);
        new HRESULT GetLastPresentCount(out uint pLastPresentCount);
        #endregion

        HRESULT GetDesc1(out DXGI_SWAP_CHAIN_DESC1 pDesc);
        HRESULT GetFullscreenDesc(out DXGI_SWAP_CHAIN_FULLSCREEN_DESC pDesc);
        HRESULT GetIntPtr(out IntPtr pIntPtr);
        HRESULT GetCoreWindow(ref Guid refiid, out IntPtr ppUnk);
        [PreserveSig]
        HRESULT Present1(uint SyncInterval, uint PresentFlags, DXGI_PRESENT_PARAMETERS pPresentParameters);
        bool IsTemporaryMonoSupported();
        HRESULT GetRestrictToOutput(out IDXGIOutput ppRestrictToOutput);
        HRESULT SetBackgroundColor(DXGI_RGBA pColor);
        HRESULT GetBackgroundColor(out DXGI_RGBA pColor);
        HRESULT SetRotation(DXGI_MODE_ROTATION Rotation);
        HRESULT GetRotation(out DXGI_MODE_ROTATION pRotation);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DXGI_SWAP_CHAIN_DESC1
    {
        public uint Width;
        public uint Height;
        public DXGI_FORMAT Format;
        public bool Stereo;
        public DXGI_SAMPLE_DESC SampleDesc;
        public uint BufferUsage;
        public uint BufferCount;
        public DXGI_SCALING Scaling;
        public DXGI_SWAP_EFFECT SwapEffect;
        public DXGI_ALPHA_MODE AlphaMode;
        public DXGI_SWAP_CHAIN_FLAG Flags;
    }

    public enum DXGI_SCALING
    {
        DXGI_SCALING_STRETCH = 0,
        DXGI_SCALING_NONE = 1,
        DXGI_SCALING_ASPECT_RATIO_STRETCH = 2
    }

    public enum DXGI_ALPHA_MODE : uint
    {
        DXGI_ALPHA_MODE_UNSPECIFIED = 0,
        DXGI_ALPHA_MODE_PREMULTIPLIED = 1,
        DXGI_ALPHA_MODE_STRAIGHT = 2,
        DXGI_ALPHA_MODE_IGNORE = 3,
        DXGI_ALPHA_MODE_FORCE_DWORD = 0xffffffff
    }

    public enum DXGI_SWAP_CHAIN_FLAG
    {
        DXGI_SWAP_CHAIN_FLAG_NONPREROTATED = 1,
        DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH = 2,
        DXGI_SWAP_CHAIN_FLAG_GDI_COMPATIBLE = 4,
        DXGI_SWAP_CHAIN_FLAG_RESTRICTED_CONTENT = 8,
        DXGI_SWAP_CHAIN_FLAG_RESTRICT_SHARED_RESOURCE_DRIVER = 16,
        DXGI_SWAP_CHAIN_FLAG_DISPLAY_ONLY = 32,
        DXGI_SWAP_CHAIN_FLAG_FRAME_LATENCY_WAITABLE_OBJECT = 64,
        DXGI_SWAP_CHAIN_FLAG_FOREGROUND_LAYER = 128,
        DXGI_SWAP_CHAIN_FLAG_FULLSCREEN_VIDEO = 256,
        DXGI_SWAP_CHAIN_FLAG_YUV_VIDEO = 512,
        DXGI_SWAP_CHAIN_FLAG_HW_PROTECTED = 1024,
        DXGI_SWAP_CHAIN_FLAG_ALLOW_TEARING = 2048,
        DXGI_SWAP_CHAIN_FLAG_RESTRICTED_TO_ALL_HOLOGRAPHIC_DISPLAYS = 4096
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DXGI_SWAP_CHAIN_FULLSCREEN_DESC
    {
        public DXGI_RATIONAL RefreshRate;
        public DXGI_MODE_SCANLINE_ORDER ScanlineOrdering;
        public DXGI_MODE_SCALING Scaling;
        public bool Windowed;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DXGI_PRESENT_PARAMETERS
    {
        public uint DirtyRectsCount;
        public RECT pDirtyRects;
        public RECT pScrollRect;
        public POINT pScrollOffset;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DXGI_RGBA
    {
        public float r;
        public float g;
        public float b;
        public float a;
    }

    [ComImport]
    [Guid("54ec77fa-1377-44e6-8c32-88fd5f44c84c")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDXGIDevice : IDXGIObject
    {
        #region IDXGIObject
        new HRESULT SetPrivateData(ref Guid Name, uint DataSize, IntPtr pData);
        //HRESULT SetPrivateDataInterface(ref Guid Name, IUnknown* pUnknown);
        new HRESULT SetPrivateDataInterface(ref Guid Name, IntPtr pUnknown);
        new HRESULT GetPrivateData(ref Guid Name, ref uint pDataSize, out IntPtr pData);
        new HRESULT GetParent(ref Guid riid, out IntPtr ppParent);
        #endregion

        HRESULT GetAdapter(out IDXGIAdapter pAdapter);
        HRESULT CreateSurface(DXGI_SURFACE_DESC pDesc, uint NumSurfaces, uint Usage, ref DXGI_SHARED_RESOURCE pSharedResource, out IDXGISurface ppSurface);
        HRESULT QueryResourceResidency(IntPtr ppResources, out DXGI_RESIDENCY pResidencyStatus, uint NumResources);
        HRESULT SetGPUThreadPriority(int Priority);
        HRESULT GetGPUThreadPriority(out int pPriority);
    }

    [ComImport]
    [Guid("77db970f-6276-48ba-ba28-070143b4392c")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDXGIDevice1 : IDXGIDevice
    {
        #region IDXGIDevice
        #region IDXGIObject
        new HRESULT SetPrivateData(ref Guid Name, uint DataSize, IntPtr pData);
        //HRESULT SetPrivateDataInterface(ref Guid Name, IUnknown* pUnknown);
        new HRESULT SetPrivateDataInterface(ref Guid Name, IntPtr pUnknown);
        new HRESULT GetPrivateData(ref Guid Name, ref uint pDataSize, out IntPtr pData);
        new HRESULT GetParent(ref Guid riid, out IntPtr ppParent);
        #endregion

        new HRESULT GetAdapter(out IDXGIAdapter pAdapter);
        new HRESULT CreateSurface(DXGI_SURFACE_DESC pDesc, uint NumSurfaces, uint Usage, ref DXGI_SHARED_RESOURCE pSharedResource, out IDXGISurface ppSurface);
        new HRESULT QueryResourceResidency(IntPtr ppResources, out DXGI_RESIDENCY pResidencyStatus, uint NumResources);
        new HRESULT SetGPUThreadPriority(int Priority);
        new HRESULT GetGPUThreadPriority(out int pPriority);
        #endregion

        HRESULT SetMaximumFrameLatency(uint MaxLatency);
        HRESULT GetMaximumFrameLatency(out uint pMaxLatency);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DXGI_SHARED_RESOURCE
    {
        public IntPtr Handle;
    }

    public enum DXGI_RESIDENCY
    {
        DXGI_RESIDENCY_FULLY_RESIDENT = 1,
        DXGI_RESIDENCY_RESIDENT_IN_SHARED_MEMORY = 2,
        DXGI_RESIDENCY_EVICTED_TO_DISK = 3
    }

    [ComImport]
    [Guid("eb533d5d-2db6-40f8-97a9-494692014f07")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFDXGIDeviceManager
    {
        HRESULT CloseDeviceIntPtr(IntPtr hDevice);
        HRESULT GetVideoService(IntPtr hDevice, ref Guid riid, out IntPtr ppService);
        HRESULT LockDevice(IntPtr hDevice, ref Guid riid, out IntPtr ppUnkDevice, bool fBlock);
        HRESULT OpenDeviceHandle(out IntPtr phDevice);
        HRESULT ResetDevice(IntPtr pUnkDevice, uint resetToken);
        HRESULT TestDevice(IntPtr hDevice);
        HRESULT UnlockDevice(IntPtr hDevice, bool fSaveState);
    }

    [ComImport]
    [Guid("9B7E4E00-342C-4106-A19F-4F2704F689F0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D10Multithread
    {
        void Enter();
        void Leave();
        bool SetMultithreadProtected(bool bMTProtect);
        bool GetMultithreadProtected();
    }

    [ComImport]
    [Guid("a8be2ac4-199f-4946-b331-79599fb98de7")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDXGISwapChain2 : IDXGISwapChain1
    {
        #region IDXGISwapChain1
        #region IDXGISwapChain
        #region IDXGIDeviceSubObject
        #region IDXGIObject
        new HRESULT SetPrivateData(ref Guid Name, uint DataSize, IntPtr pData);
        //HRESULT SetPrivateDataInterface(ref Guid Name, IUnknown* pUnknown);
        new HRESULT SetPrivateDataInterface(ref Guid Name, IntPtr pUnknown);
        new HRESULT GetPrivateData(ref Guid Name, ref uint pDataSize, out IntPtr pData);
        new HRESULT GetParent(ref Guid riid, out IntPtr ppParent);
        #endregion

        new HRESULT GetDevice(ref Guid riid, out IntPtr ppDevice);
        #endregion

        [PreserveSig]
        new HRESULT Present(uint SyncInterval, uint Flags);
        new HRESULT GetBuffer(uint Buffer, ref Guid riid, out IntPtr ppSurface);
        new HRESULT SetFullscreenState(bool Fullscreen, IDXGIOutput pTarget);
        new HRESULT GetFullscreenState(out bool pFullscreen, out IDXGIOutput ppTarget);
        new HRESULT GetDesc(out DXGI_SWAP_CHAIN_DESC pDesc);

        [PreserveSig]
        new HRESULT ResizeBuffers(uint BufferCount, uint Width, uint Height, DXGI_FORMAT NewFormat, uint SwapChainFlags);
        new HRESULT ResizeTarget(DXGI_MODE_DESC pNewTargetParameters);
        new HRESULT GetContainingOutput(out IDXGIOutput ppOutput);
        new HRESULT GetFrameStatistics(out DXGI_FRAME_STATISTICS pStats);
        new HRESULT GetLastPresentCount(out uint pLastPresentCount);
        #endregion

        new HRESULT GetDesc1(out DXGI_SWAP_CHAIN_DESC1 pDesc);
        new HRESULT GetFullscreenDesc(out DXGI_SWAP_CHAIN_FULLSCREEN_DESC pDesc);
        new HRESULT GetIntPtr(out IntPtr pIntPtr);
        new HRESULT GetCoreWindow(ref Guid refiid, out IntPtr ppUnk);
        [PreserveSig]
        new HRESULT Present1(uint SyncInterval, uint PresentFlags, DXGI_PRESENT_PARAMETERS pPresentParameters);
        new bool IsTemporaryMonoSupported();
        new HRESULT GetRestrictToOutput(out IDXGIOutput ppRestrictToOutput);
        new HRESULT SetBackgroundColor(DXGI_RGBA pColor);
        new HRESULT GetBackgroundColor(out DXGI_RGBA pColor);
        new HRESULT SetRotation(DXGI_MODE_ROTATION Rotation);
        new HRESULT GetRotation(out DXGI_MODE_ROTATION pRotation);
        #endregion

        [PreserveSig]
        HRESULT SetSourceSize(uint Width, uint Height);
        HRESULT GetSourceSize(out uint pWidth, out uint pHeight);
        HRESULT SetMaximumFrameLatency(uint MaxLatency);
        HRESULT GetMaximumFrameLatency(out uint pMaxLatency);
        IntPtr GetFrameLatencyWaitableObject();
        [PreserveSig]
        HRESULT SetMatrixTransform(ref DXGI_MATRIX_3X2_F pMatrix);
        HRESULT GetMatrixTransform(out DXGI_MATRIX_3X2_F pMatrix);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DXGI_MATRIX_3X2_F
    {
        public float _11;
        public float _12;
        public float _21;
        public float _22;
        public float _31;
        public float _32;
    }

    [ComImport]
    [Guid("4AE63092-6327-4c1b-80AE-BFE12EA32B86")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDXGISurface1 : IDXGISurface
    {
        #region <IDXGISurface>
        #region <IDXGIDeviceSubObject>
        #region <IDXGIObject>
        new HRESULT SetPrivateData(ref Guid Name, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid Name, IntPtr pUnknown);
        new HRESULT GetPrivateData(ref Guid Name, ref uint pDataSize, out IntPtr pData);
        new HRESULT GetParent(ref Guid riid, out IntPtr ppParent);
        #endregion
        new HRESULT GetDevice(ref Guid riid, out IntPtr ppDevice);
        #endregion
        new HRESULT GetDesc(out DXGI_SURFACE_DESC pDesc);
        new HRESULT Map(out DXGI_MAPPED_RECT pLockedRect, uint MapFlags);
        new HRESULT Unmap();
        #endregion

        [PreserveSig]
        HRESULT GetDC(bool Discard, out IntPtr phdc);
        //HRESULT ReleaseDC( ref RECT pDirtyRect);
        HRESULT ReleaseDC(IntPtr pDirtyRect);
    }

    [ComImport]
    [Guid("aba496dd-b617-4cb8-a866-bc44d7eb1fa2")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDXGISurface2 : IDXGISurface1
    {
        #region <IDXGISurface1>
        #region <IDXGISurface>
        #region <IDXGIDeviceSubObject>
        #region <IDXGIObject>
        new HRESULT SetPrivateData(ref Guid Name, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid Name, IntPtr pUnknown);
        new HRESULT GetPrivateData(ref Guid Name, ref uint pDataSize, out IntPtr pData);
        new HRESULT GetParent(ref Guid riid, out IntPtr ppParent);
        #endregion
        new HRESULT GetDevice(ref Guid riid, out IntPtr ppDevice);
        #endregion
        new HRESULT GetDesc(out DXGI_SURFACE_DESC pDesc);
        new HRESULT Map(out DXGI_MAPPED_RECT pLockedRect, uint MapFlags);
        new HRESULT Unmap();
        #endregion

        [PreserveSig]
        new HRESULT GetDC(bool Discard, out IntPtr phdc);
        //new HRESULT ReleaseDC(ref RECT pDirtyRect);
        new HRESULT ReleaseDC(IntPtr pDirtyRect);
        #endregion

        HRESULT GetResource(ref Guid riid, out IntPtr ppParentResource, out uint pSubresourceIndex);
    }

    // D3D11

    [ComImport]
    [Guid("1841e5c8-16b0-489b-bcc8-44cfb0d5deae")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11DeviceChild
    {
        //void GetDevice(out ID3D11Device ppDevice);
        void GetDevice(out IntPtr ppDevice);
        HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
    }

    [ComImport]
    [Guid("dc8e63f3-d12b-4952-b47b-5e45026a862d")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11Resource : ID3D11DeviceChild
    {
        #region ID3D11DeviceChild
        //new void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion

        void GetType(out D3D11_RESOURCE_DIMENSION pResourceDimension);
        void SetEvictionPriority(uint EvictionPriority);
        uint GetEvictionPriority();
    }

    public enum D3D11_RESOURCE_DIMENSION
    {
        D3D11_RESOURCE_DIMENSION_UNKNOWN = 0,
        D3D11_RESOURCE_DIMENSION_BUFFER = 1,
        D3D11_RESOURCE_DIMENSION_TEXTURE1D = 2,
        D3D11_RESOURCE_DIMENSION_TEXTURE2D = 3,
        D3D11_RESOURCE_DIMENSION_TEXTURE3D = 4
    }

    [ComImport]
    [Guid("6f15aaf2-d208-4e89-9ab4-489535d34f9c")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11Texture2D : ID3D11Resource
    {
        #region ID3D11Resource
        #region ID3D11DeviceChild
        //new void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion

        new void GetType(out D3D11_RESOURCE_DIMENSION pResourceDimension);
        new void SetEvictionPriority(uint EvictionPriority);
        new uint GetEvictionPriority();
        #endregion

        void GetDesc(out D3D11_TEXTURE2D_DESC pDesc);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11_TEXTURE2D_DESC
    {
        public uint Width;
        public uint Height;
        public uint MipLevels;
        public uint ArraySize;
        public DXGI_FORMAT Format;
        public DXGI_SAMPLE_DESC SampleDesc;
        public D3D11_USAGE Usage;
        public uint BindFlags;
        public uint CPUAccessFlags;
        public uint MiscFlags;
    }
    public enum D3D11_USAGE
    {
        D3D11_USAGE_DEFAULT = 0,
        D3D11_USAGE_IMMUTABLE = 1,
        D3D11_USAGE_DYNAMIC = 2,
        D3D11_USAGE_STAGING = 3
    }

    [ComImport]
    [Guid("e7174cfa-1c9e-48b1-8866-626226bfc258")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFDXGIBuffer
    {
        HRESULT GetResource(ref Guid riid, out IntPtr ppvObject);
        HRESULT GetSubresourceIndex(out uint puSubresource);
        HRESULT GetUnknown(ref Guid guid, ref Guid riid, out IntPtr ppvObject);
        HRESULT SetUnknown(ref Guid guid, IntPtr pUnkData);
    }
 

    public class DXGITools
    {
        public static Guid IID_ID3D11Texture2D = new Guid("6f15aaf2-d208-4e89-9ab4-489535d34f9c");

        [DllImport("DXGI.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern HRESULT CreateDXGIFactory2(uint Flags, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IDXGIFactory2 ppFactory);

        public const int DXGI_CREATE_FACTORY_DEBUG = 0x01;

        [DllImport("Mfplat.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern HRESULT MFCreateDXGIDeviceManager(out uint resetToken, out IMFDXGIDeviceManager ppDeviceManager);

        [DllImport("D3D11.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern HRESULT D3D11CreateDevice(IDXGIAdapter pAdapter, D3D_DRIVER_TYPE DriverType, IntPtr Software, uint Flags, [MarshalAs(UnmanagedType.LPArray)] int[] pFeatureLevels,
            uint FeatureLevels, uint SDKVersion, out IntPtr ppDevice, out D3D_FEATURE_LEVEL pFeatureLevel, out IntPtr ppImmediateContext);

        public const int D3D11_SDK_VERSION = 7;

        public const int DXGI_MAP_READ = 1;
        public const int DXGI_MAP_WRITE = 2;
        public const int DXGI_MAP_DISCARD = 4;

        public const int DXGI_MAX_SWAP_CHAIN_BUFFERS = (16);
        public const int DXGI_PRESENT_TEST = 0x00000001;
        public const int DXGI_PRESENT_DO_NOT_SEQUENCE = 0x00000002;
        public const int DXGI_PRESENT_RESTART = 0x00000004;
        public const int DXGI_PRESENT_DO_NOT_WAIT = 0x00000008;
        public const int DXGI_PRESENT_STEREO_PREFER_RIGHT = 0x00000010;
        public const int DXGI_PRESENT_STEREO_TEMPORARY_MONO = 0x00000020;
        public const int DXGI_PRESENT_RESTRICT_TO_OUTPUT = 0x00000040;
        public const int DXGI_PRESENT_USE_DURATION = 0x00000100;
        public const int DXGI_PRESENT_ALLOW_TEARING = 0x00000200;

        public enum D3D_DRIVER_TYPE
        {
            D3D_DRIVER_TYPE_UNKNOWN = 0,
            D3D_DRIVER_TYPE_HARDWARE = (D3D_DRIVER_TYPE_UNKNOWN + 1),
            D3D_DRIVER_TYPE_REFERENCE = (D3D_DRIVER_TYPE_HARDWARE + 1),
            D3D_DRIVER_TYPE_NULL = (D3D_DRIVER_TYPE_REFERENCE + 1),
            D3D_DRIVER_TYPE_SOFTWARE = (D3D_DRIVER_TYPE_NULL + 1),
            D3D_DRIVER_TYPE_WARP = (D3D_DRIVER_TYPE_SOFTWARE + 1)
        }

        public enum D3D11_CREATE_DEVICE_FLAG
        {
            D3D11_CREATE_DEVICE_SINGLETHREADED = 0x1,
            D3D11_CREATE_DEVICE_DEBUG = 0x2,
            D3D11_CREATE_DEVICE_SWITCH_TO_REF = 0x4,
            D3D11_CREATE_DEVICE_PREVENT_INTERNAL_THREADING_OPTIMIZATIONS = 0x8,
            D3D11_CREATE_DEVICE_BGRA_SUPPORT = 0x20,
            D3D11_CREATE_DEVICE_DEBUGGABLE = 0x40,
            D3D11_CREATE_DEVICE_PREVENT_ALTERING_LAYER_SETTINGS_FROM_REGISTRY = 0x80,
            D3D11_CREATE_DEVICE_DISABLE_GPU_TIMEOUT = 0x100,
            D3D11_CREATE_DEVICE_VIDEO_SUPPORT = 0x800
        }

        public enum D3D_FEATURE_LEVEL
        {
            D3D_FEATURE_LEVEL_1_0_CORE = 0x1000,
            D3D_FEATURE_LEVEL_9_1 = 0x9100,
            D3D_FEATURE_LEVEL_9_2 = 0x9200,
            D3D_FEATURE_LEVEL_9_3 = 0x9300,
            D3D_FEATURE_LEVEL_10_0 = 0xa000,
            D3D_FEATURE_LEVEL_10_1 = 0xa100,
            D3D_FEATURE_LEVEL_11_0 = 0xb000,
            D3D_FEATURE_LEVEL_11_1 = 0xb100,
            D3D_FEATURE_LEVEL_12_0 = 0xc000,
            D3D_FEATURE_LEVEL_12_1 = 0xc100
        }

        public const int DXGI_USAGE_SHADER_INPUT = 0x00000010;
        public const int DXGI_USAGE_RENDER_TARGET_OUTPUT = 0x00000020;
        public const int DXGI_USAGE_BACK_BUFFER = 0x00000040;
        public const int DXGI_USAGE_SHARED = 0x00000080;
        public const int DXGI_USAGE_READ_ONLY = 0x00000100;
        public const int DXGI_USAGE_DISCARD_ON_PRESENT = 0x00000200;
        public const int DXGI_USAGE_UNORDERED_ACCESS = 0x00000400;

        //
        // DXGI status (success) codes
        //

        //
        // MessageId: DXGI_STATUS_OCCLUDED
        //
        // MessageText:
        //
        // The Present operation was invisible to the user.
        //
        public const HRESULT DXGI_STATUS_OCCLUDED = (HRESULT)0x087A0001;

        //
        // MessageId: DXGI_STATUS_CLIPPED
        //
        // MessageText:
        //
        // The Present operation was partially invisible to the user.
        //
        public const HRESULT DXGI_STATUS_CLIPPED = (HRESULT)0x087A0002;

        //
        // MessageId: DXGI_STATUS_NO_REDIRECTION
        //
        // MessageText:
        //
        // The driver is requesting that the DXGI runtime not use shared resources to communicate with the Desktop Window Manager.
        //
        public const HRESULT DXGI_STATUS_NO_REDIRECTION = (HRESULT)0x087A0004;

        //
        // MessageId: DXGI_STATUS_NO_DESKTOP_ACCESS
        //
        // MessageText:
        //
        // The Present operation was not visible because the Windows session has switched to another desktop (for example, ctrl-alt-de;.
        //
        public const HRESULT DXGI_STATUS_NO_DESKTOP_ACCESS = (HRESULT)0x087A0005;

        //
        // MessageId: DXGI_STATUS_GRAPHICS_VIDPN_SOURCE_IN_USE
        //
        // MessageText:
        //
        // The Present operation was not visible because the target monitor was being used for some other purpose.
        //
        public const HRESULT DXGI_STATUS_GRAPHICS_VIDPN_SOURCE_IN_USE = (HRESULT)0x087A0006;

        //
        // MessageId: DXGI_STATUS_MODE_CHANGED
        //
        // MessageText:
        //
        // The Present operation was not visible because the display mode changed. DXGI will have re-attempted the presentation.
        //
        public const HRESULT DXGI_STATUS_MODE_CHANGED = (HRESULT)0x087A0007;

        //
        // MessageId: DXGI_STATUS_MODE_CHANGE_IN_PROGRESS
        //
        // MessageText:
        //
        // The Present operation was not visible because another Direct3D device was attempting to take fullscreen mode at the time.
        //
        public const HRESULT DXGI_STATUS_MODE_CHANGE_IN_PROGRESS = (HRESULT)0x087A0008;

        //
        // DXGI error codes
        //

        //
        // MessageId: DXGI_ERROR_INVALID_CALL
        //
        // MessageText:
        //
        // The application made a call that is invalid. Either the parameters of the call or the state of some object was incorrect.
        // Enable the D3D debug layer in order to see details via debug messages.
        //
        public const HRESULT DXGI_ERROR_INVALID_CALL = (HRESULT)unchecked((int)0x887A0001);

        //
        // MessageId: DXGI_ERROR_NOT_FOUND
        //
        // MessageText:
        //
        // The object was not found. If calling IDXGIFactory::EnumAdaptes, there is no adapter with the specified ordinal.
        //
        public const HRESULT DXGI_ERROR_NOT_FOUND = (HRESULT)unchecked((int)0x887A0002);

        //
        // MessageId: DXGI_ERROR_MORE_DATA
        //
        // MessageText:
        //
        // The caller did not supply a sufficiently large buffer.
        //
        public const HRESULT DXGI_ERROR_MORE_DATA = (HRESULT)unchecked((int)0x887A0003);

        //
        // MessageId: DXGI_ERROR_UNSUPPORTED
        //
        // MessageText:
        //
        // The specified device interface or feature level is not supported on this system.
        //
        public const HRESULT DXGI_ERROR_UNSUPPORTED = (HRESULT)unchecked((int)0x887A0004);

        //
        // MessageId: DXGI_ERROR_DEVICE_REMOVED
        //
        // MessageText:
        //
        // The GPU device instance has been suspended. Use GetDeviceRemovedReason to determine the appropriate action.
        //
        public const HRESULT DXGI_ERROR_DEVICE_REMOVED = (HRESULT)unchecked((int)0x887A0005);

        //
        // MessageId: DXGI_ERROR_DEVICE_HUNG
        //
        // MessageText:
        //
        // The GPU will not respond to more commands, most likely because of an invalid command passed by the calling application.
        //
        public const HRESULT DXGI_ERROR_DEVICE_HUNG = (HRESULT)unchecked((int)0x887A0006);

        //
        // MessageId: DXGI_ERROR_DEVICE_RESET
        //
        // MessageText:
        //
        // The GPU will not respond to more commands, most likely because some other application submitted invalid commands.
        // The calling application should re-create the device and continue.
        //
        public const HRESULT DXGI_ERROR_DEVICE_RESET = (HRESULT)unchecked((int)0x887A0007);

        //
        // MessageId: DXGI_ERROR_WAS_STILL_DRAWING
        //
        // MessageText:
        //
        // The GPU was busy at the moment when the call was made, and the call was neither executed nor scheduled.
        //
        public const HRESULT DXGI_ERROR_WAS_STILL_DRAWING = (HRESULT)unchecked((int)0x887A000A);

        //
        // MessageId: DXGI_ERROR_FRAME_STATISTICS_DISJOINT
        //
        // MessageText:
        //
        // An event (such as power cycle) interrupted the gathering of presentation statistics. Any previous statistics should be
        // considered invalid.
        //
        public const HRESULT DXGI_ERROR_FRAME_STATISTICS_DISJOINT = (HRESULT)unchecked((int)0x887A000B);

        //
        // MessageId: DXGI_ERROR_GRAPHICS_VIDPN_SOURCE_IN_USE
        //
        // MessageText:
        //
        // Fullscreen mode could not be achieved because the specified output was already in use.
        //
        public const HRESULT DXGI_ERROR_GRAPHICS_VIDPN_SOURCE_IN_USE = (HRESULT)unchecked((int)0x887A000C);

        //
        // MessageId: DXGI_ERROR_DRIVER_INTERNAL_ERROR
        //
        // MessageText:
        //
        // An internal issue prevented the driver from carrying out the specified operation. The driver's state is probably suspect,
        // and the application should not continue.
        //
        public const HRESULT DXGI_ERROR_DRIVER_INTERNAL_ERROR = (HRESULT)unchecked((int)0x887A0020);

        //
        // MessageId: DXGI_ERROR_NONEXCLUSIVE
        //
        // MessageText:
        //
        // A global counter resource was in use, and the specified counter cannot be used by this Direct3D device at this time.
        //
        public const HRESULT DXGI_ERROR_NONEXCLUSIVE = (HRESULT)unchecked((int)0x887A0021);

        //
        // MessageId: DXGI_ERROR_NOT_CURRENTLY_AVAILABLE
        //
        // MessageText:
        //
        // A resource is not available at the time of the call, but may become available later.
        //
        public const HRESULT DXGI_ERROR_NOT_CURRENTLY_AVAILABLE = (HRESULT)unchecked((int)0x887A0022);

        //
        // MessageId: DXGI_ERROR_REMOTE_CLIENT_DISCONNECTED
        //
        // MessageText:
        //
        // The application's remote device has been removed due to session disconnect or network disconnect.
        // The application should call IDXGIFactory1::IsCurrent to find out when the remote device becomes available again.
        //
        public const HRESULT DXGI_ERROR_REMOTE_CLIENT_DISCONNECTED = (HRESULT)unchecked((int)0x887A0023);

        //
        // MessageId: DXGI_ERROR_REMOTE_OUTOFMEMORY
        //
        // MessageText:
        //
        // The device has been removed during a remote session because the remote computer ran out of memory.
        //
        public const HRESULT DXGI_ERROR_REMOTE_OUTOFMEMORY = (HRESULT)unchecked((int)0x887A0024);

        //
        // MessageId: DXGI_ERROR_ACCESS_LOST
        //
        // MessageText:
        //
        // The keyed mutex was abandoned.
        //
        public const HRESULT DXGI_ERROR_ACCESS_LOST = (HRESULT)unchecked((int)0x887A0026);

        //
        // MessageId: DXGI_ERROR_WAIT_TIMEOUT
        //
        // MessageText:
        //
        // The timeout value has elapsed and the resource is not yet available.
        //
        public const HRESULT DXGI_ERROR_WAIT_TIMEOUT = (HRESULT)unchecked((int)0x887A0027);

        //
        // MessageId: DXGI_ERROR_SESSION_DISCONNECTED
        //
        // MessageText:
        //
        // The output duplication has been turned off because the Windows session ended or was disconnected.
        // This happens when a remote user disconnects, or when "switch user" is used locally.
        //
        public const HRESULT DXGI_ERROR_SESSION_DISCONNECTED = (HRESULT)unchecked((int)0x887A0028);

        //
        // MessageId: DXGI_ERROR_RESTRICT_TO_OUTPUT_STALE
        //
        // MessageText:
        //
        // The DXGI output (monitor) to which the swapchain content was restricted, has been disconnected or changed.
        //
        public const HRESULT DXGI_ERROR_RESTRICT_TO_OUTPUT_STALE = (HRESULT)unchecked((int)0x887A0029);

        //
        // MessageId: DXGI_ERROR_CANNOT_PROTECT_CONTENT
        //
        // MessageText:
        //
        // DXGI is unable to provide content protection on the swapchain. This is typically caused by an older driver,
        // or by the application using a swapchain that is incompatible with content protection.
        //
        public const HRESULT DXGI_ERROR_CANNOT_PROTECT_CONTENT = (HRESULT)unchecked((int)0x887A002A);

        //
        // MessageId: DXGI_ERROR_ACCESS_DENIED
        //
        // MessageText:
        //
        // The application is trying to use a resource to which it does not have the required access privileges.
        // This is most commonly caused by writing to a shared resource with read-only access.
        //
        public const HRESULT DXGI_ERROR_ACCESS_DENIED = (HRESULT)unchecked((int)0x887A002B);

        //
        // MessageId: DXGI_ERROR_NAME_ALREADY_EXISTS
        //
        // MessageText:
        //
        // The application is trying to create a shared handle using a name that is already associated with some other resource.
        //
        public const HRESULT DXGI_ERROR_NAME_ALREADY_EXISTS = (HRESULT)unchecked((int)0x887A002C);

        //
        // MessageId: DXGI_ERROR_SDK_COMPONENT_MISSING
        //
        // MessageText:
        //
        // The application requested an operation that depends on an SDK component that is missing or mismatched.
        //
        public const HRESULT DXGI_ERROR_SDK_COMPONENT_MISSING = (HRESULT)unchecked((int)0x887A002D);

        //
        // MessageId: DXGI_ERROR_NOT_CURRENT
        //
        // MessageText:
        //
        // The DXGI objects that the application has created are no longer current & need to be recreated for this operation to be performed.
        //
        public const HRESULT DXGI_ERROR_NOT_CURRENT = (HRESULT)unchecked((int)0x887A002E);

        //
        // MessageId: DXGI_ERROR_HW_PROTECTION_OUTOFMEMORY
        //
        // MessageText:
        //
        // Insufficient HW protected memory exits for proper function.
        //
        public const HRESULT DXGI_ERROR_HW_PROTECTION_OUTOFMEMORY = (HRESULT)unchecked((int)0x887A0030);

        //
        // MessageId: DXGI_ERROR_DYNAMIC_CODE_POLICY_VIOLATION
        //
        // MessageText:
        //
        // Creating this device would violate the process's dynamic code policy.
        //
        public const HRESULT DXGI_ERROR_DYNAMIC_CODE_POLICY_VIOLATION = (HRESULT)unchecked((int)0x887A0031);

        //
        // MessageId: DXGI_ERROR_NON_COMPOSITED_UI
        //
        // MessageText:
        //
        // The operation failed because the compositor is not in control of the output.
        //
        public const HRESULT DXGI_ERROR_NON_COMPOSITED_UI = (HRESULT)unchecked((int)0x887A0032);


        //
        // DXCore error codes
        //

        //
        // MessageId: DXCORE_ERROR_EVENT_NOT_UNREGISTERED
        //
        // MessageText:
        //
        // The application failed to unregister from an event it registered for.
        //
        public const HRESULT DXCORE_ERROR_EVENT_NOT_UNREGISTERED = (HRESULT)unchecked((int)0x88800001);


        //
        // DXGI errors that are internal to the Desktop Window Manager
        //

        //
        // MessageId: DXGI_STATUS_UNOCCLUDED
        //
        // MessageText:
        //
        // The swapchain has become unoccluded.
        //
        public const HRESULT DXGI_STATUS_UNOCCLUDED = (HRESULT)unchecked((int)0x087A0009);

        //
        // MessageId: DXGI_STATUS_DDA_WAS_STILL_DRAWING
        //
        // MessageText:
        //
        // The adapter did not have access to the required resources to complete the Desktop Duplication Present() call, the Present() call needs to be made again
        //
        public const HRESULT DXGI_STATUS_DDA_WAS_STILL_DRAWING = (HRESULT)unchecked((int)0x087A000A);

        //
        // MessageId: DXGI_ERROR_MODE_CHANGE_IN_PROGRESS
        //
        // MessageText:
        //
        // An on-going mode change prevented completion of the call. The call may succeed if attempted later.
        //
        public const HRESULT DXGI_ERROR_MODE_CHANGE_IN_PROGRESS = (HRESULT)unchecked((int)0x887A0025);

        //
        // MessageId: DXGI_STATUS_PRESENT_REQUIRED
        //
        // MessageText:
        //
        // The present succeeded but the caller should present again on the next V-sync, even if there are no changes to the content.
        //
        public const HRESULT DXGI_STATUS_PRESENT_REQUIRED = (HRESULT)unchecked((int)0x087A002F);


        //
        // DXGI errors that are produced by the D3D Shader Cache component
        //

        //
        // MessageId: DXGI_ERROR_CACHE_CORRUPT
        //
        // MessageText:
        //
        // The cache is corrupt and either could not be opened or could not be reset.
        //
        public const HRESULT DXGI_ERROR_CACHE_CORRUPT = (HRESULT)unchecked((int)0x887A0033);

        //
        // MessageId: DXGI_ERROR_CACHE_FULL
        //
        // MessageText:
        //
        // This entry would cause the cache to exceed its quota. On a load operation, this may indicate exceeding the maximum in-memory size.
        //
        public const HRESULT DXGI_ERROR_CACHE_FULL = (HRESULT)unchecked((int)0x887A0034);

        //
        // MessageId: DXGI_ERROR_CACHE_HASH_COLLISION
        //
        // MessageText:
        //
        // A cache entry was found, but the key provided does not match the key stored in the entry.
        //
        public const HRESULT DXGI_ERROR_CACHE_HASH_COLLISION = (HRESULT)unchecked((int)0x887A0035);

        //
        // MessageId: DXGI_ERROR_ALREADY_EXISTS
        //
        // MessageText:
        //
        // The desired element already exists.
        //
        public const HRESULT DXGI_ERROR_ALREADY_EXISTS = (HRESULT)unchecked((int)0x887A0036);


        //
        // DXGI DDI
        //

        //
        // MessageId: DXGI_DDI_ERR_WASSTILLDRAWING
        //
        // MessageText:
        //
        // The GPU was busy when the operation was requested.
        //
        public const HRESULT DXGI_DDI_ERR_WASSTILLDRAWING = (HRESULT)unchecked((int)0x887B0001);

        //
        // MessageId: DXGI_DDI_ERR_UNSUPPORTED
        //
        // MessageText:
        //
        // The driver has rejected the creation of this resource.
        //
        public const HRESULT DXGI_DDI_ERR_UNSUPPORTED = (HRESULT)unchecked((int)0x887B0002);

        //
        // MessageId: DXGI_DDI_ERR_NONEXCLUSIVE
        //
        // MessageText:
        //
        // The GPU counter was in use by another process or d3d device when application requested access to it.
        //
        public const HRESULT DXGI_DDI_ERR_NONEXCLUSIVE = (HRESULT)unchecked((int)0x887B0003);

    }
}
