using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace mui.Context
{
	public sealed class PublishedMedia
	{
		#region TYPES
		public sealed class MediaUri
		{
			[XmlAttribute] public long DataLength { get; set; }
			
			[XmlText] public string Uri { get; set; }
		}
		#endregion

		[XmlAttribute] public string Name { get;set; }
		[XmlAttribute] public string VideoId { get; set; }
		[XmlAttribute] public string subtitles { get; set; }
		
		public MediaUri[] Items { get; set; }
	}
}
