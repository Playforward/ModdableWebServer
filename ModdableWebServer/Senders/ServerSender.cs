using System;
using System.Collections.Generic;
using System.Reflection;
using ModdableWebServer.Attributes;
using ModdableWebServer.Helper;
using ModdableWebServer.Interfaces;
using NetCoreServer;
using Serilog;

namespace ModdableWebServer.Senders;

public class ServerSender
{
	public required IHttpSession Session { get; init; }

	public required IHttpServer Server { get; init; }

	public HttpRequest Request { get; internal set; }

	public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();

	public Dictionary<string, string> Parameters { get; internal set; } = new Dictionary<string, string>();

	public HttpResponse Response => Session.Response;

	public virtual void SendResponse()
	{
		Session.SendResponse();
	}

	public virtual void SendResponse(HttpResponse response)
	{
		Session.SendResponse(response);
	}

	/// <summary>
	/// Hands the request to the first route that handles it, and to no other.
	/// </summary>
	public virtual bool SendRequest(HttpRequest request)
	{
		Request = request;
		string text = Uri.UnescapeDataString(Request.Url);
		Log.Verbose<string>("Requesting with URL: {url}", text);
		Dictionary<string, string> vals = new Dictionary<string, string>();
		foreach (KeyValuePair<HTTPAttribute, MethodInfo> hTTPAttributeToMethod in Server.HTTPAttributeToMethods)
		{
			if ((hTTPAttributeToMethod.Key.Url == text || UrlHelper.Match(text, hTTPAttributeToMethod.Key.Url, out vals)) && Request.Method.Equals(hTTPAttributeToMethod.Key.Method, StringComparison.CurrentCultureIgnoreCase))
			{
				Log.Verbose("URL Matched! " + text);
				Request.PopulateHeaders(Headers);
				Parameters = vals;
				bool handled = InvokeMethod(hTTPAttributeToMethod.Value);
				Log.Verbose("Invoked!");
				if (handled)
				{
					return true;
				}
			}
		}
		foreach (KeyValuePair<HTTPHeaderAttribute, MethodInfo> headerAttributeToMethod in Server.HeaderAttributeToMethods)
		{
			if ((headerAttributeToMethod.Key.Url == text || UrlHelper.Match(text, headerAttributeToMethod.Key.Url, out vals)) && Request.Method.Equals(headerAttributeToMethod.Key.Method, StringComparison.CurrentCultureIgnoreCase))
			{
				Log.Verbose("URL Matched! " + text);
				Request.PopulateHeaders(Headers);
				Parameters = vals;
				bool handled = SendHeader(headerAttributeToMethod.Key, headerAttributeToMethod.Value);
				Log.Verbose("Invoked!");
				if (handled)
				{
					return true;
				}
			}
		}
		return false;
	}

	internal bool InvokeMethod(MethodInfo method)
	{
		return (bool)method.Invoke(Server, new object[1] { this });
	}

	private bool SendHeader(HTTPHeaderAttribute attribute, MethodInfo method)
	{
		if (string.IsNullOrEmpty(attribute.HeaderName))
		{
			return false;
		}
		if (!Headers.TryGetValue(attribute.HeaderName.ToLower(), out string value))
		{
			return false;
		}
		if (string.IsNullOrEmpty(attribute.HeaderValue))
		{
			return InvokeMethod(method);
		}
		if (attribute.UseContains && value.Contains(attribute.HeaderValue, StringComparison.InvariantCultureIgnoreCase))
		{
			return InvokeMethod(method);
		}
		if (value == attribute.HeaderValue)
		{
			return InvokeMethod(method);
		}
		return false;
	}
}
