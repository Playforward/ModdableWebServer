using System.Collections.Generic;
using System.Reflection;
using ModdableWebServer.Attributes;

namespace ModdableWebServer.Interfaces;

public interface IHttpServer : IServer
{
	bool DoReturn404IfFail { get; set; }

	Dictionary<HTTPAttribute, MethodInfo> HTTPAttributeToMethods { get; }

	Dictionary<HTTPHeaderAttribute, MethodInfo> HeaderAttributeToMethods { get; }
}
