using System;
using ModdableWebServer.Interfaces;

namespace ModdableWebServer.EventArguments;

public class ServerEventArgs(IServer server) : EventArgs
{
	public IServer Server { get; } = server;
}
