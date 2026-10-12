using System;

namespace ModdableWebServer;

[Flags]
public enum WebSocketMethodListen
{
	None = 0,
	Connected = 1,
	Connecting = 2,
	Disconnected = 4,
	Disconnecting = 8,
	Received = 0x10,
	Ping = 0x20,
	Pong = 0x40,
	Close = 0x80,
	All = Connected | Connecting | Disconnected | Disconnecting | Received | Ping | Pong | Close
}
