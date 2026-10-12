using ModdableWebServer.Helper;
using NetCoreServer;

namespace ModdableWebServer.Test.Helper;

/// <summary>
/// A cache hit used to send the cached bytes and then dispatch the request to
/// the routes anyway (and 404 it when none matched): several responses to one
/// request. TryServe says whether the request is finished.
/// </summary>
public class CachedResponseTest
{
    private string _dir = "";
    private FileCache _cache = null!;
    private static readonly byte[] Content = [1, 2, 3];

    [SetUp]
    public void SetUp()
    {
        // FindPath keys on the inserted path and Find on an entry key, so the
        // one URL that satisfies both is a path that is also an entry.
        _dir = Directory.CreateTempSubdirectory("mws-cache-").FullName;
        _cache = new FileCache();
        Assert.That(_cache.InsertPath(_dir), Is.True);
        Assert.That(_cache.Add(_dir, Content), Is.True);
    }

    [TearDown]
    public void TearDown()
    {
        _cache.Dispose();
        Directory.Delete(_dir, recursive: true);
    }

    [Test]
    public void AHitIsSentAndFinishesTheRequest()
    {
        byte[]? sent = null;
        bool finished = CachedResponse.TryServe(new HttpRequest("GET", _dir), _cache, content => sent = content);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(finished, Is.True, "a request answered from the cache must not go on to the routes");
            Assert.That(sent, Is.EqualTo(Content));
        }
    }

    [TestCase("POST", "")]
    [TestCase("GET", "?q=1")]
    public void AnythingElseIsLeftToTheRoutes(string method, string query)
    {
        bool sent = false;
        bool finished = CachedResponse.TryServe(new HttpRequest(method, _dir + query), _cache, _ => sent = true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(finished, Is.False);
            Assert.That(sent, Is.False);
        }
    }
}
