using System;
using System.Runtime.InteropServices;
using Direct2D;
using DXGI;
using GlobalStructures;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using static DXGI.DXGITools;
using Microsoft.UI.Xaml.Media;
using GDIPlus;
using static GDIPlus.GDIPlusTools;
using Microsoft.Win32;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
//This is mostly from the content in the issue report  https://github.com/microsoft/microsoft-ui-xaml/issues/1247  with some detailed work in the SwapChain items and a management class

namespace TransparentWinUIWindowLib {
	public class TransparentWindowManager {
		[ComImport, Guid("63aad0b8-7c24-40ff-85a8-640d944cc325"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface ISwapChainPanelNative {
			[PreserveSig]
			HRESULT SetSwapChain(IDXGISwapChain swapChain);
		}

		public const uint LWA_COLORKEY = 0x00000001;
		public const uint LWA_ALPHA = 0x00000002;

		[DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

		[DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, IntPtr pptDst, IntPtr psize, IntPtr hdcSrc, IntPtr pprSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct BLENDFUNCTION {
			public byte BlendOp;
			public byte BlendFlags;
			public byte SourceConstantAlpha;
			public byte AlphaFormat;
		}

		public const byte AC_SRC_OVER = 0x00;
		public const byte AC_SRC_ALPHA = 0x01;

		public const int ULW_COLORKEY = 0x00000001;
		public const int ULW_ALPHA = 0x00000002;
		public const int ULW_OPAQUE = 0x00000004;

		[DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern int GetObject(IntPtr hFont, int nSize, out BITMAP bm);

		[StructLayoutAttribute(LayoutKind.Sequential)]
		public struct BITMAP {
			public int bmType;
			public int bmWidth;
			public int bmHeight;
			public int bmWidthBytes;
			public short bmPlanes;
			public short bmBitsPixel;
			public IntPtr bmBits;
		}

		[DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

		[DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

		[DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

		[DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern bool DeleteObject(IntPtr hObject);

		[DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern bool DeleteDC(IntPtr hdc);

		[DllImport("User32.dll", SetLastError = true)]
		public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		const int GWL_STYLE = (-16);
		const int GWL_EXSTYLE = (-20);
		public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong) {
			if (IntPtr.Size == 4) {
				return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
			}
			return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
		}

		[DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
		public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
		public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		public static long GetWindowLong(IntPtr hWnd, int nIndex) {
			if (IntPtr.Size == 4) {
				return GetWindowLong32(hWnd, nIndex);
			}
			return GetWindowLongPtr64(hWnd, nIndex);
		}

		[DllImport("User32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
		public static extern long GetWindowLong32(IntPtr hWnd, int nIndex);

		[DllImport("User32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
		public static extern long GetWindowLongPtr64(IntPtr hWnd, int nIndex);

		public const int WS_EX_LAYERED = 0x00080000;
		public const int WS_POPUP = unchecked((int)0x80000000L);
		public const int WS_VISIBLE = 0x10000000;

		[DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern bool GetCursorPos(out Windows.Graphics.PointInt32 lpPoint);

		[DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

		public const int RDW_INVALIDATE = 0x0001;
		public const int RDW_INTERNALPAINT = 0x0002;
		public const int RDW_ERASE = 0x0004;

		public const int RDW_VALIDATE = 0x0008;
		public const int RDW_NOINTERNALPAINT = 0x0010;
		public const int RDW_NOERASE = 0x0020;

		public const int RDW_NOCHILDREN = 0x0040;
		public const int RDW_ALLCHILDREN = 0x0080;

		public const int RDW_UPDATENOW = 0x0100;
		public const int RDW_ERASENOW = 0x0200;

		public const int RDW_FRAME = 0x0400;
		public const int RDW_NOFRAME = 0x0800;

		[DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		public const int SWP_NOSIZE = 0x0001;
		public const int SWP_NOMOVE = 0x0002;
		public const int SWP_NOZORDER = 0x0004;
		public const int SWP_NOREDRAW = 0x0008;
		public const int SWP_NOACTIVATE = 0x0010;
		public const int SWP_FRAMECHANGED = 0x0020;  /* The frame changed: send WM_NCCALCSIZE */
		public const int SWP_SHOWWINDOW = 0x0040;
		public const int SWP_HIDEWINDOW = 0x0080;
		public const int SWP_NOCOPYBITS = 0x0100;
		public const int SWP_NOOWNERZORDER = 0x0200;  /* Don't do owner Z ordering */
		public const int SWP_NOSENDCHANGING = 0x0400;  /* Don't send WM_WINDOWPOSCHANGING */
		public const int SWP_DRAWFRAME = SWP_FRAMECHANGED;
		public const int SWP_NOREPOSITION = SWP_NOOWNERZORDER;
		public const int SWP_DEFERERASE = 0x2000;
		public const int SWP_ASYNCWINDOWPOS = 0x4000;


		[DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);


		public enum DWMWINDOWATTRIBUTE {
			DWMWA_NCRENDERING_ENABLED = 1,
			DWMWA_NCRENDERING_POLICY,
			DWMWA_TRANSITIONS_FORCEDISABLED,
			DWMWA_ALLOW_NCPAINT,
			DWMWA_CAPTION_BUTTON_BOUNDS,
			DWMWA_NONCLIENT_RTL_LAYOUT,
			DWMWA_FORCE_ICONIC_REPRESENTATION,
			DWMWA_FLIP3D_POLICY,
			DWMWA_EXTENDED_FRAME_BOUNDS,
			DWMWA_HAS_ICONIC_BITMAP,
			DWMWA_DISALLOW_PEEK,
			DWMWA_EXCLUDED_FROM_PEEK,
			DWMWA_CLOAK,
			DWMWA_CLOAKED,
			DWMWA_FREEZE_REPRESENTATION,
			DWMWA_PASSIVE_UPDATE_MODE,
			DWMWA_USE_HOSTBACKDROPBRUSH,
			DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
			DWMWA_WINDOW_CORNER_PREFERENCE = 33,
			DWMWA_BORDER_COLOR,
			DWMWA_CAPTION_COLOR,
			DWMWA_TEXT_COLOR,
			DWMWA_VISIBLE_FRAME_BORDER_THICKNESS,
			DWMWA_SYSTEMBACKDROP_TYPE,
			DWMWA_LAST
		};

		public enum DWMNCRENDERINGPOLICY {
			DWMNCRP_USEWINDOWSTYLE, // Enable/disable non-client rendering based on window style
			DWMNCRP_DISABLED,       // Disabled non-client rendering; window style is ignored
			DWMNCRP_ENABLED,        // Enabled non-client rendering; window style is ignored
			DWMNCRP_LAST
		};

		public enum DWM_WINDOW_CORNER_PREFERENCE {
			DWMWCP_DEFAULT = 0,
			DWMWCP_DONOTROUND = 1,
			DWMWCP_ROUND = 2,
			DWMWCP_ROUNDSMALL = 3
		}

		[DllImport("Dwmapi.dll", SetLastError = true, CharSet = CharSet.Unicode, PreserveSig = false)]
		public static extern HRESULT DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute, uint cbAttribute);

		[DllImport("Dwmapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern HRESULT DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);


		private IntPtr hWndMain = IntPtr.Zero;
		private Microsoft.UI.Windowing.AppWindow _apw;
		private Microsoft.UI.Windowing.OverlappedPresenter _presenter;

		IntPtr m_initToken = IntPtr.Zero;
		IntPtr m_hBitmap = IntPtr.Zero;

		ID2D1Factory m_pD2DFactory = null;
		ID2D1Factory1 m_pD2DFactory1 = null;

		IntPtr m_pD3D11DevicePtr = IntPtr.Zero;
		ID3D11DeviceContext m_pD3D11DeviceContext = null;
		IDXGIDevice1 m_pDXGIDevice = null;

		ID2D1DeviceContext m_pD2DDeviceContext = null;

		IDXGISwapChain1 m_pDXGISwapChain1 = null;

		private SwapChainPanel swapChainPanel1;
		public TransparentWindowManager(Window window, SwapChainPanel swapChainPanel1, bool EnableDragMoveAnywhere, String background_alpha_file = null) {
			this.window = window;
			window.Closed += Window_Closed;
			this.swapChainPanel1 = swapChainPanel1;
			this.background_file = background_alpha_file;
			this.EnableDragMoveAnywhere = EnableDragMoveAnywhere;

		}
		private string? background_file;
		private bool EnableDragMoveAnywhere;
		public void AfterInitialize() {
			HRESULT hr = HRESULT.S_OK;

			hWndMain = WinRT.Interop.WindowNative.GetWindowHandle(window);
			Microsoft.UI.WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWndMain);
			_apw = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(myWndId);

			_presenter = _apw.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
			//_presenter.IsResizable = true;
			_presenter.SetBorderAndTitleBar(false, false);

			// Update for Windows 11 from michalleptuch comment
			// otherwise there are borders + shadow from his test
			// Returns logically 0x80070057 (E_INVALIDARG) on Windows 10
			int nValue = (int)DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DEFAULT;
			hr = DwmSetWindowAttribute(hWndMain, (int)DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref nValue, Marshal.SizeOf(typeof(int)));
			

			StartupInput input = StartupInput.GetDefault();
			StartupOutput output;
			GpStatus nStatus = GdiplusStartup(out m_initToken, ref input, out output);

			IntPtr pImage = IntPtr.Zero;
			if (background_file != null) {
				nStatus = GdipCreateBitmapFromFile(background_file, out pImage);
				if (nStatus == GpStatus.Ok) {
					GdipCreateHBITMAPFromBitmap(pImage, out m_hBitmap, RGB(Microsoft.UI.Colors.Black.R, Microsoft.UI.Colors.Black.G, Microsoft.UI.Colors.Black.B));
					GdipDisposeImage(pImage);
				}
			}

			hr = CreateD2D1Factory();
			if (hr == HRESULT.S_OK) {
				hr = CreateDeviceContext();
				hr = CreateSwapChain(IntPtr.Zero);
				if (hr == HRESULT.S_OK) {
					ISwapChainPanelNative panelNative = WinRT.CastExtensions.As<ISwapChainPanelNative>(swapChainPanel1);
					hr = panelNative.SetSwapChain(m_pDXGISwapChain1);
				}
			}
			//GetAlphaChromaKeyColor();
			long nExStyle = GetWindowLong(hWndMain, GWL_EXSTYLE);
			if ((nExStyle & WS_EX_LAYERED) == 0) {
				SetWindowLong(hWndMain, GWL_EXSTYLE, (IntPtr)(nExStyle | WS_EX_LAYERED));

				//bool bReturn = SetLayeredWindowAttributes(hWndMain, nAlphaBackgroundColor, 255, LWA_COLORKEY); // NOTE this only applies to GDI drawn items which most of WinUI is not,  this will also  transparent out the titlebar which also hittests through then so generrally not what you want (plus swindow border by default is not transparent).
			}
			if (EnableDragMoveAnywhere) {
				UIElement root = (UIElement)window.Content;

				root.PointerMoved += Root_PointerMoved;
				root.PointerPressed += Root_PointerPressed;
				root.PointerReleased += Root_PointerReleased;
			}
		}

		bool bSet = false;
		private void GetAlphaChromaKeyColor() {
			int nAppsUseLightTheme = 0;
			int nSystemUsesLightTheme = 0;
			string sPathKey = @"Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";
			using (RegistryKey rkLocal = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)) {
				using (RegistryKey rk = rkLocal.OpenSubKey(sPathKey, false)) {
					nAppsUseLightTheme = (int)rk.GetValue("AppsUseLightTheme", 0);
					nSystemUsesLightTheme = (int)rk.GetValue("SystemUsesLightTheme", 0);
				}
			}
			nAlphaBackgroundColor = (uint)System.Drawing.ColorTranslator.ToWin32(System.Drawing.Color.Black);
			if (nAppsUseLightTheme == 1)
				nAlphaBackgroundColor = (uint)System.Drawing.ColorTranslator.ToWin32(System.Drawing.Color.White);
		}
		private uint nAlphaBackgroundColor;

		public void RemoveBorderSetTransparentMap() {

			if (!bSet && m_hBitmap != IntPtr.Zero) {
				SetWindowLong(hWndMain, GWL_EXSTYLE, (IntPtr)(GetWindowLong(hWndMain, GWL_EXSTYLE) & ~WS_EX_LAYERED));
				RedrawWindow(hWndMain, IntPtr.Zero, IntPtr.Zero, RDW_ERASE | RDW_INVALIDATE | RDW_FRAME | RDW_ALLCHILDREN);
				SetWindowLong(hWndMain, GWL_EXSTYLE, (IntPtr)(GetWindowLong(hWndMain, GWL_EXSTYLE) | WS_EX_LAYERED));

					SetPictureToLayeredWindow(hWndMain, m_hBitmap);
					//bool bReturn = SetLayeredWindowAttributes(hWndMain, nAlphaBackgroundColor, 0, LWA_COLORKEY);
					//if (bReturn != true)
					//	throw new Exception("Unable to set window attribs");
				//}

				GetWindowRect(hWndMain, out var rectWnd);
				
				//PInvoke.SetLayeredWindowAttributes((HWND)WindowHandle, new COLORREF(it), 0, LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_COLORKEY);

				SetWindowPos(hWndMain, IntPtr.Zero, rectWnd.left, rectWnd.top - 1, 0, 0, SWP_NOSIZE | SWP_NOZORDER | SWP_SHOWWINDOW | SWP_FRAMECHANGED);
				SetWindowPos(hWndMain, IntPtr.Zero, rectWnd.left, rectWnd.top, 0, 0, SWP_NOSIZE | SWP_NOZORDER | SWP_SHOWWINDOW | SWP_FRAMECHANGED);

				bSet = true;
			}
		}





		private void Window_Closed(object sender, WindowEventArgs args) {
			Cleanup();
		}
		private int nX = 0, nY = 0, nXWindow = 0, nYWindow = 0;
		private bool bMoving = false;

		private void Root_PointerReleased(object sender, PointerRoutedEventArgs e) {
			((UIElement)sender).ReleasePointerCaptures();
			bMoving = false;
		}

		private void Root_PointerPressed(object sender, PointerRoutedEventArgs e) {
			var properties = e.GetCurrentPoint((UIElement)sender).Properties;
			if (properties.IsLeftButtonPressed) {
				((UIElement)sender).CapturePointer(e.Pointer);
				nXWindow = _apw.Position.X;
				nYWindow = _apw.Position.Y;
				Windows.Graphics.PointInt32 pt;
				GetCursorPos(out pt);
				nX = pt.X;
				nY = pt.Y;
				bMoving = true;
			}
		}

		private void Root_PointerMoved(object sender, PointerRoutedEventArgs e) {
			var properties = e.GetCurrentPoint((UIElement)sender).Properties;
			if (properties.IsLeftButtonPressed) {
				Windows.Graphics.PointInt32 pt;
				GetCursorPos(out pt);
				if (bMoving)
					_apw.Move(new Windows.Graphics.PointInt32(nXWindow + (pt.X - nX), nYWindow + (pt.Y - nY)));
				e.Handled = true;
			}
		}

		public HRESULT CreateD2D1Factory() {
			HRESULT hr = HRESULT.S_OK;
			D2D1_FACTORY_OPTIONS options = new D2D1_FACTORY_OPTIONS();

			options.debugLevel = D2D1_DEBUG_LEVEL.D2D1_DEBUG_LEVEL_INFORMATION;

			hr = D2DTools.D2D1CreateFactory(D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_SINGLE_THREADED, ref D2DTools.CLSID_D2D1Factory, ref options, out m_pD2DFactory);
			m_pD2DFactory1 = (ID2D1Factory1)m_pD2DFactory;
			return hr;
		}

		public HRESULT CreateDeviceContext() {
			HRESULT hr = HRESULT.S_OK;
			uint creationFlags = (uint)D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_BGRA_SUPPORT;

			creationFlags |= (uint)D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_DEBUG;

			int[] aD3D_FEATURE_LEVEL = new int[] { (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1, (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0,
				(int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_1, (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_0, (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_3,
				(int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_2, (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_1};

			D3D_FEATURE_LEVEL featureLevel;
			hr = D2DTools.D3D11CreateDevice(null,    // specify null to use the default adapter
				D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE,
				IntPtr.Zero,
				creationFlags,      // optionally set debug and Direct2D compatibility flags
				aD3D_FEATURE_LEVEL, // list of feature levels this app can support
									// (uint)Marshal.SizeOf(aD3D_FEATURE_LEVEL),   // number of possible feature levels
				(uint)aD3D_FEATURE_LEVEL.Length, // number of possible feature levels
				D2DTools.D3D11_SDK_VERSION,
				out m_pD3D11DevicePtr,    // returns the Direct3D device created
				out featureLevel,         // returns feature level of device created            
				out m_pD3D11DeviceContext // returns the device immediate context
			);
			if (hr == HRESULT.S_OK) {
				m_pDXGIDevice = Marshal.GetObjectForIUnknown(m_pD3D11DevicePtr) as IDXGIDevice1;
				if (m_pD2DFactory1 != null) {
					ID2D1Device pD2DDevice = null;
					hr = m_pD2DFactory1.CreateDevice(m_pDXGIDevice, out pD2DDevice);
					if (hr == HRESULT.S_OK) {
						hr = pD2DDevice.CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS.D2D1_DEVICE_CONTEXT_OPTIONS_NONE, out m_pD2DDeviceContext);
						GlobalTools.SafeRelease(ref pD2DDevice);
					}
				}
			}
			return hr;
		}

		HRESULT CreateSwapChain(IntPtr hWnd) {
			HRESULT hr = HRESULT.S_OK;
			DXGI_SWAP_CHAIN_DESC1 swapChainDesc = new DXGI_SWAP_CHAIN_DESC1();
			swapChainDesc.Width = 1;
			swapChainDesc.Height = 1;
			swapChainDesc.Format = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM; // this is the most common swapchain format
			swapChainDesc.Stereo = false;
			swapChainDesc.SampleDesc.Count = 1;                // don't use multi-sampling
			swapChainDesc.SampleDesc.Quality = 0;
			swapChainDesc.BufferUsage = D2DTools.DXGI_USAGE_RENDER_TARGET_OUTPUT;
			swapChainDesc.BufferCount = 2;                     // use double buffering to enable flip
			swapChainDesc.Scaling = (hWnd != IntPtr.Zero) ? DXGI_SCALING.DXGI_SCALING_NONE : DXGI_SCALING.DXGI_SCALING_STRETCH;
			swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL; // all apps must use this SwapEffect       
			swapChainDesc.Flags = 0;

			IDXGIAdapter pDXGIAdapter;
			hr = m_pDXGIDevice.GetAdapter(out pDXGIAdapter);
			if (hr == HRESULT.S_OK) {
				IntPtr pDXGIFactory2Ptr;
				hr = pDXGIAdapter.GetParent(typeof(IDXGIFactory2).GUID, out pDXGIFactory2Ptr);
				if (hr == HRESULT.S_OK) {
					IDXGIFactory2 pDXGIFactory2 = Marshal.GetObjectForIUnknown(pDXGIFactory2Ptr) as IDXGIFactory2;
					if (hWnd != IntPtr.Zero)
						hr = pDXGIFactory2.CreateSwapChainForHwnd(m_pD3D11DevicePtr, hWnd, ref swapChainDesc, IntPtr.Zero, null, out m_pDXGISwapChain1);
					else
						hr = pDXGIFactory2.CreateSwapChainForComposition(m_pD3D11DevicePtr, ref swapChainDesc, null, out m_pDXGISwapChain1);

					hr = m_pDXGIDevice.SetMaximumFrameLatency(1);
					GlobalTools.SafeRelease(ref pDXGIFactory2);
					Marshal.Release(pDXGIFactory2Ptr);
				}
				GlobalTools.SafeRelease(ref pDXGIAdapter);
			}
			return hr;
		}

		private bool SetPictureToLayeredWindow(IntPtr hWnd, IntPtr hBitmap) {
			BITMAP bm;
			GetObject(hBitmap, Marshal.SizeOf(typeof(BITMAP)), out bm);
			System.Drawing.Size sizeBitmap = new System.Drawing.Size(bm.bmWidth, bm.bmHeight);

			IntPtr hDCScreen = GetDC(IntPtr.Zero);
			IntPtr hDCMem = CreateCompatibleDC(hDCScreen);
			IntPtr hBitmapOld = SelectObject(hDCMem, hBitmap);

			BLENDFUNCTION bf = new BLENDFUNCTION();
			bf.BlendOp = AC_SRC_OVER;
			bf.SourceConstantAlpha = 255;
			bf.AlphaFormat = AC_SRC_ALPHA;

			RECT rectWnd;
			GetWindowRect(hWnd, out rectWnd);

			System.Drawing.Point ptSrc = new System.Drawing.Point();
			System.Drawing.Point ptDest = new System.Drawing.Point(rectWnd.left, rectWnd.top);

			IntPtr pptSrc = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(System.Drawing.Point)));
			Marshal.StructureToPtr(ptSrc, pptSrc, false);

			IntPtr pptDest = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(System.Drawing.Point)));
			Marshal.StructureToPtr(ptDest, pptDest, false);

			IntPtr psizeBitmap = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(System.Drawing.Size)));
			Marshal.StructureToPtr(sizeBitmap, psizeBitmap, false);

			bool bRet = UpdateLayeredWindow(hWnd, hDCScreen, pptDest, psizeBitmap, hDCMem, pptSrc, 0, ref bf, ULW_ALPHA);

			Marshal.FreeHGlobal(pptSrc);
			Marshal.FreeHGlobal(pptDest);
			Marshal.FreeHGlobal(psizeBitmap);

			SelectObject(hDCMem, hBitmapOld);
			DeleteDC(hDCMem);
			ReleaseDC(IntPtr.Zero, hDCScreen);

			return bRet;
		}

		private Window window;
		private volatile bool cleaned = false;
		public void Cleanup() {
			if (cleaned)
				return;
			cleaned = true;
			GlobalTools.SafeRelease(ref m_pD2DDeviceContext);
			GlobalTools.SafeRelease(ref m_pDXGISwapChain1);

			GlobalTools.SafeRelease(ref m_pDXGIDevice);
			GlobalTools.SafeRelease(ref m_pD3D11DeviceContext);
			Marshal.Release(m_pD3D11DevicePtr);

			GlobalTools.SafeRelease(ref m_pD2DFactory1);
			GlobalTools.SafeRelease(ref m_pD2DFactory);

			GdiplusShutdown(m_initToken);
		}
	}
}
