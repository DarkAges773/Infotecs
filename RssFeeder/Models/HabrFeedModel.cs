using System.Xml.Serialization;
namespace Infotecs.RssFeeder.Models
{
	[XmlRoot("rss")]
	public class HabrFeedDocument
	{
		[XmlElement(ElementName = "channel")]
		public HabrFeedChannel? Channel { get; set; }
	}	
	public class HabrFeedChannel
	{
		[XmlElement(ElementName = "title")]
		public string? Title { get; set; }

		[XmlElement(ElementName = "link")]
		public string? Link { get; set; }

		[XmlElement(ElementName = "description")]
		public string? Description { get; set; }

		[XmlElement(ElementName = "pubDate")]
		public string? PubDateRaw
		{
			get { return PubDate.ToString(); }
			set
			{
				_ = value == null ? PubDate = null : PubDate = DateTime.Parse(value);
			}
		}
		[XmlIgnore]
		public DateTime? PubDate { get; set; }

		[XmlElement(ElementName = "item")]
		public HabrFeedItem[]? Items { get; set; }
	}
	public class HabrFeedItem
	{
		[XmlElement(ElementName = "title")]
		public string? Title { get; set; }

		[XmlElement(ElementName = "link")]
		public string? Link { get; set; }

		[XmlElement(ElementName = "description")]
		public string? Description { get; set; }

		[XmlElement(ElementName = "pubDate")]
		public string? PubDateRaw { 
			get { return PubDate.ToString(); } 
			set {
				_ = value == null ? PubDate = null : PubDate = DateTime.Parse(value);
			}
		}
		[XmlIgnore]
		public DateTime? PubDate { get; set; }
	}	
}
