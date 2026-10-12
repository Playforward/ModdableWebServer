using System.Collections.Generic;
using NetCoreServer;

namespace ModdableWebServer.Helper;

public static class RequestExport
{
	public static Dictionary<string, string> GetHeaders(this HttpRequest request)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		for (int i = 0; i < request.Headers; i++)
		{
			(string, string) tuple = request.Header(i);
			if (!dictionary.ContainsKey(tuple.Item1.ToLower()))
			{
				dictionary.Add(tuple.Item1.ToLower(), tuple.Item2);
			}
		}
		return dictionary;
	}

	public static void PopulateHeaders(this HttpRequest request, Dictionary<string, string> Headers)
	{
		Headers.Clear();
		for (int i = 0; i < request.Headers; i++)
		{
			(string, string) tuple = request.Header(i);
			if (!Headers.ContainsKey(tuple.Item1.ToLower()))
			{
				Headers.Add(tuple.Item1.ToLower(), tuple.Item2);
			}
		}
	}
}
