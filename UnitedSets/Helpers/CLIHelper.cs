using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitedSets.Classes
{
    static class CLI
    {
		private static string[]? args;
		public static bool GetFlag(String name) {
			args ??= Environment.GetCommandLineArgs();
			return ArgPosition(name) != -1;
		}
		public static string? GetVal(String name) {
			args ??= Environment.GetCommandLineArgs();
			var pos = ArgPosition(name);
			if (pos == -1 || args.Length == pos)
				return null;
			return args[pos + 1];
		}
		private static int ArgPosition(String name) {
			return args.FirstMatch(a => a.StartsWith($"--{name}",StringComparison.OrdinalIgnoreCase));
		}
		public static IEnumerable<string> GetArrVal(String name) {
			var ret = new List<string>();
			var including = false;
			args ??= Environment.GetCommandLineArgs();
			foreach (var arg in args) {
				if (arg.StartsWith("--")) {
					including = arg.Substring(2).Equals(name, StringComparison.CurrentCultureIgnoreCase);
					continue;
				}
				if (including)
					ret.Add(arg);
			}
			return ret;
		}
		private static int FirstMatch<T>(this IEnumerable<T> items, Func<T, bool> cond) {
			var i = 0;
			foreach (var item in items) {
				if (cond.Invoke(item))
					return i;
				i++;
			}
			return -1;
		}

	}
}
