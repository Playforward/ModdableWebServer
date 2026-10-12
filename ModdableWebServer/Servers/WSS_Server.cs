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

public class WSS_Server : WssServer, IWSServer, IHttpServer, IServer
{
	public bool DoReturn404IfFail { get; set; } = true;

	public Dictionary<WSAttribute, MethodInfo> WSAttributeToMethods { get; } = new Dictionary<WSAttribute, MethodInfo>();

	public Dictionary<HTTPAttribute, MethodInfo> HTTPAttributeToMethods { get; } = new Dictionary<HTTPAttribute, MethodInfo>();

	public Dictionary<HTTPHeaderAttribute, MethodInfo> HeaderAttributeToMethods { get; } = new Dictionary<HTTPHeaderAttribute, MethodInfo>();

	public WSS_Server(SslContext context, string address, int port)
		: base(context, address, port)
	{
	}

	public WSS_Server(SslContext context, IPAddress address, int port)
		: base(context, address, port)
	{
	}

	public WSS_Server(SslContext context, DnsEndPoint endPoint)
		: base(context, endPoint)
	{
	}

	public WSS_Server(SslContext context, IPEndPoint endPoint)
		: base(context, endPoint)
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

	protected override SslSession CreateSession()
	{
		return (SslSession)(object)new WSS_Session((WssServer)(object)this);
	}

	protected override void OnError(SocketError error)
	{
		IServer server = this;
		ServerEvents.OnSocketError(in server, (Exception)null, in error);
	}

	bool IWSServer.MulticastText(ReadOnlySpan<char> text)
	{
		return ((WssServer)this).MulticastText(text);
	}

	bool IWSServer.MulticastText(ReadOnlySpan<byte> buffer)
	{
		return ((WssServer)this).MulticastText(buffer);
	}

	bool IWSServer.MulticastBinary(ReadOnlySpan<char> text)
	{
		return ((WssServer)this).MulticastBinary(text);
	}

	bool IWSServer.MulticastBinary(ReadOnlySpan<byte> buffer)
	{
		return ((WssServer)this).MulticastBinary(buffer);
	}

	bool IWSServer.MulticastPing(ReadOnlySpan<char> text)
	{
		return ((WssServer)this).MulticastPing(text);
	}

	bool IWSServer.MulticastPing(ReadOnlySpan<byte> buffer)
	{
		return ((WssServer)this).MulticastPing(buffer);
	}
}
