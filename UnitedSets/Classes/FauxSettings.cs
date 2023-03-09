using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
#if UNPKG

namespace UnitedSets.Classes {
	internal class FauxSettings {

		protected const string settings_filename = "our_settings.json";
		public FauxSettings() {
			if (File.Exists(SettingsFilePortable)) {
				SettingsFile = SettingsFilePortable;
				LoadSettings(SettingsFilePortable);
			} else if (File.Exists(SettingsFileNormal)) {
				SettingsFile = SettingsFileNormal;
				LoadSettings(SettingsFilePortable);
			}
			Values.SettingChanged += Values_SettingChanged;
		}

		private async void Values_SettingChanged(object? sender, string e) {
			var us = CurSaving = new object();
			await System.Threading.Tasks.Task.Delay(500);
			if (us == CurSaving)
				SaveSettings();
		}
		private object CurSaving;
		private void LoadSettings(string file) {
			try {
				var dict = JsonSerializer.Deserialize<ConcurrentDictionary<string, FauxValues.DataVal>>(File.ReadAllText(file));
				if (dict == null)
					return;
				Values.dict = dict;
			} catch { }
		}
		public void SaveSettings() {
			SettingsFile ??= SettingsFilePortable;//for now we will ust always default to portable if normal does not exist unless we cnanot write

			var serialized = JsonSerializer.Serialize(Values.dict);
			try {
				File.WriteAllText(SettingsFile, serialized);
			} catch (Exception) {
				if (SettingsFile != SettingsFileNormal) {
					SettingsFile = SettingsFileNormal;
					Directory.CreateDirectory(SettingsFileNormal);
					SaveSettings();
				}

			}


		}
		private string SettingsFile;
		private string SettingsFilePortable => System.IO.Path.Combine(Path, settings_filename);
		private string SettingsFileNormal => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, settings_filename);
		public FauxSettings Current => this;
		public FauxSettings InstalledLocation => this;
		public string Path => System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

		public class FauxValues {
			public class DataVal {
				public string type { get; set; }
				public object value { get; set; }
			}
			public ConcurrentDictionary<string, DataVal> dict = new();
			public object this[string key] {
				get {
					if (dict.TryGetValue(key, out var value) && value != null) {
						if (value.value is JsonElement je)
							value.value = je.Deserialize(Type.GetType(value.type));
						return value.value;
					}
					dict[key] = null;
					return null;
				}
				set {
					var orig = this[key];
					if (orig == value)
						return;
					dict[key] = new DataVal { type = value.GetType().FullName, value = value };
					SettingChanged?.Invoke(this, key);
				}
			}
			public event EventHandler<string> SettingChanged;
		}
		public FauxValues Values = new();
	}
}
#endif
