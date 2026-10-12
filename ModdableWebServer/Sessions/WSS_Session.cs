using System;
using System.Net.Sockets;
using ModdableWebServer.Helper;
using ModdableWebServer.Interfaces;
using ModdableWebServer.Senders;
using NetCoreServer;
using Serilog;

namespace ModdableWebServer.Sessions;

public class WSS_Session : WssSession, IWSSession, IHttpSession, ISession
{
	private WebSocketSender Sender;

	public IServer IServer => (IServer)((SslSession)this).Server;

	public WSS_Session(WssServer server)
		: base(server)
	{
	}

	protected override void OnConnecting()
	{
		Sender = new WebSocketSender
		{
			Server = (IHttpServer)IServer,
			Session = this
		};
	}

	protected override void OnReceivedRequest(HttpRequest request)
	{
		if (CachedResponse.TryServe(request, ((HttpsSession)this).Cache, content => ((SslSession)this).SendAsync(content)))
		{
			return;
		}
		if (request.GetHeaders().ContainsValue("websocket"))
		{
			Log.Verbose("[OnReceivedRequest] Websocket Value on Headers Send back to base!");
			base.OnReceivedRequest(request);
			return;
		}
		bool flag = Sender.SendRequest(request);
		if (!flag)
		{
			ServerEvents.OnReceivedFailed(IServer, in request);
		}
		if (IServer is IHttpServer { DoReturn404IfFail: not false } && !flag)
		{
			((HttpsSession)this).SendResponse(((HttpsSession)this).Response.MakeErrorResponse(404, "", "text/plain; charset=UTF-8"));
		}
	}

	public override void OnWsConnected(HttpRequest request)
	{
		Sender.Send(WebSocketMethodListen.Connected, request);
		Log.Verbose("OnWsConnected!");
	}

	public override bool OnWsConnecting(HttpRequest request, HttpResponse response)
	{
		Log.Verbose("OnWsConnecting!");
		Sender.Send(WebSocketMethodListen.Connecting, request);
		return ((WssSession)this).OnWsConnecting(request, response);
	}

	public override void OnWsReceived(byte[] buffer, long offset, long size)
	{
		Sender.Buffer = buffer;
		Sender.Offset = offset;
		Sender.Size = size;
		Sender.Send(WebSocketMethodListen.Received);
		Log.Verbose("OnWsReceived!");
	}

	public override void OnWsDisconnected()
	{
		Sender.Send(WebSocketMethodListen.Disconnected);
		Log.Verbose("OnWsDisconnected!");
	}

	public override void OnWsDisconnecting()
	{
		Sender.Send(WebSocketMethodListen.Disconnecting);
		Log.Verbose("OnWsDisconnecting!");
	}

	public override void OnWsPing(byte[] buffer, long offset, long size)
	{
		Sender.Buffer = buffer;
		Sender.Offset = offset;
		Sender.Size = size;
		((WssSession)this).OnWsPing(buffer, offset, size);
		Sender.Send(WebSocketMethodListen.Ping);
		Log.Verbose("OnWsPing!");
	}

	public override void OnWsPong(byte[] buffer, long offset, long size)
	{
		Sender.Buffer = buffer;
		Sender.Offset = offset;
		Sender.Size = size;
		Sender.Send(WebSocketMethodListen.Pong);
		Log.Verbose("OnWsPong!");
	}

	public override void OnWsClose(byte[] buffer, long offset, long size, int status = 1000)
	{
		Sender.Buffer = buffer;
		Sender.Offset = offset;
		Sender.Size = size;
		Sender.CloseStatus = status;
		Sender.Send(WebSocketMethodListen.Close);
		Log.Verbose("OnWsClose!");
		((WssSession)this).OnWsClose(buffer, offset, size, status);
	}

	public override void OnWsError(string error)
	{
		ServerEvents.OnWebSocketError((IWSServer)IServer, in error);
	}

	protected override void OnReceivedRequestError(HttpRequest request, string error)
	{
		ServerEvents.OnReceivedRequestError(IServer, in request, in error);
	}

	protected override void OnError(Exception ex, SocketError error)
	{
		ServerEvents.OnSocketError(IServer, in ex, in error);
	}

	long IWSSession.SendText(ReadOnlySpan<char> text)
	{
		return ((WssSession)this).SendText(text);
	}

	long IWSSession.SendText(ReadOnlySpan<byte> buffer)
	{
		return ((WssSession)this).SendText(buffer);
	}

	long IWSSession.SendBinary(ReadOnlySpan<char> text)
	{
		return ((WssSession)this).SendBinary(text);
	}

	long IWSSession.SendBinary(ReadOnlySpan<byte> buffer)
	{
		return ((WssSession)this).SendBinary(buffer);
	}

	long IWSSession.SendPing(ReadOnlySpan<char> text)
	{
		return ((WssSession)this).SendPing(text);
	}

	long IWSSession.SendPing(ReadOnlySpan<byte> buffer)
	{
		return ((WssSession)this).SendPing(buffer);
	}

	long IWSSession.SendPong(ReadOnlySpan<char> text)
	{
		return ((WssSession)this).SendPong(text);
	}

	long IWSSession.SendPong(ReadOnlySpan<byte> buffer)
	{
		return ((WssSession)this).SendPong(buffer);
	}

	long IWSSession.SendClose(int status, ReadOnlySpan<char> text)
	{
		return ((WssSession)this).SendClose(status, text);
	}

	long IWSSession.SendClose(int status, ReadOnlySpan<byte> buffer)
	{
		return ((WssSession)this).SendClose(status, buffer);
	}

	long IHttpSession.SendResponse()
	{
		return ((HttpsSession)this).SendResponse();
	}

	long IHttpSession.SendResponse(HttpResponse response)
	{
		return ((HttpsSession)this).SendResponse(response);
	}
}
