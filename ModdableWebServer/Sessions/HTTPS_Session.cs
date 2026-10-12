using System;
using System.Net.Sockets;
using ModdableWebServer.Helper;
using ModdableWebServer.Interfaces;
using ModdableWebServer.Senders;
using NetCoreServer;

namespace ModdableWebServer.Sessions;

public class HTTPS_Session : HttpsSession, IHttpSession, ISession
{
	private ServerSender Sender;

	public IServer IServer => (IServer)((SslSession)this).Server;

	public HTTPS_Session(HttpsServer server)
		: base(server)
	{
	}

	protected override void OnConnecting()
	{
		Sender = new ServerSender
		{
			Server = (IHttpServer)((SslSession)this).Server,
			Session = this
		};
	}

	protected override void OnReceivedRequest(HttpRequest request)
	{
		if (CachedResponse.TryServe(request, ((HttpsSession)this).Cache, content => ((SslSession)this).SendAsync(content)))
		{
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

	protected override void OnReceivedRequestError(HttpRequest request, string error)
	{
		ServerEvents.OnReceivedRequestError(IServer, in request, in error);
	}

	protected override void OnError(Exception ex, SocketError error)
	{
		ServerEvents.OnSocketError(IServer, in ex, in error);
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
