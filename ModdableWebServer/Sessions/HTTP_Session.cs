using System;
using System.Net.Sockets;
using ModdableWebServer.Helper;
using ModdableWebServer.Interfaces;
using ModdableWebServer.Senders;
using NetCoreServer;

namespace ModdableWebServer.Sessions;

public class HTTP_Session : HttpSession, IHttpSession, ISession
{
	private ServerSender Sender;

	public IServer IServer => (IServer)((TcpSession)this).Server;

	public HTTP_Session(HttpServer server)
		: base(server)
	{
	}

	protected override void OnConnecting()
	{
		Sender = new ServerSender
		{
			Server = (IHttpServer)((TcpSession)this).Server,
			Session = this
		};
	}

	protected override void OnReceivedRequest(HttpRequest request)
	{
		if (CachedResponse.TryServe(request, ((HttpSession)this).Cache, content => ((TcpSession)this).SendAsync(content)))
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
			((HttpSession)this).SendResponse(((HttpSession)this).Response.MakeErrorResponse(404, "", "text/plain; charset=UTF-8"));
		}
	}

	protected override void OnReceivedRequestError(HttpRequest request, string error)
	{
		ServerEvents.OnReceivedRequestError(IServer, in request, in error);
	}

	protected override void OnError(SocketError error)
	{
		ServerEvents.OnSocketError(IServer, (Exception)null, in error);
	}

	long IHttpSession.SendResponse()
	{
		return ((HttpSession)this).SendResponse();
	}

	long IHttpSession.SendResponse(HttpResponse response)
	{
		return ((HttpSession)this).SendResponse(response);
	}
}
