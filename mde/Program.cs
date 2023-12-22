using mde.Context;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mde
{
	internal class Program
	{
		static void Main( string[] args )
		{
			Console.OutputEncoding = Encoding.UTF8;

			foreach( string filename in args )
			{
				WebDownload[] Items = HandleWebDownload.LoadFromFile( filename );
				foreach( WebDownload wdItem in Items )
					wdItem.Download();
			}
		}
	}
}
