using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace mde.Context
{
	public class WebDownload
	{
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
