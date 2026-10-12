using System;
using System.Collections.Generic;
using System.Reflection;
using ModdableWebServer.Attributes;

namespace ModdableWebServer.Interfaces;

public interface IWSServer : IHttpServer, IServer
{
	Dictionary<WSAttribute, MethodInfo> WSAttributeToMethods { get; }

	bool CloseAll();

	bool CloseAll(int status);

	bool CloseAll(int status, ReadOnlySpan<byte> buffer);

	bool MulticastText(ReadOnlySpan<char> text);

	bool MulticastText(ReadOnlySpan<byte> buffer);

	bool MulticastBinary(ReadOnlySpan<char> text);

	bool MulticastBinary(ReadOnlySpan<byte> buffer);

	bool MulticastPing(ReadOnlySpan<char> text);

	bool MulticastPing(ReadOnlySpan<byte> buffer);
}
