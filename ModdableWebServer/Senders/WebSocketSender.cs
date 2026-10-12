using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModdableWebServer.Attributes;
using ModdableWebServer.Helper;
using ModdableWebServer.Interfaces;
using NetCoreServer;
using Serilog;

namespace ModdableWebServer.Senders;

public class WebSocketSender : ServerSender
{
	private string? CachedURL;

	public byte[] Buffer { get; set; } = Array.Empty<byte>();

	public long Offset { get; set; }

	public long Size { get; set; }

	public int CloseStatus { get; set; }

	public WebSocketMethodListen CurrentMethod { get; protected set; }

	public void Send(WebSocketMethodListen currentMethod, HttpRequest? request = null)
	{
		if (currentMethod == WebSocketMethodListen.None)
		{
			return;
		}
		CurrentMethod = currentMethod;
		if (request != null)
		{
			base.Request = request;
		}
		if (!string.IsNullOrEmpty(base.Request.Url))
		{
			CachedURL = base.Request.Url;
		}
		if (string.IsNullOrEmpty(CachedURL) || !(base.Server is IWSServer iWSServer))
		{
			return;
		}
		Dictionary<string, string> vals = new Dictionary<string, string>();
		foreach (KeyValuePair<WSAttribute, MethodInfo> item in iWSServer.WSAttributeToMethods.Where<KeyValuePair<WSAttribute, MethodInfo>>((KeyValuePair<WSAttribute, MethodInfo> x) => x.Key.ListenMethod.HasFlag(currentMethod)))
		{
			if (item.Key.Url == CachedURL || UrlHelper.Match(CachedURL, item.Key.Url, out vals))
			{
				Log.Verbose("URL Matched! " + CachedURL);
				base.Request?.PopulateHeaders(base.Headers);
				base.Parameters = vals;
				item.Value.Invoke(base.Server, new object[1] { this });
			}
		}
		Buffer = Array.Empty<byte>();
		Offset = 0L;
		Size = 0L;
		CloseStatus = 0;
		CurrentMethod = WebSocketMethodListen.None;
	}
}
