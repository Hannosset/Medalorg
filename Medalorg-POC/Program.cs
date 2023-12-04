using Google.Apis.YouTube.v3.Data;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using VideoLibrary;

using YoutubeTranscriptApi;

using static System.Collections.Specialized.BitVector32;
using static System.Net.WebRequestMethods;
//	Install-Package VideoLibrary
//		https://stackoverflow.com/questions/44126644/download-you-tube-video-using-c
//
//	https://www.youtube.com/watch?v=_3scLqnUU3Y&list=RDEMOkYeH-TEy6z6ucRChOXTAQ&index=26
//	https://www.bing.com/search?q=c%23+youtube+subtitle+downloader&qs=n&form=QBRE&sp=-1&lq=0&pq=c%23+youtube+subtitle+downloader&sc=4-30&sk=&cvid=9858C1BA249F4F628E643F4F50162FE1&ghsh=0&ghacc=0&ghpl=
/*
 * subtitles
 * Install-Package Google.Apis.YouTube.v3
 *		https://github.com/jdepoix/youtube-transcript-api/blob/master/youtube_transcript_api/_api.py
 *		https://github.com/BobLd/youtube-transcript-api-sharp
 * 
 * example: https://developers.google.com/api-client-library/dotnet/guide/aaa_oauth#installed-applications
 * https://www.nuget.org/packages/Google.Apis.YouTube.v3/
 * https://stackoverflow.com/questions/30133501/youtube-v3-api-caption-download-using-sdk-nuget-package
 * https://stackoverflow.com/questions/46255952/youtube-v3-api-captions-downloading
 */

/*
 * Get url from browser:
 * https://stackoverflow.com/questions/3579649/get-url-from-browser-to-c-sharp-application
 * https://social.msdn.microsoft.com/Forums/en-US/8fa6e4cf-1a8a-4f88-90fe-b7aecb3f51c5/to-get-the-url-from-the-address-bar-using-c-windows-application?forum=ncl
 * 
 * From thwe clipboard:
 * https://www.bing.com/search?q=c%23+Get+the+clipboard+data&qs=n&form=QBRE&sp=-1&lq=0&pq=c%23+get+the+clipboard+data&sc=11-25&sk=&cvid=5F9E6E96B6314B90A6AE7729CF69D333&ghsh=0&ghacc=0&ghpl=
 */
namespace VideoLibrary
{
	public class MediaFile
	{
		public YouTubeVideo Audio { get; set; }
		public YouTubeVideo Video { get; set; }
	}

	class Program
	{
		static string TargetPath;

		/// <summary>
		/// What: extract the Video_ID of a URL
		/// Why: the id is unique and we might need to have a unique key unifying the audio, video.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		static string GetVideoId( string uri )
		{
			string videoId = uri.Replace( "/watch?v=" , "" ).Replace( "\\u0026" , "&" );

			int at = videoId.IndexOf( "&" );
			if( at > 0 )
				videoId = videoId.Substring( 0 , at - 1 );

			return videoId;
		}
		/// <summary>
		/// What: convert a video title to a filename
		/// Why: need to remove all characters that are not accepted in the OS filename
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		static string ToName( YouTubeVideo item )
		{
			string fname = item.Title.Replace( "\t" , " " ).Replace( "\f" , "" ).Replace( "\n" , "" ).Replace( "\r" , "" )
							.Replace( "\"" , "'" ).Replace( "|" , "-" ).Replace( "\\" , "-" ).Replace( "/" , "-" ).Replace( ":" , "" ).Replace( "*" , "" ).Replace( "?" , "" )/*.Replace( "!" , "" )*/.Replace( "<" , "" ).Replace( ">" , "" )
							.Replace( "HQ Audio" , "" ).Replace( "_" , " " );

			int at = fname.IndexOf( '(' );
			if( at != -1 )
				fname = fname.Replace( fname.Substring( at - 1 , fname.IndexOf( ')' ) - at + 2 ) , "" );
			return fname.Trim();
		}
		/// <summary>
		/// Whar: convert a video title to a valid windows filename
		/// Why: the default convertion adds numerous '_' and we want to control the extenstion as well as the full target pathname.
		/// </summary>
		/// <param name="item">YouTubeVideo object</param>
		/// <returns></returns>
		static string ToFilename( YouTubeVideo item )
		{
			string fname = ToName( item );
			if( item.AdaptiveKind == AdaptiveKind.Audio )
				switch( item.AudioFormat )
				{
					case AudioFormat.Aac: fname += ".aac"; break;
					case AudioFormat.Vorbis: fname += ".ogg"; break;
					case AudioFormat.Opus: fname += ".opus"; break;
					default: fname += ".mp3"; break;
				}
			else switch( item.Format )
				{
					default:
					case VideoFormat.Mp4: fname += ".mpeg"; break;
					case VideoFormat.WebM: fname += ".mpeg"; break;
				}
			return fname;
		}
		/// <summary>
		/// What: prefix the target path to the filename
		/// Why: whe we are merging files to produce an mkv
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		static string ToFullfilename( string filename ) => TargetPath + "\\" + filename;
		static string ToFullfilename( YouTubeVideo item ) => TargetPath + "\\" + ToFilename( item );

		/// <summary>
		/// What: from a url extract the video information provided by the libvideo external dll
		/// why: filter the existring video from the list and allows to download each audio and mpeg file concurrently.
		/// </summary>
		/// <param name="uri">of a video</param>
		/// <returns>If the video is not available, the function return an empty array</returns>
		static MediaFile[] GetMedia( string uri , AudioFormat preferredAudio = AudioFormat.Opus , VideoFormat prefrerredVideo = VideoFormat.WebM )
		{
			try
			{
				IEnumerable<YouTubeVideo> videos = YouTube.Default.GetAllVideos( uri );
				Console.Write( $"GetMedia( {GetVideoId( uri )} )" );
				int hightaudio = 1;
				int hightvideo = 1;

				YouTubeVideo BestAudio = null, Bestmpeg = null;

				foreach( YouTubeVideo item in videos )
				{
					if( item.AdaptiveKind == AdaptiveKind.Audio && item.AudioBitrate > hightaudio )
					{
						if( hightaudio < item.AudioBitrate )
						{
							hightaudio = item.AudioBitrate;
							BestAudio = item;
						}
						else if( BestAudio.AudioBitrate == item.AudioBitrate && BestAudio.AudioFormat == preferredAudio )
							BestAudio = item;
					}
					else if( item.Resolution >= 360 && item.Resolution <= 1080 )
					{
						if( hightvideo < item.Resolution )
						{
							hightvideo = item.Resolution;
							Bestmpeg = item;
						}
						else if( item.Resolution == Bestmpeg.Resolution && item.Format == prefrerredVideo )
							Bestmpeg = item;

					}
				}

				if( BestAudio != null && Bestmpeg != null )
				{
					Console.WriteLine( $" -> {ToName( BestAudio )}  OK" );
					return new MediaFile[] { new MediaFile { Audio = BestAudio } , new MediaFile { Video = Bestmpeg } , new MediaFile { Audio = BestAudio , Video = Bestmpeg } };
				}

				Console.WriteLine( "  NOK" );
			}
			catch( Exception ex )
			{
				Console.WriteLine( $"Error {ex.Message}" );
			}
			return new MediaFile[] { };
		}
		/// <summary>
		/// What: download the video ID from a playlist
		/// Why: we want to be able to download the content of playlists.
		/// </summary>
		/// <param name="url">Playlist url</param>
		/// <returns>an array of youtube video</returns>
		static MediaFile[] GetVideoList( string url )
		{
			List<MediaFile> lst = new List<MediaFile>();

			foreach( string uri in DownloadPlaylist( url ) )
				lst.AddRange( GetMedia( uri ) );

			return lst.ToArray();
		}
		/// <summary>
		/// What: download either an audio or a video file
		/// Why: the ultimate purpose is to create a video media file.
		/// Note: the file will only download if the file does not exists of if the file length is different (in that case the existing file will be deleted)
		/// </summary>
		/// <param name="media"></param>
		/// <param name="filename"></param>
		/// <returns>true if the file fully downloaded</returns>
		static bool DownloadMedia( YouTubeVideo media , string filename )
		{
			bool ans = false;

			Console.WriteLine( $"\n{media.FullName} ({media.ContentLength/1024} kb)" );
			if( !System.IO.File.Exists( filename ) )
			{
				int timeout = 1000;
				byte[] buffer = new byte[16 * 1024];

				for( int attempt = 0 ; attempt < 10 ; attempt++ )
				{
					int total = 0;
					Stream streamweb = null;
					WebResponse w_response = null;
					try
					{
						WebRequest w_request = WebRequest.Create( media.Uri );
						if( w_request != null )
						{
							w_request.Timeout = timeout;
							w_response = w_request.GetResponse();
							if( w_response != null )
							{
								streamweb = w_response.GetResponseStream();

								int bytesRead = 0;
								MemoryStream ms = new MemoryStream( media.ContentLength is null || media.ContentLength < buffer.Length ? buffer.Length : (int)(media.ContentLength) );
								try
								{
									do
									{
										bytesRead = streamweb.Read( buffer , 0 , buffer.Length );
										if( bytesRead > 0 )
											ms.Write( buffer , 0 , bytesRead );

										total += bytesRead;
										Console.Write( $"\r{attempt}) {media.FullName} ({media.ContentLength / 1024:#,###,###,###} kb - {Math.Round( ((double)total / (int)media.ContentLength) * 100 , 2 ):00.00}%) {total / 1024:#,###,###,###} Kb.                        " );

									} while( bytesRead > 0 );
								}
								catch( Exception ex )
								{
									throw ex;
								}
								FileStream fs = null;
								try
								{
									fs = new FileStream( filename , FileMode.Create );
									fs.Write( ms.GetBuffer() , 0 , (int)ms.Length );
									fs.Flush();
									fs.Close();
								}
								catch( Exception ex )
								{
									//	Delete the file if already created
									fs?.Close();
									System.IO.File.Delete( filename );
									throw ex;
								}
								finally
								{
									fs?.Dispose();

									//	Release the memory
									ms?.Close();
									ms?.Dispose();
								}
							}
						}
						ans = true;
						break;
					}
					catch( WebException ex )
					{
						if( ex.Status == WebExceptionStatus.ProtocolError )
						{
							Console.Write( $" --> Protocol error... abort any attempts and restart much later\r" );
							break;
						}
						else if( ex.Status == WebExceptionStatus.Timeout )
						{
							Console.Write( $" --> timeout....                     \r" );
							timeout *= 2;
						}
						else if( ex.Status == WebExceptionStatus.Pending )
						{
							Console.Write( $" --> wait a {timeout / 1000} secs    \r" );
							timeout *= 2;
							System.Threading.Thread.Sleep( timeout );
						}
						else
							Console.WriteLine( "\n\n" + ex );
					}
					catch( IOException ex )
					{
						if( ex.HResult == -2146232800 )  //	Received an unexpected EOF or 0 bytes from the transport stream.
						{
							Console.Write( $" --> wait a {timeout / 1000} secs\r" );
							System.Threading.Thread.Sleep( timeout );
						}
						else
						{
							//	Log the error for investigation
							Console.WriteLine( "*******(IO) " + ex );
							break;
						}
					}
					catch( Exception ex )
					{
						//	Log the error for investigation
						Console.WriteLine( "******* " + ex );
						break;
					}
					finally
					{
						w_response?.Close();
						streamweb?.Close();
					}
				}
			}

			return ans;
		}
		//	https://ffmpeg.org/download.html#build-windows
		//	https://github.com/BtbN/FFmpeg-Builds/releases
		static bool combine( string audiotitle , string videotitle )
		{
			if( !System.IO.File.Exists( videotitle.Substring( 0 , videotitle.LastIndexOf( '.' ) ) + ".mkv" ) )
			{
				if( System.IO.File.Exists( audiotitle ) && System.IO.File.Exists( videotitle ) )
				{
					Process p = new Process();
					p.StartInfo.FileName = new FileInfo( @".\ffmpeg.exe" ).FullName;
					p.StartInfo.Arguments = "-v 0 -y -max_error_rate 0.0 -i \"" + $"{audiotitle}\" -i \"" + $"{videotitle}\" -preset veryfast " + $"\"{videotitle.Substring( 0 , videotitle.LastIndexOf( '.' ) )}.mkv\"";

					if( videotitle.Contains( ".aac" ) )
						Console.WriteLine( p.StartInfo.Arguments );

					p.Start();
					p.WaitForExit();

					return p.ExitCode == 0;
				}
				return false;
			}
			return true;
		}
		static bool DownloadSubtitle( string uri , string lang , string fname )
		{
			bool ans = false;

			using( YouTubeTranscriptApi uttla = new YouTubeTranscriptApi() )
			{
				FileStream fs = null;

				try
				{
					TranscriptList tl = uttla.ListTranscripts( uri.Substring( uri.IndexOf( '=' ) + 1 ) );
					foreach( Transcript t in tl )
					{
						if( t.LanguageCode == lang )
						{
							Console.Write( $"\rVideo_id = {t.VideoId}, Language {t.Language}" );
							fs = new FileStream( $"{fname}.{t.LanguageCode}.srt" , FileMode.Create );

							int i = 1;
							foreach( TranscriptItem ti in t.Fetch() )
							{
								DateTime from = new DateTime( ti.Start * 10 * 1000 );
								//	Console.WriteLine( $"{i++}\n{from:HH:mm:ss,fff} --> {from.AddMilliseconds( ti.Duration ):HH:mm:ss,fff}\n{ti.Text}\n" );
								byte[] txt = Encoding.UTF8.GetBytes( string.Format( $"{(i > 1 ? "\n" : "")}{i++}\n{from:HH:mm:ss,fff} --> {from.AddMilliseconds( ti.Duration ):HH:mm:ss,fff}\n{ti.Text}\n" ) );
								fs.Write( txt , 0 , txt.Length );
							}
							ans = true;
							Console.WriteLine( " -> " + new FileInfo( $"{fname}.{t.LanguageCode}.srt" ).Name );
							break;
						}
					}
				}
				catch( Exception )
				{
				}
				finally
				{
					if( fs != null )
						fs.Close();
				}
			}
			return ans;
		}
		static void Main( string[] args )
		{
			TargetPath = new DirectoryInfo( "." ).CreateSubdirectory( "Media" ).FullName;

			List<string> activeuri = new List<string>()
				{
					"https://www.youtube.com/watch?v=wTP2RUD_cL0&list=RDwTP2RUD_cL0",
					"https://www.youtube.com/watch?v=NuZklVrHspM&list=PLDA56F24B0A270792",
					"https://www.youtube.com/watch?v=i2wmKcBm4Ik&list=PLrl15fpG8H1yY7UOoO2kJGo_yJ9UGX-ox",
					"https://www.youtube.com/watch?v=XPn52kRQx3k&list=PLDintB9nu_R505y2Z7673a57-x4pbqLVa",
					"https://www.youtube.com/watch?v=SMR8S154_zA&list=PLMmHE6UVFkH_YUAIHCnZi5jKWbPra7qrb",
					 "https://www.youtube.com/watch?v=BlD6jCGVU4A",
					 "https://www.youtube.com/watch?v=BUavFsfbFv8",
					 "https://www.youtube.com/watch?v=kd9TlGDZGkI",
					 "https://www.youtube.com/watch?v=8Pa9x9fZBtY",
					 "https://www.youtube.com/watch?v=LRSeDLtMq1k",
					 "https://www.youtube.com/watch?v=jhdFe3evXpk",
					 "https://www.youtube.com/watch?v=gAirINwjaxE",
					 "https://www.youtube.com/watch?v=h0ffIJ7ZO4U",
				};
			List<MediaFile> VideoList = new List<MediaFile>();
			foreach( string uri in activeuri )
				if( uri.Contains( "&list=" ) )
					VideoList.AddRange( GetVideoList( uri ) );
				else
					VideoList.AddRange( GetMedia( uri ) );

			while( VideoList.Count > 0 )
			{
				MediaFile[] medialist = VideoList.ToArray();

				foreach( MediaFile media in medialist )
					if( media.Video != null && media.Audio != null )
					{
						if( combine( ToFullfilename( media.Audio ) , ToFullfilename( media.Video ) ) )
							VideoList.Remove( media );
					}
					else if( media.Audio != null )
					{
						if( DownloadMedia( media.Audio , ToFullfilename( media.Audio ) ) )
							VideoList.Remove( media );
					}
					else if( media.Video != null )
					{
						if( DownloadMedia( media.Video , ToFullfilename( media.Video ) ) )
							VideoList.Remove( media );
					}
			}

			Console.WriteLine( "press any key to continue..." );
			Console.ReadKey();
			Process.Start( TargetPath );
		}
		static string[] DownloadPlaylist( string uri )
		{
			StringBuilder sb = new StringBuilder();

			for( int timeout = 1000 ; timeout < 60000 ; timeout *= 2 )
			{
				sb.Clear();

				Stream streamweb = null;
				WebResponse w_response = null;
				try
				{
					WebRequest w_request = WebRequest.Create( uri );
					w_request.Timeout = timeout;
					if( w_request != null )
					{
						w_response = w_request.GetResponse();
						if( w_response != null )
						{
							byte[] buffer = new byte[1024 * 1024];
							int bytesRead = 0;
							streamweb = w_response.GetResponseStream();
							do
							{
								bytesRead = streamweb.Read( buffer , 0 , buffer.Length );
								if( bytesRead > 0 )
									sb.Append( Encoding.Default.GetString( buffer , 0 , bytesRead ) );
							} while( bytesRead > 0 );
						}
					}
					break;
				}
				catch( WebException ex )
				{
					if( ex.Status == WebExceptionStatus.ProtocolError )
					{
						Console.WriteLine( " --> skip task and restart after several hours" );
						break;
					}
					if( ex.Status == WebExceptionStatus.Timeout || ex.Status == WebExceptionStatus.Pending || ex.Status == WebExceptionStatus.ProtocolError )
					{
						Console.WriteLine( " -- > wait a few secs" );
						System.Threading.Thread.Sleep( 1000 );
					}
					else
						Console.WriteLine( "\n\n" + ex );
				}
				catch( IOException ex )
				{
					if( ex.HResult == -2146232800 )  //	Received an unexpected EOF or 0 bytes from the transport stream.
					{
						Console.WriteLine( " -- > wait a few secs" );
						System.Threading.Thread.Sleep( 1000 );
					}
					else
						Console.WriteLine( "\n\n" + ex );
				}
				catch( Exception ex )
				{
					Console.WriteLine( "\n\n" + ex );
				}
				finally
				{
					if( w_response != null )
						w_response.Close();
					if( streamweb != null )
						streamweb.Close();
				}
			}

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
	}
}

