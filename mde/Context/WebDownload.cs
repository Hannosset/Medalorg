using mde.Subtitle;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

using VideoLibrary;

namespace mde.Context
{
	[
	  XmlInclude( typeof( WebDownload ) )
	, XmlInclude( typeof( DownloadSubtitle ) )
	, XmlInclude( typeof( DownloadLyrics ) )
	, XmlInclude( typeof( DownloadBinary ) )
	, XmlInclude( typeof( DownloadAudio ) )
	, XmlInclude( typeof( DownloadVideo ) )
	]
	public class WebDownload
	{
		#region LOCAL VARIABLE
		protected byte[] _buffer;
		string _VideoId;
		protected IEnumerable<YouTubeVideo> _videos;
		#endregion LOCAL VARIABLE

		#region PUBLIC PROPERTIES
		[XmlAttribute]
		public string VideoId
		{
			get => _VideoId;
			set
			{
				_VideoId = value;
				_videos = YouTube.Default.GetAllVideos( $"https://www.youtube.com/watch?v={value}" );
			}
		}
		[XmlAttribute] public int Id { get; set; }
		[XmlText] public string Filename { get; set; }
		#endregion PUBLIC PROPERTIES

		#region CONSTRUCTOR
		public WebDownload()
		{
			_buffer = new byte[64 * 1024];
		}
		#endregion CONSTRUCTOR

		#region PUBLIC METHODS
		public virtual bool Initialize() { return true; }
		public virtual long Download() { return 0; }
		public virtual void Close() { }
		#endregion PUBLIC METHODS
	}
	public class DownloadLyrics : WebDownload
	{
		[XmlAttribute] public string Lang { get; set; }

		string LangList = string.Empty;
		protected int line = 0;

		public override bool Initialize()
		{
			LangList = Lang;
			foreach( string LanguageCode in Lang.Split( ',' ) )
				if( File.Exists( Filename.Replace( "{lang}" , LanguageCode ) ) )
					LangList = LangList.Replace( LanguageCode , "" );
			LangList = LangList.Replace( ",," , "," ).Trim();

			return base.Initialize();
		}
		public override long Download()
		{
			if( string.IsNullOrEmpty( LangList ) )
				return 0;


			using( YouTubeTranscriptApi uttla = new YouTubeTranscriptApi() )
			{
				try
				{
					TranscriptList tl = uttla.ListTranscripts( VideoId );
					foreach( Transcript t in tl )
					{
						if( LangList.Contains( t.LanguageCode ) )
						{
							Console.WriteLine( $"{VideoId}\t \t \tDownload transcript for " + t.Language );

							using( FileStream fs = new FileStream( Filename.Replace( "{lang}" , t.LanguageCode ) , FileMode.Create ) )
							{
								line = 0;
								foreach( TranscriptItem ti in t.Fetch() )
								{
									byte[] txt = Encoding.UTF8.GetBytes( FormatLine( ti ) );
									fs.Write( txt , 0 , txt.Length );
								}
							}
						}
					}
				}
				catch( Exception )
				{
				}
			}

			return 0;
		}
		protected virtual string FormatLine( TranscriptItem ti ) => $"{ti.Text}\n";
	}
	public class DownloadSubtitle : DownloadLyrics
	{
		protected override string FormatLine( TranscriptItem ti )
		{
			DateTime from = new DateTime( ti.Start * 10 * 1000 );
			return string.Format( $"{(line > 1 ? "\n" : "")}{line++}\n{from:HH:mm:ss,fff} --> {from.AddMilliseconds( ti.Duration ):HH:mm:ss,fff}\n{ti.Text}\n" );
		}
	}
	public class DownloadBinary : WebDownload
	{
		#region LOCAL VARIABLE
		HttpWebRequest _request;
		HttpWebResponse _response;
		int timeout;
		protected string _uri = string.Empty;
		#endregion LOCAL VARIABLE

		protected string label = string.Empty;

		#region PUBLIC PROPERTIES
		[XmlAttribute] public long DataLength { get; set; }
		#endregion PUBLIC PROPERTIES

		#region CONSTRUCTOR
		public DownloadBinary()
		{
			timeout = 1000;
		}
		public override bool Initialize()
		{
			Close();

			FileInfo fi = new FileInfo( Filename );
			if( !fi.Directory.Exists )
				fi.Directory.Create();
			else if( fi.Exists )
			{
				Console.WriteLine( $"{VideoId}\t \t \t{label} Media downloaded" );
				return true;
			}

			try
			{
				_request = (HttpWebRequest)WebRequest.Create( _uri );
				if( _request != null )
				{
					//	https://stackoverflow.com/questions/716436/is-there-a-correct-way-to-dispose-of-a-httpwebrequest
					_request.ServicePoint.MaxIdleTime = 1000;
					_request.KeepAlive = false;
					_request.Timeout = 300000;
					return true;
				}
			}
			catch( Exception )
			{
			}
			return false;
		}
		#endregion CONSTRUCTOR

		#region PUBLIC METHODS
		public override long Download()
		{
			if( _request == null )
				return DataLength;

			try
			{
				for( int attempt = 0 ; attempt < 3 ; attempt++ )
				{
					try
					{
						Console.WriteLine( $"{VideoId}\t{Id}\t0\tRequest {label} data stream" );
						_response = (HttpWebResponse)_request.GetResponse();
						if( _response != null )
						{
							using( Stream streamweb = _response.GetResponseStream() )
							{
								DownloadFromStream( streamweb );
								streamweb.Close();
							}
							_response.Close();
							return DataLength;
						}
					}
					catch( WebException ex )
					{
						if( ex.Status == WebExceptionStatus.ProtocolError )
						{
							Console.WriteLine( $"{VideoId}\t{Id}\t0\tProtocol error: {ex.Message}" );
							break;
						}
						else
						{
							Console.WriteLine( $"{VideoId}\t{Id}\t0\tError: {ex.Status}: {ex.Message}" );
							if( ex.Status == WebExceptionStatus.Timeout )
								_request.Timeout += 100000;
							else if( ex.Status == WebExceptionStatus.Pending )
							{
								timeout *= 2;
								Thread.Sleep( timeout );
							}
						}
					}
					catch( IOException ex )
					{
						if( ex.HResult == -2146232800 )  //	Received an unexpected EOF or 0 bytes from the transport stream.
						{
							Console.WriteLine( $"{VideoId}\t{Id}\t0\tError: EOF - {ex.Message}" );
							Thread.Sleep( timeout );
							Initialize();
						}
						else
						{
							Console.WriteLine( $"{VideoId}\t{Id}\t0\tError: {ex.Message}" );
							break;
						}
					}
					catch( Exception ex )
					{
						//	Log the error for investigation
						Console.WriteLine( $"{VideoId}\t{Id}\t0\tError: {ex.Message}" );
						break;
					}
					finally
					{
					}
				}
			}
			finally
			{
				_response?.Close();
				_response?.Dispose();
				_response = null;
			}
			return -1;
		}
		public override void Close()
		{
			_response?.Close();
			_response?.Dispose();
			_response = null;
			_request?.Abort();
			_request = null;
			base.Close();
		}
		#endregion PUBLIC METHODS

		#region LOCAL METHODS
		bool DownloadFromStream( Stream streamweb )
		{
			FileInfo fifilename = new FileInfo( Filename );

			Console.WriteLine( $"{VideoId}\t{Id}\t0\tDownloading {label}" );
			if( fifilename.Exists )
			{
				Console.WriteLine( $"\r{VideoId}\t{Id}\t{fifilename.Length}\tDownloading {label}" );
				return true;
			}

			decimal total = 0;
			int bytesRead = 0;
			using( var tempFile = new TemporaryFile() )
			{
				using( FileStream fileStream = File.Create( tempFile.FilePath ) )
				{
					do
					{
						try
						{
							bytesRead = streamweb.Read( _buffer , 0 , _buffer.Length );
							if( bytesRead > 0 )
								fileStream.Write( _buffer , 0 , bytesRead );

							total += bytesRead;
							if( bytesRead > 0 )
								Console.Write( $"\r{VideoId}\t{Id}\t{bytesRead}\tDownloading {label} [{total / 1024:#,###,##0} Kb]" );
						}
						catch( Exception ex )
						{
							if( total != DataLength )
								throw ex;
							else
								bytesRead = 0;
						}
					} while( bytesRead > 0 );
				}

				File.Move( tempFile.FilePath , Filename );

				return true;
			}
		}
		#endregion LOCAL METHODS
	}
	public class DownloadAudio : DownloadBinary
	{
		[XmlAttribute] public int BitRate { get; set; }
		[XmlAttribute] public AudioFormat Format { get; set; }

		public override bool Initialize()
		{
			label = "audio";
			try
			{
				foreach( YouTubeVideo item in _videos )
					if( item.AdaptiveKind == AdaptiveKind.Audio && item.AudioBitrate == BitRate && item.AudioFormat == Format )
					{
						Console.WriteLine( $"{VideoId}\t \t \tGet the {label} media uri" );
						_uri = item.Uri;
						return base.Initialize();
					}
			}
			catch( Exception ex )
			{
			}
			return false;
		}
	}
	public class DownloadVideo : DownloadBinary
	{
		[XmlAttribute] public int Resolution { get; set; }
		[XmlAttribute] public VideoFormat Format { get; set; }

		public override bool Initialize()
		{
			label = "video";
			try
			{
				foreach( YouTubeVideo item in _videos )
					if( item.AdaptiveKind == AdaptiveKind.Video && item.Resolution == Resolution && item.Format == Format )
					{
						Console.WriteLine( $"{VideoId}\t \t \tGet the {label} media uri" );
						_uri = item.Uri;
						return base.Initialize();
					}
			}
			catch( Exception ex )
			{
			}
			return false;
		}
	}
}