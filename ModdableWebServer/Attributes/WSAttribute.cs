using System;

namespace ModdableWebServer.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class WSAttribute(string url, WebSocketMethodListen listenMethod = WebSocketMethodListen.All) : Attribute
{
	public string Url { get; } = url;

	public WebSocketMethodListen ListenMethod { get; } = listenMethod;
}
