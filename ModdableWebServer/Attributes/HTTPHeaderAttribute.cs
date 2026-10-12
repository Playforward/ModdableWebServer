using System;

namespace ModdableWebServer.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class HTTPHeaderAttribute(string method, string url, string headername, string headervalue = "", bool useContains = false) : HTTPAttribute(method, url)
{
	public string HeaderName { get; } = headername;

	public string HeaderValue { get; } = headervalue;

	public bool UseContains { get; } = useContains;
}
