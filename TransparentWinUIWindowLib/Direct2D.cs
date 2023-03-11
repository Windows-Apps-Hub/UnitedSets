using System;
using System.Runtime.InteropServices;
using DXGI;
using WIC;
using GlobalStructures;
using static DXGI.DXGITools;

namespace Direct2D
{
    public enum D2D1_FACTORY_TYPE : uint
    {
        //
        // The resulting factory and derived resources may only be invoked serially.
        // Reference counts on resources are interlocked, however, resource and render
        // target state is not protected from multi-threaded access.
        //
        D2D1_FACTORY_TYPE_SINGLE_THREADED = 0,
        //
        // The resulting factory may be invoked from multiple threads. Returned resources
        // use interlocked reference counting and their state is protected.
        //
        D2D1_FACTORY_TYPE_MULTI_THREADED = 1,
        D2D1_FACTORY_TYPE_FORCE_DWORD = 0xffffffff
    }

    public enum D2D1_DEBUG_LEVEL : uint
    {
        D2D1_DEBUG_LEVEL_NONE = 0,
        D2D1_DEBUG_LEVEL_ERROR = 1,
        D2D1_DEBUG_LEVEL_WARNING = 2,
        D2D1_DEBUG_LEVEL_INFORMATION = 3,
        D2D1_DEBUG_LEVEL_FORCE_DWORD = 0xffffffff
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_FACTORY_OPTIONS
    {
        public D2D1_DEBUG_LEVEL debugLevel;
    }     

    public enum D2D1_RENDER_TARGET_TYPE
    {
        //
        // D2D is free to choose the render target type for the caller.
        //
        D2D1_RENDER_TARGET_TYPE_DEFAULT = 0,
        //
        // The render target will render using the CPU.
        //
        D2D1_RENDER_TARGET_TYPE_SOFTWARE = 1,
        //
        // The render target will render using the GPU.
        //
        D2D1_RENDER_TARGET_TYPE_HARDWARE = 2,
        D2D1_RENDER_TARGET_TYPE_FORCE_DWORD = unchecked((int)0xffffffff)
    }    

    public enum D2D1_ALPHA_MODE
    {
        //
        // Alpha mode should be determined implicitly. Some target surfaces do not supply
        // or imply this information in which case alpha must be specified.
        //
        D2D1_ALPHA_MODE_UNKNOWN = 0,
        //
        // Treat the alpha as premultipled.
        //
        D2D1_ALPHA_MODE_PREMULTIPLIED = 1,
        //
        // Opacity is in the 'A' component only.
        //
        D2D1_ALPHA_MODE_STRAIGHT = 2,
        //
        // Ignore any alpha channel information.
        //
        D2D1_ALPHA_MODE_IGNORE = 3,

        D2D1_ALPHA_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_PIXEL_FORMAT
    {
        public DXGI_FORMAT format;
        public D2D1_ALPHA_MODE alphaMode;
    }

    public enum D2D1_RENDER_TARGET_USAGE
    {
        D2D1_RENDER_TARGET_USAGE_NONE = 0x00000000,
        //
        // Rendering will occur locally, if a terminal-services session is established, the
        // bitmap updates will be sent to the terminal services client.
        //
        D2D1_RENDER_TARGET_USAGE_FORCE_BITMAP_REMOTING = 0x00000001,
        //
        // The render target will allow a call to GetDC on the ID2D1GdiInteropRenderTarget
        // interface. Rendering will also occur locally.
        //
        D2D1_RENDER_TARGET_USAGE_GDI_COMPATIBLE = 0x00000002,
        D2D1_RENDER_TARGET_USAGE_FORCE_DWORD = unchecked((int)0xffffffff)
    }   

    public enum D2D1_FEATURE_LEVEL
    {
        //
        // The caller does not require a particular underlying D3D device level.
        //
        D2D1_FEATURE_LEVEL_DEFAULT = 0,

        //
        // The D3D device level is DX9 compatible.
        //
        D2D1_FEATURE_LEVEL_9 = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_1,

        //
        // The D3D device level is DX10 compatible.
        //
        D2D1_FEATURE_LEVEL_10 = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_0,
        D2D1_FEATURE_LEVEL_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_RENDER_TARGET_PROPERTIES
    {
        public D2D1_RENDER_TARGET_TYPE type;
        public D2D1_PIXEL_FORMAT pixelFormat;
        public float dpiX;
        public float dpiY;
        public D2D1_RENDER_TARGET_USAGE usage;
        public D2D1_FEATURE_LEVEL minLevel;
    }

    public enum D2D1_WINDOW_STATE
    {
        D2D1_WINDOW_STATE_NONE = 0x0000000,
        D2D1_WINDOW_STATE_OCCLUDED = 0x0000001,
        D2D1_WINDOW_STATE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [ComImport]
    [Guid("2cd906a8-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Brush : ID2D1Resource
    {
        #region ID2D1Resource
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        void SetOpacity(float opacity);
        void SetTransform(D2D1_MATRIX_3X2_F transform);
        float GetOpacity();
        void GetTransform(out D2D1_MATRIX_3X2_F transform);
        //void  SetTransform(D2D1_MATRIX_3X2_F &transform)
        //{
        //    SetTransform(&transform);
        //}
    }

    [ComImport]
    [Guid("2cd906aa-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1BitmapBrush : ID2D1Brush
    {
        #region ID2D1Brush
        new void SetOpacity(float opacity);
        new void SetTransform(D2D1_MATRIX_3X2_F transform);
        new float GetOpacity();
        new void GetTransform(out D2D1_MATRIX_3X2_F transform);
        #endregion

        void SetExtendModeX(D2D1_EXTEND_MODE extendModeX);
        void SetExtendModeY(D2D1_EXTEND_MODE extendModeY);
        void SetInterpolationMode(D2D1_BITMAP_INTERPOLATION_MODE interpolationMode);
        void SetBitmap(ID2D1Bitmap bitmap);
        D2D1_EXTEND_MODE GetExtendModeX();
        D2D1_EXTEND_MODE GetExtendModeY();
        D2D1_BITMAP_INTERPOLATION_MODE GetInterpolationMode();
        void GetBitmap(out ID2D1Bitmap bitmap);
    }

    [ComImport]
    [Guid("2cd906ab-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1LinearGradientBrush : ID2D1Brush
    {
        #region ID2D1Brush
        new void SetOpacity(float opacity);
        new void SetTransform(D2D1_MATRIX_3X2_F transform);
        new float GetOpacity();
        new void GetTransform(out D2D1_MATRIX_3X2_F transform);
        #endregion

        void SetStartPoint(ref D2D1_POINT_2F startPoint);
        void SetEndPoint(ref D2D1_POINT_2F endPoint);
        D2D1_POINT_2F GetStartPoint();
        D2D1_POINT_2F GetEndPoint();
        void GetGradientStopCollection(out ID2D1GradientStopCollection gradientStopCollection);
    }

    [ComImport]
    [Guid("2cd906ac-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1RadialGradientBrush : ID2D1Brush
    {
        #region ID2D1Brush
        new void SetOpacity(float opacity);
        new void SetTransform(D2D1_MATRIX_3X2_F transform);
        new float GetOpacity();
        new void GetTransform(out D2D1_MATRIX_3X2_F transform);
        #endregion

        void SetCenter(ref D2D1_POINT_2F center);
        void SetGradientOriginOffset(ref D2D1_POINT_2F gradientOriginOffset);
        void SetRadiusX(float radiusX);
        void SetRadiusY(float radiusY);
        D2D1_POINT_2F GetCenter();
        D2D1_POINT_2F GetGradientOriginOffset();
        float GetRadiusX();
        float GetRadiusY();
        void GetGradientStopCollection(out ID2D1GradientStopCollection gradientStopCollection);
    }

    [ComImport]
    [Guid("2cd9069d-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1StrokeStyle : ID2D1Resource
    {
        #region ID2D1Resource
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        D2D1_CAP_STYLE GetStartCap();
        D2D1_CAP_STYLE GetEndCap();
        D2D1_CAP_STYLE GetDashCap();
        float GetMiterLimit();
        D2D1_LINE_JOIN GetLineJoin();
        float GetDashOffset();
        D2D1_DASH_STYLE GetDashStyle();
        uint GetDashesCount();
        void GetDashes(out float dashes, uint dashesCount);
    }

    public enum D2D1_GEOMETRY_RELATION
    {
        //
        // The relation between the geometries couldn't be determined. This value is never
        // returned by any D2D method.
        //
        D2D1_GEOMETRY_RELATION_UNKNOWN = 0,
        //
        // The two geometries do not intersect at all.
        //
        D2D1_GEOMETRY_RELATION_DISJOINT = 1,
        //
        // The passed in geometry is entirely contained by the object.            //
        D2D1_GEOMETRY_RELATION_IS_CONTAINED = 2,
        //
        // The object entirely contains the passed in geometry.
        //
        D2D1_GEOMETRY_RELATION_CONTAINS = 3,
        //
        // The two geometries overlap but neither completely contains the other.
        //
        D2D1_GEOMETRY_RELATION_OVERLAP = 4,
        D2D1_GEOMETRY_RELATION_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_GEOMETRY_SIMPLIFICATION_OPTION
    {
        D2D1_GEOMETRY_SIMPLIFICATION_OPTION_CUBICS_AND_LINES = 0,
        D2D1_GEOMETRY_SIMPLIFICATION_OPTION_LINES = 1,
        D2D1_GEOMETRY_SIMPLIFICATION_OPTION_FORCE_DWORD = unchecked((int)0xffffffff)
    }
    public enum D2D1_COMBINE_MODE
    {
        //
        // Produce a geometry representing the set of points contained in either
        // the first or the second geometry.
        //
        D2D1_COMBINE_MODE_UNION = 0,
        //
        // Produce a geometry representing the set of points common to the first
        // and the second geometries.
        //
        D2D1_COMBINE_MODE_INTERSECT = 1,
        //
        // Produce a geometry representing the set of points contained in the
        // first geometry or the second geometry, but not both.
        //
        D2D1_COMBINE_MODE_XOR = 2,
        //
        // Produce a geometry representing the set of points contained in the
        // first geometry but not the second geometry.
        //
        D2D1_COMBINE_MODE_EXCLUDE = 3,
        D2D1_COMBINE_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_PATH_SEGMENT
    {
        D2D1_PATH_SEGMENT_NONE = 0x00000000,
        D2D1_PATH_SEGMENT_FORCE_UNSTROKED = 0x00000001,
        D2D1_PATH_SEGMENT_FORCE_ROUND_LINE_JOIN = 0x00000002,
        D2D1_PATH_SEGMENT_FORCE_DWORD = unchecked((int)0xffffffff)
    }
    public enum D2D1_FIGURE_BEGIN
    {
        D2D1_FIGURE_BEGIN_FILLED = 0,
        D2D1_FIGURE_BEGIN_HOLLOW = 1,
        D2D1_FIGURE_BEGIN_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_BEZIER_SEGMENT
    {
        public D2D1_POINT_2F point1;
        public D2D1_POINT_2F point2;
        public D2D1_POINT_2F point3;
    }

    public enum D2D1_FIGURE_END
    {
        D2D1_FIGURE_END_OPEN = 0,
        D2D1_FIGURE_END_CLOSED = 1,
        D2D1_FIGURE_END_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [ComImport]
    [Guid("2cd9069e-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1SimplifiedGeometrySink
    {
        HRESULT SetFillMode(D2D1_FILL_MODE fillMode);
        HRESULT SetSegmentFlags(D2D1_PATH_SEGMENT vertexFlags);
        HRESULT BeginFigure(ref D2D1_POINT_2F startPoint, D2D1_FIGURE_BEGIN figureBegin);
        HRESULT AddLines([MarshalAs(UnmanagedType.LPArray)] D2D1_POINT_2F[] points, uint pointsCount);
        HRESULT AddBeziers(D2D1_BEZIER_SEGMENT beziers, uint beziersCount);
        HRESULT EndFigure(D2D1_FIGURE_END figureEnd);
        HRESULT Close();
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_QUADRATIC_BEZIER_SEGMENT
    {
        public D2D1_POINT_2F point1;
        public D2D1_POINT_2F point2;
    }

    public enum D2D1_SWEEP_DIRECTION
    {
        D2D1_SWEEP_DIRECTION_COUNTER_CLOCKWISE = 0,
        D2D1_SWEEP_DIRECTION_CLOCKWISE = 1,
        D2D1_SWEEP_DIRECTION_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_ARC_SIZE
    {
        D2D1_ARC_SIZE_SMALL = 0,
        D2D1_ARC_SIZE_LARGE = 1,
        D2D1_ARC_SIZE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_ARC_SEGMENT
    {
        public D2D1_POINT_2F point;
        public D2D1_SIZE_F size;
        public float rotationAngle;
        public D2D1_SWEEP_DIRECTION sweepDirection;
        public D2D1_ARC_SIZE arcSize;
    }

    [ComImport]
    [Guid("e0db51c3-6f77-4bae-b3d5-e47509b35838")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1GdiInteropRenderTarget
    {
        HRESULT GetDC(D2D1_DC_INITIALIZE_MODE mode, out IntPtr hdc);
        HRESULT ReleaseDC(ref RECT update);
    }

    public enum D2D1_DC_INITIALIZE_MODE
    {
        /// <summary>
        /// The contents of the D2D render target will be copied to the DC.
        /// </summary>
        D2D1_DC_INITIALIZE_MODE_COPY = 0,

        /// <summary>
        /// The contents of the DC will be cleared.
        /// </summary>
        D2D1_DC_INITIALIZE_MODE_CLEAR = 1,
        D2D1_DC_INITIALIZE_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [ComImport]
    [Guid("2cd9069f-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1GeometrySink : ID2D1SimplifiedGeometrySink
    {
        #region ID2D1SimplifiedGeometrySink
        new HRESULT SetFillMode(D2D1_FILL_MODE fillMode);
        new HRESULT SetSegmentFlags(D2D1_PATH_SEGMENT vertexFlags);
        new HRESULT BeginFigure(ref D2D1_POINT_2F startPoint, D2D1_FIGURE_BEGIN figureBegin);
        new HRESULT AddLines([MarshalAs(UnmanagedType.LPArray)] D2D1_POINT_2F[] points, uint pointsCount);
        new HRESULT AddBeziers(D2D1_BEZIER_SEGMENT beziers, uint beziersCount);
        new HRESULT EndFigure(D2D1_FIGURE_END figureEnd);
        new HRESULT Close();
        #endregion

        HRESULT AddLine(ref D2D1_POINT_2F point);
        HRESULT AddBezier(D2D1_BEZIER_SEGMENT bezier);
        HRESULT AddQuadraticBezier(D2D1_QUADRATIC_BEZIER_SEGMENT bezier);
        HRESULT AddQuadraticBeziers(D2D1_QUADRATIC_BEZIER_SEGMENT beziers, int beziersCount);
        HRESULT AddArc(D2D1_ARC_SEGMENT arc);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_TRIANGLE
    {
        public D2D1_POINT_2F point1;
        public D2D1_POINT_2F point2;
        public D2D1_POINT_2F point3;
    }

    [ComImport]
    [Guid("2cd906c1-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1TessellationSink
    {
        void AddTriangles(D2D1_TRIANGLE triangles, uint trianglesCount);
        void Close();
    }

    [ComImport]
    [Guid("2cd906a1-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Geometry : ID2D1Resource
    {
        #region ID2D1Resource
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        void GetBounds(D2D1_MATRIX_3X2_F worldTransform, out D2D1_RECT_F bounds);
        void GetWidenedBounds(float strokeWidth, ID2D1StrokeStyle strokeStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out D2D1_RECT_F bounds);
        void StrokeContainsPoint(ref D2D1_POINT_2F point, float strokeWidth, ID2D1StrokeStyle strokeStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out bool contains);
        void FillContainsPoint(ref D2D1_POINT_2F point, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out bool contains);
        void CompareWithGeometry(ID2D1Geometry inputGeometry, D2D1_MATRIX_3X2_F inputGeometryTransform, float flatteningTolerance, out D2D1_GEOMETRY_RELATION relation);
        void Simplify(D2D1_GEOMETRY_SIMPLIFICATION_OPTION simplificationOption, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        void Tessellate(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1TessellationSink tessellationSink);
        void CombineWithGeometry(ID2D1Geometry inputGeometry, D2D1_COMBINE_MODE combineMode, D2D1_MATRIX_3X2_F inputGeometryTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        void Outline(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        void ComputeArea(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out float area);
        void ComputeLength(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out float length);
        void ComputePointAtLength(float length, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out D2D1_POINT_2F point, out D2D1_POINT_2F unitTangentVector);
        void Widen(float strokeWidth, ID2D1StrokeStyle strokeStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
    }

    [ComImport]
    [Guid("2cd906a3-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1RoundedRectangleGeometry : ID2D1Geometry
    {
        #region ID2D1Geometry
        #region ID2D1Resource
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        new void GetBounds(D2D1_MATRIX_3X2_F worldTransform, out D2D1_RECT_F bounds);
        new void GetWidenedBounds(float strokeWidth, ID2D1StrokeStyle strokeStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out D2D1_RECT_F bounds);
        new void StrokeContainsPoint(ref D2D1_POINT_2F point, float strokeWidth, ID2D1StrokeStyle strokeStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out bool contains);
        new void FillContainsPoint(ref D2D1_POINT_2F point, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out bool contains);
        new void CompareWithGeometry(ID2D1Geometry inputGeometry, D2D1_MATRIX_3X2_F inputGeometryTransform, float flatteningTolerance, out D2D1_GEOMETRY_RELATION relation);
        new void Simplify(D2D1_GEOMETRY_SIMPLIFICATION_OPTION simplificationOption, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        new void Tessellate(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1TessellationSink tessellationSink);
        new void CombineWithGeometry(ID2D1Geometry inputGeometry, D2D1_COMBINE_MODE combineMode, D2D1_MATRIX_3X2_F inputGeometryTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        new void Outline(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        new void ComputeArea(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out float area);
        new void ComputeLength(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out float length);
        new void ComputePointAtLength(float length, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out D2D1_POINT_2F point, out D2D1_POINT_2F unitTangentVector);
        new void Widen(float strokeWidth, ID2D1StrokeStyle strokeStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);

        #endregion

        void GetRoundedRect(out D2D1_ROUNDED_RECT roundedRect);
    }

    [ComImport]
    [Guid("2cd906a4-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1EllipseGeometry : ID2D1Geometry
    {
        #region ID2D1Geometry
        #region ID2D1Resource
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        new void GetBounds(D2D1_MATRIX_3X2_F worldTransform, out D2D1_RECT_F bounds);
        new void GetWidenedBounds(float strokeWidth, ID2D1StrokeStyle strokeStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out D2D1_RECT_F bounds);
        new void StrokeContainsPoint(ref D2D1_POINT_2F point, float strokeWidth, ID2D1StrokeStyle strokeStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out bool contains);
        new void FillContainsPoint(ref D2D1_POINT_2F point, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out bool contains);
        new void CompareWithGeometry(ID2D1Geometry inputGeometry, D2D1_MATRIX_3X2_F inputGeometryTransform, float flatteningTolerance, out D2D1_GEOMETRY_RELATION relation);
        new void Simplify(D2D1_GEOMETRY_SIMPLIFICATION_OPTION simplificationOption, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        new void Tessellate(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1TessellationSink tessellationSink);
        new void CombineWithGeometry(ID2D1Geometry inputGeometry, D2D1_COMBINE_MODE combineMode, D2D1_MATRIX_3X2_F inputGeometryTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        new void Outline(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        new void ComputeArea(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out float area);
        new void ComputeLength(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out float length);
        new void ComputePointAtLength(float length, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out D2D1_POINT_2F point, out D2D1_POINT_2F unitTangentVector);
        new void Widen(float strokeWidth, ID2D1StrokeStyle strokeStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);

        #endregion

        void GetEllipse(out D2D1_ELLIPSE ellipse);
    }

    [ComImport]
    [Guid("2cd906a6-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1GeometryGroup : ID2D1Geometry
    {
        #region ID2D1Geometry
        #region ID2D1Resource
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        new void GetBounds(D2D1_MATRIX_3X2_F worldTransform, out D2D1_RECT_F bounds);
        new void GetWidenedBounds(float strokeWidth, ID2D1StrokeStyle strokeStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out D2D1_RECT_F bounds);
        new void StrokeContainsPoint(ref D2D1_POINT_2F point, float strokeWidth, ID2D1StrokeStyle strokeStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out bool contains);
        new void FillContainsPoint(ref D2D1_POINT_2F point, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out bool contains);
        new void CompareWithGeometry(ID2D1Geometry inputGeometry, D2D1_MATRIX_3X2_F inputGeometryTransform, float flatteningTolerance, out D2D1_GEOMETRY_RELATION relation);
        new void Simplify(D2D1_GEOMETRY_SIMPLIFICATION_OPTION simplificationOption, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        new void Tessellate(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1TessellationSink tessellationSink);
        new void CombineWithGeometry(ID2D1Geometry inputGeometry, D2D1_COMBINE_MODE combineMode, D2D1_MATRIX_3X2_F inputGeometryTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        new void Outline(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        new void ComputeArea(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out float area);
        new void ComputeLength(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out float length);
        new void ComputePointAtLength(float length, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out D2D1_POINT_2F point, out D2D1_POINT_2F unitTangentVector);
        new void Widen(float strokeWidth, ID2D1StrokeStyle strokeStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);

        #endregion

        D2D1_FILL_MODE GetFillMode();
        int GetSourceGeometryCount();
        void GetSourceGeometries(out ID2D1Geometry geometries, int geometriesCount);
    }

    [ComImport]
    [Guid("2cd906bb-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1TransformedGeometry : ID2D1Geometry
    {
        #region ID2D1Geometry
        #region ID2D1Resource
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        new void GetBounds(D2D1_MATRIX_3X2_F worldTransform, out D2D1_RECT_F bounds);
        new void GetWidenedBounds(float strokeWidth, ID2D1StrokeStyle strokeStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out D2D1_RECT_F bounds);
        new void StrokeContainsPoint(ref D2D1_POINT_2F point, float strokeWidth, ID2D1StrokeStyle strokeStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out bool contains);
        new void FillContainsPoint(ref D2D1_POINT_2F point, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out bool contains);
        new void CompareWithGeometry(ID2D1Geometry inputGeometry, D2D1_MATRIX_3X2_F inputGeometryTransform, float flatteningTolerance, out D2D1_GEOMETRY_RELATION relation);
        new void Simplify(D2D1_GEOMETRY_SIMPLIFICATION_OPTION simplificationOption, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        new void Tessellate(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1TessellationSink tessellationSink);
        new void CombineWithGeometry(ID2D1Geometry inputGeometry, D2D1_COMBINE_MODE combineMode, D2D1_MATRIX_3X2_F inputGeometryTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        new void Outline(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        new void ComputeArea(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out float area);
        new void ComputeLength(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out float length);
        new void ComputePointAtLength(float length, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out D2D1_POINT_2F point, out D2D1_POINT_2F unitTangentVector);
        new void Widen(float strokeWidth, ID2D1StrokeStyle strokeStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);

        #endregion

        void GetSourceGeometry(out ID2D1Geometry sourceGeometry);
        void GetTransform(out D2D1_MATRIX_3X2_F transform);
    }

    [ComImport]
    [Guid("2cd906a5-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1PathGeometry : ID2D1Geometry
    {
        #region ID2D1Geometry
        #region ID2D1Resource
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        new void GetBounds(D2D1_MATRIX_3X2_F worldTransform, out D2D1_RECT_F bounds);
        new void GetWidenedBounds(float strokeWidth, ID2D1StrokeStyle strokeStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out D2D1_RECT_F bounds);
        new void StrokeContainsPoint(ref D2D1_POINT_2F point, float strokeWidth, ID2D1StrokeStyle strokeStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out bool contains);
        new void FillContainsPoint(ref D2D1_POINT_2F point, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out bool contains);
        new void CompareWithGeometry(ID2D1Geometry inputGeometry, D2D1_MATRIX_3X2_F inputGeometryTransform, float flatteningTolerance, out D2D1_GEOMETRY_RELATION relation);
        new void Simplify(D2D1_GEOMETRY_SIMPLIFICATION_OPTION simplificationOption, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        new void Tessellate(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1TessellationSink tessellationSink);
        new void CombineWithGeometry(ID2D1Geometry inputGeometry, D2D1_COMBINE_MODE combineMode, D2D1_MATRIX_3X2_F inputGeometryTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        new void Outline(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        new void ComputeArea(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out float area);
        new void ComputeLength(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out float length);
        new void ComputePointAtLength(float length, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out D2D1_POINT_2F point, out D2D1_POINT_2F unitTangentVector);
        new void Widen(float strokeWidth, ID2D1StrokeStyle strokeStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);

        #endregion

        HRESULT Open(out ID2D1GeometrySink geometrySink);
        HRESULT Stream(ID2D1GeometrySink geometrySink);
        HRESULT GetSegmentCount(out int count);
        HRESULT GetFigureCount(out int count);
    }

    [ComImport]
    [Guid("28506e39-ebf6-46a1-bb47-fd85565ab957")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1DrawingStateBlock : ID2D1Resource
    {
        #region ID2D1Resource
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        void GetDescription(out D2D1_DRAWING_STATE_DESCRIPTION stateDescription);
        void SetDescription(D2D1_DRAWING_STATE_DESCRIPTION stateDescription);
        void SetTextRenderingParams(IDWriteRenderingParams textRenderingParams = null);
        void GetTextRenderingParams(out IDWriteRenderingParams textRenderingParams);
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct DWRITE_TEXT_RANGE
    {
        /// <summary>
        ///         ''' The start text position of the range.
        ///         ''' </summary>
        public uint startPosition;
        /// <summary>
        ///         ''' The number of text positions in the range.
        ///         ''' </summary>
        public uint length;
        public DWRITE_TEXT_RANGE(uint startPosition, uint length)
        {
            this.startPosition = startPosition;
            this.length = length;
        }
    }

    public enum DWRITE_FONT_FEATURE_TAG
    {
        DWRITE_FONT_FEATURE_TAG_ALTERNATIVE_FRACTIONS = 0x63726661, // 'afrc'
        DWRITE_FONT_FEATURE_TAG_PETITE_CAPITALS_FROM_CAPITALS = 0x63703263, // 'c2pc'
        DWRITE_FONT_FEATURE_TAG_SMALL_CAPITALS_FROM_CAPITALS = 0x63733263, // 'c2sc'
        DWRITE_FONT_FEATURE_TAG_CONTEXTUAL_ALTERNATES = 0x746c6163, // 'calt'
        DWRITE_FONT_FEATURE_TAG_CASE_SENSITIVE_FORMS = 0x65736163, // 'case'
        DWRITE_FONT_FEATURE_TAG_GLYPH_COMPOSITION_DECOMPOSITION = 0x706d6363, // 'ccmp'
        DWRITE_FONT_FEATURE_TAG_CONTEXTUAL_LIGATURES = 0x67696c63, // 'clig'
        DWRITE_FONT_FEATURE_TAG_CAPITAL_SPACING = 0x70737063, // 'cpsp'
        DWRITE_FONT_FEATURE_TAG_CONTEXTUAL_SWASH = 0x68777363, // 'cswh'
        DWRITE_FONT_FEATURE_TAG_CURSIVE_POSITIONING = 0x73727563, // 'curs'
        DWRITE_FONT_FEATURE_TAG_DEFAULT = 0x746c6664, // 'dflt'
        DWRITE_FONT_FEATURE_TAG_DISCRETIONARY_LIGATURES = 0x67696c64, // 'dlig'
        DWRITE_FONT_FEATURE_TAG_EXPERT_FORMS = 0x74707865, // 'expt'
        DWRITE_FONT_FEATURE_TAG_FRACTIONS = 0x63617266, // 'frac'
        DWRITE_FONT_FEATURE_TAG_FULL_WIDTH = 0x64697766, // 'fwid'
        DWRITE_FONT_FEATURE_TAG_HALF_FORMS = 0x666c6168, // 'half'
        DWRITE_FONT_FEATURE_TAG_HALANT_FORMS = 0x6e6c6168, // 'haln'
        DWRITE_FONT_FEATURE_TAG_ALTERNATE_HALF_WIDTH = 0x746c6168, // 'halt'
        DWRITE_FONT_FEATURE_TAG_HISTORICAL_FORMS = 0x74736968, // 'hist'
        DWRITE_FONT_FEATURE_TAG_HORIZONTAL_KANA_ALTERNATES = 0x616e6b68, // 'hkna'
        DWRITE_FONT_FEATURE_TAG_HISTORICAL_LIGATURES = 0x67696c68, // 'hlig'
        DWRITE_FONT_FEATURE_TAG_HALF_WIDTH = 0x64697768, // 'hwid'
        DWRITE_FONT_FEATURE_TAG_HOJO_KANJI_FORMS = 0x6f6a6f68, // 'hojo'
        DWRITE_FONT_FEATURE_TAG_JIS04_FORMS = 0x3430706a, // 'jp04'
        DWRITE_FONT_FEATURE_TAG_JIS78_FORMS = 0x3837706a, // 'jp78'
        DWRITE_FONT_FEATURE_TAG_JIS83_FORMS = 0x3338706a, // 'jp83'
        DWRITE_FONT_FEATURE_TAG_JIS90_FORMS = 0x3039706a, // 'jp90'
        DWRITE_FONT_FEATURE_TAG_KERNING = 0x6e72656b, // 'kern'
        DWRITE_FONT_FEATURE_TAG_STANDARD_LIGATURES = 0x6167696c, // 'liga'
        DWRITE_FONT_FEATURE_TAG_LINING_FIGURES = 0x6d756e6c, // 'lnum'
        DWRITE_FONT_FEATURE_TAG_LOCALIZED_FORMS = 0x6c636f6c, // 'locl'
        DWRITE_FONT_FEATURE_TAG_MARK_POSITIONING = 0x6b72616d, // 'mark'
        DWRITE_FONT_FEATURE_TAG_MATHEMATICAL_GREEK = 0x6b72676d, // 'mgrk'
        DWRITE_FONT_FEATURE_TAG_MARK_TO_MARK_POSITIONING = 0x6b6d6b6d, // 'mkmk'
        DWRITE_FONT_FEATURE_TAG_ALTERNATE_ANNOTATION_FORMS = 0x746c616e, // 'nalt'
        DWRITE_FONT_FEATURE_TAG_NLC_KANJI_FORMS = 0x6b636c6e, // 'nlck'
        DWRITE_FONT_FEATURE_TAG_OLD_STYLE_FIGURES = 0x6d756e6f, // 'onum'
        DWRITE_FONT_FEATURE_TAG_ORDINALS = 0x6e64726f, // 'ordn'
        DWRITE_FONT_FEATURE_TAG_PROPORTIONAL_ALTERNATE_WIDTH = 0x746c6170, // 'palt'
        DWRITE_FONT_FEATURE_TAG_PETITE_CAPITALS = 0x70616370, // 'pcap'
        DWRITE_FONT_FEATURE_TAG_PROPORTIONAL_FIGURES = 0x6d756e70, // 'pnum'
        DWRITE_FONT_FEATURE_TAG_PROPORTIONAL_WIDTHS = 0x64697770, // 'pwid'
        DWRITE_FONT_FEATURE_TAG_QUARTER_WIDTHS = 0x64697771, // 'qwid'
        DWRITE_FONT_FEATURE_TAG_REQUIRED_LIGATURES = 0x67696c72, // 'rlig'
        DWRITE_FONT_FEATURE_TAG_RUBY_NOTATION_FORMS = 0x79627572, // 'ruby'
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_ALTERNATES = 0x746c6173, // 'salt'
        DWRITE_FONT_FEATURE_TAG_SCIENTIFIC_INFERIORS = 0x666e6973, // 'sinf'
        DWRITE_FONT_FEATURE_TAG_SMALL_CAPITALS = 0x70636d73, // 'smcp'
        DWRITE_FONT_FEATURE_TAG_SIMPLIFIED_FORMS = 0x6c706d73, // 'smpl'
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_1 = 0x31307373, // 'ss01'
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_2 = 0x32307373, // 'ss02'
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_3 = 0x33307373, // 'ss03'
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_4 = 0x34307373, // 'ss04'
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_5 = 0x35307373, // 'ss05'
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_6 = 0x36307373, // 'ss06'
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_7 = 0x37307373, // 'ss07'
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_8 = 0x38307373, // 'ss08'
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_9 = 0x39307373, // 'ss09'
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_10 = 0x30317373, // 'ss10'
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_11 = 0x31317373, // 'ss11'
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_12 = 0x32317373, // 'ss12'
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_13 = 0x33317373, // 'ss13'
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_14 = 0x34317373, // 'ss14'
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_15 = 0x35317373, // 'ss15'
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_16 = 0x36317373, // 'ss16'
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_17 = 0x37317373, // 'ss17'
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_18 = 0x38317373, // 'ss18'
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_19 = 0x39317373, // 'ss19'
        DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_20 = 0x30327373, // 'ss20'
        DWRITE_FONT_FEATURE_TAG_SUBSCRIPT = 0x73627573, // 'subs'
        DWRITE_FONT_FEATURE_TAG_SUPERSCRIPT = 0x73707573, // 'sups'
        DWRITE_FONT_FEATURE_TAG_SWASH = 0x68737773, // 'swsh'
        DWRITE_FONT_FEATURE_TAG_TITLING = 0x6c746974, // 'titl'
        DWRITE_FONT_FEATURE_TAG_TRADITIONAL_NAME_FORMS = 0x6d616e74, // 'tnam'
        DWRITE_FONT_FEATURE_TAG_TABULAR_FIGURES = 0x6d756e74, // 'tnum'
        DWRITE_FONT_FEATURE_TAG_TRADITIONAL_FORMS = 0x64617274, // 'trad'
        DWRITE_FONT_FEATURE_TAG_THIRD_WIDTHS = 0x64697774, // 'twid'
        DWRITE_FONT_FEATURE_TAG_UNICASE = 0x63696e75, // 'unic'
        DWRITE_FONT_FEATURE_TAG_VERTICAL_WRITING = 0x74726576, // 'vert'
        DWRITE_FONT_FEATURE_TAG_VERTICAL_ALTERNATES_AND_ROTATION = 0x32747276, // 'vrt2'
        DWRITE_FONT_FEATURE_TAG_SLASHED_ZERO = 0x6f72657a, // 'zero'
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct DWRITE_FONT_FEATURE
    {
        /// <summary>
        /// The feature OpenType name identifier.
        /// </summary>
        public DWRITE_FONT_FEATURE_TAG nameTag;
        /// <summary>
        /// Execution parameter of the feature.
        /// </summary>
        /// <remarks>
        /// The parameter should be non-zero to enable the feature.  Once enabled, a feature can't be disabled again within
        /// the same range.  Features requiring a selector use this value to indicate the selector index. 
        /// </remarks>
        public uint parameter;
    };

    [ComImport]
    [Guid("55f1112b-1dc2-4b3c-9541-f46894ed85b6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteTypography
    {
        HRESULT AddFontFeature(DWRITE_FONT_FEATURE fontFeature);
        uint GetFontFeatureCount();
        HRESULT GetFontFeature(uint fontFeatureIndex, out DWRITE_FONT_FEATURE fontFeature);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DWRITE_LINE_METRICS
    {
        /// <summary>
        /// The number of total text positions in the line.
        /// This includes any trailing whitespace and newline characters.
        /// </summary>
        public int length;
        /// <summary>
        /// The number of whitespace positions at the end of the line.  Newline
        /// sequences are considered whitespace.
        /// </summary>
        public int trailingWhitespaceLength;
        /// <summary>
        /// The number of characters in the newline sequence at the end of the line.
        /// If the count is zero, then the line was either wrapped or it is the
        /// end of the text.
        /// </summary>
        public int newlineLength;
        /// <summary>
        /// Height of the line as measured from top to bottom.
        /// </summary>
        public float height;
        /// <summary>
        /// Distance from the top of the line to its baseline.
        /// </summary>
        public float baseline;
        /// <summary>
        /// The line is trimmed.
        /// </summary>
        public bool isTrimmed;
    };


    public struct DWRITE_TEXT_METRICS
    {
        /// <summary>
        /// Left-most point of formatted text relative to layout box
        /// (excluding any glyph overhang).
        /// </summary>
        public float left;
        /// <summary>
        /// Top-most point of formatted text relative to layout box
        /// (excluding any glyph overhang).
        /// </summary>
        public float top;
        /// <summary>
        /// The width of the formatted text ignoring trailing whitespace
        /// at the end of each line.
        /// </summary>
        public float width;
        /// <summary>
        /// The width of the formatted text taking into account the
        /// trailing whitespace at the end of each line.
        /// </summary>
        public float widthIncludingTrailingWhitespace;
        /// <summary>
        /// The height of the formatted text. The height of an empty string
        /// is determined by the size of the default font's line height.
        /// </summary>
        public float height;
        /// <summary>
        /// Initial width given to the layout. Depending on whether the text
        /// was wrapped or not, it can be either larger or smaller than the
        /// text content width.
        /// </summary>
        public float layoutWidth;
        /// <summary>
        /// Initial height given to the layout. Depending on the length of the
        /// text, it may be larger or smaller than the text content height.
        /// </summary>
        public float layoutHeight;
        /// <summary>
        /// The maximum reordering count of any line of text, used
        /// to calculate the most number of hit-testing boxes needed.
        /// If the layout has no bidirectional text or no text at all,
        /// the minimum level is 1.
        /// </summary>
        public int maxBidiReorderingDepth;
        /// <summary>
        /// Total number of lines.
        /// </summary>
        public int lineCount;
    };
    public struct DWRITE_CLUSTER_METRICS
    {
        /// <summary>
        /// The total advance width of all glyphs in the cluster.
        /// </summary>
        public float width;
        /// <summary>
        /// The number of text positions in the cluster.
        /// </summary>
        public UInt16 length;
        /// <summary>
        /// Indicate whether line can be broken right after the cluster.
        /// </summary>
        [MarshalAs(UnmanagedType.U2, SizeConst = 1)]
        public UInt16 canWrapLineAfter;
        /// <summary>
        /// Indicate whether the cluster corresponds to whitespace character.
        /// </summary>
        [MarshalAs(UnmanagedType.U2, SizeConst = 1)]
        public UInt16 isWhitespace;
        /// <summary>
        /// Indicate whether the cluster corresponds to a newline character.
        /// </summary>
        [MarshalAs(UnmanagedType.U2, SizeConst = 1)]
        public UInt16 isNewline;
        /// <summary>
        /// Indicate whether the cluster corresponds to soft hyphen character.
        /// </summary>
        [MarshalAs(UnmanagedType.U2, SizeConst = 1)]
        public UInt16 isSoftHyphen;
        /// <summary>
        /// Indicate whether the cluster is read from right to left.
        /// </summary>
        public UInt16 isRightToLeft;
        [MarshalAs(UnmanagedType.U2, SizeConst = 11)]
        public UInt16 padding;
    };

    public struct DWRITE_HIT_TEST_METRICS
    {
        /// <summary>
        /// First text position within the geometry.
        /// </summary>
        public int textPosition;
        /// <summary>
        /// Number of text positions within the geometry.
        /// </summary>
        public int length;
        /// <summary>
        /// Left position of the top-left coordinate of the geometry.
        /// </summary>
        public float left;
        /// <summary>
        /// Top position of the top-left coordinate of the geometry.
        /// </summary>
        public float top;
        /// <summary>
        /// Geometry's width.
        /// </summary>
        public float width;
        /// <summary>
        /// Geometry's height.
        /// </summary>
        public float height;
        /// <summary>
        /// Bidi level of text positions enclosed within the geometry.
        /// </summary>
        public int bidiLevel;
        /// <summary>
        /// Geometry encloses text?
        /// </summary>
        public bool isText;
        /// <summary>
        /// Range is trimmed.
        /// </summary>
        public bool isTrimmed;
    };

    [ComImport]
    [Guid("53737037-6d14-410b-9bfe-0b182bb70961")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteTextLayout : IDWriteTextFormat
    {
        #region IDWriteTextFormat
        new HRESULT SetTextAlignment(DWRITE_TEXT_ALIGNMENT textAlignment);
        new HRESULT SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT paragraphAlignment);
        new HRESULT SetWordWrapping(DWRITE_WORD_WRAPPING wordWrapping);
        new HRESULT SetReadingDirection(DWRITE_READING_DIRECTION readingDirection);
        new HRESULT SetFlowDirection(DWRITE_FLOW_DIRECTION flowDirection);
        new HRESULT SetIncrementalTabStop(float incrementalTabStop);
        new HRESULT SetTrimming(DWRITE_TRIMMING trimmingOptions, IDWriteInlineObject trimmingSign);
        new HRESULT SetLineSpacing(DWRITE_LINE_SPACING_METHOD lineSpacingMethod, float lineSpacing, float baseline);
        new DWRITE_TEXT_ALIGNMENT GetTextAlignment();
        new DWRITE_PARAGRAPH_ALIGNMENT GetParagraphAlignment();
        new DWRITE_WORD_WRAPPING GetWordWrapping();
        new DWRITE_READING_DIRECTION GetReadingDirection();
        new DWRITE_FLOW_DIRECTION GetFlowDirection();
        new float GetIncrementalTabStop();
        new HRESULT GetTrimming(out DWRITE_TRIMMING trimmingOptions, out IDWriteInlineObject trimmingSign);
        new HRESULT GetLineSpacing(out DWRITE_LINE_SPACING_METHOD lineSpacingMethod, out float lineSpacing, out float baseline);
        new HRESULT GetFontCollection(out IDWriteFontCollection fontCollection);
        new uint GetFontFamilyNameLength();
        new HRESULT GetFontFamilyName(out string fontFamilyName, uint nameSize);
        new DWRITE_FONT_WEIGHT GetFontWeight();
        new DWRITE_FONT_STYLE GetFontStyle();
        new DWRITE_FONT_STRETCH GetFontStretch();
        new float GetFontSize();
        new uint GetLocaleNameLength();
        new HRESULT GetLocaleName(out string localeName, uint nameSize);

        #endregion

        HRESULT SetMaxWidth(float maxWidth);
        HRESULT SetMaxHeight(float maxHeight);
        HRESULT SetFontCollection(IDWriteFontCollection fontCollection, DWRITE_TEXT_RANGE textRange);
        HRESULT SetFontFamilyName(string fontFamilyName, DWRITE_TEXT_RANGE textRange);
        HRESULT SetFontWeight(DWRITE_FONT_WEIGHT fontWeight, DWRITE_TEXT_RANGE textRange);
        HRESULT SetFontStyle(DWRITE_FONT_STYLE fontStyle, DWRITE_TEXT_RANGE textRange);
        HRESULT SetFontStretch(DWRITE_FONT_STRETCH fontStretch, DWRITE_TEXT_RANGE textRange);
        HRESULT SetFontSize(float fontSize, DWRITE_TEXT_RANGE textRange);
        HRESULT SetUnderline(bool hasUnderline, DWRITE_TEXT_RANGE textRange);
        HRESULT SetStrikethrough(bool hasStrikethrough, DWRITE_TEXT_RANGE textRange);
        //HRESULT SetDrawingEffect(IUnknown drawingEffect, DWRITE_TEXT_RANGE textRange);
        HRESULT SetDrawingEffect(IntPtr drawingEffect, DWRITE_TEXT_RANGE textRange);
        HRESULT SetInlineObject(IDWriteInlineObject inlineObject, DWRITE_TEXT_RANGE textRange);
        HRESULT SetTypography(IDWriteTypography typography, DWRITE_TEXT_RANGE textRange);
        HRESULT SetLocaleName(string localeName, DWRITE_TEXT_RANGE textRange);
        float GetMaxWidth();
        float GetMaxHeight();
        HRESULT GetFontCollection(uint currentPosition, out IDWriteFontCollection fontCollection, out DWRITE_TEXT_RANGE textRange);
        HRESULT GetFontFamilyNameLength(uint currentPosition, out uint nameLength, out DWRITE_TEXT_RANGE textRange);
        HRESULT GetFontFamilyName(uint currentPosition, out string fontFamilyName, uint nameSize, out DWRITE_TEXT_RANGE textRange);
        HRESULT GetFontWeight(uint currentPosition, out DWRITE_FONT_WEIGHT fontWeight, out DWRITE_TEXT_RANGE textRange);
        HRESULT GetFontStyle(uint currentPosition, out DWRITE_FONT_STYLE fontStyle, out DWRITE_TEXT_RANGE textRange);
        HRESULT GetFontStretch(uint currentPosition, out DWRITE_FONT_STRETCH fontStretch, out DWRITE_TEXT_RANGE textRange);
        HRESULT GetFontSize(uint currentPosition, out float fontSize, out DWRITE_TEXT_RANGE textRange);
        HRESULT GetUnderline(uint currentPosition, out bool hasUnderline, out DWRITE_TEXT_RANGE textRange);
        HRESULT GetStrikethrough(uint currentPosition, out bool hasStrikethrough, out DWRITE_TEXT_RANGE textRange);
        //HRESULT GetDrawingEffect(uint currentPosition, out IUnknown drawingEffect, out DWRITE_TEXT_RANGE textRange);
        HRESULT GetDrawingEffect(uint currentPosition, out IntPtr drawingEffect, out DWRITE_TEXT_RANGE textRange);
        HRESULT GetInlineObject(uint currentPosition, out IDWriteInlineObject inlineObject, out DWRITE_TEXT_RANGE textRange);
        HRESULT GetTypography(uint currentPosition, out IDWriteTypography typography, out DWRITE_TEXT_RANGE textRange);
        HRESULT GetLocaleNameLength(uint currentPosition, out uint nameLength, out DWRITE_TEXT_RANGE textRange);
        HRESULT GetLocaleName(uint currentPosition, out string localeName, uint nameSize, out DWRITE_TEXT_RANGE textRange);
        HRESULT Draw(IntPtr clientDrawingContext, IDWriteTextRenderer renderer, float originX, float originY);
        HRESULT GetLineMetrics(out DWRITE_LINE_METRICS lineMetrics, uint maxLineCount, out uint actualLineCount);
        HRESULT GetMetrics(out DWRITE_TEXT_METRICS textMetrics);
        HRESULT GetOverhangMetrics(out DWRITE_OVERHANG_METRICS overhangs);
        HRESULT GetClusterMetrics(out DWRITE_CLUSTER_METRICS clusterMetrics, uint maxClusterCount, out uint actualClusterCount);
        HRESULT DetermineMinWidth(out float minWidth);
        HRESULT HitTestPoint(float pointX, float pointY, out bool isTrailingHit, out bool isInside, out DWRITE_HIT_TEST_METRICS hitTestMetrics);
        HRESULT HitTestTextPosition(uint textPosition, bool isTrailingHit, out float pointX, out float pointY, out DWRITE_HIT_TEST_METRICS hitTestMetrics);
        HRESULT HitTestTextRange(uint textPosition, uint textLength, float originX, float originY, out DWRITE_HIT_TEST_METRICS hitTestMetrics, uint maxHitTestMetricsCount, out uint actualHitTestMetricsCount);
    }

    [ComImport()]
    [Guid("9064D822-80A7-465C-A986-DF65F78B8FEB")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteTextLayout1 : IDWriteTextLayout
    {
        new HRESULT SetTextAlignment(DWRITE_TEXT_ALIGNMENT textAlignment);
        new HRESULT SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT paragraphAlignment);
        new HRESULT SetWordWrapping(DWRITE_WORD_WRAPPING wordWrapping);
        new HRESULT SetReadingDirection(DWRITE_READING_DIRECTION readingDirection);
        new HRESULT SetFlowDirection(DWRITE_FLOW_DIRECTION flowDirection);
        new HRESULT SetIncrementalTabStop(float incrementalTabStop);
        new HRESULT SetTrimming(DWRITE_TRIMMING trimmingOptions, IDWriteInlineObject trimmingSign);
        new HRESULT SetLineSpacing(DWRITE_LINE_SPACING_METHOD lineSpacingMethod, float lineSpacing, float baseline);
        new DWRITE_TEXT_ALIGNMENT GetTextAlignment();
        new DWRITE_PARAGRAPH_ALIGNMENT GetParagraphAlignment();
        new DWRITE_WORD_WRAPPING GetWordWrapping();
        new DWRITE_READING_DIRECTION GetReadingDirection();
        new DWRITE_FLOW_DIRECTION GetFlowDirection();
        new float GetIncrementalTabStop();
        new HRESULT GetTrimming(out DWRITE_TRIMMING trimmingOptions, out IDWriteInlineObject trimmingSign);
        new HRESULT GetLineSpacing(out DWRITE_LINE_SPACING_METHOD lineSpacingMethod, out float lineSpacing, out float baseline);
        new HRESULT GetFontCollection(out IDWriteFontCollection fontCollection);
        new uint GetFontFamilyNameLength();
        new HRESULT GetFontFamilyName(out string fontFamilyName, uint nameSize);
        new DWRITE_FONT_WEIGHT GetFontWeight();
        new DWRITE_FONT_STYLE GetFontStyle();
        new DWRITE_FONT_STRETCH GetFontStretch();
        new float GetFontSize();
        new uint GetLocaleNameLength();
        new HRESULT GetLocaleName(out string localeName, uint nameSize);
        new HRESULT SetMaxWidth(float maxWidth);
        new HRESULT SetMaxHeight(float maxHeight);
        new HRESULT SetFontCollection(IDWriteFontCollection fontCollection, DWRITE_TEXT_RANGE textRange);
        new HRESULT SetFontFamilyName(string fontFamilyName, DWRITE_TEXT_RANGE textRange);
        new HRESULT SetFontWeight(DWRITE_FONT_WEIGHT fontWeight, DWRITE_TEXT_RANGE textRange);
        new HRESULT SetFontStyle(DWRITE_FONT_STYLE fontStyle, DWRITE_TEXT_RANGE textRange);
        new HRESULT SetFontStretch(DWRITE_FONT_STRETCH fontStretch, DWRITE_TEXT_RANGE textRange);
        new HRESULT SetFontSize(float fontSize, DWRITE_TEXT_RANGE textRange);
        new HRESULT SetUnderline(bool hasUnderline, DWRITE_TEXT_RANGE textRange);
        new HRESULT SetStrikethrough(bool hasStrikethrough, DWRITE_TEXT_RANGE textRange);

        new HRESULT SetDrawingEffect(IntPtr drawingEffect, DWRITE_TEXT_RANGE textRange);
        new HRESULT SetInlineObject(IDWriteInlineObject inlineObject, DWRITE_TEXT_RANGE textRange);
        new HRESULT SetTypography(IDWriteTypography typography, DWRITE_TEXT_RANGE textRange);
        new HRESULT SetLocaleName(string localeName, DWRITE_TEXT_RANGE textRange);
        new float GetMaxWidth();
        new float GetMaxHeight();
        new HRESULT GetFontCollection(uint currentPosition, out IDWriteFontCollection fontCollection, out DWRITE_TEXT_RANGE textRange);
        new HRESULT GetFontFamilyNameLength(uint currentPosition, out uint nameLength, out DWRITE_TEXT_RANGE textRange);
        new HRESULT GetFontFamilyName(uint currentPosition, out string fontFamilyName, uint nameSize, out DWRITE_TEXT_RANGE textRange);
        new HRESULT GetFontWeight(uint currentPosition, out DWRITE_FONT_WEIGHT fontWeight, out DWRITE_TEXT_RANGE textRange);
        new HRESULT GetFontStyle(uint currentPosition, out DWRITE_FONT_STYLE fontStyle, out DWRITE_TEXT_RANGE textRange);
        new HRESULT GetFontStretch(uint currentPosition, out DWRITE_FONT_STRETCH fontStretch, out DWRITE_TEXT_RANGE textRange);
        new HRESULT GetFontSize(uint currentPosition, out float fontSize, out DWRITE_TEXT_RANGE textRange);
        new HRESULT GetUnderline(uint currentPosition, out bool hasUnderline, out DWRITE_TEXT_RANGE textRange);
        new HRESULT GetStrikethrough(uint currentPosition, out bool hasStrikethrough, out DWRITE_TEXT_RANGE textRange);

        new HRESULT GetDrawingEffect(uint currentPosition, out IntPtr drawingEffect, out DWRITE_TEXT_RANGE textRange);
        new HRESULT GetInlineObject(uint currentPosition, out IDWriteInlineObject inlineObject, out DWRITE_TEXT_RANGE textRange);
        new HRESULT GetTypography(uint currentPosition, out IDWriteTypography typography, out DWRITE_TEXT_RANGE textRange);
        new HRESULT GetLocaleNameLength(uint currentPosition, out uint nameLength, out DWRITE_TEXT_RANGE textRange);
        new HRESULT GetLocaleName(uint currentPosition, out string localeName, uint nameSize, out DWRITE_TEXT_RANGE textRange);
        new HRESULT Draw(IntPtr clientDrawingContext, IDWriteTextRenderer renderer, float originX, float originY);
        new HRESULT GetLineMetrics(out DWRITE_LINE_METRICS lineMetrics, uint maxLineCount, out uint actualLineCount);
        new HRESULT GetMetrics(out DWRITE_TEXT_METRICS textMetrics);
        new HRESULT GetOverhangMetrics(out DWRITE_OVERHANG_METRICS overhangs);
        new HRESULT GetClusterMetrics(out DWRITE_CLUSTER_METRICS clusterMetrics, uint maxClusterCount, out uint actualClusterCount);
        new HRESULT DetermineMinWidth(out float minWidth);
        new HRESULT HitTestPoint(float pointX, float pointY, out bool isTrailingHit, out bool isInside, out DWRITE_HIT_TEST_METRICS hitTestMetrics);
        new HRESULT HitTestTextPosition(uint textPosition, bool isTrailingHit, out float pointX, out float pointY, out DWRITE_HIT_TEST_METRICS hitTestMetrics);
        new HRESULT HitTestTextRange(uint textPosition, uint textLength, float originX, float originY, out DWRITE_HIT_TEST_METRICS hitTestMetrics, uint maxHitTestMetricsCount, out uint actualHitTestMetricsCount);

        HRESULT SetPairKerning(bool isPairKerningEnabled, DWRITE_TEXT_RANGE textRange);
        HRESULT GetPairKerning(uint currentPosition, ref bool isPairKerningEnabled, ref DWRITE_TEXT_RANGE textRange);
        HRESULT SetCharacterSpacing(float leadingSpacing, float trailingSpacing, float minimumAdvanceWidth, DWRITE_TEXT_RANGE textRange);
        HRESULT GetCharacterSpacing(uint currentPosition, ref float leadingSpacing, out float trailingSpacing, out float minimumAdvanceWidth, out DWRITE_TEXT_RANGE textRange);
    }

    [ComImport]
    [Guid("2cd90698-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1HwndRenderTarget : ID2D1RenderTarget
    {
        #region <ID2D1RenderTarget>

        #region <ID2D1Resource>

        new void GetFactory(out ID2D1Factory factory);

        #endregion
        new void CreateBitmap(D2D1_SIZE_U size, IntPtr srcData, uint pitch, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new HRESULT CreateBitmapFromWicBitmap(IWICBitmapSource wicBitmapSource, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new void CreateSharedBitmap(ref Guid riid, [In, Out] IntPtr data, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new void CreateBitmapBrush(ID2D1Bitmap bitmap, ref D2D1_BITMAP_BRUSH_PROPERTIES bitmapBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1BitmapBrush bitmapBrush);
        new HRESULT CreateSolidColorBrush(D2D1_COLOR_F color, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1SolidColorBrush solidColorBrush);
        //new HRESULT CreateSolidColorBrush(D2D1_COLOR_F color, IntPtr brushProperties, out ID2D1SolidColorBrush solidColorBrush);

        new void CreateGradientStopCollection(D2D1_GRADIENT_STOP gradientStops, uint gradientStopsCount, D2D1_GAMMA colorInterpolationGamma, D2D1_EXTEND_MODE extendMode, out ID2D1GradientStopCollection gradientStopCollection);
        new void CreateLinearGradientBrush(D2D1_LINEAR_GRADIENT_BRUSH_PROPERTIES linearGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1LinearGradientBrush linearGradientBrush);
        new void CreateRadialGradientBrush(D2D1_RADIAL_GRADIENT_BRUSH_PROPERTIES radialGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1RadialGradientBrush radialGradientBrush);
        new void CreateCompatibleRenderTarget(D2D1_SIZE_F desiredSize, D2D1_SIZE_U desiredPixelSize, D2D1_PIXEL_FORMAT desiredFormat, D2D1_COMPATIBLE_RENDER_TARGET_OPTIONS options, out ID2D1BitmapRenderTarget bitmapRenderTarget);
        new void CreateLayer(D2D1_SIZE_F size, out ID2D1Layer layer);
        new void CreateMesh(out ID2D1Mesh mesh);
        new void DrawLine(ref D2D1_POINT_2F point0, ref D2D1_POINT_2F point1, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void DrawRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush);
        new void DrawRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush);
        new void DrawEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush);
        new void DrawGeometry(ID2D1Geometry geometry, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillGeometry(ID2D1Geometry geometry, ID2D1Brush brush, ID2D1Brush opacityBrush = null);
        new void FillMesh(ID2D1Mesh mesh, ID2D1Brush brush);
        new void FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, D2D1_OPACITY_MASK_CONTENT content, D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        new void DrawBitmap(ID2D1Bitmap bitmap, ref D2D1_RECT_F destinationRectangle, float opacity, D2D1_BITMAP_INTERPOLATION_MODE interpolationMode, ref D2D1_RECT_F sourceRectangle);
        new void DrawText(string str, uint stringLength, IDWriteTextFormat textFormat, D2D1_RECT_F layoutRect, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options, DWRITE_MEASURING_MODE measuringMode);
        new void DrawTextLayout(ref D2D1_POINT_2F origin, IDWriteTextLayout textLayout, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options);
        new void DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        new void SetTransform(D2D1_MATRIX_3X2_F transform);
        new void GetTransform(out D2D1_MATRIX_3X2_F transform);
        new void SetAntialiasMode(D2D1_ANTIALIAS_MODE antialiasMode);
        new D2D1_ANTIALIAS_MODE GetAntialiasMode();
        new void SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode);
        new D2D1_TEXT_ANTIALIAS_MODE GetTextAntialiasMode();
        new void SetTextRenderingParams(IDWriteRenderingParams textRenderingParams = null);
        new void GetTextRenderingParams(out IDWriteRenderingParams textRenderingParams);
        new void SetTags(UInt64 tag1, UInt64 tag2);
        new void GetTags(out UInt64 tag1, out UInt64 tag2);
        new void PushLayer(D2D1_LAYER_PARAMETERS layerParameters, ID2D1Layer layer);
        new void PopLayer();
        new void Flush(out UInt64 tag1, out UInt64 tag2);
        new void SaveDrawingState([In, Out] ID2D1DrawingStateBlock drawingStateBlock);
        new void RestoreDrawingState(ID2D1DrawingStateBlock drawingStateBlock);
        new void PushAxisAlignedClip(D2D1_RECT_F clipRect, D2D1_ANTIALIAS_MODE antialiasMode);
        new void PopAxisAlignedClip();
        new void Clear(D2D1_COLOR_F clearColor);
        new void BeginDraw();
        new HRESULT EndDraw(out UInt64 tag1, out UInt64 tag2);
        new D2D1_PIXEL_FORMAT GetPixelFormat();
        new void SetDpi(float dpiX, float dpiY);
        new void GetDpi(out float dpiX, out float dpiY);
        new D2D1_SIZE_F GetSize();
        new D2D1_SIZE_U GetPixelSize();
        [PreserveSig]
        new uint GetMaximumBitmapSize();
        new bool IsSupported(D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties);

        //new HRESULT Function1();
        //new HRESULT Function2();
        //new HRESULT Function3();
        //new HRESULT Function4();
        //new HRESULT Function5();
        //new HRESULT Function6();
        //new HRESULT Function7();
        //new HRESULT Function8();
        //new HRESULT Function9();
        //new HRESULT Function10();
        //new HRESULT Function11();
        //new HRESULT Function12();
        //new HRESULT Function13();
        //new HRESULT Function14();
        //new HRESULT Function15();
        //new HRESULT Function16();
        //new HRESULT Function17();
        //new HRESULT Function18();
        //new HRESULT Function19();
        //new HRESULT Function20();
        //new HRESULT Function21();
        //new void Function22();
        //new void Function23();
        //new void Function24();
        //new void Function25();
        //new void Function26();
        //new void Function27();
        //new void Function28();
        //new void Function29();
        //new void Function30();
        //new void Function31();
        //new void Function32();
        //new void Function33();
        //new void Function34();
        //new void Function35();
        //new bool Function36();

        #endregion

        D2D1_WINDOW_STATE CheckWindowState();
        HRESULT Resize(ref D2D1_SIZE_U pixelSize);

        //[return: MarshalAs(UnmanagedType.SysInt)]
        [PreserveSig]
        IntPtr GetHwnd();
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_SIZE_U
    {
        public uint width;
        public uint height;
        public D2D1_SIZE_U(uint width, uint height)
        {
            this.width = width;
            this.height = height;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_VECTOR_2F
    {
        public float x;
        public float y;
        public D2D1_VECTOR_2F(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_VECTOR_3F
    {
        public float x;
        public float y;
        public float z;
        public D2D1_VECTOR_3F(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_VECTOR_4F
    {
        public float x;
        public float y;
        public float z;
        public float w;
        public D2D1_VECTOR_4F(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }



    //D2D1_SIZE_U SizeU(UInt32 width = 0, UInt32 height = 0)
    //{
    //    return Size<UInt32>(width, height);
    //}

    //  public D2D1_RENDER_TARGET_PROPERTIES RenderTargetProperties(
    //D2D1_RENDER_TARGET_TYPE type = D2D1_RENDER_TARGET_TYPE.D2D1_RENDER_TARGET_TYPE_DEFAULT,
    //D2D1_PIXEL_FORMAT pixelFormat = PixelFormat(),
    //float dpiX = 0.0f,
    //float dpiY = 0.0f,
    //D2D1_RENDER_TARGET_USAGE usage = D2D1_RENDER_TARGET_USAGE.D2D1_RENDER_TARGET_USAGE_NONE,
    //D2D1_FEATURE_LEVEL minLevel = D2D1_FEATURE_LEVEL.D2D1_FEATURE_LEVEL_DEFAULT
    //)
    //  {
    //      D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties;

    //      renderTargetProperties.type = type;
    //      renderTargetProperties.pixelFormat = pixelFormat;
    //      renderTargetProperties.dpiX = dpiX;
    //      renderTargetProperties.dpiY = dpiY;
    //      renderTargetProperties.usage = usage;
    //      renderTargetProperties.minLevel = minLevel;

    //      return renderTargetProperties;
    //  }

    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_RECT_F
    {
        public float left;
        public float top;
        public float right;
        public float bottom;
        public D2D1_RECT_F(float left, float top, float right, float bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }
    }

    [ComImport]
    [Guid("2cd906a2-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1RectangleGeometry
    //ID2D1RectangleGeometry  : public ID2D1Geometry
    {
        #region <ID2D1Geometry>

        // A faire

        #endregion
        void GetRect(out D2D1_RECT_F rect);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_ROUNDED_RECT
    {
        public D2D1_RECT_F rect;
        public float radiusX;
        public float radiusY;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_POINT_2F
    {
        public float x;
        public float y;

        public D2D1_POINT_2F(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_ELLIPSE
    {
        public D2D1_POINT_2F point;
        public float radiusX;
        public float radiusY;
    }

    public enum D2D1_FILL_MODE
    {
        D2D1_FILL_MODE_ALTERNATE = 0,
        D2D1_FILL_MODE_WINDING = 1,
        D2D1_FILL_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Explicit, Size = 24)]
    public class D2D1_MATRIX_3X2_F
    {
        [FieldOffset(0)]
        public float _11;
        [FieldOffset(4)]
        public float _12;
        [FieldOffset(8)]
        public float _21;
        [FieldOffset(12)]
        public float _22;
        [FieldOffset(16)]
        public float _31;
        [FieldOffset(20)]
        public float _32;
    }

    [StructLayout(LayoutKind.Explicit, Size = 24)]
    public struct D2D1_MATRIX_3X2_F_STRUCT
    {
        [FieldOffset(0)]
        public float _11;
        [FieldOffset(4)]
        public float _12;
        [FieldOffset(8)]
        public float _21;
        [FieldOffset(12)]
        public float _22;
        [FieldOffset(16)]
        public float _31;
        [FieldOffset(20)]
        public float _32;
    }       

    [StructLayout(LayoutKind.Explicit, Size = 64)]
    public class D2D1_MATRIX_4X4_F
    {
        [FieldOffset(0)]
        public float _11;
        [FieldOffset(4)]
        public float _12;
        [FieldOffset(8)]
        public float _13;
        [FieldOffset(12)]
        public float _14;

        [FieldOffset(16)]
        public float _21;
        [FieldOffset(20)]
        public float _22;
        [FieldOffset(24)]
        public float _23;
        [FieldOffset(28)]
        public float _24;

        [FieldOffset(32)]
        public float _31;
        [FieldOffset(36)]
        public float _32;
        [FieldOffset(40)]
        public float _33;
        [FieldOffset(44)]
        public float _34;

        [FieldOffset(48)]
        public float _41;
        [FieldOffset(52)]
        public float _42;
        [FieldOffset(56)]
        public float _43;
        [FieldOffset(60)]
        public float _44;
    }

    public enum D2D1_DASH_STYLE
    {
        D2D1_DASH_STYLE_SOLID = 0,
        D2D1_DASH_STYLE_DASH = 1,
        D2D1_DASH_STYLE_DOT = 2,
        D2D1_DASH_STYLE_DASH_DOT = 3,
        D2D1_DASH_STYLE_DASH_DOT_DOT = 4,
        D2D1_DASH_STYLE_CUSTOM = 5,
        D2D1_DASH_STYLE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_LINE_JOIN
    {
        //
        // Miter join.
        //
        D2D1_LINE_JOIN_MITER = 0,
        //
        // Bevel join.
        //
        D2D1_LINE_JOIN_BEVEL = 1,
        //
        // Round join.
        //
        D2D1_LINE_JOIN_ROUND = 2,
        //
        // Miter/Bevel join.
        //
        D2D1_LINE_JOIN_MITER_OR_BEVEL = 3,
        D2D1_LINE_JOIN_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    public class D2D1_STROKE_STYLE_PROPERTIES
    {
        public D2D1_CAP_STYLE startCap;
        public D2D1_CAP_STYLE endCap;
        public D2D1_CAP_STYLE dashCap;
        public D2D1_LINE_JOIN lineJoin;
        public float miterLimit;
        public D2D1_DASH_STYLE dashStyle;
        public float dashOffset;
        public D2D1_STROKE_STYLE_PROPERTIES(D2D1_CAP_STYLE startCap, D2D1_CAP_STYLE endCap, D2D1_CAP_STYLE dashCap, D2D1_LINE_JOIN lineJoin, float miterLimit, D2D1_DASH_STYLE dashStyle, float dashOffset)
        {
            this.startCap = startCap;
            this.endCap = endCap;
            this.dashCap = dashCap;
            this.lineJoin = lineJoin;
            this.miterLimit = miterLimit;
            this.dashStyle = dashStyle;
            this.dashOffset = dashOffset;
        }
    }

    public enum D2D1_ANTIALIAS_MODE
    {
        //
        // The edges of each primitive are antialiased sequentially.
        //
        D2D1_ANTIALIAS_MODE_PER_PRIMITIVE = 0,

        //
        // Each pixel is rendered if its pixel center is contained by the geometry.
        //
        D2D1_ANTIALIAS_MODE_ALIASED = 1,
        D2D1_ANTIALIAS_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }
    public enum D2D1_TEXT_ANTIALIAS_MODE
    {
        //
        // Render text using the current system setting.
        //
        D2D1_TEXT_ANTIALIAS_MODE_DEFAULT = 0,
        //
        // Render text using ClearType.
        //
        D2D1_TEXT_ANTIALIAS_MODE_CLEARTYPE = 1,
        //
        // Render text using gray-scale.
        //
        D2D1_TEXT_ANTIALIAS_MODE_GRAYSCALE = 2,
        //
        // Render text aliased.
        //
        D2D1_TEXT_ANTIALIAS_MODE_ALIASED = 3,
        D2D1_TEXT_ANTIALIAS_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_DRAWING_STATE_DESCRIPTION
    {
        public D2D1_ANTIALIAS_MODE antialiasMode;
        public D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode;
        public UInt64 tag1;
        public UInt64 tag2;
        public D2D1_MATRIX_3X2_F transform;
    }

    public enum D2D1_PRESENT_OPTIONS
    {
        D2D1_PRESENT_OPTIONS_NONE = 0x00000000,
        //
        // Keep the target contents intact through present.
        //
        D2D1_PRESENT_OPTIONS_RETAIN_CONTENTS = 0x00000001,
        //
        // Do not wait for display refresh to commit changes to display.
        //
        D2D1_PRESENT_OPTIONS_IMMEDIATELY = 0x00000002,
        D2D1_PRESENT_OPTIONS_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_HWND_RENDER_TARGET_PROPERTIES
    {
        public IntPtr hwnd;
        public D2D1_SIZE_U pixelSize;
        public D2D1_PRESENT_OPTIONS presentOptions;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_BITMAP_PROPERTIES
    {
        public D2D1_PIXEL_FORMAT pixelFormat;
        public float dpiX;
        public float dpiY;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_SIZE_F
    {
        public float width;
        public float height;

        public D2D1_SIZE_F(float width, float height)
        {
            this.width = width;
            this.height = height;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_POINT_2U
    {
        public uint x;
        public uint y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_RECT_U
    {
        public uint left;
        public uint top;
        public uint right;
        public uint bottom;
        public D2D1_RECT_U(uint left, uint top, uint right, uint bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }
    }

    [ComImport]
    [Guid("65019f75-8da2-497c-b32c-dfa34e48ede6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Image : ID2D1Resource
    {
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);

        #endregion
    }

    [ComImport]
    [Guid("a2296057-ea42-4099-983b-539fb6505426")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Bitmap : ID2D1Image
    {
        #region <ID2D1Image>

        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);

        #endregion

        #endregion

        D2D1_SIZE_F GetSize();
        D2D1_SIZE_U GetPixelSize();
        D2D1_PIXEL_FORMAT GetPixelFormat();
        void GetDpi(out float dpiX, out float dpiY);
        void CopyFromBitmap(D2D1_POINT_2U destPoint, ID2D1Bitmap bitmap, D2D1_RECT_U srcRect);
        void CopyFromRenderTarget(D2D1_POINT_2U destPoint, ID2D1RenderTarget renderTarget, D2D1_RECT_U srcRect);
        void CopyFromMemory(D2D1_RECT_U dstRect, IntPtr srcData, uint pitch);
    }

    public enum D2D1_EXTEND_MODE
    {
        //
        // Extend the edges of the source out by clamping sample points outside the source
        // to the edges.
        //
        D2D1_EXTEND_MODE_CLAMP = 0,
        //
        // The base tile is drawn untransformed and the remainder are filled by repeating
        // the base tile.
        //
        D2D1_EXTEND_MODE_WRAP = 1,
        //
        // The same as wrap, but alternate tiles are flipped  The base tile is drawn
        // untransformed.
        //
        D2D1_EXTEND_MODE_MIRROR = 2,
        D2D1_EXTEND_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_INTERPOLATION_MODE_DEFINITION
    {
        D2D1_INTERPOLATION_MODE_DEFINITION_NEAREST_NEIGHBOR = 0,
        D2D1_INTERPOLATION_MODE_DEFINITION_LINEAR = 1,
        D2D1_INTERPOLATION_MODE_DEFINITION_CUBIC = 2,
        D2D1_INTERPOLATION_MODE_DEFINITION_MULTI_SAMPLE_LINEAR = 3,
        D2D1_INTERPOLATION_MODE_DEFINITION_ANISOTROPIC = 4,
        D2D1_INTERPOLATION_MODE_DEFINITION_HIGH_QUALITY_CUBIC = 5,
        D2D1_INTERPOLATION_MODE_DEFINITION_FANT = 6,
        D2D1_INTERPOLATION_MODE_DEFINITION_MIPMAP_LINEAR = 7

    }

    public enum D2D1_BITMAP_INTERPOLATION_MODE
    {
        //
        // Nearest Neighbor filtering. Also known as nearest pixel or nearest point
        // sampling.
        //
        D2D1_BITMAP_INTERPOLATION_MODE_NEAREST_NEIGHBOR = D2D1_INTERPOLATION_MODE_DEFINITION.D2D1_INTERPOLATION_MODE_DEFINITION_NEAREST_NEIGHBOR,
        //
        // Linear filtering.
        //
        D2D1_BITMAP_INTERPOLATION_MODE_LINEAR = D2D1_INTERPOLATION_MODE_DEFINITION.D2D1_INTERPOLATION_MODE_DEFINITION_LINEAR,
        D2D1_BITMAP_INTERPOLATION_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_BITMAP_BRUSH_PROPERTIES
    {
        public D2D1_EXTEND_MODE extendModeX;
        public D2D1_EXTEND_MODE extendModeY;
        public D2D1_BITMAP_INTERPOLATION_MODE interpolationMode;
    }

    //[StructLayout(LayoutKind.Sequential)]
    //public struct D2D1_BRUSH_PROPERTIES
    //{
    //    public float opacity;
    //    public D2D1_MATRIX_3X2_F transform;
    //}

    public class D2D1_BRUSH_PROPERTIES
    {
        public float opacity;
        public D2D1_MATRIX_3X2_F transform;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class D2D1_COLOR_F
    {
        public float r;
        public float g;
        public float b;
        public float a;
    }
    

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_COLOR_F_STRUCT
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public D2D1_COLOR_F_STRUCT(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }

    //[StructLayout(LayoutKind.Sequential)]
    //public struct D2D1_COLOR_F_STRUCT
    //{
    //    public float r;
    //    public float g;
    //    public float b;
    //    public float a;

    //    public D2D1_COLOR_F_STRUCT(float r, float g, float b, float a)
    //    {
    //        this.r = r;
    //        this.g = g;
    //        this.b = b;       
    //        this.a = a;
    //    }

    //    //public static implicit operator ColorF(D2D1_COLOR_F_STRUCT d)
    //    //{
    //    //    return d.r;
    //    //}

    //    //public static explicit operator D2D1_COLOR_F_STRUCT(ColorF c)
    //    //{
    //    //    return new D2D1_COLOR_F_STRUCT(c.r, c.g, c.b, c.a);
    //    //}

    //    //public static implicit operator D2D1_COLOR_F_STRUCT(ColorF c)
    //    //{

    //    //   return new D2D1_COLOR_F_STRUCT(c.r, c.g, c.b, c.a);
    //    //}
    //}

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_GRADIENT_STOP
    {
        public float position;
        public D2D1_COLOR_F color;
    }

    public enum D2D1_GAMMA
    {
        //
        // Colors are manipulated in 2.2 gamma color space.
        //
        D2D1_GAMMA_2_2 = 0,
        //
        // Colors are manipulated in 1.0 gamma color space.
        //
        D2D1_GAMMA_1_0 = 1,
        D2D1_GAMMA_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_LINEAR_GRADIENT_BRUSH_PROPERTIES
    {
        public D2D1_POINT_2F startPoint;
        public D2D1_POINT_2F endPoint;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_RADIAL_GRADIENT_BRUSH_PROPERTIES
    {
        public D2D1_POINT_2F center;
        public D2D1_POINT_2F gradientOriginOffset;
        public float radiusX;
        public float radiusY;
    }

    public enum D2D1_COMPATIBLE_RENDER_TARGET_OPTIONS
    {
        D2D1_COMPATIBLE_RENDER_TARGET_OPTIONS_NONE = 0x00000000,
        //
        // The compatible render target will allow a call to GetDC on the
        // ID2D1GdiInteropRenderTarget interface. This can be specified even if the parent
        // render target is not GDI compatible.
        //
        D2D1_COMPATIBLE_RENDER_TARGET_OPTIONS_GDI_COMPATIBLE = 0x00000001,
        D2D1_COMPATIBLE_RENDER_TARGET_OPTIONS_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_OPACITY_MASK_CONTENT
    {
        //
        // The mask contains geometries or bitmaps.
        //
        D2D1_OPACITY_MASK_CONTENT_GRAPHICS = 0,
        //
        // The mask contains text rendered using one of the natural text modes.
        //
        D2D1_OPACITY_MASK_CONTENT_TEXT_NATURAL = 1,
        //
        // The mask contains text rendered using one of the GDI compatible text modes.
        //
        D2D1_OPACITY_MASK_CONTENT_TEXT_GDI_COMPATIBLE = 2,
        D2D1_OPACITY_MASK_CONTENT_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_DRAW_TEXT_OPTIONS
    {
        //
        // Do not snap the baseline of the text vertically.
        //
        D2D1_DRAW_TEXT_OPTIONS_NO_SNAP = 0x00000001,
        //
        // Clip the text to the content bounds.
        //
        D2D1_DRAW_TEXT_OPTIONS_CLIP = 0x00000002,
        //
        // Render color versions of glyphs if defined by the font.
        //
        D2D1_DRAW_TEXT_OPTIONS_ENABLE_COLOR_FONT = 0x00000004,
        D2D1_DRAW_TEXT_OPTIONS_NONE = 0x00000000,
        D2D1_DRAW_TEXT_OPTIONS_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum DWRITE_MEASURING_MODE
    {
        //
        // Text is measured using glyph ideal metrics whose values are independent to the current display resolution.
        //
        DWRITE_MEASURING_MODE_NATURAL,
        //
        // Text is measured using glyph display compatible metrics whose values tuned for the current display resolution.
        //
        DWRITE_MEASURING_MODE_GDI_CLASSIC,
        //
        // Text is measured using the same glyph display metrics as text measured by GDI using a font
        // created with CLEARTYPE_NATURAL_QUALITY.
        //
        DWRITE_MEASURING_MODE_GDI_NATURAL
    }

    public enum D2D1_LAYER_OPTIONS
    {
        D2D1_LAYER_OPTIONS_NONE = 0x00000000,
        //
        // The layer will render correctly for ClearType text. If the render target was set
        // to ClearType previously, the layer will continue to render ClearType. If the
        // render target was set to ClearType and this option is not specified, the render
        // target will be set to render gray-scale until the layer is popped. The caller
        // can override this default by calling SetTextAntialiasMode while within the
        // layer. This flag is slightly slower than the default.
        //
        D2D1_LAYER_OPTIONS_INITIALIZE_FOR_CLEARTYPE = 0x00000001,
        D2D1_LAYER_OPTIONS_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_LAYER_PARAMETERS
    {
        public D2D1_RECT_F contentBounds;
        //_Field_size_opt_(1) ID2D1Geometry* geometricMask;
        public IntPtr geometricMask;
        public D2D1_ANTIALIAS_MODE maskAntialiasMode;
        public D2D1_MATRIX_3X2_F maskTransform;
        public float opacity;
        //_Field_size_opt_(1) ID2D1Brush* opacityBrush;
        public IntPtr opacityBrush;
        public D2D1_LAYER_OPTIONS layerOptions;
    }

    [ComImport]
    [Guid("2cd90691-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Resource
    {
        void GetFactory(out ID2D1Factory factory);
    }

    [ComImport]
    [Guid("2cd906a9-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1SolidColorBrush : ID2D1Brush
    {
        #region ID2D1Brush
        #region ID2D1Resource
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        new void SetOpacity(float opacity);
        new void SetTransform(D2D1_MATRIX_3X2_F transform);
        new float GetOpacity();
        new void GetTransform(out D2D1_MATRIX_3X2_F transform);
        #endregion

        void SetColor(D2D1_COLOR_F color);
        D2D1_COLOR_F GetColor();
        //void SetColor(D2D1_COLOR_F &color)
        //{
        //    SetColor(&color);
        //}
    }

    [ComImport]
    [Guid("2cd906a7-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1GradientStopCollection : ID2D1Resource
    {
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);

        #endregion
        uint GetGradientStopCount();
        void GetGradientStops(out D2D1_GRADIENT_STOP gradientStops, uint gradientStopsCount);
        D2D1_GAMMA GetColorInterpolationGamma();
        D2D1_EXTEND_MODE GetExtendMode();
    }

    public enum DWRITE_PIXEL_GEOMETRY
    {
        /// <summary>
        /// The red, green, and blue color components of each pixel are assumed to occupy the same point.
        /// </summary>
        DWRITE_PIXEL_GEOMETRY_FLAT,
        /// <summary>
        /// Each pixel comprises three vertical stripes, with red on the left, green in the center, and 
        /// blue on the right. This is the most common pixel geometry for LCD monitors.
        /// </summary>
        DWRITE_PIXEL_GEOMETRY_RGB,
        /// <summary>
        /// Each pixel comprises three vertical stripes, with blue on the left, green in the center, and 
        /// red on the right.
        /// </summary>
        DWRITE_PIXEL_GEOMETRY_BGR
    };

    public enum DWRITE_RENDERING_MODE
    {
        /// <summary>
        /// Specifies that the rendering mode is determined automatically based on the font and size.
        /// </summary>
        DWRITE_RENDERING_MODE_DEFAULT,
        /// <summary>
        /// Specifies that no antialiasing is performed. Each pixel is either set to the foreground 
        /// color of the text or retains the color of the background.
        /// </summary>
        DWRITE_RENDERING_MODE_ALIASED,
        /// <summary>
        /// Specifies that antialiasing is performed in the horizontal direction and the appearance
        /// of glyphs is layout-compatible with GDI using CLEARTYPE_QUALITY. Use DWRITE_MEASURING_MODE_GDI_CLASSIC 
        /// to get glyph advances. The antialiasing may be either ClearType or grayscale depending on
        /// the text antialiasing mode.
        /// </summary>
        DWRITE_RENDERING_MODE_GDI_CLASSIC,
        /// <summary>
        /// Specifies that antialiasing is performed in the horizontal direction and the appearance
        /// of glyphs is layout-compatible with GDI using CLEARTYPE_NATURAL_QUALITY. Glyph advances
        /// are close to the font design advances, but are still rounded to whole pixels. Use
        /// DWRITE_MEASURING_MODE_GDI_NATURAL to get glyph advances. The antialiasing may be either
        /// ClearType or grayscale depending on the text antialiasing mode.
        /// </summary>
        DWRITE_RENDERING_MODE_GDI_NATURAL,
        /// <summary>
        /// Specifies that antialiasing is performed in the horizontal direction. This rendering
        /// mode allows glyphs to be positioned with subpixel precision and is therefore suitable
        /// for natural (i.e., resolution-independent) layout. The antialiasing may be either
        /// ClearType or grayscale depending on the text antialiasing mode.
        /// </summary>
        DWRITE_RENDERING_MODE_NATURAL,
        /// <summary>
        /// Similar to natural mode except that antialiasing is performed in both the horizontal
        /// and vertical directions. This is typically used at larger sizes to make curves and
        /// diagonal lines look smoother. The antialiasing may be either ClearType or grayscale
        /// depending on the text antialiasing mode.
        /// </summary>
        DWRITE_RENDERING_MODE_NATURAL_SYMMETRIC,
        /// <summary>
        /// Specifies that rendering should bypass the rasterizer and use the outlines directly. 
        /// This is typically used at very large sizes.
        /// </summary>
        DWRITE_RENDERING_MODE_OUTLINE,
        // The following names are obsolete, but are kept as aliases to avoid breaking existing code.
        // Each of these rendering modes may result in either ClearType or grayscale antialiasing 
        // depending on the DWRITE_TEXT_ANTIALIASING_MODE.
        DWRITE_RENDERING_MODE_CLEARTYPE_GDI_CLASSIC = DWRITE_RENDERING_MODE_GDI_CLASSIC,
        DWRITE_RENDERING_MODE_CLEARTYPE_GDI_NATURAL = DWRITE_RENDERING_MODE_GDI_NATURAL,
        DWRITE_RENDERING_MODE_CLEARTYPE_NATURAL = DWRITE_RENDERING_MODE_NATURAL,
        DWRITE_RENDERING_MODE_CLEARTYPE_NATURAL_SYMMETRIC = DWRITE_RENDERING_MODE_NATURAL_SYMMETRIC
    };

    [ComImport]
    [Guid("2f0da53a-2add-47cd-82ee-d9ec34688e75")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteRenderingParams
    {
        float GetGamma();
        float GetEnhancedContrast();
        float GetClearTypeLevel();
        DWRITE_PIXEL_GEOMETRY GetPixelGeometry();
        DWRITE_RENDERING_MODE GetRenderingMode();
    }

    [ComImport]
    [Guid("2cd90695-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1BitmapRenderTarget : ID2D1RenderTarget
    {
        #region <ID2D1RenderTarget>

        #region <ID2D1Resource>

        new void GetFactory(out ID2D1Factory factory);

        #endregion
        new void CreateBitmap(D2D1_SIZE_U size, IntPtr srcData, uint pitch, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new HRESULT CreateBitmapFromWicBitmap(IWICBitmapSource wicBitmapSource, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new void CreateSharedBitmap(ref Guid riid, [In, Out] IntPtr data, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new void CreateBitmapBrush(ID2D1Bitmap bitmap, ref D2D1_BITMAP_BRUSH_PROPERTIES bitmapBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1BitmapBrush bitmapBrush);
        new HRESULT CreateSolidColorBrush(D2D1_COLOR_F color, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1SolidColorBrush solidColorBrush);
        new void CreateGradientStopCollection(D2D1_GRADIENT_STOP gradientStops, uint gradientStopsCount, D2D1_GAMMA colorInterpolationGamma, D2D1_EXTEND_MODE extendMode, out ID2D1GradientStopCollection gradientStopCollection);
        new void CreateLinearGradientBrush(D2D1_LINEAR_GRADIENT_BRUSH_PROPERTIES linearGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1LinearGradientBrush linearGradientBrush);
        new void CreateRadialGradientBrush(D2D1_RADIAL_GRADIENT_BRUSH_PROPERTIES radialGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1RadialGradientBrush radialGradientBrush);
        new void CreateCompatibleRenderTarget(D2D1_SIZE_F desiredSize, D2D1_SIZE_U desiredPixelSize, D2D1_PIXEL_FORMAT desiredFormat, D2D1_COMPATIBLE_RENDER_TARGET_OPTIONS options, out ID2D1BitmapRenderTarget bitmapRenderTarget);
        new void CreateLayer(D2D1_SIZE_F size, out ID2D1Layer layer);
        new void CreateMesh(out ID2D1Mesh mesh);
        new void DrawLine(ref D2D1_POINT_2F point0, ref D2D1_POINT_2F point1, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void DrawRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush);
        new void DrawRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush);
        new void DrawEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush);
        new void DrawGeometry(ID2D1Geometry geometry, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillGeometry(ID2D1Geometry geometry, ID2D1Brush brush, ID2D1Brush opacityBrush = null);
        new void FillMesh(ID2D1Mesh mesh, ID2D1Brush brush);
        new void FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, D2D1_OPACITY_MASK_CONTENT content, D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        new void DrawBitmap(ID2D1Bitmap bitmap, ref D2D1_RECT_F destinationRectangle, float opacity, D2D1_BITMAP_INTERPOLATION_MODE interpolationMode, ref D2D1_RECT_F sourceRectangle);
        new void DrawText(string str, uint stringLength, IDWriteTextFormat textFormat, D2D1_RECT_F layoutRect, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options, DWRITE_MEASURING_MODE measuringMode);
        new void DrawTextLayout(ref D2D1_POINT_2F origin, IDWriteTextLayout textLayout, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options);
        new void DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        new void SetTransform(D2D1_MATRIX_3X2_F transform);
        new void GetTransform(out D2D1_MATRIX_3X2_F transform);
        new void SetAntialiasMode(D2D1_ANTIALIAS_MODE antialiasMode);
        new D2D1_ANTIALIAS_MODE GetAntialiasMode();
        new void SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode);
        new D2D1_TEXT_ANTIALIAS_MODE GetTextAntialiasMode();
        new void SetTextRenderingParams(IDWriteRenderingParams textRenderingParams = null);
        new void GetTextRenderingParams(out IDWriteRenderingParams textRenderingParams);
        new void SetTags(UInt64 tag1, UInt64 tag2);
        new void GetTags(out UInt64 tag1, out UInt64 tag2);
        new void PushLayer(D2D1_LAYER_PARAMETERS layerParameters, ID2D1Layer layer);
        new void PopLayer();
        new void Flush(out UInt64 tag1, out UInt64 tag2);
        new void SaveDrawingState([In, Out] ID2D1DrawingStateBlock drawingStateBlock);
        new void RestoreDrawingState(ID2D1DrawingStateBlock drawingStateBlock);
        new void PushAxisAlignedClip(D2D1_RECT_F clipRect, D2D1_ANTIALIAS_MODE antialiasMode);
        new void PopAxisAlignedClip();
        new void Clear(D2D1_COLOR_F clearColor);
        new void BeginDraw();
        new HRESULT EndDraw(out UInt64 tag1, out UInt64 tag2);
        new D2D1_PIXEL_FORMAT GetPixelFormat();
        new void SetDpi(float dpiX, float dpiY);
        new void GetDpi(out float dpiX, out float dpiY);
        new D2D1_SIZE_F GetSize();
        new D2D1_SIZE_U GetPixelSize();
        new uint GetMaximumBitmapSize();
        new bool IsSupported(D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties);
        #endregion

        void GetBitmap(out ID2D1Bitmap bitmap);
    }

    [ComImport]
    [Guid("2cd9069b-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Layer : ID2D1Resource
    {
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion
        D2D1_SIZE_F GetSize();
    }

    [ComImport]
    [Guid("2cd906c2-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Mesh : ID2D1Resource
    {
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion
        //void Open(out ID2D1TessellationSink tessellationSink);
        void Open(out IntPtr tessellationSink);
    }

    [ComImport]
    [Guid("2cd90694-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1RenderTarget : ID2D1Resource
    {
        #region <ID2D1Resource>

        new void GetFactory(out ID2D1Factory factory);

        #endregion
        void CreateBitmap(D2D1_SIZE_U size, IntPtr srcData, uint pitch, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        HRESULT CreateBitmapFromWicBitmap(IWICBitmapSource wicBitmapSource, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        void CreateSharedBitmap(ref Guid riid, [In, Out] IntPtr data, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        void CreateBitmapBrush(ID2D1Bitmap bitmap, ref D2D1_BITMAP_BRUSH_PROPERTIES bitmapBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1BitmapBrush bitmapBrush);
        HRESULT CreateSolidColorBrush(D2D1_COLOR_F color, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1SolidColorBrush solidColorBrush);
        void CreateGradientStopCollection(D2D1_GRADIENT_STOP gradientStops, uint gradientStopsCount, D2D1_GAMMA colorInterpolationGamma, D2D1_EXTEND_MODE extendMode, out ID2D1GradientStopCollection gradientStopCollection);
        void CreateLinearGradientBrush(D2D1_LINEAR_GRADIENT_BRUSH_PROPERTIES linearGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1LinearGradientBrush linearGradientBrush);
        void CreateRadialGradientBrush(D2D1_RADIAL_GRADIENT_BRUSH_PROPERTIES radialGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1RadialGradientBrush radialGradientBrush);
        void CreateCompatibleRenderTarget(D2D1_SIZE_F desiredSize, D2D1_SIZE_U desiredPixelSize, D2D1_PIXEL_FORMAT desiredFormat, D2D1_COMPATIBLE_RENDER_TARGET_OPTIONS options, out ID2D1BitmapRenderTarget bitmapRenderTarget);
        void CreateLayer(D2D1_SIZE_F size, out ID2D1Layer layer);
        void CreateMesh(out ID2D1Mesh mesh);
        void DrawLine(ref D2D1_POINT_2F point0, ref D2D1_POINT_2F point1, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        void DrawRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        void FillRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush);
        void DrawRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        void FillRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush);
        void DrawEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        void FillEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush);
        void DrawGeometry(ID2D1Geometry geometry, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        void FillGeometry(ID2D1Geometry geometry, ID2D1Brush brush, ID2D1Brush opacityBrush = null);
        void FillMesh(ID2D1Mesh mesh, ID2D1Brush brush);
        void FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, D2D1_OPACITY_MASK_CONTENT content, D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        void DrawBitmap(ID2D1Bitmap bitmap, ref D2D1_RECT_F destinationRectangle, float opacity, D2D1_BITMAP_INTERPOLATION_MODE interpolationMode, ref D2D1_RECT_F sourceRectangle);
        void DrawText(string str, uint stringLength, IDWriteTextFormat textFormat, D2D1_RECT_F layoutRect, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options, DWRITE_MEASURING_MODE measuringMode);
        void DrawTextLayout(ref D2D1_POINT_2F origin, IDWriteTextLayout textLayout, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options);
        void DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        void SetTransform(D2D1_MATRIX_3X2_F transform);
        void GetTransform(out D2D1_MATRIX_3X2_F transform);
        void SetAntialiasMode(D2D1_ANTIALIAS_MODE antialiasMode);
        D2D1_ANTIALIAS_MODE GetAntialiasMode();
        void SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode);
        D2D1_TEXT_ANTIALIAS_MODE GetTextAntialiasMode();
        void SetTextRenderingParams(IDWriteRenderingParams textRenderingParams = null);
        void GetTextRenderingParams(out IDWriteRenderingParams textRenderingParams);
        void SetTags(UInt64 tag1, UInt64 tag2);
        void GetTags(out UInt64 tag1, out UInt64 tag2);
        void PushLayer(D2D1_LAYER_PARAMETERS layerParameters, ID2D1Layer layer);
        void PopLayer();
        void Flush(out UInt64 tag1, out UInt64 tag2);
        void SaveDrawingState([In, Out] ID2D1DrawingStateBlock drawingStateBlock);
        void RestoreDrawingState(ID2D1DrawingStateBlock drawingStateBlock);
        void PushAxisAlignedClip(D2D1_RECT_F clipRect, D2D1_ANTIALIAS_MODE antialiasMode);
        void PopAxisAlignedClip();
        void Clear(D2D1_COLOR_F clearColor);
        void BeginDraw();
        void EndDraw(out UInt64 tag1, out UInt64 tag2);
        D2D1_PIXEL_FORMAT GetPixelFormat();
        void SetDpi(float dpiX, float dpiY);
        void GetDpi(out float dpiX, out float dpiY);
        D2D1_SIZE_F GetSize();
        D2D1_SIZE_U GetPixelSize();
        uint GetMaximumBitmapSize();
        bool IsSupported(D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties);

        HRESULT Function1();
        HRESULT Function2();
        HRESULT Function3();
        HRESULT Function4();
        HRESULT Function5();
        HRESULT Function6();
        HRESULT Function7();
        HRESULT Function8();
        HRESULT Function9();
        HRESULT Function10();
        HRESULT Function11();
        HRESULT Function12();
        HRESULT Function13();
        HRESULT Function14();
        HRESULT Function15();
        HRESULT Function16();
        HRESULT Function17();
        HRESULT Function18();
        HRESULT Function19();
        HRESULT Function20();
        HRESULT Function21();
        void Function22();
        void Function23();
        void Function24();
        void Function25();
        void Function26();
        void Function27();
        void Function28();
        void Function29();
        void Function30();
        void Function31();
        void Function32();
        void Function33();
        void Function34();
        void Function35();
        bool Function36();
    }

    // Incomplete
    [ComImport]
    [Guid("06152247-6f50-465a-9245-118bfd3b6007")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Factory
    {
        HRESULT ReloadSystemMetrics();
        HRESULT GetDesktopDpi(out float dpiX, out float dpiY);
        HRESULT CreateRectangleGeometry(ref D2D1_RECT_F rectangle, out ID2D1RectangleGeometry rectangleGeometry);
        HRESULT CreateRoundedRectangleGeometry(D2D1_ROUNDED_RECT roundedRectangle, out ID2D1RoundedRectangleGeometry roundedRectangleGeometry);
        HRESULT CreateEllipseGeometry(ref D2D1_ELLIPSE ellipse, out ID2D1EllipseGeometry ellipseGeometry);
        HRESULT CreateGeometryGroup(D2D1_FILL_MODE fillMode, ID2D1Geometry geometries, uint geometriesCount, out ID2D1GeometryGroup geometryGroup);
        HRESULT CreateTransformedGeometry(ID2D1Geometry sourceGeometry, D2D1_MATRIX_3X2_F transform, out ID2D1TransformedGeometry transformedGeometry);
        HRESULT CreatePathGeometry(out ID2D1PathGeometry pathGeometry);
        ID2D1StrokeStyle CreateStrokeStyle(D2D1_STROKE_STYLE_PROPERTIES strokeStyleProperties, [MarshalAs(UnmanagedType.LPArray)] float[] dashes = null, uint dashesCount = 0);
        HRESULT CreateDrawingStateBlock(D2D1_DRAWING_STATE_DESCRIPTION drawingStateDescription, IDWriteRenderingParams textRenderingParams, out ID2D1DrawingStateBlock drawingStateBlock);
        HRESULT CreateWicBitmapRenderTarget(IWICBitmap target, D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, out ID2D1RenderTarget renderTarget);
        HRESULT CreateHwndRenderTarget(ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref D2D1_HWND_RENDER_TARGET_PROPERTIES hwndRenderTargetProperties, out ID2D1HwndRenderTarget hwndRenderTarget);
        HRESULT CreateDxgiSurfaceRenderTarget(IntPtr dxgiSurface, ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref ID2D1RenderTarget renderTarget);
        HRESULT CreateDCRenderTarget(ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref ID2D1DCRenderTarget dcRenderTarget);
    }


    [ComImport]
    [Guid("bb12d362-daee-4b9a-aa1d-14ba401cfa1f")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Factory1 : ID2D1Factory
    {
        #region <ID2D1Factory>
        new HRESULT ReloadSystemMetrics();
        new HRESULT GetDesktopDpi(out float dpiX, out float dpiY);
        new HRESULT CreateRectangleGeometry(ref D2D1_RECT_F rectangle, out ID2D1RectangleGeometry rectangleGeometry);
        new HRESULT CreateRoundedRectangleGeometry(D2D1_ROUNDED_RECT roundedRectangle, out ID2D1RoundedRectangleGeometry roundedRectangleGeometry);
        new HRESULT CreateEllipseGeometry(ref D2D1_ELLIPSE ellipse, out ID2D1EllipseGeometry ellipseGeometry);
        new HRESULT CreateGeometryGroup(D2D1_FILL_MODE fillMode, ID2D1Geometry geometries, uint geometriesCount, out ID2D1GeometryGroup geometryGroup);
        new HRESULT CreateTransformedGeometry(ID2D1Geometry sourceGeometry, D2D1_MATRIX_3X2_F transform, out ID2D1TransformedGeometry transformedGeometry);
        new HRESULT CreatePathGeometry(out ID2D1PathGeometry pathGeometry);
        new ID2D1StrokeStyle CreateStrokeStyle(D2D1_STROKE_STYLE_PROPERTIES strokeStyleProperties, [MarshalAs(UnmanagedType.LPArray)] float[] dashes = null, uint dashesCount = 0);
        new HRESULT CreateDrawingStateBlock(D2D1_DRAWING_STATE_DESCRIPTION drawingStateDescription, IDWriteRenderingParams textRenderingParams, out ID2D1DrawingStateBlock drawingStateBlock);
        new HRESULT CreateWicBitmapRenderTarget(IWICBitmap target, D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, out ID2D1RenderTarget renderTarget);
        new HRESULT CreateHwndRenderTarget(ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref D2D1_HWND_RENDER_TARGET_PROPERTIES hwndRenderTargetProperties, out ID2D1HwndRenderTarget hwndRenderTarget);
        new HRESULT CreateDxgiSurfaceRenderTarget(IntPtr dxgiSurface, ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref ID2D1RenderTarget renderTarget);
        new HRESULT CreateDCRenderTarget(ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref ID2D1DCRenderTarget dcRenderTarget);
        #endregion

        HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device d2dDevice);
        HRESULT CreateStrokeStyle(D2D1_STROKE_STYLE_PROPERTIES1 strokeStyleProperties, float dashes, uint dashesCount, out ID2D1StrokeStyle1 strokeStyle);
        HRESULT CreatePathGeometry(out ID2D1PathGeometry1 pathGeometry);
        HRESULT CreateDrawingStateBlock(D2D1_DRAWING_STATE_DESCRIPTION1 drawingStateDescription, ref IDWriteRenderingParams textRenderingParams, out ID2D1DrawingStateBlock1 drawingStateBlock);
        HRESULT CreateGdiMetafile(System.Runtime.InteropServices.ComTypes.IStream metafileStream, out ID2D1GdiMetafile metafile);
    }

    //    /// <summary>
    //    /// This globally registers the given effect. The effect can later be instantiated
    //    /// by using the registered class id. The effect registration is reference counted.
    //    /// </summary>
    //    STDMETHOD(RegisterEffectFromStream)(
    //        _In_ REFCLSID classId,
    //        _In_ IStream* propertyXml,
    //        _In_reads_opt_(bindingsCount) CONST D2D1_PROPERTY_BINDING * bindings,
    //        UINT32 bindingsCount,
    //        _In_ CONST PD2D1_EFFECT_FACTORY effectFactory
    //        ) PURE;

    //    /// <summary>
    //    /// This globally registers the given effect. The effect can later be instantiated
    //    /// by using the registered class id. The effect registration is reference counted.
    //    /// </summary>
    //    STDMETHOD(RegisterEffectFromString)(
    //        _In_ REFCLSID classId,
    //        _In_ PCWSTR propertyXml,
    //        _In_reads_opt_(bindingsCount) CONST D2D1_PROPERTY_BINDING * bindings,
    //        UINT32 bindingsCount,
    //        _In_ CONST PD2D1_EFFECT_FACTORY effectFactory
    //        ) PURE;

    //    /// <summary>
    //    /// This unregisters the given effect by its class id, you need to call
    //    /// UnregisterEffect for every call to ID2D1Factory1::RegisterEffectFromStream and
    //    /// ID2D1Factory1::RegisterEffectFromString to completely unregister it.
    //    /// </summary>
    //    STDMETHOD(UnregisterEffect)(
    //        _In_ REFCLSID classId
    //        ) PURE;

    //    /// <summary>
    //    /// This returns all of the registered effects in the process, including any
    //    /// built-in effects.
    //    /// </summary>
    //    /// <param name="effectsReturned">The number of effects returned into the passed in
    //    /// effects array.</param>
    //    /// <param name="effectsRegistered">The number of effects currently registered in
    //    /// the system.</param>
    //    STDMETHOD(GetRegisteredEffects)(
    //        _Out_writes_to_opt_(effectsCount, * effectsReturned) CLSID * effects,
    //         UINT32 effectsCount,
    //        _Out_opt_ UINT32 * effectsReturned,
    //        _Out_opt_ UINT32* effectsRegistered
    //        ) CONST PURE;

    //    /// <summary>
    //    /// This retrieves the effect properties for the given effect, all of the effect
    //    /// properties will be set to a default value since an effect is not instantiated to
    //    /// implement the returned property interface.
    //    /// </summary>
    //    STDMETHOD(GetEffectProperties)(
    //        _In_ REFCLSID effectId,
    //        _COM_Outptr_ ID2D1Properties** properties
    //        ) CONST PURE;

    //    COM_DECLSPEC_NOTHROW
    //    HRESULT
    //    CreateStrokeStyle(
    //        CONST D2D1_STROKE_STYLE_PROPERTIES1 &strokeStyleProperties,
    //        _In_reads_opt_(dashesCount) CONST FLOAT* dashes,
    //        UINT32 dashesCount,
    //        _COM_Outptr_ ID2D1StrokeStyle1 **strokeStyle
    //        )
    //    {
    //        return CreateStrokeStyle(&strokeStyleProperties, dashes, dashesCount, strokeStyle);
    //    }

    //    COM_DECLSPEC_NOTHROW
    //    HRESULT
    //    CreateDrawingStateBlock(
    //        CONST D2D1_DRAWING_STATE_DESCRIPTION1 &drawingStateDescription,
    //        _COM_Outptr_ ID2D1DrawingStateBlock1 **drawingStateBlock
    //        )
    //    {
    //        return CreateDrawingStateBlock(&drawingStateDescription, NULL, drawingStateBlock);
    //    }

    //    COM_DECLSPEC_NOTHROW
    //    HRESULT
    //    CreateDrawingStateBlock(
    //        _COM_Outptr_ ID2D1DrawingStateBlock1 **drawingStateBlock
    //        )
    //    {
    //        return CreateDrawingStateBlock(NULL, NULL, drawingStateBlock);
    //    }
    //}; // interface ID2D1Factory1


    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_DRAWING_STATE_DESCRIPTION1
    {
        D2D1_ANTIALIAS_MODE antialiasMode;
        D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode;
        UInt64 tag1;
        UInt64 tag2;
        D2D1_MATRIX_3X2_F transform;
        D2D1_PRIMITIVE_BLEND primitiveBlend;
        D2D1_UNIT_MODE unitMode;
    }

    [ComImport]
    [Guid("689f1f85-c72e-4e33-8f19-85754efd5ace")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1DrawingStateBlock1 : ID2D1DrawingStateBlock
    {
        #region ID2D1DrawingStateBlock
        #region ID2D1Resource
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        new void GetDescription(out D2D1_DRAWING_STATE_DESCRIPTION stateDescription);
        new void SetDescription(D2D1_DRAWING_STATE_DESCRIPTION stateDescription);
        new void SetTextRenderingParams(IDWriteRenderingParams textRenderingParams = null);
        new void GetTextRenderingParams(out IDWriteRenderingParams textRenderingParams);
        #endregion

        void GetDescription(out D2D1_DRAWING_STATE_DESCRIPTION1 stateDescription);     
        void SetDescription(D2D1_DRAWING_STATE_DESCRIPTION1 stateDescription);
    }

    [ComImport]
    [Guid("10a72a66-e91c-43f4-993f-ddf4b82b0b4a")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1StrokeStyle1 : ID2D1StrokeStyle
    {
        #region <ID2D1StrokeStyle>
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion
        new D2D1_CAP_STYLE GetStartCap();
        new D2D1_CAP_STYLE GetEndCap();
        new D2D1_CAP_STYLE GetDashCap();
        new float GetMiterLimit();
        new D2D1_LINE_JOIN GetLineJoin();
        new float GetDashOffset();
        new D2D1_DASH_STYLE GetDashStyle();
        new uint GetDashesCount();
        new void GetDashes(out float dashes, uint dashesCount);
        #endregion

        D2D1_STROKE_TRANSFORM_TYPE GetStrokeTransformType();
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_STROKE_STYLE_PROPERTIES1
    {
        D2D1_CAP_STYLE startCap;
        D2D1_CAP_STYLE endCap;
        D2D1_CAP_STYLE dashCap;
        D2D1_LINE_JOIN lineJoin;
        float miterLimit;
        D2D1_DASH_STYLE dashStyle;
        float dashOffset;
        /// <summary>
        /// How the nib of the stroke is influenced by the context properties.
        /// </summary>
        D2D1_STROKE_TRANSFORM_TYPE transformType;
    }

    public enum D2D1_STROKE_TRANSFORM_TYPE
    {
        /// <summary>
        /// The stroke respects the world transform, the DPI, and the stroke width.
        /// </summary>
        D2D1_STROKE_TRANSFORM_TYPE_NORMAL = 0,

        /// <summary>
        /// The stroke does not respect the world transform, but it does respect the DPI and
        /// the stroke width.
        /// </summary>
        D2D1_STROKE_TRANSFORM_TYPE_FIXED = 1,

        /// <summary>
        /// The stroke is forced to one pixel wide.
        /// </summary>
        D2D1_STROKE_TRANSFORM_TYPE_HAIRLINE = 2,
        D2D1_STROKE_TRANSFORM_TYPE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [ComImport()]
    [Guid("1c51bc64-de61-46fd-9899-63a5d8f03950")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1DCRenderTarget : ID2D1RenderTarget
    {
        new void GetFactory(out ID2D1Factory factory);

        new void CreateBitmap(D2D1_SIZE_U size, IntPtr srcData, uint pitch, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new HRESULT CreateBitmapFromWicBitmap(IWICBitmapSource wicBitmapSource, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new void CreateSharedBitmap(ref Guid riid, [In, Out] IntPtr data, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new void CreateBitmapBrush(ID2D1Bitmap bitmap, ref D2D1_BITMAP_BRUSH_PROPERTIES bitmapBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1BitmapBrush bitmapBrush);
        new HRESULT CreateSolidColorBrush(D2D1_COLOR_F color, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1SolidColorBrush solidColorBrush);
        new void CreateGradientStopCollection(D2D1_GRADIENT_STOP gradientStops, uint gradientStopsCount, D2D1_GAMMA colorInterpolationGamma, D2D1_EXTEND_MODE extendMode, out ID2D1GradientStopCollection gradientStopCollection);
        new void CreateLinearGradientBrush(D2D1_LINEAR_GRADIENT_BRUSH_PROPERTIES linearGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1LinearGradientBrush linearGradientBrush);
        new void CreateRadialGradientBrush(D2D1_RADIAL_GRADIENT_BRUSH_PROPERTIES radialGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1RadialGradientBrush radialGradientBrush);
        new void CreateCompatibleRenderTarget(D2D1_SIZE_F desiredSize, D2D1_SIZE_U desiredPixelSize, D2D1_PIXEL_FORMAT desiredFormat, D2D1_COMPATIBLE_RENDER_TARGET_OPTIONS options, out ID2D1BitmapRenderTarget bitmapRenderTarget);
        new void CreateLayer(D2D1_SIZE_F size, out ID2D1Layer layer);
        new void CreateMesh(out ID2D1Mesh mesh);
        new void DrawLine(ref D2D1_POINT_2F point0, ref D2D1_POINT_2F point1, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void DrawRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush);
        new void DrawRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush);
        new void DrawEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush);
        new void DrawGeometry(ID2D1Geometry geometry, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillGeometry(ID2D1Geometry geometry, ID2D1Brush brush, ID2D1Brush opacityBrush = null);
        new void FillMesh(ID2D1Mesh mesh, ID2D1Brush brush);
        new void FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, D2D1_OPACITY_MASK_CONTENT content, D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        new void DrawBitmap(ID2D1Bitmap bitmap, ref D2D1_RECT_F destinationRectangle, float opacity, D2D1_BITMAP_INTERPOLATION_MODE interpolationMode, ref D2D1_RECT_F sourceRectangle);
        new void DrawText(string str, uint stringLength, IDWriteTextFormat textFormat, D2D1_RECT_F layoutRect, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options, DWRITE_MEASURING_MODE measuringMode);
        new void DrawTextLayout(ref D2D1_POINT_2F origin, IDWriteTextLayout textLayout, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options);
        new void DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        new void SetTransform(D2D1_MATRIX_3X2_F transform);
        new void GetTransform(out D2D1_MATRIX_3X2_F transform);
        new void SetAntialiasMode(D2D1_ANTIALIAS_MODE antialiasMode);
        new D2D1_ANTIALIAS_MODE GetAntialiasMode();
        new void SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode);
        new D2D1_TEXT_ANTIALIAS_MODE GetTextAntialiasMode();
        new void SetTextRenderingParams(IDWriteRenderingParams textRenderingParams = null/* TODO Change to default(_) if this is not a reference type */);
        new void GetTextRenderingParams(out IDWriteRenderingParams textRenderingParams);
        new void SetTags(UInt64 tag1, UInt64 tag2);
        new void GetTags(out UInt64 tag1, out UInt64 tag2);
        new void PushLayer(D2D1_LAYER_PARAMETERS layerParameters, ID2D1Layer layer);
        new void PopLayer();
        new void Flush(out UInt64 tag1, out UInt64 tag2);
        new void SaveDrawingState(ID2D1DrawingStateBlock drawingStateBlock);
        new void RestoreDrawingState(ID2D1DrawingStateBlock drawingStateBlock);
        new void PushAxisAlignedClip(D2D1_RECT_F clipRect, D2D1_ANTIALIAS_MODE antialiasMode);
        new void PopAxisAlignedClip();
        new void Clear(D2D1_COLOR_F clearColor);
        new void BeginDraw();
        new void EndDraw(out UInt64 tag1, out UInt64 tag2);
        new D2D1_PIXEL_FORMAT GetPixelFormat();
        new void SetDpi(float dpiX, float dpiY);
        new void GetDpi(out float dpiX, out float dpiY);
        new D2D1_SIZE_F GetSize();
        new D2D1_SIZE_U GetPixelSize();
        new uint GetMaximumBitmapSize();
        new bool IsSupported(D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties);
        new HRESULT Function1();
        new HRESULT Function2();
        new HRESULT Function3();
        new HRESULT Function4();
        new HRESULT Function5();
        new HRESULT Function6();
        new HRESULT Function7();
        new HRESULT Function8();
        new HRESULT Function9();
        new HRESULT Function10();
        new HRESULT Function11();
        new HRESULT Function12();
        new HRESULT Function13();
        new HRESULT Function14();
        new HRESULT Function15();
        new HRESULT Function16();
        new HRESULT Function17();
        new HRESULT Function18();
        new HRESULT Function19();
        new HRESULT Function20();
        new HRESULT Function21();
        new void Function22();
        new void Function23();
        new void Function24();
        new void Function25();
        new void Function26();
        new void Function27();
        new void Function28();
        new void Function29();
        new void Function30();
        new void Function31();
        new void Function32();
        new void Function33();
        new void Function34();
        new void Function35();
        new bool Function36();

        HRESULT BindDC(IntPtr hDC, RECT pSubRect);
    }

    [ComImport]
    [Guid("28211a43-7d89-476f-8181-2d6159b220ad")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Effect : ID2D1Properties
    {
        #region <ID2D1Properties>
        new uint GetPropertyCount();
        new HRESULT GetPropertyName(uint index, out string name, uint nameCount);
        new uint GetPropertyNameLength(uint index);
        new D2D1_PROPERTY_TYPE GetType(uint index);
        new uint GetPropertyIndex(string name);
        new HRESULT SetValueByName(string name, D2D1_PROPERTY_TYPE type, IntPtr data, uint dataSize);
        new HRESULT SetValue(uint index, D2D1_PROPERTY_TYPE type, IntPtr data, uint dataSize);
        new HRESULT GetValueByName(string name, D2D1_PROPERTY_TYPE type, out IntPtr data, uint dataSize);
        new HRESULT GetValue(uint index, D2D1_PROPERTY_TYPE type, out IntPtr data, uint dataSize);
        new uint GetValueSize(uint index);
        new HRESULT GetSubProperties(uint index, out ID2D1Properties subProperties);
        #endregion

        void SetInput(uint index, ID2D1Image input, bool invalidate = true);
        HRESULT SetInputCount(uint inputCount);
        void GetInput(uint index, out ID2D1Image input);
        uint GetInputCount();
        void GetOutput(out ID2D1Image outputImage);

        //void SetInputEffect(uint index, ID2D1Effect inputEffect, bool invalidate = true)
        //{
        //    ID2D1Image output = null;
        //    if (inputEffect != null)
        //    {
        //        inputEffect.GetOutput(out output);
        //    }
        //    SetInput(index, output, invalidate);
        //    if (output != null)
        //    {
        //        Marshal.ReleaseComObject(output);
        //    }
        //}
    }

    [ComImport]
    [Guid("483473d7-cd46-4f9d-9d3a-3112aa80159d")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Properties
    {
        uint GetPropertyCount();
        HRESULT GetPropertyName(uint index, out string name, uint nameCount);
        uint GetPropertyNameLength(uint index);
        D2D1_PROPERTY_TYPE GetType(uint index);
        uint GetPropertyIndex(string name);
        HRESULT SetValueByName(string name, D2D1_PROPERTY_TYPE type, IntPtr data, uint dataSize);
        HRESULT SetValue(uint index, D2D1_PROPERTY_TYPE type, IntPtr data, uint dataSize);
        HRESULT GetValueByName(string name, D2D1_PROPERTY_TYPE type, out IntPtr data, uint dataSize);
        HRESULT GetValue(uint index, D2D1_PROPERTY_TYPE type, out IntPtr data, uint dataSize);
        uint GetValueSize(uint index);
        HRESULT GetSubProperties(uint index, out ID2D1Properties subProperties);
    }

    public enum D2D1_PROPERTY_TYPE
    {
        D2D1_PROPERTY_TYPE_UNKNOWN = 0,
        D2D1_PROPERTY_TYPE_STRING = 1,
        D2D1_PROPERTY_TYPE_BOOL = 2,
        D2D1_PROPERTY_TYPE_UINT32 = 3,
        D2D1_PROPERTY_TYPE_INT32 = 4,
        D2D1_PROPERTY_TYPE_FLOAT = 5,
        D2D1_PROPERTY_TYPE_VECTOR2 = 6,
        D2D1_PROPERTY_TYPE_VECTOR3 = 7,
        D2D1_PROPERTY_TYPE_VECTOR4 = 8,
        D2D1_PROPERTY_TYPE_BLOB = 9,
        D2D1_PROPERTY_TYPE_IUNKNOWN = 10,
        D2D1_PROPERTY_TYPE_ENUM = 11,
        D2D1_PROPERTY_TYPE_ARRAY = 12,
        D2D1_PROPERTY_TYPE_CLSID = 13,
        D2D1_PROPERTY_TYPE_MATRIX_3X2 = 14,
        D2D1_PROPERTY_TYPE_MATRIX_4X3 = 15,
        D2D1_PROPERTY_TYPE_MATRIX_4X4 = 16,
        D2D1_PROPERTY_TYPE_MATRIX_5X4 = 17,
        D2D1_PROPERTY_TYPE_COLOR_CONTEXT = 18,
        D2D1_PROPERTY_TYPE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [ComImport]
    [Guid("e8f7fe7a-191c-466d-ad95-975678bda998")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1DeviceContext : ID2D1RenderTarget
    {
        #region <ID2D1RenderTarget>

        #region <ID2D1Resource>

        new void GetFactory(out ID2D1Factory factory);

        #endregion
        new void CreateBitmap(D2D1_SIZE_U size, IntPtr srcData, uint pitch, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new HRESULT CreateBitmapFromWicBitmap(IWICBitmapSource wicBitmapSource, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new void CreateSharedBitmap(ref Guid riid, [In, Out] IntPtr data, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new void CreateBitmapBrush(ID2D1Bitmap bitmap, ref D2D1_BITMAP_BRUSH_PROPERTIES bitmapBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1BitmapBrush bitmapBrush);
        new HRESULT CreateSolidColorBrush(D2D1_COLOR_F color, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1SolidColorBrush solidColorBrush);
        new void CreateGradientStopCollection(D2D1_GRADIENT_STOP gradientStops, uint gradientStopsCount, D2D1_GAMMA colorInterpolationGamma, D2D1_EXTEND_MODE extendMode, out ID2D1GradientStopCollection gradientStopCollection);
        new void CreateLinearGradientBrush(D2D1_LINEAR_GRADIENT_BRUSH_PROPERTIES linearGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1LinearGradientBrush linearGradientBrush);
        new void CreateRadialGradientBrush(D2D1_RADIAL_GRADIENT_BRUSH_PROPERTIES radialGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1RadialGradientBrush radialGradientBrush);
        new void CreateCompatibleRenderTarget(D2D1_SIZE_F desiredSize, D2D1_SIZE_U desiredPixelSize, D2D1_PIXEL_FORMAT desiredFormat, D2D1_COMPATIBLE_RENDER_TARGET_OPTIONS options, out ID2D1BitmapRenderTarget bitmapRenderTarget);
        new void CreateLayer(D2D1_SIZE_F size, out ID2D1Layer layer);
        new void CreateMesh(out ID2D1Mesh mesh);
        new void DrawLine(ref D2D1_POINT_2F point0, ref D2D1_POINT_2F point1, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void DrawRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush);
        new void DrawRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush);
        new void DrawEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush);
        new void DrawGeometry(ID2D1Geometry geometry, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillGeometry(ID2D1Geometry geometry, ID2D1Brush brush, ID2D1Brush opacityBrush = null);
        new void FillMesh(ID2D1Mesh mesh, ID2D1Brush brush);
        new void FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, D2D1_OPACITY_MASK_CONTENT content, D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        new void DrawBitmap(ID2D1Bitmap bitmap, ref D2D1_RECT_F destinationRectangle, float opacity, D2D1_BITMAP_INTERPOLATION_MODE interpolationMode, ref D2D1_RECT_F sourceRectangle);
        new void DrawText(string str, uint stringLength, IDWriteTextFormat textFormat, D2D1_RECT_F layoutRect, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options, DWRITE_MEASURING_MODE measuringMode);
        new void DrawTextLayout(ref D2D1_POINT_2F origin, IDWriteTextLayout textLayout, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options);
        new void DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        new void SetTransform(D2D1_MATRIX_3X2_F transform);
        new void GetTransform(out D2D1_MATRIX_3X2_F transform);
        new void SetAntialiasMode(D2D1_ANTIALIAS_MODE antialiasMode);
        new D2D1_ANTIALIAS_MODE GetAntialiasMode();
        new void SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode);
        new D2D1_TEXT_ANTIALIAS_MODE GetTextAntialiasMode();
        new void SetTextRenderingParams(IDWriteRenderingParams textRenderingParams = null);
        new void GetTextRenderingParams(out IDWriteRenderingParams textRenderingParams);
        new void SetTags(UInt64 tag1, UInt64 tag2);
        new void GetTags(out UInt64 tag1, out UInt64 tag2);
        new void PushLayer(D2D1_LAYER_PARAMETERS layerParameters, ID2D1Layer layer);
        new void PopLayer();
        new void Flush(out UInt64 tag1, out UInt64 tag2);
        new void SaveDrawingState([In, Out] ID2D1DrawingStateBlock drawingStateBlock);
        new void RestoreDrawingState(ID2D1DrawingStateBlock drawingStateBlock);
        new void PushAxisAlignedClip(D2D1_RECT_F clipRect, D2D1_ANTIALIAS_MODE antialiasMode);
        new void PopAxisAlignedClip();
        new void Clear(D2D1_COLOR_F clearColor);
        new void BeginDraw();
        new HRESULT EndDraw(out UInt64 tag1, out UInt64 tag2);
        new D2D1_PIXEL_FORMAT GetPixelFormat();
        new void SetDpi(float dpiX, float dpiY);
        new void GetDpi(out float dpiX, out float dpiY);
        new D2D1_SIZE_F GetSize();
        new D2D1_SIZE_U GetPixelSize();
        new uint GetMaximumBitmapSize();
        new bool IsSupported(D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties);
        #endregion

        HRESULT CreateBitmap(D2D1_SIZE_U size, IntPtr sourceData, uint pitch, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        HRESULT CreateBitmapFromWicBitmap(IWICBitmapSource wicBitmapSource, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        HRESULT CreateColorContext(D2D1_COLOR_SPACE space, IntPtr profile, uint profileSize, out ID2D1ColorContext colorContext);
        HRESULT CreateColorContextFromFilename(string filename, out ID2D1ColorContext colorContext);
        HRESULT CreateColorContextFromWicColorContext(IWICColorContext wicColorContext, out ID2D1ColorContext colorContext);
        HRESULT CreateBitmapFromDxgiSurface(IDXGISurface surface, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        HRESULT CreateEffect(ref Guid effectId, out ID2D1Effect effect);
        HRESULT CreateGradientStopCollection(D2D1_GRADIENT_STOP straightAlphaGradientStops, uint straightAlphaGradientStopsCount, D2D1_COLOR_SPACE preInterpolationSpace,
            D2D1_COLOR_SPACE postInterpolationSpace, D2D1_BUFFER_PRECISION bufferPrecision, D2D1_EXTEND_MODE extendMode, D2D1_COLOR_INTERPOLATION_MODE colorInterpolationMode,
            out ID2D1GradientStopCollection1 gradientStopCollection1);
        HRESULT CreateImageBrush(ID2D1Image image, ref D2D1_IMAGE_BRUSH_PROPERTIES imageBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1ImageBrush imageBrush);
        HRESULT CreateBitmapBrush(ID2D1Bitmap bitmap, ref D2D1_BITMAP_BRUSH_PROPERTIES1 bitmapBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1BitmapBrush1 bitmapBrush);
        HRESULT CreateCommandList(out ID2D1CommandList commandList);
        bool IsDxgiFormatSupported(DXGI_FORMAT format);
        bool IsBufferPrecisionSupported(D2D1_BUFFER_PRECISION bufferPrecision);
        HRESULT GetImageLocalBounds(ID2D1Image image, out D2D1_RECT_F localBounds);
        HRESULT GetImageWorldBounds(ID2D1Image image, out D2D1_RECT_F worldBounds);
        HRESULT GetGlyphRunWorldBounds(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, DWRITE_MEASURING_MODE measuringMode, out D2D1_RECT_F bounds);
        void GetDevice(out ID2D1Device device);
        void SetTarget(ID2D1Image image);
        void GetTarget(out ID2D1Image image);
        void SetRenderingControls(D2D1_RENDERING_CONTROLS renderingControls);
        void GetRenderingControls(out D2D1_RENDERING_CONTROLS renderingControls);
        void SetPrimitiveBlend(D2D1_PRIMITIVE_BLEND primitiveBlend);
        D2D1_PRIMITIVE_BLEND GetPrimitiveBlend();
        void SetUnitMode(D2D1_UNIT_MODE unitMode);
        D2D1_UNIT_MODE GetUnitMode();
        void DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, DWRITE_GLYPH_RUN_DESCRIPTION glyphRunDescription, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        void DrawImage(ID2D1Image image, ref D2D1_POINT_2F targetOffset, ref D2D1_RECT_F imageRectangle, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_COMPOSITE_MODE compositeMode);
        void DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_POINT_2F targetOffset);
        void DrawBitmap(ID2D1Bitmap bitmap, ref D2D1_RECT_F destinationRectangle, float opacity, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_RECT_F sourceRectangle = new D2D1_RECT_F(), D2D1_MATRIX_4X4_F perspectiveTransform = null);
        void PushLayer(D2D1_LAYER_PARAMETERS1 layerParameters, ID2D1Layer layer);
        HRESULT InvalidateEffectInputRectangle(ID2D1Effect effect, uint input, D2D1_RECT_F inputRectangle);
        HRESULT GetEffectInvalidRectangleCount(ID2D1Effect effect, out uint rectangleCount);
        //HRESULT GetEffectInvalidRectangles(ID2D1Effect effect, out D2D1_RECT_F* rectangles, uint rectanglesCount);
        HRESULT GetEffectInvalidRectangles(ID2D1Effect effect, out IntPtr rectangles, uint rectanglesCount);
        HRESULT GetEffectRequiredInputRectangles(ID2D1Effect renderEffect, D2D1_RECT_F renderImageRectangle, D2D1_EFFECT_INPUT_DESCRIPTION inputDescriptions,
            //out D2D1_RECT_F* requiredInputRects, uint inputCount);
            out IntPtr requiredInputRects, uint inputCount);
        void FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, ref D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
    }

    [ComImport]
    [Guid("d37f57e4-6908-459f-a199-e72f24f79987")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1DeviceContext1 : ID2D1DeviceContext
    {
        #region <ID2D1DeviceContext>
        #region <ID2D1RenderTarget>

        #region <ID2D1Resource>

        new void GetFactory(out ID2D1Factory factory);

        #endregion
        new void CreateBitmap(D2D1_SIZE_U size, IntPtr srcData, uint pitch, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new HRESULT CreateBitmapFromWicBitmap(IWICBitmapSource wicBitmapSource, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new void CreateSharedBitmap(ref Guid riid, [In, Out] IntPtr data, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new void CreateBitmapBrush(ID2D1Bitmap bitmap, ref D2D1_BITMAP_BRUSH_PROPERTIES bitmapBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1BitmapBrush bitmapBrush);
        new HRESULT CreateSolidColorBrush(D2D1_COLOR_F color, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1SolidColorBrush solidColorBrush);
        new void CreateGradientStopCollection(D2D1_GRADIENT_STOP gradientStops, uint gradientStopsCount, D2D1_GAMMA colorInterpolationGamma, D2D1_EXTEND_MODE extendMode, out ID2D1GradientStopCollection gradientStopCollection);
        new void CreateLinearGradientBrush(D2D1_LINEAR_GRADIENT_BRUSH_PROPERTIES linearGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1LinearGradientBrush linearGradientBrush);
        new void CreateRadialGradientBrush(D2D1_RADIAL_GRADIENT_BRUSH_PROPERTIES radialGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1RadialGradientBrush radialGradientBrush);
        new void CreateCompatibleRenderTarget(D2D1_SIZE_F desiredSize, D2D1_SIZE_U desiredPixelSize, D2D1_PIXEL_FORMAT desiredFormat, D2D1_COMPATIBLE_RENDER_TARGET_OPTIONS options, out ID2D1BitmapRenderTarget bitmapRenderTarget);
        new void CreateLayer(D2D1_SIZE_F size, out ID2D1Layer layer);
        new void CreateMesh(out ID2D1Mesh mesh);
        new void DrawLine(ref D2D1_POINT_2F point0, ref D2D1_POINT_2F point1, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void DrawRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush);
        new void DrawRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush);
        new void DrawEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush);
        new void DrawGeometry(ID2D1Geometry geometry, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillGeometry(ID2D1Geometry geometry, ID2D1Brush brush, ID2D1Brush opacityBrush = null);
        new void FillMesh(ID2D1Mesh mesh, ID2D1Brush brush);
        new void FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, D2D1_OPACITY_MASK_CONTENT content, D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        new void DrawBitmap(ID2D1Bitmap bitmap, ref D2D1_RECT_F destinationRectangle, float opacity, D2D1_BITMAP_INTERPOLATION_MODE interpolationMode, ref D2D1_RECT_F sourceRectangle);
        new void DrawText(string str, uint stringLength, IDWriteTextFormat textFormat, D2D1_RECT_F layoutRect, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options, DWRITE_MEASURING_MODE measuringMode);
        new void DrawTextLayout(ref D2D1_POINT_2F origin, IDWriteTextLayout textLayout, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options);
        new void DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        new void SetTransform(D2D1_MATRIX_3X2_F transform);
        new void GetTransform(out D2D1_MATRIX_3X2_F transform);
        new void SetAntialiasMode(D2D1_ANTIALIAS_MODE antialiasMode);
        new D2D1_ANTIALIAS_MODE GetAntialiasMode();
        new void SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode);
        new D2D1_TEXT_ANTIALIAS_MODE GetTextAntialiasMode();
        new void SetTextRenderingParams(IDWriteRenderingParams textRenderingParams = null);
        new void GetTextRenderingParams(out IDWriteRenderingParams textRenderingParams);
        new void SetTags(UInt64 tag1, UInt64 tag2);
        new void GetTags(out UInt64 tag1, out UInt64 tag2);
        new void PushLayer(D2D1_LAYER_PARAMETERS layerParameters, ID2D1Layer layer);
        new void PopLayer();
        new void Flush(out UInt64 tag1, out UInt64 tag2);
        new void SaveDrawingState([In, Out] ID2D1DrawingStateBlock drawingStateBlock);
        new void RestoreDrawingState(ID2D1DrawingStateBlock drawingStateBlock);
        new void PushAxisAlignedClip(D2D1_RECT_F clipRect, D2D1_ANTIALIAS_MODE antialiasMode);
        new void PopAxisAlignedClip();
        new void Clear(D2D1_COLOR_F clearColor);
        new void BeginDraw();
        new HRESULT EndDraw(out UInt64 tag1, out UInt64 tag2);
        new D2D1_PIXEL_FORMAT GetPixelFormat();
        new void SetDpi(float dpiX, float dpiY);
        new void GetDpi(out float dpiX, out float dpiY);
        new D2D1_SIZE_F GetSize();
        new D2D1_SIZE_U GetPixelSize();
        new uint GetMaximumBitmapSize();
        new bool IsSupported(D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties);
        #endregion

        new HRESULT CreateBitmap(D2D1_SIZE_U size, IntPtr sourceData, uint pitch, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        new HRESULT CreateBitmapFromWicBitmap(IWICBitmapSource wicBitmapSource, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        new HRESULT CreateColorContext(D2D1_COLOR_SPACE space, IntPtr profile, uint profileSize, out ID2D1ColorContext colorContext);
        new HRESULT CreateColorContextFromFilename(string filename, out ID2D1ColorContext colorContext);
        new HRESULT CreateColorContextFromWicColorContext(IWICColorContext wicColorContext, out ID2D1ColorContext colorContext);
        new HRESULT CreateBitmapFromDxgiSurface(IDXGISurface surface, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        new HRESULT CreateEffect(ref Guid effectId, out ID2D1Effect effect);
        new HRESULT CreateGradientStopCollection(D2D1_GRADIENT_STOP straightAlphaGradientStops, uint straightAlphaGradientStopsCount, D2D1_COLOR_SPACE preInterpolationSpace,
            D2D1_COLOR_SPACE postInterpolationSpace, D2D1_BUFFER_PRECISION bufferPrecision, D2D1_EXTEND_MODE extendMode, D2D1_COLOR_INTERPOLATION_MODE colorInterpolationMode,
            out ID2D1GradientStopCollection1 gradientStopCollection1);
        new HRESULT CreateImageBrush(ID2D1Image image, ref D2D1_IMAGE_BRUSH_PROPERTIES imageBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1ImageBrush imageBrush);
        new HRESULT CreateBitmapBrush(ID2D1Bitmap bitmap, ref D2D1_BITMAP_BRUSH_PROPERTIES1 bitmapBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1BitmapBrush1 bitmapBrush);
        new HRESULT CreateCommandList(out ID2D1CommandList commandList);
        new bool IsDxgiFormatSupported(DXGI_FORMAT format);
        new bool IsBufferPrecisionSupported(D2D1_BUFFER_PRECISION bufferPrecision);
        new HRESULT GetImageLocalBounds(ID2D1Image image, out D2D1_RECT_F localBounds);
        new HRESULT GetImageWorldBounds(ID2D1Image image, out D2D1_RECT_F worldBounds);
        new HRESULT GetGlyphRunWorldBounds(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, DWRITE_MEASURING_MODE measuringMode, out D2D1_RECT_F bounds);
        new void GetDevice(out ID2D1Device device);
        new void SetTarget(ID2D1Image image);
        new void GetTarget(out ID2D1Image image);
        new void SetRenderingControls(D2D1_RENDERING_CONTROLS renderingControls);
        new void GetRenderingControls(out D2D1_RENDERING_CONTROLS renderingControls);
        new void SetPrimitiveBlend(D2D1_PRIMITIVE_BLEND primitiveBlend);
        new D2D1_PRIMITIVE_BLEND GetPrimitiveBlend();
        new void SetUnitMode(D2D1_UNIT_MODE unitMode);
        new D2D1_UNIT_MODE GetUnitMode();
        new void DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, DWRITE_GLYPH_RUN_DESCRIPTION glyphRunDescription, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        new void DrawImage(ID2D1Image image, ref D2D1_POINT_2F targetOffset, ref D2D1_RECT_F imageRectangle, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_COMPOSITE_MODE compositeMode);
        new void DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_POINT_2F targetOffset);
        new void DrawBitmap(ID2D1Bitmap bitmap, ref D2D1_RECT_F destinationRectangle, float opacity, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_RECT_F sourceRectangle, D2D1_MATRIX_4X4_F perspectiveTransform);
        new void PushLayer(D2D1_LAYER_PARAMETERS1 layerParameters, ID2D1Layer layer);
        new HRESULT InvalidateEffectInputRectangle(ID2D1Effect effect, uint input, D2D1_RECT_F inputRectangle);
        new HRESULT GetEffectInvalidRectangleCount(ID2D1Effect effect, out uint rectangleCount);
        //new  HRESULT GetEffectInvalidRectangles(ID2D1Effect effect, out D2D1_RECT_F* rectangles, uint rectanglesCount);
        new HRESULT GetEffectInvalidRectangles(ID2D1Effect effect, out IntPtr rectangles, uint rectanglesCount);
        new HRESULT GetEffectRequiredInputRectangles(ID2D1Effect renderEffect, D2D1_RECT_F renderImageRectangle, D2D1_EFFECT_INPUT_DESCRIPTION inputDescriptions,
            // out D2D1_RECT_F* requiredInputRects, uint inputCount);
            out IntPtr requiredInputRects, uint inputCount);
        new void FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, ref D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        #endregion

        HRESULT CreateFilledGeometryRealization(ID2D1Geometry geometry, float flatteningTolerance, out ID2D1GeometryRealization geometryRealization);
        HRESULT CreateStrokedGeometryRealization(ID2D1Geometry geometry, float flatteningTolerance, float strokeWidth, ID2D1StrokeStyle strokeStyle, out ID2D1GeometryRealization geometryRealization);
        void DrawGeometryRealization(ID2D1GeometryRealization geometryRealization, ID2D1Brush brush);
    }

    [ComImport]
    [Guid("a16907d7-bc02-4801-99e8-8cf7f485f774")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1GeometryRealization : ID2D1Resource
    {
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion
    }

    [ComImport]
    [Guid("394ea6a3-0c34-4321-950b-6ca20f0be6c7")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1DeviceContext2 : ID2D1DeviceContext1
    {
        #region <ID2D1DeviceContext1>
        #region <ID2D1DeviceContext>
        #region <ID2D1RenderTarget>

        #region <ID2D1Resource>

        new void GetFactory(out ID2D1Factory factory);

        #endregion
        new void CreateBitmap(D2D1_SIZE_U size, IntPtr srcData, uint pitch, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new HRESULT CreateBitmapFromWicBitmap(IWICBitmapSource wicBitmapSource, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new void CreateSharedBitmap(ref Guid riid, [In, Out] IntPtr data, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new void CreateBitmapBrush(ID2D1Bitmap bitmap, ref D2D1_BITMAP_BRUSH_PROPERTIES bitmapBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1BitmapBrush bitmapBrush);
        new HRESULT CreateSolidColorBrush(D2D1_COLOR_F color, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1SolidColorBrush solidColorBrush);
        new void CreateGradientStopCollection(D2D1_GRADIENT_STOP gradientStops, uint gradientStopsCount, D2D1_GAMMA colorInterpolationGamma, D2D1_EXTEND_MODE extendMode, out ID2D1GradientStopCollection gradientStopCollection);
        new void CreateLinearGradientBrush(D2D1_LINEAR_GRADIENT_BRUSH_PROPERTIES linearGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1LinearGradientBrush linearGradientBrush);
        new void CreateRadialGradientBrush(D2D1_RADIAL_GRADIENT_BRUSH_PROPERTIES radialGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1RadialGradientBrush radialGradientBrush);
        new void CreateCompatibleRenderTarget(D2D1_SIZE_F desiredSize, D2D1_SIZE_U desiredPixelSize, D2D1_PIXEL_FORMAT desiredFormat, D2D1_COMPATIBLE_RENDER_TARGET_OPTIONS options, out ID2D1BitmapRenderTarget bitmapRenderTarget);
        new void CreateLayer(D2D1_SIZE_F size, out ID2D1Layer layer);
        new void CreateMesh(out ID2D1Mesh mesh);
        new void DrawLine(ref D2D1_POINT_2F point0, ref D2D1_POINT_2F point1, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void DrawRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush);
        new void DrawRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush);
        new void DrawEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush);
        new void DrawGeometry(ID2D1Geometry geometry, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillGeometry(ID2D1Geometry geometry, ID2D1Brush brush, ID2D1Brush opacityBrush = null);
        new void FillMesh(ID2D1Mesh mesh, ID2D1Brush brush);
        new void FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, D2D1_OPACITY_MASK_CONTENT content, D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        new void DrawBitmap(ID2D1Bitmap bitmap, ref D2D1_RECT_F destinationRectangle, float opacity, D2D1_BITMAP_INTERPOLATION_MODE interpolationMode, ref D2D1_RECT_F sourceRectangle);
        new void DrawText(string str, uint stringLength, IDWriteTextFormat textFormat, D2D1_RECT_F layoutRect, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options, DWRITE_MEASURING_MODE measuringMode);
        new void DrawTextLayout(ref D2D1_POINT_2F origin, IDWriteTextLayout textLayout, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options);
        new void DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        new void SetTransform(D2D1_MATRIX_3X2_F transform);
        new void GetTransform(out D2D1_MATRIX_3X2_F transform);
        new void SetAntialiasMode(D2D1_ANTIALIAS_MODE antialiasMode);
        new D2D1_ANTIALIAS_MODE GetAntialiasMode();
        new void SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode);
        new D2D1_TEXT_ANTIALIAS_MODE GetTextAntialiasMode();
        new void SetTextRenderingParams(IDWriteRenderingParams textRenderingParams = null);
        new void GetTextRenderingParams(out IDWriteRenderingParams textRenderingParams);
        new void SetTags(UInt64 tag1, UInt64 tag2);
        new void GetTags(out UInt64 tag1, out UInt64 tag2);
        new void PushLayer(D2D1_LAYER_PARAMETERS layerParameters, ID2D1Layer layer);
        new void PopLayer();
        new void Flush(out UInt64 tag1, out UInt64 tag2);
        new void SaveDrawingState([In, Out] ID2D1DrawingStateBlock drawingStateBlock);
        new void RestoreDrawingState(ID2D1DrawingStateBlock drawingStateBlock);
        new void PushAxisAlignedClip(D2D1_RECT_F clipRect, D2D1_ANTIALIAS_MODE antialiasMode);
        new void PopAxisAlignedClip();
        new void Clear(D2D1_COLOR_F clearColor);
        new void BeginDraw();
        new HRESULT EndDraw(out UInt64 tag1, out UInt64 tag2);
        new D2D1_PIXEL_FORMAT GetPixelFormat();
        new void SetDpi(float dpiX, float dpiY);
        new void GetDpi(out float dpiX, out float dpiY);
        new D2D1_SIZE_F GetSize();
        new D2D1_SIZE_U GetPixelSize();
        new uint GetMaximumBitmapSize();
        new bool IsSupported(D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties);
        #endregion

        new HRESULT CreateBitmap(D2D1_SIZE_U size, IntPtr sourceData, uint pitch, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        new HRESULT CreateBitmapFromWicBitmap(IWICBitmapSource wicBitmapSource, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        new HRESULT CreateColorContext(D2D1_COLOR_SPACE space, IntPtr profile, uint profileSize, out ID2D1ColorContext colorContext);
        new HRESULT CreateColorContextFromFilename(string filename, out ID2D1ColorContext colorContext);
        new HRESULT CreateColorContextFromWicColorContext(IWICColorContext wicColorContext, out ID2D1ColorContext colorContext);
        new HRESULT CreateBitmapFromDxgiSurface(IDXGISurface surface, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        new HRESULT CreateEffect(ref Guid effectId, out ID2D1Effect effect);
        new HRESULT CreateGradientStopCollection(D2D1_GRADIENT_STOP straightAlphaGradientStops, uint straightAlphaGradientStopsCount, D2D1_COLOR_SPACE preInterpolationSpace,
            D2D1_COLOR_SPACE postInterpolationSpace, D2D1_BUFFER_PRECISION bufferPrecision, D2D1_EXTEND_MODE extendMode, D2D1_COLOR_INTERPOLATION_MODE colorInterpolationMode,
            out ID2D1GradientStopCollection1 gradientStopCollection1);
        new HRESULT CreateImageBrush(ID2D1Image image, ref D2D1_IMAGE_BRUSH_PROPERTIES imageBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1ImageBrush imageBrush);
        new HRESULT CreateBitmapBrush(ID2D1Bitmap bitmap, ref D2D1_BITMAP_BRUSH_PROPERTIES1 bitmapBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1BitmapBrush1 bitmapBrush);
        new HRESULT CreateCommandList(out ID2D1CommandList commandList);
        new bool IsDxgiFormatSupported(DXGI_FORMAT format);
        new bool IsBufferPrecisionSupported(D2D1_BUFFER_PRECISION bufferPrecision);
        new HRESULT GetImageLocalBounds(ID2D1Image image, out D2D1_RECT_F localBounds);
        new HRESULT GetImageWorldBounds(ID2D1Image image, out D2D1_RECT_F worldBounds);
        new HRESULT GetGlyphRunWorldBounds(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, DWRITE_MEASURING_MODE measuringMode, out D2D1_RECT_F bounds);
        new void GetDevice(out ID2D1Device device);
        new void SetTarget(ID2D1Image image);
        new void GetTarget(out ID2D1Image image);
        new void SetRenderingControls(D2D1_RENDERING_CONTROLS renderingControls);
        new void GetRenderingControls(out D2D1_RENDERING_CONTROLS renderingControls);
        new void SetPrimitiveBlend(D2D1_PRIMITIVE_BLEND primitiveBlend);
        new D2D1_PRIMITIVE_BLEND GetPrimitiveBlend();
        new void SetUnitMode(D2D1_UNIT_MODE unitMode);
        new D2D1_UNIT_MODE GetUnitMode();
        new void DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, DWRITE_GLYPH_RUN_DESCRIPTION glyphRunDescription, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        new void DrawImage(ID2D1Image image, ref D2D1_POINT_2F targetOffset, ref D2D1_RECT_F imageRectangle, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_COMPOSITE_MODE compositeMode);
        new void DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_POINT_2F targetOffset);
        new void DrawBitmap(ID2D1Bitmap bitmap, ref D2D1_RECT_F destinationRectangle, float opacity, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_RECT_F sourceRectangle, D2D1_MATRIX_4X4_F perspectiveTransform);
        new void PushLayer(D2D1_LAYER_PARAMETERS1 layerParameters, ID2D1Layer layer);
        new HRESULT InvalidateEffectInputRectangle(ID2D1Effect effect, uint input, D2D1_RECT_F inputRectangle);
        new HRESULT GetEffectInvalidRectangleCount(ID2D1Effect effect, out uint rectangleCount);
        //new  HRESULT GetEffectInvalidRectangles(ID2D1Effect effect, out D2D1_RECT_F* rectangles, uint rectanglesCount);
        new HRESULT GetEffectInvalidRectangles(ID2D1Effect effect, out IntPtr rectangles, uint rectanglesCount);
        new HRESULT GetEffectRequiredInputRectangles(ID2D1Effect renderEffect, D2D1_RECT_F renderImageRectangle, D2D1_EFFECT_INPUT_DESCRIPTION inputDescriptions,
            // out D2D1_RECT_F* requiredInputRects, uint inputCount);
            out IntPtr requiredInputRects, uint inputCount);
        new void FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, ref D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        #endregion

        new HRESULT CreateFilledGeometryRealization(ID2D1Geometry geometry, float flatteningTolerance, out ID2D1GeometryRealization geometryRealization);
        new HRESULT CreateStrokedGeometryRealization(ID2D1Geometry geometry, float flatteningTolerance, float strokeWidth, ID2D1StrokeStyle strokeStyle, out ID2D1GeometryRealization geometryRealization);
        new void DrawGeometryRealization(ID2D1GeometryRealization geometryRealization, ID2D1Brush brush);
        #endregion

        HRESULT CreateInk(D2D1_INK_POINT startPoint, out ID2D1Ink ink);
        HRESULT CreateInkStyle(D2D1_INK_STYLE_PROPERTIES inkStyleProperties, out ID2D1InkStyle inkStyle);
        HRESULT CreateGradientMesh(D2D1_GRADIENT_MESH_PATCH patches, uint patchesCount, out ID2D1GradientMesh gradientMesh);
        HRESULT CreateImageSourceFromWic(IWICBitmapSource wicBitmapSource, D2D1_IMAGE_SOURCE_LOADING_OPTIONS loadingOptions, D2D1_ALPHA_MODE alphaMode, out ID2D1ImageSourceFromWic imageSource);
        HRESULT CreateLookupTable3D(D2D1_BUFFER_PRECISION precision, uint extents, IntPtr data, uint dataCount, uint strides, out ID2D1LookupTable3D lookupTable);
        //HRESULT CreateImageSourceFromDxgi(IDXGISurface** surfaces, uint surfaceCount, DXGI_COLOR_SPACE_TYPE colorSpace, D2D1_IMAGE_SOURCE_FROM_DXGI_OPTIONS options, out ID2D1ImageSource** imageSource);
        HRESULT CreateImageSourceFromDxgi(IntPtr surfaces, uint surfaceCount, DXGI_COLOR_SPACE_TYPE colorSpace, D2D1_IMAGE_SOURCE_FROM_DXGI_OPTIONS options, out ID2D1ImageSource imageSource);
        HRESULT GetGradientMeshWorldBounds(ID2D1GradientMesh gradientMesh, out D2D1_RECT_F pBounds);
        void DrawInk(ID2D1Ink ink, ID2D1Brush brush, ID2D1InkStyle inkStyle);
        void DrawGradientMesh(ID2D1GradientMesh gradientMesh);
        void DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_RECT_F destinationRectangle, ref D2D1_RECT_F sourceRectangle);
        HRESULT CreateTransformedImageSource(ID2D1ImageSource imageSource, D2D1_TRANSFORMED_IMAGE_SOURCE_PROPERTIES properties, out ID2D1TransformedImageSource transformedImageSource);
    }

    [ComImport]
    [Guid("235a7496-8351-414c-bcd4-6672ab2d8e00")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1DeviceContext3 : ID2D1DeviceContext2
    {
        #region <ID2D1DeviceContext2>

        #region <ID2D1DeviceContext1>
        #region <ID2D1DeviceContext>
        #region <ID2D1RenderTarget>

        #region <ID2D1Resource>

        new void GetFactory(out ID2D1Factory factory);

        #endregion
        new void CreateBitmap(D2D1_SIZE_U size, IntPtr srcData, uint pitch, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new HRESULT CreateBitmapFromWicBitmap(IWICBitmapSource wicBitmapSource, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new void CreateSharedBitmap(ref Guid riid, [In, Out] IntPtr data, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new void CreateBitmapBrush(ID2D1Bitmap bitmap, ref D2D1_BITMAP_BRUSH_PROPERTIES bitmapBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1BitmapBrush bitmapBrush);
        new HRESULT CreateSolidColorBrush(D2D1_COLOR_F color, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1SolidColorBrush solidColorBrush);
        new void CreateGradientStopCollection(D2D1_GRADIENT_STOP gradientStops, uint gradientStopsCount, D2D1_GAMMA colorInterpolationGamma, D2D1_EXTEND_MODE extendMode, out ID2D1GradientStopCollection gradientStopCollection);
        new void CreateLinearGradientBrush(D2D1_LINEAR_GRADIENT_BRUSH_PROPERTIES linearGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1LinearGradientBrush linearGradientBrush);
        new void CreateRadialGradientBrush(D2D1_RADIAL_GRADIENT_BRUSH_PROPERTIES radialGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1RadialGradientBrush radialGradientBrush);
        new void CreateCompatibleRenderTarget(D2D1_SIZE_F desiredSize, D2D1_SIZE_U desiredPixelSize, D2D1_PIXEL_FORMAT desiredFormat, D2D1_COMPATIBLE_RENDER_TARGET_OPTIONS options, out ID2D1BitmapRenderTarget bitmapRenderTarget);
        new void CreateLayer(D2D1_SIZE_F size, out ID2D1Layer layer);
        new void CreateMesh(out ID2D1Mesh mesh);
        new void DrawLine(ref D2D1_POINT_2F point0, ref D2D1_POINT_2F point1, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void DrawRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush);
        new void DrawRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush);
        new void DrawEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush);
        new void DrawGeometry(ID2D1Geometry geometry, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillGeometry(ID2D1Geometry geometry, ID2D1Brush brush, ID2D1Brush opacityBrush = null);
        new void FillMesh(ID2D1Mesh mesh, ID2D1Brush brush);
        new void FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, D2D1_OPACITY_MASK_CONTENT content, D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        new void DrawBitmap(ID2D1Bitmap bitmap, ref D2D1_RECT_F destinationRectangle, float opacity, D2D1_BITMAP_INTERPOLATION_MODE interpolationMode, ref D2D1_RECT_F sourceRectangle);
        new void DrawText(string str, uint stringLength, IDWriteTextFormat textFormat, D2D1_RECT_F layoutRect, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options, DWRITE_MEASURING_MODE measuringMode);
        new void DrawTextLayout(ref D2D1_POINT_2F origin, IDWriteTextLayout textLayout, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options);
        new void DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        new void SetTransform(D2D1_MATRIX_3X2_F transform);
        new void GetTransform(out D2D1_MATRIX_3X2_F transform);
        new void SetAntialiasMode(D2D1_ANTIALIAS_MODE antialiasMode);
        new D2D1_ANTIALIAS_MODE GetAntialiasMode();
        new void SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode);
        new D2D1_TEXT_ANTIALIAS_MODE GetTextAntialiasMode();
        new void SetTextRenderingParams(IDWriteRenderingParams textRenderingParams = null);
        new void GetTextRenderingParams(out IDWriteRenderingParams textRenderingParams);
        new void SetTags(UInt64 tag1, UInt64 tag2);
        new void GetTags(out UInt64 tag1, out UInt64 tag2);
        new void PushLayer(D2D1_LAYER_PARAMETERS layerParameters, ID2D1Layer layer);
        new void PopLayer();
        new void Flush(out UInt64 tag1, out UInt64 tag2);
        new void SaveDrawingState([In, Out] ID2D1DrawingStateBlock drawingStateBlock);
        new void RestoreDrawingState(ID2D1DrawingStateBlock drawingStateBlock);
        new void PushAxisAlignedClip(D2D1_RECT_F clipRect, D2D1_ANTIALIAS_MODE antialiasMode);
        new void PopAxisAlignedClip();
        new void Clear(D2D1_COLOR_F clearColor);
        new void BeginDraw();
        new HRESULT EndDraw(out UInt64 tag1, out UInt64 tag2);
        new D2D1_PIXEL_FORMAT GetPixelFormat();
        new void SetDpi(float dpiX, float dpiY);
        new void GetDpi(out float dpiX, out float dpiY);
        new D2D1_SIZE_F GetSize();
        new D2D1_SIZE_U GetPixelSize();
        new uint GetMaximumBitmapSize();
        new bool IsSupported(D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties);
        #endregion

        new HRESULT CreateBitmap(D2D1_SIZE_U size, IntPtr sourceData, uint pitch, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        new HRESULT CreateBitmapFromWicBitmap(IWICBitmapSource wicBitmapSource, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        new HRESULT CreateColorContext(D2D1_COLOR_SPACE space, IntPtr profile, uint profileSize, out ID2D1ColorContext colorContext);
        new HRESULT CreateColorContextFromFilename(string filename, out ID2D1ColorContext colorContext);
        new HRESULT CreateColorContextFromWicColorContext(IWICColorContext wicColorContext, out ID2D1ColorContext colorContext);
        new HRESULT CreateBitmapFromDxgiSurface(IDXGISurface surface, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        new HRESULT CreateEffect(ref Guid effectId, out ID2D1Effect effect);
        new HRESULT CreateGradientStopCollection(D2D1_GRADIENT_STOP straightAlphaGradientStops, uint straightAlphaGradientStopsCount, D2D1_COLOR_SPACE preInterpolationSpace,
            D2D1_COLOR_SPACE postInterpolationSpace, D2D1_BUFFER_PRECISION bufferPrecision, D2D1_EXTEND_MODE extendMode, D2D1_COLOR_INTERPOLATION_MODE colorInterpolationMode,
            out ID2D1GradientStopCollection1 gradientStopCollection1);
        new HRESULT CreateImageBrush(ID2D1Image image, ref D2D1_IMAGE_BRUSH_PROPERTIES imageBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1ImageBrush imageBrush);
        new HRESULT CreateBitmapBrush(ID2D1Bitmap bitmap, ref D2D1_BITMAP_BRUSH_PROPERTIES1 bitmapBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1BitmapBrush1 bitmapBrush);
        new HRESULT CreateCommandList(out ID2D1CommandList commandList);
        new bool IsDxgiFormatSupported(DXGI_FORMAT format);
        new bool IsBufferPrecisionSupported(D2D1_BUFFER_PRECISION bufferPrecision);
        new HRESULT GetImageLocalBounds(ID2D1Image image, out D2D1_RECT_F localBounds);
        new HRESULT GetImageWorldBounds(ID2D1Image image, out D2D1_RECT_F worldBounds);
        new HRESULT GetGlyphRunWorldBounds(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, DWRITE_MEASURING_MODE measuringMode, out D2D1_RECT_F bounds);
        new void GetDevice(out ID2D1Device device);
        new void SetTarget(ID2D1Image image);
        new void GetTarget(out ID2D1Image image);
        new void SetRenderingControls(D2D1_RENDERING_CONTROLS renderingControls);
        new void GetRenderingControls(out D2D1_RENDERING_CONTROLS renderingControls);
        new void SetPrimitiveBlend(D2D1_PRIMITIVE_BLEND primitiveBlend);
        new D2D1_PRIMITIVE_BLEND GetPrimitiveBlend();
        new void SetUnitMode(D2D1_UNIT_MODE unitMode);
        new D2D1_UNIT_MODE GetUnitMode();
        new void DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, DWRITE_GLYPH_RUN_DESCRIPTION glyphRunDescription, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        new void DrawImage(ID2D1Image image, ref D2D1_POINT_2F targetOffset, ref D2D1_RECT_F imageRectangle, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_COMPOSITE_MODE compositeMode);
        new void DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_POINT_2F targetOffset);
        new void DrawBitmap(ID2D1Bitmap bitmap, ref D2D1_RECT_F destinationRectangle, float opacity, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_RECT_F sourceRectangle, D2D1_MATRIX_4X4_F perspectiveTransform);
        new void PushLayer(D2D1_LAYER_PARAMETERS1 layerParameters, ID2D1Layer layer);
        new HRESULT InvalidateEffectInputRectangle(ID2D1Effect effect, uint input, D2D1_RECT_F inputRectangle);
        new HRESULT GetEffectInvalidRectangleCount(ID2D1Effect effect, out uint rectangleCount);
        //new  HRESULT GetEffectInvalidRectangles(ID2D1Effect effect, out D2D1_RECT_F* rectangles, uint rectanglesCount);
        new HRESULT GetEffectInvalidRectangles(ID2D1Effect effect, out IntPtr rectangles, uint rectanglesCount);
        new HRESULT GetEffectRequiredInputRectangles(ID2D1Effect renderEffect, D2D1_RECT_F renderImageRectangle, D2D1_EFFECT_INPUT_DESCRIPTION inputDescriptions,
            // out D2D1_RECT_F* requiredInputRects, uint inputCount);
            out IntPtr requiredInputRects, uint inputCount);
        new void FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, ref D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        #endregion

        new HRESULT CreateFilledGeometryRealization(ID2D1Geometry geometry, float flatteningTolerance, out ID2D1GeometryRealization geometryRealization);
        new HRESULT CreateStrokedGeometryRealization(ID2D1Geometry geometry, float flatteningTolerance, float strokeWidth, ID2D1StrokeStyle strokeStyle, out ID2D1GeometryRealization geometryRealization);
        new void DrawGeometryRealization(ID2D1GeometryRealization geometryRealization, ID2D1Brush brush);
        #endregion

        new HRESULT CreateInk(D2D1_INK_POINT startPoint, out ID2D1Ink ink);
        new HRESULT CreateInkStyle(D2D1_INK_STYLE_PROPERTIES inkStyleProperties, out ID2D1InkStyle inkStyle);
        new HRESULT CreateGradientMesh(D2D1_GRADIENT_MESH_PATCH patches, uint patchesCount, out ID2D1GradientMesh gradientMesh);
        new HRESULT CreateImageSourceFromWic(IWICBitmapSource wicBitmapSource, D2D1_IMAGE_SOURCE_LOADING_OPTIONS loadingOptions, D2D1_ALPHA_MODE alphaMode, out ID2D1ImageSourceFromWic imageSource);
        new HRESULT CreateLookupTable3D(D2D1_BUFFER_PRECISION precision, uint extents, IntPtr data, uint dataCount, uint strides, out ID2D1LookupTable3D lookupTable);
        //new HRESULT CreateImageSourceFromDxgi(IDXGISurface** surfaces, uint surfaceCount, DXGI_COLOR_SPACE_TYPE colorSpace, D2D1_IMAGE_SOURCE_FROM_DXGI_OPTIONS options, out ID2D1ImageSource** imageSource);
        new HRESULT CreateImageSourceFromDxgi(IntPtr surfaces, uint surfaceCount, DXGI_COLOR_SPACE_TYPE colorSpace, D2D1_IMAGE_SOURCE_FROM_DXGI_OPTIONS options, out ID2D1ImageSource imageSource);
        new HRESULT GetGradientMeshWorldBounds(ID2D1GradientMesh gradientMesh, out D2D1_RECT_F pBounds);
        new void DrawInk(ID2D1Ink ink, ID2D1Brush brush, ID2D1InkStyle inkStyle);
        new void DrawGradientMesh(ID2D1GradientMesh gradientMesh);
        new void DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_RECT_F destinationRectangle, ref D2D1_RECT_F sourceRectangle);
        new HRESULT CreateTransformedImageSource(ID2D1ImageSource imageSource, D2D1_TRANSFORMED_IMAGE_SOURCE_PROPERTIES properties, out ID2D1TransformedImageSource transformedImageSource);

        #endregion

        HRESULT CreateSpriteBatch(out ID2D1SpriteBatch spriteBatch);
        void DrawSpriteBatch(ID2D1SpriteBatch spriteBatch, uint startIndex, uint spriteCount, ID2D1Bitmap bitmap,
            D2D1_BITMAP_INTERPOLATION_MODE interpolationMode = D2D1_BITMAP_INTERPOLATION_MODE.D2D1_BITMAP_INTERPOLATION_MODE_LINEAR, D2D1_SPRITE_OPTIONS spriteOptions = D2D1_SPRITE_OPTIONS.D2D1_SPRITE_OPTIONS_NONE);
    }

    public enum D2D1_SPRITE_OPTIONS
    {
        /// <summary>
        /// Use default sprite rendering behavior.
        /// </summary>
        D2D1_SPRITE_OPTIONS_NONE = 0,

        /// <summary>
        /// Bitmap interpolation will be clamped to the sprite's source rectangle.
        /// </summary>
        D2D1_SPRITE_OPTIONS_CLAMP_TO_SOURCE_RECTANGLE = 1,
        D2D1_SPRITE_OPTIONS_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [ComImport]
    [Guid("4dc583bf-3a10-438a-8722-e9765224f1f1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1SpriteBatch : ID2D1Resource
    {
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        //unsafe
        //       HRESULT AddSprites(uint spriteCount,  D2D1_RECT_F* destinationRectangles, void* sourceRectangles, void* colors, void* transforms,
        //    uint destinationRectanglesStride, uint sourceRectanglesStride, uint colorsStride, uint transformsStride);

        HRESULT AddSprites(uint spriteCount, [MarshalAs(UnmanagedType.LPArray)] D2D1_RECT_F[] destinationRectangles, [MarshalAs(UnmanagedType.LPArray)] D2D1_RECT_U[] sourceRectangles,
         [MarshalAs(UnmanagedType.LPArray)] D2D1_COLOR_F_STRUCT[] colors, [MarshalAs(UnmanagedType.LPArray)] D2D1_MATRIX_3X2_F_STRUCT[] transforms,
        uint destinationRectanglesStride, uint sourceRectanglesStride, uint colorsStride, uint transformsStride);
    
        HRESULT SetSprites(uint startIndex, uint spriteCount,
           [MarshalAs(UnmanagedType.LPArray)] D2D1_RECT_F[] destinationRectangles, [MarshalAs(UnmanagedType.LPArray)] D2D1_RECT_U[] sourceRectangles,
            [MarshalAs(UnmanagedType.LPArray)] D2D1_COLOR_F_STRUCT[] colors, [MarshalAs(UnmanagedType.LPArray)] D2D1_MATRIX_3X2_F_STRUCT[] transforms,
           uint destinationRectanglesStride, uint sourceRectanglesStride, uint colorsStride, uint transformsStride);

        HRESULT GetSprites(uint startIndex, uint spriteCount, out IntPtr destinationRectangles, out IntPtr sourceRectangles, out IntPtr colors, out IntPtr transforms);

        [return: MarshalAs(UnmanagedType.U4)]
        [PreserveSig]
        uint GetSpriteCount();
        void Clear();
    }

    [ComImport]
    [Guid("8c427831-3d90-4476-b647-c4fae349e4db")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1DeviceContext4 : ID2D1DeviceContext3
    {
        #region <ID2D1DeviceContext3>

        #region <ID2D1DeviceContext2>

        #region <ID2D1DeviceContext1>
        #region <ID2D1DeviceContext>
        #region <ID2D1RenderTarget>

        #region <ID2D1Resource>

        new void GetFactory(out ID2D1Factory factory);

        #endregion
        new void CreateBitmap(D2D1_SIZE_U size, IntPtr srcData, uint pitch, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new HRESULT CreateBitmapFromWicBitmap(IWICBitmapSource wicBitmapSource, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new void CreateSharedBitmap(ref Guid riid, [In, Out] IntPtr data, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new void CreateBitmapBrush(ID2D1Bitmap bitmap, ref D2D1_BITMAP_BRUSH_PROPERTIES bitmapBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1BitmapBrush bitmapBrush);
        new HRESULT CreateSolidColorBrush(D2D1_COLOR_F color, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1SolidColorBrush solidColorBrush);
        new void CreateGradientStopCollection(D2D1_GRADIENT_STOP gradientStops, uint gradientStopsCount, D2D1_GAMMA colorInterpolationGamma, D2D1_EXTEND_MODE extendMode, out ID2D1GradientStopCollection gradientStopCollection);
        new void CreateLinearGradientBrush(D2D1_LINEAR_GRADIENT_BRUSH_PROPERTIES linearGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1LinearGradientBrush linearGradientBrush);
        new void CreateRadialGradientBrush(D2D1_RADIAL_GRADIENT_BRUSH_PROPERTIES radialGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1RadialGradientBrush radialGradientBrush);
        new void CreateCompatibleRenderTarget(D2D1_SIZE_F desiredSize, D2D1_SIZE_U desiredPixelSize, D2D1_PIXEL_FORMAT desiredFormat, D2D1_COMPATIBLE_RENDER_TARGET_OPTIONS options, out ID2D1BitmapRenderTarget bitmapRenderTarget);
        new void CreateLayer(D2D1_SIZE_F size, out ID2D1Layer layer);
        new void CreateMesh(out ID2D1Mesh mesh);
        new void DrawLine(ref D2D1_POINT_2F point0, ref D2D1_POINT_2F point1, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void DrawRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush);
        new void DrawRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush);
        new void DrawEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush);
        new void DrawGeometry(ID2D1Geometry geometry, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillGeometry(ID2D1Geometry geometry, ID2D1Brush brush, ID2D1Brush opacityBrush = null);
        new void FillMesh(ID2D1Mesh mesh, ID2D1Brush brush);
        new void FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, D2D1_OPACITY_MASK_CONTENT content, D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        new void DrawBitmap(ID2D1Bitmap bitmap, ref D2D1_RECT_F destinationRectangle, float opacity, D2D1_BITMAP_INTERPOLATION_MODE interpolationMode, ref D2D1_RECT_F sourceRectangle);
        new void DrawText(string str, uint stringLength, IDWriteTextFormat textFormat, D2D1_RECT_F layoutRect, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options, DWRITE_MEASURING_MODE measuringMode);
        new void DrawTextLayout(ref D2D1_POINT_2F origin, IDWriteTextLayout textLayout, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options);
        new void DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        new void SetTransform(D2D1_MATRIX_3X2_F transform);
        new void GetTransform(out D2D1_MATRIX_3X2_F transform);
        new void SetAntialiasMode(D2D1_ANTIALIAS_MODE antialiasMode);
        new D2D1_ANTIALIAS_MODE GetAntialiasMode();
        new void SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode);
        new D2D1_TEXT_ANTIALIAS_MODE GetTextAntialiasMode();
        new void SetTextRenderingParams(IDWriteRenderingParams textRenderingParams = null);
        new void GetTextRenderingParams(out IDWriteRenderingParams textRenderingParams);
        new void SetTags(UInt64 tag1, UInt64 tag2);
        new void GetTags(out UInt64 tag1, out UInt64 tag2);
        new void PushLayer(D2D1_LAYER_PARAMETERS layerParameters, ID2D1Layer layer);
        new void PopLayer();
        new void Flush(out UInt64 tag1, out UInt64 tag2);
        new void SaveDrawingState([In, Out] ID2D1DrawingStateBlock drawingStateBlock);
        new void RestoreDrawingState(ID2D1DrawingStateBlock drawingStateBlock);
        new void PushAxisAlignedClip(D2D1_RECT_F clipRect, D2D1_ANTIALIAS_MODE antialiasMode);
        new void PopAxisAlignedClip();
        new void Clear(D2D1_COLOR_F clearColor);
        new void BeginDraw();
        new HRESULT EndDraw(out UInt64 tag1, out UInt64 tag2);
        new D2D1_PIXEL_FORMAT GetPixelFormat();
        new void SetDpi(float dpiX, float dpiY);
        new void GetDpi(out float dpiX, out float dpiY);
        new D2D1_SIZE_F GetSize();
        new D2D1_SIZE_U GetPixelSize();
        new uint GetMaximumBitmapSize();
        new bool IsSupported(D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties);
        #endregion

        new HRESULT CreateBitmap(D2D1_SIZE_U size, IntPtr sourceData, uint pitch, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        new HRESULT CreateBitmapFromWicBitmap(IWICBitmapSource wicBitmapSource, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        new HRESULT CreateColorContext(D2D1_COLOR_SPACE space, IntPtr profile, uint profileSize, out ID2D1ColorContext colorContext);
        new HRESULT CreateColorContextFromFilename(string filename, out ID2D1ColorContext colorContext);
        new HRESULT CreateColorContextFromWicColorContext(IWICColorContext wicColorContext, out ID2D1ColorContext colorContext);
        new HRESULT CreateBitmapFromDxgiSurface(IDXGISurface surface, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        new HRESULT CreateEffect(ref Guid effectId, out ID2D1Effect effect);
        new HRESULT CreateGradientStopCollection(D2D1_GRADIENT_STOP straightAlphaGradientStops, uint straightAlphaGradientStopsCount, D2D1_COLOR_SPACE preInterpolationSpace,
            D2D1_COLOR_SPACE postInterpolationSpace, D2D1_BUFFER_PRECISION bufferPrecision, D2D1_EXTEND_MODE extendMode, D2D1_COLOR_INTERPOLATION_MODE colorInterpolationMode,
            out ID2D1GradientStopCollection1 gradientStopCollection1);
        new HRESULT CreateImageBrush(ID2D1Image image, ref D2D1_IMAGE_BRUSH_PROPERTIES imageBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1ImageBrush imageBrush);
        new HRESULT CreateBitmapBrush(ID2D1Bitmap bitmap, ref D2D1_BITMAP_BRUSH_PROPERTIES1 bitmapBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1BitmapBrush1 bitmapBrush);
        new HRESULT CreateCommandList(out ID2D1CommandList commandList);
        new bool IsDxgiFormatSupported(DXGI_FORMAT format);
        new bool IsBufferPrecisionSupported(D2D1_BUFFER_PRECISION bufferPrecision);
        new HRESULT GetImageLocalBounds(ID2D1Image image, out D2D1_RECT_F localBounds);
        new HRESULT GetImageWorldBounds(ID2D1Image image, out D2D1_RECT_F worldBounds);
        new HRESULT GetGlyphRunWorldBounds(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, DWRITE_MEASURING_MODE measuringMode, out D2D1_RECT_F bounds);
        new void GetDevice(out ID2D1Device device);
        new void SetTarget(ID2D1Image image);
        new void GetTarget(out ID2D1Image image);
        new void SetRenderingControls(D2D1_RENDERING_CONTROLS renderingControls);
        new void GetRenderingControls(out D2D1_RENDERING_CONTROLS renderingControls);
        new void SetPrimitiveBlend(D2D1_PRIMITIVE_BLEND primitiveBlend);
        new D2D1_PRIMITIVE_BLEND GetPrimitiveBlend();
        new void SetUnitMode(D2D1_UNIT_MODE unitMode);
        new D2D1_UNIT_MODE GetUnitMode();
        new void DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, DWRITE_GLYPH_RUN_DESCRIPTION glyphRunDescription, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        new void DrawImage(ID2D1Image image, ref D2D1_POINT_2F targetOffset, ref D2D1_RECT_F imageRectangle, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_COMPOSITE_MODE compositeMode);
        new void DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_POINT_2F targetOffset);
        new void DrawBitmap(ID2D1Bitmap bitmap, ref D2D1_RECT_F destinationRectangle, float opacity, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_RECT_F sourceRectangle, D2D1_MATRIX_4X4_F perspectiveTransform);
        new void PushLayer(D2D1_LAYER_PARAMETERS1 layerParameters, ID2D1Layer layer);
        new HRESULT InvalidateEffectInputRectangle(ID2D1Effect effect, uint input, D2D1_RECT_F inputRectangle);
        new HRESULT GetEffectInvalidRectangleCount(ID2D1Effect effect, out uint rectangleCount);
        //new  HRESULT GetEffectInvalidRectangles(ID2D1Effect effect, out D2D1_RECT_F* rectangles, uint rectanglesCount);
        new HRESULT GetEffectInvalidRectangles(ID2D1Effect effect, out IntPtr rectangles, uint rectanglesCount);
        new HRESULT GetEffectRequiredInputRectangles(ID2D1Effect renderEffect, D2D1_RECT_F renderImageRectangle, D2D1_EFFECT_INPUT_DESCRIPTION inputDescriptions,
            // out D2D1_RECT_F* requiredInputRects, uint inputCount);
            out IntPtr requiredInputRects, uint inputCount);
        new void FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, ref D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        #endregion

        new HRESULT CreateFilledGeometryRealization(ID2D1Geometry geometry, float flatteningTolerance, out ID2D1GeometryRealization geometryRealization);
        new HRESULT CreateStrokedGeometryRealization(ID2D1Geometry geometry, float flatteningTolerance, float strokeWidth, ID2D1StrokeStyle strokeStyle, out ID2D1GeometryRealization geometryRealization);
        new void DrawGeometryRealization(ID2D1GeometryRealization geometryRealization, ID2D1Brush brush);
        #endregion

        new HRESULT CreateInk(D2D1_INK_POINT startPoint, out ID2D1Ink ink);
        new HRESULT CreateInkStyle(D2D1_INK_STYLE_PROPERTIES inkStyleProperties, out ID2D1InkStyle inkStyle);
        new HRESULT CreateGradientMesh(D2D1_GRADIENT_MESH_PATCH patches, uint patchesCount, out ID2D1GradientMesh gradientMesh);
        new HRESULT CreateImageSourceFromWic(IWICBitmapSource wicBitmapSource, D2D1_IMAGE_SOURCE_LOADING_OPTIONS loadingOptions, D2D1_ALPHA_MODE alphaMode, out ID2D1ImageSourceFromWic imageSource);
        new HRESULT CreateLookupTable3D(D2D1_BUFFER_PRECISION precision, uint extents, IntPtr data, uint dataCount, uint strides, out ID2D1LookupTable3D lookupTable);
        //new HRESULT CreateImageSourceFromDxgi(IDXGISurface** surfaces, uint surfaceCount, DXGI_COLOR_SPACE_TYPE colorSpace, D2D1_IMAGE_SOURCE_FROM_DXGI_OPTIONS options, out ID2D1ImageSource** imageSource);
        new HRESULT CreateImageSourceFromDxgi(IntPtr surfaces, uint surfaceCount, DXGI_COLOR_SPACE_TYPE colorSpace, D2D1_IMAGE_SOURCE_FROM_DXGI_OPTIONS options, out ID2D1ImageSource imageSource);
        new HRESULT GetGradientMeshWorldBounds(ID2D1GradientMesh gradientMesh, out D2D1_RECT_F pBounds);
        new void DrawInk(ID2D1Ink ink, ID2D1Brush brush, ID2D1InkStyle inkStyle);
        new void DrawGradientMesh(ID2D1GradientMesh gradientMesh);
        new void DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_RECT_F destinationRectangle, ref D2D1_RECT_F sourceRectangle);
        new HRESULT CreateTransformedImageSource(ID2D1ImageSource imageSource, D2D1_TRANSFORMED_IMAGE_SOURCE_PROPERTIES properties, out ID2D1TransformedImageSource transformedImageSource);

        #endregion

        new HRESULT CreateSpriteBatch(out ID2D1SpriteBatch spriteBatch);
        new void DrawSpriteBatch(ID2D1SpriteBatch spriteBatch, uint startIndex, uint spriteCount, ID2D1Bitmap bitmap,
            D2D1_BITMAP_INTERPOLATION_MODE interpolationMode = D2D1_BITMAP_INTERPOLATION_MODE.D2D1_BITMAP_INTERPOLATION_MODE_LINEAR, D2D1_SPRITE_OPTIONS spriteOptions = D2D1_SPRITE_OPTIONS.D2D1_SPRITE_OPTIONS_NONE);

        #endregion

        HRESULT CreateSvgGlyphStyle(out ID2D1SvgGlyphStyle svgGlyphStyle);
        void DrawText(string sString, uint stringLength, IDWriteTextFormat textFormat, D2D1_RECT_F layoutRect, ID2D1Brush defaultFillBrush,
            ID2D1SvgGlyphStyle svgGlyphStyle, uint colorPaletteIndex = 0,
            D2D1_DRAW_TEXT_OPTIONS options = D2D1_DRAW_TEXT_OPTIONS.D2D1_DRAW_TEXT_OPTIONS_ENABLE_COLOR_FONT,
            DWRITE_MEASURING_MODE measuringMode = DWRITE_MEASURING_MODE.DWRITE_MEASURING_MODE_NATURAL);
        void DrawTextLayout(ref D2D1_POINT_2F origin,
            IDWriteTextLayout textLayout,
            ID2D1Brush defaultFillBrush,
            ID2D1SvgGlyphStyle svgGlyphStyle,
            uint colorPaletteIndex = 0,
            D2D1_DRAW_TEXT_OPTIONS options = D2D1_DRAW_TEXT_OPTIONS.D2D1_DRAW_TEXT_OPTIONS_ENABLE_COLOR_FONT);
        void DrawColorBitmapGlyphRun(
            DWRITE_GLYPH_IMAGE_FORMATS glyphImageFormat,
            ref D2D1_POINT_2F baselineOrigin,
            DWRITE_GLYPH_RUN glyphRun,
            DWRITE_MEASURING_MODE measuringMode = DWRITE_MEASURING_MODE.DWRITE_MEASURING_MODE_NATURAL,
            D2D1_COLOR_BITMAP_GLYPH_SNAP_OPTION bitmapSnapOption = D2D1_COLOR_BITMAP_GLYPH_SNAP_OPTION.D2D1_COLOR_BITMAP_GLYPH_SNAP_OPTION_DEFAULT);
        void DrawSvgGlyphRun(
            ref D2D1_POINT_2F baselineOrigin,
            DWRITE_GLYPH_RUN glyphRun,
            ID2D1Brush defaultFillBrush = null,
            ID2D1SvgGlyphStyle svgGlyphStyle = null,
            uint colorPaletteIndex = 0,
            DWRITE_MEASURING_MODE measuringMode = DWRITE_MEASURING_MODE.DWRITE_MEASURING_MODE_NATURAL);
        HRESULT GetColorBitmapGlyphImage(
            DWRITE_GLYPH_IMAGE_FORMATS glyphImageFormat,
            ref D2D1_POINT_2F glyphOrigin,
            IDWriteFontFace fontFace,
            float fontEmSize,
            UInt16 glyphIndex,
            bool isSideways,
            D2D1_MATRIX_3X2_F worldTransform,
            float dpiX,
            float dpiY,
            out D2D1_MATRIX_3X2_F glyphTransform,
            out ID2D1Image glyphImage);
        HRESULT GetSvgGlyphImage(
            ref D2D1_POINT_2F glyphOrigin,
            IDWriteFontFace fontFace,
            float fontEmSize,
            UInt16 glyphIndex,
            bool isSideways,
            D2D1_MATRIX_3X2_F worldTransform,
            ID2D1Brush defaultFillBrush,
            ID2D1SvgGlyphStyle svgGlyphStyle,
            uint colorPaletteIndex,
            out D2D1_MATRIX_3X2_F glyphTransform,
            out ID2D1CommandList glyphImage);
    }

    [ComImport]
    [Guid("af671749-d241-4db8-8e41-dcc2e5c1a438")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1SvgGlyphStyle : ID2D1Resource
    {
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        HRESULT SetFill(ID2D1Brush brush);
        void GetFill(out ID2D1Brush brush);
        HRESULT SetStroke(ID2D1Brush brush, float strokeWidth = 1.0f, float dashes = 0.0f, uint dashesCount = 0, float dashOffset = 1.0f);
        uint GetStrokeDashesCount();
        void GetStroke(out ID2D1Brush brush, out float strokeWidth, out float dashes, uint dashesCount, out float dashOffset);
    }

    /// <summary>
    /// Specifies the pixel snapping policy when rendering color bitmap glyphs.
    /// </summary>
    public enum D2D1_COLOR_BITMAP_GLYPH_SNAP_OPTION
    {
        /// <summary>
        /// Color bitmap glyph positions are snapped to the nearest pixel if the bitmap
        /// resolution matches that of the device context.
        /// </summary>
        D2D1_COLOR_BITMAP_GLYPH_SNAP_OPTION_DEFAULT = 0,

        /// <summary>
        /// Color bitmap glyph positions are not snapped.
        /// </summary>
        D2D1_COLOR_BITMAP_GLYPH_SNAP_OPTION_DISABLE = 1,
        D2D1_COLOR_BITMAP_GLYPH_SNAP_OPTION_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// Fonts may contain multiple drawable data formats for glyphs. These flags specify which formats
    /// are supported in the font, either at a font-wide level or per glyph, and the app may use them
    /// to tell DWrite which formats to return when splitting a color glyph run.
    /// </summary>
    public enum DWRITE_GLYPH_IMAGE_FORMATS
    {
        /// <summary>
        /// Indicates no data is available for this glyph.
        /// </summary>
        DWRITE_GLYPH_IMAGE_FORMATS_NONE = 0x00000000,

        /// <summary>
        /// The glyph has TrueType outlines.
        /// </summary>
        DWRITE_GLYPH_IMAGE_FORMATS_TRUETYPE = 0x00000001,

        /// <summary>
        /// The glyph has CFF outlines.
        /// </summary>
        DWRITE_GLYPH_IMAGE_FORMATS_CFF = 0x00000002,

        /// <summary>
        /// The glyph has multilayered COLR data.
        /// </summary>
        DWRITE_GLYPH_IMAGE_FORMATS_COLR = 0x00000004,

        /// <summary>
        /// The glyph has SVG outlines as standard XML.
        /// </summary>
        /// <remarks>
        /// Fonts may store the content gzip'd rather than plain text,
        /// indicated by the first two bytes as gzip header {0x1F 0x8B}.
        /// </remarks>
        DWRITE_GLYPH_IMAGE_FORMATS_SVG = 0x00000008,

        /// <summary>
        /// The glyph has PNG image data, with standard PNG IHDR.
        /// </summary>
        DWRITE_GLYPH_IMAGE_FORMATS_PNG = 0x00000010,

        /// <summary>
        /// The glyph has JPEG image data, with standard JIFF SOI header.
        /// </summary>
        DWRITE_GLYPH_IMAGE_FORMATS_JPEG = 0x00000020,

        /// <summary>
        /// The glyph has TIFF image data.
        /// </summary>
        DWRITE_GLYPH_IMAGE_FORMATS_TIFF = 0x00000040,

        /// <summary>
        /// The glyph has raw 32-bit premultiplied BGRA data.
        /// </summary>
        DWRITE_GLYPH_IMAGE_FORMATS_PREMULTIPLIED_B8G8R8A8 = 0x00000080,
    };


    [ComImport]
    [Guid("7836d248-68cc-4df6-b9e8-de991bf62eb7")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1DeviceContext5 : ID2D1DeviceContext4
    {
        #region <ID2D1DeviceContext4>

        #region <ID2D1DeviceContext3>

        #region <ID2D1DeviceContext2>

        #region <ID2D1DeviceContext1>
        #region <ID2D1DeviceContext>
        #region <ID2D1RenderTarget>

        #region <ID2D1Resource>

        new void GetFactory(out ID2D1Factory factory);

        #endregion
        new void CreateBitmap(D2D1_SIZE_U size, IntPtr srcData, uint pitch, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new HRESULT CreateBitmapFromWicBitmap(IWICBitmapSource wicBitmapSource, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new void CreateSharedBitmap(ref Guid riid, [In, Out] IntPtr data, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new void CreateBitmapBrush(ID2D1Bitmap bitmap, ref D2D1_BITMAP_BRUSH_PROPERTIES bitmapBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1BitmapBrush bitmapBrush);
        new HRESULT CreateSolidColorBrush(D2D1_COLOR_F color, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1SolidColorBrush solidColorBrush);
        new void CreateGradientStopCollection(D2D1_GRADIENT_STOP gradientStops, uint gradientStopsCount, D2D1_GAMMA colorInterpolationGamma, D2D1_EXTEND_MODE extendMode, out ID2D1GradientStopCollection gradientStopCollection);
        new void CreateLinearGradientBrush(D2D1_LINEAR_GRADIENT_BRUSH_PROPERTIES linearGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1LinearGradientBrush linearGradientBrush);
        new void CreateRadialGradientBrush(D2D1_RADIAL_GRADIENT_BRUSH_PROPERTIES radialGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1RadialGradientBrush radialGradientBrush);
        new void CreateCompatibleRenderTarget(D2D1_SIZE_F desiredSize, D2D1_SIZE_U desiredPixelSize, D2D1_PIXEL_FORMAT desiredFormat, D2D1_COMPATIBLE_RENDER_TARGET_OPTIONS options, out ID2D1BitmapRenderTarget bitmapRenderTarget);
        new void CreateLayer(D2D1_SIZE_F size, out ID2D1Layer layer);
        new void CreateMesh(out ID2D1Mesh mesh);
        new void DrawLine(ref D2D1_POINT_2F point0, ref D2D1_POINT_2F point1, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void DrawRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush);
        new void DrawRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush);
        new void DrawEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush);
        new void DrawGeometry(ID2D1Geometry geometry, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillGeometry(ID2D1Geometry geometry, ID2D1Brush brush, ID2D1Brush opacityBrush = null);
        new void FillMesh(ID2D1Mesh mesh, ID2D1Brush brush);
        new void FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, D2D1_OPACITY_MASK_CONTENT content, D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        new void DrawBitmap(ID2D1Bitmap bitmap, ref D2D1_RECT_F destinationRectangle, float opacity, D2D1_BITMAP_INTERPOLATION_MODE interpolationMode, ref D2D1_RECT_F sourceRectangle);
        new void DrawText(string str, uint stringLength, IDWriteTextFormat textFormat, D2D1_RECT_F layoutRect, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options, DWRITE_MEASURING_MODE measuringMode);
        new void DrawTextLayout(ref D2D1_POINT_2F origin, IDWriteTextLayout textLayout, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options);
        new void DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        new void SetTransform(D2D1_MATRIX_3X2_F transform);
        new void GetTransform(out D2D1_MATRIX_3X2_F transform);
        new void SetAntialiasMode(D2D1_ANTIALIAS_MODE antialiasMode);
        new D2D1_ANTIALIAS_MODE GetAntialiasMode();
        new void SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode);
        new D2D1_TEXT_ANTIALIAS_MODE GetTextAntialiasMode();
        new void SetTextRenderingParams(IDWriteRenderingParams textRenderingParams = null);
        new void GetTextRenderingParams(out IDWriteRenderingParams textRenderingParams);
        new void SetTags(UInt64 tag1, UInt64 tag2);
        new void GetTags(out UInt64 tag1, out UInt64 tag2);
        new void PushLayer(D2D1_LAYER_PARAMETERS layerParameters, ID2D1Layer layer);
        new void PopLayer();
        new void Flush(out UInt64 tag1, out UInt64 tag2);
        new void SaveDrawingState([In, Out] ID2D1DrawingStateBlock drawingStateBlock);
        new void RestoreDrawingState(ID2D1DrawingStateBlock drawingStateBlock);
        new void PushAxisAlignedClip(D2D1_RECT_F clipRect, D2D1_ANTIALIAS_MODE antialiasMode);
        new void PopAxisAlignedClip();
        new void Clear(D2D1_COLOR_F clearColor);
        new void BeginDraw();
        new HRESULT EndDraw(out UInt64 tag1, out UInt64 tag2);
        new D2D1_PIXEL_FORMAT GetPixelFormat();
        new void SetDpi(float dpiX, float dpiY);
        new void GetDpi(out float dpiX, out float dpiY);
        new D2D1_SIZE_F GetSize();
        new D2D1_SIZE_U GetPixelSize();
        new uint GetMaximumBitmapSize();
        new bool IsSupported(D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties);
        #endregion

        new HRESULT CreateBitmap(D2D1_SIZE_U size, IntPtr sourceData, uint pitch, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        new HRESULT CreateBitmapFromWicBitmap(IWICBitmapSource wicBitmapSource, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        new HRESULT CreateColorContext(D2D1_COLOR_SPACE space, IntPtr profile, uint profileSize, out ID2D1ColorContext colorContext);
        new HRESULT CreateColorContextFromFilename(string filename, out ID2D1ColorContext colorContext);
        new HRESULT CreateColorContextFromWicColorContext(IWICColorContext wicColorContext, out ID2D1ColorContext colorContext);
        new HRESULT CreateBitmapFromDxgiSurface(IDXGISurface surface, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        new HRESULT CreateEffect(ref Guid effectId, out ID2D1Effect effect);
        new HRESULT CreateGradientStopCollection(D2D1_GRADIENT_STOP straightAlphaGradientStops, uint straightAlphaGradientStopsCount, D2D1_COLOR_SPACE preInterpolationSpace,
            D2D1_COLOR_SPACE postInterpolationSpace, D2D1_BUFFER_PRECISION bufferPrecision, D2D1_EXTEND_MODE extendMode, D2D1_COLOR_INTERPOLATION_MODE colorInterpolationMode,
            out ID2D1GradientStopCollection1 gradientStopCollection1);
        new HRESULT CreateImageBrush(ID2D1Image image, ref D2D1_IMAGE_BRUSH_PROPERTIES imageBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1ImageBrush imageBrush);
        new HRESULT CreateBitmapBrush(ID2D1Bitmap bitmap, ref D2D1_BITMAP_BRUSH_PROPERTIES1 bitmapBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1BitmapBrush1 bitmapBrush);
        new HRESULT CreateCommandList(out ID2D1CommandList commandList);
        new bool IsDxgiFormatSupported(DXGI_FORMAT format);
        new bool IsBufferPrecisionSupported(D2D1_BUFFER_PRECISION bufferPrecision);
        new HRESULT GetImageLocalBounds(ID2D1Image image, out D2D1_RECT_F localBounds);
        new HRESULT GetImageWorldBounds(ID2D1Image image, out D2D1_RECT_F worldBounds);
        new HRESULT GetGlyphRunWorldBounds(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, DWRITE_MEASURING_MODE measuringMode, out D2D1_RECT_F bounds);
        new void GetDevice(out ID2D1Device device);
        new void SetTarget(ID2D1Image image);
        new void GetTarget(out ID2D1Image image);
        new void SetRenderingControls(D2D1_RENDERING_CONTROLS renderingControls);
        new void GetRenderingControls(out D2D1_RENDERING_CONTROLS renderingControls);
        new void SetPrimitiveBlend(D2D1_PRIMITIVE_BLEND primitiveBlend);
        new D2D1_PRIMITIVE_BLEND GetPrimitiveBlend();
        new void SetUnitMode(D2D1_UNIT_MODE unitMode);
        new D2D1_UNIT_MODE GetUnitMode();
        new void DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, DWRITE_GLYPH_RUN_DESCRIPTION glyphRunDescription, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        new void DrawImage(ID2D1Image image, ref D2D1_POINT_2F targetOffset, ref D2D1_RECT_F imageRectangle, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_COMPOSITE_MODE compositeMode);
        new void DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_POINT_2F targetOffset);
        new void DrawBitmap(ID2D1Bitmap bitmap, ref D2D1_RECT_F destinationRectangle, float opacity, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_RECT_F sourceRectangle, D2D1_MATRIX_4X4_F perspectiveTransform);
        new void PushLayer(D2D1_LAYER_PARAMETERS1 layerParameters, ID2D1Layer layer);
        new HRESULT InvalidateEffectInputRectangle(ID2D1Effect effect, uint input, D2D1_RECT_F inputRectangle);
        new HRESULT GetEffectInvalidRectangleCount(ID2D1Effect effect, out uint rectangleCount);
        //new  HRESULT GetEffectInvalidRectangles(ID2D1Effect effect, out D2D1_RECT_F* rectangles, uint rectanglesCount);
        new HRESULT GetEffectInvalidRectangles(ID2D1Effect effect, out IntPtr rectangles, uint rectanglesCount);
        new HRESULT GetEffectRequiredInputRectangles(ID2D1Effect renderEffect, D2D1_RECT_F renderImageRectangle, D2D1_EFFECT_INPUT_DESCRIPTION inputDescriptions,
            // out D2D1_RECT_F* requiredInputRects, uint inputCount);
            out IntPtr requiredInputRects, uint inputCount);
        new void FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, ref D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        #endregion

        new HRESULT CreateFilledGeometryRealization(ID2D1Geometry geometry, float flatteningTolerance, out ID2D1GeometryRealization geometryRealization);
        new HRESULT CreateStrokedGeometryRealization(ID2D1Geometry geometry, float flatteningTolerance, float strokeWidth, ID2D1StrokeStyle strokeStyle, out ID2D1GeometryRealization geometryRealization);
        new void DrawGeometryRealization(ID2D1GeometryRealization geometryRealization, ID2D1Brush brush);
        #endregion

        new HRESULT CreateInk(D2D1_INK_POINT startPoint, out ID2D1Ink ink);
        new HRESULT CreateInkStyle(D2D1_INK_STYLE_PROPERTIES inkStyleProperties, out ID2D1InkStyle inkStyle);
        new HRESULT CreateGradientMesh(D2D1_GRADIENT_MESH_PATCH patches, uint patchesCount, out ID2D1GradientMesh gradientMesh);
        new HRESULT CreateImageSourceFromWic(IWICBitmapSource wicBitmapSource, D2D1_IMAGE_SOURCE_LOADING_OPTIONS loadingOptions, D2D1_ALPHA_MODE alphaMode, out ID2D1ImageSourceFromWic imageSource);
        new HRESULT CreateLookupTable3D(D2D1_BUFFER_PRECISION precision, uint extents, IntPtr data, uint dataCount, uint strides, out ID2D1LookupTable3D lookupTable);
        //new HRESULT CreateImageSourceFromDxgi(IDXGISurface** surfaces, uint surfaceCount, DXGI_COLOR_SPACE_TYPE colorSpace, D2D1_IMAGE_SOURCE_FROM_DXGI_OPTIONS options, out ID2D1ImageSource** imageSource);
        new HRESULT CreateImageSourceFromDxgi(IntPtr surfaces, uint surfaceCount, DXGI_COLOR_SPACE_TYPE colorSpace, D2D1_IMAGE_SOURCE_FROM_DXGI_OPTIONS options, out ID2D1ImageSource imageSource);
        new HRESULT GetGradientMeshWorldBounds(ID2D1GradientMesh gradientMesh, out D2D1_RECT_F pBounds);
        new void DrawInk(ID2D1Ink ink, ID2D1Brush brush, ID2D1InkStyle inkStyle);
        new void DrawGradientMesh(ID2D1GradientMesh gradientMesh);
        new void DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_RECT_F destinationRectangle, ref D2D1_RECT_F sourceRectangle);
        new HRESULT CreateTransformedImageSource(ID2D1ImageSource imageSource, D2D1_TRANSFORMED_IMAGE_SOURCE_PROPERTIES properties, out ID2D1TransformedImageSource transformedImageSource);

        #endregion

        new HRESULT CreateSpriteBatch(out ID2D1SpriteBatch spriteBatch);
        new void DrawSpriteBatch(ID2D1SpriteBatch spriteBatch, uint startIndex, uint spriteCount, ID2D1Bitmap bitmap,
            D2D1_BITMAP_INTERPOLATION_MODE interpolationMode = D2D1_BITMAP_INTERPOLATION_MODE.D2D1_BITMAP_INTERPOLATION_MODE_LINEAR, D2D1_SPRITE_OPTIONS spriteOptions = D2D1_SPRITE_OPTIONS.D2D1_SPRITE_OPTIONS_NONE);

        #endregion

        new HRESULT CreateSvgGlyphStyle(out ID2D1SvgGlyphStyle svgGlyphStyle);
        new void DrawText(string sString, uint stringLength, IDWriteTextFormat textFormat, D2D1_RECT_F layoutRect, ID2D1Brush defaultFillBrush,
            ID2D1SvgGlyphStyle svgGlyphStyle, uint colorPaletteIndex = 0,
            D2D1_DRAW_TEXT_OPTIONS options = D2D1_DRAW_TEXT_OPTIONS.D2D1_DRAW_TEXT_OPTIONS_ENABLE_COLOR_FONT,
            DWRITE_MEASURING_MODE measuringMode = DWRITE_MEASURING_MODE.DWRITE_MEASURING_MODE_NATURAL);
        new void DrawTextLayout(ref D2D1_POINT_2F origin,
            IDWriteTextLayout textLayout,
            ID2D1Brush defaultFillBrush,
            ID2D1SvgGlyphStyle svgGlyphStyle,
            uint colorPaletteIndex = 0,
            D2D1_DRAW_TEXT_OPTIONS options = D2D1_DRAW_TEXT_OPTIONS.D2D1_DRAW_TEXT_OPTIONS_ENABLE_COLOR_FONT);
        new void DrawColorBitmapGlyphRun(
            DWRITE_GLYPH_IMAGE_FORMATS glyphImageFormat,
            ref D2D1_POINT_2F baselineOrigin,
            DWRITE_GLYPH_RUN glyphRun,
            DWRITE_MEASURING_MODE measuringMode = DWRITE_MEASURING_MODE.DWRITE_MEASURING_MODE_NATURAL,
            D2D1_COLOR_BITMAP_GLYPH_SNAP_OPTION bitmapSnapOption = D2D1_COLOR_BITMAP_GLYPH_SNAP_OPTION.D2D1_COLOR_BITMAP_GLYPH_SNAP_OPTION_DEFAULT);
        new void DrawSvgGlyphRun(
            ref D2D1_POINT_2F baselineOrigin,
            DWRITE_GLYPH_RUN glyphRun,
            ID2D1Brush defaultFillBrush = null,
            ID2D1SvgGlyphStyle svgGlyphStyle = null,
            uint colorPaletteIndex = 0,
            DWRITE_MEASURING_MODE measuringMode = DWRITE_MEASURING_MODE.DWRITE_MEASURING_MODE_NATURAL);
        new HRESULT GetColorBitmapGlyphImage(
            DWRITE_GLYPH_IMAGE_FORMATS glyphImageFormat,
            ref D2D1_POINT_2F glyphOrigin,
            IDWriteFontFace fontFace,
            float fontEmSize,
            UInt16 glyphIndex,
            bool isSideways,
            D2D1_MATRIX_3X2_F worldTransform,
            float dpiX,
            float dpiY,
            out D2D1_MATRIX_3X2_F glyphTransform,
            out ID2D1Image glyphImage);
        new HRESULT GetSvgGlyphImage(
            ref D2D1_POINT_2F glyphOrigin,
            IDWriteFontFace fontFace,
            float fontEmSize,
            UInt16 glyphIndex,
            bool isSideways,
            D2D1_MATRIX_3X2_F worldTransform,
            ID2D1Brush defaultFillBrush,
            ID2D1SvgGlyphStyle svgGlyphStyle,
            uint colorPaletteIndex,
            out D2D1_MATRIX_3X2_F glyphTransform,
            out ID2D1CommandList glyphImage);
        #endregion

        HRESULT CreateSvgDocument(System.Runtime.InteropServices.ComTypes.IStream inputXmlStream, D2D1_SIZE_F viewportSize, out ID2D1SvgDocument svgDocument);
        void DrawSvgDocument(ID2D1SvgDocument svgDocument);
        HRESULT CreateColorContextFromDxgiColorSpace(DXGI_COLOR_SPACE_TYPE colorSpace, out ID2D1ColorContext1 colorContext);
        HRESULT CreateColorContextFromSimpleColorProfile(D2D1_SIMPLE_COLOR_PROFILE simpleProfile, out ID2D1ColorContext1 colorContext);
    }

    [ComImport]
    [Guid("985f7e37-4ed0-4a19-98a3-15b0edfde306")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1DeviceContext6 : ID2D1DeviceContext5
    {
        #region <ID2D1DeviceContext5>
        #region <ID2D1DeviceContext4>
        #region <ID2D1DeviceContext3>
        #region <ID2D1DeviceContext2>
        #region <ID2D1DeviceContext1>
        #region <ID2D1DeviceContext>
        #region <ID2D1RenderTarget>

        #region <ID2D1Resource>

        new void GetFactory(out ID2D1Factory factory);

        #endregion
        new void CreateBitmap(D2D1_SIZE_U size, IntPtr srcData, uint pitch, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new HRESULT CreateBitmapFromWicBitmap(IWICBitmapSource wicBitmapSource, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new void CreateSharedBitmap(ref Guid riid, [In, Out] IntPtr data, ref D2D1_BITMAP_PROPERTIES bitmapProperties, out ID2D1Bitmap bitmap);
        new void CreateBitmapBrush(ID2D1Bitmap bitmap, ref D2D1_BITMAP_BRUSH_PROPERTIES bitmapBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1BitmapBrush bitmapBrush);
        new HRESULT CreateSolidColorBrush(D2D1_COLOR_F color, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1SolidColorBrush solidColorBrush);
        new void CreateGradientStopCollection(D2D1_GRADIENT_STOP gradientStops, uint gradientStopsCount, D2D1_GAMMA colorInterpolationGamma, D2D1_EXTEND_MODE extendMode, out ID2D1GradientStopCollection gradientStopCollection);
        new void CreateLinearGradientBrush(D2D1_LINEAR_GRADIENT_BRUSH_PROPERTIES linearGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1LinearGradientBrush linearGradientBrush);
        new void CreateRadialGradientBrush(D2D1_RADIAL_GRADIENT_BRUSH_PROPERTIES radialGradientBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, ID2D1GradientStopCollection gradientStopCollection, out ID2D1RadialGradientBrush radialGradientBrush);
        new void CreateCompatibleRenderTarget(D2D1_SIZE_F desiredSize, D2D1_SIZE_U desiredPixelSize, D2D1_PIXEL_FORMAT desiredFormat, D2D1_COMPATIBLE_RENDER_TARGET_OPTIONS options, out ID2D1BitmapRenderTarget bitmapRenderTarget);
        new void CreateLayer(D2D1_SIZE_F size, out ID2D1Layer layer);
        new void CreateMesh(out ID2D1Mesh mesh);
        new void DrawLine(ref D2D1_POINT_2F point0, ref D2D1_POINT_2F point1, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void DrawRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillRectangle(ref D2D1_RECT_F rect, ID2D1Brush brush);
        new void DrawRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillRoundedRectangle(D2D1_ROUNDED_RECT roundedRect, ID2D1Brush brush);
        new void DrawEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillEllipse(ref D2D1_ELLIPSE ellipse, ID2D1Brush brush);
        new void DrawGeometry(ID2D1Geometry geometry, ID2D1Brush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle strokeStyle = null);
        new void FillGeometry(ID2D1Geometry geometry, ID2D1Brush brush, ID2D1Brush opacityBrush = null);
        new void FillMesh(ID2D1Mesh mesh, ID2D1Brush brush);
        new void FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, D2D1_OPACITY_MASK_CONTENT content, D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        new void DrawBitmap(ID2D1Bitmap bitmap, ref D2D1_RECT_F destinationRectangle, float opacity, D2D1_BITMAP_INTERPOLATION_MODE interpolationMode, ref D2D1_RECT_F sourceRectangle);
        new void DrawText(string str, uint stringLength, IDWriteTextFormat textFormat, D2D1_RECT_F layoutRect, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options, DWRITE_MEASURING_MODE measuringMode);
        new void DrawTextLayout(ref D2D1_POINT_2F origin, IDWriteTextLayout textLayout, ID2D1Brush defaultForegroundBrush, D2D1_DRAW_TEXT_OPTIONS options);
        new void DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        new void SetTransform(D2D1_MATRIX_3X2_F transform);
        new void GetTransform(out D2D1_MATRIX_3X2_F transform);
        new void SetAntialiasMode(D2D1_ANTIALIAS_MODE antialiasMode);
        new D2D1_ANTIALIAS_MODE GetAntialiasMode();
        new void SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode);
        new D2D1_TEXT_ANTIALIAS_MODE GetTextAntialiasMode();
        new void SetTextRenderingParams(IDWriteRenderingParams textRenderingParams = null);
        new void GetTextRenderingParams(out IDWriteRenderingParams textRenderingParams);
        new void SetTags(UInt64 tag1, UInt64 tag2);
        new void GetTags(out UInt64 tag1, out UInt64 tag2);
        new void PushLayer(D2D1_LAYER_PARAMETERS layerParameters, ID2D1Layer layer);
        new void PopLayer();
        new void Flush(out UInt64 tag1, out UInt64 tag2);
        new void SaveDrawingState([In, Out] ID2D1DrawingStateBlock drawingStateBlock);
        new void RestoreDrawingState(ID2D1DrawingStateBlock drawingStateBlock);
        new void PushAxisAlignedClip(D2D1_RECT_F clipRect, D2D1_ANTIALIAS_MODE antialiasMode);
        new void PopAxisAlignedClip();
        new void Clear(D2D1_COLOR_F clearColor);
        new void BeginDraw();
        new HRESULT EndDraw(out UInt64 tag1, out UInt64 tag2);
        new D2D1_PIXEL_FORMAT GetPixelFormat();
        new void SetDpi(float dpiX, float dpiY);
        new void GetDpi(out float dpiX, out float dpiY);
        new D2D1_SIZE_F GetSize();
        new D2D1_SIZE_U GetPixelSize();
        new uint GetMaximumBitmapSize();
        new bool IsSupported(D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties);
        #endregion

        new HRESULT CreateBitmap(D2D1_SIZE_U size, IntPtr sourceData, uint pitch, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        new HRESULT CreateBitmapFromWicBitmap(IWICBitmapSource wicBitmapSource, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        new HRESULT CreateColorContext(D2D1_COLOR_SPACE space, IntPtr profile, uint profileSize, out ID2D1ColorContext colorContext);
        new HRESULT CreateColorContextFromFilename(string filename, out ID2D1ColorContext colorContext);
        new HRESULT CreateColorContextFromWicColorContext(IWICColorContext wicColorContext, out ID2D1ColorContext colorContext);
        new HRESULT CreateBitmapFromDxgiSurface(IDXGISurface surface, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        new HRESULT CreateEffect(ref Guid effectId, out ID2D1Effect effect);
        new HRESULT CreateGradientStopCollection(D2D1_GRADIENT_STOP straightAlphaGradientStops, uint straightAlphaGradientStopsCount, D2D1_COLOR_SPACE preInterpolationSpace,
            D2D1_COLOR_SPACE postInterpolationSpace, D2D1_BUFFER_PRECISION bufferPrecision, D2D1_EXTEND_MODE extendMode, D2D1_COLOR_INTERPOLATION_MODE colorInterpolationMode,
            out ID2D1GradientStopCollection1 gradientStopCollection1);
        new HRESULT CreateImageBrush(ID2D1Image image, ref D2D1_IMAGE_BRUSH_PROPERTIES imageBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1ImageBrush imageBrush);
        new HRESULT CreateBitmapBrush(ID2D1Bitmap bitmap, ref D2D1_BITMAP_BRUSH_PROPERTIES1 bitmapBrushProperties, D2D1_BRUSH_PROPERTIES brushProperties, out ID2D1BitmapBrush1 bitmapBrush);
        new HRESULT CreateCommandList(out ID2D1CommandList commandList);
        new bool IsDxgiFormatSupported(DXGI_FORMAT format);
        new bool IsBufferPrecisionSupported(D2D1_BUFFER_PRECISION bufferPrecision);
        new HRESULT GetImageLocalBounds(ID2D1Image image, out D2D1_RECT_F localBounds);
        new HRESULT GetImageWorldBounds(ID2D1Image image, out D2D1_RECT_F worldBounds);
        new HRESULT GetGlyphRunWorldBounds(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, DWRITE_MEASURING_MODE measuringMode, out D2D1_RECT_F bounds);
        new void GetDevice(out ID2D1Device device);
        new void SetTarget(ID2D1Image image);
        new void GetTarget(out ID2D1Image image);
        new void SetRenderingControls(D2D1_RENDERING_CONTROLS renderingControls);
        new void GetRenderingControls(out D2D1_RENDERING_CONTROLS renderingControls);
        new void SetPrimitiveBlend(D2D1_PRIMITIVE_BLEND primitiveBlend);
        new D2D1_PRIMITIVE_BLEND GetPrimitiveBlend();
        new void SetUnitMode(D2D1_UNIT_MODE unitMode);
        new D2D1_UNIT_MODE GetUnitMode();
        new void DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, DWRITE_GLYPH_RUN_DESCRIPTION glyphRunDescription, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        new void DrawImage(ID2D1Image image, ref D2D1_POINT_2F targetOffset, ref D2D1_RECT_F imageRectangle, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_COMPOSITE_MODE compositeMode);
        new void DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_POINT_2F targetOffset);
        new void DrawBitmap(ID2D1Bitmap bitmap, ref D2D1_RECT_F destinationRectangle, float opacity, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_RECT_F sourceRectangle, D2D1_MATRIX_4X4_F perspectiveTransform);
        new void PushLayer(D2D1_LAYER_PARAMETERS1 layerParameters, ID2D1Layer layer);
        new HRESULT InvalidateEffectInputRectangle(ID2D1Effect effect, uint input, D2D1_RECT_F inputRectangle);
        new HRESULT GetEffectInvalidRectangleCount(ID2D1Effect effect, out uint rectangleCount);
        //new  HRESULT GetEffectInvalidRectangles(ID2D1Effect effect, out D2D1_RECT_F* rectangles, uint rectanglesCount);
        new HRESULT GetEffectInvalidRectangles(ID2D1Effect effect, out IntPtr rectangles, uint rectanglesCount);
        new HRESULT GetEffectRequiredInputRectangles(ID2D1Effect renderEffect, D2D1_RECT_F renderImageRectangle, D2D1_EFFECT_INPUT_DESCRIPTION inputDescriptions,
            // out D2D1_RECT_F* requiredInputRects, uint inputCount);
            out IntPtr requiredInputRects, uint inputCount);
        new void FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, ref D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        #endregion

        new HRESULT CreateFilledGeometryRealization(ID2D1Geometry geometry, float flatteningTolerance, out ID2D1GeometryRealization geometryRealization);
        new HRESULT CreateStrokedGeometryRealization(ID2D1Geometry geometry, float flatteningTolerance, float strokeWidth, ID2D1StrokeStyle strokeStyle, out ID2D1GeometryRealization geometryRealization);
        new void DrawGeometryRealization(ID2D1GeometryRealization geometryRealization, ID2D1Brush brush);
        #endregion

        new HRESULT CreateInk(D2D1_INK_POINT startPoint, out ID2D1Ink ink);
        new HRESULT CreateInkStyle(D2D1_INK_STYLE_PROPERTIES inkStyleProperties, out ID2D1InkStyle inkStyle);
        new HRESULT CreateGradientMesh(D2D1_GRADIENT_MESH_PATCH patches, uint patchesCount, out ID2D1GradientMesh gradientMesh);
        new HRESULT CreateImageSourceFromWic(IWICBitmapSource wicBitmapSource, D2D1_IMAGE_SOURCE_LOADING_OPTIONS loadingOptions, D2D1_ALPHA_MODE alphaMode, out ID2D1ImageSourceFromWic imageSource);
        new HRESULT CreateLookupTable3D(D2D1_BUFFER_PRECISION precision, uint extents, IntPtr data, uint dataCount, uint strides, out ID2D1LookupTable3D lookupTable);
        //new HRESULT CreateImageSourceFromDxgi(IDXGISurface** surfaces, uint surfaceCount, DXGI_COLOR_SPACE_TYPE colorSpace, D2D1_IMAGE_SOURCE_FROM_DXGI_OPTIONS options, out ID2D1ImageSource** imageSource);
        new HRESULT CreateImageSourceFromDxgi(IntPtr surfaces, uint surfaceCount, DXGI_COLOR_SPACE_TYPE colorSpace, D2D1_IMAGE_SOURCE_FROM_DXGI_OPTIONS options, out ID2D1ImageSource imageSource);
        new HRESULT GetGradientMeshWorldBounds(ID2D1GradientMesh gradientMesh, out D2D1_RECT_F pBounds);
        new void DrawInk(ID2D1Ink ink, ID2D1Brush brush, ID2D1InkStyle inkStyle);
        new void DrawGradientMesh(ID2D1GradientMesh gradientMesh);
        new void DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_RECT_F destinationRectangle, ref D2D1_RECT_F sourceRectangle);
        new HRESULT CreateTransformedImageSource(ID2D1ImageSource imageSource, D2D1_TRANSFORMED_IMAGE_SOURCE_PROPERTIES properties, out ID2D1TransformedImageSource transformedImageSource);

        #endregion

        new HRESULT CreateSpriteBatch(out ID2D1SpriteBatch spriteBatch);
        new void DrawSpriteBatch(ID2D1SpriteBatch spriteBatch, uint startIndex, uint spriteCount, ID2D1Bitmap bitmap,
            D2D1_BITMAP_INTERPOLATION_MODE interpolationMode = D2D1_BITMAP_INTERPOLATION_MODE.D2D1_BITMAP_INTERPOLATION_MODE_LINEAR, D2D1_SPRITE_OPTIONS spriteOptions = D2D1_SPRITE_OPTIONS.D2D1_SPRITE_OPTIONS_NONE);

        #endregion

        new HRESULT CreateSvgGlyphStyle(out ID2D1SvgGlyphStyle svgGlyphStyle);
        new void DrawText(string sString, uint stringLength, IDWriteTextFormat textFormat, D2D1_RECT_F layoutRect, ID2D1Brush defaultFillBrush,
            ID2D1SvgGlyphStyle svgGlyphStyle, uint colorPaletteIndex = 0,
            D2D1_DRAW_TEXT_OPTIONS options = D2D1_DRAW_TEXT_OPTIONS.D2D1_DRAW_TEXT_OPTIONS_ENABLE_COLOR_FONT,
            DWRITE_MEASURING_MODE measuringMode = DWRITE_MEASURING_MODE.DWRITE_MEASURING_MODE_NATURAL);
        new void DrawTextLayout(ref D2D1_POINT_2F origin,
            IDWriteTextLayout textLayout,
            ID2D1Brush defaultFillBrush,
            ID2D1SvgGlyphStyle svgGlyphStyle,
            uint colorPaletteIndex = 0,
            D2D1_DRAW_TEXT_OPTIONS options = D2D1_DRAW_TEXT_OPTIONS.D2D1_DRAW_TEXT_OPTIONS_ENABLE_COLOR_FONT);
        new void DrawColorBitmapGlyphRun(
            DWRITE_GLYPH_IMAGE_FORMATS glyphImageFormat,
            ref D2D1_POINT_2F baselineOrigin,
            DWRITE_GLYPH_RUN glyphRun,
            DWRITE_MEASURING_MODE measuringMode = DWRITE_MEASURING_MODE.DWRITE_MEASURING_MODE_NATURAL,
            D2D1_COLOR_BITMAP_GLYPH_SNAP_OPTION bitmapSnapOption = D2D1_COLOR_BITMAP_GLYPH_SNAP_OPTION.D2D1_COLOR_BITMAP_GLYPH_SNAP_OPTION_DEFAULT);
        new void DrawSvgGlyphRun(
            ref D2D1_POINT_2F baselineOrigin,
            DWRITE_GLYPH_RUN glyphRun,
            ID2D1Brush defaultFillBrush = null,
            ID2D1SvgGlyphStyle svgGlyphStyle = null,
            uint colorPaletteIndex = 0,
            DWRITE_MEASURING_MODE measuringMode = DWRITE_MEASURING_MODE.DWRITE_MEASURING_MODE_NATURAL);
        new HRESULT GetColorBitmapGlyphImage(
            DWRITE_GLYPH_IMAGE_FORMATS glyphImageFormat,
            ref D2D1_POINT_2F glyphOrigin,
            IDWriteFontFace fontFace,
            float fontEmSize,
            UInt16 glyphIndex,
            bool isSideways,
            D2D1_MATRIX_3X2_F worldTransform,
            float dpiX,
            float dpiY,
            out D2D1_MATRIX_3X2_F glyphTransform,
            out ID2D1Image glyphImage);
        new HRESULT GetSvgGlyphImage(
            ref D2D1_POINT_2F glyphOrigin,
            IDWriteFontFace fontFace,
            float fontEmSize,
            UInt16 glyphIndex,
            bool isSideways,
            D2D1_MATRIX_3X2_F worldTransform,
            ID2D1Brush defaultFillBrush,
            ID2D1SvgGlyphStyle svgGlyphStyle,
            uint colorPaletteIndex,
            out D2D1_MATRIX_3X2_F glyphTransform,
            out ID2D1CommandList glyphImage);
        #endregion

        new HRESULT CreateSvgDocument(System.Runtime.InteropServices.ComTypes.IStream inputXmlStream, D2D1_SIZE_F viewportSize, out ID2D1SvgDocument svgDocument);
        new void DrawSvgDocument(ID2D1SvgDocument svgDocument);
        new HRESULT CreateColorContextFromDxgiColorSpace(DXGI_COLOR_SPACE_TYPE colorSpace, out ID2D1ColorContext1 colorContext);
        new HRESULT CreateColorContextFromSimpleColorProfile(D2D1_SIMPLE_COLOR_PROFILE simpleProfile, out ID2D1ColorContext1 colorContext);
        #endregion

        void BlendImage(ID2D1Image image, D2D1_BLEND_MODE blendMode, ref D2D1_POINT_2F targetOffset,
            ref D2D1_RECT_F imageRectangle, D2D1_INTERPOLATION_MODE interpolationMode = D2D1_INTERPOLATION_MODE.D2D1_INTERPOLATION_MODE_LINEAR);
    }

    [ComImport]
    [Guid("1ab42875-c57f-4be9-bd85-9cd78d6f55ee")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1ColorContext1 : ID2D1ColorContext
    {
        #region <ID2D1ColorContext>

        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        new D2D1_COLOR_SPACE GetColorSpace();
        new uint GetProfileSize();
        new HRESULT GetProfile(out IntPtr profile, uint profileSize);
        #endregion

        D2D1_COLOR_CONTEXT_TYPE GetColorContextType();
        DXGI_COLOR_SPACE_TYPE GetDXGIColorSpace();
        HRESULT GetSimpleColorProfile(out D2D1_SIMPLE_COLOR_PROFILE simpleProfile);
    }

    /// <summary>
    /// Specifies which way a color profile is defined.
    /// </summary>
    public enum D2D1_COLOR_CONTEXT_TYPE
    {
        D2D1_COLOR_CONTEXT_TYPE_ICC = 0,
        D2D1_COLOR_CONTEXT_TYPE_SIMPLE = 1,
        D2D1_COLOR_CONTEXT_TYPE_DXGI = 2,
        D2D1_COLOR_CONTEXT_TYPE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [ComImport]
    [Guid("86b88e4d-afa4-4d7b-88e4-68a51c4a0aec")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1SvgDocument : ID2D1Resource
    {
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        HRESULT SetViewportSize(D2D1_SIZE_F viewportSize);
        D2D1_SIZE_F GetViewportSize();
        HRESULT SetRoot(ID2D1SvgElement root);
        void GetRoot(out ID2D1SvgElement root);
        HRESULT FindElementById(string id, out ID2D1SvgElement svgElement);
        HRESULT Serialize(System.Runtime.InteropServices.ComTypes.IStream outputXmlStream, ID2D1SvgElement subtree = null);
        HRESULT Deserialize(System.Runtime.InteropServices.ComTypes.IStream inputXmlStream, out ID2D1SvgElement subtree);
        HRESULT CreatePaint(D2D1_SVG_PAINT_TYPE paintType, D2D1_COLOR_F color, string id, out ID2D1SvgPaint paint);
        HRESULT CreateStrokeDashArray(D2D1_SVG_LENGTH dashes, uint dashesCount, out ID2D1SvgStrokeDashArray strokeDashArray);
        HRESULT CreatePointCollection(ref D2D1_POINT_2F points, uint pointsCount, out ID2D1SvgPointCollection pointCollection);
        HRESULT CreatePathData(float segmentData, uint segmentDataCount, D2D1_SVG_PATH_COMMAND commands, uint commandsCount, out ID2D1SvgPathData pathData);
    }

    [ComImport]
    [Guid("ac7b67a6-183e-49c1-a823-0ebe40b0db29")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1SvgElement : ID2D1Resource
    {
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        void GetDocument(out ID2D1SvgDocument document);
        HRESULT GetTagName(out string name, uint nameCount);
        uint GetTagNameLength();
        bool IsTextContent();
        void GetParent(out ID2D1SvgElement parent);
        bool HasChildren();
        void GetFirstChild(out ID2D1SvgElement child);
        void GetLastChild(out ID2D1SvgElement child);
        HRESULT GetPreviousChild(ID2D1SvgElement referenceChild, out ID2D1SvgElement previousChild);
        HRESULT GetNextChild(ID2D1SvgElement referenceChild, out ID2D1SvgElement nextChild);
        HRESULT InsertChildBefore(ID2D1SvgElement newChild, ID2D1SvgElement referenceChild = null);
        HRESULT AppendChild(ID2D1SvgElement newChild);
        HRESULT ReplaceChild(ID2D1SvgElement newChild, ID2D1SvgElement oldChild);
        HRESULT RemoveChild(ID2D1SvgElement oldChild);
        HRESULT CreateChild(string tagName, out ID2D1SvgElement newChild);
        bool IsAttributeSpecified(string name, out bool inherited);
        uint GetSpecifiedAttributeCount();
        HRESULT GetSpecifiedAttributeName(uint index, out string name, uint nameCount, out bool inherited);
        HRESULT GetSpecifiedAttributeNameLength(uint index, out uint nameLength, out bool inherited);
        HRESULT RemoveAttribute(string name);
        HRESULT SetTextValue(string name, uint nameCount);
        HRESULT GetTextValue(out string name, uint nameCount);
        uint GetTextValueLength();
        HRESULT SetAttributeValue(string name, D2D1_SVG_ATTRIBUTE_STRING_TYPE type, string value);
        HRESULT GetAttributeValue(string name, D2D1_SVG_ATTRIBUTE_STRING_TYPE type, out string value, uint valueCount);
        HRESULT GetAttributeValueLength(string name, D2D1_SVG_ATTRIBUTE_STRING_TYPE type, out uint valueLength);
        HRESULT SetAttributeValue(string name, D2D1_SVG_ATTRIBUTE_POD_TYPE type, IntPtr value, uint valueSizeInBytes);
        HRESULT GetAttributeValue(string name, D2D1_SVG_ATTRIBUTE_POD_TYPE type, out IntPtr value, uint valueSizeInBytes);
        HRESULT SetAttributeValue(string name, ID2D1SvgAttribute value);
        HRESULT GetAttributeValue(string name, ref Guid riid, out IntPtr value);
    }

    [ComImport]
    [Guid("c9cdb0dd-f8c9-4e70-b7c2-301c80292c5e")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1SvgAttribute : ID2D1Resource
    {
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        void GetElement(out ID2D1SvgElement element);
        HRESULT Clone(out ID2D1SvgAttribute attribute);
    }

    [ComImport]
    [Guid("d59bab0a-68a2-455b-a5dc-9eb2854e2490")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1SvgPaint : ID2D1SvgAttribute
    {
        #region <ID2D1SvgAttribute>
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        new void GetElement(out ID2D1SvgElement element);
        new HRESULT Clone(out ID2D1SvgAttribute attribute);
        #endregion

        HRESULT SetPaintType(D2D1_SVG_PAINT_TYPE paintType);
        D2D1_SVG_PAINT_TYPE GetPaintType();
        HRESULT SetColor(D2D1_COLOR_F color);
        void GetColor(out D2D1_COLOR_F color);
        HRESULT SetId(string id);
        HRESULT GetId(out string id, uint idCount);
        uint GetIdLength();
    }

    [ComImport]
    [Guid("f1c0ca52-92a3-4f00-b4ce-f35691efd9d9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1SvgStrokeDashArray : ID2D1SvgAttribute
    {
        #region <ID2D1SvgAttribute>
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        new void GetElement(out ID2D1SvgElement element);
        new HRESULT Clone(out ID2D1SvgAttribute attribute);
        #endregion

        HRESULT RemoveDashesAtEnd(uint dashesCount);
        HRESULT UpdateDashes(float dashes, uint dashesCount, uint startIndex = 0);
        HRESULT UpdateDashes(D2D1_SVG_LENGTH dashes, uint dashesCount, uint startIndex = 0);
        HRESULT GetDashes(out float dashes, uint dashesCount, uint startIndex = 0);
        HRESULT GetDashes(out D2D1_SVG_LENGTH dashes, uint dashesCount, uint startIndex = 0);
        uint GetDashesCount();
    }

    [ComImport]
    [Guid("9dbe4c0d-3572-4dd9-9825-5530813bb712")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1SvgPointCollection : ID2D1SvgAttribute
    {
        #region <ID2D1SvgAttribute>
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        new void GetElement(out ID2D1SvgElement element);
        new HRESULT Clone(out ID2D1SvgAttribute attribute);
        #endregion

        HRESULT RemovePointsAtEnd(uint pointsCount);
        HRESULT UpdatePoints(ref D2D1_POINT_2F points, uint pointsCount, uint startIndex = 0);
        HRESULT GetPoints(out D2D1_POINT_2F points, uint pointsCount, uint startIndex = 0);
        uint GetPointsCount();
    }

    [ComImport]
    [Guid("c095e4f4-bb98-43d6-9745-4d1b84ec9888")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1SvgPathData : ID2D1SvgAttribute
    {
        #region <ID2D1SvgAttribute>
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        new void GetElement(out ID2D1SvgElement element);
        new HRESULT Clone(out ID2D1SvgAttribute attribute);
        #endregion

        HRESULT RemoveSegmentDataAtEnd(uint dataCount);
        HRESULT UpdateSegmentData(float data, uint dataCount, uint startIndex = 0);
        HRESULT GetSegmentData(out float data, uint dataCount, uint startIndex = 0);
        uint GetSegmentDataCount();
        HRESULT RemoveCommandsAtEnd(uint commandsCount);
        HRESULT UpdateCommands(D2D1_SVG_PATH_COMMAND commands, uint commandsCount, uint startIndex = 0);
        HRESULT GetCommands(out D2D1_SVG_PATH_COMMAND commands, uint commandsCount, uint startIndex = 0);
        uint GetCommandsCount();
        HRESULT CreatePathGeometry(D2D1_FILL_MODE fillMode, out ID2D1PathGeometry1 pathGeometry);
    }

    [ComImport]
    [Guid("62baa2d2-ab54-41b7-b872-787e0106a421")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1PathGeometry1 : ID2D1PathGeometry
    {
        #region <ID2D1PathGeometry>

        #region ID2D1Geometry
        #region ID2D1Resource
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        new void GetBounds(D2D1_MATRIX_3X2_F worldTransform, out D2D1_RECT_F bounds);
        new void GetWidenedBounds(float strokeWidth, ID2D1StrokeStyle strokeStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out D2D1_RECT_F bounds);
        new void StrokeContainsPoint(ref D2D1_POINT_2F point, float strokeWidth, ID2D1StrokeStyle strokeStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out bool contains);
        new void FillContainsPoint(ref D2D1_POINT_2F point, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out bool contains);
        new void CompareWithGeometry(ID2D1Geometry inputGeometry, D2D1_MATRIX_3X2_F inputGeometryTransform, float flatteningTolerance, out D2D1_GEOMETRY_RELATION relation);
        new void Simplify(D2D1_GEOMETRY_SIMPLIFICATION_OPTION simplificationOption, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        new void Tessellate(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1TessellationSink tessellationSink);
        new void CombineWithGeometry(ID2D1Geometry inputGeometry, D2D1_COMBINE_MODE combineMode, D2D1_MATRIX_3X2_F inputGeometryTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        new void Outline(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        new void ComputeArea(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out float area);
        new void ComputeLength(D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out float length);
        new void ComputePointAtLength(float length, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out D2D1_POINT_2F point, out D2D1_POINT_2F unitTangentVector);
        new void Widen(float strokeWidth, ID2D1StrokeStyle strokeStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);

        #endregion

        new HRESULT Open(out ID2D1GeometrySink geometrySink);
        new HRESULT Stream(ID2D1GeometrySink geometrySink);
        new HRESULT GetSegmentCount(out int count);
        new HRESULT GetFigureCount(out int count);
        #endregion

        HRESULT ComputePointAndSegmentAtLength(float length, uint startSegment, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, out D2D1_POINT_DESCRIPTION pointDescription);
    }

    /// <summary>
    /// Describes a point along a path.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_POINT_DESCRIPTION
    {
        public D2D1_POINT_2F point;
        public D2D1_POINT_2F unitTangentVector;
        public uint endSegment;
        public uint endFigure;
        public float lengthToEndSegment;
    }  

    /// <summary>
    /// Simple description of a color space.
    /// </summary>
    /// 
    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_SIMPLE_COLOR_PROFILE
    {
        /// <summary>
        /// The XY coordinates of the red primary in CIEXYZ space.
        /// </summary>
        public D2D1_POINT_2F redPrimary;

        /// <summary>
        /// The XY coordinates of the green primary in CIEXYZ space.
        /// </summary>
        public D2D1_POINT_2F greenPrimary;

        /// <summary>
        /// The XY coordinates of the blue primary in CIEXYZ space.
        /// </summary>
        public D2D1_POINT_2F bluePrimary;

        /// <summary>
        /// The X/Z tristimulus values for the whitepoint, normalized for relative
        /// luminance.
        /// </summary>
        public D2D1_POINT_2F whitePointXZ;

        /// <summary>
        /// The gamma encoding to use for this color space.
        /// </summary>
        public D2D1_GAMMA1 gamma;
    }

    /// <summary>
    /// This determines what gamma is used for interpolation/blending.
    /// </summary>
    public enum D2D1_GAMMA1
    {
        /// <summary>
        /// Colors are manipulated in 2.2 gamma color space.
        /// </summary>
        D2D1_GAMMA1_G22 = D2D1_GAMMA.D2D1_GAMMA_2_2,

        /// <summary>
        /// Colors are manipulated in 1.0 gamma color space.
        /// </summary>
        D2D1_GAMMA1_G10 = D2D1_GAMMA.D2D1_GAMMA_1_0,

        /// <summary>
        /// Colors are manipulated in ST.2084 PQ gamma color space.
        /// </summary>
        D2D1_GAMMA1_G2084 = 2,
        D2D1_GAMMA1_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [ComImport]
    [Guid("b499923b-7029-478f-a8b3-432c7c5f5312")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Ink : ID2D1Resource
    {
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        void SetStartPoint(D2D1_INK_POINT startPoint);
        D2D1_INK_POINT GetStartPoint();
        HRESULT AddSegments(D2D1_INK_BEZIER_SEGMENT segments, uint segmentsCount);
        HRESULT RemoveSegmentsAtEnd(uint segmentsCount);
        HRESULT SetSegments(uint startSegment, D2D1_INK_BEZIER_SEGMENT segments, uint segmentsCount);
        HRESULT SetSegmentAtEnd(D2D1_INK_BEZIER_SEGMENT segment);
        uint GetSegmentCount();
        HRESULT GetSegments(uint startSegment, out D2D1_INK_BEZIER_SEGMENT segments, uint segmentsCount);
        HRESULT StreamAsGeometry(ID2D1InkStyle inkStyle, D2D1_MATRIX_3X2_F worldTransform, float flatteningTolerance, ID2D1SimplifiedGeometrySink geometrySink);
        HRESULT GetBounds(ID2D1InkStyle inkStyle, D2D1_MATRIX_3X2_F worldTransform, out D2D1_RECT_F bounds);
    }

    [ComImport]
    [Guid("bae8b344-23fc-4071-8cb5-d05d6f073848")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1InkStyle : ID2D1Resource
    {
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        void SetNibTransform(D2D1_MATRIX_3X2_F transform);
        void GetNibTransform(out D2D1_MATRIX_3X2_F transform);
        void SetNibShape(D2D1_INK_NIB_SHAPE nibShape);
        D2D1_INK_NIB_SHAPE GetNibShape();
    }

    [ComImport]
    [Guid("f292e401-c050-4cde-83d7-04962d3b23c2")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1GradientMesh : ID2D1Resource
    {
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        uint GetPatchCount();
        HRESULT GetPatches(uint startIndex, out D2D1_GRADIENT_MESH_PATCH patches, uint patchesCount);
    }

    [ComImport]
    [Guid("c9b664e5-74a1-4378-9ac2-eefc37a3f4d8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1ImageSource : ID2D1Image
    {
        #region <ID2D1Image>
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);

        #endregion
        #endregion

        HRESULT OfferResources();
        HRESULT TryReclaimResources(out bool resourcesDiscarded);
    }

    [ComImport]
    [Guid("7f1f79e5-2796-416c-8f55-700f911445e5")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1TransformedImageSource : ID2D1Image
    {
        #region <ID2D1Image>
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);

        #endregion
        #endregion

        void GetSource(out ID2D1ImageSource imageSource);
        void GetProperties(out D2D1_TRANSFORMED_IMAGE_SOURCE_PROPERTIES properties);
    }

    [ComImport]
    [Guid("77395441-1c8f-4555-8683-f50dab0fe792")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1ImageSourceFromWic : ID2D1ImageSource
    {
        #region <ID2D1ImageSource>

        #region <ID2D1Image>
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);

        #endregion
        #endregion

        new HRESULT OfferResources();
        new HRESULT TryReclaimResources(out bool resourcesDiscarded);
        #endregion

        HRESULT EnsureCached(D2D1_RECT_U rectangleToFill);
        HRESULT TrimCache(D2D1_RECT_U rectangleToPreserve);
        void GetSource(out IWICBitmapSource wicBitmapSource);
    }

    [ComImport]
    [Guid("53dd9855-a3b0-4d5b-82e1-26e25c5e5797")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1LookupTable3D : ID2D1Resource
    {
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion      
    }

    /// <summary>
    ///  Represents a Bezier segment to be used in the creation of an ID2D1Ink object.
    /// This structure differs from D2D1_BEZIER_SEGMENT in that it is composed of
    /// D2D1_INK_POINT s, which contain a radius in addition to x- and y-coordinates.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_INK_BEZIER_SEGMENT
    {
        public D2D1_INK_POINT point1;
        public D2D1_INK_POINT point2;
        public D2D1_INK_POINT point3;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_INK_POINT
    {
        public float x;
        public float y;
        public float radius;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_INK_STYLE_PROPERTIES
    {
        /// <summary>
        /// The general shape of the nib used to draw a given ink object.
        /// </summary>
        public D2D1_INK_NIB_SHAPE nibShape;

        /// <summary>
        /// The transform applied to shape of the nib. _31 and _32 are ignored.
        /// </summary>
        public D2D1_MATRIX_3X2_F nibTransform;
    };

    /// <summary>
    /// Specifies the appearance of the ink nib (pen tip) as part of an
    /// D2D1_INK_STYLE_PROPERTIES structure.
    /// </summary>
    public enum D2D1_INK_NIB_SHAPE
    {
        D2D1_INK_NIB_SHAPE_ROUND = 0,
        D2D1_INK_NIB_SHAPE_SQUARE = 1,
        D2D1_INK_NIB_SHAPE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// Represents a tensor patch with 16 control points, 4 corner colors, and boundary
    /// flags. An ID2D1GradientMesh is made up of 1 or more gradient mesh patches. Use
    /// the GradientMeshPatch function or the GradientMeshPatchFromCoonsPatch function
    /// to create one.
    /// </summary>
    /// 
    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_GRADIENT_MESH_PATCH
    {

        /// <summary>
        /// The gradient mesh patch control point at position 00.
        /// </summary>
        public D2D1_POINT_2F point00;

        /// <summary>
        /// The gradient mesh patch control point at position 01.
        /// </summary>
        public D2D1_POINT_2F point01;

        /// <summary>
        /// The gradient mesh patch control point at position 02.
        /// </summary>
        public D2D1_POINT_2F point02;

        /// <summary>
        /// The gradient mesh patch control point at position 03.
        /// </summary>
        public D2D1_POINT_2F point03;

        /// <summary>
        /// The gradient mesh patch control point at position 10.
        /// </summary>
        public D2D1_POINT_2F point10;

        /// <summary>
        /// The gradient mesh patch control point at position 11.
        /// </summary>
        public D2D1_POINT_2F point11;

        /// <summary>
        /// The gradient mesh patch control point at position 12.
        /// </summary>
        public D2D1_POINT_2F point12;

        /// <summary>
        /// The gradient mesh patch control point at position 13.
        /// </summary>
        public D2D1_POINT_2F point13;

        /// <summary>
        /// The gradient mesh patch control point at position 20.
        /// </summary>
        public D2D1_POINT_2F point20;

        /// <summary>
        /// The gradient mesh patch control point at position 21.
        /// </summary>
        public D2D1_POINT_2F point21;

        /// <summary>
        /// The gradient mesh patch control point at position 22.
        /// </summary>
        public D2D1_POINT_2F point22;

        /// <summary>
        /// The gradient mesh patch control point at position 23.
        /// </summary>
        public D2D1_POINT_2F point23;

        /// <summary>
        /// The gradient mesh patch control point at position 30.
        /// </summary>
        public D2D1_POINT_2F point30;

        /// <summary>
        /// The gradient mesh patch control point at position 31.
        /// </summary>
        public D2D1_POINT_2F point31;

        /// <summary>
        /// The gradient mesh patch control point at position 32.
        /// </summary>
        public D2D1_POINT_2F point32;

        /// <summary>
        /// The gradient mesh patch control point at position 33.
        /// </summary>
        public D2D1_POINT_2F point33;

        /// <summary>
        /// The color associated with control point at position 00.
        /// </summary>
        public D2D1_COLOR_F color00;

        /// <summary>
        /// The color associated with control point at position 03.
        /// </summary>
        public D2D1_COLOR_F color03;

        /// <summary>
        /// The color associated with control point at position 30.
        /// </summary>
        public D2D1_COLOR_F color30;

        /// <summary>
        /// The color associated with control point at position 33.
        /// </summary>
        public D2D1_COLOR_F color33;

        /// <summary>
        /// The edge mode for the top edge of the patch.
        /// </summary>
        public D2D1_PATCH_EDGE_MODE topEdgeMode;

        /// <summary>
        /// The edge mode for the left edge of the patch.
        /// </summary>
        public D2D1_PATCH_EDGE_MODE leftEdgeMode;

        /// <summary>
        /// The edge mode for the bottom edge of the patch.
        /// </summary>
        public D2D1_PATCH_EDGE_MODE bottomEdgeMode;

        /// <summary>
        /// The edge mode for the right edge of the patch.
        /// </summary>
        public D2D1_PATCH_EDGE_MODE rightEdgeMode;
    }

    /// <summary>
    /// Specifies how to render gradient mesh edges.
    /// </summary>
    public enum D2D1_PATCH_EDGE_MODE
    {
        /// <summary>
        /// Render this edge aliased.
        /// </summary>
        D2D1_PATCH_EDGE_MODE_ALIASED = 0,

        /// <summary>
        /// Render this edge antialiased.
        /// </summary>
        D2D1_PATCH_EDGE_MODE_ANTIALIASED = 1,

        /// <summary>
        /// Render this edge aliased and inflated out slightly.
        /// </summary>
        D2D1_PATCH_EDGE_MODE_ALIASED_INFLATED = 2,
        D2D1_PATCH_EDGE_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// Option flags controlling how images sources are loaded during
    /// CreateImageSourceFromWic.
    /// </summary>
    public enum D2D1_IMAGE_SOURCE_LOADING_OPTIONS
    {
        D2D1_IMAGE_SOURCE_LOADING_OPTIONS_NONE = 0,
        D2D1_IMAGE_SOURCE_LOADING_OPTIONS_RELEASE_SOURCE = 1,
        D2D1_IMAGE_SOURCE_LOADING_OPTIONS_CACHE_ON_DEMAND = 2,
        D2D1_IMAGE_SOURCE_LOADING_OPTIONS_FORCE_DWORD = unchecked((int)0xffffffff)
    }   

    /// <summary>
    /// Option flags controlling primary conversion performed by
    /// CreateImageSourceFromDxgi, if any.
    /// </summary>
    public enum D2D1_IMAGE_SOURCE_FROM_DXGI_OPTIONS
    {
        D2D1_IMAGE_SOURCE_FROM_DXGI_OPTIONS_NONE = 0,
        D2D1_IMAGE_SOURCE_FROM_DXGI_OPTIONS_LOW_QUALITY_PRIMARY_CONVERSION = 1,
        D2D1_IMAGE_SOURCE_FROM_DXGI_OPTIONS_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// Properties of a transformed image source.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_TRANSFORMED_IMAGE_SOURCE_PROPERTIES
    {
        /// <summary>
        /// The orientation at which the image source is drawn.
        /// </summary>
        public D2D1_ORIENTATION orientation;

        /// <summary>
        /// The horizontal scale factor at which the image source is drawn.
        /// </summary>
        public float scaleX;

        /// <summary>
        /// The vertical scale factor at which the image source is drawn.
        /// </summary>
        public float scaleY;

        /// <summary>
        /// The interpolation mode used when the image source is drawn.  This is ignored if
        /// the image source is drawn using the DrawImage method, or using an image brush.
        /// </summary>
        public D2D1_INTERPOLATION_MODE interpolationMode;

        /// <summary>
        /// Option flags.
        /// </summary>
        public D2D1_TRANSFORMED_IMAGE_SOURCE_OPTIONS options;
    }

    /// <summary>
    /// Specifies the orientation of an image.
    /// </summary>
    public enum D2D1_ORIENTATION
    {
        D2D1_ORIENTATION_DEFAULT = 1,
        D2D1_ORIENTATION_FLIP_HORIZONTAL = 2,
        D2D1_ORIENTATION_ROTATE_CLOCKWISE180 = 3,
        D2D1_ORIENTATION_ROTATE_CLOCKWISE180_FLIP_HORIZONTAL = 4,
        D2D1_ORIENTATION_ROTATE_CLOCKWISE90_FLIP_HORIZONTAL = 5,
        D2D1_ORIENTATION_ROTATE_CLOCKWISE270 = 6,
        D2D1_ORIENTATION_ROTATE_CLOCKWISE270_FLIP_HORIZONTAL = 7,
        D2D1_ORIENTATION_ROTATE_CLOCKWISE90 = 8,
        D2D1_ORIENTATION_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// Option flags for transformed image sources.
    /// </summary>
    public enum D2D1_TRANSFORMED_IMAGE_SOURCE_OPTIONS
    {
        D2D1_TRANSFORMED_IMAGE_SOURCE_OPTIONS_NONE = 0,

        /// <summary>
        /// Prevents the image source from being automatically scaled (by a ratio of the
        /// context DPI divided by 96) while drawn.
        /// </summary>
        D2D1_TRANSFORMED_IMAGE_SOURCE_OPTIONS_DISABLE_DPI_SCALE = 1,
        D2D1_TRANSFORMED_IMAGE_SOURCE_OPTIONS_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_EFFECT_INPUT_DESCRIPTION
    {
        /// <summary>
        /// The effect whose input connection is being specified.
        /// </summary>
        public ID2D1Effect effect;

        /// <summary>
        /// The index of the input connection into the specified effect.
        /// </summary>
        uint inputIndex;

        /// <summary>
        /// The rectangle which would be available on the specified input connection during
        /// render operations.
        /// </summary>
        D2D1_RECT_F inputRectangle;
    };

    [ComImport]
    [Guid("2c1d867d-c290-41c8-ae7e-34a98702e9a5")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1PrintControl
    {
        HRESULT AddPage(ID2D1CommandList commandList, D2D_SIZE_F pageSize, System.Runtime.InteropServices.ComTypes.IStream pagePrintTicketStream, out UInt64 tag1, out UInt64 tag2);
        HRESULT Close();
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D_SIZE_F
    {
        public float width;
        public float height;
    };

    [ComImport]
    [Guid("31e6e7bc-e0ff-4d46-8c64-a0a8c41c15d3")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Multithread
    {
        bool GetMultithreadProtected();
        void Enter();
        void Leave();
    }

    [ComImport]
    [Guid("47dd575d-ac05-4cdd-8049-9b02cd16f44c")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Device : ID2D1Resource
    {
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext deviceContext);
        //HRESULT CreatePrintControl(IWICImagingFactory wicFactory, IPrintDocumentPackageTarget documentTarget, D2D1_PRINT_CONTROL_PROPERTIES printControlProperties, out ID2D1PrintControl printControl);
        HRESULT CreatePrintControl(IWICImagingFactory wicFactory, IntPtr documentTarget, D2D1_PRINT_CONTROL_PROPERTIES printControlProperties, out ID2D1PrintControl printControl);
        void SetMaximumTextureMemory(UInt64 maximumInBytes);
        UInt64 GetMaximumTextureMemory();
        void ClearResources(uint millisecondsSinceUse = 0);
        //COM_DECLSPEC_NOTHROW
        //HRESULT
        //CreatePrintControl(
        //     IWICImagingFactory* wicFactory,
        //     IPrintDocumentPackageTarget* documentTarget,
        //    CONST D2D1_PRINT_CONTROL_PROPERTIES &printControlProperties,
        //    _COM_Outptr_ ID2D1PrintControl **printControl
        //    )
        //    {
        //        return CreatePrintControl(wicFactory, documentTarget, &printControlProperties, printControl);
        //    }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_PRINT_CONTROL_PROPERTIES
    {
        public D2D1_PRINT_FONT_SUBSET_MODE fontSubset;

        /// <summary>
        /// DPI for rasterization of all unsupported D2D commands or options, defaults to
        /// 150.0
        /// </summary>
        float rasterDPI;

        /// <summary>
        /// Color space for vector graphics in XPS package
        /// </summary>
        D2D1_COLOR_SPACE colorSpace;
    };

    public enum D2D1_PRINT_FONT_SUBSET_MODE
    {
        /// <summary>
        /// Subset for used glyphs, send and discard font resource after every five pages
        /// </summary>
        D2D1_PRINT_FONT_SUBSET_MODE_DEFAULT = 0,

        /// <summary>
        /// Subset for used glyphs, send and discard font resource after each page
        /// </summary>
        D2D1_PRINT_FONT_SUBSET_MODE_EACHPAGE = 1,

        /// <summary>
        /// Do not subset, reuse font for all pages, send it after first page
        /// </summary>
        D2D1_PRINT_FONT_SUBSET_MODE_NONE = 2,
        D2D1_PRINT_FONT_SUBSET_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_DEVICE_CONTEXT_OPTIONS
    {
        D2D1_DEVICE_CONTEXT_OPTIONS_NONE = 0,
        /// <summary>
        /// Geometry rendering will be performed on many threads in parallel, a single
        /// thread is the default.
        /// </summary>
        D2D1_DEVICE_CONTEXT_OPTIONS_ENABLE_MULTITHREADED_OPTIMIZATIONS = 1,
        D2D1_DEVICE_CONTEXT_OPTIONS_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [ComImport]
    [Guid("54d7898a-a061-40a7-bec7-e465bcba2c4f")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1CommandSink
    {
        HRESULT BeginDraw();
        HRESULT EndDraw();
        HRESULT SetAntialiasMode(D2D1_ANTIALIAS_MODE antialiasMode);
        HRESULT SetTags(UInt64 tag1, UInt64 tag2);
        HRESULT SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode);
        HRESULT SetTextRenderingParams(IDWriteRenderingParams textRenderingParams);
        HRESULT SetTransform(D2D1_MATRIX_3X2_F transform);
        HRESULT SetPrimitiveBlend(D2D1_PRIMITIVE_BLEND primitiveBlend);
        HRESULT SetUnitMode(D2D1_UNIT_MODE unitMode);
        HRESULT Clear(D2D1_COLOR_F color);
        HRESULT DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, DWRITE_GLYPH_RUN_DESCRIPTION glyphRunDescription, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        HRESULT DrawLine(ref D2D1_POINT_2F point0, D2D1_POINT_2F point1, ID2D1Brush brush, float strokeWidth, ID2D1StrokeStyle strokeStyle);
        HRESULT DrawGeometry(ID2D1Geometry geometry, ID2D1Brush brush, float strokeWidth, ID2D1StrokeStyle strokeStyle);
        HRESULT DrawRectangle(D2D1_RECT_F rect, ID2D1Brush brush, float strokeWidth, ID2D1StrokeStyle strokeStyle);
        HRESULT DrawBitmap(ID2D1Bitmap bitmap, D2D1_RECT_F destinationRectangle, float opacity, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_RECT_F sourceRectangle, D2D1_MATRIX_4X4_F perspectiveTransform);
        HRESULT DrawImage(ID2D1Image image, ref D2D1_POINT_2F targetOffset, D2D1_RECT_F imageRectangle, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_COMPOSITE_MODE compositeMode);
        HRESULT DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_POINT_2F targetOffset);
        HRESULT FillMesh(ID2D1Mesh mesh, ID2D1Brush brush);
        HRESULT FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        HRESULT FillGeometry(ID2D1Geometry geometry, ID2D1Brush brush, ID2D1Brush opacityBrush);
        HRESULT FillRectangle(D2D1_RECT_F rect, ID2D1Brush brush);
        HRESULT PushAxisAlignedClip(D2D1_RECT_F clipRect, D2D1_ANTIALIAS_MODE antialiasMode);
        HRESULT PushLayer(D2D1_LAYER_PARAMETERS1 layerParameters1, ID2D1Layer layer);
        HRESULT PopAxisAlignedClip();
        HRESULT PopLayer();
    }

    [ComImport]
    [Guid("82237326-8111-4f7c-bcf4-b5c1175564fe")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1GdiMetafileSink
    {
        HRESULT ProcessRecord(uint recordType, IntPtr recordData, uint recordDataSize);
    }

    [ComImport]
    [Guid("fd0ecb6b-91e6-411e-8655-395e760f91b4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1GdiMetafileSink1 : ID2D1GdiMetafileSink
    {
        #region <ID2D1GdiMetafileSink>
        new HRESULT ProcessRecord(uint recordType, IntPtr recordData, uint recordDataSize);
        #endregion

        HRESULT ProcessRecord(uint recordType, IntPtr recordData, uint recordDataSize, uint flags);
    }

    [ComImport]
    [Guid("2f543dc3-cfc1-4211-864f-cfd91c6f3395")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1GdiMetafile : ID2D1Resource
    {
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        HRESULT Stream(ID2D1GdiMetafileSink sink);
        HRESULT GetBounds(out D2D1_RECT_F bounds);
    }

    [ComImport]
    [Guid("2e69f9e8-dd3f-4bf9-95ba-c04f49d788df")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1GdiMetafile1 : ID2D1GdiMetafile
    {
        #region <ID2D1GdiMetafile>
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        new HRESULT Stream(ID2D1GdiMetafileSink sink);
        new HRESULT GetBounds(out D2D1_RECT_F bounds);
        #endregion

        HRESULT GetDpi(out float dpiX, out float dpiY);
        HRESULT GetSourceBounds(out D2D1_RECT_F bounds);
    }



    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_LAYER_PARAMETERS1
    {
        public D2D1_RECT_F contentBounds;
        public ID2D1Geometry geometricMask;
        public D2D1_ANTIALIAS_MODE maskAntialiasMode;
        public D2D1_MATRIX_3X2_F maskTransform;
        public float opacity;
        public ID2D1Brush opacityBrush;
        public D2D1_LAYER_OPTIONS1 layerOptions;
    };

    public enum D2D1_LAYER_OPTIONS1
    {
        D2D1_LAYER_OPTIONS1_NONE = 0,
        D2D1_LAYER_OPTIONS1_INITIALIZE_FROM_BACKGROUND = 1,
        D2D1_LAYER_OPTIONS1_IGNORE_ALPHA = 2,
        D2D1_LAYER_OPTIONS1_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [ComImport]
    [Guid("b4f34a19-2383-4d76-94f6-ec343657c3dc")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1CommandList : ID2D1Image
    {
        #region <ID2D1Image>
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion
        #endregion

        HRESULT Stream(ID2D1CommandSink sink);
        HRESULT Close();
    }

    [ComImport]
    [Guid("41343a53-e41a-49a2-91cd-21793bbb62e5")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1BitmapBrush1 : ID2D1BitmapBrush
    {
        #region <ID2D1BitmapBrush>

        #region ID2D1Brush
        new void SetOpacity(float opacity);
        new void SetTransform(D2D1_MATRIX_3X2_F transform);
        new float GetOpacity();
        new void GetTransform(out D2D1_MATRIX_3X2_F transform);
        #endregion
        new void SetExtendModeX(D2D1_EXTEND_MODE extendModeX);
        new void SetExtendModeY(D2D1_EXTEND_MODE extendModeY);
        new void SetInterpolationMode(D2D1_BITMAP_INTERPOLATION_MODE interpolationMode);
        new void SetBitmap(ID2D1Bitmap bitmap);
        new D2D1_EXTEND_MODE GetExtendModeX();
        new D2D1_EXTEND_MODE GetExtendModeY();
        new D2D1_BITMAP_INTERPOLATION_MODE GetInterpolationMode();
        new void GetBitmap(out ID2D1Bitmap bitmap);
        #endregion

        void SetInterpolationMode1(D2D1_INTERPOLATION_MODE interpolationMode);
        D2D1_INTERPOLATION_MODE GetInterpolationMode1();
    }

    [ComImport]
    [Guid("fe9e984d-3f95-407c-b5db-cb94d4e8f87c")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1ImageBrush : ID2D1Brush
    {
        #region <ID2D1Brush>
        #region ID2D1Resource
        new void GetFactory(out ID2D1Factory factory);
        #endregion
        new void SetOpacity(float opacity);
        new void SetTransform(D2D1_MATRIX_3X2_F transform);
        new float GetOpacity();
        new void GetTransform(out D2D1_MATRIX_3X2_F transform);
        #endregion

        void SetImage(ID2D1Image image);
        void SetExtendModeX(D2D1_EXTEND_MODE extendModeX);
        void SetExtendModeY(D2D1_EXTEND_MODE extendModeY);
        void SetInterpolationMode(D2D1_INTERPOLATION_MODE interpolationMode);
        void SetSourceRectangle(D2D1_RECT_F sourceRectangle);
        void GetImage(out ID2D1Image image);
        D2D1_EXTEND_MODE GetExtendModeX();
        D2D1_EXTEND_MODE GetExtendModeY();
        D2D1_INTERPOLATION_MODE GetInterpolationMode();
        void GetSourceRectangle(out D2D1_RECT_F sourceRectangle);
    }

    [ComImport]
    [Guid("ae1572f4-5dd0-4777-998b-9279472ae63b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1GradientStopCollection1 : ID2D1GradientStopCollection
    {
        #region <ID2D1GradientStopCollection>
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion
        new uint GetGradientStopCount();
        new void GetGradientStops(out D2D1_GRADIENT_STOP gradientStops, uint gradientStopsCount);
        new D2D1_GAMMA GetColorInterpolationGamma();
        new D2D1_EXTEND_MODE GetExtendMode();
        #endregion

        void GetGradientStops1(out D2D1_GRADIENT_STOP gradientStops, uint gradientStopsCount);
        D2D1_COLOR_SPACE GetPreInterpolationSpace();
        D2D1_COLOR_SPACE GetPostInterpolationSpace();
        D2D1_BUFFER_PRECISION GetBufferPrecision();
        D2D1_COLOR_INTERPOLATION_MODE GetColorInterpolationMode();
    }


    /// <summary>
    /// Specifies how the Crop effect handles the crop rectangle falling on fractional
    /// pixel coordinates.
    /// </summary>
    public enum D2D1_BORDER_MODE
    {
        D2D1_BORDER_MODE_SOFT = 0,
        D2D1_BORDER_MODE_HARD = 1,
        D2D1_BORDER_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }


    /// <summary>
    /// Specifies the color channel the Displacement map effect extracts the intensity
    /// from and uses it to spatially displace the image in the X or Y direction.
    /// </summary>
    public enum D2D1_CHANNEL_SELECTOR
    {
        D2D1_CHANNEL_SELECTOR_R = 0,
        D2D1_CHANNEL_SELECTOR_G = 1,
        D2D1_CHANNEL_SELECTOR_B = 2,
        D2D1_CHANNEL_SELECTOR_A = 3,
        D2D1_CHANNEL_SELECTOR_FORCE_DWORD = unchecked((int)0xffffffff)
    }


    /// <summary>
    /// Speficies whether a flip and/or rotation operation should be performed by the
    /// Bitmap source effect
    /// </summary>
    public enum D2D1_BITMAPSOURCE_ORIENTATION
    {
        D2D1_BITMAPSOURCE_ORIENTATION_DEFAULT = 1,
        D2D1_BITMAPSOURCE_ORIENTATION_FLIP_HORIZONTAL = 2,
        D2D1_BITMAPSOURCE_ORIENTATION_ROTATE_CLOCKWISE180 = 3,
        D2D1_BITMAPSOURCE_ORIENTATION_ROTATE_CLOCKWISE180_FLIP_HORIZONTAL = 4,
        D2D1_BITMAPSOURCE_ORIENTATION_ROTATE_CLOCKWISE270_FLIP_HORIZONTAL = 5,
        D2D1_BITMAPSOURCE_ORIENTATION_ROTATE_CLOCKWISE90 = 6,
        D2D1_BITMAPSOURCE_ORIENTATION_ROTATE_CLOCKWISE90_FLIP_HORIZONTAL = 7,
        D2D1_BITMAPSOURCE_ORIENTATION_ROTATE_CLOCKWISE270 = 8,
        D2D1_BITMAPSOURCE_ORIENTATION_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Gaussian Blur effect's top level properties.
    /// Effect description: Applies a gaussian blur to a bitmap with the specified blur
    /// radius and angle.
    /// </summary>
    public enum D2D1_GAUSSIANBLUR_PROP
    {
        /// <summary>
        /// Property Name: "StandardDeviation"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_GAUSSIANBLUR_PROP_STANDARD_DEVIATION = 0,

        /// <summary>
        /// Property Name: "Optimization"
        /// Property Type: D2D1_GAUSSIANBLUR_OPTIMIZATION
        /// </summary>
        D2D1_GAUSSIANBLUR_PROP_OPTIMIZATION = 1,

        /// <summary>
        /// Property Name: "BorderMode"
        /// Property Type: D2D1_BORDER_MODE
        /// </summary>
        D2D1_GAUSSIANBLUR_PROP_BORDER_MODE = 2,
        D2D1_GAUSSIANBLUR_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_GAUSSIANBLUR_OPTIMIZATION
    {
        D2D1_GAUSSIANBLUR_OPTIMIZATION_SPEED = 0,
        D2D1_GAUSSIANBLUR_OPTIMIZATION_BALANCED = 1,
        D2D1_GAUSSIANBLUR_OPTIMIZATION_QUALITY = 2,
        D2D1_GAUSSIANBLUR_OPTIMIZATION_FORCE_DWORD = unchecked((int)0xffffffff)
    }


    /// <summary>
    /// The enumeration of the Directional Blur effect's top level properties.
    /// Effect description: Applies a directional blur to a bitmap with the specified
    /// blur radius and angle.
    /// </summary>
    public enum D2D1_DIRECTIONALBLUR_PROP
    {
        /// <summary>
        /// Property Name: "StandardDeviation"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_DIRECTIONALBLUR_PROP_STANDARD_DEVIATION = 0,

        /// <summary>
        /// Property Name: "Angle"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_DIRECTIONALBLUR_PROP_ANGLE = 1,

        /// <summary>
        /// Property Name: "Optimization"
        /// Property Type: D2D1_DIRECTIONALBLUR_OPTIMIZATION
        /// </summary>
        D2D1_DIRECTIONALBLUR_PROP_OPTIMIZATION = 2,

        /// <summary>
        /// Property Name: "BorderMode"
        /// Property Type: D2D1_BORDER_MODE
        /// </summary>
        D2D1_DIRECTIONALBLUR_PROP_BORDER_MODE = 3,
        D2D1_DIRECTIONALBLUR_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_DIRECTIONALBLUR_OPTIMIZATION
    {
        D2D1_DIRECTIONALBLUR_OPTIMIZATION_SPEED = 0,
        D2D1_DIRECTIONALBLUR_OPTIMIZATION_BALANCED = 1,
        D2D1_DIRECTIONALBLUR_OPTIMIZATION_QUALITY = 2,
        D2D1_DIRECTIONALBLUR_OPTIMIZATION_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Shadow effect's top level properties.
    /// Effect description: Applies a shadow to a bitmap based on its alpha channel.
    /// </summary>
    public enum D2D1_SHADOW_PROP
    {
        /// <summary>
        /// Property Name: "BlurStandardDeviation"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_SHADOW_PROP_BLUR_STANDARD_DEVIATION = 0,

        /// <summary>
        /// Property Name: "Color"
        /// Property Type: D2D1_VECTOR_4F
        /// </summary>
        D2D1_SHADOW_PROP_COLOR = 1,

        /// <summary>
        /// Property Name: "Optimization"
        /// Property Type: D2D1_SHADOW_OPTIMIZATION
        /// </summary>
        D2D1_SHADOW_PROP_OPTIMIZATION = 2,
        D2D1_SHADOW_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_SHADOW_OPTIMIZATION
    {
        D2D1_SHADOW_OPTIMIZATION_SPEED = 0,
        D2D1_SHADOW_OPTIMIZATION_BALANCED = 1,
        D2D1_SHADOW_OPTIMIZATION_QUALITY = 2,
        D2D1_SHADOW_OPTIMIZATION_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Blend effect's top level properties.
    /// Effect description: Blends a foreground and background using a pre-defined blend
    /// mode.
    /// </summary>
    public enum D2D1_BLEND_PROP
    {
        /// <summary>
        /// Property Name: "Mode"
        /// Property Type: D2D1_BLEND_MODE
        /// </summary>
        D2D1_BLEND_PROP_MODE = 0,
        D2D1_BLEND_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_BLEND_MODE
    {
        D2D1_BLEND_MODE_MULTIPLY = 0,
        D2D1_BLEND_MODE_SCREEN = 1,
        D2D1_BLEND_MODE_DARKEN = 2,
        D2D1_BLEND_MODE_LIGHTEN = 3,
        D2D1_BLEND_MODE_DISSOLVE = 4,
        D2D1_BLEND_MODE_COLOR_BURN = 5,
        D2D1_BLEND_MODE_LINEAR_BURN = 6,
        D2D1_BLEND_MODE_DARKER_COLOR = 7,
        D2D1_BLEND_MODE_LIGHTER_COLOR = 8,
        D2D1_BLEND_MODE_COLOR_DODGE = 9,
        D2D1_BLEND_MODE_LINEAR_DODGE = 10,
        D2D1_BLEND_MODE_OVERLAY = 11,
        D2D1_BLEND_MODE_SOFT_LIGHT = 12,
        D2D1_BLEND_MODE_HARD_LIGHT = 13,
        D2D1_BLEND_MODE_VIVID_LIGHT = 14,
        D2D1_BLEND_MODE_LINEAR_LIGHT = 15,
        D2D1_BLEND_MODE_PIN_LIGHT = 16,
        D2D1_BLEND_MODE_HARD_MIX = 17,
        D2D1_BLEND_MODE_DIFFERENCE = 18,
        D2D1_BLEND_MODE_EXCLUSION = 19,
        D2D1_BLEND_MODE_HUE = 20,
        D2D1_BLEND_MODE_SATURATION = 21,
        D2D1_BLEND_MODE_COLOR = 22,
        D2D1_BLEND_MODE_LUMINOSITY = 23,
        D2D1_BLEND_MODE_SUBTRACT = 24,
        D2D1_BLEND_MODE_DIVISION = 25,
        D2D1_BLEND_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Saturation effect's top level properties.
    /// Effect description: Alters the saturation of the bitmap based on the user
    /// specified saturation value.
    /// </summary>
    public enum D2D1_SATURATION_PROP
    {
        /// <summary>
        /// Property Name: "Saturation"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_SATURATION_PROP_SATURATION = 0,
        D2D1_SATURATION_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Hue Rotation effect's top level properties.
    /// Effect description: Changes the Hue of a bitmap based on a user specified Hue
    /// Rotation angle.
    /// </summary>
    public enum D2D1_HUEROTATION_PROP
    {
        /// <summary>
        /// Property Name: "Angle"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_HUEROTATION_PROP_ANGLE = 0,
        D2D1_HUEROTATION_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }


    /// <summary>
    /// The enumeration of the Color Matrix effect's top level properties.
    /// Effect description: Applies a user specified color matrix to each channel of the
    /// input bitmap.
    /// </summary>
    public enum D2D1_COLORMATRIX_PROP
    {
        /// <summary>
        /// Property Name: "ColorMatrix"
        /// Property Type: D2D1_MATRIX_5X4_F
        /// </summary>
        D2D1_COLORMATRIX_PROP_COLOR_MATRIX = 0,

        /// <summary>
        /// Property Name: "AlphaMode"
        /// Property Type: D2D1_COLORMATRIX_ALPHA_MODE
        /// </summary>
        D2D1_COLORMATRIX_PROP_ALPHA_MODE = 1,

        /// <summary>
        /// Property Name: "ClampOutput"
        /// Property Type: BOOL
        /// </summary>
        D2D1_COLORMATRIX_PROP_CLAMP_OUTPUT = 2,
        D2D1_COLORMATRIX_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_COLORMATRIX_ALPHA_MODE
    {
        D2D1_COLORMATRIX_ALPHA_MODE_PREMULTIPLIED = 1,
        D2D1_COLORMATRIX_ALPHA_MODE_STRAIGHT = 2,
        D2D1_COLORMATRIX_ALPHA_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Bitmap Source effect's top level properties.
    /// Effect description: Provides an image source.
    /// </summary>
    public enum D2D1_BITMAPSOURCE_PROP
    {
        /// <summary>
        /// Property Name: "WicBitmapSource"
        /// Property Type: IUnknown *
        /// </summary>
        D2D1_BITMAPSOURCE_PROP_WIC_BITMAP_SOURCE = 0,

        /// <summary>
        /// Property Name: "Scale"
        /// Property Type: D2D1_VECTOR_2F
        /// </summary>
        D2D1_BITMAPSOURCE_PROP_SCALE = 1,

        /// <summary>
        /// Property Name: "InterpolationMode"
        /// Property Type: D2D1_BITMAPSOURCE_INTERPOLATION_MODE
        /// </summary>
        D2D1_BITMAPSOURCE_PROP_INTERPOLATION_MODE = 2,

        /// <summary>
        /// Property Name: "EnableDPICorrection"
        /// Property Type: BOOL
        /// </summary>
        D2D1_BITMAPSOURCE_PROP_ENABLE_DPI_CORRECTION = 3,

        /// <summary>
        /// Property Name: "AlphaMode"
        /// Property Type: D2D1_BITMAPSOURCE_ALPHA_MODE
        /// </summary>
        D2D1_BITMAPSOURCE_PROP_ALPHA_MODE = 4,

        /// <summary>
        /// Property Name: "Orientation"
        /// Property Type: D2D1_BITMAPSOURCE_ORIENTATION
        /// </summary>
        D2D1_BITMAPSOURCE_PROP_ORIENTATION = 5,
        D2D1_BITMAPSOURCE_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_BITMAPSOURCE_INTERPOLATION_MODE
    {
        D2D1_BITMAPSOURCE_INTERPOLATION_MODE_NEAREST_NEIGHBOR = 0,
        D2D1_BITMAPSOURCE_INTERPOLATION_MODE_LINEAR = 1,
        D2D1_BITMAPSOURCE_INTERPOLATION_MODE_CUBIC = 2,
        D2D1_BITMAPSOURCE_INTERPOLATION_MODE_FANT = 6,
        D2D1_BITMAPSOURCE_INTERPOLATION_MODE_MIPMAP_LINEAR = 7,
        D2D1_BITMAPSOURCE_INTERPOLATION_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_BITMAPSOURCE_ALPHA_MODE
    {
        D2D1_BITMAPSOURCE_ALPHA_MODE_PREMULTIPLIED = 1,
        D2D1_BITMAPSOURCE_ALPHA_MODE_STRAIGHT = 2,
        D2D1_BITMAPSOURCE_ALPHA_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Composite effect's top level properties.
    /// Effect description: Composites foreground and background images using the
    /// selected composition mode.
    /// </summary>
    public enum D2D1_COMPOSITE_PROP
    {
        /// <summary>
        /// Property Name: "Mode"
        /// Property Type: D2D1_COMPOSITE_MODE
        /// </summary>
        D2D1_COMPOSITE_PROP_MODE = 0,
        D2D1_COMPOSITE_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the 3D Transform effect's top level properties.
    /// Effect description: Applies a 3D transform to a bitmap.
    /// </summary>
    public enum D2D1_3DTRANSFORM_PROP
    {
        /// <summary>
        /// Property Name: "InterpolationMode"
        /// Property Type: D2D1_3DTRANSFORM_INTERPOLATION_MODE
        /// </summary>
        D2D1_3DTRANSFORM_PROP_INTERPOLATION_MODE = 0,

        /// <summary>
        /// Property Name: "BorderMode"
        /// Property Type: D2D1_BORDER_MODE
        /// </summary>
        D2D1_3DTRANSFORM_PROP_BORDER_MODE = 1,

        /// <summary>
        /// Property Name: "TransformMatrix"
        /// Property Type: D2D1_MATRIX_4X4_F
        /// </summary>
        D2D1_3DTRANSFORM_PROP_TRANSFORM_MATRIX = 2,
        D2D1_3DTRANSFORM_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_3DTRANSFORM_INTERPOLATION_MODE
    {
        D2D1_3DTRANSFORM_INTERPOLATION_MODE_NEAREST_NEIGHBOR = 0,
        D2D1_3DTRANSFORM_INTERPOLATION_MODE_LINEAR = 1,
        D2D1_3DTRANSFORM_INTERPOLATION_MODE_CUBIC = 2,
        D2D1_3DTRANSFORM_INTERPOLATION_MODE_MULTI_SAMPLE_LINEAR = 3,
        D2D1_3DTRANSFORM_INTERPOLATION_MODE_ANISOTROPIC = 4,
        D2D1_3DTRANSFORM_INTERPOLATION_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the 3D Perspective Transform effect's top level properties.
    /// Effect description: Applies a 3D perspective transform to a bitmap.
    /// </summary>
    public enum D2D1_3DPERSPECTIVETRANSFORM_PROP
    {
        /// <summary>
        /// Property Name: "InterpolationMode"
        /// Property Type: D2D1_3DPERSPECTIVETRANSFORM_INTERPOLATION_MODE
        /// </summary>
        D2D1_3DPERSPECTIVETRANSFORM_PROP_INTERPOLATION_MODE = 0,

        /// <summary>
        /// Property Name: "BorderMode"
        /// Property Type: D2D1_BORDER_MODE
        /// </summary>
        D2D1_3DPERSPECTIVETRANSFORM_PROP_BORDER_MODE = 1,

        /// <summary>
        /// Property Name: "Depth"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_3DPERSPECTIVETRANSFORM_PROP_DEPTH = 2,

        /// <summary>
        /// Property Name: "PerspectiveOrigin"
        /// Property Type: D2D1_VECTOR_2F
        /// </summary>
        D2D1_3DPERSPECTIVETRANSFORM_PROP_PERSPECTIVE_ORIGIN = 3,

        /// <summary>
        /// Property Name: "LocalOffset"
        /// Property Type: D2D1_VECTOR_3F
        /// </summary>
        D2D1_3DPERSPECTIVETRANSFORM_PROP_LOCAL_OFFSET = 4,

        /// <summary>
        /// Property Name: "GlobalOffset"
        /// Property Type: D2D1_VECTOR_3F
        /// </summary>
        D2D1_3DPERSPECTIVETRANSFORM_PROP_GLOBAL_OFFSET = 5,

        /// <summary>
        /// Property Name: "RotationOrigin"
        /// Property Type: D2D1_VECTOR_3F
        /// </summary>
        D2D1_3DPERSPECTIVETRANSFORM_PROP_ROTATION_ORIGIN = 6,

        /// <summary>
        /// Property Name: "Rotation"
        /// Property Type: D2D1_VECTOR_3F
        /// </summary>
        D2D1_3DPERSPECTIVETRANSFORM_PROP_ROTATION = 7,
        D2D1_3DPERSPECTIVETRANSFORM_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_3DPERSPECTIVETRANSFORM_INTERPOLATION_MODE
    {
        D2D1_3DPERSPECTIVETRANSFORM_INTERPOLATION_MODE_NEAREST_NEIGHBOR = 0,
        D2D1_3DPERSPECTIVETRANSFORM_INTERPOLATION_MODE_LINEAR = 1,
        D2D1_3DPERSPECTIVETRANSFORM_INTERPOLATION_MODE_CUBIC = 2,
        D2D1_3DPERSPECTIVETRANSFORM_INTERPOLATION_MODE_MULTI_SAMPLE_LINEAR = 3,
        D2D1_3DPERSPECTIVETRANSFORM_INTERPOLATION_MODE_ANISOTROPIC = 4,
        D2D1_3DPERSPECTIVETRANSFORM_INTERPOLATION_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the 2D Affine Transform effect's top level properties.
    /// Effect description: Applies a 2D affine transform to a bitmap.
    /// </summary>
    public enum D2D1_2DAFFINETRANSFORM_PROP
    {
        /// <summary>
        /// Property Name: "InterpolationMode"
        /// Property Type: D2D1_2DAFFINETRANSFORM_INTERPOLATION_MODE
        /// </summary>
        D2D1_2DAFFINETRANSFORM_PROP_INTERPOLATION_MODE = 0,

        /// <summary>
        /// Property Name: "BorderMode"
        /// Property Type: D2D1_BORDER_MODE
        /// </summary>
        D2D1_2DAFFINETRANSFORM_PROP_BORDER_MODE = 1,

        /// <summary>
        /// Property Name: "TransformMatrix"
        /// Property Type: D2D1_MATRIX_3X2_F
        /// </summary>
        D2D1_2DAFFINETRANSFORM_PROP_TRANSFORM_MATRIX = 2,

        /// <summary>
        /// Property Name: "Sharpness"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_2DAFFINETRANSFORM_PROP_SHARPNESS = 3,
        D2D1_2DAFFINETRANSFORM_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_2DAFFINETRANSFORM_INTERPOLATION_MODE
    {
        D2D1_2DAFFINETRANSFORM_INTERPOLATION_MODE_NEAREST_NEIGHBOR = 0,
        D2D1_2DAFFINETRANSFORM_INTERPOLATION_MODE_LINEAR = 1,
        D2D1_2DAFFINETRANSFORM_INTERPOLATION_MODE_CUBIC = 2,
        D2D1_2DAFFINETRANSFORM_INTERPOLATION_MODE_MULTI_SAMPLE_LINEAR = 3,
        D2D1_2DAFFINETRANSFORM_INTERPOLATION_MODE_ANISOTROPIC = 4,
        D2D1_2DAFFINETRANSFORM_INTERPOLATION_MODE_HIGH_QUALITY_CUBIC = 5,
        D2D1_2DAFFINETRANSFORM_INTERPOLATION_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the DPI Compensation effect's top level properties.
    /// Effect description: Scales according to the input DPI and the current context
    /// DPI
    /// </summary>
    public enum D2D1_DPICOMPENSATION_PROP
    {
        /// <summary>
        /// Property Name: "InterpolationMode"
        /// Property Type: D2D1_DPICOMPENSATION_INTERPOLATION_MODE
        /// </summary>
        D2D1_DPICOMPENSATION_PROP_INTERPOLATION_MODE = 0,

        /// <summary>
        /// Property Name: "BorderMode"
        /// Property Type: D2D1_BORDER_MODE
        /// </summary>
        D2D1_DPICOMPENSATION_PROP_BORDER_MODE = 1,

        /// <summary>
        /// Property Name: "InputDpi"
        /// Property Type: D2D1_VECTOR_2F
        /// </summary>
        D2D1_DPICOMPENSATION_PROP_INPUT_DPI = 2,
        D2D1_DPICOMPENSATION_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_DPICOMPENSATION_INTERPOLATION_MODE
    {
        D2D1_DPICOMPENSATION_INTERPOLATION_MODE_NEAREST_NEIGHBOR = 0,
        D2D1_DPICOMPENSATION_INTERPOLATION_MODE_LINEAR = 1,
        D2D1_DPICOMPENSATION_INTERPOLATION_MODE_CUBIC = 2,
        D2D1_DPICOMPENSATION_INTERPOLATION_MODE_MULTI_SAMPLE_LINEAR = 3,
        D2D1_DPICOMPENSATION_INTERPOLATION_MODE_ANISOTROPIC = 4,
        D2D1_DPICOMPENSATION_INTERPOLATION_MODE_HIGH_QUALITY_CUBIC = 5,
        D2D1_DPICOMPENSATION_INTERPOLATION_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Scale effect's top level properties.
    /// Effect description: Applies scaling operation to the bitmap.
    /// </summary>
    public enum D2D1_SCALE_PROP
    {
        /// <summary>
        /// Property Name: "Scale"
        /// Property Type: D2D1_VECTOR_2F
        /// </summary>
        D2D1_SCALE_PROP_SCALE = 0,

        /// <summary>
        /// Property Name: "CenterPoint"
        /// Property Type: D2D1_VECTOR_2F
        /// </summary>
        D2D1_SCALE_PROP_CENTER_POINT = 1,

        /// <summary>
        /// Property Name: "InterpolationMode"
        /// Property Type: D2D1_SCALE_INTERPOLATION_MODE
        /// </summary>
        D2D1_SCALE_PROP_INTERPOLATION_MODE = 2,

        /// <summary>
        /// Property Name: "BorderMode"
        /// Property Type: D2D1_BORDER_MODE
        /// </summary>
        D2D1_SCALE_PROP_BORDER_MODE = 3,

        /// <summary>
        /// Property Name: "Sharpness"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_SCALE_PROP_SHARPNESS = 4,
        D2D1_SCALE_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_SCALE_INTERPOLATION_MODE
    {
        D2D1_SCALE_INTERPOLATION_MODE_NEAREST_NEIGHBOR = 0,
        D2D1_SCALE_INTERPOLATION_MODE_LINEAR = 1,
        D2D1_SCALE_INTERPOLATION_MODE_CUBIC = 2,
        D2D1_SCALE_INTERPOLATION_MODE_MULTI_SAMPLE_LINEAR = 3,
        D2D1_SCALE_INTERPOLATION_MODE_ANISOTROPIC = 4,
        D2D1_SCALE_INTERPOLATION_MODE_HIGH_QUALITY_CUBIC = 5,
        D2D1_SCALE_INTERPOLATION_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Turbulence effect's top level properties.
    /// Effect description: Generates a bitmap based on the Perlin noise turbulence
    /// function.
    /// </summary>
    public enum D2D1_TURBULENCE_PROP
    {
        /// <summary>
        /// Property Name: "Offset"
        /// Property Type: D2D1_VECTOR_2F
        /// </summary>
        D2D1_TURBULENCE_PROP_OFFSET = 0,

        /// <summary>
        /// Property Name: "Size"
        /// Property Type: D2D1_VECTOR_2F
        /// </summary>
        D2D1_TURBULENCE_PROP_SIZE = 1,

        /// <summary>
        /// Property Name: "BaseFrequency"
        /// Property Type: D2D1_VECTOR_2F
        /// </summary>
        D2D1_TURBULENCE_PROP_BASE_FREQUENCY = 2,

        /// <summary>
        /// Property Name: "NumOctaves"
        /// Property Type: UINT32
        /// </summary>
        D2D1_TURBULENCE_PROP_NUM_OCTAVES = 3,

        /// <summary>
        /// Property Name: "Seed"
        /// Property Type: INT32
        /// </summary>
        D2D1_TURBULENCE_PROP_SEED = 4,

        /// <summary>
        /// Property Name: "Noise"
        /// Property Type: D2D1_TURBULENCE_NOISE
        /// </summary>
        D2D1_TURBULENCE_PROP_NOISE = 5,

        /// <summary>
        /// Property Name: "Stitchable"
        /// Property Type: BOOL
        /// </summary>
        D2D1_TURBULENCE_PROP_STITCHABLE = 6,
        D2D1_TURBULENCE_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_TURBULENCE_NOISE
    {
        D2D1_TURBULENCE_NOISE_FRACTAL_SUM = 0,
        D2D1_TURBULENCE_NOISE_TURBULENCE = 1,
        D2D1_TURBULENCE_NOISE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Displacement Map effect's top level properties.
    /// Effect description: Displaces a bitmap based on user specified setting and
    /// another bitmap.
    /// </summary>
    public enum D2D1_DISPLACEMENTMAP_PROP
    {
        /// <summary>
        /// Property Name: "Scale"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_DISPLACEMENTMAP_PROP_SCALE = 0,

        /// <summary>
        /// Property Name: "XChannelSelect"
        /// Property Type: D2D1_CHANNEL_SELECTOR
        /// </summary>
        D2D1_DISPLACEMENTMAP_PROP_X_CHANNEL_SELECT = 1,

        /// <summary>
        /// Property Name: "YChannelSelect"
        /// Property Type: D2D1_CHANNEL_SELECTOR
        /// </summary>
        D2D1_DISPLACEMENTMAP_PROP_Y_CHANNEL_SELECT = 2,
        D2D1_DISPLACEMENTMAP_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Color Management effect's top level properties.
    /// Effect description: Changes colors based on user provided color contexts.
    /// </summary>
    public enum D2D1_COLORMANAGEMENT_PROP
    {
        /// <summary>
        /// Property Name: "SourceColorContext"
        /// Property Type: ID2D1ColorContext *
        /// </summary>
        D2D1_COLORMANAGEMENT_PROP_SOURCE_COLOR_CONTEXT = 0,

        /// <summary>
        /// Property Name: "SourceRenderingIntent"
        /// Property Type: D2D1_RENDERING_INTENT
        /// </summary>
        D2D1_COLORMANAGEMENT_PROP_SOURCE_RENDERING_INTENT = 1,

        /// <summary>
        /// Property Name: "DestinationColorContext"
        /// Property Type: ID2D1ColorContext *
        /// </summary>
        D2D1_COLORMANAGEMENT_PROP_DESTINATION_COLOR_CONTEXT = 2,

        /// <summary>
        /// Property Name: "DestinationRenderingIntent"
        /// Property Type: D2D1_RENDERING_INTENT
        /// </summary>
        D2D1_COLORMANAGEMENT_PROP_DESTINATION_RENDERING_INTENT = 3,

        /// <summary>
        /// Property Name: "AlphaMode"
        /// Property Type: D2D1_COLORMANAGEMENT_ALPHA_MODE
        /// </summary>
        D2D1_COLORMANAGEMENT_PROP_ALPHA_MODE = 4,

        /// <summary>
        /// Property Name: "Quality"
        /// Property Type: D2D1_COLORMANAGEMENT_QUALITY
        /// </summary>
        D2D1_COLORMANAGEMENT_PROP_QUALITY = 5,
        D2D1_COLORMANAGEMENT_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_COLORMANAGEMENT_ALPHA_MODE
    {
        D2D1_COLORMANAGEMENT_ALPHA_MODE_PREMULTIPLIED = 1,
        D2D1_COLORMANAGEMENT_ALPHA_MODE_STRAIGHT = 2,
        D2D1_COLORMANAGEMENT_ALPHA_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_COLORMANAGEMENT_QUALITY
    {
        D2D1_COLORMANAGEMENT_QUALITY_PROOF = 0,
        D2D1_COLORMANAGEMENT_QUALITY_NORMAL = 1,
        D2D1_COLORMANAGEMENT_QUALITY_BEST = 2,
        D2D1_COLORMANAGEMENT_QUALITY_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// Specifies which ICC rendering intent the Color management effect should use.
    /// </summary>
    public enum D2D1_COLORMANAGEMENT_RENDERING_INTENT
    {
        D2D1_COLORMANAGEMENT_RENDERING_INTENT_PERCEPTUAL = 0,
        D2D1_COLORMANAGEMENT_RENDERING_INTENT_RELATIVE_COLORIMETRIC = 1,
        D2D1_COLORMANAGEMENT_RENDERING_INTENT_SATURATION = 2,
        D2D1_COLORMANAGEMENT_RENDERING_INTENT_ABSOLUTE_COLORIMETRIC = 3,
        D2D1_COLORMANAGEMENT_RENDERING_INTENT_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Histogram effect's top level properties.
    /// Effect description: Computes the histogram of an image.
    /// </summary>
    public enum D2D1_HISTOGRAM_PROP
    {
        /// <summary>
        /// Property Name: "NumBins"
        /// Property Type: UINT32
        /// </summary>
        D2D1_HISTOGRAM_PROP_NUM_BINS = 0,

        /// <summary>
        /// Property Name: "ChannelSelect"
        /// Property Type: D2D1_CHANNEL_SELECTOR
        /// </summary>
        D2D1_HISTOGRAM_PROP_CHANNEL_SELECT = 1,

        /// <summary>
        /// Property Name: "HistogramOutput"
        /// Property Type: (blob)
        /// </summary>
        D2D1_HISTOGRAM_PROP_HISTOGRAM_OUTPUT = 2,
        D2D1_HISTOGRAM_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Point-Specular effect's top level properties.
    /// Effect description: Creates a specular lighting effect with a point light
    /// source.
    /// </summary>
    public enum D2D1_POINTSPECULAR_PROP
    {
        /// <summary>
        /// Property Name: "LightPosition"
        /// Property Type: D2D1_VECTOR_3F
        /// </summary>
        D2D1_POINTSPECULAR_PROP_LIGHT_POSITION = 0,

        /// <summary>
        /// Property Name: "SpecularExponent"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_POINTSPECULAR_PROP_SPECULAR_EXPONENT = 1,

        /// <summary>
        /// Property Name: "SpecularConstant"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_POINTSPECULAR_PROP_SPECULAR_CONSTANT = 2,

        /// <summary>
        /// Property Name: "SurfaceScale"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_POINTSPECULAR_PROP_SURFACE_SCALE = 3,

        /// <summary>
        /// Property Name: "Color"
        /// Property Type: D2D1_VECTOR_3F
        /// </summary>
        D2D1_POINTSPECULAR_PROP_COLOR = 4,

        /// <summary>
        /// Property Name: "KernelUnitLength"
        /// Property Type: D2D1_VECTOR_2F
        /// </summary>
        D2D1_POINTSPECULAR_PROP_KERNEL_UNIT_LENGTH = 5,

        /// <summary>
        /// Property Name: "ScaleMode"
        /// Property Type: D2D1_POINTSPECULAR_SCALE_MODE
        /// </summary>
        D2D1_POINTSPECULAR_PROP_SCALE_MODE = 6,
        D2D1_POINTSPECULAR_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_POINTSPECULAR_SCALE_MODE
    {
        D2D1_POINTSPECULAR_SCALE_MODE_NEAREST_NEIGHBOR = 0,
        D2D1_POINTSPECULAR_SCALE_MODE_LINEAR = 1,
        D2D1_POINTSPECULAR_SCALE_MODE_CUBIC = 2,
        D2D1_POINTSPECULAR_SCALE_MODE_MULTI_SAMPLE_LINEAR = 3,
        D2D1_POINTSPECULAR_SCALE_MODE_ANISOTROPIC = 4,
        D2D1_POINTSPECULAR_SCALE_MODE_HIGH_QUALITY_CUBIC = 5,
        D2D1_POINTSPECULAR_SCALE_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }


    /// <summary>
    /// The enumeration of the Spot-Specular effect's top level properties.
    /// Effect description: Creates a specular lighting effect with a spot light source.
    /// </summary>
    public enum D2D1_SPOTSPECULAR_PROP
    {
        /// <summary>
        /// Property Name: "LightPosition"
        /// Property Type: D2D1_VECTOR_3F
        /// </summary>
        D2D1_SPOTSPECULAR_PROP_LIGHT_POSITION = 0,

        /// <summary>
        /// Property Name: "PointsAt"
        /// Property Type: D2D1_VECTOR_3F
        /// </summary>
        D2D1_SPOTSPECULAR_PROP_POINTS_AT = 1,

        /// <summary>
        /// Property Name: "Focus"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_SPOTSPECULAR_PROP_FOCUS = 2,

        /// <summary>
        /// Property Name: "LimitingConeAngle"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_SPOTSPECULAR_PROP_LIMITING_CONE_ANGLE = 3,

        /// <summary>
        /// Property Name: "SpecularExponent"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_SPOTSPECULAR_PROP_SPECULAR_EXPONENT = 4,

        /// <summary>
        /// Property Name: "SpecularConstant"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_SPOTSPECULAR_PROP_SPECULAR_CONSTANT = 5,

        /// <summary>
        /// Property Name: "SurfaceScale"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_SPOTSPECULAR_PROP_SURFACE_SCALE = 6,

        /// <summary>
        /// Property Name: "Color"
        /// Property Type: D2D1_VECTOR_3F
        /// </summary>
        D2D1_SPOTSPECULAR_PROP_COLOR = 7,

        /// <summary>
        /// Property Name: "KernelUnitLength"
        /// Property Type: D2D1_VECTOR_2F
        /// </summary>
        D2D1_SPOTSPECULAR_PROP_KERNEL_UNIT_LENGTH = 8,

        /// <summary>
        /// Property Name: "ScaleMode"
        /// Property Type: D2D1_SPOTSPECULAR_SCALE_MODE
        /// </summary>
        D2D1_SPOTSPECULAR_PROP_SCALE_MODE = 9,
        D2D1_SPOTSPECULAR_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_SPOTSPECULAR_SCALE_MODE
    {
        D2D1_SPOTSPECULAR_SCALE_MODE_NEAREST_NEIGHBOR = 0,
        D2D1_SPOTSPECULAR_SCALE_MODE_LINEAR = 1,
        D2D1_SPOTSPECULAR_SCALE_MODE_CUBIC = 2,
        D2D1_SPOTSPECULAR_SCALE_MODE_MULTI_SAMPLE_LINEAR = 3,
        D2D1_SPOTSPECULAR_SCALE_MODE_ANISOTROPIC = 4,
        D2D1_SPOTSPECULAR_SCALE_MODE_HIGH_QUALITY_CUBIC = 5,
        D2D1_SPOTSPECULAR_SCALE_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Distant-Specular effect's top level properties.
    /// Effect description: Creates a specular lighting effect with a distant light
    /// source.
    /// </summary>
    public enum D2D1_DISTANTSPECULAR_PROP
    {
        /// <summary>
        /// Property Name: "Azimuth"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_DISTANTSPECULAR_PROP_AZIMUTH = 0,

        /// <summary>
        /// Property Name: "Elevation"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_DISTANTSPECULAR_PROP_ELEVATION = 1,

        /// <summary>
        /// Property Name: "SpecularExponent"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_DISTANTSPECULAR_PROP_SPECULAR_EXPONENT = 2,

        /// <summary>
        /// Property Name: "SpecularConstant"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_DISTANTSPECULAR_PROP_SPECULAR_CONSTANT = 3,

        /// <summary>
        /// Property Name: "SurfaceScale"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_DISTANTSPECULAR_PROP_SURFACE_SCALE = 4,

        /// <summary>
        /// Property Name: "Color"
        /// Property Type: D2D1_VECTOR_3F
        /// </summary>
        D2D1_DISTANTSPECULAR_PROP_COLOR = 5,

        /// <summary>
        /// Property Name: "KernelUnitLength"
        /// Property Type: D2D1_VECTOR_2F
        /// </summary>
        D2D1_DISTANTSPECULAR_PROP_KERNEL_UNIT_LENGTH = 6,

        /// <summary>
        /// Property Name: "ScaleMode"
        /// Property Type: D2D1_DISTANTSPECULAR_SCALE_MODE
        /// </summary>
        D2D1_DISTANTSPECULAR_PROP_SCALE_MODE = 7,
        D2D1_DISTANTSPECULAR_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_DISTANTSPECULAR_SCALE_MODE
    {
        D2D1_DISTANTSPECULAR_SCALE_MODE_NEAREST_NEIGHBOR = 0,
        D2D1_DISTANTSPECULAR_SCALE_MODE_LINEAR = 1,
        D2D1_DISTANTSPECULAR_SCALE_MODE_CUBIC = 2,
        D2D1_DISTANTSPECULAR_SCALE_MODE_MULTI_SAMPLE_LINEAR = 3,
        D2D1_DISTANTSPECULAR_SCALE_MODE_ANISOTROPIC = 4,
        D2D1_DISTANTSPECULAR_SCALE_MODE_HIGH_QUALITY_CUBIC = 5,
        D2D1_DISTANTSPECULAR_SCALE_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Point-Diffuse effect's top level properties.
    /// Effect description: Creates a diffuse lighting effect with a point light source.
    /// </summary>
    public enum D2D1_POINTDIFFUSE_PROP
    {
        /// <summary>
        /// Property Name: "LightPosition"
        /// Property Type: D2D1_VECTOR_3F
        /// </summary>
        D2D1_POINTDIFFUSE_PROP_LIGHT_POSITION = 0,

        /// <summary>
        /// Property Name: "DiffuseConstant"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_POINTDIFFUSE_PROP_DIFFUSE_CONSTANT = 1,

        /// <summary>
        /// Property Name: "SurfaceScale"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_POINTDIFFUSE_PROP_SURFACE_SCALE = 2,

        /// <summary>
        /// Property Name: "Color"
        /// Property Type: D2D1_VECTOR_3F
        /// </summary>
        D2D1_POINTDIFFUSE_PROP_COLOR = 3,

        /// <summary>
        /// Property Name: "KernelUnitLength"
        /// Property Type: D2D1_VECTOR_2F
        /// </summary>
        D2D1_POINTDIFFUSE_PROP_KERNEL_UNIT_LENGTH = 4,

        /// <summary>
        /// Property Name: "ScaleMode"
        /// Property Type: D2D1_POINTDIFFUSE_SCALE_MODE
        /// </summary>
        D2D1_POINTDIFFUSE_PROP_SCALE_MODE = 5,
        D2D1_POINTDIFFUSE_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_POINTDIFFUSE_SCALE_MODE
    {
        D2D1_POINTDIFFUSE_SCALE_MODE_NEAREST_NEIGHBOR = 0,
        D2D1_POINTDIFFUSE_SCALE_MODE_LINEAR = 1,
        D2D1_POINTDIFFUSE_SCALE_MODE_CUBIC = 2,
        D2D1_POINTDIFFUSE_SCALE_MODE_MULTI_SAMPLE_LINEAR = 3,
        D2D1_POINTDIFFUSE_SCALE_MODE_ANISOTROPIC = 4,
        D2D1_POINTDIFFUSE_SCALE_MODE_HIGH_QUALITY_CUBIC = 5,
        D2D1_POINTDIFFUSE_SCALE_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Spot-Diffuse effect's top level properties.
    /// Effect description: Creates a diffuse lighting effect with a spot light source.
    /// </summary>
    public enum D2D1_SPOTDIFFUSE_PROP
    {
        /// <summary>
        /// Property Name: "LightPosition"
        /// Property Type: D2D1_VECTOR_3F
        /// </summary>
        D2D1_SPOTDIFFUSE_PROP_LIGHT_POSITION = 0,

        /// <summary>
        /// Property Name: "PointsAt"
        /// Property Type: D2D1_VECTOR_3F
        /// </summary>
        D2D1_SPOTDIFFUSE_PROP_POINTS_AT = 1,

        /// <summary>
        /// Property Name: "Focus"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_SPOTDIFFUSE_PROP_FOCUS = 2,

        /// <summary>
        /// Property Name: "LimitingConeAngle"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_SPOTDIFFUSE_PROP_LIMITING_CONE_ANGLE = 3,

        /// <summary>
        /// Property Name: "DiffuseConstant"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_SPOTDIFFUSE_PROP_DIFFUSE_CONSTANT = 4,

        /// <summary>
        /// Property Name: "SurfaceScale"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_SPOTDIFFUSE_PROP_SURFACE_SCALE = 5,

        /// <summary>
        /// Property Name: "Color"
        /// Property Type: D2D1_VECTOR_3F
        /// </summary>
        D2D1_SPOTDIFFUSE_PROP_COLOR = 6,

        /// <summary>
        /// Property Name: "KernelUnitLength"
        /// Property Type: D2D1_VECTOR_2F
        /// </summary>
        D2D1_SPOTDIFFUSE_PROP_KERNEL_UNIT_LENGTH = 7,

        /// <summary>
        /// Property Name: "ScaleMode"
        /// Property Type: D2D1_SPOTDIFFUSE_SCALE_MODE
        /// </summary>
        D2D1_SPOTDIFFUSE_PROP_SCALE_MODE = 8,
        D2D1_SPOTDIFFUSE_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_SPOTDIFFUSE_SCALE_MODE
    {
        D2D1_SPOTDIFFUSE_SCALE_MODE_NEAREST_NEIGHBOR = 0,
        D2D1_SPOTDIFFUSE_SCALE_MODE_LINEAR = 1,
        D2D1_SPOTDIFFUSE_SCALE_MODE_CUBIC = 2,
        D2D1_SPOTDIFFUSE_SCALE_MODE_MULTI_SAMPLE_LINEAR = 3,
        D2D1_SPOTDIFFUSE_SCALE_MODE_ANISOTROPIC = 4,
        D2D1_SPOTDIFFUSE_SCALE_MODE_HIGH_QUALITY_CUBIC = 5,
        D2D1_SPOTDIFFUSE_SCALE_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Distant-Diffuse effect's top level properties.
    /// Effect description: Creates a diffuse lighting effect with a distant light
    /// source.
    /// </summary>
    public enum D2D1_DISTANTDIFFUSE_PROP
    {
        /// <summary>
        /// Property Name: "Azimuth"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_DISTANTDIFFUSE_PROP_AZIMUTH = 0,

        /// <summary>
        /// Property Name: "Elevation"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_DISTANTDIFFUSE_PROP_ELEVATION = 1,

        /// <summary>
        /// Property Name: "DiffuseConstant"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_DISTANTDIFFUSE_PROP_DIFFUSE_CONSTANT = 2,

        /// <summary>
        /// Property Name: "SurfaceScale"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_DISTANTDIFFUSE_PROP_SURFACE_SCALE = 3,

        /// <summary>
        /// Property Name: "Color"
        /// Property Type: D2D1_VECTOR_3F
        /// </summary>
        D2D1_DISTANTDIFFUSE_PROP_COLOR = 4,

        /// <summary>
        /// Property Name: "KernelUnitLength"
        /// Property Type: D2D1_VECTOR_2F
        /// </summary>
        D2D1_DISTANTDIFFUSE_PROP_KERNEL_UNIT_LENGTH = 5,

        /// <summary>
        /// Property Name: "ScaleMode"
        /// Property Type: D2D1_DISTANTDIFFUSE_SCALE_MODE
        /// </summary>
        D2D1_DISTANTDIFFUSE_PROP_SCALE_MODE = 6,
        D2D1_DISTANTDIFFUSE_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_DISTANTDIFFUSE_SCALE_MODE
    {
        D2D1_DISTANTDIFFUSE_SCALE_MODE_NEAREST_NEIGHBOR = 0,
        D2D1_DISTANTDIFFUSE_SCALE_MODE_LINEAR = 1,
        D2D1_DISTANTDIFFUSE_SCALE_MODE_CUBIC = 2,
        D2D1_DISTANTDIFFUSE_SCALE_MODE_MULTI_SAMPLE_LINEAR = 3,
        D2D1_DISTANTDIFFUSE_SCALE_MODE_ANISOTROPIC = 4,
        D2D1_DISTANTDIFFUSE_SCALE_MODE_HIGH_QUALITY_CUBIC = 5,
        D2D1_DISTANTDIFFUSE_SCALE_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Flood effect's top level properties.
    /// Effect description: Renders an infinite sized floodfill of the given color.
    /// </summary>
    public enum D2D1_FLOOD_PROP
    {
        /// <summary>
        /// Property Name: "Color"
        /// Property Type: D2D1_VECTOR_4F
        /// </summary>
        D2D1_FLOOD_PROP_COLOR = 0,
        D2D1_FLOOD_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Linear Transfer effect's top level properties.
    /// Effect description: Remaps the color intensities of the input bitmap based on a
    /// user specified linear transfer function for each RGBA channel.
    /// </summary>
    public enum D2D1_LINEARTRANSFER_PROP
    {
        /// <summary>
        /// Property Name: "RedYIntercept"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_LINEARTRANSFER_PROP_RED_Y_INTERCEPT = 0,

        /// <summary>
        /// Property Name: "RedSlope"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_LINEARTRANSFER_PROP_RED_SLOPE = 1,

        /// <summary>
        /// Property Name: "RedDisable"
        /// Property Type: BOOL
        /// </summary>
        D2D1_LINEARTRANSFER_PROP_RED_DISABLE = 2,

        /// <summary>
        /// Property Name: "GreenYIntercept"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_LINEARTRANSFER_PROP_GREEN_Y_INTERCEPT = 3,

        /// <summary>
        /// Property Name: "GreenSlope"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_LINEARTRANSFER_PROP_GREEN_SLOPE = 4,

        /// <summary>
        /// Property Name: "GreenDisable"
        /// Property Type: BOOL
        /// </summary>
        D2D1_LINEARTRANSFER_PROP_GREEN_DISABLE = 5,

        /// <summary>
        /// Property Name: "BlueYIntercept"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_LINEARTRANSFER_PROP_BLUE_Y_INTERCEPT = 6,

        /// <summary>
        /// Property Name: "BlueSlope"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_LINEARTRANSFER_PROP_BLUE_SLOPE = 7,

        /// <summary>
        /// Property Name: "BlueDisable"
        /// Property Type: BOOL
        /// </summary>
        D2D1_LINEARTRANSFER_PROP_BLUE_DISABLE = 8,

        /// <summary>
        /// Property Name: "AlphaYIntercept"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_LINEARTRANSFER_PROP_ALPHA_Y_INTERCEPT = 9,

        /// <summary>
        /// Property Name: "AlphaSlope"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_LINEARTRANSFER_PROP_ALPHA_SLOPE = 10,

        /// <summary>
        /// Property Name: "AlphaDisable"
        /// Property Type: BOOL
        /// </summary>
        D2D1_LINEARTRANSFER_PROP_ALPHA_DISABLE = 11,

        /// <summary>
        /// Property Name: "ClampOutput"
        /// Property Type: BOOL
        /// </summary>
        D2D1_LINEARTRANSFER_PROP_CLAMP_OUTPUT = 12,
        D2D1_LINEARTRANSFER_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Gamma Transfer effect's top level properties.
    /// Effect description: Remaps the color intensities of the input bitmap based on a
    /// user specified gamma transfer function for each RGBA channel.
    /// </summary>
    public enum D2D1_GAMMATRANSFER_PROP
    {
        /// <summary>
        /// Property Name: "RedAmplitude"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_GAMMATRANSFER_PROP_RED_AMPLITUDE = 0,

        /// <summary>
        /// Property Name: "RedExponent"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_GAMMATRANSFER_PROP_RED_EXPONENT = 1,

        /// <summary>
        /// Property Name: "RedOffset"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_GAMMATRANSFER_PROP_RED_OFFSET = 2,

        /// <summary>
        /// Property Name: "RedDisable"
        /// Property Type: BOOL
        /// </summary>
        D2D1_GAMMATRANSFER_PROP_RED_DISABLE = 3,

        /// <summary>
        /// Property Name: "GreenAmplitude"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_GAMMATRANSFER_PROP_GREEN_AMPLITUDE = 4,

        /// <summary>
        /// Property Name: "GreenExponent"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_GAMMATRANSFER_PROP_GREEN_EXPONENT = 5,

        /// <summary>
        /// Property Name: "GreenOffset"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_GAMMATRANSFER_PROP_GREEN_OFFSET = 6,

        /// <summary>
        /// Property Name: "GreenDisable"
        /// Property Type: BOOL
        /// </summary>
        D2D1_GAMMATRANSFER_PROP_GREEN_DISABLE = 7,

        /// <summary>
        /// Property Name: "BlueAmplitude"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_GAMMATRANSFER_PROP_BLUE_AMPLITUDE = 8,

        /// <summary>
        /// Property Name: "BlueExponent"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_GAMMATRANSFER_PROP_BLUE_EXPONENT = 9,

        /// <summary>
        /// Property Name: "BlueOffset"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_GAMMATRANSFER_PROP_BLUE_OFFSET = 10,

        /// <summary>
        /// Property Name: "BlueDisable"
        /// Property Type: BOOL
        /// </summary>
        D2D1_GAMMATRANSFER_PROP_BLUE_DISABLE = 11,

        /// <summary>
        /// Property Name: "AlphaAmplitude"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_GAMMATRANSFER_PROP_ALPHA_AMPLITUDE = 12,

        /// <summary>
        /// Property Name: "AlphaExponent"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_GAMMATRANSFER_PROP_ALPHA_EXPONENT = 13,

        /// <summary>
        /// Property Name: "AlphaOffset"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_GAMMATRANSFER_PROP_ALPHA_OFFSET = 14,

        /// <summary>
        /// Property Name: "AlphaDisable"
        /// Property Type: BOOL
        /// </summary>
        D2D1_GAMMATRANSFER_PROP_ALPHA_DISABLE = 15,

        /// <summary>
        /// Property Name: "ClampOutput"
        /// Property Type: BOOL
        /// </summary>
        D2D1_GAMMATRANSFER_PROP_CLAMP_OUTPUT = 16,
        D2D1_GAMMATRANSFER_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Table Transfer effect's top level properties.
    /// Effect description: Remaps the color intensities of the input bitmap based on a
    /// transfer function generated by a user specified list of values for each RGBA
    /// channel.
    /// </summary>
    public enum D2D1_TABLETRANSFER_PROP
    {
        /// <summary>
        /// Property Name: "RedTable"
        /// Property Type: (blob)
        /// </summary>
        D2D1_TABLETRANSFER_PROP_RED_TABLE = 0,

        /// <summary>
        /// Property Name: "RedDisable"
        /// Property Type: BOOL
        /// </summary>
        D2D1_TABLETRANSFER_PROP_RED_DISABLE = 1,

        /// <summary>
        /// Property Name: "GreenTable"
        /// Property Type: (blob)
        /// </summary>
        D2D1_TABLETRANSFER_PROP_GREEN_TABLE = 2,

        /// <summary>
        /// Property Name: "GreenDisable"
        /// Property Type: BOOL
        /// </summary>
        D2D1_TABLETRANSFER_PROP_GREEN_DISABLE = 3,

        /// <summary>
        /// Property Name: "BlueTable"
        /// Property Type: (blob)
        /// </summary>
        D2D1_TABLETRANSFER_PROP_BLUE_TABLE = 4,

        /// <summary>
        /// Property Name: "BlueDisable"
        /// Property Type: BOOL
        /// </summary>
        D2D1_TABLETRANSFER_PROP_BLUE_DISABLE = 5,

        /// <summary>
        /// Property Name: "AlphaTable"
        /// Property Type: (blob)
        /// </summary>
        D2D1_TABLETRANSFER_PROP_ALPHA_TABLE = 6,

        /// <summary>
        /// Property Name: "AlphaDisable"
        /// Property Type: BOOL
        /// </summary>
        D2D1_TABLETRANSFER_PROP_ALPHA_DISABLE = 7,

        /// <summary>
        /// Property Name: "ClampOutput"
        /// Property Type: BOOL
        /// </summary>
        D2D1_TABLETRANSFER_PROP_CLAMP_OUTPUT = 8,
        D2D1_TABLETRANSFER_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Discrete Transfer effect's top level properties.
    /// Effect description: Remaps the color intensities of the input bitmap based on a
    /// discrete function generated by a user specified list of values for each RGBA
    /// channel.
    /// </summary>
    public enum D2D1_DISCRETETRANSFER_PROP
    {
        /// <summary>
        /// Property Name: "RedTable"
        /// Property Type: (blob)
        /// </summary>
        D2D1_DISCRETETRANSFER_PROP_RED_TABLE = 0,

        /// <summary>
        /// Property Name: "RedDisable"
        /// Property Type: BOOL
        /// </summary>
        D2D1_DISCRETETRANSFER_PROP_RED_DISABLE = 1,

        /// <summary>
        /// Property Name: "GreenTable"
        /// Property Type: (blob)
        /// </summary>
        D2D1_DISCRETETRANSFER_PROP_GREEN_TABLE = 2,

        /// <summary>
        /// Property Name: "GreenDisable"
        /// Property Type: BOOL
        /// </summary>
        D2D1_DISCRETETRANSFER_PROP_GREEN_DISABLE = 3,

        /// <summary>
        /// Property Name: "BlueTable"
        /// Property Type: (blob)
        /// </summary>
        D2D1_DISCRETETRANSFER_PROP_BLUE_TABLE = 4,

        /// <summary>
        /// Property Name: "BlueDisable"
        /// Property Type: BOOL
        /// </summary>
        D2D1_DISCRETETRANSFER_PROP_BLUE_DISABLE = 5,

        /// <summary>
        /// Property Name: "AlphaTable"
        /// Property Type: (blob)
        /// </summary>
        D2D1_DISCRETETRANSFER_PROP_ALPHA_TABLE = 6,

        /// <summary>
        /// Property Name: "AlphaDisable"
        /// Property Type: BOOL
        /// </summary>
        D2D1_DISCRETETRANSFER_PROP_ALPHA_DISABLE = 7,

        /// <summary>
        /// Property Name: "ClampOutput"
        /// Property Type: BOOL
        /// </summary>
        D2D1_DISCRETETRANSFER_PROP_CLAMP_OUTPUT = 8,
        D2D1_DISCRETETRANSFER_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Convolve Matrix effect's top level properties.
    /// Effect description: Applies a user specified convolution kernel to a bitmap.
    /// </summary>
    public enum D2D1_CONVOLVEMATRIX_PROP
    {
        /// <summary>
        /// Property Name: "KernelUnitLength"
        /// Property Type: D2D1_VECTOR_2F
        /// </summary>
        D2D1_CONVOLVEMATRIX_PROP_KERNEL_UNIT_LENGTH = 0,

        /// <summary>
        /// Property Name: "ScaleMode"
        /// Property Type: D2D1_CONVOLVEMATRIX_SCALE_MODE
        /// </summary>
        D2D1_CONVOLVEMATRIX_PROP_SCALE_MODE = 1,

        /// <summary>
        /// Property Name: "KernelSizeX"
        /// Property Type: UINT32
        /// </summary>
        D2D1_CONVOLVEMATRIX_PROP_KERNEL_SIZE_X = 2,

        /// <summary>
        /// Property Name: "KernelSizeY"
        /// Property Type: UINT32
        /// </summary>
        D2D1_CONVOLVEMATRIX_PROP_KERNEL_SIZE_Y = 3,

        /// <summary>
        /// Property Name: "KernelMatrix"
        /// Property Type: (blob)
        /// </summary>
        D2D1_CONVOLVEMATRIX_PROP_KERNEL_MATRIX = 4,

        /// <summary>
        /// Property Name: "Divisor"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_CONVOLVEMATRIX_PROP_DIVISOR = 5,

        /// <summary>
        /// Property Name: "Bias"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_CONVOLVEMATRIX_PROP_BIAS = 6,

        /// <summary>
        /// Property Name: "KernelOffset"
        /// Property Type: D2D1_VECTOR_2F
        /// </summary>
        D2D1_CONVOLVEMATRIX_PROP_KERNEL_OFFSET = 7,

        /// <summary>
        /// Property Name: "PreserveAlpha"
        /// Property Type: BOOL
        /// </summary>
        D2D1_CONVOLVEMATRIX_PROP_PRESERVE_ALPHA = 8,

        /// <summary>
        /// Property Name: "BorderMode"
        /// Property Type: D2D1_BORDER_MODE
        /// </summary>
        D2D1_CONVOLVEMATRIX_PROP_BORDER_MODE = 9,

        /// <summary>
        /// Property Name: "ClampOutput"
        /// Property Type: BOOL
        /// </summary>
        D2D1_CONVOLVEMATRIX_PROP_CLAMP_OUTPUT = 10,
        D2D1_CONVOLVEMATRIX_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_CONVOLVEMATRIX_SCALE_MODE
    {
        D2D1_CONVOLVEMATRIX_SCALE_MODE_NEAREST_NEIGHBOR = 0,
        D2D1_CONVOLVEMATRIX_SCALE_MODE_LINEAR = 1,
        D2D1_CONVOLVEMATRIX_SCALE_MODE_CUBIC = 2,
        D2D1_CONVOLVEMATRIX_SCALE_MODE_MULTI_SAMPLE_LINEAR = 3,
        D2D1_CONVOLVEMATRIX_SCALE_MODE_ANISOTROPIC = 4,
        D2D1_CONVOLVEMATRIX_SCALE_MODE_HIGH_QUALITY_CUBIC = 5,
        D2D1_CONVOLVEMATRIX_SCALE_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Brightness effect's top level properties.
    /// Effect description: Adjusts the brightness of the image based on the specified
    /// white and black point.
    /// </summary>
    public enum D2D1_BRIGHTNESS_PROP
    {
        /// <summary>
        /// Property Name: "WhitePoint"
        /// Property Type: D2D1_VECTOR_2F
        /// </summary>
        D2D1_BRIGHTNESS_PROP_WHITE_POINT = 0,

        /// <summary>
        /// Property Name: "BlackPoint"
        /// Property Type: D2D1_VECTOR_2F
        /// </summary>
        D2D1_BRIGHTNESS_PROP_BLACK_POINT = 1,
        D2D1_BRIGHTNESS_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Arithmetic Composite effect's top level properties.
    /// Effect description: Composites two bitmaps based on the following algorithm:
    /// Output = Coefficients_1 * Source * Destination + Coefficients_2 * Source+
    /// Coefficients_3 * Destination + Coefficients_4.
    /// </summary>
    public enum D2D1_ARITHMETICCOMPOSITE_PROP
    {
        /// <summary>
        /// Property Name: "Coefficients"
        /// Property Type: D2D1_VECTOR_4F
        /// </summary>
        D2D1_ARITHMETICCOMPOSITE_PROP_COEFFICIENTS = 0,

        /// <summary>
        /// Property Name: "ClampOutput"
        /// Property Type: BOOL
        /// </summary>
        D2D1_ARITHMETICCOMPOSITE_PROP_CLAMP_OUTPUT = 1,
        D2D1_ARITHMETICCOMPOSITE_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Crop effect's top level properties.
    /// Effect description: Crops the input bitmap according to the specified
    /// parameters.
    /// </summary>
    public enum D2D1_CROP_PROP
    {
        /// <summary>
        /// Property Name: "Rect"
        /// Property Type: D2D1_VECTOR_4F
        /// </summary>
        D2D1_CROP_PROP_RECT = 0,

        /// <summary>
        /// Property Name: "BorderMode"
        /// Property Type: D2D1_BORDER_MODE
        /// </summary>
        D2D1_CROP_PROP_BORDER_MODE = 1,
        D2D1_CROP_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Border effect's top level properties.
    /// Effect description: Extends the region of the bitmap based on the selected
    /// border mode.
    /// </summary>
    public enum D2D1_BORDER_PROP
    {
        /// <summary>
        /// Property Name: "EdgeModeX"
        /// Property Type: D2D1_BORDER_EDGE_MODE
        /// </summary>
        D2D1_BORDER_PROP_EDGE_MODE_X = 0,

        /// <summary>
        /// Property Name: "EdgeModeY"
        /// Property Type: D2D1_BORDER_EDGE_MODE
        /// </summary>
        D2D1_BORDER_PROP_EDGE_MODE_Y = 1,
        D2D1_BORDER_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The edge mode for the Border effect.
    /// </summary>
    public enum D2D1_BORDER_EDGE_MODE
    {
        D2D1_BORDER_EDGE_MODE_CLAMP = 0,
        D2D1_BORDER_EDGE_MODE_WRAP = 1,
        D2D1_BORDER_EDGE_MODE_MIRROR = 2,
        D2D1_BORDER_EDGE_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Morphology effect's top level properties.
    /// Effect description: Erodes or dilates a bitmap by the given radius.
    /// </summary>
    public enum D2D1_MORPHOLOGY_PROP
    {
        /// <summary>
        /// Property Name: "Mode"
        /// Property Type: D2D1_MORPHOLOGY_MODE
        /// </summary>
        D2D1_MORPHOLOGY_PROP_MODE = 0,

        /// <summary>
        /// Property Name: "Width"
        /// Property Type: UINT32
        /// </summary>
        D2D1_MORPHOLOGY_PROP_WIDTH = 1,

        /// <summary>
        /// Property Name: "Height"
        /// Property Type: UINT32
        /// </summary>
        D2D1_MORPHOLOGY_PROP_HEIGHT = 2,
        D2D1_MORPHOLOGY_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_MORPHOLOGY_MODE
    {
        D2D1_MORPHOLOGY_MODE_ERODE = 0,
        D2D1_MORPHOLOGY_MODE_DILATE = 1,
        D2D1_MORPHOLOGY_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }


    /// <summary>
    /// The enumeration of the Tile effect's top level properties.
    /// Effect description: Tiles the specified region of the input bitmap.
    /// </summary>
    public enum D2D1_TILE_PROP
    {
        /// <summary>
        /// Property Name: "Rect"
        /// Property Type: D2D1_VECTOR_4F
        /// </summary>
        D2D1_TILE_PROP_RECT = 0,
        D2D1_TILE_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Atlas effect's top level properties.
    /// Effect description: Changes the available area of the input to the specified
    /// rectangle. Provides an optimization for scenarios where a bitmap is used as an
    /// atlas.
    /// </summary>
    public enum D2D1_ATLAS_PROP
    {
        /// <summary>
        /// Property Name: "InputRect"
        /// Property Type: D2D1_VECTOR_4F
        /// </summary>
        D2D1_ATLAS_PROP_INPUT_RECT = 0,

        /// <summary>
        /// Property Name: "InputPaddingRect"
        /// Property Type: D2D1_VECTOR_4F
        /// </summary>
        D2D1_ATLAS_PROP_INPUT_PADDING_RECT = 1,
        D2D1_ATLAS_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Opacity Metadata effect's top level properties.
    /// Effect description: Changes the rectangle which is assumed to be opaque.
    /// Provides optimizations in certain scenarios.
    /// </summary>
    public enum D2D1_OPACITYMETADATA_PROP
    {
        /// <summary>
        /// Property Name: "InputOpaqueRect"
        /// Property Type: D2D1_VECTOR_4F
        /// </summary>
        D2D1_OPACITYMETADATA_PROP_INPUT_OPAQUE_RECT = 0,
        D2D1_OPACITYMETADATA_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }


    /// <summary>
    /// The enumeration of the Contrast effect's top level properties.
    /// Effect description: Adjusts the contrast of an image.
    /// </summary>
    public enum D2D1_CONTRAST_PROP
    {
        /// <summary>
        /// Property Name: "Contrast"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_CONTRAST_PROP_CONTRAST = 0,

        /// <summary>
        /// Property Name: "ClampInput"
        /// Property Type: BOOL
        /// </summary>
        D2D1_CONTRAST_PROP_CLAMP_INPUT = 1,
        D2D1_CONTRAST_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the RgbToHue effect's top level properties.
    /// Effect description: Converts an RGB bitmap to HSV or HSL.
    /// </summary>
    public enum D2D1_RGBTOHUE_PROP
    {
        /// <summary>
        /// Property Name: "OutputColorSpace"
        /// Property Type: D2D1_RGBTOHUE_OUTPUT_COLOR_SPACE
        /// </summary>
        D2D1_RGBTOHUE_PROP_OUTPUT_COLOR_SPACE = 0,
        D2D1_RGBTOHUE_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_RGBTOHUE_OUTPUT_COLOR_SPACE
    {
        D2D1_RGBTOHUE_OUTPUT_COLOR_SPACE_HUE_SATURATION_VALUE = 0,
        D2D1_RGBTOHUE_OUTPUT_COLOR_SPACE_HUE_SATURATION_LIGHTNESS = 1,
        D2D1_RGBTOHUE_OUTPUT_COLOR_SPACE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the HueToRgb effect's top level properties.
    /// Effect description: Converts an HSV or HSL bitmap into an RGB bitmap.
    /// </summary>
    public enum D2D1_HUETORGB_PROP
    {
        /// <summary>
        /// Property Name: "InputColorSpace"
        /// Property Type: D2D1_HUETORGB_INPUT_COLOR_SPACE
        /// </summary>
        D2D1_HUETORGB_PROP_INPUT_COLOR_SPACE = 0,
        D2D1_HUETORGB_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_HUETORGB_INPUT_COLOR_SPACE
    {
        D2D1_HUETORGB_INPUT_COLOR_SPACE_HUE_SATURATION_VALUE = 0,
        D2D1_HUETORGB_INPUT_COLOR_SPACE_HUE_SATURATION_LIGHTNESS = 1,
        D2D1_HUETORGB_INPUT_COLOR_SPACE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Chroma Key effect's top level properties.
    /// Effect description: Converts a specified color to transparent.
    /// </summary>
    public enum D2D1_CHROMAKEY_PROP
    {
        /// <summary>
        /// Property Name: "Color"
        /// Property Type: D2D1_VECTOR_3F
        /// </summary>
        D2D1_CHROMAKEY_PROP_COLOR = 0,

        /// <summary>
        /// Property Name: "Tolerance"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_CHROMAKEY_PROP_TOLERANCE = 1,

        /// <summary>
        /// Property Name: "InvertAlpha"
        /// Property Type: BOOL
        /// </summary>
        D2D1_CHROMAKEY_PROP_INVERT_ALPHA = 2,

        /// <summary>
        /// Property Name: "Feather"
        /// Property Type: BOOL
        /// </summary>
        D2D1_CHROMAKEY_PROP_FEATHER = 3,
        D2D1_CHROMAKEY_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Emboss effect's top level properties.
    /// Effect description: Applies an embossing effect to an image.
    /// </summary>
    public enum D2D1_EMBOSS_PROP
    {
        /// <summary>
        /// Property Name: "Height"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_EMBOSS_PROP_HEIGHT = 0,

        /// <summary>
        /// Property Name: "Direction"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_EMBOSS_PROP_DIRECTION = 1,
        D2D1_EMBOSS_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Exposure effect's top level properties.
    /// Effect description: Simulates camera exposure adjustment.
    /// </summary>
    public enum D2D1_EXPOSURE_PROP
    {
        /// <summary>
        /// Property Name: "ExposureValue"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_EXPOSURE_PROP_EXPOSURE_VALUE = 0,
        D2D1_EXPOSURE_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Posterize effect's top level properties.
    /// Effect description: Reduces the number of colors in an image.
    /// </summary>
    public enum D2D1_POSTERIZE_PROP
    {
        /// <summary>
        /// Property Name: "RedValueCount"
        /// Property Type: UINT32
        /// </summary>
        D2D1_POSTERIZE_PROP_RED_VALUE_COUNT = 0,

        /// <summary>
        /// Property Name: "GreenValueCount"
        /// Property Type: UINT32
        /// </summary>
        D2D1_POSTERIZE_PROP_GREEN_VALUE_COUNT = 1,

        /// <summary>
        /// Property Name: "BlueValueCount"
        /// Property Type: UINT32
        /// </summary>
        D2D1_POSTERIZE_PROP_BLUE_VALUE_COUNT = 2,
        D2D1_POSTERIZE_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Sepia effect's top level properties.
    /// Effect description: Applies a Sepia tone to an image.
    /// </summary>
    public enum D2D1_SEPIA_PROP
    {
        /// <summary>
        /// Property Name: "Intensity"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_SEPIA_PROP_INTENSITY = 0,

        /// <summary>
        /// Property Name: "AlphaMode"
        /// Property Type: D2D1_ALPHA_MODE
        /// </summary>
        D2D1_SEPIA_PROP_ALPHA_MODE = 1,
        D2D1_SEPIA_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Sharpen effect's top level properties.
    /// Effect description: Performs sharpening adjustment
    /// </summary>
    public enum D2D1_SHARPEN_PROP
    {
        /// <summary>
        /// Property Name: "Sharpness"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_SHARPEN_PROP_SHARPNESS = 0,

        /// <summary>
        /// Property Name: "Threshold"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_SHARPEN_PROP_THRESHOLD = 1,
        D2D1_SHARPEN_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Straighten effect's top level properties.
    /// Effect description: Straightens an image.
    /// </summary>
    public enum D2D1_STRAIGHTEN_PROP
    {
        /// <summary>
        /// Property Name: "Angle"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_STRAIGHTEN_PROP_ANGLE = 0,

        /// <summary>
        /// Property Name: "MaintainSize"
        /// Property Type: BOOL
        /// </summary>
        D2D1_STRAIGHTEN_PROP_MAINTAIN_SIZE = 1,

        /// <summary>
        /// Property Name: "ScaleMode"
        /// Property Type: D2D1_STRAIGHTEN_SCALE_MODE
        /// </summary>
        D2D1_STRAIGHTEN_PROP_SCALE_MODE = 2,
        D2D1_STRAIGHTEN_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_STRAIGHTEN_SCALE_MODE
    {
        D2D1_STRAIGHTEN_SCALE_MODE_NEAREST_NEIGHBOR = 0,
        D2D1_STRAIGHTEN_SCALE_MODE_LINEAR = 1,
        D2D1_STRAIGHTEN_SCALE_MODE_CUBIC = 2,
        D2D1_STRAIGHTEN_SCALE_MODE_MULTI_SAMPLE_LINEAR = 3,
        D2D1_STRAIGHTEN_SCALE_MODE_ANISOTROPIC = 4,
        D2D1_STRAIGHTEN_SCALE_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Temperature And Tint effect's top level properties.
    /// Effect description: Adjusts the temperature and tint of an image.
    /// </summary>
    public enum D2D1_TEMPERATUREANDTINT_PROP
    {
        /// <summary>
        /// Property Name: "Temperature"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_TEMPERATUREANDTINT_PROP_TEMPERATURE = 0,

        /// <summary>
        /// Property Name: "Tint"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_TEMPERATUREANDTINT_PROP_TINT = 1,
        D2D1_TEMPERATUREANDTINT_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Vignette effect's top level properties.
    /// Effect description: Fades the edges of an image to the specified color.
    /// </summary>
    public enum D2D1_VIGNETTE_PROP
    {
        /// <summary>
        /// Property Name: "Color"
        /// Property Type: D2D1_VECTOR_4F
        /// </summary>
        D2D1_VIGNETTE_PROP_COLOR = 0,

        /// <summary>
        /// Property Name: "TransitionSize"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_VIGNETTE_PROP_TRANSITION_SIZE = 1,

        /// <summary>
        /// Property Name: "Strength"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_VIGNETTE_PROP_STRENGTH = 2,
        D2D1_VIGNETTE_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Edge Detection effect's top level properties.
    /// Effect description: Detects edges of an image.
    /// </summary>
    public enum D2D1_EDGEDETECTION_PROP
    {

        /// <summary>
        /// Property Name: "Strength"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_EDGEDETECTION_PROP_STRENGTH = 0,

        /// <summary>
        /// Property Name: "BlurRadius"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_EDGEDETECTION_PROP_BLUR_RADIUS = 1,

        /// <summary>
        /// Property Name: "Mode"
        /// Property Type: D2D1_EDGEDETECTION_MODE
        /// </summary>
        D2D1_EDGEDETECTION_PROP_MODE = 2,

        /// <summary>
        /// Property Name: "OverlayEdges"
        /// Property Type: BOOL
        /// </summary>
        D2D1_EDGEDETECTION_PROP_OVERLAY_EDGES = 3,

        /// <summary>
        /// Property Name: "AlphaMode"
        /// Property Type: D2D1_ALPHA_MODE
        /// </summary>
        D2D1_EDGEDETECTION_PROP_ALPHA_MODE = 4,
        D2D1_EDGEDETECTION_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_EDGEDETECTION_MODE
    {
        D2D1_EDGEDETECTION_MODE_SOBEL = 0,
        D2D1_EDGEDETECTION_MODE_PREWITT = 1,
        D2D1_EDGEDETECTION_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Highlights and Shadows effect's top level properties.
    /// Effect description: Adjusts the highlight and shadow strength of an image.
    /// </summary>
    public enum D2D1_HIGHLIGHTSANDSHADOWS_PROP
    {
        /// <summary>
        /// Property Name: "Highlights"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_HIGHLIGHTSANDSHADOWS_PROP_HIGHLIGHTS = 0,

        /// <summary>
        /// Property Name: "Shadows"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_HIGHLIGHTSANDSHADOWS_PROP_SHADOWS = 1,

        /// <summary>
        /// Property Name: "Clarity"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_HIGHLIGHTSANDSHADOWS_PROP_CLARITY = 2,

        /// <summary>
        /// Property Name: "InputGamma"
        /// Property Type: D2D1_HIGHLIGHTSANDSHADOWS_INPUT_GAMMA
        /// </summary>
        D2D1_HIGHLIGHTSANDSHADOWS_PROP_INPUT_GAMMA = 3,

        /// <summary>
        /// Property Name: "MaskBlurRadius"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_HIGHLIGHTSANDSHADOWS_PROP_MASK_BLUR_RADIUS = 4,
        D2D1_HIGHLIGHTSANDSHADOWS_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_HIGHLIGHTSANDSHADOWS_INPUT_GAMMA
    {
        D2D1_HIGHLIGHTSANDSHADOWS_INPUT_GAMMA_LINEAR = 0,
        D2D1_HIGHLIGHTSANDSHADOWS_INPUT_GAMMA_SRGB = 1,
        D2D1_HIGHLIGHTSANDSHADOWS_INPUT_GAMMA_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Lookup Table 3D effect's top level properties.
    /// Effect description: Remaps colors in an image via a 3D lookup table.
    /// </summary>
    public enum D2D1_LOOKUPTABLE3D_PROP
    {
        /// <summary>
        /// Property Name: "Lut"
        /// Property Type: IUnknown *
        /// </summary>
        D2D1_LOOKUPTABLE3D_PROP_LUT = 0,

        /// <summary>
        /// Property Name: "AlphaMode"
        /// Property Type: D2D1_ALPHA_MODE
        /// </summary>
        D2D1_LOOKUPTABLE3D_PROP_ALPHA_MODE = 1,
        D2D1_LOOKUPTABLE3D_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Opacity effect's top level properties.
    /// Effect description: Adjusts the opacity of an image by multiplying the alpha
    /// channel by the specified opacity.
    /// </summary>
    public enum D2D1_OPACITY_PROP
    {
        /// <summary>
        /// Property Name: "Opacity"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_OPACITY_PROP_OPACITY = 0,
        D2D1_OPACITY_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Cross Fade effect's top level properties.
    /// Effect description: This effect combines two images by adding weighted pixels
    /// from input images. The formula can be expressed as output = weight * Destination
    /// + (1 - weight) * Source
    /// </summary>
    public enum D2D1_CROSSFADE_PROP
    {
        /// <summary>
        /// Property Name: "Weight"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_CROSSFADE_PROP_WEIGHT = 0,
        D2D1_CROSSFADE_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Tint effect's top level properties.
    /// Effect description: This effect tints the source image by multiplying the
    /// specified color by the source image.
    /// </summary>
    public enum D2D1_TINT_PROP
    {
        /// <summary>
        /// Property Name: "Color"
        /// Property Type: D2D1_VECTOR_4F
        /// </summary>
        D2D1_TINT_PROP_COLOR = 0,

        /// <summary>
        /// Property Name: "ClampOutput"
        /// Property Type: BOOL
        /// </summary>
        D2D1_TINT_PROP_CLAMP_OUTPUT = 1,
        D2D1_TINT_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the White Level Adjustment effect's top level properties.
    /// Effect description: This effect adjusts the white level of the source image by
    /// multiplying the source image color by the ratio of the input and output white
    /// levels. Input and output white levels are specified in nits.
    /// </summary>
    public enum D2D1_WHITELEVELADJUSTMENT_PROP
    {
        /// <summary>
        /// Property Name: "InputWhiteLevel"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_WHITELEVELADJUSTMENT_PROP_INPUT_WHITE_LEVEL = 0,

        /// <summary>
        /// Property Name: "OutputWhiteLevel"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_WHITELEVELADJUSTMENT_PROP_OUTPUT_WHITE_LEVEL = 1,
        D2D1_WHITELEVELADJUSTMENT_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the HDR Tone Map effect's top level properties.
    /// Effect description: Adjusts the maximum luminance of the source image to fit
    /// within the maximum output luminance supported. Input and output luminance values
    /// are specified in nits. Note that the color space of the image is assumed to be
    /// scRGB.
    /// </summary>
    public enum D2D1_HDRTONEMAP_PROP
    {
        /// <summary>
        /// Property Name: "InputMaxLuminance"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_HDRTONEMAP_PROP_INPUT_MAX_LUMINANCE = 0,

        /// <summary>
        /// Property Name: "OutputMaxLuminance"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_HDRTONEMAP_PROP_OUTPUT_MAX_LUMINANCE = 1,

        /// <summary>
        /// Property Name: "DisplayMode"
        /// Property Type: D2D1_HDRTONEMAP_DISPLAY_MODE
        /// </summary>
        D2D1_HDRTONEMAP_PROP_DISPLAY_MODE = 2,
        D2D1_HDRTONEMAP_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_HDRTONEMAP_DISPLAY_MODE
    {
        D2D1_HDRTONEMAP_DISPLAY_MODE_SDR = 0,
        D2D1_HDRTONEMAP_DISPLAY_MODE_HDR = 1,
        D2D1_HDRTONEMAP_DISPLAY_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }




    public enum D2D1_UNIT_MODE
    {
        D2D1_UNIT_MODE_DIPS = 0,
        D2D1_UNIT_MODE_PIXELS = 1,
        D2D1_UNIT_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_RENDERING_CONTROLS
    {
        /// <summary>
        /// The default buffer precision, used if the precision isn't otherwise specified.
        /// </summary>
        public D2D1_BUFFER_PRECISION bufferPrecision;

        /// <summary>
        /// The size of allocated tiles used to render imaging effects.
        /// </summary>
        public D2D1_SIZE_U tileSize;
    };

    public enum D2D1_COMPOSITE_MODE
    {
        D2D1_COMPOSITE_MODE_SOURCE_OVER = 0,
        D2D1_COMPOSITE_MODE_DESTINATION_OVER = 1,
        D2D1_COMPOSITE_MODE_SOURCE_IN = 2,
        D2D1_COMPOSITE_MODE_DESTINATION_IN = 3,
        D2D1_COMPOSITE_MODE_SOURCE_OUT = 4,
        D2D1_COMPOSITE_MODE_DESTINATION_OUT = 5,
        D2D1_COMPOSITE_MODE_SOURCE_ATOP = 6,
        D2D1_COMPOSITE_MODE_DESTINATION_ATOP = 7,
        D2D1_COMPOSITE_MODE_XOR = 8,
        D2D1_COMPOSITE_MODE_PLUS = 9,
        D2D1_COMPOSITE_MODE_SOURCE_COPY = 10,
        D2D1_COMPOSITE_MODE_BOUNDED_SOURCE_COPY = 11,
        D2D1_COMPOSITE_MODE_MASK_INVERT = 12,
        D2D1_COMPOSITE_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_PRIMITIVE_BLEND
    {
        D2D1_PRIMITIVE_BLEND_SOURCE_OVER = 0,
        D2D1_PRIMITIVE_BLEND_COPY = 1,
        D2D1_PRIMITIVE_BLEND_MIN = 2,
        D2D1_PRIMITIVE_BLEND_ADD = 3,
        D2D1_PRIMITIVE_BLEND_MAX = 4,
        D2D1_PRIMITIVE_BLEND_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_IMAGE_BRUSH_PROPERTIES
    {
        public D2D1_RECT_F sourceRectangle;
        public D2D1_EXTEND_MODE extendModeX;
        public D2D1_EXTEND_MODE extendModeY;
        public D2D1_INTERPOLATION_MODE interpolationMode;
    };

    public enum D2D1_INTERPOLATION_MODE
    {
        D2D1_INTERPOLATION_MODE_NEAREST_NEIGHBOR = D2D1_INTERPOLATION_MODE_DEFINITION.D2D1_INTERPOLATION_MODE_DEFINITION_NEAREST_NEIGHBOR,
        D2D1_INTERPOLATION_MODE_LINEAR = D2D1_INTERPOLATION_MODE_DEFINITION.D2D1_INTERPOLATION_MODE_DEFINITION_LINEAR,
        D2D1_INTERPOLATION_MODE_CUBIC = D2D1_INTERPOLATION_MODE_DEFINITION.D2D1_INTERPOLATION_MODE_DEFINITION_CUBIC,
        D2D1_INTERPOLATION_MODE_MULTI_SAMPLE_LINEAR = D2D1_INTERPOLATION_MODE_DEFINITION.D2D1_INTERPOLATION_MODE_DEFINITION_MULTI_SAMPLE_LINEAR,
        D2D1_INTERPOLATION_MODE_ANISOTROPIC = D2D1_INTERPOLATION_MODE_DEFINITION.D2D1_INTERPOLATION_MODE_DEFINITION_ANISOTROPIC,
        D2D1_INTERPOLATION_MODE_HIGH_QUALITY_CUBIC = D2D1_INTERPOLATION_MODE_DEFINITION.D2D1_INTERPOLATION_MODE_DEFINITION_HIGH_QUALITY_CUBIC,
        D2D1_INTERPOLATION_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_BITMAP_BRUSH_PROPERTIES1
    {
        public D2D1_EXTEND_MODE extendModeX;
        public D2D1_EXTEND_MODE extendModeY;
        public D2D1_INTERPOLATION_MODE interpolationMode;
    };

    public enum D2D1_BUFFER_PRECISION
    {
        D2D1_BUFFER_PRECISION_UNKNOWN = 0,
        D2D1_BUFFER_PRECISION_8BPC_UNORM = 1,
        D2D1_BUFFER_PRECISION_8BPC_UNORM_SRGB = 2,
        D2D1_BUFFER_PRECISION_16BPC_UNORM = 3,
        D2D1_BUFFER_PRECISION_16BPC_FLOAT = 4,
        D2D1_BUFFER_PRECISION_32BPC_FLOAT = 5,
        D2D1_BUFFER_PRECISION_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_COLOR_INTERPOLATION_MODE
    {
        /// <summary>
        /// Colors will be interpolated in straight alpha space.
        /// </summary>
        D2D1_COLOR_INTERPOLATION_MODE_STRAIGHT = 0,

        /// <summary>
        /// Colors will be interpolated in premultiplied alpha space.
        /// </summary>
        D2D1_COLOR_INTERPOLATION_MODE_PREMULTIPLIED = 1,
        D2D1_COLOR_INTERPOLATION_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_BITMAP_PROPERTIES1
    {
        public D2D1_PIXEL_FORMAT pixelFormat;
        public float dpiX;
        public float dpiY;
        public D2D1_BITMAP_OPTIONS bitmapOptions;

        public ID2D1ColorContext colorContext;
    };

    [ComImport]
    [Guid("a898a84c-3873-4588-b08b-ebbf978df041")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Bitmap1 : ID2D1Bitmap
    {
        #region <ID2D1Bitmap>

        #region <ID2D1Image>

        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);

        #endregion

        #endregion

        new D2D1_SIZE_F GetSize();
        new D2D1_SIZE_U GetPixelSize();
        new D2D1_PIXEL_FORMAT GetPixelFormat();
        new void GetDpi(out float dpiX, out float dpiY);
        new void CopyFromBitmap(D2D1_POINT_2U destPoint, ID2D1Bitmap bitmap, D2D1_RECT_U srcRect);
        new void CopyFromRenderTarget(D2D1_POINT_2U destPoint, ID2D1RenderTarget renderTarget, D2D1_RECT_U srcRect);
        new void CopyFromMemory(D2D1_RECT_U dstRect, IntPtr srcData, uint pitch);
        #endregion

        void GetColorContext(out ID2D1ColorContext colorContext);
        D2D1_BITMAP_OPTIONS GetOptions();
        HRESULT GetSurface(out IDXGISurface dxgiSurface);
        HRESULT Map(D2D1_MAP_OPTIONS options, out D2D1_MAPPED_RECT mappedRect);
        HRESULT Unmap();
    }

    public enum D2D1_BITMAP_OPTIONS
    {
        /// <summary>
        /// The bitmap is created with default properties.
        /// </summary>
        D2D1_BITMAP_OPTIONS_NONE = 0x00000000,

        /// <summary>
        /// The bitmap can be specified as a target in ID2D1DeviceContext::SetTarget
        /// </summary>
        D2D1_BITMAP_OPTIONS_TARGET = 0x00000001,

        /// <summary>
        /// The bitmap cannot be used as an input to DrawBitmap, DrawImage, in a bitmap
        /// brush or as an input to an effect.
        /// </summary>
        D2D1_BITMAP_OPTIONS_CANNOT_DRAW = 0x00000002,

        /// <summary>
        /// The bitmap can be read from the CPU.
        /// </summary>
        D2D1_BITMAP_OPTIONS_CPU_READ = 0x00000004,

        /// <summary>
        /// The bitmap works with the ID2D1GdiInteropRenderTarget::GetDC API.
        /// </summary>
        D2D1_BITMAP_OPTIONS_GDI_COMPATIBLE = 0x00000008,
        D2D1_BITMAP_OPTIONS_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    public enum D2D1_MAP_OPTIONS
    {
        /// <summary>
        /// The mapped pointer has undefined behavior.
        /// </summary>
        D2D1_MAP_OPTIONS_NONE = 0,

        /// <summary>
        /// The mapped pointer can be read from.
        /// </summary>
        D2D1_MAP_OPTIONS_READ = 1,

        /// <summary>
        /// The mapped pointer can be written to.
        /// </summary>
        D2D1_MAP_OPTIONS_WRITE = 2,

        /// <summary>
        /// The previous contents of the bitmap are discarded when it is mapped.
        /// </summary>
        D2D1_MAP_OPTIONS_DISCARD = 4,
        D2D1_MAP_OPTIONS_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_MAPPED_RECT
    {
        public uint pitch;
        public IntPtr bits;
    };

    [ComImport]
    [Guid("1c4820bb-5771-4518-a581-2fe4dd0ec657")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1ColorContext : ID2D1Resource
    {
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        D2D1_COLOR_SPACE GetColorSpace();
        uint GetProfileSize();
        HRESULT GetProfile(out IntPtr profile, uint profileSize);
    }

    public enum D2D1_COLOR_SPACE
    {
        /// <summary>
        /// The color space is described by accompanying data, such as a color profile.
        /// </summary>
        D2D1_COLOR_SPACE_CUSTOM = 0,

        /// <summary>
        /// The sRGB color space.
        /// </summary>
        D2D1_COLOR_SPACE_SRGB = 1,

        /// <summary>
        /// The scRGB color space.
        /// </summary>
        D2D1_COLOR_SPACE_SCRGB = 2,
        D2D1_COLOR_SPACE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

  


    public class D2DTools
    {
        public static Guid CLSID_D2D1Factory = new Guid("06152247-6f50-465a-9245-118bfd3b6007");

        public static Guid CLSID_D2D12DAffineTransform = new Guid("{6AA97485-6354-4CFC-908C-E4A74F62C96C}");
        public static Guid CLSID_D2D13DPerspectiveTransform = new Guid("{C2844D0B-3D86-46E7-85BA-526C9240F3FB}");
        public static Guid CLSID_D2D13DTransform = new Guid("{E8467B04-EC61-4B8A-B5DE-D4D73DEBEA5A}");
        public static Guid CLSID_D2D1ArithmeticComposite = new Guid("{FC151437-049A-4784-A24A-F1C4DAF20987}");
        public static Guid CLSID_D2D1Atlas = new Guid("{913E2BE4-FDCF-4FE2-A5F0-2454F14FF408}");
        public static Guid CLSID_D2D1BitmapSource = new Guid("{5FB6C24D-C6DD-4231-9404-50F4D5C3252D}");
        public static Guid CLSID_D2D1Blend = new Guid("{81C5B77B-13F8-4CDD-AD20-C890547AC65D}");
        public static Guid CLSID_D2D1Border = new Guid("{2A2D49C0-4ACF-43C7-8C6A-7C4A27874D27}");
        public static Guid CLSID_D2D1Brightness = new Guid("{8CEA8D1E-77B0-4986-B3B9-2F0C0EAE7887}");
        public static Guid CLSID_D2D1ColorManagement = new Guid("{1A28524C-FDD6-4AA4-AE8F-837EB8267B37}");
        public static Guid CLSID_D2D1ColorMatrix = new Guid("{921F03D6-641C-47DF-852D-B4BB6153AE11}");
        public static Guid CLSID_D2D1Composite = new Guid("{48FC9F51-F6AC-48F1-8B58-3B28AC46F76D}");
        public static Guid CLSID_D2D1ConvolveMatrix = new Guid("{407F8C08-5533-4331-A341-23CC3877843E}");
        public static Guid CLSID_D2D1Crop = new Guid("{E23F7110-0E9A-4324-AF47-6A2C0C46F35B}");
        public static Guid CLSID_D2D1DirectionalBlur = new Guid("{174319A6-58E9-49B2-BB63-CAF2C811A3DB}");
        public static Guid CLSID_D2D1DiscreteTransfer = new Guid("{90866FCD-488E-454B-AF06-E5041B66C36C}");
        public static Guid CLSID_D2D1DisplacementMap = new Guid("{EDC48364-0417-4111-9450-43845FA9F890}");
        public static Guid CLSID_D2D1DistantDiffuse = new Guid("{3E7EFD62-A32D-46D4-A83C-5278889AC954}");
        public static Guid CLSID_D2D1DistantSpecular = new Guid("{428C1EE5-77B8-4450-8AB5-72219C21ABDA}");
        public static Guid CLSID_D2D1DpiCompensation = new Guid("{6C26C5C7-34E0-46FC-9CFD-E5823706E228}");
        public static Guid CLSID_D2D1Flood = new Guid("{61C23C20-AE69-4D8E-94CF-50078DF638F2}");
        public static Guid CLSID_D2D1GammaTransfer = new Guid("{409444C4-C419-41A0-B0C1-8CD0C0A18E42}");
        public static Guid CLSID_D2D1GaussianBlur = new Guid("{1FEB6D69-2FE6-4AC9-8C58-1D7F93E7A6A5}");
        public static Guid CLSID_D2D1Scale = new Guid("{9DAF9369-3846-4D0E-A44E-0C607934A5D7}");
        public static Guid CLSID_D2D1Histogram = new Guid("{881DB7D0-F7EE-4D4D-A6D2-4697ACC66EE8}");
        public static Guid CLSID_D2D1HueRotation = new Guid("{0F4458EC-4B32-491B-9E85-BD73F44D3EB6}");
        public static Guid CLSID_D2D1LinearTransfer = new Guid("{AD47C8FD-63EF-4ACC-9B51-67979C036C06}");
        public static Guid CLSID_D2D1LuminanceToAlpha = new Guid("{41251AB7-0BEB-46F8-9DA7-59E93FCCE5DE}");
        public static Guid CLSID_D2D1Morphology = new Guid("{EAE6C40D-626A-4C2D-BFCB-391001ABE202}");
        public static Guid CLSID_D2D1OpacityMetadata = new Guid("{6C53006A-4450-4199-AA5B-AD1656FECE5E}");
        public static Guid CLSID_D2D1PointDiffuse = new Guid("{B9E303C3-C08C-4F91-8B7B-38656BC48C20}");
        public static Guid CLSID_D2D1PointSpecular = new Guid("{09C3CA26-3AE2-4F09-9EBC-ED3865D53F22}");
        public static Guid CLSID_D2D1Premultiply = new Guid("{06EAB419-DEED-4018-80D2-3E1D471ADEB2}");
        public static Guid CLSID_D2D1Saturation = new Guid("{5CB2D9CF-327D-459F-A0CE-40C0B2086BF7}");
        public static Guid CLSID_D2D1Shadow = new Guid("{C67EA361-1863-4E69-89DB-695D3E9A5B6B}");
        public static Guid CLSID_D2D1SpotDiffuse = new Guid("{818A1105-7932-44F4-AA86-08AE7B2F2C93}");
        public static Guid CLSID_D2D1SpotSpecular = new Guid("{EDAE421E-7654-4A37-9DB8-71ACC1BEB3C1}");
        public static Guid CLSID_D2D1TableTransfer = new Guid("{5BF818C3-5E43-48CB-B631-868396D6A1D4}");
        public static Guid CLSID_D2D1Tile = new Guid("{B0784138-3B76-4BC5-B13B-0FA2AD02659F}");
        public static Guid CLSID_D2D1Turbulence = new Guid("{CF2BB6AE-889A-4AD7-BA29-A2FD732C9FC9}");
        public static Guid CLSID_D2D1UnPremultiply = new Guid("{FB9AC489-AD8D-41ED-9999-BB6347D110F7}");

        public static Guid CLSID_D2D1Contrast = new Guid("{B648A78A-0ED5-4F80-A94A-8E825ACA6B77}");
        public static Guid CLSID_D2D1RgbToHue = new Guid("{23F3E5EC-91E8-4D3D-AD0A-AFADC1004AA1}");
        public static Guid CLSID_D2D1HueToRgb = new Guid("{7B78A6BD-0141-4DEF-8A52-6356EE0CBDD5}");
        public static Guid CLSID_D2D1ChromaKey = new Guid("{74C01F5B-2A0D-408C-88E2-C7A3C7197742}");
        public static Guid CLSID_D2D1Emboss = new Guid("{B1C5EB2B-0348-43F0-8107-4957CACBA2AE}");
        public static Guid CLSID_D2D1Exposure = new Guid("{B56C8CFA-F634-41EE-BEE0-FFA617106004}");
        public static Guid CLSID_D2D1Grayscale = new Guid("{36DDE0EB-3725-42E0-836D-52FB20AEE644}");
        public static Guid CLSID_D2D1Invert = new Guid("{E0C3784D-CB39-4E84-B6FD-6B72F0810263}");
        public static Guid CLSID_D2D1Posterize = new Guid("{2188945E-33A3-4366-B7BC-086BD02D0884}");
        public static Guid CLSID_D2D1Sepia = new Guid("{3A1AF410-5F1D-4DBE-84DF-915DA79B7153}");
        public static Guid CLSID_D2D1Sharpen = new Guid("{C9B887CB-C5FF-4DC5-9779-273DCF417C7D}");
        public static Guid CLSID_D2D1Straighten = new Guid("{4DA47B12-79A3-4FB0-8237-BBC3B2A4DE08}");
        public static Guid CLSID_D2D1TemperatureTint = new Guid("{89176087-8AF9-4A08-AEB1-895F38DB1766}");
        public static Guid CLSID_D2D1Vignette = new Guid("{C00C40BE-5E67-4CA3-95B4-F4B02C115135}");
        public static Guid CLSID_D2D1EdgeDetection = new Guid("{EFF583CA-CB07-4AA9-AC5D-2CC44C76460F}");
        public static Guid CLSID_D2D1HighlightsShadows = new Guid("{CADC8384-323F-4C7E-A361-2E2B24DF6EE4}");
        public static Guid CLSID_D2D1LookupTable3D = new Guid("{349E0EDA-0088-4A79-9CA3-C7E300202020}");
        public static Guid CLSID_D2D1Opacity = new Guid("{811D79A4-DE28-4454-8094-C64685F8BD4C}");
        public static Guid CLSID_D2D1AlphaMask = new Guid("{C80ECFF0-3FD5-4F05-8328-C5D1724B4F0A}");
        public static Guid CLSID_D2D1CrossFade = new Guid("{12F575E8-4DB1-485F-9A84-03A07DD3829F}");
        public static Guid CLSID_D2D1Tint = new Guid("{36312B17-F7DD-4014-915D-FFCA768CF211}");
        public static Guid CLSID_D2D1WhiteLevelAdjustment = new Guid("{44A1CADB-6CDD-4818-8FF4-26C1CFE95BDB}");
        public static Guid CLSID_D2D1HdrToneMap = new Guid("{7B0B748D-4610-4486-A90C-999D9A2E2B11}");

        [DllImport("D2D1.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern HRESULT D2D1CreateFactory(D2D1_FACTORY_TYPE factoryType, ref Guid riid, ref D2D1_FACTORY_OPTIONS pFactoryOptions, out ID2D1Factory ppIFactory);

        [DllImport("DWrite.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern HRESULT DWriteCreateFactory(DWRITE_FACTORY_TYPE factoryType, ref Guid iid, out IDWriteFactory factory);

        // [Build] "Allow unsafe code"
        [DllImport("D2D1.dll", EntryPoint = "D2D1MakeRotateMatrix", CallingConvention = CallingConvention.StdCall)]
        public unsafe static extern void D2D1MakeRotateMatrix_(float angle, D2D1_POINT_2F center, void* matrix);

        [DllImport("D2D1.dll", EntryPoint = "D2D1MakeSkewMatrix", CallingConvention = CallingConvention.StdCall)]
        public unsafe static extern void D2D1MakeSkewMatrix_(float angleX, float angleY, D2D1_POINT_2F center, void* matrix);

        [DllImport("D2D1.dll", EntryPoint = "D2D1InvertMatrix", CallingConvention = CallingConvention.StdCall)]
        public unsafe static extern bool D2D1InvertMatrix_(void* matrix);

        [DllImport("D2D1.dll", EntryPoint = "D2D1IsMatrixInvertible", CallingConvention = CallingConvention.StdCall)]
        public unsafe static extern bool D2D1IsMatrixInvertible_(void* matrix);
        //

        //[DllImport("D2D1.dll", SetLastError = true, CharSet = CharSet.Auto)]
        //[return: MarshalAs(UnmanagedType.IUnknown)]
        //public static extern object D2D1ConvertColorSpace(D2D1_COLOR_SPACE sourceColorSpace, D2D1_COLOR_SPACE destinationColorSpace, D2D1_COLOR_F color);

        //[DllImport("D2D1.dll", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //public static extern IntPtr D2D1ConvertColorSpace(D2D1_COLOR_SPACE sourceColorSpace, D2D1_COLOR_SPACE destinationColorSpace, [In, Out] IntPtr color);

        [DllImport("D2D1.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern HRESULT D2D1CreateDevice(IDXGIDevice dxgiDevice, ref D2D1_CREATION_PROPERTIES creationProperties, out ID2D1Device d2dDevice);

        [DllImport("D2D1.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern HRESULT D2D1CreateDeviceContext(IDXGISurface dxgiSurface,ref D2D1_CREATION_PROPERTIES creationProperties, out ID2D1DeviceContext d2dDeviceContext);

        [DllImport("D2D1.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern void D2D1SinCos(float angle, out float s, out float c);

        [DllImport("D2D1.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern float D2D1Tan(float angle);

        [DllImport("D2D1.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern float D2D1Vec3Length(float x, float y, float z);

        public delegate HRESULT PD2D1_EFFECT_FACTORY(out IntPtr effectImpl);

        public const long D2DERR_RECREATE_TARGET = (0x8899000CL);

        public enum DWRITE_FACTORY_TYPE
        {
            /// <summary>
            /// Shared factory allow for re-use of cached font data across multiple in process components.
            /// Such factories also take advantage of cross process font caching components for better performance.
            /// </summary>
            DWRITE_FACTORY_TYPE_SHARED,
            /// <summary>
            /// Objects created from the isolated factory do not interact with internal DirectWrite state from other components.
            /// </summary>
            DWRITE_FACTORY_TYPE_ISOLATED
        };

        public static D2D1_PIXEL_FORMAT PixelFormat(DXGI_FORMAT dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN, D2D1_ALPHA_MODE alphaMode = D2D1_ALPHA_MODE.D2D1_ALPHA_MODE_UNKNOWN)
        {
            D2D1_PIXEL_FORMAT pixelFormat;
            pixelFormat.format = dxgiFormat;
            pixelFormat.alphaMode = alphaMode;
            return pixelFormat;
        }

        //public D2D1_PIXEL_FORMAT PixelFormat()
        //{
        //    return this.PixelFormat(DXGI_FORMAT.DXGI_FORMAT_UNKNOWN, D2D1_ALPHA_MODE.D2D1_ALPHA_MODE_UNKNOWN);
        //}

        //public D2D1_PIXEL_FORMAT PixelFormat(DXGI_FORMAT dxgiFormat, D2D1_ALPHA_MODE alphaMode)
        //{
        //    D2D1_PIXEL_FORMAT pixelFormat;
        //     pixelFormat.format = dxgiFormat;
        //      pixelFormat.alphaMode = alphaMode;
        //       return pixelFormat;
        //}

        //public D2D1_RENDER_TARGET_PROPERTIES RenderTargetProperties()
        //{
        //    return this.RenderTargetProperties(D2D1_RENDER_TARGET_TYPE.D2D1_RENDER_TARGET_TYPE_DEFAULT, PixelFormat(),
        //        0.0f,
        //        0.0f,
        //        D2D1_RENDER_TARGET_USAGE.D2D1_RENDER_TARGET_USAGE_NONE,
        //        D2D1_FEATURE_LEVEL.D2D1_FEATURE_LEVEL_DEFAULT);
        //}

        //public D2D1_RENDER_TARGET_PROPERTIES RenderTargetProperties(
        //    D2D1_RENDER_TARGET_TYPE type,
        //    D2D1_PIXEL_FORMAT pixelFormat,
        //    float dpiX,
        //    float dpiY,
        //    D2D1_RENDER_TARGET_USAGE usage,
        //    D2D1_FEATURE_LEVEL minLevel
        //    )
        //{
        //    D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties;

        //    renderTargetProperties.type = type;
        //    renderTargetProperties.pixelFormat = pixelFormat;
        //    renderTargetProperties.dpiX = dpiX;
        //    renderTargetProperties.dpiY = dpiY;
        //    renderTargetProperties.usage = usage;
        //    renderTargetProperties.minLevel = minLevel;

        //    return renderTargetProperties;
        //}

        static public D2D1_RENDER_TARGET_PROPERTIES RenderTargetProperties(D2D1_RENDER_TARGET_TYPE type, D2D1_PIXEL_FORMAT pixelFormat, float dpiX = 0.0f, float dpiY = 0.0f,
            D2D1_RENDER_TARGET_USAGE usage = D2D1_RENDER_TARGET_USAGE.D2D1_RENDER_TARGET_USAGE_NONE, D2D1_FEATURE_LEVEL minLevel = D2D1_FEATURE_LEVEL.D2D1_FEATURE_LEVEL_DEFAULT)
        {
            D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties;
            renderTargetProperties.type = type;
            renderTargetProperties.pixelFormat = pixelFormat;
            renderTargetProperties.dpiX = dpiX;
            renderTargetProperties.dpiY = dpiY;
            renderTargetProperties.usage = usage;
            renderTargetProperties.minLevel = minLevel;
            return renderTargetProperties;
        }
        public static D2D1_HWND_RENDER_TARGET_PROPERTIES HwndRenderTargetProperties(IntPtr hwnd, D2D1_SIZE_U pixelSize, D2D1_PRESENT_OPTIONS presentOptions = D2D1_PRESENT_OPTIONS.D2D1_PRESENT_OPTIONS_NONE)
        {
            D2D1_HWND_RENDER_TARGET_PROPERTIES hwndRenderTargetProperties;
            hwndRenderTargetProperties.hwnd = hwnd;
            hwndRenderTargetProperties.pixelSize = pixelSize;
            hwndRenderTargetProperties.presentOptions = presentOptions;
            return hwndRenderTargetProperties;
        }

        public const int D3D11_SDK_VERSION = 7;

        //private extern static int CreateD3D11Device(IntPtr adapter, int driverType, IntPtr softwareRasterizer, int flags, IntPtr featureLevels, 
        //    int featureLevelCount, int sdkVersion, out IntPtr device, out IntPtr featureLevel, out IntPtr immediateContext);


        [DllImport("D3D11.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern HRESULT D3D11CreateDevice(IDXGIAdapter pAdapter, D3D_DRIVER_TYPE DriverType, IntPtr Software, uint Flags, [MarshalAs(UnmanagedType.LPArray)] int[] pFeatureLevels,
             //uint FeatureLevels, uint SDKVersion, out ID3D11Device ppDevice, out D3D_FEATURE_LEVEL pFeatureLevel, out ID3D11DeviceContext ppImmediateContext);
             uint FeatureLevels, uint SDKVersion, out IntPtr ppDevice, out D3D_FEATURE_LEVEL pFeatureLevel, out ID3D11DeviceContext ppImmediateContext);

        [DllImport("D3D11.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern HRESULT D3D11CreateDeviceAndSwapChain(IDXGIAdapter pAdapter, D3D_DRIVER_TYPE DriverType, IntPtr Software, uint Flags, IntPtr pFeatureLevels, uint FeatureLevels,
            //uint SDKVersion, ref DXGI_SWAP_CHAIN_DESC pSwapChainDesc, out IDXGISwapChain ppSwapChain, [Out, MarshalAs(UnmanagedType.SysUInt)] out IntPtr ppDevice, out D3D_FEATURE_LEVEL pFeatureLevel, out IntPtr ppImmediateContext);
            uint SDKVersion, ref DXGI_SWAP_CHAIN_DESC pSwapChainDesc, out IDXGISwapChain ppSwapChain, out IntPtr ppDevice, out D3D_FEATURE_LEVEL pFeatureLevel, out IntPtr ppImmediateContext);

        public const int DXGI_USAGE_SHADER_INPUT = 0x00000010;
        public const int DXGI_USAGE_RENDER_TARGET_OUTPUT = 0x00000020;
        public const int DXGI_USAGE_BACK_BUFFER = 0x00000040;
        public const int DXGI_USAGE_SHARED = 0x00000080;
        public const int DXGI_USAGE_READ_ONLY = 0x00000100;
        public const int DXGI_USAGE_DISCARD_ON_PRESENT = 0x00000200;
        public const int DXGI_USAGE_UNORDERED_ACCESS = 0x00000400;


        [StructLayout(LayoutKind.Sequential)]
        public struct D2D1_CREATION_PROPERTIES
        {
            D2D1_THREADING_MODE threadingMode;
            D2D1_DEBUG_LEVEL debugLevel;
            D2D1_DEVICE_CONTEXT_OPTIONS options;
        }

        public enum D2D1_THREADING_MODE : uint
        {
            /// <summary>
            /// Resources may only be invoked serially.  Reference counts on resources are
            /// interlocked, however, resource and render target state is not protected from
            /// multi-threaded access
            /// </summary>
            D2D1_THREADING_MODE_SINGLE_THREADED = D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_SINGLE_THREADED,
            /// <summary>
            /// Resources may be invoked from multiple threads. Resources use interlocked
            /// reference counting and their state is protected.
            /// </summary>
            D2D1_THREADING_MODE_MULTI_THREADED = D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_MULTI_THREADED,
            D2D1_THREADING_MODE_FORCE_DWORD = 0xffffffff
        }

        [DllImport("D2D1.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern float D2D1ComputeMaximumScaleFactor(ref D2D1_MATRIX_3X2_F matrix);

        // 
        // Set to alignedByteOffset within D2D1_INPUT_ELEMENT_DESC for elements that 
        // immediately follow preceding elements in memory
        //
        public const uint D2D1_APPEND_ALIGNED_ELEMENT = 0xffffffff;

    }

    public enum DWRITE_TEXT_ALIGNMENT
    {
        /// <summary>
        /// The leading edge of the paragraph text is aligned to the layout box's leading edge.
        /// </summary>
        DWRITE_TEXT_ALIGNMENT_LEADING,
        /// <summary>
        /// The trailing edge of the paragraph text is aligned to the layout box's trailing edge.
        /// </summary>
        DWRITE_TEXT_ALIGNMENT_TRAILING,
        /// <summary>
        /// The center of the paragraph text is aligned to the center of the layout box.
        /// </summary>
        DWRITE_TEXT_ALIGNMENT_CENTER,
        /// <summary>
        /// Align text to the leading side, and also justify text to fill the lines.
        /// </summary>
        DWRITE_TEXT_ALIGNMENT_JUSTIFIED
    };

    public enum DWRITE_PARAGRAPH_ALIGNMENT
    {
        /// <summary>
        /// The first line of paragraph is aligned to the flow's beginning edge of the layout box.
        /// </summary>
        DWRITE_PARAGRAPH_ALIGNMENT_NEAR,
        /// <summary>
        /// The last line of paragraph is aligned to the flow's ending edge of the layout box.
        /// </summary>
        DWRITE_PARAGRAPH_ALIGNMENT_FAR,
        /// <summary>
        /// The center of the paragraph is aligned to the center of the flow of the layout box.
        /// </summary>
        DWRITE_PARAGRAPH_ALIGNMENT_CENTER
    };
    public enum DWRITE_WORD_WRAPPING
    {
        /// <summary>
        /// Words are broken across lines to avoid text overflowing the layout box.
        /// </summary>
        DWRITE_WORD_WRAPPING_WRAP = 0,
        /// <summary>
        /// Words are kept within the same line even when it overflows the layout box.
        /// This option is often used with scrolling to reveal overflow text. 
        /// </summary>
        DWRITE_WORD_WRAPPING_NO_WRAP = 1,
        /// <summary>
        /// Words are broken across lines to avoid text overflowing the layout box.
        /// Emergency wrapping occurs if the word is larger than the maximum width.
        /// </summary>
        DWRITE_WORD_WRAPPING_EMERGENCY_BREAK = 2,
        /// <summary>
        /// Only wrap whole words, never breaking words (emergency wrapping) when the
        /// layout width is too small for even a single word.
        /// </summary>
        DWRITE_WORD_WRAPPING_WHOLE_WORD = 3,
        /// <summary>
        /// Wrap between any valid characters clusters.
        /// </summary>
        DWRITE_WORD_WRAPPING_CHARACTER = 4,
    };

    public enum DWRITE_LINE_SPACING_METHOD
    {
        /// <summary>
        /// Line spacing depends solely on the content, growing to accommodate the size of fonts and inline objects.
        /// </summary>
        DWRITE_LINE_SPACING_METHOD_DEFAULT,
        /// <summary>
        /// Lines are explicitly set to uniform spacing, regardless of contained font sizes.
        /// This can be useful to avoid the uneven appearance that can occur from font fallback.
        /// </summary>
        DWRITE_LINE_SPACING_METHOD_UNIFORM,
        /// <summary>
        /// Line spacing and baseline distances are proportional to the computed values based on the content, the size of the fonts and inline objects.
        /// </summary>
        DWRITE_LINE_SPACING_METHOD_PROPORTIONAL
    };
    public enum DWRITE_READING_DIRECTION
    {
        /// <summary>
        /// Reading progresses from left to right.
        /// </summary>
        DWRITE_READING_DIRECTION_LEFT_TO_RIGHT = 0,
        /// <summary>
        /// Reading progresses from right to left.
        /// </summary>
        DWRITE_READING_DIRECTION_RIGHT_TO_LEFT = 1,
        /// <summary>
        /// Reading progresses from top to bottom.
        /// </summary>
        DWRITE_READING_DIRECTION_TOP_TO_BOTTOM = 2,
        /// <summary>
        /// Reading progresses from bottom to top.
        /// </summary>
        DWRITE_READING_DIRECTION_BOTTOM_TO_TOP = 3,
    };

    public enum DWRITE_FLOW_DIRECTION
    {
        /// <summary>
        /// Text lines are placed from top to bottom.
        /// </summary>
        DWRITE_FLOW_DIRECTION_TOP_TO_BOTTOM = 0,
        /// <summary>
        /// Text lines are placed from bottom to top.
        /// </summary>
        DWRITE_FLOW_DIRECTION_BOTTOM_TO_TOP = 1,
        /// <summary>
        /// Text lines are placed from left to right.
        /// </summary>
        DWRITE_FLOW_DIRECTION_LEFT_TO_RIGHT = 2,
        /// <summary>
        /// Text lines are placed from right to left.
        /// </summary>
        DWRITE_FLOW_DIRECTION_RIGHT_TO_LEFT = 3,
    };

    public enum DWRITE_TRIMMING_GRANULARITY
    {
        /// <summary>
        /// No trimming occurs. Text flows beyond the layout width.
        /// </summary>
        DWRITE_TRIMMING_GRANULARITY_NONE,
        /// <summary>
        /// Trimming occurs at character cluster boundary.
        /// </summary>
        DWRITE_TRIMMING_GRANULARITY_CHARACTER,
        /// <summary>
        /// Trimming occurs at word boundary.
        /// </summary>
        DWRITE_TRIMMING_GRANULARITY_WORD
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct DWRITE_TRIMMING
    {
        /// <summary>
        /// Text granularity of which trimming applies.
        /// </summary>
        public DWRITE_TRIMMING_GRANULARITY granularity;
        /// <summary>
        /// Character code used as the delimiter signaling the beginning of the portion of text to be preserved,
        /// most useful for path ellipsis, where the delimiter would be a slash. Leave this zero if there is no
        /// delimiter.
        /// </summary>
        public uint delimiter;
        /// <summary>
        /// How many occurrences of the delimiter to step back. Leave this zero if there is no delimiter.
        /// </summary>
        public uint delimiterCount;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct DWRITE_INLINE_OBJECT_METRICS
    {
        /// <summary>
        /// Width of the inline object.
        /// </summary>
        public float width;
        /// <summary>
        /// Height of the inline object as measured from top to bottom.
        /// </summary>
        public float height;
        /// <summary>
        /// Distance from the top of the object to the baseline where it is lined up with the adjacent text.
        /// If the baseline is at the bottom, baseline simply equals height.
        /// </summary>
        public float baseline;
        /// <summary>
        /// Flag indicating whether the object is to be placed upright or alongside the text baseline
        /// for vertical text.
        /// </summary>
        public bool supportsSideways;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct DWRITE_OVERHANG_METRICS
    {
        /// <summary>
        /// The distance from the left-most visible DIP to its left alignment edge.
        /// </summary>
        public float left;
        /// <summary>
        /// The distance from the top-most visible DIP to its top alignment edge.
        /// </summary>
        public float top;
        /// <summary>
        /// The distance from the right-most visible DIP to its right alignment edge.
        /// </summary>
        public float right;
        /// <summary>
        /// The distance from the bottom-most visible DIP to its bottom alignment edge.
        /// </summary>
        public float bottom;
    };

    public enum DWRITE_BREAK_CONDITION
    {
        /// <summary>
        /// Whether a break is allowed is determined by the condition of the
        /// neighboring text span or inline object.
        /// </summary>
        DWRITE_BREAK_CONDITION_NEUTRAL,
        /// <summary>
        /// A break is allowed, unless overruled by the condition of the
        /// neighboring text span or inline object, either prohibited by a
        /// May Not or forced by a Must.
        /// </summary>
        DWRITE_BREAK_CONDITION_CAN_BREAK,
        /// <summary>
        /// There should be no break, unless overruled by a Must condition from
        /// the neighboring text span or inline object.
        /// </summary>
        DWRITE_BREAK_CONDITION_MAY_NOT_BREAK,
        /// <summary>
        /// The break must happen, regardless of the condition of the adjacent
        /// text span or inline object.
        /// </summary>
        DWRITE_BREAK_CONDITION_MUST_BREAK
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct DWRITE_GLYPH_OFFSET
    {
        /// <summary>
        /// Offset in the advance direction of the run. A positive advance offset moves the glyph to the right
        /// (in pre-transform coordinates) if the run is left-to-right or to the left if the run is right-to-left.
        /// </summary>
        public float advanceOffset;
        /// <summary>
        /// Offset in the ascent direction, i.e., the direction ascenders point. A positive ascender offset moves
        /// the glyph up (in pre-transform coordinates).
        /// </summary>
        public float ascenderOffset;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct DWRITE_GLYPH_RUN
    {
        /// <summary>
        /// The physical font face to draw with.
        /// </summary>
        //IDWriteFontFace* fontFace;
        public IntPtr fontFace;
        /// <summary>
        /// Logical size of the font in DIPs, not points (equals 1/96 inch).
        /// </summary>
        public float fontEmSize;
        /// <summary>
        /// The number of glyphs.
        /// </summary>
        public uint glyphCount;

        public UInt16 glyphIndices;
        /// <summary>
        /// Glyph advance widths.
        /// </summary>
        public float glyphAdvances;
        /// <summary>
        /// Glyph offsets.
        /// </summary>
        public DWRITE_GLYPH_OFFSET glyphOffsets;
        /// <summary>
        /// If true, specifies that glyphs are rotated 90 degrees to the left and
        /// vertical metrics are used. Vertical writing is achieved by specifying
        /// isSideways = true and rotating the entire run 90 degrees to the right
        /// via a rotate transform.
        /// </summary>
        public bool isSideways;
        /// <summary>
        /// The implicit resolved bidi level of the run. Odd levels indicate
        /// right-to-left languages like Hebrew and Arabic, while even levels
        /// indicate left-to-right languages like English and Japanese (when
        /// written horizontally). For right-to-left languages, the text origin
        /// is on the right, and text should be drawn to the left.
        /// </summary>
        public uint bidiLevel;
    };


    [StructLayout(LayoutKind.Sequential)]
    public struct DWRITE_GLYPH_RUN_DESCRIPTION
    {
        /// <summary>
        /// The locale name associated with this run.
        /// </summary>
        public string localeName;
        /// <summary>
        /// The text associated with the glyphs.
        /// </summary>
        public string str;
        /// <summary>
        /// The number of characters (UTF16 code-units).
        /// Note that this may be different than the number of glyphs.
        /// </summary>
        public uint stringLength;

        public UInt16 clusterMap;

        /// <summary>
        /// Corresponding text position in the original string
        /// this glyph run came from.
        /// </summary>
        public uint textPosition;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct DWRITE_UNDERLINE
    {
        /// <summary>
        /// Width of the underline, measured parallel to the baseline.
        /// </summary>
        public float width;
        /// <summary>
        /// Thickness of the underline, measured perpendicular to the
        /// baseline.
        /// </summary>
        public float thickness;
        /// <summary>
        /// Offset of the underline from the baseline.
        /// A positive offset represents a position below the baseline and
        /// a negative offset is above.
        /// </summary>
        public float offset;
        /// <summary>
        /// Height of the tallest run where the underline applies.
        /// </summary>
        public float runHeight;
        /// <summary>
        /// Reading direction of the text associated with the underline.  This 
        /// value is used to interpret whether the width value runs horizontally 
        /// or vertically.
        /// </summary>
        public DWRITE_READING_DIRECTION readingDirection;
        /// <summary>
        /// Flow direction of the text associated with the underline.  This value
        /// is used to interpret whether the thickness value advances top to 
        /// bottom, left to right, or right to left.
        /// </summary>
        public DWRITE_FLOW_DIRECTION flowDirection;
        /// <summary>
        /// Locale of the text the underline is being drawn under. Can be
        /// pertinent where the locale affects how the underline is drawn.
        /// For example, in vertical text, the underline belongs on the
        /// left for Chinese but on the right for Japanese.
        /// This choice is completely left up to higher levels.
        /// </summary>
        public string localeName;
        /// <summary>
        /// The measuring mode can be useful to the renderer to determine how
        /// underlines are rendered, e.g. rounding the thickness to a whole pixel
        /// in GDI-compatible modes.
        /// </summary>
        public DWRITE_MEASURING_MODE measuringMode;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct DWRITE_STRIKETHROUGH
    {
        /// <summary>
        /// Width of the strikethrough, measured parallel to the baseline.
        /// </summary>
        public float width;
        /// <summary>
        /// Thickness of the strikethrough, measured perpendicular to the
        /// baseline.
        /// </summary>
        public float thickness;
        /// <summary>
        /// Offset of the strikethrough from the baseline.
        /// A positive offset represents a position below the baseline and
        /// a negative offset is above.
        /// </summary>
        public float offset;
        /// <summary>
        /// Reading direction of the text associated with the strikethrough.  This
        /// value is used to interpret whether the width value runs horizontally 
        /// or vertically.
        /// </summary>
        public DWRITE_READING_DIRECTION readingDirection;
        /// <summary>
        /// Flow direction of the text associated with the strikethrough.  This 
        /// value is used to interpret whether the thickness value advances top to
        /// bottom, left to right, or right to left.
        /// </summary>
        public DWRITE_FLOW_DIRECTION flowDirection;
        /// <summary>
        /// Locale of the range. Can be pertinent where the locale affects the style.
        /// </summary>
        public string localeName;
        /// <summary>
        /// The measuring mode can be useful to the renderer to determine how
        /// underlines are rendered, e.g. rounding the thickness to a whole pixel
        /// in GDI-compatible modes.
        /// </summary>
        public DWRITE_MEASURING_MODE measuringMode;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct DWRITE_MATRIX
    {
        /// <summary>
        /// Horizontal scaling / cosine of rotation
        /// </summary>
        public float m11;
        /// <summary>
        /// Vertical shear / sine of rotation
        /// </summary>
        public float m12;
        /// <summary>
        /// Horizontal shear / negative sine of rotation
        /// </summary>
        public float m21;
        /// <summary>
        /// Vertical scaling / cosine of rotation
        /// </summary>
        public float m22;
        /// <summary>
        /// Horizontal shift (always orthogonal regardless of rotation)
        /// </summary>
        public float dx;
        /// <summary>
        /// Vertical shift (always orthogonal regardless of rotation)
        /// </summary>
        public float dy;
    };


    [ComImport]
    [Guid("eaf3a2da-ecf4-4d24-b644-b34f6842024b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWritePixelSnapping
    {
        HRESULT IsPixelSnappingDisabled(IntPtr clientDrawingContext, out bool isDisabled);
        HRESULT GetCurrentTransform(IntPtr clientDrawingContext, out DWRITE_MATRIX transform);
        HRESULT GetPixelsPerDip(IntPtr clientDrawingContext, out float pixelsPerDip);
    }

    [ComImport]
    [Guid("ef8a8135-5cc6-45fe-8825-c5a0724eb819")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteTextRenderer : IDWritePixelSnapping
    {
        #region IDWritePixelSnapping
        new HRESULT IsPixelSnappingDisabled(IntPtr clientDrawingContext, out bool isDisabled);
        new HRESULT GetCurrentTransform(IntPtr clientDrawingContext, out DWRITE_MATRIX transform);
        new HRESULT GetPixelsPerDip(IntPtr clientDrawingContext, out float pixelsPerDip);
        #endregion

        //HRESULT DrawGlyphRun(IntPtr clientDrawingContext, float baselineOriginX, float baselineOriginY, DWRITE_MEASURING_MODE measuringMode,
        //    DWRITE_GLYPH_RUN  glyphRun, DWRITE_GLYPH_RUN_DESCRIPTION glyphRunDescription, IUnknown* clientDrawingEffect);
        HRESULT DrawGlyphRun(IntPtr clientDrawingContext, float baselineOriginX, float baselineOriginY, DWRITE_MEASURING_MODE measuringMode,
         DWRITE_GLYPH_RUN glyphRun, DWRITE_GLYPH_RUN_DESCRIPTION glyphRunDescription, IntPtr clientDrawingEffect);
        //HRESULT DrawUnderline(IntPtr clientDrawingContext, float baselineOriginX, float baselineOriginY, DWRITE_UNDERLINE underline, IUnknown* clientDrawingEffect);
        HRESULT DrawUnderline(IntPtr clientDrawingContext, float baselineOriginX, float baselineOriginY, DWRITE_UNDERLINE underline, IntPtr clientDrawingEffect);
        //HRESULT DrawStrikethrough(IntPtr clientDrawingContext, float baselineOriginX, float baselineOriginY, DWRITE_STRIKETHROUGH strikethrough, IUnknown* clientDrawingEffect);
        HRESULT DrawStrikethrough(IntPtr clientDrawingContext, float baselineOriginX, float baselineOriginY, DWRITE_STRIKETHROUGH strikethrough, IntPtr clientDrawingEffect);
        //HRESULT DrawInlineObject(IntPtr clientDrawingContext, float originX, float originY, IDWriteInlineObject inlineObject, bool isSideways, bool isRightToLeft, IUnknown* clientDrawingEffect);
        HRESULT DrawInlineObject(IntPtr clientDrawingContext, float originX, float originY, IDWriteInlineObject inlineObject, bool isSideways, bool isRightToLeft, IntPtr clientDrawingEffect);
    }

    [ComImport]
    [Guid("8339FDE3-106F-47ab-8373-1C6295EB10B3")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteInlineObject
    {
        //HRESULT Draw(IntPtr clientDrawingContext, IDWriteTextRenderer renderer, float originX, float originY, bool isSideways, bool isRightToLeft, IUnknown* clientDrawingEffect);
        HRESULT Draw(IntPtr clientDrawingContext, IDWriteTextRenderer renderer, float originX, float originY, bool isSideways, bool isRightToLeft, IntPtr clientDrawingEffect);
        HRESULT GetMetrics(out DWRITE_INLINE_OBJECT_METRICS metrics);
        HRESULT GetOverhangMetrics(out DWRITE_OVERHANG_METRICS overhangs);
        HRESULT GetBreakConditions(out DWRITE_BREAK_CONDITION breakConditionBefore, out DWRITE_BREAK_CONDITION breakConditionAfter);
    }

    public enum DWRITE_FONT_WEIGHT
    {
        /// <summary>
        /// Predefined font weight : Thin (100).
        /// </summary>
        DWRITE_FONT_WEIGHT_THIN = 100,
        /// <summary>
        /// Predefined font weight : Extra-light (200).
        /// </summary>
        DWRITE_FONT_WEIGHT_EXTRA_LIGHT = 200,
        /// <summary>
        /// Predefined font weight : Ultra-light (200).
        /// </summary>
        DWRITE_FONT_WEIGHT_ULTRA_LIGHT = 200,
        /// <summary>
        /// Predefined font weight : Light (300).
        /// </summary>
        DWRITE_FONT_WEIGHT_LIGHT = 300,
        /// <summary>
        /// Predefined font weight : Semi-light (350).
        /// </summary>
        DWRITE_FONT_WEIGHT_SEMI_LIGHT = 350,
        /// <summary>
        /// Predefined font weight : Normal (400).
        /// </summary>
        DWRITE_FONT_WEIGHT_NORMAL = 400,
        /// <summary>
        /// Predefined font weight : Regular (400).
        /// </summary>
        DWRITE_FONT_WEIGHT_REGULAR = 400,
        /// <summary>
        /// Predefined font weight : Medium (500).
        /// </summary>
        DWRITE_FONT_WEIGHT_MEDIUM = 500,
        /// <summary>
        /// Predefined font weight : Demi-bold (600).
        /// </summary>
        DWRITE_FONT_WEIGHT_DEMI_BOLD = 600,
        /// <summary>
        /// Predefined font weight : Semi-bold (600).
        /// </summary>
        DWRITE_FONT_WEIGHT_SEMI_BOLD = 600,
        /// <summary>
        /// Predefined font weight : Bold (700).
        /// </summary>
        DWRITE_FONT_WEIGHT_BOLD = 700,
        /// <summary>
        /// Predefined font weight : Extra-bold (800).
        /// </summary>
        DWRITE_FONT_WEIGHT_EXTRA_BOLD = 800,
        /// <summary>
        /// Predefined font weight : Ultra-bold (800).
        /// </summary>
        DWRITE_FONT_WEIGHT_ULTRA_BOLD = 800,
        /// <summary>
        /// Predefined font weight : Black (900).
        /// </summary>
        DWRITE_FONT_WEIGHT_BLACK = 900,
        /// <summary>
        /// Predefined font weight : Heavy (900).
        /// </summary>
        DWRITE_FONT_WEIGHT_HEAVY = 900,
        /// <summary>
        /// Predefined font weight : Extra-black (950).
        /// </summary>
        DWRITE_FONT_WEIGHT_EXTRA_BLACK = 950,
        /// <summary>
        /// Predefined font weight : Ultra-black (950).
        /// </summary>
        DWRITE_FONT_WEIGHT_ULTRA_BLACK = 950
    };

    /// <summary>
    /// The font stretch enumeration describes relative change from the normal aspect ratio
    /// as specified by a font designer for the glyphs in a font.
    /// Values less than 1 or greater than 9 are considered to be invalid, and they are rejected by font API functions.
    /// </summary>
    public enum DWRITE_FONT_STRETCH
    {
        /// <summary>
        /// Predefined font stretch : Not known (0).
        /// </summary>
        DWRITE_FONT_STRETCH_UNDEFINED = 0,
        /// <summary>
        /// Predefined font stretch : Ultra-condensed (1).
        /// </summary>
        DWRITE_FONT_STRETCH_ULTRA_CONDENSED = 1,
        /// <summary>
        /// Predefined font stretch : Extra-condensed (2).
        /// </summary>
        DWRITE_FONT_STRETCH_EXTRA_CONDENSED = 2,
        /// <summary>
        /// Predefined font stretch : Condensed (3).
        /// </summary>
        DWRITE_FONT_STRETCH_CONDENSED = 3,
        /// <summary>
        /// Predefined font stretch : Semi-condensed (4).
        /// </summary>
        DWRITE_FONT_STRETCH_SEMI_CONDENSED = 4,
        /// <summary>
        /// Predefined font stretch : Normal (5).
        /// </summary>
        DWRITE_FONT_STRETCH_NORMAL = 5,
        /// <summary>
        /// Predefined font stretch : Medium (5).
        /// </summary>
        DWRITE_FONT_STRETCH_MEDIUM = 5,
        /// <summary>
        /// Predefined font stretch : Semi-expanded (6).
        /// </summary>
        DWRITE_FONT_STRETCH_SEMI_EXPANDED = 6,
        /// <summary>
        /// Predefined font stretch : Expanded (7).
        /// </summary>
        DWRITE_FONT_STRETCH_EXPANDED = 7,
        /// <summary>
        /// Predefined font stretch : Extra-expanded (8).
        /// </summary>
        DWRITE_FONT_STRETCH_EXTRA_EXPANDED = 8,
        /// <summary>
        /// Predefined font stretch : Ultra-expanded (9).
        /// </summary>
        DWRITE_FONT_STRETCH_ULTRA_EXPANDED = 9
    };

    public enum DWRITE_FONT_STYLE
    {
        /// <summary>
        /// Font slope style : Normal.
        /// </summary>
        DWRITE_FONT_STYLE_NORMAL,
        /// <summary>
        /// Font slope style : Oblique.
        /// </summary>
        DWRITE_FONT_STYLE_OBLIQUE,
        /// <summary>
        /// Font slope style : Italic.
        /// </summary>
        DWRITE_FONT_STYLE_ITALIC
    };


    [ComImport]
    [Guid("a84cee02-3eea-4eee-a827-87c1a02a0fcc")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteFontCollection
    {
        [return: MarshalAs(UnmanagedType.U4)]
        [PreserveSig]
        uint GetFontFamilyCount();
        //HRESULT GetFontFamilyCount(out uint nFonFamilies);
        HRESULT GetFontFamily(uint index, out IDWriteFontFamily fontFamily);
        HRESULT FindFamilyName(string familyName, out uint index, out bool exists);
        HRESULT GetFontFromFontFace(IDWriteFontFace fontFace, out IDWriteFont font);
    }

    [ComImport]
    [Guid("cca920e4-52f0-492b-bfa8-29c72ee0a468")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteFontCollectionLoader
    {
        //HRESULT CreateEnumeratorFromKey(IDWriteFactory factory, IntPtr collectionKey, uint collectionKeySize, out IDWriteFontFileEnumerator fontFileEnumerator);
        HRESULT CreateEnumeratorFromKey(IntPtr factory, IntPtr collectionKey, uint collectionKeySize, out IntPtr fontFileEnumerator);
    }

    [ComImport]
    [Guid("9c906818-31d7-4fd3-a151-7c5e225db55a")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteTextFormat
    {
        HRESULT SetTextAlignment(DWRITE_TEXT_ALIGNMENT textAlignment);
        HRESULT SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT paragraphAlignment);
        HRESULT SetWordWrapping(DWRITE_WORD_WRAPPING wordWrapping);
        HRESULT SetReadingDirection(DWRITE_READING_DIRECTION readingDirection);
        HRESULT SetFlowDirection(DWRITE_FLOW_DIRECTION flowDirection);
        HRESULT SetIncrementalTabStop(float incrementalTabStop);
        HRESULT SetTrimming(DWRITE_TRIMMING trimmingOptions, IDWriteInlineObject trimmingSign);
        HRESULT SetLineSpacing(DWRITE_LINE_SPACING_METHOD lineSpacingMethod, float lineSpacing, float baseline);
        DWRITE_TEXT_ALIGNMENT GetTextAlignment();
        DWRITE_PARAGRAPH_ALIGNMENT GetParagraphAlignment();
        DWRITE_WORD_WRAPPING GetWordWrapping();
        DWRITE_READING_DIRECTION GetReadingDirection();
        DWRITE_FLOW_DIRECTION GetFlowDirection();
        float GetIncrementalTabStop();
        HRESULT GetTrimming(out DWRITE_TRIMMING trimmingOptions, out IDWriteInlineObject trimmingSign);
        HRESULT GetLineSpacing(out DWRITE_LINE_SPACING_METHOD lineSpacingMethod, out float lineSpacing, out float baseline);
        HRESULT GetFontCollection(out IDWriteFontCollection fontCollection);
        uint GetFontFamilyNameLength();
        HRESULT GetFontFamilyName(out string fontFamilyName, uint nameSize);
        DWRITE_FONT_WEIGHT GetFontWeight();
        DWRITE_FONT_STYLE GetFontStyle();
        DWRITE_FONT_STRETCH GetFontStretch();
        float GetFontSize();
        uint GetLocaleNameLength();
        HRESULT GetLocaleName(out string localeName, uint nameSize);
    }

    [ComImport]
    [Guid("727cad4e-d6af-4c9e-8a08-d695b11caa49")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteFontFileLoader
    {
        HRESULT CreateStreamFromKey(IntPtr fontFileReferenceKey, int fontFileReferenceKeySize, out IDWriteFontFileStream fontFileStream);
    }

    [ComImport]
    [Guid("6d4865fe-0ab8-4d91-8f62-5dd6be34a3e0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteFontFileStream
    {
        HRESULT ReadFileFragment(out IntPtr fragmentStart, UInt64 fileOffset, UInt64 fragmentSize, out IntPtr fragmentContext);
        void ReleaseFileFragment(IntPtr fragmentContext);
        HRESULT GetFileSize(out UInt64 fileSize);
        HRESULT GetLastWriteTime(out UInt64 lastWriteTime);
    }

    public enum DWRITE_FONT_FILE_TYPE
    {
        /// <summary>
        /// Font type is not recognized by the DirectWrite font system.
        /// </summary>
        DWRITE_FONT_FILE_TYPE_UNKNOWN,
        /// <summary>
        /// OpenType font with CFF outlines.
        /// </summary>
        DWRITE_FONT_FILE_TYPE_CFF,
        /// <summary>
        /// OpenType font with TrueType outlines.
        /// </summary>
        DWRITE_FONT_FILE_TYPE_TRUETYPE,
        /// <summary>
        /// OpenType font that contains a TrueType collection.
        /// </summary>
        DWRITE_FONT_FILE_TYPE_OPENTYPE_COLLECTION,
        /// <summary>
        /// Type 1 PFM font.
        /// </summary>
        DWRITE_FONT_FILE_TYPE_TYPE1_PFM,
        /// <summary>
        /// Type 1 PFB font.
        /// </summary>
        DWRITE_FONT_FILE_TYPE_TYPE1_PFB,
        /// <summary>
        /// Vector .FON font.
        /// </summary>
        DWRITE_FONT_FILE_TYPE_VECTOR,
        /// <summary>
        /// Bitmap .FON font.
        /// </summary>
        DWRITE_FONT_FILE_TYPE_BITMAP,
        // The following name is obsolete, but kept as an alias to avoid breaking existing code.
        DWRITE_FONT_FILE_TYPE_TRUETYPE_COLLECTION = DWRITE_FONT_FILE_TYPE_OPENTYPE_COLLECTION,
    };

    public enum DWRITE_FONT_FACE_TYPE
    {
        /// <summary>
        /// OpenType font face with CFF outlines.
        /// </summary>
        DWRITE_FONT_FACE_TYPE_CFF,
        /// <summary>
        /// OpenType font face with TrueType outlines.
        /// </summary>
        DWRITE_FONT_FACE_TYPE_TRUETYPE,
        /// <summary>
        /// OpenType font face that is a part of a TrueType or CFF collection.
        /// </summary>
        DWRITE_FONT_FACE_TYPE_OPENTYPE_COLLECTION,
        /// <summary>
        /// A Type 1 font face.
        /// </summary>
        DWRITE_FONT_FACE_TYPE_TYPE1,
        /// <summary>
        /// A vector .FON format font face.
        /// </summary>
        DWRITE_FONT_FACE_TYPE_VECTOR,
        /// <summary>
        /// A bitmap .FON format font face.
        /// </summary>
        DWRITE_FONT_FACE_TYPE_BITMAP,
        /// <summary>
        /// Font face type is not recognized by the DirectWrite font system.
        /// </summary>
        DWRITE_FONT_FACE_TYPE_UNKNOWN,
        /// <summary>
        /// The font data includes only the CFF table from an OpenType CFF font.
        /// This font face type can be used only for embedded fonts (i.e., custom
        /// font file loaders) and the resulting font face object supports only the
        /// minimum functionality necessary to render glyphs.
        /// </summary>
        DWRITE_FONT_FACE_TYPE_RAW_CFF,
        // The following name is obsolete, but kept as an alias to avoid breaking existing code.
        DWRITE_FONT_FACE_TYPE_TRUETYPE_COLLECTION = DWRITE_FONT_FACE_TYPE_OPENTYPE_COLLECTION,
    };

    [ComImport]
    [Guid("739d886a-cef5-47dc-8769-1a8b41bebbb0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteFontFile
    {
        HRESULT GetReferenceKey(out IntPtr fontFileReferenceKey, out int fontFileReferenceKeySize);

        HRESULT GetLoader(out IDWriteFontFileLoader fontFileLoader);

        HRESULT Analyze(out bool isSupportedFontType, out DWRITE_FONT_FILE_TYPE fontFileType, out DWRITE_FONT_FACE_TYPE fontFaceType, out int numberOfFaces);
    }

    public enum DWRITE_FONT_SIMULATIONS
    {
        /// <summary>
        /// No simulations are performed.
        /// </summary>
        DWRITE_FONT_SIMULATIONS_NONE = 0x0000,
        /// <summary>
        /// Algorithmic emboldening is performed.
        /// </summary>
        DWRITE_FONT_SIMULATIONS_BOLD = 0x0001,
        /// <summary>
        /// Algorithmic italicization is performed.
        /// </summary>
        DWRITE_FONT_SIMULATIONS_OBLIQUE = 0x0002
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct DWRITE_FONT_METRICS
    {
        /// <summary>
        /// The number of font design units per em unit.
        /// Font files use their own coordinate system of font design units.
        /// A font design unit is the smallest measurable unit in the em square,
        /// an imaginary square that is used to size and align glyphs.
        /// The concept of em square is used as a reference scale factor when defining font size and device transformation semantics.
        /// The size of one em square is also commonly used to compute the paragraph indentation value.
        /// </summary>
        public UInt16 designUnitsPerEm;
        /// <summary>
        /// Ascent value of the font face in font design units.
        /// Ascent is the distance from the top of font character alignment box to English baseline.
        /// </summary>
        public UInt16 ascent;
        /// <summary>
        /// Descent value of the font face in font design units.
        /// Descent is the distance from the bottom of font character alignment box to English baseline.
        /// </summary>
        public UInt16 descent;
        /// <summary>
        /// Line gap in font design units.
        /// Recommended additional white space to add between lines to improve legibility. The recommended line spacing 
        /// (baseline-to-baseline distance) is thus the sum of ascent, descent, and lineGap. The line gap is usually 
        /// positive or zero but can be negative, in which case the recommended line spacing is less than the height
        /// of the character alignment box.
        /// </summary>
        public UInt16 lineGap;
        /// <summary>
        /// Cap height value of the font face in font design units.
        /// Cap height is the distance from English baseline to the top of a typical English capital.
        /// Capital "H" is often used as a reference character for the purpose of calculating the cap height value.
        /// </summary>
        public UInt16 capHeight;
        /// <summary>
        /// x-height value of the font face in font design units.
        /// x-height is the distance from English baseline to the top of lowercase letter "x", or a similar lowercase character.
        /// </summary>
        public UInt16 xHeight;
        /// <summary>
        /// The underline position value of the font face in font design units.
        /// Underline position is the position of underline relative to the English baseline.
        /// The value is usually made negative in order to place the underline below the baseline.
        /// </summary>
        public UInt16 underlinePosition;
        /// <summary>
        /// The suggested underline thickness value of the font face in font design units.
        /// </summary>
        public UInt16 underlineThickness;
        /// <summary>
        /// The strikethrough position value of the font face in font design units.
        /// Strikethrough position is the position of strikethrough relative to the English baseline.
        /// The value is usually made positive in order to place the strikethrough above the baseline.
        /// </summary>
        public UInt16 strikethroughPosition;
        /// <summary>
        /// The suggested strikethrough thickness value of the font face in font design units.
        /// </summary>
        public UInt16 strikethroughThickness;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct DWRITE_GLYPH_METRICS
    {
        /// <summary>
        /// Specifies the X offset from the glyph origin to the left edge of the black box.
        /// The glyph origin is the current horizontal writing position.
        /// A negative value means the black box extends to the left of the origin (often true for lowercase italic 'f').
        /// </summary>
        public int leftSideBearing;
        /// <summary>
        /// Specifies the X offset from the origin of the current glyph to the origin of the next glyph when writing horizontally.
        /// </summary>
        public int advanceWidth;
        /// <summary>
        /// Specifies the X offset from the right edge of the black box to the origin of the next glyph when writing horizontally.
        /// The value is negative when the right edge of the black box overhangs the layout box.
        /// </summary>
        public int rightSideBearing;
        /// <summary>
        /// Specifies the vertical offset from the vertical origin to the top of the black box.
        /// Thus, a positive value adds whitespace whereas a negative value means the glyph overhangs the top of the layout box.
        /// </summary>
        public int topSideBearing;
        /// <summary>
        /// Specifies the Y offset from the vertical origin of the current glyph to the vertical origin of the next glyph when writing vertically.
        /// (Note that the term "origin" by itself denotes the horizontal origin. The vertical origin is different.
        /// Its Y coordinate is specified by verticalOriginY value,
        /// and its X coordinate is half the advanceWidth to the right of the horizontal origin).
        /// </summary>
        public int advanceHeight;
        /// <summary>
        /// Specifies the vertical distance from the black box's bottom edge to the advance height.
        /// Positive when the bottom edge of the black box is within the layout box.
        /// Negative when the bottom edge of black box overhangs the layout box.
        /// </summary>
        public int bottomSideBearing;
        /// <summary>
        /// Specifies the Y coordinate of a glyph's vertical origin, in the font's design coordinate system.
        /// The y coordinate of a glyph's vertical origin is the sum of the glyph's top side bearing
        /// and the top (i.e. yMax) of the glyph's bounding box.
        /// </summary>
        public int verticalOriginY;
    };

    [ComImport]
    [Guid("5f49804d-7024-4d43-bfa9-d25984f53849")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteFontFace
    {
        DWRITE_FONT_FACE_TYPE GetType();
        HRESULT GetFiles([In, Out] int numberOfFiles, out IDWriteFontFile fontFiles);
        int GetIndex();
        DWRITE_FONT_SIMULATIONS GetSimulations();
        bool IsSymbolFont();
        void GetMetrics(out DWRITE_FONT_METRICS fontFaceMetrics);
        UInt16 GetGlyphCount();
        HRESULT GetDesignGlyphMetrics(UInt16 glyphIndices, int glyphCount, out DWRITE_GLYPH_METRICS glyphMetrics, bool isSideways = false);
        HRESULT GetGlyphIndices(int codePoints, int codePointCount, out UInt16 glyphIndices);
        HRESULT TryGetFontTable(int openTypeTableTag, out IntPtr tableData, out int tableSize, out IntPtr tableContext, out bool exists);
        void ReleaseFontTable(IntPtr tableContext);
        //HRESULT GetGlyphRunOutline(float emSize, UInt16 glyphIndices, float glyphAdvances, DWRITE_GLYPH_OFFSET glyphOffsets, int glyphCount, bool isSideways, bool isRightToLeft, IDWriteGeometrySink geometrySink);
        HRESULT GetGlyphRunOutline(float emSize, UInt16 glyphIndices, float glyphAdvances, DWRITE_GLYPH_OFFSET glyphOffsets, int glyphCount, bool isSideways, bool isRightToLeft, ID2D1SimplifiedGeometrySink geometrySink);

        HRESULT GetRecommendedRenderingMode(float emSize, float pixelsPerDip, DWRITE_MEASURING_MODE measuringMode, IDWriteRenderingParams renderingParams, out DWRITE_RENDERING_MODE renderingMode);
        HRESULT GetGdiCompatibleMetrics(float emSize, float pixelsPerDip, DWRITE_MATRIX transform, out DWRITE_FONT_METRICS fontFaceMetrics);
        HRESULT GetGdiCompatibleGlyphMetrics(float emSize, float pixelsPerDip, DWRITE_MATRIX transform, bool useGdiNatural, UInt16 glyphIndices, int glyphCount, out DWRITE_GLYPH_METRICS glyphMetrics, bool isSideways = false);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class LOGFONT
    {
        public int lfHeight = 0;
        public int lfWidth = 0;
        public int lfEscapement = 0;
        public int lfOrientation = 0;
        public int lfWeight = 0;
        public byte lfItalic = 0;
        public byte lfUnderline = 0;
        public byte lfStrikeOut = 0;
        public byte lfCharSet = 0;
        public byte lfOutPrecision = 0;
        public byte lfClipPrecision = 0;
        public byte lfQuality = 0;
        public byte lfPitchAndFamily = 0;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string lfFaceName = string.Empty;
    }

    [ComImport]
    [Guid("1edd9491-9853-4299-898f-6432983b6f3a")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteGdiInterop
    {
        HRESULT CreateFontFromLOGFONT(LOGFONT logFont, out IDWriteFont font);
        HRESULT ConvertFontToLOGFONT(IDWriteFont font, out LOGFONT logFont, out bool isSystemFont);
        HRESULT ConvertFontFaceToLOGFONT(IDWriteFontFace font, out LOGFONT logFont);
        HRESULT CreateFontFaceFromHdc(IntPtr hdc, out IDWriteFontFace fontFace);
        HRESULT CreateBitmapRenderTarget(IntPtr hdc, int width, int height, out IDWriteBitmapRenderTarget renderTarget);
    }  

    [ComImport]
    [Guid("5e5a32a3-8dff-4773-9ff6-0696eab77267")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteBitmapRenderTarget
    {
        HRESULT DrawGlyphRun(float baselineOriginX, float baselineOriginY, DWRITE_MEASURING_MODE measuringMode, DWRITE_GLYPH_RUN glyphRun, IDWriteRenderingParams renderingParams, int textColor, out RECT blackBoxRect);
        IntPtr GetMemoryDC();
        float GetPixelsPerDip();
        HRESULT SetPixelsPerDip(float pixelsPerDip);
        HRESULT GetCurrentTransform(out DWRITE_MATRIX transform);
        HRESULT SetCurrentTransform(DWRITE_MATRIX transform);
        HRESULT GetSize(out SIZE size);
        HRESULT Resize(int width, int height);
    }
    public enum DWRITE_INFORMATIONAL_STRING_ID
    {
        /// <summary>
        /// Unspecified name ID.
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_NONE,
        /// <summary>
        /// Copyright notice provided by the font.
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_COPYRIGHT_NOTICE,
        /// <summary>
        /// String containing a version number.
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_VERSION_STRINGS,
        /// <summary>
        /// Trademark information provided by the font.
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_TRADEMARK,
        /// <summary>
        /// Name of the font manufacturer.
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_MANUFACTURER,
        /// <summary>
        /// Name of the font designer.
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_DESIGNER,
        /// <summary>
        /// URL of font designer (with protocol, e.g., http://, ftp://).
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_DESIGNER_URL,
        /// <summary>
        /// Description of the font. Can contain revision information, usage recommendations, history, features, etc.
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_DESCRIPTION,
        /// <summary>
        /// URL of font vendor (with protocol, e.g., http://, ftp://). If a unique serial number is embedded in the URL, it can be used to register the font.
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_FONT_VENDOR_URL,
        /// <summary>
        /// Description of how the font may be legally used, or different example scenarios for licensed use. This field should be written in plain language, not legalese.
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_LICENSE_DESCRIPTION,
        /// <summary>
        /// URL where additional licensing information can be found.
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_LICENSE_INFO_URL,
        /// <summary>
        /// GDI-compatible family name. Because GDI allows a maximum of four fonts per family, fonts in the same family may have different GDI-compatible family names
        /// (e.g., "Arial", "Arial Narrow", "Arial Black").
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_WIN32_FAMILY_NAMES,
        /// <summary>
        /// GDI-compatible subfamily name.
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_WIN32_SUBFAMILY_NAMES,
        /// <summary>
        /// Family name preferred by the designer. This enables font designers to group more than four fonts in a single family without losing compatibility with
        /// GDI. This name is typically only present if it differs from the GDI-compatible family name.
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_PREFERRED_FAMILY_NAMES,
        /// <summary>
        /// Subfamily name preferred by the designer. This name is typically only present if it differs from the GDI-compatible subfamily name. 
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_PREFERRED_SUBFAMILY_NAMES,
        /// <summary>
        /// Sample text. This can be the font name or any other text that the designer thinks is the best example to display the font in.
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_SAMPLE_TEXT,
        /// <summary>
        /// The full name of the font, e.g. "Arial Bold", from name id 4 in the name table.
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_FULL_NAME,
        /// <summary>
        /// The postscript name of the font, e.g. "GillSans-Bold" from name id 6 in the name table.
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_POSTSCRIPT_NAME,
        /// <summary>
        /// The postscript CID findfont name, from name id 20 in the name table.
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_POSTSCRIPT_CID_NAME,
        /// <summary>
        /// Family name for the weight-width-slope model.
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_WWS_FAMILY_NAME,
        /// <summary>
        /// Script/language tag to identify the scripts or languages that the font was
        /// primarily designed to support. See DWRITE_FONT_PROPERTY_ID_DESIGN_SCRIPT_LANGUAGE_TAG
        /// for a longer description.
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_DESIGN_SCRIPT_LANGUAGE_TAG,
        /// <summary>
        /// Script/language tag to identify the scripts or languages that the font declares
        /// it is able to support.
        /// </summary>
        DWRITE_INFORMATIONAL_STRING_SUPPORTED_SCRIPT_LANGUAGE_TAG,
    };

    [ComImport]
    [Guid("acd16696-8c14-4f5d-877e-fe3fc1d32737")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteFont
    {
        HRESULT GetFontFamily(out IDWriteFontFamily fontFamily);
        DWRITE_FONT_WEIGHT GetWeight();
        DWRITE_FONT_STRETCH GetStretch();
        DWRITE_FONT_STYLE GetStyle();
        bool IsSymbolFont();
        HRESULT GetFaceNames(out IDWriteLocalizedStrings names);
        HRESULT GetInformationalStrings(DWRITE_INFORMATIONAL_STRING_ID informationalStringID, out IDWriteLocalizedStrings informationalStrings, out bool exists);
        DWRITE_FONT_SIMULATIONS GetSimulations();
        void GetMetrics(out DWRITE_FONT_METRICS fontMetrics);
        HRESULT HasCharacter(int unicodeValue, out bool exists);
        HRESULT CreateFontFace(out IDWriteFontFace fontFace);
    }

    [ComImport]
    [Guid("08256209-099a-4b34-b86d-c22b110e7771")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteLocalizedStrings
    {
        int GetCount();
        HRESULT FindLocaleName(string localeName, out int index, out bool exists);
        HRESULT GetLocaleNameLength(int index, out int length);
        HRESULT GetLocaleName(int index, out string localeName, int size);
        HRESULT GetStringLength(int index, out int length);
        HRESULT GetString(int index, out string stringBuffer, int size);
    }

    [ComImport]
    [Guid("da20d8ef-812a-4c43-9802-62ec4abd7add")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteFontFamily : IDWriteFontList
    {
        #region IDWriteFontList
        new HRESULT GetFontCollection(out IDWriteFontCollection fontCollection);
        new int GetFontCount();
        new HRESULT GetFont(int index, out IDWriteFont font);
        #endregion

        HRESULT GetFamilyNames(out IDWriteLocalizedStrings names);
        HRESULT GetFirstMatchingFont(DWRITE_FONT_WEIGHT weight, DWRITE_FONT_STRETCH stretch, DWRITE_FONT_STYLE style, out IDWriteFont matchingFont);
        HRESULT GetMatchingFonts(DWRITE_FONT_WEIGHT weight, DWRITE_FONT_STRETCH stretch, DWRITE_FONT_STYLE style, out IDWriteFontList matchingFonts);
    }

    [ComImport]
    [Guid("1a0d8438-1d97-4ec1-aef9-a2fb86ed6acb")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteFontList
    {
        HRESULT GetFontCollection(out IDWriteFontCollection fontCollection);
        int GetFontCount();
        HRESULT GetFont(int index, out IDWriteFont font);
    }

    [ComImport]
    [Guid("688e1a58-5094-47c8-adc8-fbcea60ae92b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteTextAnalysisSource
    {
        HRESULT GetTextAtPosition(int textPosition, out string textString, out int textLength);
        HRESULT GetTextBeforePosition(int textPosition, out string textString, out int textLength);
        DWRITE_READING_DIRECTION GetParagraphReadingDirection();
        HRESULT GetLocaleName(int textPosition, out int textLength, out string localeName);
        HRESULT GetNumberSubstitution(int textPosition, out int textLength, out IDWriteNumberSubstitution numberSubstitution);
    }

    [ComImport]
    [Guid("14885CC9-BAB0-4f90-B6ED-5C366A2CD03D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteNumberSubstitution
    {

    }
    public enum DWRITE_SCRIPT_SHAPES
    {
        /// <summary>
        /// No additional shaping requirement. Text is shaped with the writing system default behavior.
        /// </summary>
        DWRITE_SCRIPT_SHAPES_DEFAULT = 0,
        /// <summary>
        /// Text should leave no visual on display i.e. control or format control characters.
        /// </summary>
        DWRITE_SCRIPT_SHAPES_NO_VISUAL = 1
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct DWRITE_SCRIPT_ANALYSIS
    {
        /// <summary>
        /// Zero-based index representation of writing system script.
        /// </summary>
        public UInt16 script;
        /// <summary>
        /// Additional shaping requirement of text.
        /// </summary>
        public DWRITE_SCRIPT_SHAPES shapes;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct DWRITE_LINE_BREAKPOINT
    {
        /// <summary>
        /// Breaking condition before the character.
        /// </summary>
        [MarshalAs(UnmanagedType.U1, SizeConst = 2)]
        public byte breakConditionBefore;
        /// <summary>
        /// Breaking condition after the character.
        /// </summary>
        [MarshalAs(UnmanagedType.U1, SizeConst = 2)]
        public byte breakConditionAfter;
        /// <summary>
        /// The character is some form of whitespace, which may be meaningful
        /// for justification.
        /// </summary>
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte isWhitespace;
        /// <summary>
        /// The character is a soft hyphen, often used to indicate hyphenation
        /// points inside words.
        /// </summary>
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte isSoftHyphen;

        [MarshalAs(UnmanagedType.U1, SizeConst = 2)]
        public byte padding;
    };
    [ComImport]
    [Guid("5810cd44-0ca0-4701-b3fa-bec5182ae4f6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteTextAnalysisSink
    {
        HRESULT SetScriptAnalysis(int textPosition, int textLength, DWRITE_SCRIPT_ANALYSIS scriptAnalysis);
        HRESULT SetLineBreakpoints(int textPosition, int textLength, DWRITE_LINE_BREAKPOINT lineBreakpoints);
        HRESULT SetBidiLevel(int textPosition, int textLength, byte explicitLevel, byte resolvedLevel);
        HRESULT SetNumberSubstitution(int textPosition, int textLength, IDWriteNumberSubstitution numberSubstitution);
    }
    public enum DWRITE_TEXTURE_TYPE
    {
        /// <summary>
        /// Specifies an alpha texture for aliased text rendering (i.e., bi-level, where each pixel is either fully opaque or fully transparent),
        /// with one byte per pixel.
        /// </summary>
        DWRITE_TEXTURE_ALIASED_1x1,
        /// <summary>
        /// Specifies an alpha texture for ClearType text rendering, with three bytes per pixel in the horizontal dimension and 
        /// one byte per pixel in the vertical dimension.
        /// </summary>
        DWRITE_TEXTURE_CLEARTYPE_3x1
    };

    [ComImport]
    [Guid("7d97dbf7-e085-42d4-81e3-6a883bded118")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteGlyphRunAnalysis
    {
        HRESULT GetAlphaTextureBounds(DWRITE_TEXTURE_TYPE textureType, out RECT textureBounds);
        HRESULT CreateAlphaTexture(DWRITE_TEXTURE_TYPE textureType, RECT textureBounds, out IntPtr alphaValues, int bufferSize);
        HRESULT GetAlphaBlendParams(IDWriteRenderingParams renderingParams, out float blendGamma, out float blendEnhancedContrast, out float blendClearTypeLevel);

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DWRITE_TYPOGRAPHIC_FEATURES
    {
        /// <summary>
        /// Array of font features.
        /// </summary>
        public DWRITE_FONT_FEATURE features;

        /// <summary>
        /// The number of features.
        /// </summary>
        public int featureCount;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct DWRITE_SHAPING_TEXT_PROPERTIES
    {
        /// <summary>
        /// This character can be shaped independently from the others
        /// (usually set for the space character).
        /// </summary>
        [MarshalAs(UnmanagedType.U2, SizeConst = 1)]
        public UInt16 isShapedAlone;
        /// <summary>
        /// Reserved for use by shaping engine.
        /// </summary>
        [MarshalAs(UnmanagedType.U2, SizeConst = 15)]
        public UInt16 reserved;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct DWRITE_SHAPING_GLYPH_PROPERTIES
    {
        /// <summary>
        /// Justification class, whether to use spacing, kashidas, or
        /// another method. This exists for backwards compatibility
        /// with Uniscribe's SCRIPT_JUSTIFY enum.
        /// </summary>
        [MarshalAs(UnmanagedType.U2, SizeConst = 4)]
        public UInt16 justification;
        /// <summary>
        /// Indicates glyph is the first of a cluster.
        /// </summary>
        [MarshalAs(UnmanagedType.U2, SizeConst = 1)]
        public UInt16 isClusterStart1;
        /// <summary>
        /// Glyph is a diacritic.
        /// </summary>
        [MarshalAs(UnmanagedType.U2, SizeConst = 1)]
        public UInt16 isDiacritic;
        /// <summary>
        /// Glyph has no width, blank, ZWJ, ZWNJ etc.
        /// </summary>
        [MarshalAs(UnmanagedType.U2, SizeConst = 1)]
        public UInt16 isZeroWidthSpace;
        /// <summary>
        /// Reserved for use by shaping engine.
        /// </summary>
        [MarshalAs(UnmanagedType.U2, SizeConst = 9)]
        public UInt16 reserved;
    };

    [ComImport]
    [Guid("b7e6163e-7f46-43b4-84b3-e4e6249c365d")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteTextAnalyzer
    {
        HRESULT AnalyzeScript(IDWriteTextAnalysisSource analysisSource, int textPosition, int textLength, IDWriteTextAnalysisSink analysisSink);

        HRESULT AnalyzeBidi(IDWriteTextAnalysisSource analysisSource, int textPosition, int textLength, IDWriteTextAnalysisSink analysisSink);

        HRESULT AnalyzeNumberSubstitution(IDWriteTextAnalysisSource analysisSource, int textPosition, int textLength, IDWriteTextAnalysisSink analysisSink);

        HRESULT AnalyzeLineBreakpoints(IDWriteTextAnalysisSource analysisSource, int textPosition, int textLength, IDWriteTextAnalysisSink analysisSink);

        HRESULT GetGlyphs(string textString,
            int textLength,
            IDWriteFontFace fontFace,
            bool isSideways,
            bool isRightToLeft,
            DWRITE_SCRIPT_ANALYSIS scriptAnalysis,
           string localeName,
           IDWriteNumberSubstitution numberSubstitution,
            DWRITE_TYPOGRAPHIC_FEATURES features,
           int featureRangeLengths,
            int featureRanges,
            int maxGlyphCount,
            out UInt16 clusterMap,
            out DWRITE_SHAPING_TEXT_PROPERTIES textProps,
            out UInt16 glyphIndices,
            out DWRITE_SHAPING_GLYPH_PROPERTIES glyphProps,
            out int actualGlyphCount);

        HRESULT GetGlyphPlacements(string textString,
           UInt16 clusterMap,
           DWRITE_SHAPING_TEXT_PROPERTIES textProps,
            int textLength,
            UInt16 glyphIndices,
           DWRITE_SHAPING_GLYPH_PROPERTIES glyphProps,
        int glyphCount,
        IDWriteFontFace fontFace,
        float fontEmSize,
        bool isSideways,
        bool isRightToLeft,
        DWRITE_SCRIPT_ANALYSIS scriptAnalysis,
        string localeName,
       DWRITE_TYPOGRAPHIC_FEATURES features,
     int featureRangeLengths,
        int featureRanges,
        out float glyphAdvances,
        out DWRITE_GLYPH_OFFSET glyphOffsets);

        HRESULT GetGdiCompatibleGlyphPlacements(string textString,
           UInt16 clusterMap,
           DWRITE_SHAPING_TEXT_PROPERTIES textProps,
            int textLength,
           UInt16 glyphIndices,
           DWRITE_SHAPING_GLYPH_PROPERTIES glyphProps,
        int glyphCount,
        IDWriteFontFace fontFace,
        float fontEmSize,
        float pixelsPerDip,
       DWRITE_MATRIX transform,
        bool useGdiNatural,
        bool isSideways,
        bool isRightToLeft,
        DWRITE_SCRIPT_ANALYSIS scriptAnalysis,
       string localeName,
       DWRITE_TYPOGRAPHIC_FEATURES features,
      int featureRangeLengths,
        int featureRanges,
        out float glyphAdvances,
        out DWRITE_GLYPH_OFFSET glyphOffsets);
    }

    public enum DWRITE_NUMBER_SUBSTITUTION_METHOD
    {
        /// <summary>
        /// Specifies that the substitution method should be determined based
        /// on LOCALE_IDIGITSUBSTITUTION value of the specified text culture.
        /// </summary>
        DWRITE_NUMBER_SUBSTITUTION_METHOD_FROM_CULTURE,
        /// <summary>
        /// If the culture is Arabic or Farsi, specifies that the number shape
        /// depend on the context. Either traditional or nominal number shape
        /// are used depending on the nearest preceding strong character or (if
        /// there is none) the reading direction of the paragraph.
        /// </summary>
        DWRITE_NUMBER_SUBSTITUTION_METHOD_CONTEXTUAL,
        /// <summary>
        /// Specifies that code points 0x30-0x39 are always rendered as nominal numeral 
        /// shapes (ones of the European number), i.e., no substitution is performed.
        /// </summary>
        DWRITE_NUMBER_SUBSTITUTION_METHOD_NONE,
        /// <summary>
        /// Specifies that number are rendered using the national number shape 
        /// as specified by the LOCALE_SNATIVEDIGITS value of the specified text culture.
        /// </summary>
        DWRITE_NUMBER_SUBSTITUTION_METHOD_NATIONAL,
        /// <summary>
        /// Specifies that number are rendered using the traditional shape
        /// for the specified culture. For most cultures, this is the same as
        /// NativeNational. However, NativeNational results in Latin number
        /// for some Arabic cultures, whereas this value results in Arabic
        /// number for all Arabic cultures.
        /// </summary>
        DWRITE_NUMBER_SUBSTITUTION_METHOD_TRADITIONAL
    };

    [ComImport]
    [Guid("b859ee5a-d838-4b5b-a2e8-1adc7d93db48")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDWriteFactory
    {
        HRESULT GetSystemFontCollection(out IDWriteFontCollection fontCollection, bool checkForUpdates = false);
        HRESULT CreateCustomFontCollection(IDWriteFontCollectionLoader collectionLoader, IntPtr collectionKey, int collectionKeySize, out IDWriteFontCollection fontCollection);
        HRESULT RegisterFontCollectionLoader(IDWriteFontCollectionLoader fontCollectionLoader);
        HRESULT UnregisterFontCollectionLoader(IDWriteFontCollectionLoader fontCollectionLoader);
        HRESULT CreateFontFileReference(string filePath, System.Runtime.InteropServices.ComTypes.FILETIME lastWriteTime, out IDWriteFontFile fontFile);
        HRESULT CreateCustomFontFileReference(IntPtr fontFileReferenceKey, int fontFileReferenceKeySize, IDWriteFontFileLoader fontFileLoader, out IDWriteFontFile fontFile);
        HRESULT CreateFontFace(DWRITE_FONT_FACE_TYPE fontFaceType, int numberOfFiles, IDWriteFontFile fontFiles, int faceIndex, DWRITE_FONT_SIMULATIONS fontFaceSimulationFlags, out IDWriteFontFace fontFace);
        HRESULT CreateRenderingParams(out IDWriteRenderingParams renderingParams);
        HRESULT CreateMonitorRenderingParams(IntPtr monitor, out IDWriteRenderingParams renderingParams);
        HRESULT CreateCustomRenderingParams(float gamma, float enhancedContrast, float clearTypeLevel, DWRITE_PIXEL_GEOMETRY pixelGeometry, DWRITE_RENDERING_MODE renderingMode, out IDWriteRenderingParams renderingParams);
        HRESULT RegisterFontFileLoader(IDWriteFontFileLoader fontFileLoader);
        HRESULT UnregisterFontFileLoader(IDWriteFontFileLoader fontFileLoader);
        HRESULT CreateTextFormat(string fontFamilyName, IDWriteFontCollection fontCollection, DWRITE_FONT_WEIGHT fontWeight, DWRITE_FONT_STYLE fontStyle, DWRITE_FONT_STRETCH fontStretch, float fontSize,
             string localeName, out IDWriteTextFormat textFormat);
        HRESULT CreateTypography(out IDWriteTypography typography);
        HRESULT GetGdiInterop(out IDWriteGdiInterop gdiInterop);
        HRESULT CreateTextLayout(string str, int stringLength, IDWriteTextFormat textFormat, float maxWidth, float maxHeight, out IDWriteTextLayout textLayout);
        HRESULT CreateGdiCompatibleTextLayout(string str, int stringLength, IDWriteTextFormat textFormat, float layoutWidth,
            float layoutHeight, float pixelsPerDip, DWRITE_MATRIX transform, bool useGdiNatural, out IDWriteTextLayout textLayout);
        HRESULT CreateEllipsisTrimmingSign(IDWriteTextFormat textFormat, out IDWriteInlineObject trimmingSign);
        HRESULT CreateTextAnalyzer(out IDWriteTextAnalyzer textAnalyzer);
        HRESULT CreateNumberSubstitution(DWRITE_NUMBER_SUBSTITUTION_METHOD substitutionMethod, string localeName, bool ignoreUserOverride, out IDWriteNumberSubstitution numberSubstitution);
        HRESULT CreateGlyphRunAnalysis(DWRITE_GLYPH_RUN glyphRun, float pixelsPerDip, DWRITE_MATRIX transform, DWRITE_RENDERING_MODE renderingMode,
            DWRITE_MEASURING_MODE measuringMode, float baselineOriginX, float baselineOriginY, out IDWriteGlyphRunAnalysis glyphRunAnalysis);
    }

    public class ColorF : D2D1_COLOR_F
    {
        public enum Enum
        {
            AliceBlue = 0xF0F8FF,
            AntiqueWhite = 0xFAEBD7,
            Aqua = 0x00FFFF,
            Aquamarine = 0x7FFFD4,
            Azure = 0xF0FFFF,
            Beige = 0xF5F5DC,
            Bisque = 0xFFE4C4,
            Black = 0x000000,
            BlanchedAlmond = 0xFFEBCD,
            Blue = 0x0000FF,
            BlueViolet = 0x8A2BE2,
            Brown = 0xA52A2A,
            BurlyWood = 0xDEB887,
            CadetBlue = 0x5F9EA0,
            Chartreuse = 0x7FFF00,
            Chocolate = 0xD2691E,
            Coral = 0xFF7F50,
            CornflowerBlue = 0x6495ED,
            Cornsilk = 0xFFF8DC,
            Crimson = 0xDC143C,
            Cyan = 0x00FFFF,
            DarkBlue = 0x00008B,
            DarkCyan = 0x008B8B,
            DarkGoldenrod = 0xB8860B,
            DarkGray = 0xA9A9A9,
            DarkGreen = 0x006400,
            DarkKhaki = 0xBDB76B,
            DarkMagenta = 0x8B008B,
            DarkOliveGreen = 0x556B2F,
            DarkOrange = 0xFF8C00,
            DarkOrchid = 0x9932CC,
            DarkRed = 0x8B0000,
            DarkSalmon = 0xE9967A,
            DarkSeaGreen = 0x8FBC8F,
            DarkSlateBlue = 0x483D8B,
            DarkSlateGray = 0x2F4F4F,
            DarkTurquoise = 0x00CED1,
            DarkViolet = 0x9400D3,
            DeepPink = 0xFF1493,
            DeepSkyBlue = 0x00BFFF,
            DimGray = 0x696969,
            DodgerBlue = 0x1E90FF,
            Firebrick = 0xB22222,
            FloralWhite = 0xFFFAF0,
            ForestGreen = 0x228B22,
            Fuchsia = 0xFF00FF,
            Gainsboro = 0xDCDCDC,
            GhostWhite = 0xF8F8FF,
            Gold = 0xFFD700,
            Goldenrod = 0xDAA520,
            Gray = 0x808080,
            Green = 0x008000,
            GreenYellow = 0xADFF2F,
            Honeydew = 0xF0FFF0,
            HotPink = 0xFF69B4,
            IndianRed = 0xCD5C5C,
            Indigo = 0x4B0082,
            Ivory = 0xFFFFF0,
            Khaki = 0xF0E68C,
            Lavender = 0xE6E6FA,
            LavenderBlush = 0xFFF0F5,
            LawnGreen = 0x7CFC00,
            LemonChiffon = 0xFFFACD,
            LightBlue = 0xADD8E6,
            LightCoral = 0xF08080,
            LightCyan = 0xE0FFFF,
            LightGoldenrodYellow = 0xFAFAD2,
            LightGreen = 0x90EE90,
            LightGray = 0xD3D3D3,
            LightPink = 0xFFB6C1,
            LightSalmon = 0xFFA07A,
            LightSeaGreen = 0x20B2AA,
            LightSkyBlue = 0x87CEFA,
            LightSlateGray = 0x778899,
            LightSteelBlue = 0xB0C4DE,
            LightYellow = 0xFFFFE0,
            Lime = 0x00FF00,
            LimeGreen = 0x32CD32,
            Linen = 0xFAF0E6,
            Magenta = 0xFF00FF,
            Maroon = 0x800000,
            MediumAquamarine = 0x66CDAA,
            MediumBlue = 0x0000CD,
            MediumOrchid = 0xBA55D3,
            MediumPurple = 0x9370DB,
            MediumSeaGreen = 0x3CB371,
            MediumSlateBlue = 0x7B68EE,
            MediumSpringGreen = 0x00FA9A,
            MediumTurquoise = 0x48D1CC,
            MediumVioletRed = 0xC71585,
            MidnightBlue = 0x191970,
            MintCream = 0xF5FFFA,
            MistyRose = 0xFFE4E1,
            Moccasin = 0xFFE4B5,
            NavajoWhite = 0xFFDEAD,
            Navy = 0x000080,
            OldLace = 0xFDF5E6,
            Olive = 0x808000,
            OliveDrab = 0x6B8E23,
            Orange = 0xFFA500,
            OrangeRed = 0xFF4500,
            Orchid = 0xDA70D6,
            PaleGoldenrod = 0xEEE8AA,
            PaleGreen = 0x98FB98,
            PaleTurquoise = 0xAFEEEE,
            PaleVioletRed = 0xDB7093,
            PapayaWhip = 0xFFEFD5,
            PeachPuff = 0xFFDAB9,
            Peru = 0xCD853F,
            Pink = 0xFFC0CB,
            Plum = 0xDDA0DD,
            PowderBlue = 0xB0E0E6,
            Purple = 0x800080,
            Red = 0xFF0000,
            RosyBrown = 0xBC8F8F,
            RoyalBlue = 0x4169E1,
            SaddleBrown = 0x8B4513,
            Salmon = 0xFA8072,
            SandyBrown = 0xF4A460,
            SeaGreen = 0x2E8B57,
            SeaShell = 0xFFF5EE,
            Sienna = 0xA0522D,
            Silver = 0xC0C0C0,
            SkyBlue = 0x87CEEB,
            SlateBlue = 0x6A5ACD,
            SlateGray = 0x708090,
            Snow = 0xFFFAFA,
            SpringGreen = 0x00FF7F,
            SteelBlue = 0x4682B4,
            Tan = 0xD2B48C,
            Teal = 0x008080,
            Thistle = 0xD8BFD8,
            Tomato = 0xFF6347,
            Turquoise = 0x40E0D0,
            Violet = 0xEE82EE,
            Wheat = 0xF5DEB3,
            White = 0xFFFFFF,
            WhiteSmoke = 0xF5F5F5,
            Yellow = 0xFFFF00,
            YellowGreen = 0x9ACD32,
        };

        //
        // Construct a color, note that the alpha value from the "rgb" component
        // is never used.
        // 

        public ColorF(uint rgb, float a = 1.0f)
        {
            Init(rgb, a);
        }

        public ColorF(Enum knownColor, float a = 1.0f)
        {
            Init((uint)knownColor, a);
        }
        public ColorF(float red, float green, float blue, float alpha = 1.0f)
        {
            r = red;
            g = green;
            b = blue;
            a = alpha;
        }
        void Init(uint rgb, float alpha)
        {
            r = ((rgb & sc_redMask) >> (int)sc_redShift) / 255.0f;
            g = ((rgb & sc_greenMask) >> (int)sc_greenShift) / 255.0f;
            b = ((rgb & sc_blueMask) >> (int)sc_blueShift) / 255.0f;
            a = alpha;
        }
        const uint sc_redShift = 16;
        const uint sc_greenShift = 8;
        const uint sc_blueShift = 0;

        const uint sc_redMask = 0xff << (int)sc_redShift;
        const uint sc_greenMask = 0xff << (int)sc_greenShift;
        const uint sc_blueMask = 0xff << (int)sc_blueShift;
    };

    public class Matrix3x2F : D2D1_MATRIX_3X2_F
    {
        public Matrix3x2F(
          float m11,
          float m12,
          float m21,
          float m22,
          float m31,
          float m32
          )
        {
            _11 = m11;
            _12 = m12;
            _21 = m21;
            _22 = m22;
            _31 = m31;
            _32 = m32;
        }

        //
        // Creates an identity matrix
        // 
        public Matrix3x2F()
        {
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct Matrix3x2
        {
            /// <summary>
            /// Gets the identity matrix.
            /// </summary>
            /// <value>The identity matrix.</value>
            public readonly static Matrix3x2 Identity = new Matrix3x2(1, 0, 0, 1, 0, 0);

            /// <summary>
            /// Element (1,1)
            /// </summary>
            public float M11;

            /// <summary>
            /// Element (1,2)
            /// </summary>
            public float M12;

            /// <summary>
            /// Element (2,1)
            /// </summary>
            public float M21;

            /// <summary>
            /// Element (2,2)
            /// </summary>
            public float M22;

            /// <summary>
            /// Element (3,1)
            /// </summary>
            public float M31;

            /// <summary>
            /// Element (3,2)
            /// </summary>
            public float M32;

            /// <summary>
            /// Initializes a new instance of the <see cref="Matrix3x2"/> struct.
            /// </summary>
            /// <param name="value">The value that will be assigned to all components.</param>
            public Matrix3x2(float value)
            {
                M11 = M12 =
                M21 = M22 =
                M31 = M32 = value;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Matrix3x2"/> struct.
            /// </summary>
            /// <param name="M11">The value to assign at row 1 column 1 of the matrix.</param>
            /// <param name="M12">The value to assign at row 1 column 2 of the matrix.</param>
            /// <param name="M21">The value to assign at row 2 column 1 of the matrix.</param>
            /// <param name="M22">The value to assign at row 2 column 2 of the matrix.</param>
            /// <param name="M31">The value to assign at row 3 column 1 of the matrix.</param>
            /// <param name="M32">The value to assign at row 3 column 2 of the matrix.</param>
            public Matrix3x2(float M11, float M12, float M21, float M22, float M31, float M32)
            {
                this.M11 = M11; this.M12 = M12;
                this.M21 = M21; this.M22 = M22;
                this.M31 = M31; this.M32 = M32;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Matrix3x2"/> struct.
            /// </summary>
            /// <param name="values">The values to assign to the components of the matrix. This must be an array with six elements.</param>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
            /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than six elements.</exception>
            public Matrix3x2(float[] values)
            {
                if (values == null)
                    throw new ArgumentNullException("values");
                if (values.Length != 6)
                    throw new ArgumentOutOfRangeException("values", "There must be six input values for Matrix3x2.");

                M11 = values[0];
                M12 = values[1];

                M21 = values[2];
                M22 = values[3];

                M31 = values[4];
                M32 = values[5];
            }
        }

        //public static void MakeRotateMatrix(float angle, D2D1_POINT_2F center, out Matrix3x2 matrix)
        //{
        //    unsafe
        //    {
        //        IntPtr pRotation = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(D2D1_MATRIX_3X2_F)));
        //        D2D1_MATRIX_3X2_F rotation = new D2D1_MATRIX_3X2_F();
        //        D2DTools.D2D1MakeRotateMatrix_(angle, center,(void*) pRotation);
        //        rotation = (D2D1_MATRIX_3X2_F)Marshal.PtrToStructure(pRotation, typeof(D2D1_MATRIX_3X2_F));

        //        matrix = new Matrix3x2();
        //        fixed (void* matrix_ = &matrix)
        //            D2DTools.D2D1MakeRotateMatrix_(angle, center, matrix_);               
        //    }
        //}

        public static Matrix3x2 MakeRotateMatrix(float angle, D2D1_POINT_2F center = new D2D1_POINT_2F())
        {
            unsafe
            {
                IntPtr pRotation = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(D2D1_MATRIX_3X2_F)));
                D2D1_MATRIX_3X2_F rotation = new D2D1_MATRIX_3X2_F();
                D2DTools.D2D1MakeRotateMatrix_(angle, center, (void*)pRotation);
                rotation = (D2D1_MATRIX_3X2_F)Marshal.PtrToStructure(pRotation, typeof(D2D1_MATRIX_3X2_F));

                Matrix3x2 matrix = new Matrix3x2();
                void* matrix_ = &matrix;
                D2DTools.D2D1MakeRotateMatrix_(angle, center, matrix_);
                return matrix;
            }
        }

        public static Matrix3x2F Rotation(float angle, D2D1_POINT_2F center = new D2D1_POINT_2F())
        {
            D2D1_MATRIX_3X2_F rotation = new D2D1_MATRIX_3X2_F();
            IntPtr pRotation = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(D2D1_MATRIX_3X2_F)));

            //Marshal.StructureToPtr(rotation, pRotation, false);

            unsafe
            {
                //D2D1_MATRIX_3X2_F_BIS rot2 = new D2D1_MATRIX_3X2_F_BIS();
                D2DTools.D2D1MakeRotateMatrix_(angle, center, (void*)pRotation);
            }

            //IntPtr p = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(D2D1_MATRIX_3X2_F_BIS)));
            //D2DTools.D2D1MakeRotateMatrix(angle, center, out pRotation);
            // D2DTools.D2D1MakeRotateMatrix(angle, center, out p);

            //rot2 = (D2D1_MATRIX_3X2_F_BIS)Marshal.PtrToStructure(p, typeof(D2D1_MATRIX_3X2_F_BIS));

            //var ItemIDList = (D2D1_MATRIX_3X2_F)Marshal.PtrToStructure(pRotation, typeof(D2D1_MATRIX_3X2_F));
            rotation = (D2D1_MATRIX_3X2_F)Marshal.PtrToStructure(pRotation, typeof(D2D1_MATRIX_3X2_F));
            Matrix3x2F m = new Matrix3x2F(rotation._11, rotation._12, rotation._21, rotation._22, rotation._31, rotation._32);

            //D2DTools.D2D1MakeRotateMatrix(angle, center, out rotation);    

            return m;
        }

        public static Matrix3x2F Scale(D2D1_SIZE_F size, D2D1_POINT_2F center = new D2D1_POINT_2F())
        {
            Matrix3x2F scale = new Matrix3x2F();
            scale._11 = size.width; scale._12 = 0.0f;
            scale._21 = 0.0f; scale._22 = size.height;
            scale._31 = center.x - size.width * center.x;
            scale._32 = center.y - size.height * center.y;
            return scale;
        }

        public static Matrix3x2F Scale(float x, float y, D2D1_POINT_2F center = new D2D1_POINT_2F())
        {
            return Scale(new D2D1_SIZE_F(x, y), center);
        }

        public static Matrix3x2F Translation(D2D1_SIZE_F size)
        {
            Matrix3x2F translation = new Matrix3x2F();
            translation._11 = 1.0f; translation._12 = 0.0f;
            translation._21 = 0.0f; translation._22 = 1.0f;
            translation._31 = size.width; translation._32 = size.height;
            return translation;
        }

        public static Matrix3x2F Translation(float x, float y)
        {
            return Translation(new D2D1_SIZE_F(x, y));
        }

        public void SetProduct(Matrix3x2F a, Matrix3x2F b)
        {
            _11 = a._11 * b._11 + a._12 * b._21;
            _12 = a._11 * b._12 + a._12 * b._22;
            _21 = a._21 * b._11 + a._22 * b._21;
            _22 = a._21 * b._12 + a._22 * b._22;
            _31 = a._31 * b._11 + a._32 * b._21 + b._31;
            _32 = a._31 * b._12 + a._32 * b._22 + b._32;
        }

        public static Matrix3x2F operator *(Matrix3x2F b, Matrix3x2F c)
        {
            Matrix3x2F result = new Matrix3x2F();
            result.SetProduct(b, c);
            return result;
        }


        //        public static Matrix3x2F
        //            operator *(
        //                 Matrix3x2F matrix
        //               ) 
        //            {
        //               Matrix3x2F result = null;

        //            result.SetProduct(matrix);

        //                return result;
        //            }

        //        public static
        //   D2D1_MATRIX_3X2_F
        //operator *(
        //    D2D1_MATRIX_3X2_F matrix1,
        //    D2D1_MATRIX_3X2_F matrix2
        //    )
        //{
        //            return matrix1 * matrix2;
        //        //(*D2D1::Matrix3x2F::ReinterpretBaseType(matrix1)) *
        //        //(*D2D1::Matrix3x2F::ReinterpretBaseType(matrix2));
        //}
    }

    [ComImport]
    [Guid("6f15aaf2-d208-4e89-9ab4-489535d34f9c")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11Texture2D : ID3D11Resource
    {
        #region ID3D11Resource
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
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
    [Guid("dc8e63f3-d12b-4952-b47b-5e45026a862d")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11Resource : ID3D11DeviceChild
    {
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
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
    [Guid("c0bfa96c-e089-44fb-8eaf-26f8796190da")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11DeviceContext : ID3D11DeviceChild
    {
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion

        void VSSetConstantBuffers(uint StartSlot, uint NumBuffers, ID3D11Buffer ppConstantBuffers);
        void PSSetShaderResources(uint StartSlot, uint NumViews, ID3D11ShaderResourceView ppShaderResourceViews);
        void PSSetShader(ID3D11PixelShader pPixelShader, ID3D11ClassInstance ppClassInstances, uint NumClassInstances);
        void PSSetSamplers(uint StartSlot, uint NumSamplers, ID3D11SamplerState ppSamplers);
        void VSSetShader(ID3D11VertexShader pVertexShader, ID3D11ClassInstance ppClassInstances, uint NumClassInstances);
        void DrawIndexed(uint IndexCount, uint StartIndexLocation, int BaseVertexLocation);
        void Draw(uint VertexCount, uint StartVertexLocation);
        HRESULT Map(ID3D11Resource pResource, uint Subresource, D3D11_MAP MapType, uint MapFlags, out D3D11_MAPPED_SUBRESOURCE pMappedResource);
        void Unmap(ID3D11Resource pResource, uint Subresource);
        void PSSetConstantBuffers(uint StartSlot, uint NumBuffers, ID3D11Buffer ppConstantBuffers);
        void IASetInputLayout(ID3D11InputLayout pInputLayout);
        void IASetVertexBuffers(uint StartSlot, uint NumBuffers, ID3D11Buffer ppVertexBuffers, uint pStrides, uint pOffsets);
        void IASetIndexBuffer(ID3D11Buffer pIndexBuffer, DXGI_FORMAT Format, uint Offset);
        void DrawIndexedInstanced(uint IndexCountPerInstance, uint InstanceCount, uint StartIndexLocation, int BaseVertexLocation, uint StartInstanceLocation);
        void DrawInstanced(uint VertexCountPerInstance, uint InstanceCount, uint StartVertexLocation, uint StartInstanceLocation);
        void GSSetConstantBuffers(uint StartSlot, uint NumBuffers, ID3D11Buffer ppConstantBuffers);
        void GSSetShader(ID3D11GeometryShader pShader, ID3D11ClassInstance ppClassInstances, uint NumClassInstances);
        //void IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY Topology);
        void IASetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY Topology);
        void VSSetShaderResources(uint StartSlot, uint NumViews, ID3D11ShaderResourceView ppShaderResourceViews);
        void VSSetSamplers(uint StartSlot, uint NumSamplers, ID3D11SamplerState ppSamplers);
        void Begin(ID3D11Asynchronous pAsync);
        void End(ID3D11Asynchronous pAsync);
        HRESULT GetData(ID3D11Asynchronous pAsync, out IntPtr pData, uint DataSize, uint GetDataFlags);
        void SetPredication(ID3D11Predicate pPredicate, bool PredicateValue);
        void GSSetShaderResources(uint StartSlot, uint NumViews, ID3D11ShaderResourceView ppShaderResourceViews);
        void GSSetSamplers(uint StartSlot, uint NumSamplers, ID3D11SamplerState ppSamplers);
        void OMSetRenderTargets(uint NumViews, ID3D11RenderTargetView ppRenderTargetViews, ID3D11DepthStencilView pDepthStencilView);
        void OMSetRenderTargetsAndUnorderedAccessViews(uint NumRTVs, ID3D11RenderTargetView ppRenderTargetViews, ID3D11DepthStencilView pDepthStencilView,
            uint UAVStartSlot, uint NumUAVs, ID3D11UnorderedAccessView ppUnorderedAccessViews, uint pUAVInitialCounts);
        //   _In_opt_  const FLOAT BlendFactor[ 4 ],
        void OMSetBlendState(ID3D11BlendState pBlendState, float[] BlendFactor, uint SampleMask);
        void OMSetDepthStencilState(ID3D11DepthStencilState pDepthStencilState, uint StencilRef);
        void SOSetTargets(uint NumBuffers, ID3D11Buffer ppSOTargets, uint pOffsets);
        void DrawAuto();
        void DrawIndexedInstancedIndirect(ID3D11Buffer pBufferForArgs, uint AlignedByteOffsetForArgs);
        void DrawInstancedIndirect(ID3D11Buffer pBufferForArgs, uint AlignedByteOffsetForArgs);
        void Dispatch(uint ThreadGroupCountX, uint ThreadGroupCountY, uint ThreadGroupCountZ);
        void DispatchIndirect(ID3D11Buffer pBufferForArgs, uint AlignedByteOffsetForArgs);
        void RSSetState(ID3D11RasterizerState pRasterizerState);
        void RSSetViewports(uint NumViewports, D3D11_VIEWPORT pViewports);
        ////void RSSetScissorRects(uint NumRects, D3D11_RECT pRects);
        void RSSetScissorRects(uint NumRects, RECT pRects);
        //void CopySubresourceRegion(ID3D11Resource pDstResource, uint DstSubresource, uint DstX, uint DstY, uint DstZ, ID3D11Resource pSrcResource, uint SrcSubresource, D3D11_BOX pSrcBox);
        void CopySubresourceRegion(ID3D11Resource pDstResource, uint DstSubresource, uint DstX, uint DstY, uint DstZ, ID3D11Resource pSrcResource, uint SrcSubresource, IntPtr pSrcBox);
        void CopyResource(ID3D11Resource pDstResource, ID3D11Resource pSrcResource);
        void UpdateSubresource(ID3D11Resource pDstResource, uint DstSubresource, D3D11_BOX pDstBox, IntPtr pSrcData, uint SrcRowPitch, uint SrcDepthPitch);
        void CopyStructureCount(ID3D11Buffer pDstBuffer, uint DstAlignedByteOffset, ID3D11UnorderedAccessView pSrcView);
        // float ColorRGBA[ 4 ]
        void ClearRenderTargetView(ID3D11RenderTargetView pRenderTargetView, float[] ColorRGBA);
        // uint Values[ 4 ]
        void ClearUnorderedAccessViewuint(ID3D11UnorderedAccessView pUnorderedAccessView, uint[] Values);
        // float Values[ 4 ]
        void ClearUnorderedAccessViewfloat(ID3D11UnorderedAccessView pUnorderedAccessView, float[] Values);
        void ClearDepthStencilView(ID3D11DepthStencilView pDepthStencilView, uint ClearFlags, float Depth, byte Stencil);
        void GenerateMips(ID3D11ShaderResourceView pShaderResourceView);
        void SetResourceMinLOD(ID3D11Resource pResource, float MinLOD);
        float GetResourceMinLOD(ID3D11Resource pResource);
        void ResolveSubresource(ID3D11Resource pDstResource, uint DstSubresource, ID3D11Resource pSrcResource, uint SrcSubresource, DXGI_FORMAT Format);
        void ExecuteCommandList(ID3D11CommandList pCommandList, bool RestoreContextState);
        void HSSetShaderResources(uint StartSlot, uint NumViews,ID3D11ShaderResourceView ppShaderResourceViews);
        void HSSetShader(ID3D11HullShader pHullShader, ID3D11ClassInstance ppClassInstances, uint NumClassInstances);
        void HSSetSamplers(uint StartSlot, uint NumSamplers, ID3D11SamplerState ppSamplers);
        void HSSetConstantBuffers(uint StartSlot, uint NumBuffers, ID3D11Buffer ppConstantBuffers);
        void DSSetShaderResources(uint StartSlot, uint NumViews, ID3D11ShaderResourceView ppShaderResourceViews);
        void DSSetShader(ID3D11DomainShader pDomainShader, ID3D11ClassInstance ppClassInstances, uint NumClassInstances);
        void DSSetSamplers(uint StartSlot, uint NumSamplers, ID3D11SamplerState ppSamplers);
        void DSSetConstantBuffers(uint StartSlot, uint NumBuffers, ID3D11Buffer ppConstantBuffers);
        void CSSetShaderResources(uint StartSlot, uint NumViews, ID3D11ShaderResourceView ppShaderResourceViews);
        void CSSetUnorderedAccessViews(uint StartSlot, uint NumUAVs, ID3D11UnorderedAccessView ppUnorderedAccessViews, uint pUAVInitialCounts);
        void CSSetShader(ID3D11ComputeShader pComputeShader, ID3D11ClassInstance ppClassInstances, uint NumClassInstances);
        void CSSetSamplers(uint StartSlot, uint NumSamplers, ID3D11SamplerState ppSamplers);
        void CSSetConstantBuffers(uint StartSlot, uint NumBuffers, ID3D11Buffer ppConstantBuffers);
        void VSGetConstantBuffers(uint StartSlot, uint NumBuffers, out ID3D11Buffer ppConstantBuffers);
        void PSGetShaderResources(uint StartSlot, uint NumViews, out ID3D11ShaderResourceView ppShaderResourceViews);
        void PSGetShader(out ID3D11PixelShader ppPixelShader, out ID3D11ClassInstance ppClassInstances, ref uint pNumClassInstances);
        void PSGetSamplers(uint StartSlot, uint NumSamplers, out ID3D11SamplerState ppSamplers);
        void VSGetShader(out ID3D11VertexShader ppVertexShader, out ID3D11ClassInstance ppClassInstances, ref uint pNumClassInstances);
        void PSGetConstantBuffers(uint StartSlot, uint NumBuffers, out ID3D11Buffer ppConstantBuffers);
        void IAGetInputLayout(out ID3D11InputLayout ppInputLayout);
        void IAGetVertexBuffers(uint StartSlot, uint NumBuffers, out ID3D11Buffer ppVertexBuffers, out uint pStrides, out uint pOffsets);
        void IAGetIndexBuffer(out ID3D11Buffer pIndexBuffer, out DXGI_FORMAT Format, out uint Offset);
        void GSGetConstantBuffers(uint StartSlot, uint NumBuffers, out ID3D11Buffer ppConstantBuffers);
        void GSGetShader(out ID3D11GeometryShader ppGeometryShader, out ID3D11ClassInstance ppClassInstances, ref uint pNumClassInstances);
        //void IAGetPrimitiveTopology(out D3D11_PRIMITIVE_TOPOLOGY pTopology);
        void IAGetPrimitiveTopology(out D3D_PRIMITIVE_TOPOLOGY pTopology);
        void VSGetShaderResources(uint StartSlot, uint NumViews, out ID3D11ShaderResourceView ppShaderResourceViews);
        void VSGetSamplers(uint StartSlot, uint NumSamplers, out ID3D11SamplerState ppSamplers);
        void GetPredication(out ID3D11Predicate ppPredicate, out bool pPredicateValue);
        void GSGetShaderResources(uint StartSlot, uint NumViews, out ID3D11ShaderResourceView ppShaderResourceViews);
        void GSGetSamplers(uint StartSlot, uint NumSamplers, out ID3D11SamplerState ppSamplers);
        void OMGetRenderTargets(uint NumViews, out ID3D11RenderTargetView ppRenderTargetViews, out ID3D11DepthStencilView ppDepthStencilView);
        void OMGetRenderTargetsAndUnorderedAccessViews(uint NumRTVs, out ID3D11RenderTargetView ppRenderTargetViews, out ID3D11DepthStencilView ppDepthStencilView, uint UAVStartSlot, uint NumUAVs, out ID3D11UnorderedAccessView ppUnorderedAccessViews);
        //  float BlendFactor[ 4 ]
        void OMGetBlendState(out ID3D11BlendState ppBlendState, out float[] BlendFactor, out uint pSampleMask);
        void OMGetDepthStencilState(out ID3D11DepthStencilState ppDepthStencilState, out uint pStencilRef);
        void SOGetTargets(uint NumBuffers, out ID3D11Buffer ppSOTargets);
        void RSGetState(out ID3D11RasterizerState ppRasterizerState);
        void RSGetViewports(ref uint pNumViewports, out D3D11_VIEWPORT pViewports);
        //void RSGetScissorRects(ref uint pNumRects, out D3D11_RECT pRects);
        void RSGetScissorRects(ref uint pNumRects, out RECT pRects);
        void HSGetShaderResources(uint StartSlot, uint NumViews, out ID3D11ShaderResourceView ppShaderResourceViews);
        void HSGetShader(out ID3D11HullShader ppHullShader, out ID3D11ClassInstance ppClassInstances, ref uint pNumClassInstances);
        void HSGetSamplers(uint StartSlot, uint NumSamplers, out ID3D11SamplerState ppSamplers);
        void HSGetConstantBuffers(uint StartSlot, uint NumBuffers, out ID3D11Buffer ppConstantBuffers);
        void DSGetShaderResources(uint StartSlot, uint NumViews, out ID3D11ShaderResourceView ppShaderResourceViews);
        void DSGetShader(out ID3D11DomainShader ppDomainShader, out ID3D11ClassInstance ppClassInstances, ref uint pNumClassInstances);
        void DSGetSamplers(uint StartSlot, uint NumSamplers, out ID3D11SamplerState ppSamplers);
        void DSGetConstantBuffers(uint StartSlot, uint NumBuffers, out ID3D11Buffer ppConstantBuffers);
        void CSGetShaderResources(uint StartSlot, uint NumViews, out ID3D11ShaderResourceView ppShaderResourceViews);
        void CSGetUnorderedAccessViews(uint StartSlot, uint NumUAVs, out ID3D11UnorderedAccessView ppUnorderedAccessViews);
        void CSGetShader(out ID3D11ComputeShader ppComputeShader, out ID3D11ClassInstance ppClassInstances, ref uint pNumClassInstances);
        void CSGetSamplers(uint StartSlot, uint NumSamplers, out ID3D11SamplerState ppSamplers);
        void CSGetConstantBuffers(uint StartSlot, uint NumBuffers, out ID3D11Buffer ppConstantBuffers);
        void ClearState();
        void Flush();
        D3D11_DEVICE_CONTEXT_TYPE GetType();
        uint GetContextFlags();
        HRESULT FinishCommandList(bool RestoreDeferredContextState, out ID3D11CommandList ppCommandList);
    }

    [ComImport]
    [Guid("48570b85-d1ee-4fcd-a250-eb350722b037")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11Buffer : ID3D11Resource
    {

        #region ID3D11Resource
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion

        new void GetType(out D3D11_RESOURCE_DIMENSION pResourceDimension);
        new void SetEvictionPriority(uint EvictionPriority);
        new uint GetEvictionPriority();
        #endregion

        void GetDesc(out D3D11_BUFFER_DESC pDesc);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11_BUFFER_DESC
    {
        public uint ByteWidth;
        public D3D11_USAGE Usage;
        public uint BindFlags;
        public uint CPUAccessFlags;
        public uint MiscFlags;
        public uint StructureByteStride;
    }

    [ComImport]
    [Guid("839d1216-bb2e-412b-b7f4-a9dbebe08ed1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11View : ID3D11DeviceChild
    {
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion

        void GetResource(out ID3D11Resource ppResource);
    }

    [ComImport]
    [Guid("b0e06fe0-8192-4e1a-b1ca-36d7414710b2")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11ShaderResourceView : ID3D11View
    {
        #region ID3D11View
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion

        new void GetResource(out ID3D11Resource ppResource);
        #endregion

        void GetDesc(out D3D11_SHADER_RESOURCE_VIEW_DESC pDesc);
    }

    public enum D3D_SRV_DIMENSION
    {
        D3D_SRV_DIMENSION_UNKNOWN = 0,
        D3D_SRV_DIMENSION_BUFFER = 1,
        D3D_SRV_DIMENSION_TEXTURE1D = 2,
        D3D_SRV_DIMENSION_TEXTURE1DARRAY = 3,
        D3D_SRV_DIMENSION_TEXTURE2D = 4,
        D3D_SRV_DIMENSION_TEXTURE2DARRAY = 5,
        D3D_SRV_DIMENSION_TEXTURE2DMS = 6,
        D3D_SRV_DIMENSION_TEXTURE2DMSARRAY = 7,
        D3D_SRV_DIMENSION_TEXTURE3D = 8,
        D3D_SRV_DIMENSION_TEXTURECUBE = 9,
        D3D_SRV_DIMENSION_TEXTURECUBEARRAY = 10,
        D3D_SRV_DIMENSION_BUFFEREX = 11,
        D3D10_SRV_DIMENSION_UNKNOWN = D3D_SRV_DIMENSION_UNKNOWN,
        D3D10_SRV_DIMENSION_BUFFER = D3D_SRV_DIMENSION_BUFFER,
        D3D10_SRV_DIMENSION_TEXTURE1D = D3D_SRV_DIMENSION_TEXTURE1D,
        D3D10_SRV_DIMENSION_TEXTURE1DARRAY = D3D_SRV_DIMENSION_TEXTURE1DARRAY,
        D3D10_SRV_DIMENSION_TEXTURE2D = D3D_SRV_DIMENSION_TEXTURE2D,
        D3D10_SRV_DIMENSION_TEXTURE2DARRAY = D3D_SRV_DIMENSION_TEXTURE2DARRAY,
        D3D10_SRV_DIMENSION_TEXTURE2DMS = D3D_SRV_DIMENSION_TEXTURE2DMS,
        D3D10_SRV_DIMENSION_TEXTURE2DMSARRAY = D3D_SRV_DIMENSION_TEXTURE2DMSARRAY,
        D3D10_SRV_DIMENSION_TEXTURE3D = D3D_SRV_DIMENSION_TEXTURE3D,
        D3D10_SRV_DIMENSION_TEXTURECUBE = D3D_SRV_DIMENSION_TEXTURECUBE,
        D3D10_1_SRV_DIMENSION_UNKNOWN = D3D_SRV_DIMENSION_UNKNOWN,
        D3D10_1_SRV_DIMENSION_BUFFER = D3D_SRV_DIMENSION_BUFFER,
        D3D10_1_SRV_DIMENSION_TEXTURE1D = D3D_SRV_DIMENSION_TEXTURE1D,
        D3D10_1_SRV_DIMENSION_TEXTURE1DARRAY = D3D_SRV_DIMENSION_TEXTURE1DARRAY,
        D3D10_1_SRV_DIMENSION_TEXTURE2D = D3D_SRV_DIMENSION_TEXTURE2D,
        D3D10_1_SRV_DIMENSION_TEXTURE2DARRAY = D3D_SRV_DIMENSION_TEXTURE2DARRAY,
        D3D10_1_SRV_DIMENSION_TEXTURE2DMS = D3D_SRV_DIMENSION_TEXTURE2DMS,
        D3D10_1_SRV_DIMENSION_TEXTURE2DMSARRAY = D3D_SRV_DIMENSION_TEXTURE2DMSARRAY,
        D3D10_1_SRV_DIMENSION_TEXTURE3D = D3D_SRV_DIMENSION_TEXTURE3D,
        D3D10_1_SRV_DIMENSION_TEXTURECUBE = D3D_SRV_DIMENSION_TEXTURECUBE,
        D3D10_1_SRV_DIMENSION_TEXTURECUBEARRAY = D3D_SRV_DIMENSION_TEXTURECUBEARRAY,
        D3D11_SRV_DIMENSION_UNKNOWN = D3D_SRV_DIMENSION_UNKNOWN,
        D3D11_SRV_DIMENSION_BUFFER = D3D_SRV_DIMENSION_BUFFER,
        D3D11_SRV_DIMENSION_TEXTURE1D = D3D_SRV_DIMENSION_TEXTURE1D,
        D3D11_SRV_DIMENSION_TEXTURE1DARRAY = D3D_SRV_DIMENSION_TEXTURE1DARRAY,
        D3D11_SRV_DIMENSION_TEXTURE2D = D3D_SRV_DIMENSION_TEXTURE2D,
        D3D11_SRV_DIMENSION_TEXTURE2DARRAY = D3D_SRV_DIMENSION_TEXTURE2DARRAY,
        D3D11_SRV_DIMENSION_TEXTURE2DMS = D3D_SRV_DIMENSION_TEXTURE2DMS,
        D3D11_SRV_DIMENSION_TEXTURE2DMSARRAY = D3D_SRV_DIMENSION_TEXTURE2DMSARRAY,
        D3D11_SRV_DIMENSION_TEXTURE3D = D3D_SRV_DIMENSION_TEXTURE3D,
        D3D11_SRV_DIMENSION_TEXTURECUBE = D3D_SRV_DIMENSION_TEXTURECUBE,
        D3D11_SRV_DIMENSION_TEXTURECUBEARRAY = D3D_SRV_DIMENSION_TEXTURECUBEARRAY,
        D3D11_SRV_DIMENSION_BUFFEREX = D3D_SRV_DIMENSION_BUFFEREX
    }

    //typedef D3D_SRV_DIMENSION D3D11_SRV_DIMENSION;
    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11_SHADER_RESOURCE_VIEW_DESC
    {
        public DXGI_FORMAT Format;
        //public D3D11_SRV_DIMENSION ViewDimension;
        public D3D_SRV_DIMENSION ViewDimension;

        //    union 
        //    {
        //    D3D11_BUFFER_SRV Buffer;
        //    D3D11_TEX1D_SRV Texture1D;
        //    D3D11_TEX1D_ARRAY_SRV Texture1DArray;
        //    D3D11_TEX2D_SRV Texture2D;
        //    D3D11_TEX2D_ARRAY_SRV Texture2DArray;
        //    D3D11_TEX2DMS_SRV Texture2DMS;
        //    D3D11_TEX2DMS_ARRAY_SRV Texture2DMSArray;
        //    D3D11_TEX3D_SRV Texture3D;
        //    D3D11_TEXCUBE_SRV TextureCube;
        //    D3D11_TEXCUBE_ARRAY_SRV TextureCubeArray;
        //    D3D11_BUFFEREX_SRV BufferEx;
        //};
    }

    [ComImport]
    [Guid("ea82e40d-51dc-4f33-93d4-db7c9125ae8c")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11PixelShader : ID3D11DeviceChild
    {
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion        
    }

    [ComImport]
    [Guid("a6cd7faa-b0b7-4a2f-9436-8662a65797cb")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11ClassInstance : ID3D11DeviceChild
    {
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion

        void GetClassLinkage(out ID3D11ClassLinkage ppLinkage);
        void GetDesc(out D3D11_CLASS_INSTANCE_DESC pDesc);
        void GetInstanceName(out System.Text.StringBuilder pInstanceName, ref uint pBufferLength);
        void GetTypeName(out System.Text.StringBuilder pTypeName, ref uint pBufferLength);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11_CLASS_INSTANCE_DESC
    {
        public uint InstanceId;
        public uint InstanceIndex;
        public uint TypeId;
        public uint ConstantBuffer;
        public uint BaseConstantBufferOffset;
        public uint BaseTexture;
        public uint BaseSampler;
        public bool Created;
    }

    [ComImport]
    [Guid("ddf57cba-9543-46e4-a12b-f207a0fe7fed")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11ClassLinkage : ID3D11DeviceChild
    {
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion

        HRESULT GetClassInstance(string pClassInstanceName, uint InstanceIndex, out ID3D11ClassInstance ppInstance);
        HRESULT CreateClassInstance(string pClassTypeName, uint ConstantBufferOffset, uint ConstantVectorOffset, uint TextureOffset, uint SamplerOffset, out ID3D11ClassInstance ppInstance);
    }


    [ComImport]
    [Guid("da6fea51-564c-4487-9810-f0d0f9b4e3a5")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11SamplerState : ID3D11DeviceChild
    {
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion

        void GetDesc(out D3D11_SAMPLER_DESC pDesc);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11_SAMPLER_DESC
    {
        D3D11_FILTER Filter;
        D3D11_TEXTURE_ADDRESS_MODE AddressU;
        D3D11_TEXTURE_ADDRESS_MODE AddressV;
        D3D11_TEXTURE_ADDRESS_MODE AddressW;
        public float MipLODBias;
        public uint MaxAnisotropy;
        D3D11_COMPARISON_FUNC ComparisonFunc;
        [MarshalAs(UnmanagedType.R4, SizeConst = 4)]
        public float BorderColor;
        public float MinLOD;
        public float MaxLOD;
    }
    public enum D3D11_FILTER
    {
        D3D11_FILTER_MIN_MAG_MIP_POINT = 0,
        D3D11_FILTER_MIN_MAG_POINT_MIP_LINEAR = 0x1,
        D3D11_FILTER_MIN_POINT_MAG_LINEAR_MIP_POINT = 0x4,
        D3D11_FILTER_MIN_POINT_MAG_MIP_LINEAR = 0x5,
        D3D11_FILTER_MIN_LINEAR_MAG_MIP_POINT = 0x10,
        D3D11_FILTER_MIN_LINEAR_MAG_POINT_MIP_LINEAR = 0x11,
        D3D11_FILTER_MIN_MAG_LINEAR_MIP_POINT = 0x14,
        D3D11_FILTER_MIN_MAG_MIP_LINEAR = 0x15,
        D3D11_FILTER_ANISOTROPIC = 0x55,
        D3D11_FILTER_COMPARISON_MIN_MAG_MIP_POINT = 0x80,
        D3D11_FILTER_COMPARISON_MIN_MAG_POINT_MIP_LINEAR = 0x81,
        D3D11_FILTER_COMPARISON_MIN_POINT_MAG_LINEAR_MIP_POINT = 0x84,
        D3D11_FILTER_COMPARISON_MIN_POINT_MAG_MIP_LINEAR = 0x85,
        D3D11_FILTER_COMPARISON_MIN_LINEAR_MAG_MIP_POINT = 0x90,
        D3D11_FILTER_COMPARISON_MIN_LINEAR_MAG_POINT_MIP_LINEAR = 0x91,
        D3D11_FILTER_COMPARISON_MIN_MAG_LINEAR_MIP_POINT = 0x94,
        D3D11_FILTER_COMPARISON_MIN_MAG_MIP_LINEAR = 0x95,
        D3D11_FILTER_COMPARISON_ANISOTROPIC = 0xd5,
        D3D11_FILTER_MINIMUM_MIN_MAG_MIP_POINT = 0x100,
        D3D11_FILTER_MINIMUM_MIN_MAG_POINT_MIP_LINEAR = 0x101,
        D3D11_FILTER_MINIMUM_MIN_POINT_MAG_LINEAR_MIP_POINT = 0x104,
        D3D11_FILTER_MINIMUM_MIN_POINT_MAG_MIP_LINEAR = 0x105,
        D3D11_FILTER_MINIMUM_MIN_LINEAR_MAG_MIP_POINT = 0x110,
        D3D11_FILTER_MINIMUM_MIN_LINEAR_MAG_POINT_MIP_LINEAR = 0x111,
        D3D11_FILTER_MINIMUM_MIN_MAG_LINEAR_MIP_POINT = 0x114,
        D3D11_FILTER_MINIMUM_MIN_MAG_MIP_LINEAR = 0x115,
        D3D11_FILTER_MINIMUM_ANISOTROPIC = 0x155,
        D3D11_FILTER_MAXIMUM_MIN_MAG_MIP_POINT = 0x180,
        D3D11_FILTER_MAXIMUM_MIN_MAG_POINT_MIP_LINEAR = 0x181,
        D3D11_FILTER_MAXIMUM_MIN_POINT_MAG_LINEAR_MIP_POINT = 0x184,
        D3D11_FILTER_MAXIMUM_MIN_POINT_MAG_MIP_LINEAR = 0x185,
        D3D11_FILTER_MAXIMUM_MIN_LINEAR_MAG_MIP_POINT = 0x190,
        D3D11_FILTER_MAXIMUM_MIN_LINEAR_MAG_POINT_MIP_LINEAR = 0x191,
        D3D11_FILTER_MAXIMUM_MIN_MAG_LINEAR_MIP_POINT = 0x194,
        D3D11_FILTER_MAXIMUM_MIN_MAG_MIP_LINEAR = 0x195,
        D3D11_FILTER_MAXIMUM_ANISOTROPIC = 0x1d5
    }

    public enum D3D11_TEXTURE_ADDRESS_MODE
    {
        D3D11_TEXTURE_ADDRESS_WRAP = 1,
        D3D11_TEXTURE_ADDRESS_MIRROR = 2,
        D3D11_TEXTURE_ADDRESS_CLAMP = 3,
        D3D11_TEXTURE_ADDRESS_BORDER = 4,
        D3D11_TEXTURE_ADDRESS_MIRROR_ONCE = 5
    }

    public enum D3D11_COMPARISON_FUNC
    {
        D3D11_COMPARISON_NEVER = 1,
        D3D11_COMPARISON_LESS = 2,
        D3D11_COMPARISON_EQUAL = 3,
        D3D11_COMPARISON_LESS_EQUAL = 4,
        D3D11_COMPARISON_GREATER = 5,
        D3D11_COMPARISON_NOT_EQUAL = 6,
        D3D11_COMPARISON_GREATER_EQUAL = 7,
        D3D11_COMPARISON_ALWAYS = 8
    }

    [ComImport]
    [Guid("3b301d64-d678-4289-8897-22f8928b72f3")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11VertexShader : ID3D11DeviceChild
    {
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion        
    }

    public enum D3D11_MAP
    {
        D3D11_MAP_READ = 1,
        D3D11_MAP_WRITE = 2,
        D3D11_MAP_READ_WRITE = 3,
        D3D11_MAP_WRITE_DISCARD = 4,
        D3D11_MAP_WRITE_NO_OVERWRITE = 5
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11_MAPPED_SUBRESOURCE
    {
        public IntPtr pData;
        public uint RowPitch;
        public uint DepthPitch;
    }

    [ComImport]
    [Guid("e4819ddc-4cf0-4025-bd26-5de82a3e07b7")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11InputLayout : ID3D11DeviceChild
    {
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion        
    }

    [ComImport]
    [Guid("38325b96-effb-4022-ba02-2e795b70275c")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11GeometryShader : ID3D11DeviceChild
    {
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion        
    }

    //typedef D3D_PRIMITIVE_TOPOLOGY D3D11_PRIMITIVE_TOPOLOGY;
    public enum D3D_PRIMITIVE_TOPOLOGY
    {
        D3D_PRIMITIVE_TOPOLOGY_UNDEFINED = 0,
        D3D_PRIMITIVE_TOPOLOGY_POINTLIST = 1,
        D3D_PRIMITIVE_TOPOLOGY_LINELIST = 2,
        D3D_PRIMITIVE_TOPOLOGY_LINESTRIP = 3,
        D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST = 4,
        D3D_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP = 5,
        D3D_PRIMITIVE_TOPOLOGY_LINELIST_ADJ = 10,
        D3D_PRIMITIVE_TOPOLOGY_LINESTRIP_ADJ = 11,
        D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST_ADJ = 12,
        D3D_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP_ADJ = 13,
        D3D_PRIMITIVE_TOPOLOGY_1_CONTROL_POINT_PATCHLIST = 33,
        D3D_PRIMITIVE_TOPOLOGY_2_CONTROL_POINT_PATCHLIST = 34,
        D3D_PRIMITIVE_TOPOLOGY_3_CONTROL_POINT_PATCHLIST = 35,
        D3D_PRIMITIVE_TOPOLOGY_4_CONTROL_POINT_PATCHLIST = 36,
        D3D_PRIMITIVE_TOPOLOGY_5_CONTROL_POINT_PATCHLIST = 37,
        D3D_PRIMITIVE_TOPOLOGY_6_CONTROL_POINT_PATCHLIST = 38,
        D3D_PRIMITIVE_TOPOLOGY_7_CONTROL_POINT_PATCHLIST = 39,
        D3D_PRIMITIVE_TOPOLOGY_8_CONTROL_POINT_PATCHLIST = 40,
        D3D_PRIMITIVE_TOPOLOGY_9_CONTROL_POINT_PATCHLIST = 41,
        D3D_PRIMITIVE_TOPOLOGY_10_CONTROL_POINT_PATCHLIST = 42,
        D3D_PRIMITIVE_TOPOLOGY_11_CONTROL_POINT_PATCHLIST = 43,
        D3D_PRIMITIVE_TOPOLOGY_12_CONTROL_POINT_PATCHLIST = 44,
        D3D_PRIMITIVE_TOPOLOGY_13_CONTROL_POINT_PATCHLIST = 45,
        D3D_PRIMITIVE_TOPOLOGY_14_CONTROL_POINT_PATCHLIST = 46,
        D3D_PRIMITIVE_TOPOLOGY_15_CONTROL_POINT_PATCHLIST = 47,
        D3D_PRIMITIVE_TOPOLOGY_16_CONTROL_POINT_PATCHLIST = 48,
        D3D_PRIMITIVE_TOPOLOGY_17_CONTROL_POINT_PATCHLIST = 49,
        D3D_PRIMITIVE_TOPOLOGY_18_CONTROL_POINT_PATCHLIST = 50,
        D3D_PRIMITIVE_TOPOLOGY_19_CONTROL_POINT_PATCHLIST = 51,
        D3D_PRIMITIVE_TOPOLOGY_20_CONTROL_POINT_PATCHLIST = 52,
        D3D_PRIMITIVE_TOPOLOGY_21_CONTROL_POINT_PATCHLIST = 53,
        D3D_PRIMITIVE_TOPOLOGY_22_CONTROL_POINT_PATCHLIST = 54,
        D3D_PRIMITIVE_TOPOLOGY_23_CONTROL_POINT_PATCHLIST = 55,
        D3D_PRIMITIVE_TOPOLOGY_24_CONTROL_POINT_PATCHLIST = 56,
        D3D_PRIMITIVE_TOPOLOGY_25_CONTROL_POINT_PATCHLIST = 57,
        D3D_PRIMITIVE_TOPOLOGY_26_CONTROL_POINT_PATCHLIST = 58,
        D3D_PRIMITIVE_TOPOLOGY_27_CONTROL_POINT_PATCHLIST = 59,
        D3D_PRIMITIVE_TOPOLOGY_28_CONTROL_POINT_PATCHLIST = 60,
        D3D_PRIMITIVE_TOPOLOGY_29_CONTROL_POINT_PATCHLIST = 61,
        D3D_PRIMITIVE_TOPOLOGY_30_CONTROL_POINT_PATCHLIST = 62,
        D3D_PRIMITIVE_TOPOLOGY_31_CONTROL_POINT_PATCHLIST = 63,
        D3D_PRIMITIVE_TOPOLOGY_32_CONTROL_POINT_PATCHLIST = 64,
        D3D10_PRIMITIVE_TOPOLOGY_UNDEFINED = D3D_PRIMITIVE_TOPOLOGY_UNDEFINED,
        D3D10_PRIMITIVE_TOPOLOGY_POINTLIST = D3D_PRIMITIVE_TOPOLOGY_POINTLIST,
        D3D10_PRIMITIVE_TOPOLOGY_LINELIST = D3D_PRIMITIVE_TOPOLOGY_LINELIST,
        D3D10_PRIMITIVE_TOPOLOGY_LINESTRIP = D3D_PRIMITIVE_TOPOLOGY_LINESTRIP,
        D3D10_PRIMITIVE_TOPOLOGY_TRIANGLELIST = D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST,
        D3D10_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP = D3D_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP,
        D3D10_PRIMITIVE_TOPOLOGY_LINELIST_ADJ = D3D_PRIMITIVE_TOPOLOGY_LINELIST_ADJ,
        D3D10_PRIMITIVE_TOPOLOGY_LINESTRIP_ADJ = D3D_PRIMITIVE_TOPOLOGY_LINESTRIP_ADJ,
        D3D10_PRIMITIVE_TOPOLOGY_TRIANGLELIST_ADJ = D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST_ADJ,
        D3D10_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP_ADJ = D3D_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP_ADJ,
        D3D11_PRIMITIVE_TOPOLOGY_UNDEFINED = D3D_PRIMITIVE_TOPOLOGY_UNDEFINED,
        D3D11_PRIMITIVE_TOPOLOGY_POINTLIST = D3D_PRIMITIVE_TOPOLOGY_POINTLIST,
        D3D11_PRIMITIVE_TOPOLOGY_LINELIST = D3D_PRIMITIVE_TOPOLOGY_LINELIST,
        D3D11_PRIMITIVE_TOPOLOGY_LINESTRIP = D3D_PRIMITIVE_TOPOLOGY_LINESTRIP,
        D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST = D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST,
        D3D11_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP = D3D_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP,
        D3D11_PRIMITIVE_TOPOLOGY_LINELIST_ADJ = D3D_PRIMITIVE_TOPOLOGY_LINELIST_ADJ,
        D3D11_PRIMITIVE_TOPOLOGY_LINESTRIP_ADJ = D3D_PRIMITIVE_TOPOLOGY_LINESTRIP_ADJ,
        D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST_ADJ = D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST_ADJ,
        D3D11_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP_ADJ = D3D_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP_ADJ,
        D3D11_PRIMITIVE_TOPOLOGY_1_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_1_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_2_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_2_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_3_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_3_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_4_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_4_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_5_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_5_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_6_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_6_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_7_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_7_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_8_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_8_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_9_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_9_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_10_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_10_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_11_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_11_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_12_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_12_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_13_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_13_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_14_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_14_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_15_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_15_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_16_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_16_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_17_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_17_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_18_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_18_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_19_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_19_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_20_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_20_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_21_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_21_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_22_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_22_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_23_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_23_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_24_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_24_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_25_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_25_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_26_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_26_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_27_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_27_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_28_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_28_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_29_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_29_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_30_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_30_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_31_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_31_CONTROL_POINT_PATCHLIST,
        D3D11_PRIMITIVE_TOPOLOGY_32_CONTROL_POINT_PATCHLIST = D3D_PRIMITIVE_TOPOLOGY_32_CONTROL_POINT_PATCHLIST
    }

    [ComImport]
    [Guid("4b35d0cd-1e15-4258-9c98-1b1333f6dd3b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11Asynchronous : ID3D11DeviceChild
    {
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion

        uint GetDataSize();
    }

    [ComImport]
    [Guid("d6c00747-87b7-425e-b84d-44d108560afd")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11Query : ID3D11Asynchronous
    {
        #region ID3D11Asynchronous
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion

        new uint GetDataSize();
        #endregion

        void GetDesc(out D3D11_QUERY_DESC pDesc);
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11_QUERY_DESC
    {
        public D3D11_QUERY Query;
        public uint MiscFlags;
    }

    public enum D3D11_QUERY
    {
        D3D11_QUERY_EVENT = 0,
        D3D11_QUERY_OCCLUSION = (D3D11_QUERY_EVENT + 1),
        D3D11_QUERY_TIMESTAMP = (D3D11_QUERY_OCCLUSION + 1),
        D3D11_QUERY_TIMESTAMP_DISJOINT = (D3D11_QUERY_TIMESTAMP + 1),
        D3D11_QUERY_PIPELINE_STATISTICS = (D3D11_QUERY_TIMESTAMP_DISJOINT + 1),
        D3D11_QUERY_OCCLUSION_PREDICATE = (D3D11_QUERY_PIPELINE_STATISTICS + 1),
        D3D11_QUERY_SO_STATISTICS = (D3D11_QUERY_OCCLUSION_PREDICATE + 1),
        D3D11_QUERY_SO_OVERFLOW_PREDICATE = (D3D11_QUERY_SO_STATISTICS + 1),
        D3D11_QUERY_SO_STATISTICS_STREAM0 = (D3D11_QUERY_SO_OVERFLOW_PREDICATE + 1),
        D3D11_QUERY_SO_OVERFLOW_PREDICATE_STREAM0 = (D3D11_QUERY_SO_STATISTICS_STREAM0 + 1),
        D3D11_QUERY_SO_STATISTICS_STREAM1 = (D3D11_QUERY_SO_OVERFLOW_PREDICATE_STREAM0 + 1),
        D3D11_QUERY_SO_OVERFLOW_PREDICATE_STREAM1 = (D3D11_QUERY_SO_STATISTICS_STREAM1 + 1),
        D3D11_QUERY_SO_STATISTICS_STREAM2 = (D3D11_QUERY_SO_OVERFLOW_PREDICATE_STREAM1 + 1),
        D3D11_QUERY_SO_OVERFLOW_PREDICATE_STREAM2 = (D3D11_QUERY_SO_STATISTICS_STREAM2 + 1),
        D3D11_QUERY_SO_STATISTICS_STREAM3 = (D3D11_QUERY_SO_OVERFLOW_PREDICATE_STREAM2 + 1),
        D3D11_QUERY_SO_OVERFLOW_PREDICATE_STREAM3 = (D3D11_QUERY_SO_STATISTICS_STREAM3 + 1)
    }

    [ComImport]
    [Guid("9eb576dd-9f77-4d86-81aa-8bab5fe490e2")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11Predicate : ID3D11Query
    {
        #region ID3D11Query
        #region ID3D11Asynchronous
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion
        new uint GetDataSize();
        #endregion
        new void GetDesc(out D3D11_QUERY_DESC pDesc);
        #endregion
    }

    [ComImport]
    [Guid("dfdba067-0b8d-4865-875b-d7b4516cc164")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11RenderTargetView : ID3D11View
    {
        #region ID3D11View
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion
        new void GetResource(out ID3D11Resource ppResource);
        #endregion

        void GetDesc(out D3D11_RENDER_TARGET_VIEW_DESC pDesc);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11_RENDER_TARGET_VIEW_DESC
    {
        public DXGI_FORMAT Format;
        public D3D11_RTV_DIMENSION ViewDimension;
        //    union 
        //    {
        //    D3D11_BUFFER_RTV Buffer;
        //    D3D11_TEX1D_RTV Texture1D;
        //    D3D11_TEX1D_ARRAY_RTV Texture1DArray;
        //    D3D11_TEX2D_RTV Texture2D;
        //    D3D11_TEX2D_ARRAY_RTV Texture2DArray;
        //    D3D11_TEX2DMS_RTV Texture2DMS;
        //    D3D11_TEX2DMS_ARRAY_RTV Texture2DMSArray;
        //    D3D11_TEX3D_RTV Texture3D;
        //};

    }

    public enum D3D11_RTV_DIMENSION
    {
        D3D11_RTV_DIMENSION_UNKNOWN = 0,
        D3D11_RTV_DIMENSION_BUFFER = 1,
        D3D11_RTV_DIMENSION_TEXTURE1D = 2,
        D3D11_RTV_DIMENSION_TEXTURE1DARRAY = 3,
        D3D11_RTV_DIMENSION_TEXTURE2D = 4,
        D3D11_RTV_DIMENSION_TEXTURE2DARRAY = 5,
        D3D11_RTV_DIMENSION_TEXTURE2DMS = 6,
        D3D11_RTV_DIMENSION_TEXTURE2DMSARRAY = 7,
        D3D11_RTV_DIMENSION_TEXTURE3D = 8
    }


    [ComImport]
    [Guid("9fdac92a-1876-48c3-afad-25b94f84a9b6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11DepthStencilView : ID3D11View
    {
        #region ID3D11View
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion
        new void GetResource(out ID3D11Resource ppResource);
        #endregion

        void GetDesc(out D3D11_DEPTH_STENCIL_VIEW_DESC pDesc);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11_DEPTH_STENCIL_VIEW_DESC
    {
        public DXGI_FORMAT Format;
        public D3D11_DSV_DIMENSION ViewDimension;
        public uint Flags;
        //    union 
        //    {
        //    D3D11_TEX1D_DSV Texture1D;
        //    D3D11_TEX1D_ARRAY_DSV Texture1DArray;
        //    D3D11_TEX2D_DSV Texture2D;
        //    D3D11_TEX2D_ARRAY_DSV Texture2DArray;
        //    D3D11_TEX2DMS_DSV Texture2DMS;
        //    D3D11_TEX2DMS_ARRAY_DSV Texture2DMSArray;
        //};

    }

    public enum D3D11_DSV_DIMENSION
    {
        D3D11_DSV_DIMENSION_UNKNOWN = 0,
        D3D11_DSV_DIMENSION_TEXTURE1D = 1,
        D3D11_DSV_DIMENSION_TEXTURE1DARRAY = 2,
        D3D11_DSV_DIMENSION_TEXTURE2D = 3,
        D3D11_DSV_DIMENSION_TEXTURE2DARRAY = 4,
        D3D11_DSV_DIMENSION_TEXTURE2DMS = 5,
        D3D11_DSV_DIMENSION_TEXTURE2DMSARRAY = 6
    }

    [ComImport]
    [Guid("28acf509-7f5c-48f6-8611-f316010a6380")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11UnorderedAccessView : ID3D11View
    {
        #region ID3D11View
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion
        new void GetResource(out ID3D11Resource ppResource);
        #endregion

        void GetDesc(out D3D11_UNORDERED_ACCESS_VIEW_DESC pDesc);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11_UNORDERED_ACCESS_VIEW_DESC
    {
        public DXGI_FORMAT Format;
        public D3D11_UAV_DIMENSION ViewDimension;
        //    union 
        //    {
        //    D3D11_BUFFER_UAV Buffer;
        //    D3D11_TEX1D_UAV Texture1D;
        //    D3D11_TEX1D_ARRAY_UAV Texture1DArray;
        //    D3D11_TEX2D_UAV Texture2D;
        //    D3D11_TEX2D_ARRAY_UAV Texture2DArray;
        //    D3D11_TEX3D_UAV Texture3D;
        //};

    }

    public enum D3D11_UAV_DIMENSION
    {
        D3D11_UAV_DIMENSION_UNKNOWN = 0,
        D3D11_UAV_DIMENSION_BUFFER = 1,
        D3D11_UAV_DIMENSION_TEXTURE1D = 2,
        D3D11_UAV_DIMENSION_TEXTURE1DARRAY = 3,
        D3D11_UAV_DIMENSION_TEXTURE2D = 4,
        D3D11_UAV_DIMENSION_TEXTURE2DARRAY = 5,
        D3D11_UAV_DIMENSION_TEXTURE3D = 8
    }

    [ComImport]
    [Guid("75b68faa-347d-4159-8f45-a0640f01cd9a")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11BlendState : ID3D11DeviceChild
    {
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion

        void GetDesc(out D3D11_BLEND_DESC pDesc);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11_BLEND_DESC
    {
        public bool AlphaToCoverageEnable;
        public bool IndependentBlendEnable;
        [MarshalAs(UnmanagedType.LPStruct, SizeConst = 8)]
        public D3D11_RENDER_TARGET_BLEND_DESC RenderTarget;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11_RENDER_TARGET_BLEND_DESC
    {
        public bool BlendEnable;
        public D3D11_BLEND SrcBlend;
        public D3D11_BLEND DestBlend;
        public D3D11_BLEND_OP BlendOp;
        public D3D11_BLEND SrcBlendAlpha;
        public D3D11_BLEND DestBlendAlpha;
        public D3D11_BLEND_OP BlendOpAlpha;
        public byte RenderTargetWriteMask;
    }

    public enum D3D11_BLEND
    {
        D3D11_BLEND_ZERO = 1,
        D3D11_BLEND_ONE = 2,
        D3D11_BLEND_SRC_COLOR = 3,
        D3D11_BLEND_INV_SRC_COLOR = 4,
        D3D11_BLEND_SRC_ALPHA = 5,
        D3D11_BLEND_INV_SRC_ALPHA = 6,
        D3D11_BLEND_DEST_ALPHA = 7,
        D3D11_BLEND_INV_DEST_ALPHA = 8,
        D3D11_BLEND_DEST_COLOR = 9,
        D3D11_BLEND_INV_DEST_COLOR = 10,
        D3D11_BLEND_SRC_ALPHA_SAT = 11,
        D3D11_BLEND_BLEND_FACTOR = 14,
        D3D11_BLEND_INV_BLEND_FACTOR = 15,
        D3D11_BLEND_SRC1_COLOR = 16,
        D3D11_BLEND_INV_SRC1_COLOR = 17,
        D3D11_BLEND_SRC1_ALPHA = 18,
        D3D11_BLEND_INV_SRC1_ALPHA = 19
    }

    public enum D3D11_BLEND_OP
    {
        D3D11_BLEND_OP_ADD = 1,
        D3D11_BLEND_OP_SUBTRACT = 2,
        D3D11_BLEND_OP_REV_SUBTRACT = 3,
        D3D11_BLEND_OP_MIN = 4,
        D3D11_BLEND_OP_MAX = 5
    }

    [ComImport]
    [Guid("03823efb-8d8f-4e1c-9aa2-f64bb2cbfdf1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11DepthStencilState : ID3D11DeviceChild
    {
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion

        void GetDesc(out D3D11_DEPTH_STENCIL_DESC pDesc);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11_DEPTH_STENCIL_DESC
    {
        public bool DepthEnable;
        public D3D11_DEPTH_WRITE_MASK DepthWriteMask;
        public D3D11_COMPARISON_FUNC DepthFunc;
        public bool StencilEnable;
        public byte StencilReadMask;
        public byte StencilWriteMask;
        public D3D11_DEPTH_STENCILOP_DESC FrontFace;
        public D3D11_DEPTH_STENCILOP_DESC BackFace;
    }

    public enum D3D11_DEPTH_WRITE_MASK
    {
        D3D11_DEPTH_WRITE_MASK_ZERO = 0,
        D3D11_DEPTH_WRITE_MASK_ALL = 1
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11_DEPTH_STENCILOP_DESC
    {
        public D3D11_STENCIL_OP StencilFailOp;
        public D3D11_STENCIL_OP StencilDepthFailOp;
        public D3D11_STENCIL_OP StencilPassOp;
        public D3D11_COMPARISON_FUNC StencilFunc;
    }
    public enum D3D11_STENCIL_OP
    {
        D3D11_STENCIL_OP_KEEP = 1,
        D3D11_STENCIL_OP_ZERO = 2,
        D3D11_STENCIL_OP_REPLACE = 3,
        D3D11_STENCIL_OP_INCR_SAT = 4,
        D3D11_STENCIL_OP_DECR_SAT = 5,
        D3D11_STENCIL_OP_INVERT = 6,
        D3D11_STENCIL_OP_INCR = 7,
        D3D11_STENCIL_OP_DECR = 8
    }

    [ComImport]
    [Guid("9bb4ab81-ab1a-4d8f-b506-fc04200b6ee7")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11RasterizerState : ID3D11DeviceChild
    {
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion

        void GetDesc(out D3D11_RASTERIZER_DESC pDesc);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11_RASTERIZER_DESC
    {
        public D3D11_FILL_MODE FillMode;
        public D3D11_CULL_MODE CullMode;
        public bool FrontCounterClockwise;
        public int DepthBias;
        public float DepthBiasClamp;
        public float SlopeScaledDepthBias;
        public bool DepthClipEnable;
        public bool ScissorEnable;
        public bool MultisampleEnable;
        public bool AntialiasedLineEnable;
    }

    public enum D3D11_FILL_MODE
    {
        D3D11_FILL_WIREFRAME = 2,
        D3D11_FILL_SOLID = 3
    }

    public enum D3D11_CULL_MODE
    {
        D3D11_CULL_NONE = 1,
        D3D11_CULL_FRONT = 2,
        D3D11_CULL_BACK = 3
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11_VIEWPORT
    {
        public float TopLeftX;
        public float TopLeftY;
        public float Width;
        public float Height;
        public float MinDepth;
        public float MaxDepth;
    }

    [ComImport]
    [Guid("a24bc4d1-769e-43f7-8013-98ff566c18e2")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11CommandList : ID3D11DeviceChild
    {
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion

        uint GetContextFlags();
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11_BOX
    {
        public uint left;
        public uint top;
        public uint front;
        public uint right;
        public uint bottom;
        public uint back;
    }

    [ComImport]
    [Guid("8e5c6061-628a-4c8e-8264-bbe45cb3d5dd")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11HullShader : ID3D11DeviceChild
    {
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion        
    }   

    [ComImport]
    [Guid("f582c508-0f36-490c-9977-31eece268cfa")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11DomainShader : ID3D11DeviceChild
    {
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion        
    }

    [ComImport]
    [Guid("4f5b196e-c2bd-495e-bd01-1fded38e4969")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID3D11ComputeShader : ID3D11DeviceChild
    {
        #region ID3D11DeviceChild
        //void GetDevice(out ID3D11Device ppDevice);
        new void GetDevice(out IntPtr ppDevice);
        new HRESULT GetPrivateData(ref Guid guid, ref uint pDataSize, out IntPtr pData);
        new HRESULT SetPrivateData(ref Guid guid, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface(ref Guid guid, IntPtr pData);
        #endregion        
    }

    public enum D3D11_DEVICE_CONTEXT_TYPE
    {
        D3D11_DEVICE_CONTEXT_IMMEDIATE = 0,
        D3D11_DEVICE_CONTEXT_DEFERRED = (D3D11_DEVICE_CONTEXT_IMMEDIATE + 1)
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

    //[StructLayout(LayoutKind.Explicit, Size = 24)]
    //public class DXGI_MATRIX_3X2_F
    //{
    //    [FieldOffset(0)]
    //    public float _11;
    //    [FieldOffset(4)]
    //    public float _12;
    //    [FieldOffset(8)]
    //    public float _21;
    //    [FieldOffset(12)]
    //    public float _22;
    //    [FieldOffset(16)]
    //    public float _31;
    //    [FieldOffset(20)]
    //    public float _32;
    //}

    public enum D2D1_RENDERING_PRIORITY : uint
    {
        D2D1_RENDERING_PRIORITY_NORMAL = 0,
        D2D1_RENDERING_PRIORITY_LOW = 1,
        D2D1_RENDERING_PRIORITY_FORCE_DWORD = 0xffffffff
    }


    [ComImport]
    [Guid("9eb767fd-4269-4467-b8c2-eb30cb305743")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1CommandSink1 : ID2D1CommandSink
    {
        #region ID2D1CommandSink
        new HRESULT BeginDraw();
        new HRESULT EndDraw();
        new HRESULT SetAntialiasMode(D2D1_ANTIALIAS_MODE antialiasMode);
        new HRESULT SetTags(UInt64 tag1, UInt64 tag2);
        new HRESULT SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode);
        new HRESULT SetTextRenderingParams(IDWriteRenderingParams textRenderingParams);
        new HRESULT SetTransform(D2D1_MATRIX_3X2_F transform);
        new HRESULT SetPrimitiveBlend(D2D1_PRIMITIVE_BLEND primitiveBlend);
        new HRESULT SetUnitMode(D2D1_UNIT_MODE unitMode);
        new HRESULT Clear(D2D1_COLOR_F color);
        new HRESULT DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, DWRITE_GLYPH_RUN_DESCRIPTION glyphRunDescription, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        new HRESULT DrawLine(ref D2D1_POINT_2F point0, D2D1_POINT_2F point1, ID2D1Brush brush, float strokeWidth, ID2D1StrokeStyle strokeStyle);
        new HRESULT DrawGeometry(ID2D1Geometry geometry, ID2D1Brush brush, float strokeWidth, ID2D1StrokeStyle strokeStyle);
        new HRESULT DrawRectangle(D2D1_RECT_F rect, ID2D1Brush brush, float strokeWidth, ID2D1StrokeStyle strokeStyle);
        new HRESULT DrawBitmap(ID2D1Bitmap bitmap, D2D1_RECT_F destinationRectangle, float opacity, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_RECT_F sourceRectangle, D2D1_MATRIX_4X4_F perspectiveTransform);
        new HRESULT DrawImage(ID2D1Image image, ref D2D1_POINT_2F targetOffset, D2D1_RECT_F imageRectangle, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_COMPOSITE_MODE compositeMode);
        new HRESULT DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_POINT_2F targetOffset);
        new HRESULT FillMesh(ID2D1Mesh mesh, ID2D1Brush brush);
        new HRESULT FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        new HRESULT FillGeometry(ID2D1Geometry geometry, ID2D1Brush brush, ID2D1Brush opacityBrush);
        new HRESULT FillRectangle(D2D1_RECT_F rect, ID2D1Brush brush);
        new HRESULT PushAxisAlignedClip(D2D1_RECT_F clipRect, D2D1_ANTIALIAS_MODE antialiasMode);
        new HRESULT PushLayer(D2D1_LAYER_PARAMETERS1 layerParameters1, ID2D1Layer layer);
        new HRESULT PopAxisAlignedClip();
        new HRESULT PopLayer();
        #endregion

        HRESULT SetPrimitiveBlend1(D2D1_PRIMITIVE_BLEND primitiveBlend);
    }

    [ComImport]
    [Guid("d21768e1-23a4-4823-a14b-7c3eba85d658")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Device1 : ID2D1Device
    {
        #region <ID2D1Device>
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        new HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext deviceContext);
        //HRESULT CreatePrintControl(IWICImagingFactory wicFactory, IPrintDocumentPackageTarget documentTarget, D2D1_PRINT_CONTROL_PROPERTIES printControlProperties, out ID2D1PrintControl printControl);
        new HRESULT CreatePrintControl(IWICImagingFactory wicFactory, IntPtr documentTarget, D2D1_PRINT_CONTROL_PROPERTIES printControlProperties, out ID2D1PrintControl printControl);
        new void SetMaximumTextureMemory(UInt64 maximumInBytes);
        new UInt64 GetMaximumTextureMemory();
        new void ClearResources(uint millisecondsSinceUse = 0);
        #endregion

        D2D1_RENDERING_PRIORITY GetRenderingPriority();
        void SetRenderingPriority(D2D1_RENDERING_PRIORITY renderingPriority);
        HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext1 deviceContext1);
    }

    [ComImport]
    [Guid("94f81a73-9212-4376-9c58-b16a3a0d3992")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Factory2 : ID2D1Factory1
    {
        #region <ID2D1Factory1>
        #region <ID2D1Factory>
        new HRESULT ReloadSystemMetrics();
        new HRESULT GetDesktopDpi(out float dpiX, out float dpiY);
        new HRESULT CreateRectangleGeometry(ref D2D1_RECT_F rectangle, out ID2D1RectangleGeometry rectangleGeometry);
        new HRESULT CreateRoundedRectangleGeometry(D2D1_ROUNDED_RECT roundedRectangle, out ID2D1RoundedRectangleGeometry roundedRectangleGeometry);
        new HRESULT CreateEllipseGeometry(ref D2D1_ELLIPSE ellipse, out ID2D1EllipseGeometry ellipseGeometry);
        new HRESULT CreateGeometryGroup(D2D1_FILL_MODE fillMode, ID2D1Geometry geometries, uint geometriesCount, out ID2D1GeometryGroup geometryGroup);
        new HRESULT CreateTransformedGeometry(ID2D1Geometry sourceGeometry, D2D1_MATRIX_3X2_F transform, out ID2D1TransformedGeometry transformedGeometry);
        new HRESULT CreatePathGeometry(out ID2D1PathGeometry pathGeometry);
        new ID2D1StrokeStyle CreateStrokeStyle(D2D1_STROKE_STYLE_PROPERTIES strokeStyleProperties, [MarshalAs(UnmanagedType.LPArray)] float[] dashes = null, uint dashesCount = 0);
        new HRESULT CreateDrawingStateBlock(D2D1_DRAWING_STATE_DESCRIPTION drawingStateDescription, IDWriteRenderingParams textRenderingParams, out ID2D1DrawingStateBlock drawingStateBlock);
        new HRESULT CreateWicBitmapRenderTarget(IWICBitmap target, D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, out ID2D1RenderTarget renderTarget);
        new HRESULT CreateHwndRenderTarget(ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref D2D1_HWND_RENDER_TARGET_PROPERTIES hwndRenderTargetProperties, out ID2D1HwndRenderTarget hwndRenderTarget);
        new HRESULT CreateDxgiSurfaceRenderTarget(IntPtr dxgiSurface, ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref ID2D1RenderTarget renderTarget);
        new HRESULT CreateDCRenderTarget(ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref ID2D1DCRenderTarget dcRenderTarget);
        #endregion

        new HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device d2dDevice);
        new HRESULT CreateStrokeStyle(D2D1_STROKE_STYLE_PROPERTIES1 strokeStyleProperties, float dashes, uint dashesCount, out ID2D1StrokeStyle1 strokeStyle);
        new HRESULT CreatePathGeometry(out ID2D1PathGeometry1 pathGeometry);
        new HRESULT CreateDrawingStateBlock(D2D1_DRAWING_STATE_DESCRIPTION1 drawingStateDescription, ref IDWriteRenderingParams textRenderingParams, out ID2D1DrawingStateBlock1 drawingStateBlock);
        new HRESULT CreateGdiMetafile(System.Runtime.InteropServices.ComTypes.IStream metafileStream, out ID2D1GdiMetafile metafile);
        #endregion

        HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device1 d2dDevice1);
    }

    [ComImport]
    [Guid("0869759f-4f00-413f-b03e-2bda45404d0f")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Factory3 : ID2D1Factory2
    {
        #region <ID2D1Factory2>
        #region <ID2D1Factory1>
        #region <ID2D1Factory>
        new HRESULT ReloadSystemMetrics();
        new HRESULT GetDesktopDpi(out float dpiX, out float dpiY);
        new HRESULT CreateRectangleGeometry(ref D2D1_RECT_F rectangle, out ID2D1RectangleGeometry rectangleGeometry);
        new HRESULT CreateRoundedRectangleGeometry(D2D1_ROUNDED_RECT roundedRectangle, out ID2D1RoundedRectangleGeometry roundedRectangleGeometry);
        new HRESULT CreateEllipseGeometry(ref D2D1_ELLIPSE ellipse, out ID2D1EllipseGeometry ellipseGeometry);
        new HRESULT CreateGeometryGroup(D2D1_FILL_MODE fillMode, ID2D1Geometry geometries, uint geometriesCount, out ID2D1GeometryGroup geometryGroup);
        new HRESULT CreateTransformedGeometry(ID2D1Geometry sourceGeometry, D2D1_MATRIX_3X2_F transform, out ID2D1TransformedGeometry transformedGeometry);
        new HRESULT CreatePathGeometry(out ID2D1PathGeometry pathGeometry);
        new ID2D1StrokeStyle CreateStrokeStyle(D2D1_STROKE_STYLE_PROPERTIES strokeStyleProperties, [MarshalAs(UnmanagedType.LPArray)] float[] dashes = null, uint dashesCount = 0);
        new HRESULT CreateDrawingStateBlock(D2D1_DRAWING_STATE_DESCRIPTION drawingStateDescription, IDWriteRenderingParams textRenderingParams, out ID2D1DrawingStateBlock drawingStateBlock);
        new HRESULT CreateWicBitmapRenderTarget(IWICBitmap target, D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, out ID2D1RenderTarget renderTarget);
        new HRESULT CreateHwndRenderTarget(ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref D2D1_HWND_RENDER_TARGET_PROPERTIES hwndRenderTargetProperties, out ID2D1HwndRenderTarget hwndRenderTarget);
        new HRESULT CreateDxgiSurfaceRenderTarget(IntPtr dxgiSurface, ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref ID2D1RenderTarget renderTarget);
        new HRESULT CreateDCRenderTarget(ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref ID2D1DCRenderTarget dcRenderTarget);
        #endregion

        new HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device d2dDevice);
        new HRESULT CreateStrokeStyle(D2D1_STROKE_STYLE_PROPERTIES1 strokeStyleProperties, float dashes, uint dashesCount, out ID2D1StrokeStyle1 strokeStyle);
        new HRESULT CreatePathGeometry(out ID2D1PathGeometry1 pathGeometry);
        new HRESULT CreateDrawingStateBlock(D2D1_DRAWING_STATE_DESCRIPTION1 drawingStateDescription, ref IDWriteRenderingParams textRenderingParams, out ID2D1DrawingStateBlock1 drawingStateBlock);
        new HRESULT CreateGdiMetafile(System.Runtime.InteropServices.ComTypes.IStream metafileStream, out ID2D1GdiMetafile metafile);
        #endregion

        new HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device1 d2dDevice1);
        #endregion

        HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device2 d2dDevice2);
    }

    [ComImport]
    [Guid("bd4ec2d2-0662-4bee-ba8e-6f29f032e096")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Factory4 : ID2D1Factory3
    {
        #region <ID2D1Factory3>
        #region <ID2D1Factory2>
        #region <ID2D1Factory1>
        #region <ID2D1Factory>
        new HRESULT ReloadSystemMetrics();
        new HRESULT GetDesktopDpi(out float dpiX, out float dpiY);
        new HRESULT CreateRectangleGeometry(ref D2D1_RECT_F rectangle, out ID2D1RectangleGeometry rectangleGeometry);
        new HRESULT CreateRoundedRectangleGeometry(D2D1_ROUNDED_RECT roundedRectangle, out ID2D1RoundedRectangleGeometry roundedRectangleGeometry);
        new HRESULT CreateEllipseGeometry(ref D2D1_ELLIPSE ellipse, out ID2D1EllipseGeometry ellipseGeometry);
        new HRESULT CreateGeometryGroup(D2D1_FILL_MODE fillMode, ID2D1Geometry geometries, uint geometriesCount, out ID2D1GeometryGroup geometryGroup);
        new HRESULT CreateTransformedGeometry(ID2D1Geometry sourceGeometry, D2D1_MATRIX_3X2_F transform, out ID2D1TransformedGeometry transformedGeometry);
        new HRESULT CreatePathGeometry(out ID2D1PathGeometry pathGeometry);
        new ID2D1StrokeStyle CreateStrokeStyle(D2D1_STROKE_STYLE_PROPERTIES strokeStyleProperties, [MarshalAs(UnmanagedType.LPArray)] float[] dashes = null, uint dashesCount = 0);
        new HRESULT CreateDrawingStateBlock(D2D1_DRAWING_STATE_DESCRIPTION drawingStateDescription, IDWriteRenderingParams textRenderingParams, out ID2D1DrawingStateBlock drawingStateBlock);
        new HRESULT CreateWicBitmapRenderTarget(IWICBitmap target, D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, out ID2D1RenderTarget renderTarget);
        new HRESULT CreateHwndRenderTarget(ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref D2D1_HWND_RENDER_TARGET_PROPERTIES hwndRenderTargetProperties, out ID2D1HwndRenderTarget hwndRenderTarget);
        new HRESULT CreateDxgiSurfaceRenderTarget(IntPtr dxgiSurface, ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref ID2D1RenderTarget renderTarget);
        new HRESULT CreateDCRenderTarget(ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref ID2D1DCRenderTarget dcRenderTarget);
        #endregion

        new HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device d2dDevice);
        new HRESULT CreateStrokeStyle(D2D1_STROKE_STYLE_PROPERTIES1 strokeStyleProperties, float dashes, uint dashesCount, out ID2D1StrokeStyle1 strokeStyle);
        new HRESULT CreatePathGeometry(out ID2D1PathGeometry1 pathGeometry);
        new HRESULT CreateDrawingStateBlock(D2D1_DRAWING_STATE_DESCRIPTION1 drawingStateDescription, ref IDWriteRenderingParams textRenderingParams, out ID2D1DrawingStateBlock1 drawingStateBlock);
        new HRESULT CreateGdiMetafile(System.Runtime.InteropServices.ComTypes.IStream metafileStream, out ID2D1GdiMetafile metafile);
        #endregion

        new HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device1 d2dDevice1);
        #endregion

        new HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device2 d2dDevice2);
        #endregion

        HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device3 d2dDevice3);
    }

    [ComImport]
    [Guid("c4349994-838e-4b0f-8cab-44997d9eeacc")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Factory5 : ID2D1Factory4
    {
        #region <ID2D1Factory4>
        #region <ID2D1Factory3>
        #region <ID2D1Factory2>
        #region <ID2D1Factory1>
        #region <ID2D1Factory>
        new HRESULT ReloadSystemMetrics();
        new HRESULT GetDesktopDpi(out float dpiX, out float dpiY);
        new HRESULT CreateRectangleGeometry(ref D2D1_RECT_F rectangle, out ID2D1RectangleGeometry rectangleGeometry);
        new HRESULT CreateRoundedRectangleGeometry(D2D1_ROUNDED_RECT roundedRectangle, out ID2D1RoundedRectangleGeometry roundedRectangleGeometry);
        new HRESULT CreateEllipseGeometry(ref D2D1_ELLIPSE ellipse, out ID2D1EllipseGeometry ellipseGeometry);
        new HRESULT CreateGeometryGroup(D2D1_FILL_MODE fillMode, ID2D1Geometry geometries, uint geometriesCount, out ID2D1GeometryGroup geometryGroup);
        new HRESULT CreateTransformedGeometry(ID2D1Geometry sourceGeometry, D2D1_MATRIX_3X2_F transform, out ID2D1TransformedGeometry transformedGeometry);
        new HRESULT CreatePathGeometry(out ID2D1PathGeometry pathGeometry);
        new ID2D1StrokeStyle CreateStrokeStyle(D2D1_STROKE_STYLE_PROPERTIES strokeStyleProperties, [MarshalAs(UnmanagedType.LPArray)] float[] dashes = null, uint dashesCount = 0);
        new HRESULT CreateDrawingStateBlock(D2D1_DRAWING_STATE_DESCRIPTION drawingStateDescription, IDWriteRenderingParams textRenderingParams, out ID2D1DrawingStateBlock drawingStateBlock);
        new HRESULT CreateWicBitmapRenderTarget(IWICBitmap target, D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, out ID2D1RenderTarget renderTarget);
        new HRESULT CreateHwndRenderTarget(ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref D2D1_HWND_RENDER_TARGET_PROPERTIES hwndRenderTargetProperties, out ID2D1HwndRenderTarget hwndRenderTarget);
        new HRESULT CreateDxgiSurfaceRenderTarget(IntPtr dxgiSurface, ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref ID2D1RenderTarget renderTarget);
        new HRESULT CreateDCRenderTarget(ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref ID2D1DCRenderTarget dcRenderTarget);
        #endregion

        new HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device d2dDevice);
        new HRESULT CreateStrokeStyle(D2D1_STROKE_STYLE_PROPERTIES1 strokeStyleProperties, float dashes, uint dashesCount, out ID2D1StrokeStyle1 strokeStyle);
        new HRESULT CreatePathGeometry(out ID2D1PathGeometry1 pathGeometry);
        new HRESULT CreateDrawingStateBlock(D2D1_DRAWING_STATE_DESCRIPTION1 drawingStateDescription, ref IDWriteRenderingParams textRenderingParams, out ID2D1DrawingStateBlock1 drawingStateBlock);
        new HRESULT CreateGdiMetafile(System.Runtime.InteropServices.ComTypes.IStream metafileStream, out ID2D1GdiMetafile metafile);
        #endregion

        new HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device1 d2dDevice1);
        #endregion

        new HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device2 d2dDevice2);
        #endregion

        new HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device3 d2dDevice3);
        #endregion

        HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device4 d2dDevice4);
    }

    [ComImport]
    [Guid("f9976f46-f642-44c1-97ca-da32ea2a2635")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Factory6 : ID2D1Factory5
    {
        #region <ID2D1Factory5>
        #region <ID2D1Factory4>
        #region <ID2D1Factory3>
        #region <ID2D1Factory2>
        #region <ID2D1Factory1>
        #region <ID2D1Factory>
        new HRESULT ReloadSystemMetrics();
        new HRESULT GetDesktopDpi(out float dpiX, out float dpiY);
        new HRESULT CreateRectangleGeometry(ref D2D1_RECT_F rectangle, out ID2D1RectangleGeometry rectangleGeometry);
        new HRESULT CreateRoundedRectangleGeometry(D2D1_ROUNDED_RECT roundedRectangle, out ID2D1RoundedRectangleGeometry roundedRectangleGeometry);
        new HRESULT CreateEllipseGeometry(ref D2D1_ELLIPSE ellipse, out ID2D1EllipseGeometry ellipseGeometry);
        new HRESULT CreateGeometryGroup(D2D1_FILL_MODE fillMode, ID2D1Geometry geometries, uint geometriesCount, out ID2D1GeometryGroup geometryGroup);
        new HRESULT CreateTransformedGeometry(ID2D1Geometry sourceGeometry, D2D1_MATRIX_3X2_F transform, out ID2D1TransformedGeometry transformedGeometry);
        new HRESULT CreatePathGeometry(out ID2D1PathGeometry pathGeometry);
        new ID2D1StrokeStyle CreateStrokeStyle(D2D1_STROKE_STYLE_PROPERTIES strokeStyleProperties, [MarshalAs(UnmanagedType.LPArray)] float[] dashes = null, uint dashesCount = 0);
        new HRESULT CreateDrawingStateBlock(D2D1_DRAWING_STATE_DESCRIPTION drawingStateDescription, IDWriteRenderingParams textRenderingParams, out ID2D1DrawingStateBlock drawingStateBlock);
        new HRESULT CreateWicBitmapRenderTarget(IWICBitmap target, D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, out ID2D1RenderTarget renderTarget);
        new HRESULT CreateHwndRenderTarget(ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref D2D1_HWND_RENDER_TARGET_PROPERTIES hwndRenderTargetProperties, out ID2D1HwndRenderTarget hwndRenderTarget);
        new HRESULT CreateDxgiSurfaceRenderTarget(IntPtr dxgiSurface, ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref ID2D1RenderTarget renderTarget);
        new HRESULT CreateDCRenderTarget(ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref ID2D1DCRenderTarget dcRenderTarget);
        #endregion

        new HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device d2dDevice);
        new HRESULT CreateStrokeStyle(D2D1_STROKE_STYLE_PROPERTIES1 strokeStyleProperties, float dashes, uint dashesCount, out ID2D1StrokeStyle1 strokeStyle);
        new HRESULT CreatePathGeometry(out ID2D1PathGeometry1 pathGeometry);
        new HRESULT CreateDrawingStateBlock(D2D1_DRAWING_STATE_DESCRIPTION1 drawingStateDescription, ref IDWriteRenderingParams textRenderingParams, out ID2D1DrawingStateBlock1 drawingStateBlock);
        new HRESULT CreateGdiMetafile(System.Runtime.InteropServices.ComTypes.IStream metafileStream, out ID2D1GdiMetafile metafile);
        #endregion

        new HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device1 d2dDevice1);
        #endregion

        new HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device2 d2dDevice2);
        #endregion

        new HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device3 d2dDevice3);
        #endregion

        new HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device4 d2dDevice4);
        #endregion

        HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device5 d2dDevice5);
    }

    [ComImport]
    [Guid("bdc2bdd3-b96c-4de6-bdf7-99d4745454de")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Factory7 : ID2D1Factory6
    {
        #region <ID2D1Factory6>
        #region <ID2D1Factory5>
        #region <ID2D1Factory4>
        #region <ID2D1Factory3>
        #region <ID2D1Factory2>
        #region <ID2D1Factory1>
        #region <ID2D1Factory>
        new HRESULT ReloadSystemMetrics();
        new HRESULT GetDesktopDpi(out float dpiX, out float dpiY);
        new HRESULT CreateRectangleGeometry(ref D2D1_RECT_F rectangle, out ID2D1RectangleGeometry rectangleGeometry);
        new HRESULT CreateRoundedRectangleGeometry(D2D1_ROUNDED_RECT roundedRectangle, out ID2D1RoundedRectangleGeometry roundedRectangleGeometry);
        new HRESULT CreateEllipseGeometry(ref D2D1_ELLIPSE ellipse, out ID2D1EllipseGeometry ellipseGeometry);
        new HRESULT CreateGeometryGroup(D2D1_FILL_MODE fillMode, ID2D1Geometry geometries, uint geometriesCount, out ID2D1GeometryGroup geometryGroup);
        new HRESULT CreateTransformedGeometry(ID2D1Geometry sourceGeometry, D2D1_MATRIX_3X2_F transform, out ID2D1TransformedGeometry transformedGeometry);
        new HRESULT CreatePathGeometry(out ID2D1PathGeometry pathGeometry);
        new ID2D1StrokeStyle CreateStrokeStyle(D2D1_STROKE_STYLE_PROPERTIES strokeStyleProperties, [MarshalAs(UnmanagedType.LPArray)] float[] dashes = null, uint dashesCount = 0);
        new HRESULT CreateDrawingStateBlock(D2D1_DRAWING_STATE_DESCRIPTION drawingStateDescription, IDWriteRenderingParams textRenderingParams, out ID2D1DrawingStateBlock drawingStateBlock);
        new HRESULT CreateWicBitmapRenderTarget(IWICBitmap target, D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, out ID2D1RenderTarget renderTarget);
        new HRESULT CreateHwndRenderTarget(ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref D2D1_HWND_RENDER_TARGET_PROPERTIES hwndRenderTargetProperties, out ID2D1HwndRenderTarget hwndRenderTarget);
        new HRESULT CreateDxgiSurfaceRenderTarget(IntPtr dxgiSurface, ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref ID2D1RenderTarget renderTarget);
        new HRESULT CreateDCRenderTarget(ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref ID2D1DCRenderTarget dcRenderTarget);
        #endregion

        new HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device d2dDevice);
        new HRESULT CreateStrokeStyle(D2D1_STROKE_STYLE_PROPERTIES1 strokeStyleProperties, float dashes, uint dashesCount, out ID2D1StrokeStyle1 strokeStyle);
        new HRESULT CreatePathGeometry(out ID2D1PathGeometry1 pathGeometry);
        new HRESULT CreateDrawingStateBlock(D2D1_DRAWING_STATE_DESCRIPTION1 drawingStateDescription, ref IDWriteRenderingParams textRenderingParams, out ID2D1DrawingStateBlock1 drawingStateBlock);
        new HRESULT CreateGdiMetafile(System.Runtime.InteropServices.ComTypes.IStream metafileStream, out ID2D1GdiMetafile metafile);
        #endregion

        new HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device1 d2dDevice1);
        #endregion

        new HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device2 d2dDevice2);
        #endregion

        new HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device3 d2dDevice3);
        #endregion

        new HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device4 d2dDevice4);
        #endregion

        new HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device5 d2dDevice5);
        #endregion

        HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device6 d2dDevice6);
    }

    [ComImport]
    [Guid("3bab440e-417e-47df-a2e2-bc0be6a00916")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1CommandSink2 : ID2D1CommandSink1
    {
        #region ID2D1CommandSink1
        #region ID2D1CommandSink
        new HRESULT BeginDraw();
        new HRESULT EndDraw();
        new HRESULT SetAntialiasMode(D2D1_ANTIALIAS_MODE antialiasMode);
        new HRESULT SetTags(UInt64 tag1, UInt64 tag2);
        new HRESULT SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode);
        new HRESULT SetTextRenderingParams(IDWriteRenderingParams textRenderingParams);
        new HRESULT SetTransform(D2D1_MATRIX_3X2_F transform);
        new HRESULT SetPrimitiveBlend(D2D1_PRIMITIVE_BLEND primitiveBlend);
        new HRESULT SetUnitMode(D2D1_UNIT_MODE unitMode);
        new HRESULT Clear(D2D1_COLOR_F color);
        new HRESULT DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, DWRITE_GLYPH_RUN_DESCRIPTION glyphRunDescription, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        new HRESULT DrawLine(ref D2D1_POINT_2F point0, D2D1_POINT_2F point1, ID2D1Brush brush, float strokeWidth, ID2D1StrokeStyle strokeStyle);
        new HRESULT DrawGeometry(ID2D1Geometry geometry, ID2D1Brush brush, float strokeWidth, ID2D1StrokeStyle strokeStyle);
        new HRESULT DrawRectangle(D2D1_RECT_F rect, ID2D1Brush brush, float strokeWidth, ID2D1StrokeStyle strokeStyle);
        new HRESULT DrawBitmap(ID2D1Bitmap bitmap, D2D1_RECT_F destinationRectangle, float opacity, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_RECT_F sourceRectangle, D2D1_MATRIX_4X4_F perspectiveTransform);
        new HRESULT DrawImage(ID2D1Image image, ref D2D1_POINT_2F targetOffset, D2D1_RECT_F imageRectangle, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_COMPOSITE_MODE compositeMode);
        new HRESULT DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_POINT_2F targetOffset);
        new HRESULT FillMesh(ID2D1Mesh mesh, ID2D1Brush brush);
        new HRESULT FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        new HRESULT FillGeometry(ID2D1Geometry geometry, ID2D1Brush brush, ID2D1Brush opacityBrush);
        new HRESULT FillRectangle(D2D1_RECT_F rect, ID2D1Brush brush);
        new HRESULT PushAxisAlignedClip(D2D1_RECT_F clipRect, D2D1_ANTIALIAS_MODE antialiasMode);
        new HRESULT PushLayer(D2D1_LAYER_PARAMETERS1 layerParameters1, ID2D1Layer layer);
        new HRESULT PopAxisAlignedClip();
        new HRESULT PopLayer();
        #endregion

        new HRESULT SetPrimitiveBlend1(D2D1_PRIMITIVE_BLEND primitiveBlend);
        #endregion

        HRESULT DrawInk(ID2D1Ink ink,ID2D1Brush brush, ID2D1InkStyle inkStyle);
        HRESULT DrawGradientMesh(ID2D1GradientMesh gradientMesh); 
        HRESULT DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_RECT_F destinationRectangle, ref D2D1_RECT_F sourceRectangle);
    }

    [ComImport]
    [Guid("18079135-4cf3-4868-bc8e-06067e6d242d")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1CommandSink3 : ID2D1CommandSink2
    {
        #region ID2D1CommandSink2
        #region ID2D1CommandSink1
        #region ID2D1CommandSink
        new HRESULT BeginDraw();
        new HRESULT EndDraw();
        new HRESULT SetAntialiasMode(D2D1_ANTIALIAS_MODE antialiasMode);
        new HRESULT SetTags(UInt64 tag1, UInt64 tag2);
        new HRESULT SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode);
        new HRESULT SetTextRenderingParams(IDWriteRenderingParams textRenderingParams);
        new HRESULT SetTransform(D2D1_MATRIX_3X2_F transform);
        new HRESULT SetPrimitiveBlend(D2D1_PRIMITIVE_BLEND primitiveBlend);
        new HRESULT SetUnitMode(D2D1_UNIT_MODE unitMode);
        new HRESULT Clear(D2D1_COLOR_F color);
        new HRESULT DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, DWRITE_GLYPH_RUN_DESCRIPTION glyphRunDescription, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        new HRESULT DrawLine(ref D2D1_POINT_2F point0, D2D1_POINT_2F point1, ID2D1Brush brush, float strokeWidth, ID2D1StrokeStyle strokeStyle);
        new HRESULT DrawGeometry(ID2D1Geometry geometry, ID2D1Brush brush, float strokeWidth, ID2D1StrokeStyle strokeStyle);
        new HRESULT DrawRectangle(D2D1_RECT_F rect, ID2D1Brush brush, float strokeWidth, ID2D1StrokeStyle strokeStyle);
        new HRESULT DrawBitmap(ID2D1Bitmap bitmap, D2D1_RECT_F destinationRectangle, float opacity, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_RECT_F sourceRectangle, D2D1_MATRIX_4X4_F perspectiveTransform);
        new HRESULT DrawImage(ID2D1Image image, ref D2D1_POINT_2F targetOffset, D2D1_RECT_F imageRectangle, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_COMPOSITE_MODE compositeMode);
        new HRESULT DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_POINT_2F targetOffset);
        new HRESULT FillMesh(ID2D1Mesh mesh, ID2D1Brush brush);
        new HRESULT FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        new HRESULT FillGeometry(ID2D1Geometry geometry, ID2D1Brush brush, ID2D1Brush opacityBrush);
        new HRESULT FillRectangle(D2D1_RECT_F rect, ID2D1Brush brush);
        new HRESULT PushAxisAlignedClip(D2D1_RECT_F clipRect, D2D1_ANTIALIAS_MODE antialiasMode);
        new HRESULT PushLayer(D2D1_LAYER_PARAMETERS1 layerParameters1, ID2D1Layer layer);
        new HRESULT PopAxisAlignedClip();
        new HRESULT PopLayer();
        #endregion

        new HRESULT SetPrimitiveBlend1(D2D1_PRIMITIVE_BLEND primitiveBlend);
        #endregion

        new HRESULT DrawInk(ID2D1Ink ink, ID2D1Brush brush, ID2D1InkStyle inkStyle);
        new HRESULT DrawGradientMesh(ID2D1GradientMesh gradientMesh);
        new HRESULT DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_RECT_F destinationRectangle, ref D2D1_RECT_F sourceRectangle);
        #endregion

        HRESULT DrawSpriteBatch(ID2D1SpriteBatch spriteBatch, uint startIndex, uint spriteCount, ID2D1Bitmap bitmap, 
            D2D1_BITMAP_INTERPOLATION_MODE interpolationMode, D2D1_SPRITE_OPTIONS spriteOptions);
    }

    [ComImport]
    [Guid("c78a6519-40d6-4218-b2de-beeeb744bb3e")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1CommandSink4 : ID2D1CommandSink3
    {
        #region ID2D1CommandSink3
        #region ID2D1CommandSink2
        #region ID2D1CommandSink1
        #region ID2D1CommandSink
        new HRESULT BeginDraw();
        new HRESULT EndDraw();
        new HRESULT SetAntialiasMode(D2D1_ANTIALIAS_MODE antialiasMode);
        new HRESULT SetTags(UInt64 tag1, UInt64 tag2);
        new HRESULT SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode);
        new HRESULT SetTextRenderingParams(IDWriteRenderingParams textRenderingParams);
        new HRESULT SetTransform(D2D1_MATRIX_3X2_F transform);
        new HRESULT SetPrimitiveBlend(D2D1_PRIMITIVE_BLEND primitiveBlend);
        new HRESULT SetUnitMode(D2D1_UNIT_MODE unitMode);
        new HRESULT Clear(D2D1_COLOR_F color);
        new HRESULT DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, DWRITE_GLYPH_RUN_DESCRIPTION glyphRunDescription, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        new HRESULT DrawLine(ref D2D1_POINT_2F point0, D2D1_POINT_2F point1, ID2D1Brush brush, float strokeWidth, ID2D1StrokeStyle strokeStyle);
        new HRESULT DrawGeometry(ID2D1Geometry geometry, ID2D1Brush brush, float strokeWidth, ID2D1StrokeStyle strokeStyle);
        new HRESULT DrawRectangle(D2D1_RECT_F rect, ID2D1Brush brush, float strokeWidth, ID2D1StrokeStyle strokeStyle);
        new HRESULT DrawBitmap(ID2D1Bitmap bitmap, D2D1_RECT_F destinationRectangle, float opacity, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_RECT_F sourceRectangle, D2D1_MATRIX_4X4_F perspectiveTransform);
        new HRESULT DrawImage(ID2D1Image image, ref D2D1_POINT_2F targetOffset, D2D1_RECT_F imageRectangle, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_COMPOSITE_MODE compositeMode);
        new HRESULT DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_POINT_2F targetOffset);
        new HRESULT FillMesh(ID2D1Mesh mesh, ID2D1Brush brush);
        new HRESULT FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        new HRESULT FillGeometry(ID2D1Geometry geometry, ID2D1Brush brush, ID2D1Brush opacityBrush);
        new HRESULT FillRectangle(D2D1_RECT_F rect, ID2D1Brush brush);
        new HRESULT PushAxisAlignedClip(D2D1_RECT_F clipRect, D2D1_ANTIALIAS_MODE antialiasMode);
        new HRESULT PushLayer(D2D1_LAYER_PARAMETERS1 layerParameters1, ID2D1Layer layer);
        new HRESULT PopAxisAlignedClip();
        new HRESULT PopLayer();
        #endregion

        new HRESULT SetPrimitiveBlend1(D2D1_PRIMITIVE_BLEND primitiveBlend);
        #endregion

        new HRESULT DrawInk(ID2D1Ink ink, ID2D1Brush brush, ID2D1InkStyle inkStyle);
        new HRESULT DrawGradientMesh(ID2D1GradientMesh gradientMesh);
        new HRESULT DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_RECT_F destinationRectangle, ref D2D1_RECT_F sourceRectangle);
        #endregion

        new HRESULT DrawSpriteBatch(ID2D1SpriteBatch spriteBatch, uint startIndex, uint spriteCount, ID2D1Bitmap bitmap,
            D2D1_BITMAP_INTERPOLATION_MODE interpolationMode, D2D1_SPRITE_OPTIONS spriteOptions);
        #endregion

        HRESULT SetPrimitiveBlend2(D2D1_PRIMITIVE_BLEND primitiveBlend);
    }

    [ComImport]
    [Guid("7047dd26-b1e7-44a7-959a-8349e2144fa8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1CommandSink5 : ID2D1CommandSink4
    {
        #region ID2D1CommandSink4
        #region ID2D1CommandSink3
        #region ID2D1CommandSink2
        #region ID2D1CommandSink1
        #region ID2D1CommandSink
        new HRESULT BeginDraw();
        new HRESULT EndDraw();
        new HRESULT SetAntialiasMode(D2D1_ANTIALIAS_MODE antialiasMode);
        new HRESULT SetTags(UInt64 tag1, UInt64 tag2);
        new HRESULT SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode);
        new HRESULT SetTextRenderingParams(IDWriteRenderingParams textRenderingParams);
        new HRESULT SetTransform(D2D1_MATRIX_3X2_F transform);
        new HRESULT SetPrimitiveBlend(D2D1_PRIMITIVE_BLEND primitiveBlend);
        new HRESULT SetUnitMode(D2D1_UNIT_MODE unitMode);
        new HRESULT Clear(D2D1_COLOR_F color);
        new HRESULT DrawGlyphRun(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, DWRITE_GLYPH_RUN_DESCRIPTION glyphRunDescription, ID2D1Brush foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        new HRESULT DrawLine(ref D2D1_POINT_2F point0, D2D1_POINT_2F point1, ID2D1Brush brush, float strokeWidth, ID2D1StrokeStyle strokeStyle);
        new HRESULT DrawGeometry(ID2D1Geometry geometry, ID2D1Brush brush, float strokeWidth, ID2D1StrokeStyle strokeStyle);
        new HRESULT DrawRectangle(D2D1_RECT_F rect, ID2D1Brush brush, float strokeWidth, ID2D1StrokeStyle strokeStyle);
        new HRESULT DrawBitmap(ID2D1Bitmap bitmap, D2D1_RECT_F destinationRectangle, float opacity, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_RECT_F sourceRectangle, D2D1_MATRIX_4X4_F perspectiveTransform);
        new HRESULT DrawImage(ID2D1Image image, ref D2D1_POINT_2F targetOffset, D2D1_RECT_F imageRectangle, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_COMPOSITE_MODE compositeMode);
        new HRESULT DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_POINT_2F targetOffset);
        new HRESULT FillMesh(ID2D1Mesh mesh, ID2D1Brush brush);
        new HRESULT FillOpacityMask(ID2D1Bitmap opacityMask, ID2D1Brush brush, D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
        new HRESULT FillGeometry(ID2D1Geometry geometry, ID2D1Brush brush, ID2D1Brush opacityBrush);
        new HRESULT FillRectangle(D2D1_RECT_F rect, ID2D1Brush brush);
        new HRESULT PushAxisAlignedClip(D2D1_RECT_F clipRect, D2D1_ANTIALIAS_MODE antialiasMode);
        new HRESULT PushLayer(D2D1_LAYER_PARAMETERS1 layerParameters1, ID2D1Layer layer);
        new HRESULT PopAxisAlignedClip();
        new HRESULT PopLayer();
        #endregion

        new HRESULT SetPrimitiveBlend1(D2D1_PRIMITIVE_BLEND primitiveBlend);
        #endregion

        new HRESULT DrawInk(ID2D1Ink ink, ID2D1Brush brush, ID2D1InkStyle inkStyle);
        new HRESULT DrawGradientMesh(ID2D1GradientMesh gradientMesh);
        new HRESULT DrawGdiMetafile(ID2D1GdiMetafile gdiMetafile, ref D2D1_RECT_F destinationRectangle, ref D2D1_RECT_F sourceRectangle);
        #endregion

        new HRESULT DrawSpriteBatch(ID2D1SpriteBatch spriteBatch, uint startIndex, uint spriteCount, ID2D1Bitmap bitmap,
            D2D1_BITMAP_INTERPOLATION_MODE interpolationMode, D2D1_SPRITE_OPTIONS spriteOptions);
        #endregion

        new HRESULT SetPrimitiveBlend2(D2D1_PRIMITIVE_BLEND primitiveBlend);
        #endregion

        HRESULT BlendImage(ID2D1Image image, D2D1_BLEND_MODE blendMode, ref D2D1_POINT_2F targetOffset,
            ref D2D1_RECT_F imageRectangle, D2D1_INTERPOLATION_MODE interpolationMode);
    }

    [ComImport]
    [Guid("a44472e1-8dfb-4e60-8492-6e2861c9ca8b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Device2 : ID2D1Device1
    {
        #region <ID2D1Device1>
        #region <ID2D1Device>
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        new HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext deviceContext);
        //HRESULT CreatePrintControl(IWICImagingFactory wicFactory, IPrintDocumentPackageTarget documentTarget, D2D1_PRINT_CONTROL_PROPERTIES printControlProperties, out ID2D1PrintControl printControl);
        new HRESULT CreatePrintControl(IWICImagingFactory wicFactory, IntPtr documentTarget, D2D1_PRINT_CONTROL_PROPERTIES printControlProperties, out ID2D1PrintControl printControl);
        new void SetMaximumTextureMemory(UInt64 maximumInBytes);
        new UInt64 GetMaximumTextureMemory();
        new void ClearResources(uint millisecondsSinceUse = 0);
        #endregion

        new D2D1_RENDERING_PRIORITY GetRenderingPriority();
        new void SetRenderingPriority(D2D1_RENDERING_PRIORITY renderingPriority);
        new HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext1 deviceContext1);
        #endregion

        HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext2 deviceContext2);
        void FlushDeviceContexts(ID2D1Bitmap bitmap);
        HRESULT GetDxgiDevice(out IDXGIDevice dxgiDevice);
    }

    [ComImport]
    [Guid("852f2087-802c-4037-ab60-ff2e7ee6fc01")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Device3 : ID2D1Device2
    {
        #region <ID2D1Device2>
        #region <ID2D1Device1>
        #region <ID2D1Device>
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        new HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext deviceContext);
        //HRESULT CreatePrintControl(IWICImagingFactory wicFactory, IPrintDocumentPackageTarget documentTarget, D2D1_PRINT_CONTROL_PROPERTIES printControlProperties, out ID2D1PrintControl printControl);
        new HRESULT CreatePrintControl(IWICImagingFactory wicFactory, IntPtr documentTarget, D2D1_PRINT_CONTROL_PROPERTIES printControlProperties, out ID2D1PrintControl printControl);
        new void SetMaximumTextureMemory(UInt64 maximumInBytes);
        new UInt64 GetMaximumTextureMemory();
        new void ClearResources(uint millisecondsSinceUse = 0);
        #endregion

        new D2D1_RENDERING_PRIORITY GetRenderingPriority();
        new void SetRenderingPriority(D2D1_RENDERING_PRIORITY renderingPriority);
        new HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext1 deviceContext1);
        #endregion

        new HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext2 deviceContext2);
        new void FlushDeviceContexts(ID2D1Bitmap bitmap);
        new HRESULT GetDxgiDevice(out IDXGIDevice dxgiDevice);
        #endregion

        HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext3 deviceContext3);
    }

    [ComImport]
    [Guid("d7bdb159-5683-4a46-bc9c-72dc720b858b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Device4 : ID2D1Device3
    {
        #region <ID2D1Device3>
        #region <ID2D1Device2>
        #region <ID2D1Device1>
        #region <ID2D1Device>
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        new HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext deviceContext);
        //HRESULT CreatePrintControl(IWICImagingFactory wicFactory, IPrintDocumentPackageTarget documentTarget, D2D1_PRINT_CONTROL_PROPERTIES printControlProperties, out ID2D1PrintControl printControl);
        new HRESULT CreatePrintControl(IWICImagingFactory wicFactory, IntPtr documentTarget, D2D1_PRINT_CONTROL_PROPERTIES printControlProperties, out ID2D1PrintControl printControl);
        new void SetMaximumTextureMemory(UInt64 maximumInBytes);
        new UInt64 GetMaximumTextureMemory();
        new void ClearResources(uint millisecondsSinceUse = 0);
        #endregion

        new D2D1_RENDERING_PRIORITY GetRenderingPriority();
        new void SetRenderingPriority(D2D1_RENDERING_PRIORITY renderingPriority);
        new HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext1 deviceContext1);
        #endregion

        new HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext2 deviceContext2);
        new void FlushDeviceContexts(ID2D1Bitmap bitmap);
        new HRESULT GetDxgiDevice(out IDXGIDevice dxgiDevice);
        #endregion

        new HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext3 deviceContext3);
        #endregion

        HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext4 deviceContext4);
        void SetMaximumColorGlyphCacheMemory(UInt64 maximumInBytes);
        UInt64 GetMaximumColorGlyphCacheMemory();
    }

    [ComImport]
    [Guid("d55ba0a4-6405-4694-aef5-08ee1a4358b4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Device5 : ID2D1Device4
    {
        #region <ID2D1Device4>
        #region <ID2D1Device3>
        #region <ID2D1Device2>
        #region <ID2D1Device1>
        #region <ID2D1Device>
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        new HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext deviceContext);
        //HRESULT CreatePrintControl(IWICImagingFactory wicFactory, IPrintDocumentPackageTarget documentTarget, D2D1_PRINT_CONTROL_PROPERTIES printControlProperties, out ID2D1PrintControl printControl);
        new HRESULT CreatePrintControl(IWICImagingFactory wicFactory, IntPtr documentTarget, D2D1_PRINT_CONTROL_PROPERTIES printControlProperties, out ID2D1PrintControl printControl);
        new void SetMaximumTextureMemory(UInt64 maximumInBytes);
        new UInt64 GetMaximumTextureMemory();
        new void ClearResources(uint millisecondsSinceUse = 0);
        #endregion

        new D2D1_RENDERING_PRIORITY GetRenderingPriority();
        new void SetRenderingPriority(D2D1_RENDERING_PRIORITY renderingPriority);
        new HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext1 deviceContext1);
        #endregion

        new HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext2 deviceContext2);
        new void FlushDeviceContexts(ID2D1Bitmap bitmap);
        new HRESULT GetDxgiDevice(out IDXGIDevice dxgiDevice);
        #endregion

        new HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext3 deviceContext3);
        #endregion

        new HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext4 deviceContext4);
        new void SetMaximumColorGlyphCacheMemory(UInt64 maximumInBytes);
        new UInt64 GetMaximumColorGlyphCacheMemory();
        #endregion

        HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext5 deviceContext5);
    }

    [ComImport]
    [Guid("7bfef914-2d75-4bad-be87-e18ddb077b6d")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Device6 : ID2D1Device5
    {
        #region <ID2D1Device5>
        #region <ID2D1Device4>
        #region <ID2D1Device3>
        #region <ID2D1Device2>
        #region <ID2D1Device1>
        #region <ID2D1Device>
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        new HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext deviceContext);
        //HRESULT CreatePrintControl(IWICImagingFactory wicFactory, IPrintDocumentPackageTarget documentTarget, D2D1_PRINT_CONTROL_PROPERTIES printControlProperties, out ID2D1PrintControl printControl);
        new HRESULT CreatePrintControl(IWICImagingFactory wicFactory, IntPtr documentTarget, D2D1_PRINT_CONTROL_PROPERTIES printControlProperties, out ID2D1PrintControl printControl);
        new void SetMaximumTextureMemory(UInt64 maximumInBytes);
        new UInt64 GetMaximumTextureMemory();
        new void ClearResources(uint millisecondsSinceUse = 0);
        #endregion

        new D2D1_RENDERING_PRIORITY GetRenderingPriority();
        new void SetRenderingPriority(D2D1_RENDERING_PRIORITY renderingPriority);
        new HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext1 deviceContext1);
        #endregion

        new HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext2 deviceContext2);
        new void FlushDeviceContexts(ID2D1Bitmap bitmap);
        new HRESULT GetDxgiDevice(out IDXGIDevice dxgiDevice);
        #endregion

        new HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext3 deviceContext3);
        #endregion

        new HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext4 deviceContext4);
        new void SetMaximumColorGlyphCacheMemory(UInt64 maximumInBytes);
        new UInt64 GetMaximumColorGlyphCacheMemory();
        #endregion

        new HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext5 deviceContext5);
        #endregion

        HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext6 deviceContext6);
    }   

    public enum D2D1_YCBCR_CHROMA_SUBSAMPLING : uint
    {
        D2D1_YCBCR_CHROMA_SUBSAMPLING_AUTO = 0,
        D2D1_YCBCR_CHROMA_SUBSAMPLING_420 = 1,
        D2D1_YCBCR_CHROMA_SUBSAMPLING_422 = 2,
        D2D1_YCBCR_CHROMA_SUBSAMPLING_444 = 3,
        D2D1_YCBCR_CHROMA_SUBSAMPLING_440 = 4,
        D2D1_YCBCR_CHROMA_SUBSAMPLING_FORCE_DWORD = 0xffffffff
    };

    public enum D2D1_YCBCR_INTERPOLATION_MODE : uint
    {
        D2D1_YCBCR_INTERPOLATION_MODE_NEAREST_NEIGHBOR = 0,
        D2D1_YCBCR_INTERPOLATION_MODE_LINEAR = 1,
        D2D1_YCBCR_INTERPOLATION_MODE_CUBIC = 2,
        D2D1_YCBCR_INTERPOLATION_MODE_MULTI_SAMPLE_LINEAR = 3,
        D2D1_YCBCR_INTERPOLATION_MODE_ANISOTROPIC = 4,
        D2D1_YCBCR_INTERPOLATION_MODE_HIGH_QUALITY_CUBIC = 5,
        D2D1_YCBCR_INTERPOLATION_MODE_FORCE_DWORD = 0xffffffff
    };

    public enum D2D1_YCBCR_PROP : uint
    {
        D2D1_YCBCR_PROP_CHROMA_SUBSAMPLING = 0,
        D2D1_YCBCR_PROP_TRANSFORM_MATRIX = 1,
        D2D1_YCBCR_PROP_INTERPOLATION_MODE = 2,
        D2D1_YCBCR_PROP_FORCE_DWORD = 0xffffffff
    };

    public enum D2D1_SVG_PAINT_TYPE : uint
    {
        /// <summary>
        /// The fill or stroke is not rendered.
        /// </summary>
        D2D1_SVG_PAINT_TYPE_NONE = 0,

        /// <summary>
        /// A solid color is rendered.
        /// </summary>
        D2D1_SVG_PAINT_TYPE_COLOR = 1,

        /// <summary>
        /// The current color is rendered.
        /// </summary>
        D2D1_SVG_PAINT_TYPE_CURRENT_COLOR = 2,

        /// <summary>
        /// A paint server, defined by another element in the SVG document, is used.
        /// </summary>
        D2D1_SVG_PAINT_TYPE_URI = 3,

        /// <summary>
        /// A paint server, defined by another element in the SVG document, is used. If the
        /// paint server reference is invalid, fall back to D2D1_SVG_PAINT_TYPE_NONE.
        /// </summary>
        D2D1_SVG_PAINT_TYPE_URI_NONE = 4,

        /// <summary>
        /// A paint server, defined by another element in the SVG document, is used. If the
        /// paint server reference is invalid, fall back to D2D1_SVG_PAINT_TYPE_COLOR.
        /// </summary>
        D2D1_SVG_PAINT_TYPE_URI_COLOR = 5,

        /// <summary>
        /// A paint server, defined by another element in the SVG document, is used. If the
        /// paint server reference is invalid, fall back to
        /// D2D1_SVG_PAINT_TYPE_CURRENT_COLOR.
        /// </summary>
        D2D1_SVG_PAINT_TYPE_URI_CURRENT_COLOR = 6,
        D2D1_SVG_PAINT_TYPE_FORCE_DWORD = 0xffffffff
    }

    /// <summary>
    /// Specifies the units for an SVG length.
    /// </summary>
    public enum D2D1_SVG_LENGTH_UNITS : uint
    {
        /// <summary>
        /// The length is unitless.
        /// </summary>
        D2D1_SVG_LENGTH_UNITS_NUMBER = 0,

        /// <summary>
        /// The length is a percentage value.
        /// </summary>
        D2D1_SVG_LENGTH_UNITS_PERCENTAGE = 1,
        D2D1_SVG_LENGTH_UNITS_FORCE_DWORD = 0xffffffff
    }

    /// <summary>
    /// Specifies a value for the SVG display property.
    /// </summary>
    public enum D2D1_SVG_DISPLAY : uint
    {
        /// <summary>
        /// The element uses the default display behavior.
        /// </summary>
        D2D1_SVG_DISPLAY_INLINE = 0,

        /// <summary>
        /// The element and all children are not rendered directly.
        /// </summary>
        D2D1_SVG_DISPLAY_NONE = 1,
        D2D1_SVG_DISPLAY_FORCE_DWORD = 0xffffffff
    }

    /// <summary>
    /// Specifies a value for the SVG visibility property.
    /// </summary>
    public enum D2D1_SVG_VISIBILITY : uint
    {
        /// <summary>
        /// The element is visible.
        /// </summary>
        D2D1_SVG_VISIBILITY_VISIBLE = 0,

        /// <summary>
        /// The element is invisible.
        /// </summary>
        D2D1_SVG_VISIBILITY_HIDDEN = 1,
        D2D1_SVG_VISIBILITY_FORCE_DWORD = 0xffffffff
    }

    /// <summary>
    /// Specifies a value for the SVG overflow property.
    /// </summary>
    public enum D2D1_SVG_OVERFLOW : uint
    {
        /// <summary>
        /// The element is not clipped to its viewport.
        /// </summary>
        D2D1_SVG_OVERFLOW_VISIBLE = 0,

        /// <summary>
        /// The element is clipped to its viewport.
        /// </summary>
        D2D1_SVG_OVERFLOW_HIDDEN = 1,
        D2D1_SVG_OVERFLOW_FORCE_DWORD = 0xffffffff
    }

    /// <summary>
    /// Specifies a value for the SVG stroke-linecap property.
    /// </summary>
    public enum D2D1_SVG_LINE_CAP : uint
    {
        /// <summary>
        /// The property is set to SVG's 'butt' value.
        /// </summary>
        D2D1_SVG_LINE_CAP_BUTT = D2D1_CAP_STYLE.D2D1_CAP_STYLE_FLAT,

        /// <summary>
        /// The property is set to SVG's 'square' value.
        /// </summary>
        D2D1_SVG_LINE_CAP_SQUARE = D2D1_CAP_STYLE.D2D1_CAP_STYLE_SQUARE,

        /// <summary>
        /// The property is set to SVG's 'round' value.
        /// </summary>
        D2D1_SVG_LINE_CAP_ROUND = D2D1_CAP_STYLE.D2D1_CAP_STYLE_ROUND,
        D2D1_SVG_LINE_CAP_FORCE_DWORD = 0xffffffff
    }

    public enum D2D1_CAP_STYLE : uint
    {
        /// <summary>
        /// Flat line cap.
        /// </summary>
        D2D1_CAP_STYLE_FLAT = 0,

        /// <summary>
        /// Square line cap.
        /// </summary>
        D2D1_CAP_STYLE_SQUARE = 1,

        /// <summary>
        /// Round line cap.
        /// </summary>
        D2D1_CAP_STYLE_ROUND = 2,

        /// <summary>
        /// Triangle line cap.
        /// </summary>
        D2D1_CAP_STYLE_TRIANGLE = 3,
        D2D1_CAP_STYLE_FORCE_DWORD = 0xffffffff

    }

    /// <summary>
    /// Specifies a value for the SVG stroke-linejoin property.
    /// </summary>
    public enum D2D1_SVG_LINE_JOIN : uint
    {
        /// <summary>
        /// The property is set to SVG's 'bevel' value.
        /// </summary>
        D2D1_SVG_LINE_JOIN_BEVEL = D2D1_LINE_JOIN.D2D1_LINE_JOIN_BEVEL,

        /// <summary>
        /// The property is set to SVG's 'miter' value. Note that this is equivalent to
        /// D2D1_LINE_JOIN_MITER_OR_BEVEL, not D2D1_LINE_JOIN_MITER.
        /// </summary>
        D2D1_SVG_LINE_JOIN_MITER = D2D1_LINE_JOIN.D2D1_LINE_JOIN_MITER_OR_BEVEL,

        /// <summary>
        /// \ The property is set to SVG's 'round' value.
        /// </summary>
        D2D1_SVG_LINE_JOIN_ROUND = D2D1_LINE_JOIN.D2D1_LINE_JOIN_ROUND,
        D2D1_SVG_LINE_JOIN_FORCE_DWORD = 0xffffffff
    }
      
    /// <summary>
    /// The alignment portion of the SVG preserveAspectRatio attribute.
    /// </summary>
    public enum D2D1_SVG_ASPECT_ALIGN : uint
    {
        /// <summary>
        /// The alignment is set to SVG's 'none' value.
        /// </summary>
        D2D1_SVG_ASPECT_ALIGN_NONE = 0,

        /// <summary>
        /// The alignment is set to SVG's 'xMinYMin' value.
        /// </summary>
        D2D1_SVG_ASPECT_ALIGN_X_MIN_Y_MIN = 1,

        /// <summary>
        /// The alignment is set to SVG's 'xMidYMin' value.
        /// </summary>
        D2D1_SVG_ASPECT_ALIGN_X_MID_Y_MIN = 2,

        /// <summary>
        /// The alignment is set to SVG's 'xMaxYMin' value.
        /// </summary>
        D2D1_SVG_ASPECT_ALIGN_X_MAX_Y_MIN = 3,

        /// <summary>
        /// The alignment is set to SVG's 'xMinYMid' value.
        /// </summary>
        D2D1_SVG_ASPECT_ALIGN_X_MIN_Y_MID = 4,

        /// <summary>
        /// The alignment is set to SVG's 'xMidYMid' value.
        /// </summary>
        D2D1_SVG_ASPECT_ALIGN_X_MID_Y_MID = 5,

        /// <summary>
        /// The alignment is set to SVG's 'xMaxYMid' value.
        /// </summary>
        D2D1_SVG_ASPECT_ALIGN_X_MAX_Y_MID = 6,

        /// <summary>
        /// The alignment is set to SVG's 'xMinYMax' value.
        /// </summary>
        D2D1_SVG_ASPECT_ALIGN_X_MIN_Y_MAX = 7,

        /// <summary>
        /// The alignment is set to SVG's 'xMidYMax' value.
        /// </summary>
        D2D1_SVG_ASPECT_ALIGN_X_MID_Y_MAX = 8,

        /// <summary>
        /// The alignment is set to SVG's 'xMaxYMax' value.
        /// </summary>
        D2D1_SVG_ASPECT_ALIGN_X_MAX_Y_MAX = 9,
        D2D1_SVG_ASPECT_ALIGN_FORCE_DWORD = 0xffffffff
    }

    /// <summary>
    /// The meetOrSlice portion of the SVG preserveAspectRatio attribute.
    /// </summary>
    public enum D2D1_SVG_ASPECT_SCALING :uint
    {
        /// <summary>
        /// Scale the viewBox up as much as possible such that the entire viewBox is visible
        /// within the viewport.
        /// </summary>
        D2D1_SVG_ASPECT_SCALING_MEET = 0,

        /// <summary>
        /// Scale the viewBox down as much as possible such that the entire viewport is
        /// covered by the viewBox.
        /// </summary>
        D2D1_SVG_ASPECT_SCALING_SLICE = 1,
        D2D1_SVG_ASPECT_SCALING_FORCE_DWORD = 0xffffffff
    }

    /// <summary>
    /// Represents a path commmand. Each command may reference floats from the segment
    /// data. Commands ending in _ABSOLUTE interpret data as absolute coordinate.
    /// Commands ending in _RELATIVE interpret data as being relative to the previous
    /// point.
    /// </summary>
    public enum D2D1_SVG_PATH_COMMAND : uint
    {
        /// <summary>
        /// Closes the current subpath. Uses no segment data.
        /// </summary>
        D2D1_SVG_PATH_COMMAND_CLOSE_PATH = 0,

        /// <summary>
        /// Starts a new subpath at the coordinate (x y). Uses 2 floats of segment data.
        /// </summary>
        D2D1_SVG_PATH_COMMAND_MOVE_ABSOLUTE = 1,

        /// <summary>
        /// Starts a new subpath at the coordinate (x y). Uses 2 floats of segment data.
        /// </summary>
        D2D1_SVG_PATH_COMMAND_MOVE_RELATIVE = 2,

        /// <summary>
        /// Draws a line to the coordinate (x y). Uses 2 floats of segment data.
        /// </summary>
        D2D1_SVG_PATH_COMMAND_LINE_ABSOLUTE = 3,

        /// <summary>
        /// Draws a line to the coordinate (x y). Uses 2 floats of segment data.
        /// </summary>
        D2D1_SVG_PATH_COMMAND_LINE_RELATIVE = 4,

        /// <summary>
        /// Draws a cubic Bezier curve (x1 y1 x2 y2 x y). The curve ends at (x, y) and is
        /// defined by the two control points (x1, y1) and (x2, y2). Uses 6 floats of
        /// segment data.
        /// </summary>
        D2D1_SVG_PATH_COMMAND_CUBIC_ABSOLUTE = 5,

        /// <summary>
        /// Draws a cubic Bezier curve (x1 y1 x2 y2 x y). The curve ends at (x, y) and is
        /// defined by the two control points (x1, y1) and (x2, y2). Uses 6 floats of
        /// segment data.
        /// </summary>
        D2D1_SVG_PATH_COMMAND_CUBIC_RELATIVE = 6,

        /// <summary>
        /// Draws a quadratic Bezier curve (x1 y1 x y). The curve ends at (x, y) and is
        /// defined by the control point (x1 y1). Uses 4 floats of segment data.
        /// </summary>
        D2D1_SVG_PATH_COMMAND_QUADRADIC_ABSOLUTE = 7,

        /// <summary>
        /// Draws a quadratic Bezier curve (x1 y1 x y). The curve ends at (x, y) and is
        /// defined by the control point (x1 y1). Uses 4 floats of segment data.
        /// </summary>
        D2D1_SVG_PATH_COMMAND_QUADRADIC_RELATIVE = 8,

        /// <summary>
        /// Draws an elliptical arc (rx ry x-axis-rotation large-arc-flag sweep-flag x y).
        /// The curve ends at (x, y) and is defined by the arc parameters. The two flags are
        /// considered set if their values are non-zero. Uses 7 floats of segment data.
        /// </summary>
        D2D1_SVG_PATH_COMMAND_ARC_ABSOLUTE = 9,

        /// <summary>
        /// Draws an elliptical arc (rx ry x-axis-rotation large-arc-flag sweep-flag x y).
        /// The curve ends at (x, y) and is defined by the arc parameters. The two flags are
        /// considered set if their values are non-zero. Uses 7 floats of segment data.
        /// </summary>
        D2D1_SVG_PATH_COMMAND_ARC_RELATIVE = 10,

        /// <summary>
        /// Draws a horizontal line to the coordinate (x). Uses 1 float of segment data.
        /// </summary>
        D2D1_SVG_PATH_COMMAND_HORIZONTAL_ABSOLUTE = 11,

        /// <summary>
        /// Draws a horizontal line to the coordinate (x). Uses 1 float of segment data.
        /// </summary>
        D2D1_SVG_PATH_COMMAND_HORIZONTAL_RELATIVE = 12,

        /// <summary>
        /// Draws a vertical line to the coordinate (y). Uses 1 float of segment data.
        /// </summary>
        D2D1_SVG_PATH_COMMAND_VERTICAL_ABSOLUTE = 13,

        /// <summary>
        /// Draws a vertical line to the coordinate (y). Uses 1 float of segment data.
        /// </summary>
        D2D1_SVG_PATH_COMMAND_VERTICAL_RELATIVE = 14,

        /// <summary>
        /// Draws a smooth cubic Bezier curve (x2 y2 x y). The curve ends at (x, y) and is
        /// defined by the control point (x2, y2). Uses 4 floats of segment data.
        /// </summary>
        D2D1_SVG_PATH_COMMAND_CUBIC_SMOOTH_ABSOLUTE = 15,

        /// <summary>
        /// Draws a smooth cubic Bezier curve (x2 y2 x y). The curve ends at (x, y) and is
        /// defined by the control point (x2, y2). Uses 4 floats of segment data.
        /// </summary>
        D2D1_SVG_PATH_COMMAND_CUBIC_SMOOTH_RELATIVE = 16,

        /// <summary>
        /// Draws a smooth quadratic Bezier curve ending at (x, y). Uses 2 floats of segment
        /// data.
        /// </summary>
        D2D1_SVG_PATH_COMMAND_QUADRADIC_SMOOTH_ABSOLUTE = 17,

        /// <summary>
        /// Draws a smooth quadratic Bezier curve ending at (x, y). Uses 2 floats of segment
        /// data.
        /// </summary>
        D2D1_SVG_PATH_COMMAND_QUADRADIC_SMOOTH_RELATIVE = 18,
        D2D1_SVG_PATH_COMMAND_FORCE_DWORD = 0xffffffff
    }

    /// <summary>
    /// Defines the coordinate system used for SVG gradient or clipPath elements.
    /// </summary>
    public enum D2D1_SVG_UNIT_TYPE : uint
    {
        /// <summary>
        /// The property is set to SVG's 'userSpaceOnUse' value.
        /// </summary>
        D2D1_SVG_UNIT_TYPE_USER_SPACE_ON_USE = 0,

        /// <summary>
        /// The property is set to SVG's 'objectBoundingBox' value.
        /// </summary>
        D2D1_SVG_UNIT_TYPE_OBJECT_BOUNDING_BOX = 1,
        D2D1_SVG_UNIT_TYPE_FORCE_DWORD = 0xffffffff
    }

    /// <summary>
    /// Defines the type of SVG string attribute to set or get.
    /// </summary>
    public enum D2D1_SVG_ATTRIBUTE_STRING_TYPE : uint
    {
        /// <summary>
        /// The attribute is a string in the same form as it would appear in the SVG XML.
        /// 
        /// Note that when getting values of this type, the value returned may not exactly
        /// match the value that was set. Instead, the output value is a normalized version
        /// of the value. For example, an input color of 'red' may be output as '#FF0000'.
        /// </summary>
        D2D1_SVG_ATTRIBUTE_STRING_TYPE_SVG = 0,

        /// <summary>
        /// The attribute is an element ID.
        /// </summary>
        D2D1_SVG_ATTRIBUTE_STRING_TYPE_ID = 1,
        D2D1_SVG_ATTRIBUTE_STRING_TYPE_FORCE_DWORD = 0xffffffff
    }

    /// <summary>
    /// Defines the type of SVG POD attribute to set or get.
    /// </summary>
    public enum D2D1_SVG_ATTRIBUTE_POD_TYPE : uint
    {
        /// <summary>
        /// The attribute is a FLOAT.
        /// </summary>
        D2D1_SVG_ATTRIBUTE_POD_TYPE_FLOAT = 0,

        /// <summary>
        /// The attribute is a D2D1_COLOR_F.
        /// </summary>
        D2D1_SVG_ATTRIBUTE_POD_TYPE_COLOR = 1,

        /// <summary>
        /// The attribute is a D2D1_FILL_MODE.
        /// </summary>
        D2D1_SVG_ATTRIBUTE_POD_TYPE_FILL_MODE = 2,

        /// <summary>
        /// The attribute is a D2D1_SVG_DISPLAY.
        /// </summary>
        D2D1_SVG_ATTRIBUTE_POD_TYPE_DISPLAY = 3,

        /// <summary>
        /// The attribute is a D2D1_SVG_OVERFLOW.
        /// </summary>
        D2D1_SVG_ATTRIBUTE_POD_TYPE_OVERFLOW = 4,

        /// <summary>
        /// The attribute is a D2D1_SVG_LINE_CAP.
        /// </summary>
        D2D1_SVG_ATTRIBUTE_POD_TYPE_LINE_CAP = 5,

        /// <summary>
        /// The attribute is a D2D1_SVG_LINE_JOIN.
        /// </summary>
        D2D1_SVG_ATTRIBUTE_POD_TYPE_LINE_JOIN = 6,

        /// <summary>
        /// The attribute is a D2D1_SVG_VISIBILITY.
        /// </summary>
        D2D1_SVG_ATTRIBUTE_POD_TYPE_VISIBILITY = 7,

        /// <summary>
        /// The attribute is a D2D1_MATRIX_3X2_F.
        /// </summary>
        D2D1_SVG_ATTRIBUTE_POD_TYPE_MATRIX = 8,

        /// <summary>
        /// The attribute is a D2D1_SVG_UNIT_TYPE.
        /// </summary>
        D2D1_SVG_ATTRIBUTE_POD_TYPE_UNIT_TYPE = 9,

        /// <summary>
        /// The attribute is a D2D1_EXTEND_MODE.
        /// </summary>
        D2D1_SVG_ATTRIBUTE_POD_TYPE_EXTEND_MODE = 10,

        /// <summary>
        /// The attribute is a D2D1_SVG_PRESERVE_ASPECT_RATIO.
        /// </summary>
        D2D1_SVG_ATTRIBUTE_POD_TYPE_PRESERVE_ASPECT_RATIO = 11,

        /// <summary>
        /// The attribute is a D2D1_SVG_VIEWBOX.
        /// </summary>
        D2D1_SVG_ATTRIBUTE_POD_TYPE_VIEWBOX = 12,

        /// <summary>
        /// The attribute is a D2D1_SVG_LENGTH.
        /// </summary>
        D2D1_SVG_ATTRIBUTE_POD_TYPE_LENGTH = 13,
        D2D1_SVG_ATTRIBUTE_POD_TYPE_FORCE_DWORD = 0xffffffff
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_SVG_LENGTH
    {
        public float value;
        public D2D1_SVG_LENGTH_UNITS units;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_SVG_PRESERVE_ASPECT_RATIO
    {
        /// <summary>
        /// Sets the 'defer' portion of the preserveAspectRatio settings. This field only
        /// has an effect on an 'image' element that references another SVG document. As
        /// this is not currently supported, the field has no impact on rendering.
        /// </summary>
        public bool defer;

        /// <summary>
        /// Sets the align portion of the preserveAspectRatio settings.
        /// </summary>
        public D2D1_SVG_ASPECT_ALIGN align;

        /// <summary>
        /// Sets the meetOrSlice portion of the preserveAspectRatio settings.
        /// </summary>
        public D2D1_SVG_ASPECT_SCALING meetOrSlice;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_SVG_VIEWBOX
    {
        public float x;
        public float y;
        public float width;
        public float height;
    }

    /// <summary>
    /// Indicates what has changed since the last time the effect was asked to prepare
    /// to render.
    /// </summary>
    public enum D2D1_CHANGE_TYPE : uint
    {
        /// <summary>
        /// Nothing has changed.
        /// </summary>
        D2D1_CHANGE_TYPE_NONE = 0,

        /// <summary>
        /// The effect's properties have changed.
        /// </summary>
        D2D1_CHANGE_TYPE_PROPERTIES = 1,

        /// <summary>
        /// The internal context has changed and should be inspected.
        /// </summary>
        D2D1_CHANGE_TYPE_CONTEXT = 2,

        /// <summary>
        /// A new graph has been set due to a change in the input count.
        /// </summary>
        D2D1_CHANGE_TYPE_GRAPH = 3,
        D2D1_CHANGE_TYPE_FORCE_DWORD = 0xffffffff
    }

    /// <summary>
    /// Indicates options for drawing using a pixel shader.
    /// </summary>
    public enum D2D1_PIXEL_OPTIONS : uint
    {
        /// <summary>
        /// Default pixel processing.
        /// </summary>
        D2D1_PIXEL_OPTIONS_NONE = 0,

        /// <summary>
        /// Indicates that the shader samples its inputs only at exactly the same scene
        /// coordinate as the output pixel, and that it returns transparent black whenever
        /// the input pixels are also transparent black.
        /// </summary>
        D2D1_PIXEL_OPTIONS_TRIVIAL_SAMPLING = 1,
        D2D1_PIXEL_OPTIONS_FORCE_DWORD = 0xffffffff
    }

    /// <summary>
    /// Indicates options for drawing custom vertices set by transforms.
    /// </summary>
    public enum D2D1_VERTEX_OPTIONS : uint
    {
        /// <summary>
        /// Default vertex processing.
        /// </summary>
        D2D1_VERTEX_OPTIONS_NONE = 0,

        /// <summary>
        /// Indicates that the output rectangle does not need to be cleared before drawing
        /// custom vertices. This must only be used by transforms whose custom vertices
        /// completely cover their output rectangle.
        /// </summary>
        D2D1_VERTEX_OPTIONS_DO_NOT_CLEAR = 1,

        /// <summary>
        /// Causes a depth buffer to be used while drawing custom vertices. This impacts
        /// drawing behavior when primitives overlap one another.
        /// </summary>
        D2D1_VERTEX_OPTIONS_USE_DEPTH_BUFFER = 2,

        /// <summary>
        /// Indicates that custom vertices do not form primitives which overlap one another.
        /// </summary>
        D2D1_VERTEX_OPTIONS_ASSUME_NO_OVERLAP = 4,
        D2D1_VERTEX_OPTIONS_FORCE_DWORD = 0xffffffff
    }

    /// <summary>
    /// Describes how a vertex buffer is to be managed.
    /// </summary>
    public enum D2D1_VERTEX_USAGE : uint
    {

        /// <summary>
        /// The vertex buffer content do not change frequently from frame to frame.
        /// </summary>
        D2D1_VERTEX_USAGE_STATIC = 0,

        /// <summary>
        /// The vertex buffer is intended to be updated frequently.
        /// </summary>
        D2D1_VERTEX_USAGE_DYNAMIC = 1,
        D2D1_VERTEX_USAGE_FORCE_DWORD = 0xffffffff
    }

    /// <summary>
    /// Describes a particular blend in the D2D1_BLEND_DESCRIPTION structure.
    /// </summary>
    public enum D2D1_BLEND_OPERATION : uint
    {
        D2D1_BLEND_OPERATION_ADD = 1,
        D2D1_BLEND_OPERATION_SUBTRACT = 2,
        D2D1_BLEND_OPERATION_REV_SUBTRACT = 3,
        D2D1_BLEND_OPERATION_MIN = 4,
        D2D1_BLEND_OPERATION_MAX = 5,
        D2D1_BLEND_OPERATION_FORCE_DWORD = 0xffffffff
    }

    /// <summary>
    /// Describes a particular blend in the D2D1_BLEND_DESCRIPTION structure.
    /// </summary>
    public enum D2D1_BLEND : uint
    {
        D2D1_BLEND_ZERO = 1,
        D2D1_BLEND_ONE = 2,
        D2D1_BLEND_SRC_COLOR = 3,
        D2D1_BLEND_INV_SRC_COLOR = 4,
        D2D1_BLEND_SRC_ALPHA = 5,
        D2D1_BLEND_INV_SRC_ALPHA = 6,
        D2D1_BLEND_DEST_ALPHA = 7,
        D2D1_BLEND_INV_DEST_ALPHA = 8,
        D2D1_BLEND_DEST_COLOR = 9,
        D2D1_BLEND_INV_DEST_COLOR = 10,
        D2D1_BLEND_SRC_ALPHA_SAT = 11,
        D2D1_BLEND_BLEND_FACTOR = 14,
        D2D1_BLEND_INV_BLEND_FACTOR = 15,
        D2D1_BLEND_FORCE_DWORD = 0xffffffff
    }

    /// <summary>
    /// Allows a caller to control the channel depth of a stage in the rendering
    /// pipeline.
    /// </summary>
    public enum D2D1_CHANNEL_DEPTH : uint
    {
        D2D1_CHANNEL_DEPTH_DEFAULT = 0,
        D2D1_CHANNEL_DEPTH_1 = 1,
        D2D1_CHANNEL_DEPTH_4 = 4,
        D2D1_CHANNEL_DEPTH_FORCE_DWORD = 0xffffffff
    }

    /// <summary>
    /// Represents filtering modes transforms may select to use on their input textures.
    /// </summary>
    public enum D2D1_FILTER : uint
    {
        D2D1_FILTER_MIN_MAG_MIP_POINT = 0x00,
        D2D1_FILTER_MIN_MAG_POINT_MIP_LINEAR = 0x01,
        D2D1_FILTER_MIN_POINT_MAG_LINEAR_MIP_POINT = 0x04,
        D2D1_FILTER_MIN_POINT_MAG_MIP_LINEAR = 0x05,
        D2D1_FILTER_MIN_LINEAR_MAG_MIP_POINT = 0x10,
        D2D1_FILTER_MIN_LINEAR_MAG_POINT_MIP_LINEAR = 0x11,
        D2D1_FILTER_MIN_MAG_LINEAR_MIP_POINT = 0x14,
        D2D1_FILTER_MIN_MAG_MIP_LINEAR = 0x15,
        D2D1_FILTER_ANISOTROPIC = 0x55,
        D2D1_FILTER_FORCE_DWORD = 0xffffffff
    }

    /// <summary>
    /// Defines capabilities of the underlying D3D device which may be queried using
    /// CheckFeatureSupport.
    /// </summary>
    public enum D2D1_FEATURE : uint
    {
        D2D1_FEATURE_DOUBLES = 0,
        D2D1_FEATURE_D3D10_X_HARDWARE_OPTIONS = 1,
        D2D1_FEATURE_FORCE_DWORD = 0xffffffff
    }

    /// <summary>
    /// Defines a property binding to a function. The name must match the property
    /// defined in the registration schema.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_PROPERTY_BINDING
    {
        /// <summary>
        /// The name of the property.
        /// </summary>
        public string propertyName;

        /// <summary>
        /// The function that will receive the data to set.
        /// </summary>
        public PD2D1_PROPERTY_SET_FUNCTION setFunction;

        /// <summary>
        /// The function that will be asked to write the output data.
        /// </summary>
        public PD2D1_PROPERTY_GET_FUNCTION getFunction;
    }

    public delegate HRESULT PD2D1_PROPERTY_GET_FUNCTION(IntPtr effect, out IntPtr data, uint dataSize, out uint actualSize);

    public delegate HRESULT PD2D1_PROPERTY_SET_FUNCTION(IntPtr effect, IntPtr data, uint dataSize);

    /// <summary>
    /// This is used to define a resource texture when that resource texture is created.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_RESOURCE_TEXTURE_PROPERTIES
    {
        //_Field_size_(dimensions) CONST uint* extents;
        public IntPtr extents;
        public uint dimensions;
        public D2D1_BUFFER_PRECISION bufferPrecision;
        public D2D1_CHANNEL_DEPTH channelDepth;
        public D2D1_FILTER filter;
        //_Field_size_(dimensions) CONST D2D1_EXTEND_MODE *extendModes;
        public IntPtr extendModes;
    }

    /// <summary>
    /// This defines a single element of the vertex layout.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_INPUT_ELEMENT_DESC
    {
        public string semanticName;
        public uint semanticIndex;
        public DXGI_FORMAT format;
        public uint inputSlot;
        public uint alignedByteOffset;
    }

    /// <summary>
    /// This defines the properties of a vertex buffer which uses the default vertex
    /// layout.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_VERTEX_BUFFER_PROPERTIES
    {
        public uint inputCount;
        public D2D1_VERTEX_USAGE usage;
        public IntPtr data;
        public uint byteWidth;
    }

    /// <summary>
    /// This defines the input layout of vertices and the vertex shader which processes
    /// them.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_CUSTOM_VERTEX_BUFFER_PROPERTIES
    {
        public IntPtr shaderBufferWithInputSignature;
        public uint shaderBufferSize;
        //public D2D1_INPUT_ELEMENT_DESC *inputElements;
        public IntPtr inputElements;
        public uint elementCount;
        public uint stride;
    }

    /// <summary>
    /// This defines the range of vertices from a vertex buffer to draw.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_VERTEX_RANGE
    {
        public uint startVertex;
        public uint vertexCount;
    }

    /// <summary>
    /// Blend description which configures a blend transform object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_BLEND_DESCRIPTION
    {
        public D2D1_BLEND sourceBlend;
        public D2D1_BLEND destinationBlend;
        public D2D1_BLEND_OPERATION blendOperation;
        public D2D1_BLEND sourceBlendAlpha;
        public D2D1_BLEND destinationBlendAlpha;
        public D2D1_BLEND_OPERATION blendOperationAlpha;
        [MarshalAs(UnmanagedType.R4, SizeConst = 4)]
        public float blendFactor;
    }

    /// <summary>
    /// Describes options transforms may select to use on their input textures.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_INPUT_DESCRIPTION
    {
        public D2D1_FILTER filter;
        public uint levelOfDetailCount;
    }

    /// <summary>
    /// Indicates whether shader support for doubles is present on the underlying
    /// hardware.  This may be populated using CheckFeatureSupport.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_FEATURE_DATA_DOUBLES
    {
        public bool doublePrecisionFloatShaderOps;
    }

    /// <summary>
    /// Indicates support for features which are optional on D3D10 feature levels.  This
    /// may be populated using CheckFeatureSupport.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_FEATURE_DATA_D3D10_X_HARDWARE_OPTIONS
    {
        public bool computeShaders_Plus_RawAndStructuredBuffers_Via_Shader_4_x;
    }

    [ComImport]
    [Guid("0359dc30-95e6-4568-9055-27720d130e93")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1AnalysisTransform
    {
        HRESULT ProcessAnalysisResults(IntPtr analysisData, uint analysisDataCount);
    }

    [ComImport]
    [Guid("63ac0b32-ba44-450f-8806-7f4ca1ff2f1b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1BlendTransform : ID2D1ConcreteTransform
    {
        #region <ID2D1ConcreteTransform>
        #region <ID2D1TransformNode>
        new uint GetInputCount();
        #endregion

        new HRESULT SetOutputBuffer(D2D1_BUFFER_PRECISION bufferPrecision, D2D1_CHANNEL_DEPTH channelDepth);
        new void SetCached(bool isCached);
        #endregion

        void SetDescription(D2D1_BLEND_DESCRIPTION description);
        void GetDescription(out D2D1_BLEND_DESCRIPTION description);
    }

    [ComImport]
    [Guid("b2efe1e7-729f-4102-949f-505fa21bf666")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1TransformNode
    {
        uint GetInputCount();
    }

    [ComImport]
    [Guid("1a799d8a-69f7-4e4c-9fed-437ccc6684cc")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1ConcreteTransform : ID2D1TransformNode
    {
        #region <ID2D1TransformNode>
        new uint GetInputCount();
        #endregion

        HRESULT SetOutputBuffer(D2D1_BUFFER_PRECISION bufferPrecision, D2D1_CHANNEL_DEPTH channelDepth);
        void SetCached(bool isCached);
    }

    [ComImport]
    [Guid("4998735c-3a19-473c-9781-656847e3a347")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1BorderTransform : ID2D1ConcreteTransform
    {
        #region <ID2D1ConcreteTransform>
        #region <ID2D1TransformNode>
        new uint GetInputCount();
        #endregion

        new HRESULT SetOutputBuffer(D2D1_BUFFER_PRECISION bufferPrecision, D2D1_CHANNEL_DEPTH channelDepth);
        new void SetCached(bool isCached);
        #endregion

        void SetExtendModeX(D2D1_EXTEND_MODE extendMode);
        void SetExtendModeY(D2D1_EXTEND_MODE extendMode);
        D2D1_EXTEND_MODE GetExtendModeX();
        D2D1_EXTEND_MODE GetExtendModeY();
    }

    [ComImport]
    [Guid("90f732e2-5092-4606-a819-8651970baccd")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1BoundsAdjustmentTransform : ID2D1TransformNode
    {
        #region <ID2D1TransformNode>
        new uint GetInputCount();
        #endregion
    
        void SetOutputBounds(ref D2D1_RECT_L outputBounds);
        void GetOutputBounds(out D2D1_RECT_L outputBounds);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_RECT_L
    {
        public long left;
        public long top;
        public long right;
        public long bottom;
        public D2D1_RECT_L(long left, long top, long right, long bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }
    }

    [ComImport]
    [Guid("519ae1bd-d19a-420d-b849-364f594776b7")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1RenderInfo
    {
        HRESULT SetInputDescription(uint inputIndex, D2D1_INPUT_DESCRIPTION inputDescription);
        HRESULT SetOutputBuffer(D2D1_BUFFER_PRECISION bufferPrecision, D2D1_CHANNEL_DEPTH channelDepth);
        void SetCached(bool isCached);
        void SetInstructionCountHint(uint instructionCount);
    }

    [ComImport]
    [Guid("5598b14b-9fd7-48b7-9bdb-8f0964eb38bc")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1ComputeInfo : ID2D1RenderInfo
    {
        #region <ID2D1RenderInfo>
        new HRESULT SetInputDescription(uint inputIndex, D2D1_INPUT_DESCRIPTION inputDescription);
        new HRESULT SetOutputBuffer(D2D1_BUFFER_PRECISION bufferPrecision, D2D1_CHANNEL_DEPTH channelDepth);
        new void SetCached(bool isCached);
        new void SetInstructionCountHint(uint instructionCount);
        #endregion

        HRESULT SetComputeShaderConstantBuffer(IntPtr buffer, uint bufferCount);        
        HRESULT SetComputeShader(ref Guid shaderId);
        HRESULT SetResourceTexture(uint textureIndex, ID2D1ResourceTexture resourceTexture);
    }

    [ComImport]
    [Guid("688d15c3-02b0-438d-b13a-d1b44c32c39a")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1ResourceTexture
    {
        HRESULT Update(uint minimumExtents, uint maximimumExtents, uint strides, uint dimensions, IntPtr data, uint dataCount);       
    }

    [ComImport]
    [Guid("db1800dd-0c34-4cf9-be90-31cc0a5653e1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1SourceTransform : ID2D1Transform
    {
        #region <ID2D1Transform>
        #region <ID2D1TransformNode>
        new uint GetInputCount();
        #endregion

        new HRESULT MapOutputRectToInputRects(D2D1_RECT_L outputRect, out D2D1_RECT_L inputRects, uint inputRectsCount);
        new HRESULT MapInputRectsToOutputRect(IntPtr inputRects, IntPtr inputOpaqueSubRects, uint inputRectCount,
            out D2D1_RECT_L outputRect, out D2D1_RECT_L outputOpaqueSubRect);
        new HRESULT MapInvalidRect(uint inputIndex, D2D1_RECT_L invalidInputRect, out D2D1_RECT_L invalidOutputRect);
        #endregion

        HRESULT SetRenderInfo(ID2D1RenderInfo renderInfo);
        HRESULT Draw(ID2D1Bitmap1 target, ref D2D1_RECT_L drawRect, D2D1_POINT_2U targetOrigin);
    }

    [ComImport]
    [Guid("ef1a287d-342a-4f76-8fdb-da0d6ea9f92b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1Transform : ID2D1TransformNode
    {
        #region <ID2D1TransformNode>
        new uint GetInputCount();
        #endregion

        HRESULT MapOutputRectToInputRects(D2D1_RECT_L outputRect, out D2D1_RECT_L inputRects, uint inputRectsCount);
        HRESULT MapInputRectsToOutputRect(IntPtr inputRects, IntPtr inputOpaqueSubRects, uint inputRectCount,
            out D2D1_RECT_L outputRect,out D2D1_RECT_L outputOpaqueSubRect); 
        HRESULT MapInvalidRect(uint inputIndex, D2D1_RECT_L invalidInputRect, out D2D1_RECT_L invalidOutputRect);
    }

    [ComImport]
    [Guid("13d29038-c3e6-4034-9081-13b53a417992")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1TransformGraph
    {       
        uint GetInputCount();
        HRESULT SetSingleTransformNode(ID2D1TransformNode node);
        HRESULT AddNode(ID2D1TransformNode node); 
        HRESULT RemoveNode(ID2D1TransformNode node);
        HRESULT SetOutputNode(ID2D1TransformNode node);
        HRESULT ConnectNode(ID2D1TransformNode fromNode, ID2D1TransformNode toNode, uint toNodeInputIndex);
        HRESULT ConnectToEffectInput(uint toEffectInputIndex, ID2D1TransformNode node, uint toNodeInputIndex); 
        void Clear();    
        HRESULT SetPassthroughGraph(uint effectInputIndex);
    }

    [ComImport]
    [Guid("693ce632-7f2f-45de-93fe-18d88b37aa21")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1DrawInfo : ID2D1RenderInfo
    {
        #region <ID2D1RenderInfo>
        new HRESULT SetInputDescription(uint inputIndex, D2D1_INPUT_DESCRIPTION inputDescription);
        new HRESULT SetOutputBuffer(D2D1_BUFFER_PRECISION bufferPrecision, D2D1_CHANNEL_DEPTH channelDepth);
        new void SetCached(bool isCached);
        new void SetInstructionCountHint(uint instructionCount);
        #endregion

        HRESULT SetPixelShaderantBuffer(IntPtr buffer, uint bufferCount);
        HRESULT SetResourceTexture(uint textureIndex, ID2D1ResourceTexture resourceTexture);
        HRESULT SetVertexShaderantBuffer(IntPtr buffer, uint bufferCount);
        HRESULT SetPixelShader(ref Guid shaderId, D2D1_PIXEL_OPTIONS pixelOptions = D2D1_PIXEL_OPTIONS.D2D1_PIXEL_OPTIONS_NONE); 
        HRESULT SetVertexProcessing(ID2D1VertexBuffer vertexBuffer, D2D1_VERTEX_OPTIONS vertexOptions, D2D1_BLEND_DESCRIPTION blendDescription,
            D2D1_VERTEX_RANGE vertexRange, ref Guid vertexShader);
    }

    [ComImport]
    [Guid("9b8b1336-00a5-4668-92b7-ced5d8bf9b7b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1VertexBuffer
    {
        HRESULT Map(out IntPtr data, uint bufferSize);
        HRESULT Unmap();
    }

    [ComImport]
    [Guid("3d9f916b-27dc-4ad7-b4f1-64945340f563")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1EffectContext
    {
        void GetDpi(out float dpiX, out float dpiY);
        HRESULT CreateEffect(ref Guid effectId, out ID2D1Effect effect);
        HRESULT GetMaximumSupportedFeatureLevel([MarshalAs(UnmanagedType.LPArray)] int[] featureLevels,
             uint featureLevelsCount, out D3D_FEATURE_LEVEL maximumSupportedFeatureLevel);
        HRESULT CreateTransformNodeFromEffect(ID2D1Effect effect, out ID2D1TransformNode transformNode);
        HRESULT CreateBlendTransform(uint numInputs, D2D1_BLEND_DESCRIPTION blendDescription, out ID2D1BlendTransform transform);
        HRESULT CreateBorderTransform(D2D1_EXTEND_MODE extendModeX, D2D1_EXTEND_MODE extendModeY, out ID2D1BorderTransform transform);
        HRESULT CreateOffsetTransform(ref D2D1_POINT_2L offset, out ID2D1OffsetTransform transform);
        HRESULT CreateBoundsAdjustmentTransform(ref D2D1_RECT_L outputRectangle, out ID2D1BoundsAdjustmentTransform transform);
        HRESULT LoadPixelShader(ref Guid shaderId, IntPtr shaderBuffer, uint shaderBufferCount);
        HRESULT LoadVertexShader(ref Guid resourceId, IntPtr shaderBuffer, uint shaderBufferCount);
        HRESULT LoadComputeShader(ref Guid resourceId, IntPtr shaderBuffer, uint shaderBufferCount);
        bool IsShaderLoaded(ref Guid shaderId);
        HRESULT CreateResourceTexture(ref Guid resourceId, ref D2D1_RESOURCE_TEXTURE_PROPERTIES resourceTextureProperties, IntPtr data, uint strides, uint dataSize, out ID2D1ResourceTexture resourceTexture);
        HRESULT FindResourceTexture(ref Guid resourceId, out ID2D1ResourceTexture resourceTexture);
        HRESULT CreateVertexBuffer(ref D2D1_VERTEX_BUFFER_PROPERTIES vertexBufferProperties, ref Guid resourceId, ref D2D1_CUSTOM_VERTEX_BUFFER_PROPERTIES customVertexBufferProperties, out ID2D1VertexBuffer buffer);
        HRESULT FindVertexBuffer(ref Guid resourceId, out ID2D1VertexBuffer buffer);
        HRESULT CreateColorContext(D2D1_COLOR_SPACE space, IntPtr profile, uint profileSize, out ID2D1ColorContext colorContext);
        HRESULT CreateColorContextFromFilename(string filename, out ID2D1ColorContext colorContext);
        HRESULT CreateColorContextFromWicColorContext(IWICColorContext wicColorContext, out ID2D1ColorContext colorContext);
        HRESULT CheckFeatureSupport(D2D1_FEATURE feature, out IntPtr featureSupportData, uint featureSupportDataSize);
        bool IsBufferPrecisionSupported(D2D1_BUFFER_PRECISION bufferPrecision);        
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D2D1_POINT_2L
    {
        public long  x;
        public long y;

        public D2D1_POINT_2L(long x, long y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [ComImport]
    [Guid("3fe6adea-7643-4f53-bd14-a0ce63f24042")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ID2D1OffsetTransform : ID2D1TransformNode
    {
        #region <ID2D1TransformNode>
        new uint GetInputCount();
        #endregion

        void SetOffset(D2D1_POINT_2L offset);
        D2D1_POINT_2L GetOffset();
    }



    // incomplete : d2d1effectauthor.h...
}
