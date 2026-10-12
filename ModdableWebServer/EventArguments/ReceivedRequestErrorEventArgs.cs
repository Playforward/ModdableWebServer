using System;
using ModdableWebServer.Interfaces;
using NetCoreServer;

namespace ModdableWebServer.EventArguments;

public class ReceivedRequestErrorEventArgs(IServer server, HttpRequest request, string error) : EventArgs
{
	public IServer Server { get; } = server;

	public HttpRequest Request { get; } = request;

	public string Error { get; } = error;
}
