using System;

namespace ModdableWebServer.Interfaces;

public interface ISession
{
	Guid Id { get; }

	IServer IServer { get; }

	bool IsConnected { get; }

	bool IsDisposed { get; }

	bool Disconnect();

	long Send(ReadOnlySpan<byte> buffer);

	long Send(string text);
}
