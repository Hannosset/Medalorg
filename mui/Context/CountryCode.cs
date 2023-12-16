using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace mui.Context
{
	public sealed class CountryCode
	{
		[XmlAttribute]
		public string Code { get; set; }
		[XmlText]
		public string Label { get; set; }
	}
}
