using System.Xml.Serialization;

namespace mui.Context.Protocol
{
	public sealed class AuthorInfo
	{
		[XmlText] public string Name { get; set; }
		[XmlAttribute] public AdaptiveKind Type { get; set; } = AdaptiveKind.Unkown;
		[XmlAttribute] public string Genre { get; set; }
		[XmlAttribute] public string Style { get; set; }
	}
}
