using Google.Apis.YouTube.v3.Data;

using Medalorg_POC;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using VideoLibrary;

using YoutubeTranscriptApi;

using static Google.Apis.Requests.BatchRequest;
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
		public string url { get; set; }
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
					return new MediaFile[] { new MediaFile { Audio = BestAudio , url = uri } , new MediaFile { Video = Bestmpeg , url = uri } , new MediaFile { Audio = BestAudio , Video = Bestmpeg , url = uri } };
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
		static MediaFile[] GetVideoList( string url , AudioFormat preferredAudio = AudioFormat.Opus , VideoFormat prefrerredVideo = VideoFormat.WebM )
		{
			List<MediaFile> lst = new List<MediaFile>();

			foreach( string uri in DownloadPlaylist( url ) )
				lst.AddRange( GetMedia( uri , preferredAudio , prefrerredVideo ) );

			return lst.ToArray();
		}

		/// <summary>
		/// What: Http client global thread object.
		/// Why: We can have only one instance per thread to prevent the waste of sockets by reusing them
		/// Note: Do not dispose of or wrap your HttpClient in a using unless you explicitly are looking for a particular behaviour (such as causing your services to fail).
		/// reference :
		/// https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
		/// </summary>
		private static HttpClient WebClientRequest = new HttpClient( new HttpClientHandler() { UseDefaultCredentials = true } );
		static readonly HttpClient client = new HttpClient() { Timeout = TimeSpan.FromSeconds( 300 ) };
		/// <summary>
		/// What Global to the thread
		/// Why: Prevent allocating all the time a buffer ans stressing the memory.
		/// </summary>
		static byte[] buffer = new byte[16 * 1024];
		static bool DownloadFromStream( YouTubeVideo media , int attempt , Stream streamweb , string filename )
		{
			int total = 0;
			int bytesRead = 0;
			using( var tempFile = new TemporaryFile() )
			{
				using( FileStream fileStream = System.IO.File.Create( tempFile.FilePath ) )
				{
					do
					{
						bytesRead = streamweb.Read( buffer , 0 , buffer.Length );
						if( bytesRead > 0 )
							fileStream.Write( buffer , 0 , bytesRead );

						total += bytesRead;
						Console.Write( $"\r{attempt}) {media.FullName} ({media.ContentLength / 1024:#,###,###,###} kb - {Math.Round( ((double)total / (int)media.ContentLength) * 100 , 2 ):00.00}%) {total / 1024:#,###,###,###} Kb.  " );

					} while( bytesRead > 0 );
				}
				System.IO.File.Move( tempFile.FilePath , filename );
				return true;
			}
		}
		static int counter = 0;
		static private bool WebDownload( YouTubeVideo media , string filename )
		{
			bool ans = false;
			int timeout = 1000;
			byte[] buffer = new byte[16 * 1024];

			WebResponse w_response = null;
			for( int attempt = 0 ; attempt < 10 && !ans ; attempt++ )
			{
				try
				{
					WebRequest w_request = WebRequest.Create( media.Uri );
					if( w_request != null )
					{
						w_request.Timeout = timeout;
						w_response = w_request.GetResponse();
						if( w_response != null )
						{
							using( Stream streamweb = w_response.GetResponseStream() )
							{
								int total = 0;
								int bytesRead = 0;
								using( MemoryStream ms = new MemoryStream( media.ContentLength is null || media.ContentLength < buffer.Length ? buffer.Length : (int)(media.ContentLength) ) )
								{
									do
									{
										bytesRead = streamweb.Read( buffer , 0 , buffer.Length );
										if( bytesRead > 0 )
											ms.Write( buffer , 0 , bytesRead );

										total += bytesRead;
										Console.Write( $"\r{attempt}) {media.FullName} ({media.ContentLength / 1024:#,###,###,###} kb - {Math.Round( ((double)total / (int)media.ContentLength) * 100 , 2 ):00.00}%) {total / 1024:#,###,###,###} Kb.                        " );

									} while( bytesRead > 0 );
									using( FileStream fs = new FileStream( filename , FileMode.Create ) )
									{
										ms.CopyTo( fs );
										fs.Flush();
									}
								}
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
						Console.Write( $"\r{attempt}) {media.FullName} ({media.ContentLength / 1024:#,###,###,###} kb) --> Protocol error... abort any attempts and restart much later\r" );
						break;
					}
					else if( ex.Status == WebExceptionStatus.Timeout )
					{
						Console.Write( $"\r{attempt}) {media.FullName} ({media.ContentLength / 1024:#,###,###,###} kb) --> timeout....                     \r" );
						timeout *= 2;
					}
					else if( ex.Status == WebExceptionStatus.Pending )
					{
						Console.Write( $"\r{attempt}) {media.FullName} ({media.ContentLength / 1024:#,###,###,###} kb) --> wait a {timeout / 1000} secs    \r" );
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
						Console.Write( $"\r{attempt}) {media.FullName} ({media.ContentLength / 1024:#,###,###,###} kb) --> wait a {timeout / 1000} secs\r" );
						System.Threading.Thread.Sleep( timeout );
					}
					else
					{
						//	Log the error for investigation
						Console.WriteLine( "\n*******(IO) " + ex );
						break;
					}
				}
				catch( Exception ex )
				{
					//	Log the error for investigation
					if( ex.HResult == -2146233029 )
						Console.WriteLine( $"  --> {ex.Message}" );
					else
						Console.WriteLine( "\n******* " + ex );
					break;
				}
				finally
				{
					//w_response?.Close();
				}
			}
			return ans;
		}
		/// <summary>
		/// What: download either an audio or a video file
		/// Why: the ultimate purpose is to create a video media file.
		/// Note: the file will only download if the file does not exists of if the file length is different (in that case the existing file will be deleted)
		/// reference :
		/// https://www.bing.com/search?form=&q=c%23+crelease+WebRequest.Create&form=QBLH&sp=-1&lq=0&pq=c%23+crelease+webrequest.create&sc=0-29&qs=n&sk=&cvid=38A275BB796340A495B26BA1667F698D&ghsh=0&ghacc=0&ghpl=
		/// https://stackoverflow.com/questions/71553075/webrequest-createstring-is-obsolete-webrequest-use-httpclient-instead
		/// </summary>
		/// <param name="media"></param>
		/// <param name="filename"></param>
		/// <returns>true if the file fully downloaded</returns>
		//		static async Task<bool> DownloadMedia( YouTubeVideo media , string filename )
		static bool DownloadMedia( YouTubeVideo media , string filename )
		{
			bool ans = false;

			Console.Write( $"{counter}) {media.FullName} ({media.ContentLength / 1024:#,###,###,###} kb)" );
			if( !System.IO.File.Exists( filename ) )
			{
#if HTTP
				for( int attempt = 1 ; attempt < 2 && !ans ; attempt++ )
				{
					try
					{
						using( Task<HttpResponseMessage> httpresponse = WebClientRequest.GetAsync( media.Uri ) )
						//using( HttpResponseMessage httpmsg = WebClientRequest.GetAsync( media.Uri ).GetAwaiter().GetResult() )
						{
							using( HttpResponseMessage httpmsg = httpresponse.GetAwaiter().GetResult() )
							{
								if( httpmsg.StatusCode == HttpStatusCode.OK )
								{
									httpmsg.EnsureSuccessStatusCode();
									using( Stream stream = httpmsg.Content.ReadAsStreamAsync().GetAwaiter().GetResult() )
									{
										using( Stream fs = System.IO.File.Create( filename ) )
											stream.CopyTo( fs );
										/*
										using( var tempFile = new TemporaryFile() )
										{
											using( FileStream fileStream = System.IO.File.Create( tempFile.FilePath ) )
											{
												int bytesRead = 0;
												int total = 0;
												do
												{
													bytesRead = stream.Read( buffer , 0 , buffer.Length );
													total += bytesRead;
													if( bytesRead > 0 )
														Console.Write( $"\r{attempt}) {media.FullName} ({media.ContentLength / 1024:#,###,###,###} kb - {Math.Round( ((double)total / (int)media.ContentLength) * 100 , 2 ):00.00}%) {total / 1024:#,###,###,###} Kb.  											" );
												} while( bytesRead > 0 );

												//stream.CopyTo( fileStream );
												ans = true;
												Console.Write( $"\r{attempt}) {media.FullName} ({media.ContentLength / 1024} kb) --> Completed" );
											}
											System.IO.File.Copy( tempFile.FilePath , filename );
										}
										*/
									}
								}
								else if( httpmsg.StatusCode == HttpStatusCode.Forbidden )
								{
									Console.Write( $"\r{attempt}) {media.FullName} ({media.ContentLength / 1024:#,###,###,###} kb --> {httpmsg.StatusCode}: ERR 403 the server refuse to fulfill the request" );
									break;
								}
								else
								{
									Console.Write( $"\r{media.FullName} ({media.ContentLength / 1024} kb) --> {httpmsg.StatusCode}\r" );
								}
							}
						}
					}
					catch( HttpRequestException ex )
					{
						Console.WriteLine( $"\r{attempt}) {media.FullName} ({media.ContentLength / 1024:#,###,###,###} kb  --> HttpRequestException: {ex.Message}" );
					}
					catch( Exception ex )
					{
						//	Log the error for investigation
						if( ex.HResult == -2146233029 )
						{
							Console.Write( $"\r{attempt}) {media.FullName} ({media.ContentLength / 1024:#,###,###,###} kb  --> Exception: {ex.Message}" );
							System.Threading.Thread.Sleep( 1000 );
						}
						else
						{
							Console.Write( "\n******* " + ex );
							break;
						}
					}
					finally
					{
						Console.WriteLine();
					}
				}
			}
#endif
				int timeout = 1000;

#if TT
				for( int attempt = 0 ; attempt < 10 ; attempt++ )
				{
					try 
					{
						using( HttpResponseMessage response = await client.GetAsync( media.Uri ) )
						{
							response.EnsureSuccessStatusCode();
							using( Stream streamweb = await response.Content.ReadAsStreamAsync() )
							{
								DownloadFromStream( media , attempt , streamweb , filename );
							}
						}
#endif
				HttpWebRequest w_request = (HttpWebRequest)WebRequest.Create( media.Uri );
				w_request.ServicePoint.MaxIdleTime = 1000;
				for( int attempt = counter ; attempt < counter + 10 ; attempt++ )
				{
					HttpWebResponse w_response = null;
					try
					{
						if( w_request != null )
						{
							//	https://stackoverflow.com/questions/716436/is-there-a-correct-way-to-dispose-of-a-httpwebrequest
							w_request.KeepAlive = false;
							w_request.Timeout = 300000;
							w_response = (HttpWebResponse)w_request.GetResponse();
							if( w_response != null )
							{
								using( Stream streamweb = w_response.GetResponseStream() )
								{
									DownloadFromStream( media , attempt , streamweb , filename );
									streamweb.Close();
								}
								w_response.Close();
							}
						}
						ans = true;
						Console.Write( "  OK" );
						counter += 1;
						break;
					}
					catch( WebException ex )
					{
						if( ex.Status == WebExceptionStatus.ProtocolError )
						{
							Console.Write( $"\r{attempt}) {media.FullName} ({media.ContentLength / 1024:#,###,###,###} kb) --> Protocol error... abort any attempts and restart much later\r" );
							break;
						}
						else if( ex.Status == WebExceptionStatus.Timeout )
						{
							Console.Write( $"\r{attempt}) {media.FullName} ({media.ContentLength / 1024:#,###,###,###} kb) --> timeout....                     \r" );
							timeout *= 2;
						}
						else if( ex.Status == WebExceptionStatus.Pending )
						{
							Console.Write( $"\r{attempt}) {media.FullName} ({media.ContentLength / 1024:#,###,###,###} kb) --> wait a {timeout / 1000} secs    \r" );
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
							Console.Write( $"\r{attempt}) {media.FullName} ({media.ContentLength / 1024:#,###,###,###} kb) --> wait a {timeout / 1000} secs\r" );
							System.Threading.Thread.Sleep( timeout );
						}
						else
						{
							//	Log the error for investigation
							Console.WriteLine( "\n*******(IO) " + ex );
							break;
						}
					}
					catch( Exception ex )
					{
						//	Log the error for investigation
						Console.WriteLine( "\n******* " + ex );
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
				}
			}
			else
				ans = true;
			Console.WriteLine();
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
		static bool DownloadSubtitle( MediaFile media , string lang , string fname )
		{
			string langlist = lang;
			foreach( string LanguageCode in lang.Split( ',' ) )
				if( System.IO.File.Exists( $"{fname}.{LanguageCode}.srt" ) )
					langlist = langlist.Replace( LanguageCode , "" );
			langlist = langlist.Replace( ",," , "," ).Trim();

			bool ans = string.IsNullOrEmpty( langlist );

			if( !ans )
			{
				using( YouTubeTranscriptApi uttla = new YouTubeTranscriptApi() )
				{
					try
					{
						TranscriptList tl = uttla.ListTranscripts( media.url.Substring( media.url.IndexOf( '=' ) + 1 ) );
						foreach( Transcript t in tl )
						{
							if( langlist.Contains( t.LanguageCode ) )
							{
								string filename = $"{fname}.{t.LanguageCode}.srt";
								Console.Write( $"\nVideo_id = {t.VideoId}, Language {t.Language}" );
								int i = 1;
								if( System.IO.File.Exists( $"{fname}.{t.LanguageCode}.srt" ) )
								{
									while( System.IO.File.Exists( $"{fname}.{i}.{t.LanguageCode}.srt" ) )
										i += 1;
									filename = $"{fname}.{i}.{t.LanguageCode}.srt";

								}
								i = 1;
								using( FileStream fs = new FileStream( filename , FileMode.Create ) )
								{
									foreach( TranscriptItem ti in t.Fetch() )
									{
										DateTime from = new DateTime( ti.Start * 10 * 1000 );
										//	Console.WriteLine( $"{i++}\n{from:HH:mm:ss,fff} --> {from.AddMilliseconds( ti.Duration ):HH:mm:ss,fff}\n{ti.Text}\n" );
										byte[] txt = Encoding.UTF8.GetBytes( string.Format( $"{(i > 1 ? "\n" : "")}{i++}\n{from:HH:mm:ss,fff} --> {from.AddMilliseconds( ti.Duration ):HH:mm:ss,fff}\n{ti.Text}\n" ) );
										fs.Write( txt , 0 , txt.Length );
									}
									ans = true;
									Console.Write( " -> " + new FileInfo( filename ).Name );
								}
							}
						}
					}
					catch( Exception )
					{
					}
					finally
					{
						Console.WriteLine();
					}
				}
			}
			return ans;
		}
		static void Main( string[] args )
		//	static async Task Main( string[] args )
		{
			TargetPath = new DirectoryInfo( "." ).CreateSubdirectory( "Media" ).FullName;

			List<string> activeuri = new List<string>()
				{
					"https://www.youtube.com/watch?v=_N7DVBpHh18"
					/*
					"https://www.youtube.com/watch?v=_N7DVBpHh18&list=PLcWEcJ1nmiTNnA6kyI0hRiBTxMYq3r9r_&index=3"
					"https://www.youtube.com/watch?v=Cfn6Nbd1UQU",
					"https://www.youtube.com/watch?v=sveQ2cuTthQ",
					"https://www.youtube.com/watch?v=16y1AkoZkmQ&list=PLGBuKfnErZlAth3g6feG58mrqNQj8p32c"
					"https://www.youtube.com/watch?v=tFMg0_bLO58&t=378s",
					"https://www.youtube.com/watch?v=xFrGuyw1V8s&list=PLGBuKfnErZlCnsp1WWMqy-LHlvpCj_RuX",
					"https://www.youtube.com/watch?v=wTP2RUD_cL0&list=RDwTP2RUD_cL0",
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
					*/
				};
			List<MediaFile> VideoList = new List<MediaFile>();
			foreach( string uri in activeuri )
				if( uri.Contains( "&list=" ) )
					VideoList.AddRange( GetVideoList( uri , AudioFormat.Opus , VideoFormat.Mp4 ) );
				else
					VideoList.AddRange( GetMedia( uri , AudioFormat.Opus , VideoFormat.Mp4 ) );

			while( VideoList.Count > 0 )
			{
				MediaFile[] medialist = VideoList.ToArray();

				foreach( MediaFile media in medialist )
					if( media.Video != null && media.Audio != null )
					{
						if( combine( ToFullfilename( media.Audio ) , ToFullfilename( media.Video ) ) )
						{
							VideoList.Remove( media );
							DownloadSubtitle( media , "en,fr" , TargetPath + $@"\{ToName( media.Video )}" );
						}
						GC.Collect( GC.MaxGeneration , GCCollectionMode.Forced );
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

				try
				{
					using( Task<HttpResponseMessage> httpresponse = WebClientRequest.GetAsync( uri ) )
					{
						HttpResponseMessage httpmsg = httpresponse.GetAwaiter().GetResult();
						if( httpmsg.StatusCode == HttpStatusCode.OK )
						{
							using( Stream stream = httpmsg.Content.ReadAsStreamAsync().GetAwaiter().GetResult() )
							{
								byte[] buffer = new byte[16 * 1024];
								int bytesRead = 0;
								do
								{
									bytesRead = stream.Read( buffer , 0 , buffer.Length );
									if( bytesRead > 0 )
										sb.Append( Encoding.Default.GetString( buffer , 0 , bytesRead ) );
								} while( bytesRead > 0 );
							}
						}
						else
						{
							Console.WriteLine( $" --> {httpmsg.StatusCode}" );
						}
					}
				}
				catch( HttpRequestException ex )
				{
					Console.WriteLine( "  ******* " + ex );
				}
				catch( Exception ex )
				{
					//	Log the error for investigation
					if( ex.HResult == -2146233029 )
					{
						Console.WriteLine( " ******* " + ex.Message );
						System.Threading.Thread.Sleep( 1000 );
					}
					else
					{
						Console.WriteLine( " ******* " + ex );
						break;
					}
				}
				finally
				{
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

#if TT
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
						streamweb = w_response.GetResponseStream();
						byte[] buffer = new byte[16 * 1024];
						int bytesRead = 0;
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
#endif
		}
	}
}

