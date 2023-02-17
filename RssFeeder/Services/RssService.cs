using Infotecs.RssFeeder.Models;
using System.Xml.Serialization;

namespace Infotecs.RssFeeder.Services;

public class RssService
{
	public static Dictionary<string, HabrFeedDocument> Feeds { get => feeds; }
	private static Dictionary<string, HabrFeedDocument> feeds = new();

	private readonly IHttpClientFactory _clientFactory;
	private readonly IConfiguration _config;
	private readonly Dictionary<string, string> _rssSources;
	public RssService(IHttpClientFactory clientFactory, IConfiguration config)
	{
		_clientFactory = clientFactory;
		_config = config;
		_rssSources = _config.GetSection("RssSources")
				.GetChildren()
				.ToDictionary(x => x.Key, x => x.Value);
	}
	public async Task ParseRssAsync()
	{
		var parsedFeeds = new Dictionary<string, HabrFeedDocument>();
		var client = _clientFactory.CreateClient();
		foreach (KeyValuePair<string, string> entry in _rssSources)
		{
			var feed = await GetFeedAsync(client, entry.Value);
			if(feed != null)
				parsedFeeds.Add(entry.Key, feed);
		}
		feeds = parsedFeeds;
	}
	private async Task<HabrFeedDocument?> GetFeedAsync(HttpClient client, string url)
	{
		var request = new HttpRequestMessage(HttpMethod.Get, url);
		request.Headers.Add("Accept", "text/xml");
		request.Headers.Add("User-Agent", "RssFeeder");
		var response = await client.SendAsync(request);
		var xmlSerializer = new XmlSerializer(typeof(HabrFeedDocument));
		var feed = (HabrFeedDocument?)xmlSerializer.Deserialize(await response.Content.ReadAsStreamAsync());
		return feed;
	}
}
