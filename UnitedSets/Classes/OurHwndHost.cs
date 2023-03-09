using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using UnitedSets.Classes.Tabs;
using WinUI3HwndHostPlus;
using WindowEx = WinWrapper.Window;

using EasyCSharp;

namespace UnitedSets.Classes {
	public interface IHwndHostParent {
		TabBase Tab { get; }
	}
	public partial class OurHwndHost : INotifyPropertyChanged {
		public event PropertyChangedEventHandler? PropertyChanged;
		protected HwndHost host;

		public HwndHost HwndHostForRenderBinding => host;
		public IHwndHostParent parent { get; }

		public OurHwndHost(IHwndHostParent parent, Window XAMLWindow, WindowEx WindowToHost) {
			host = new HwndHost(XAMLWindow, WindowToHost);
			this.parent = parent;
			host.Closed += Host_Closed;
		}

		public string GetTitle() => host.HostedWindow.TitleText;
		public string? GetOwnerProcessModuleFilename() {
			var FileName = host.HostedWindow.OwnerProcess.GetDotNetProcess.MainModule?.FileName;
			if (FileName == @"C:\WINDOWS\system32\ApplicationFrameHost.exe") {
				var child = host.HostedWindow.Children.FirstOrDefault(x =>
					x.ClassName is "Windows.UI.Core.CoreWindow", host.HostedWindow);
				FileName = child.OwnerProcess.GetDotNetProcess.MainModule?.FileName;
			}
			return FileName;
		}

		[AutoNotifyProperty(OnChanged = nameof(CropTopChanged))]
		int _CropTop;
		private void CropTopChanged() => host.CropTop = _CropTop;
		[AutoNotifyProperty(OnChanged = nameof(CropLeftChanged))]
		private int _CropLeft;
		private void CropLeftChanged() => host.CropLeft = _CropLeft;
		[AutoNotifyProperty(OnChanged = nameof(CropRightChanged))]
		private int _CropRight;
		private void CropRightChanged() => host.CropRight = _CropRight;
		[AutoNotifyProperty(OnChanged = nameof(CropBottomChanged))]
		private int _CropBottom;
		private void CropBottomChanged() => host.CropBottom = _CropBottom;

		[AutoNotifyProperty(OnChanged=nameof(ActivateCropChanged))]
		private bool _ActivateCrop;
		private void ActivateCropChanged() => host.ActivateCrop = _ActivateCrop;

		[AutoNotifyProperty(OnChanged =nameof(BorderlessWindowChanged))]
		private bool _BorderlessWindow;
		private void BorderlessWindowChanged() => host.BorderlessWindow = _BorderlessWindow;

		private void RaisePropertyChanged(params string[] properties) {
			foreach (var prop in properties)
				PropertyChanged?.Invoke(this, new(prop));
		}
		public void ClearCrop() {
			_CropTop = _CropBottom = _CropLeft = _CropRight = 0;
			RaisePropertyChanged(nameof(_CropTop),nameof(_CropBottom),nameof(_CropLeft),nameof(_CropRight));
			host.ClearCrop();
		}

		public async Task DetachAndDispose(bool Focus = true) {
			await host.DetachAndDispose(Focus);
			Detached?.Invoke(this, EventArgs.Empty);
		}
		public bool NoMoving => host.NoMovingMode;
		public bool IsOwnerSetSuccessful => host.IsOwnerSetSuccessful;
		public async Task Close() {
			_clean_close = true;
			await host.HostedWindow.TryCloseAsync();
			_closed = true;
			Closed?.Invoke(this, EventArgs.Empty);
		}
		public bool IsWindowStillValid() {
			if (host.HostedWindow.IsValid)
				return true;
			if (!_closed)
				Host_Closed();
			return false;
		}
		private void Host_Closed() {//this should only get called if the window handle is no longer valid or we called Close
			if (_closed)
                System.Diagnostics.Debug.WriteLine("OurHwndHost::Host_Closed: Already closed??");
			if (! _clean_close)
				Closed?.Invoke(this, EventArgs.Empty);
			_closed = true;
		}
		private bool _closed;
		private bool _clean_close;
		public event EventHandler Closed;
		public event EventHandler Detached;
		public void SetBorderless(bool borderless) => BorderlessWindow = borderless;

		public void SetVisible(bool visible, bool FocusOnVisible=true) {
			host.IsWindowVisible = visible;
			if (visible && FocusOnVisible)
				host.FocusWindow();
		}
		
	}
}
