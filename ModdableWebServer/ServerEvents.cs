using System;
using System.Net.Sockets;
using ModdableWebServer.EventArguments;
using ModdableWebServer.Interfaces;
using NetCoreServer;

namespace ModdableWebServer;

public static class ServerEvents
{
	public static event EventHandler<ReceivedRequestErrorEventArgs>? ReceivedRequestError;

	public static event EventHandler<SocketErrorEventArgs>? SocketError;

	public static event EventHandler<ReceivedFailedEventArgs>? ReceivedFailed;

	public static event EventHandler<WebSocketErrorEventArgs>? WebSocketError;

	public static event EventHandler<ServerEventArgs>? Started;

	public static event EventHandler<ServerEventArgs>? Stopped;

	internal static void OnStopped(in IServer server)
	{
		Stopped?.Invoke(null, new ServerEventArgs(server));
	}

	internal static void OnStarted(in IServer server)
	{
		Started?.Invoke(null, new ServerEventArgs(server));
	}

	internal static void OnReceivedRequestError(in IServer server, in HttpRequest request, in string error)
	{
		ReceivedRequestError?.Invoke(null, new ReceivedRequestErrorEventArgs(server, request, error));
	}

	internal static void OnSocketError(in IServer server, in Exception? ex, in SocketError error)
	{
		SocketError?.Invoke(null, new SocketErrorEventArgs(server, ex, error));
	}

	internal static void OnReceivedFailed(in IServer server, in HttpRequest request)
	{
		ReceivedFailed?.Invoke(null, new ReceivedFailedEventArgs(server, request));
	}

	internal static void OnWebSocketError(in IWSServer server, in string error)
	{
		WebSocketError?.Invoke(null, new WebSocketErrorEventArgs(server, error));
	}
}
