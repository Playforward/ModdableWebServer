using System;
using System.Net;

namespace ModdableWebServer.Interfaces;

public interface IServer
{
	Guid Id { get; }

	string Address { get; }

	int Port { get; }

	EndPoint Endpoint { get; }

	bool IsStarted { get; }

	bool IsDisposed { get; }

	bool Start();

	bool Stop();

	bool Restart();

	bool DisconnectAll();

	bool Multicast(ReadOnlySpan<byte> buffer);

	bool Multicast(ReadOnlySpan<char> text);
}
