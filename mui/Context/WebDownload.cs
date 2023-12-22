using System.Xml.Serialization;

namespace mui.Context
{
	[XmlInclude( typeof( Context.WebDownload ) ), XmlInclude( typeof( Context.DownloadSubtitle ) ), XmlInclude( typeof( Context.DownloadLyrics ) ), XmlInclude( typeof( Context.DownloadBinary ) )]
	public class WebDownload
	{
		[XmlAttribute] public int Id { get; set; }
		[XmlAttribute] public string Filename { get; set; }
	}
	public class DownloadLyrics : WebDownload
	{
		[XmlAttribute] public string Lang { get; set; }
		[XmlText] public string VideoId { get; set; }
	}
	public class DownloadSubtitle : DownloadLyrics
	{
	}
	public class DownloadBinary : WebDownload
	{
		[XmlAttribute] public long DataLength { get; set; }
		[XmlText] public string uri { get; set; }
	}
}
