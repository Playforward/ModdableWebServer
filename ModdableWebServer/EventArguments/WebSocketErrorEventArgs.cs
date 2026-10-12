using ModdableWebServer.Interfaces;

namespace ModdableWebServer.EventArguments;

public class WebSocketErrorEventArgs(IWSServer server, string error)
{
	public IWSServer Server { get; } = server;

	public string Error { get; } = error;
}
