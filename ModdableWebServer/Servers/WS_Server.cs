using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using ModdableWebServer.Attributes;
using ModdableWebServer.Helper;
using ModdableWebServer.Interfaces;
using ModdableWebServer.Sessions;
using NetCoreServer;

namespace ModdableWebServer.Servers;

public class WS_Server : WsServer, IWSServer, IHttpServer, IServer
{
	public bool DoReturn404IfFail { get; set; } = true;

	public Dictionary<WSAttribute, MethodInfo> WSAttributeToMethods { get; } = new Dictionary<WSAttribute, MethodInfo>();

	public Dictionary<HTTPAttribute, MethodInfo> HTTPAttributeToMethods { get; } = new Dictionary<HTTPAttribute, MethodInfo>();

	public Dictionary<HTTPHeaderAttribute, MethodInfo> HeaderAttributeToMethods { get; } = new Dictionary<HTTPHeaderAttribute, MethodInfo>();

	public WS_Server(string address, int port)
		: base(address, port)
	{
	}

	public WS_Server(IPAddress address, int port)
		: base(address, port)
	{
	}

	public WS_Server(DnsEndPoint endPoint)
		: base(endPoint)
	{
	}

	public WS_Server(IPEndPoint endPoint)
		: base(endPoint)
	{
	}

	[RequiresUnreferencedCode("This require to get all assembly type.")]
	public void OverrideAttributes(Assembly assembly)
	{
		HeaderAttributeToMethods.Override(assembly);
		WSAttributeToMethods.Override(assembly);
		HTTPAttributeToMethods.Override(assembly);
	}

	[RequiresUnreferencedCode("This require to get all assembly type.")]
	public void MergeAttributes(Assembly assembly)
	{
		HeaderAttributeToMethods.Merge(assembly);
		WSAttributeToMethods.Merge(assembly);
		HTTPAttributeToMethods.Merge(assembly);
	}

	public void OverrideAttributes([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] Type? type)
	{
		HeaderAttributeToMethods.Override(type);
		WSAttributeToMethods.Override(type);
		HTTPAttributeToMethods.Override(type);
	}

	public void MergeAttributes([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] Type? type)
	{
		HeaderAttributeToMethods.Merge(type);
		WSAttributeToMethods.Merge(type);
		HTTPAttributeToMethods.Merge(type);
	}

	public void ClearAttributes()
	{
		HeaderAttributeToMethods.Clear();
		WSAttributeToMethods.Clear();
		HTTPAttributeToMethods.Clear();
	}

	protected override void OnStarted()
	{
		IServer server = this;
		ServerEvents.OnStarted(in server);
	}

	protected override void OnStopped()
	{
		IServer server = this;
		ServerEvents.OnStopped(in server);
	}

	protected override TcpSession CreateSession()
	{
		return (TcpSession)(object)new WS_Session((WsServer)(object)this);
	}

	protected override void OnError(SocketError error)
	{
		IServer server = this;
		ServerEvents.OnSocketError(in server, (Exception)null, in error);
	}

	bool IWSServer.MulticastText(ReadOnlySpan<char> text)
	{
		return ((WsServer)this).MulticastText(text);
	}

	bool IWSServer.MulticastText(ReadOnlySpan<byte> buffer)
	{
		return ((WsServer)this).MulticastText(buffer);
	}

	bool IWSServer.MulticastBinary(ReadOnlySpan<char> text)
	{
		return ((WsServer)this).MulticastBinary(text);
	}

	bool IWSServer.MulticastBinary(ReadOnlySpan<byte> buffer)
	{
		return ((WsServer)this).MulticastBinary(buffer);
	}

	bool IWSServer.MulticastPing(ReadOnlySpan<char> text)
	{
		return ((WsServer)this).MulticastPing(text);
	}

	bool IWSServer.MulticastPing(ReadOnlySpan<byte> buffer)
	{
		return ((WsServer)this).MulticastPing(buffer);
	}
}
