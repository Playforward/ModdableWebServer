using System;
using System.Collections.Generic;
using System.Linq;

namespace ModdableWebServer.Helper;

public static class UrlHelper
{
	public static string ReasonFail { get; internal set; } = string.Empty;

	public static bool Match(string url, string pattern, out Dictionary<string, string> vals)
	{
		vals = new Dictionary<string, string>();
		ArgumentNullException.ThrowIfNull(url, "url");
		ArgumentNullException.ThrowIfNull(pattern, "pattern");
		string[] array = SplitUrl(url);
		string[] array2 = SplitUrl(pattern);
		int num = array.Count((string x) => !x.Contains('=', StringComparison.InvariantCultureIgnoreCase));
		int num2 = array2.Count((string x) => !x.Contains('=', StringComparison.InvariantCultureIgnoreCase) && !x.Contains("!args", StringComparison.InvariantCultureIgnoreCase));
		if (num != num2)
		{
			ReasonFail = $"Pattern len not same!\n {num} != {num2}\nUrlParts:{string.Join(" ", array)}\nPatternPaths: {string.Join(" ", array2)}";
			return false;
		}
		bool flag = pattern.EndsWith("{!args}");
		for (int num3 = 0; num3 < array.Length; num3++)
		{
			string text = array[num3];
			if (flag && array2.Length - 1 <= num3 && text.Contains('=', StringComparison.InvariantCultureIgnoreCase))
			{
				string[] array3 = text.Split('=');
				vals.Add(array3[0], array3[1]);
			}
			else if (!ParseParameter(text, array2[num3], ref vals))
			{
				ReasonFail = "Parsing parameters did not work out " + text + " " + array2[num3];
				return false;
			}
		}
		ReasonFail = string.Empty;
		return true;
	}

	private static bool ParseParameter(string urlPart, string patternPart, ref Dictionary<string, string> vals)
	{
		string text = ExtractParameter(patternPart);
		if (!string.IsNullOrEmpty(text))
		{
			vals.Add(text.Replace("{", string.Empty, StringComparison.InvariantCultureIgnoreCase).Replace("}", string.Empty, StringComparison.InvariantCultureIgnoreCase), urlPart.Split('=').Last());
			return true;
		}
		if (!urlPart.Equals(patternPart, StringComparison.InvariantCultureIgnoreCase))
		{
			vals = new Dictionary<string, string>();
			return false;
		}
		return true;
	}

	private static string ExtractParameter(string pattern)
	{
		ArgumentNullException.ThrowIfNull(pattern, "pattern");
		if (!pattern.Contains('{', StringComparison.InvariantCultureIgnoreCase))
		{
			return string.Empty;
		}
		if (!pattern.Contains('}', StringComparison.InvariantCultureIgnoreCase))
		{
			return string.Empty;
		}
		int num = pattern.IndexOf('{', StringComparison.InvariantCultureIgnoreCase);
		int num2 = pattern.LastIndexOf('}');
		if (num2 - 1 > num)
		{
			return pattern.Substring(num, num2 - num + 1);
		}
		return string.Empty;
	}

	private static string[] SplitUrl(string url)
	{
		return SplitUrl(SplitUrl(url.Split('/', StringSplitOptions.RemoveEmptyEntries), '?'), '&');
	}

	private static string[] SplitUrl(string[] urlParts, char splitChar)
	{
		List<string> list = new List<string>();
		foreach (string text in urlParts)
		{
			if (!text.Contains(splitChar, StringComparison.InvariantCultureIgnoreCase))
			{
				list.Add(text);
				continue;
			}
			string[] collection = text.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);
			list.AddRange(collection);
		}
		return list.ToArray();
	}
}
