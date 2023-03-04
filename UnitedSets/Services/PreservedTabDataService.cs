using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Foundation;

namespace UnitedSets.Services {
	public class PreservedTabData {
		public PreservedTabData(IntPtr HwndID, uint pid) {
			this.HwndID = HwndID;
			this.pid = pid;
		}
		public IntPtr HwndID;
		public uint pid;
		public DateTime LastDetached = DateTime.Now;

		public string CustomTitle;
		public bool Borderless;
		public bool CropEnabled;
		public CropRect CropRect;

	}
	public class CropRect {
		public int Left; public int Top; public int Right; public int Bottom;
		public CropRect(int left, int top, int right, int bottom) {
			this.Left = left;
			this.Top = top;
			this.Right = right;
			this.Bottom = bottom;
		}
	}
	internal class PreservedTabDataService {
		public ConcurrentDictionary<IntPtr, PreservedTabData> TabData=new();
		public void SaveTab(PreservedTabData data) {
			if (data.Borderless || data.CropEnabled || String.IsNullOrWhiteSpace( data.CustomTitle) == false)//only save if we have actual data--=-=-=
				TabData[data.HwndID] = data;
		}
		public PreservedTabData? LookupTab(uint pid, IntPtr hwnd) {
			if (!TabData.TryGetValue(hwnd, out var data))
				return null;
			if (data.pid != pid) {
				TabData.TryRemove(hwnd, out _);
				return null;
			}
			return data;
		}
	}
}
