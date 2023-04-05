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
using UnitedSets.Helpers;
using System.IO;
using Microsoft.Win32;

namespace UnitedSets.Classes {
	public interface IHwndHostParent {
		TabBase Tab { get; }
	}
	public partial class OurHwndHost : INotifyPropertyChanged {
		public event PropertyChangedEventHandler? PropertyChanged;
		protected HwndHost host;

		public HwndHost HwndHostForRenderBinding => host;
		public IHwndHostParent parent { get; }

		public OurHwndHost(IHwndHostParent parent, Window XAMLWindow, WindowEx? WindowToHost) {
			if (parent == null || XAMLWindow == null || WindowToHost == null)
				throw new ArgumentNullException();
			host = new HwndHost(XAMLWindow, (WindowEx)WindowToHost);
			this.parent = parent;
			host.Closed += Host_Closed;
		}

		public string GetTitle() => host.HostedWindow.TitleText;
		public string? GetOwnerProcessModuleFilename() => GetOwnerWindow().OwnerProcess.GetDotNetProcess.MainModule?.FileName;
		/// <summary>
		/// Work around WinUI/UWP as AppFrameHost is normally the owner but we want the actual app
		/// </summary>
		/// <returns></returns>
		protected WindowEx GetOwnerWindow(out bool wasUwp) {
			var owner = host.HostedWindow;
			wasUwp = false;
			var mainModulePath = owner.OwnerProcess.GetDotNetProcess.MainModule?.FileName ?? "";
			if (mainModulePath?.Equals(System.IO.Path.Combine(Environment.SystemDirectory, "ApplicationFrameHost.exe"), StringComparison.CurrentCultureIgnoreCase) != true) {
				wasUwp = mainModulePath!.Contains(WindowsAppFolder ?? LoadWindowsAppFolder(), StringComparison.CurrentCultureIgnoreCase);//some windows apps dont use appframehost, IE windows terminal
					
				return owner;
			}
			wasUwp = true;
			var child = GetCoreWindowFromAppHostWindow(host.HostedWindow);
			return child;
		}
		public static WindowEx GetCoreWindowFromAppHostWindow(WindowEx appFrameHostMainWindow) => appFrameHostMainWindow.Children.FirstOrDefault(x => x.ClassName is "Windows.UI.Core.CoreWindow", appFrameHostMainWindow);
		private static string? WindowsAppFolder = null;
		private static string LoadWindowsAppFolder() {
			if (WindowsAppFolder == null) {
				using var appx = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Appx");
				WindowsAppFolder = appx?.GetValue("PackageRoot")?.ToString() ?? "invalid";
			}
			return WindowsAppFolder;
		}
		public const string OUR_WINDOWS_STORE_APP_EXEC_PREFIX = "#WindowsApp!";
		protected WindowEx GetOwnerWindow() => GetOwnerWindow(out _);
		public (string cmd, string args) GetOwnerProcessInfo() {
			var owner = GetOwnerWindow(out var wasUWP);
			var toParse = ExternalProcessHelper.GetProcessCommandLineByPID(owner.OwnerProcess.Id.ToString());
			var parsed = ExternalProcessHelper.ParseCmdLine(toParse!);
			// So to get theofficial UWP executable we should use mainModulePath, as the command line can be different for example "wt" will launch windows terminal and will show that as its process. Now we could always use mainModulePath but as we can relaunch terminal with the same command we simply can honor what it says it is.  We must disable UWP mode though in this case so if we are not under the App Folder we can assume not uwp.
			if (wasUWP && parsed.filename.Replace("\\", "/").Contains((WindowsAppFolder ?? LoadWindowsAppFolder()).Replace("\\", "/")) == false)
				wasUWP = false;
			if (wasUWP) {
				var fileInfo = new FileInfo(parsed.filename);
				var dir = fileInfo?.Directory?.Name ?? "";
				parsed.filename = $"{OUR_WINDOWS_STORE_APP_EXEC_PREFIX}{dir}";
				var serverNamePos = parsed.args.IndexOf("-ServerName:", StringComparison.CurrentCultureIgnoreCase);
				if (serverNamePos != -1) {
					var spaceAfter = parsed.args.IndexOf(" ", serverNamePos + 1);
					var part1 = parsed.args.Substring(0, serverNamePos);
					var part2 = "";
					if (spaceAfter != -1)
						part2 = parsed.args.Substring(spaceAfter + 1);//should have a space before servername anyway
					parsed.args = part1 + part2;
				}
			}
			return parsed;
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

		[AutoNotifyProperty(OnChanged = nameof(ActivateCropChanged))]
		private bool _ActivateCrop;
		private void ActivateCropChanged() => host.ActivateCrop = _ActivateCrop;

		[AutoNotifyProperty(OnChanged = nameof(BorderlessWindowChanged))]
		private bool _BorderlessWindow;
		private void BorderlessWindowChanged() => host.BorderlessWindow = _BorderlessWindow;

		private void RaisePropertyChanged(params string[] properties) {
			foreach (var prop in properties)
				PropertyChanged?.Invoke(this, new(prop));
		}
		public void ClearCrop() {
			_CropTop = _CropBottom = _CropLeft = _CropRight = 0;
			RaisePropertyChanged(nameof(_CropTop), nameof(_CropBottom), nameof(_CropLeft), nameof(_CropRight));
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
			if (!_clean_close)
				Closed?.Invoke(this, EventArgs.Empty);
			_closed = true;
		}
		private bool _closed = false;
		private bool _clean_close = false;
		public event EventHandler? Closed;
		public event EventHandler? Detached;
		public void SetBorderless(bool borderless) => BorderlessWindow = borderless;


		private object? CurFix;
		private async void DelaySizeFix() {
			var us = CurFix = new();
			await Task.Delay(700);
			if (us != CurFix)
				return;
			if (host.MayBeSizeBug) {
				System.Diagnostics.Debug.WriteLine($"Warning for the host: {host} had to fix the size as think we had a size bug is win visible: {host.Visibility}.");
				host.FixSizeBug();
			}
		}
		public void SetVisible(bool visible, bool FocusOnVisible = true) {
			host.IsWindowVisible = visible;
			if (visible && FocusOnVisible)
				host.FocusWindow();
			if (visible) {
				if (host.MayBeSizeBug)
					DelaySizeFix();

			}
		}

	}
}
