using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Get.EasyCSharp;
using Windows.Storage.Pickers;
using WindowsOG = Windows;

namespace UnitedSets.Configurations;

public partial class ExportImportInputViewModel : INotifyPropertyChanged {
		[AutoNotifyProperty]
		string? _Filename;

		public string? FullFilename {
			get {
				var ret = _Filename;
				if (String.IsNullOrWhiteSpace(ret))
					return ret;
				if (!Path.IsPathRooted(ret))
					ret = Path.Combine(USConfig.BaseProfileFolder, ret);
				if (!Path.HasExtension(ret))
					ret += ".json";
				return ret;
			}
		}

		private static bool firstTime = true;
		public IntPtr hWnd = IntPtr.Zero;
		[RelayCommand]
		public async Task FileBrowse() {
			PickerLocationId? loc = null;
			if (firstTime)
				loc = PickerLocationId.DocumentsLibrary;//stupid winui hahahahaha lemme give you a folder nt or i shall martial a real picker in here
			firstTime = false;

			WindowsOG.Storage.StorageFile res;



			if (SaveNotLoad) {
				var picker = new FileSavePicker();
				if (loc != null)
					picker.SuggestedStartLocation = loc.Value;
				WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);
				picker.FileTypeChoices.Add("United Sets Config", new List<string>() { ".json" });
				res = await picker.PickSaveFileAsync();
			}else{
				var picker = new FileOpenPicker();
				if (loc != null)
					picker.SuggestedStartLocation = loc.Value;
				WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);
				picker.ViewMode = PickerViewMode.List;
				picker.FileTypeFilter.Add(".json");
				res = await picker.PickSingleFileAsync();
			}



			if (res != null && String.IsNullOrWhiteSpace(res.Path) == false) {
				Filename = res.Path;
				//OverrideAsSuccess = true;
				//RequestClose?.Invoke(this,EventArgs.Empty);
			}

			
			//var savePicker = new FileSavePicker();
		}
#pragma warning disable CS0067 // The event 'ExportImportInputViewModel.RequestClose' is never used
		public event EventHandler? RequestClose;
#pragma warning restore CS0067 // The event 'ExportImportInputViewModel.RequestClose' is never used
		public string AcceptFileButtonText => SaveNotLoad ? "Export Config" : "Import Config";

		[AutoNotifyProperty]
		bool _SaveNotLoad;



		[AutoNotifyProperty]
		bool _OnlyExportNonDefault=true;


		public bool OverrideAsSuccess = false;

		public event PropertyChangedEventHandler? PropertyChanged;
	}
