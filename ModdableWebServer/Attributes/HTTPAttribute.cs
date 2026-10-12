using System;

namespace ModdableWebServer.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class HTTPAttribute(string method, string url) : Attribute
{
	public string Method { get; } = method;

	public string Url { get; } = url;
}
