using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

using VideoLibrary;

using YoutubeTranscriptApi;
using static System.Net.WebRequestMethods;
//	Install-Package VideoLibrary
//		https://stackoverflow.com/questions/44126644/download-you-tube-video-using-c
//		https://github.com/omansak/libvideo
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
namespace utubdl
{
	class Program
	{
		static string TargetPath;

		static void Download( string url )
		{
			try
			{
				//Console.WriteLine( "Processing you tube " + url );
				string information = "";
				var videos = YouTube.Default.GetAllVideos( url );
				int hightaudio = 1;
				int hightvideo = 1;
				//Console.WriteLine( "\nlist all format \n" );
				string title = string.Empty;
				foreach(var item in videos)
				{
					if(item.AdaptiveKind.ToString() == "Audio" && item.AudioBitrate > hightaudio)
					{
						hightaudio = item.AudioBitrate;
						information = item.AudioFormat + "," + item.AudioBitrate + "," + item.ContentLength;
						//Console.WriteLine( " - ," + item.Format + "," + item.AudioFormat + "," + item.AudioBitrate + "," + item.ContentLength + "," + item.AdaptiveKind );
					}
					else if(item.Resolution >= 360 && item.Resolution > hightvideo && item.Resolution <= 1080)
					{
						hightvideo = item.Resolution;
						if(string.IsNullOrEmpty( title ))
							title = item.Title
								.Replace( "\"" , "\'" ).Replace( "|" , "-" ).Replace( "\\" , "-" ).Replace( "/" , "-" ).Replace( ":" , "" ).Replace( "*" , "" ).Replace( "?" , "" ).Replace( "<" , "" ).Replace( ">" , "" )
								.Replace( "\t" , " " ).Replace( "\f" , "" ).Replace( "\n" , "" ).Replace( "\r" , "" )
								.Replace( "HQ Audio" , "" ).Trim();
						//Console.WriteLine( item.Resolution + "," + item.Format + "," + item.AudioFormat + "," + item.AudioBitrate + "," + item.ContentLength + "," + item.AdaptiveKind );
					}
				}
				if( !System.IO.File.Exists( TargetPath + $@"\{title}.mp3" ) && !System.IO.File.Exists( TargetPath + $@"\{title}.mp4" ) )
					Console.WriteLine( "\ndownload {2}: video resolution {0} and audio bitrate {1}" , hightvideo , hightaudio , title );
				else if(!System.IO.File.Exists( TargetPath + $@"\{title}.mp3" ) )
					Console.WriteLine( "\ndownload {2}: audio bitrate {1}" , hightvideo , hightaudio , title );
				else if( !System.IO.File.Exists( TargetPath + $@"\{title}.mp4" ) )
					Console.WriteLine( "\ndownload {2}: video resolution {0}" , hightvideo , hightaudio , title );

				if(!System.IO.File.Exists( TargetPath + $@"\{title}.mp3" ))
				{
					string[] split = information.Split( ',' );
					foreach(var item in videos)//download audio
					{
						if(split[0] == item.AudioFormat.ToString() && split[1] == item.AudioBitrate.ToString() && split[2] == item.ContentLength.ToString())
						{
							//Console.WriteLine( "\ndownload audio with bitrate {0} and size {1} MB" , item.AudioBitrate , Math.Round( (double)item.ContentLength / (1024 * 1024.0) , 2 ) );
							if(downloadbest( item , TargetPath + $@"\{title}.mp3" ))
								Console.Write( "\rAudio downloaded                                                               " );
							else
								return;
						}
					}
				}
				if(!System.IO.File.Exists( TargetPath + $@"\{title}.mp4" ))
				{
					foreach(var item in videos)//download video
					{
						if(item.Resolution == hightvideo)
						{
							//Console.WriteLine( "\ndownload video with Resolution {0} and size {1} MB" , item.Resolution , Math.Round( (double)item.ContentLength / (1024 * 1024.0) , 2 ) );
							if(downloadbest( item , TargetPath + "\\file123456798.mp4" ))
								Console.Write( "\rVideo downloaded                                                               " );
							else
								return;
							//Console.Write( "end\n" );
							break;
						}
					}
					//				Console.WriteLine( "wait for merge" );
					combine( title );
				}

				if(!System.IO.File.Exists( TargetPath + $@"\{title}.en.srt" ))
				{
					DownloadSubtitle( url , "en" , TargetPath + $@"\{title}" );
					DownloadSubtitle( url , "fr" , TargetPath + $@"\{title}" );
					//	File.Delete( TargetPath + "\\file123456798.mp3" );
					System.IO.File.Delete( TargetPath + "\\file123456798.mp4" );
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine( "\n\n\n\n" + ex );
				//Console.ReadKey();
			}
		}
		static void Main( string[] args )
		{
			TargetPath = new DirectoryInfo( "." ).CreateSubdirectory( "Media" ).FullName;

			string[] uris = new string[]
				{
					//"https://www.youtube.com/watch?v=wTP2RUD_cL0&list=RDwTP2RUD_cL0",
					//"https://www.youtube.com/watch?v=NuZklVrHspM&list=PLDA56F24B0A270792",
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
			foreach(string uri in uris)
				if(uri.Contains( "&list=" ))
				{
					string[] videolist = DownloadPlaylist( uri );
					//	foreach(string url in videolist)
					for(int i = 0 ; i < videolist.Length ; i++)
						Download( "https://www.youtube.com" + videolist[i] );
				}
				else
					Download( uri );

			Console.WriteLine( "press any key to continue..." );
			Console.ReadKey();
			Process.Start( TargetPath );
		}
		static bool DownloadSubtitle( string uri , string lang , string fname )
		{
			bool ans = false;

			using(YouTubeTranscriptApi uttla = new YouTubeTranscriptApi())
			{
				FileStream fs = null;

				try
				{
					TranscriptList tl = uttla.ListTranscripts( uri.Substring( uri.IndexOf( '=' ) + 1 ) );
					foreach(Transcript t in tl)
					{
						Console.Write($"\rVideo_id = {t.VideoId}, Language {t.Language} ({t.LanguageCode}");
						if(t.LanguageCode == lang)
						{
							fs = new FileStream( $"{fname}.{t.LanguageCode}.srt" , FileMode.Create );

							int i = 1;
							foreach(TranscriptItem ti in t.Fetch())
							{
								DateTime from = new DateTime( ti.Start * 10 * 1000 );
								//	Console.WriteLine( $"{i++}\n{from:HH:mm:ss,fff} --> {from.AddMilliseconds( ti.Duration ):HH:mm:ss,fff}\n{ti.Text}\n" );
								byte[] txt = Encoding.UTF8.GetBytes( string.Format( $"{(i > 1? "\n" : "")}{i++}\n{from:HH:mm:ss,fff} --> {from.AddMilliseconds( ti.Duration ):HH:mm:ss,fff}\n{ti.Text}\n" ) );
								fs.Write( txt , 0 , txt.Length );
							}
							ans = true;
							Console.WriteLine( " -> " + new FileInfo( $"{fname}.{t.LanguageCode}.srt" ).Name );
							break;
						}
					}
				}
				catch(Exception)
				{
				}
				finally
				{
					if(fs != null)
						fs.Close();
				}
			}
			return ans;
		}
		//	https://ffmpeg.org/download.html#build-windows
		//	https://github.com/BtbN/FFmpeg-Builds/releases
		static void combine( string title )
		{
			Process p = new Process();

			p.StartInfo.FileName = new FileInfo( @".\ffmpeg.exe" ).FullName;
			p.StartInfo.Arguments = "-i \"" + TargetPath + "\\file123456798.mp4\" -i \"" + TargetPath + $"\\{title}.mp3\" -preset veryfast  \"" + TargetPath + $"\\{title}.mp4\"";
			
			p.Start();
			p.WaitForExit();
		}
		static bool downloadbest( YouTubeVideo y , string patch )
		{
			bool ans = false;
			string action = patch.Contains( ".mp3" ) ? "audio" : "video";
			int attempt = 0;

			for(int timeout = 1000 ; timeout < 60000 ; timeout *= 2)
			{
				attempt += 1;
				int total = 0;
				FileStream fs = null;
				Stream streamweb = null;
				WebResponse w_response = null;
				try
				{
					WebRequest w_request = WebRequest.Create( y.Uri );
					w_request.Timeout = timeout;
					if(w_request != null)
					{
						w_response = w_request.GetResponse();
						if(w_response != null)
						{
							fs = new FileStream( patch , FileMode.Create );
							byte[] buffer = new byte[1024 * 1024];
							int bytesRead = 0;
							streamweb = w_response.GetResponseStream();
							//Console.WriteLine( "Download Started" );
							do
							{
								bytesRead = streamweb.Read( buffer , 0 , buffer.Length );
								fs.Write( buffer , 0 , bytesRead );
								total += bytesRead;
								Console.Write( $"\r{attempt}) Downloading {action} ({Math.Round( ((double)total / (int)y.ContentLength) * 100 , 2 ).ToString( "00.00" )}%) {total / 1024:#,###,###,###}/{y.ContentLength / 1024:#,###,###,###)} Kb.     " );
							} while(bytesRead > 0);
							//Console.WriteLine( "\nDownload Complete" );
						}
					}
					ans = true;
					break;
				}
				catch(WebException ex)
				{
					if(ex.Status == WebExceptionStatus.ProtocolError)
					{
						Console.Write( $" --> wait a {timeout / 1000} secs                                                                                                        " );
						System.Threading.Thread.Sleep( timeout );
					}
					if(ex.Status == WebExceptionStatus.Timeout || ex.Status == WebExceptionStatus.Pending || ex.Status == WebExceptionStatus.ProtocolError)
					{
						Console.Write( $" --> wait a {timeout / 1000} secs                                                                                                        " );
						System.Threading.Thread.Sleep( timeout );
					}
					else
						Console.WriteLine( "\n\n" + ex );
				}
				catch(IOException ex)
				{
					if(ex.HResult == -2146232800)  //	Received an unexpected EOF or 0 bytes from the transport stream.
					{
						Console.Write( $" --> wait a {timeout/1000} secs                                                                                                        " );
						System.Threading.Thread.Sleep( timeout );
					}
					else
						Console.WriteLine( "\n\n" + ex );
				}
				catch(Exception ex)
				{
					Console.WriteLine( "\n\n" + ex );
					break;
				}
				finally
				{
					if(w_response != null)
						w_response.Close();
					if(fs != null)
						fs.Close();
					if(streamweb != null)
						streamweb.Close();
				}
			}
			return ans;
		}
		static string[] DownloadPlaylist( string uri )
		{
			StringBuilder sb = new StringBuilder();

			for(int timeout = 1000 ; timeout < 60000 ; timeout *= 2)
			{
				sb.Clear();

				Stream streamweb = null;
				WebResponse w_response = null;
				try
				{
					WebRequest w_request = WebRequest.Create( uri );
					w_request.Timeout = timeout;
					if(w_request != null)
					{
						w_response = w_request.GetResponse();
						if(w_response != null)
						{
							byte[] buffer = new byte[1024 * 1024];
							int bytesRead = 0;
							streamweb = w_response.GetResponseStream();
							do
							{
								bytesRead = streamweb.Read( buffer , 0 , buffer.Length );
								if(bytesRead > 0)
									sb.Append( Encoding.Default.GetString( buffer , 0 , bytesRead ) );
							} while(bytesRead > 0);
						}
					}
					break;
				}
				catch(WebException ex)
				{
					if(ex.Status == WebExceptionStatus.ProtocolError)
					{
						Console.WriteLine( "skip task and restart after several hours" );
						break;
					}
					if(ex.Status == WebExceptionStatus.Timeout || ex.Status == WebExceptionStatus.Pending || ex.Status == WebExceptionStatus.ProtocolError)
					{
						Console.WriteLine( "wait a few secs" );
						System.Threading.Thread.Sleep( 1000 );
					}
					else
						Console.WriteLine( "\n\n" + ex );
				}
				catch(IOException ex)
				{
					if(ex.HResult == -2146232800)  //	Received an unexpected EOF or 0 bytes from the transport stream.
					{
						Console.WriteLine( "wait a few secs" );
						System.Threading.Thread.Sleep( 1000 );
					}
					else
						Console.WriteLine( "\n\n" + ex );
				}
				catch(Exception ex)
				{
					Console.WriteLine( "\n\n" + ex );
				}
				finally
				{
					if(w_response != null)
						w_response.Close();
					if(streamweb != null)
						streamweb.Close();
				}
			}

			List<string> lst = new List<string>();

			if(sb.Length > 0)
			{
				string http = HttpUtility.HtmlDecode( sb.ToString() );
				for(int i = 1 ; i < 200 ; i++)
				{
					string tag = $"index={i}";
					int at = http.IndexOf( tag );
					if(at > 0)
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

