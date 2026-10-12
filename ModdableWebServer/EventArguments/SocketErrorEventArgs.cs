using System;
using System.Net.Sockets;
using ModdableWebServer.Interfaces;

namespace ModdableWebServer.EventArguments;

public class SocketErrorEventArgs(IServer server, Exception? ex, SocketError error) : EventArgs
{
	public IServer Server { get; } = server;

	public Exception? Exception { get; } = ex;

	public SocketError Error { get; } = error;
}
