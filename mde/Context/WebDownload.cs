using System;
using System.Xml.Serialization;

namespace mde.Context
{
	[XmlInclude( typeof( WebDownload ) ), XmlInclude( typeof( DownloadSubtitle ) ), XmlInclude( typeof( DownloadLyrics ) ), XmlInclude( typeof( DownloadBinary ) )]
	public class WebDownload
	{
		[XmlAttribute] public string Filename { get; set; }

		public virtual void Download()
		{ }
	}
	public class DownloadLyrics : WebDownload
	{
		[XmlAttribute] public string Lang { get; set; }
		[XmlText] public string VideoId { get; set; }

		public override void Download()
		{
			Console.WriteLine( $"Download Lyrics" + Lang );
		}
	}
	public class DownloadSubtitle : DownloadLyrics
	{
		public override void Download()
		{
			Console.WriteLine( $"Download Subtitles in " + Lang );
		}
	}
	public class DownloadBinary : WebDownload
	{
		[XmlAttribute] public long DataLength { get; set; }
		[XmlText] public string uri { get; set; }

		public override void Download()
		{
			Console.WriteLine( $"Binary Download of {DataLength / 1024:#,###,###} Kb" );
		}
	}
}
