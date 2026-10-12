using System.Net;
using System.Reflection;
using ModdableWebServer.Attributes;
using ModdableWebServer.Senders;
using ModdableWebServer.Servers;
using ModdableWebServer.Sessions;
using NetCoreServer;

namespace ModdableWebServer.Test.Senders;

/// <summary>
/// Upstream 3.1.0 invoked every route that matched a request, so a request two
/// patterns matched got two responses. These pin down that dispatch stops at
/// the first handler that handles it.
/// </summary>
public class ServerSenderTest
{
    private static readonly List<string> Invoked = [];

    private static bool Literal(ServerSender sender) { Invoked.Add(nameof(Literal)); return true; }
    private static bool Pattern(ServerSender sender) { Invoked.Add(nameof(Pattern)); return true; }
    private static bool Declines(ServerSender sender) { Invoked.Add(nameof(Declines)); return false; }
    private static bool ByHeader(ServerSender sender) { Invoked.Add(nameof(ByHeader)); return true; }

    private static MethodInfo Handler(string name) =>
        typeof(ServerSenderTest).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static)!;

    private static (HTTP_Server Server, ServerSender Sender) NewSender()
    {
        Invoked.Clear();
        // Constructed, never started: dispatch reads only its route tables.
        var server = new HTTP_Server(IPAddress.Loopback, 0);
        var sender = new ServerSender { Server = server, Session = new HTTP_Session(server) };
        return (server, sender);
    }

    [Test]
    public void ARequestTwoRoutesMatchReachesOnlyTheFirst()
    {
        var (server, sender) = NewSender();
        server.HTTPAttributeToMethods.Add(new HTTPAttribute("GET", "/thing/abc"), Handler(nameof(Literal)));
        server.HTTPAttributeToMethods.Add(new HTTPAttribute("GET", "/thing/{id}"), Handler(nameof(Pattern)));

        bool handled = sender.SendRequest(new HttpRequest("GET", "/thing/abc"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(handled, Is.True);
            Assert.That(Invoked, Is.EqualTo(new[] { nameof(Literal) }));
        }
    }

    [Test]
    public void ARouteThatDeclinesLetsTheNextMatchAnswer()
    {
        var (server, sender) = NewSender();
        server.HTTPAttributeToMethods.Add(new HTTPAttribute("GET", "/thing/abc"), Handler(nameof(Declines)));
        server.HTTPAttributeToMethods.Add(new HTTPAttribute("GET", "/thing/{id}"), Handler(nameof(Pattern)));

        bool handled = sender.SendRequest(new HttpRequest("GET", "/thing/abc"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(handled, Is.True);
            Assert.That(Invoked, Is.EqualTo(new[] { nameof(Declines), nameof(Pattern) }));
        }
    }

    [Test]
    public void AHeaderRouteIsNotRunOnceAnHttpRouteHasAnswered()
    {
        var (server, sender) = NewSender();
        server.HTTPAttributeToMethods.Add(new HTTPAttribute("GET", "/thing/abc"), Handler(nameof(Literal)));
        server.HeaderAttributeToMethods.Add(new HTTPHeaderAttribute("GET", "/thing/abc", "x-test"), Handler(nameof(ByHeader)));

        var request = new HttpRequest("GET", "/thing/abc");
        request.SetHeader("X-Test", "1");
        sender.SendRequest(request);

        Assert.That(Invoked, Is.EqualTo(new[] { nameof(Literal) }));
    }

    [Test]
    public void NothingMatchingIsNotHandled()
    {
        var (_, sender) = NewSender();
        Assert.That(sender.SendRequest(new HttpRequest("GET", "/nothing")), Is.False);
    }
}
