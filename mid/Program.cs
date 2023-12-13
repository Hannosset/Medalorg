using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

using VideoLibrary;

namespace mid
{
	internal class Program
	{
		#region PLAY LIST
		static string ReadStream( Stream strm )
		{
			StringBuilder sb = new StringBuilder();
			{
				byte[] buffer = new byte[16 * 1024];
				int bytesRead = 0;
				do
				{
					bytesRead = strm.Read( buffer , 0 , buffer.Length );
					if( bytesRead > 0 )
						sb.Append( Encoding.Default.GetString( buffer , 0 , bytesRead ) );
				} while( bytesRead > 0 );
			}
			return sb.ToString();
		}
		static string[] ExtractListFromHtml( string sb )
		{
			List<string> lst = new List<string>();

			if( sb.Length > 0 )
			{
				string http = HttpUtility.HtmlDecode( sb.ToString() );
				for( int i = 1 ; i < 200 ; i++ )
				{
					string tag = $"index={i}";
					int at = http.IndexOf( tag );
					if( at > 0 )
					{
						int from = http.LastIndexOf( "/watch?v=" , at );
						lst.Add( http.Substring( from , at - from + tag.Length ) );
					}
				}
			}
			return lst.ToArray();
		}
		static string[] DownloadPlaylist( string uri )
		{

			HttpWebRequest w_request = null;
			WebResponse w_response = null;
			w_request.ServicePoint.MaxIdleTime = 1000;
			w_request.KeepAlive = false;
			w_request.Timeout = 300000;

			for( int attempt = 0 ; attempt < 10 ; attempt++ )
				try
				{
					w_request = (HttpWebRequest)WebRequest.Create( uri );
					if( w_request != null )
					{
						w_response = w_request.GetResponse();
						if( w_response != null )
						{
							using( Stream streamweb = w_response.GetResponseStream() )
								return ExtractListFromHtml( ReadStream( streamweb ) );
						}
						break;
					}
				}
				catch( WebException ex )
				{
					if( ex.Status == WebExceptionStatus.ProtocolError )
					{
						Console.Error.WriteLine( "Error --> skip task and restart after several hours" );
						break;
					}
					if( ex.Status == WebExceptionStatus.Timeout )
					{
						Console.Error.WriteLine( "WARNING -- > wait a few secs" );
						System.Threading.Thread.Sleep( 2000 );
					}
					else if( ex.Status == WebExceptionStatus.Pending )
					{
						w_request.Timeout += 10000;
						Console.Error.WriteLine( $"WARNING -- > Increase timeout to {w_request.Timeout} sec" );
					}
					else
						Console.Error.WriteLine( "EXCEPTION: " + ex.Message );
				}
				catch( IOException ex )
				{
					if( ex.HResult == -2146232800 )  //	Received an unexpected EOF or 0 bytes from the transport stream.
					{
						Console.Error.WriteLine( " -- > wait a few secs" );
						System.Threading.Thread.Sleep( 2000 );
					}
					else
					{
						Console.Error.WriteLine( "IOEXCEPTION: " + ex.Message );
						break;
					}
				}
				catch( Exception ex )
				{
					Console.Error.WriteLine( $"{ex.GetType().FullName}: {ex.Message}" );
					break;
				}
				finally
				{
					w_response?.Close();
					w_response?.Dispose();
					w_response = null;

					w_request?.Abort();
					w_request = null;

					GC.Collect();
					GC.WaitForPendingFinalizers();
					GC.Collect();
				}
			return Array.Empty<string>();
		}
		#endregion

		static void OutputMediaInfo( string url )
		{
			int at = url.IndexOf( "?v=" );
			if( at < 0 )
				return;

			string VideoId = url.Substring( url.IndexOf( "?v=" ) + 3 );
			at = VideoId.IndexOf( "/" );
			if( at > 0 )
				VideoId = VideoId.Substring( 0 , at );

			IEnumerable<YouTubeVideo> videos = YouTube.Default.GetAllVideos( url );

			if( videos.Count() > 0 )
			{
				Console.Out.WriteLine( $"{VideoId}\t\"{videos.First().Title}\"\t\"{videos.First().Info.Author}\"\t{videos.Count()}" );
				foreach( YouTubeVideo item in videos )
				{
					Console.Out.WriteLine( $"{VideoId}\t{item.AdaptiveKind}\t{item.AudioFormat}\t{item.AudioBitrate}\t{item.Format}\t{item.Resolution}\t{item.Uri}" );
				}
			}
		}

		static void Main( string[] args )
		{
			foreach( string url in args )
				if( url.ToLower().IndexOf( "http" ) == 0 )
					if( url.Contains( "&list=" ) )
						foreach( string url_ in DownloadPlaylist( url ) )
							OutputMediaInfo( url_ );
					else
						OutputMediaInfo( url );
#if TT
						Console.Out.Write( new MediaInfo( url_ ) );
					else
						Console.Out.Write(	new MediaInfo( url ) );
#endif
		}
	}
}
