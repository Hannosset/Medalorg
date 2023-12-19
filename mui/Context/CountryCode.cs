using System.Xml.Serialization;

namespace mui.Context
{
	public sealed class CountryCode
	{
		[XmlAttribute] public string Code { get; set; }
		[XmlText] public string Label { get; set; }
	}
}
