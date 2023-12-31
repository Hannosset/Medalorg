using mde.Context;

using System;
using System.Text;

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
				long total = 0;
				bool success = true;
				foreach( WebDownload wdItem in Items )
				{
					if( wdItem.Initialize() )
					{
						long rc = wdItem.Download();
						if( rc == -1 )
							success = false;
						else
							total += rc;
						wdItem.Close();
					}
					else
						success = false;
				}

				if( Items.Length > 0 )
					if( success )
						Console.WriteLine( $"{Items[0].VideoId}\t \t{total}\tDownload Completed" );
					else
						Console.WriteLine( $"{Items[0].VideoId}\t \t{total}\tDownload incomplete" );
			}
		}
	}
}
