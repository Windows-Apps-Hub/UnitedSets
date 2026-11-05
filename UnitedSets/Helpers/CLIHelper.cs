namespace UnitedSets.Helpers;

/// <summary>
/// Command line arguments. <see cref="Args"/> defaults to the command line of this process, but can be replaced
/// by the arguments of another launch that was redirected to this instance.
/// </summary>
static class CLI
{
	private static string[]? args;
    static string[] Args => args ??= Environment.GetCommandLineArgs();
    /// <summary>
    /// Uses the arguments of another launch (redirected to this instance)
    /// </summary>
    public static void SetArgs(string[] newArgs) => args = newArgs;
	public static bool GetFlag(string name) {
		return ArgPosition(name) != -1;
	}
	public static string? GetVal(string name) {
		var pos = ArgPosition(name);
        // (used to check "args.Length == pos", which threw when the flag was the last argument)
		if (pos == -1 || pos + 1 >= Args.Length)
			return null;
		return Args[pos + 1];
	}
	private static int ArgPosition(string name) {
		return Args.FirstMatch(a => a.StartsWith($"--{name}",StringComparison.OrdinalIgnoreCase));
	}
	public static IEnumerable<string> GetArrVal(string name) {
		var ret = new List<string>();
		var including = false;
		foreach (var arg in Args) {
			if (arg.StartsWith("--")) {
				including = arg[2..].Equals(name, StringComparison.CurrentCultureIgnoreCase);
				continue;
			}
			if (including)
				ret.Add(arg);
		}
		return ret;
	}
    /// <summary>
    /// Splits a command line string into arguments (supports double quotes).
    /// </summary>
    public static string[] Split(string commandLine)
    {
        var result = new List<string>();
        var current = new System.Text.StringBuilder();
        bool inQuotes = false, hasToken = false;
        foreach (var c in commandLine)
        {
            if (c == '"') { inQuotes = !inQuotes; hasToken = true; continue; }
            if (char.IsWhiteSpace(c) && !inQuotes)
            {
                if (hasToken) { result.Add(current.ToString()); current.Clear(); hasToken = false; }
                continue;
            }
            current.Append(c);
            hasToken = true;
        }
        if (hasToken) result.Add(current.ToString());
        return [.. result];
    }
	private static int FirstMatch<T>(this IEnumerable<T>? items, Func<T, bool> cond) {
		if (items is null) goto Fail;
		var i = 0;
		foreach (var item in items) {
			if (cond.Invoke(item))
				return i;
			i++;
		}
	Fail:
		return -1;
	}

}
