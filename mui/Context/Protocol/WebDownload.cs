using System.Xml.Serialization;

namespace mui.Context.Protocol
{
	[
	  XmlInclude( typeof( WebDownload ) )
	, XmlInclude( typeof( DownloadSubtitle ) )
	, XmlInclude( typeof( DownloadLyrics ) )
	, XmlInclude( typeof( DownloadBinary ) )
	, XmlInclude( typeof( DownloadAudio ) )
	, XmlInclude( typeof( DownloadVideo ) )
	]
	public class WebDownload
	{
		[XmlAttribute] public string VideoId { get; set; }
		[XmlAttribute] public int Id { get; set; }
		[XmlAttribute] public bool Downloaded { get; set; } = false;
		[XmlText] public string Filename { get; set; }
		[XmlIgnore] public MediaInfo.MediaData MediaData { get; set; }
	}
	public class DownloadLyrics : WebDownload
	{
		[XmlAttribute] public string Lang { get; set; }
	}
	public class DownloadSubtitle : DownloadLyrics
	{
	}
	public class DownloadBinary : WebDownload
	{
		[XmlAttribute] public long DataLength { get; set; }
	}
	public class DownloadAudio : DownloadBinary
	{
		[XmlAttribute] public int BitRate { get; set; }
		[XmlAttribute] public AudioFormat Format { get; set; }
	}
	public class DownloadVideo : DownloadBinary
	{
		[XmlAttribute] public int Resolution { get; set; }
		[XmlAttribute] public VideoFormat Format { get; set; }

		[XmlAttribute] public string TargetFilename { get; set; }
	}
}