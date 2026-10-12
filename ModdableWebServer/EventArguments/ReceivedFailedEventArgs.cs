using System;
using ModdableWebServer.Interfaces;
using NetCoreServer;

namespace ModdableWebServer.EventArguments;

public class ReceivedFailedEventArgs(IServer server, HttpRequest request) : EventArgs
{
	public IServer Server { get; } = server;

	public HttpRequest Request { get; } = request;
}
