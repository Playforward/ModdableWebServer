using NetCoreServer;

namespace ModdableWebServer.Helper;

/// <summary>
/// The static-file cache check every session type makes before dispatching.
/// </summary>
internal static class CachedResponse
{
	/// <summary>
	/// Sends the cached content for a plain GET the cache holds, and reports
	/// whether it did. True means the request has been answered and must not
	/// go on to the routes.
	/// </summary>
	internal static bool TryServe(HttpRequest request, FileCache cache, Action<byte[]> send)
	{
		if (request.Method != "GET"
			|| request.Url.Contains('?', StringComparison.InvariantCultureIgnoreCase)
			|| !cache.FindPath(request.Url))
		{
			return false;
		}
		(bool found, byte[] content) = cache.Find(request.Url);
		if (!found)
		{
			return false;
		}
		send(content);
		return true;
	}
}
