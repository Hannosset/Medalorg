using System.Xml.Serialization;

namespace mui.Context
{
	public sealed class AuthorInfo
	{
		[XmlText] public string Name { get; set; }
		[XmlAttribute] public MediaType Type { get; set; } = MediaType.Unkown;
		[XmlAttribute] public string Genre { get; set; }
		[XmlAttribute] public string Style { get; set; }
	}
}
