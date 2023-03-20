using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace UnitedSets.Helpers {
	internal class ExternalProcessHelper {
		public static string? GetProcessCommandLineByPID(string pid) {
			//if we really want to read cwd https://stackoverflow.com/questions/16110936/read-other-process-current-directory-in-c-sharp/23842609#23842609
			using var searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + pid);
			using var objects = searcher.Get();
			return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
		}

		public static (string filename, string args) ParseCmdLine(string cmd_str) {
			if (String.IsNullOrWhiteSpace(cmd_str))
				return ("", "");
			cmd_str = cmd_str.Trim();
			try {
				if (cmd_str?.Length < 2 || File.Exists(cmd_str) || Directory.Exists(cmd_str))
					return (cmd_str.Trim('"'), "");
			} catch { }//invalid path and such

			string fileName = cmd_str!;
			string arguments = "";


			if (cmd_str!.StartsWith('"')) {
				int closingDoubleQuotePosition = cmd_str.IndexOf('"', 1);
				if (closingDoubleQuotePosition > 0 && cmd_str.Length > closingDoubleQuotePosition + 1) {
					fileName = cmd_str.Substring(0, closingDoubleQuotePosition + 1).Trim();
					arguments = cmd_str.Substring(closingDoubleQuotePosition + 1).Trim();
				}
			} else {
				int firstSpacePosition = cmd_str.IndexOf(@" ");

				if (firstSpacePosition > 0 && cmd_str.Length > firstSpacePosition + 1) {
					fileName = cmd_str.Substring(0, firstSpacePosition + 1).Trim();
					arguments = cmd_str.Substring(firstSpacePosition + 1).Trim();
				}
			}


			return (fileName!.Trim('"'), arguments);
		}
	}
}
