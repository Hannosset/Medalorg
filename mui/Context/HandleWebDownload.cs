using mui.Context.Protocol;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Xml;
using System.Xml.Serialization;

using xnext.Diagnostics;

namespace mui.Context
{
	internal class HandleWebDownload
	{
		#region LOCAL VARIABLE
		/// <summary>
		/// What: List of the available media of the singleton
		///  Why: easily adds a new download request in the list
		/// </summary>
		private List<WebDownload> _Details = new List<WebDownload>();
		/// <summary>
		/// What: Total size downloaded so far
		///  Why: being able to create percentage progress and in case of one media restart allow to maintain a correct downloaded percentage
		/// </summary>
		private List<decimal> _Downloads = new List<decimal>();
		/// <summary>
		/// What: number of attempt to download the media
		///  Why: provide end-user with that information.
		/// </summary>
		private List<int> _Attempts = new List<int>();
		bool HasVideo = false;
		/// <summary>
		/// What: Total size to download
		///  Why: being able to create percentage progress
		/// </summary>
		decimal _Totalsize = 0;
		decimal _Downloaded = 0;
		#endregion LOCAL VARIABLE

		#region PROPERTIES
		internal bool HasAudio { get; private set; } = false;
		internal bool VideoNeedsAudio { get; private set; } = false;
		internal DownloadAudio BestAudio { get; private set; } = null;
		#endregion PROPERTIES

		#region ACCESSORS
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		public WebDownload this[long l]
		{
			get
			{
				foreach( WebDownload item in _Details )
					if( item is DownloadBinary db && db.DataLength == l)
						return item;
				return null;
			}
		}
		public int Attempt( int id  ) => id < _Attempts.Count ? _Attempts[id] : 0;
		public decimal Progress => Math.Round( (_Downloaded / _Totalsize) * 100 , 2 , MidpointRounding.AwayFromZero );
		internal string VideoId => _Details.Count > 0 ? _Details[0].VideoId : string.Empty;
		/// <summary>
		/// What: array of the download request
		///  Why: easily access a read-only array
		/// </summary>
		internal WebDownload[] Details => _Details.ToArray();
		#endregion ACCESSORS

		public HandleWebDownload() { }
		public HandleWebDownload( WebDownload[] webDownloads )
		{
			_Details.AddRange( webDownloads );
			foreach( WebDownload wd in webDownloads )
			{
				if( wd is DownloadBinary db )
					_Totalsize = _Totalsize + db.DataLength;

				_Downloads.Add( 0 );
				_Attempts.Add( 0 );
				if( wd is DownloadVideo video )
				{
					HasVideo = true;
					if( wd.Filename.IndexOf( "@-1" ) > 0 )
					{
						VideoNeedsAudio = true;
						video.TargetFilename = wd.Filename;
					}
					else
						video.TargetFilename = wd.Filename;
				}
				else if( wd is DownloadAudio audio )
				{
					HasAudio = true;
					if( BestAudio == null || audio.BitRate > BestAudio.BitRate )
						BestAudio = audio;
				}
			}
		}

		#region PUBLIC METHODS
		/// <summary>
		/// What: 
		///  Why: 
		/// </summary>
		public void Update( int id , decimal downloaded )
		{
			if( downloaded > 0 )
			{
				if( _Downloads[id] == 0 )
					_Attempts[id] += 1;

				_Downloads[id] = Math.Min( _Totalsize , downloaded + _Downloads[id] );
				_Downloaded = Math.Min( _Totalsize , downloaded + _Downloaded );
			}
			else
			{
				_Downloads[id] = 0;
				_Downloaded = _Downloads.Sum();
			}
		}
		/// <summary>
		/// What: Adds a new media in the request list 
		///  Why: Enrich the singleton container with a new information if the media is not already registered
		/// </summary>
		public WebDownload UpdateWith( WebDownload request )
		{
			request.Id = _Details.Count;
			_Downloads.Add( 0 );
			_Attempts.Add( 0 );

			LogTrace.Label();
			_Details.Add( request );

			_Totalsize += (request as DownloadBinary).DataLength;

			if( request is DownloadAudio audio )
			{
				if( BestAudio == null || (request as DownloadAudio).BitRate > BestAudio.BitRate )
					BestAudio = request as DownloadAudio;
				if( !HasAudio )
				{
					string ext = request.Filename.Substring( request.Filename.LastIndexOf( '\\' ) + 1 );
					ext = ext.Substring( ext.IndexOf( $".{audio.BitRate}" ) );

					_Details.Add( new DownloadLyrics
					{
						Lang = xnext.Context.CltWinEnv.AppReadSetting.GetData( "Configuration" , "Subtitles" , "eng" ) ,
						Filename = request.Filename.Replace( ext , ".{lang}.lyric" ) ,
						Id = _Details.Count ,
						VideoId = request.VideoId ,
						MediaData = request.MediaData
					} );
					_Downloads.Add( 0 );
					_Attempts.Add( 0 );
					HasAudio = true;
				}
			}
			else if( request is DownloadVideo video)
			{
				(request as DownloadVideo).TargetFilename = request.Filename;

				//	Append an .mpeg extension only if the video has no sound
				if( request.Filename.IndexOf( "@-1" ) > 0  )
				{
					if( !VideoNeedsAudio )
						VideoNeedsAudio = true;
					request.Filename = request.Filename + ".mpeg";
				}

				if( !HasVideo )
				{
					//string ext = request.Filename.Substring( request.Filename.LastIndexOf( '\\' ) + 1 );
					//ext = ext.Substring( ext.IndexOf( $".{video.Resolution}@" ) );

					_Details.Add( new DownloadSubtitle
					{
						Lang = xnext.Context.CltWinEnv.AppReadSetting.GetData( "Configuration" , "Subtitles" , "eng" ) ,
						Filename = request.Filename.Replace( video.Format.ToString().ToLower() , "{lang}.srt" ) ,
						Id = _Details.Count ,
						VideoId = request.VideoId ,
						MediaData = request.MediaData
					} );
					_Downloads.Add( 0 );
					_Attempts.Add( 0 );
					HasVideo = true;
				}
			}
			return request;
		}
		/// <summary>
		/// What: 
		///  Why: 
		/// </summary>
		internal void Bind( MediaInfo mi )
		{
			MediaInfo.MediaData prev = null;
			foreach( WebDownload wd in Details )
			{
				if( wd is DownloadAudio audio )
				{
					foreach( MediaInfo.MediaData md in mi.Details )
						if( md.Type == AdaptiveKind.Audio && audio.DataLength == md.DataLength )
						{
							prev = wd.MediaData = md;
							break;
						}
				}

				else if( wd is DownloadVideo video )
				{
					foreach( MediaInfo.MediaData md in mi.Details )
						if( md.Type == AdaptiveKind.Video && video.DataLength == md.DataLength )
						{
							prev = wd.MediaData = md;
							break;
						}
				}
				else
					wd.MediaData = prev;
			}
		}
		#endregion PUBLIC METHODS

		#region SERIALIZATION
		/// <summary>
		/// What: Filename used to load/save the content of the request
		///  Why: one filename access allowing a centralization of the filename management
		/// </summary>
		internal string Filename
		{
			get
			{
				if( _Details.Count > 0 )
				{

					FileInfo fi = new FileInfo( Path.Combine( MemoryCache.Default["PublishPath"] as string , Details[0].VideoId + ".xml" ) );
					if( !fi.Directory.Exists )
						fi.Directory.Create();
					return fi.FullName;
				}
				return null;
			}
		}
		/// <summary>
		/// What: Serialization of the request
		///  Why: allow to save on the file system the content of the request - allowing manual alteration or simple recovery by deleting the file.
		/// </summary>
		internal void Serialize( string filename = null )
		{
			if( string.IsNullOrEmpty( filename ) )
				filename = Filename;
			try
			{
				LogTrace.Label( filename );
				using( FileStream fs = new FileStream( filename , FileMode.Create , FileAccess.Write , FileShare.None ) )
					new XmlSerializer( typeof( WebDownload[] ) ).Serialize( fs , Details );
			}
			catch( System.Exception ex )
			{
				Logger.TraceException( ex , "The new requests will not be saved" , $"Confirm the Data Path in the configuration file is correct and confirm read/write access to the path and the file ({filename} )" );
			}
		}
		internal static HandleWebDownload Deserialize( string filename )
		{
			if( !string.IsNullOrEmpty(filename ) )
			try
			{
				LogTrace.Label( filename );
					if( File.Exists( filename ) )
						using( FileStream fs = new FileStream( filename , FileMode.Open , FileAccess.Read , FileShare.Read ) )
						using( XmlReader reader = XmlReader.Create( fs , new XmlReaderSettings() { XmlResolver = null } ) )
						{
							HandleWebDownload webdownload = new HandleWebDownload( new XmlSerializer( typeof( WebDownload[] ) ).Deserialize( reader ) as WebDownload[] );
							return webdownload;
						}
			}
			catch( System.Exception ex )
			{
				Logger.TraceException( ex , "The new media will not be loaded" , $"Confirm the Data Path in the configuration file is correct and confirm read access to the path and the file ({filename} )" );
			}
			return new HandleWebDownload();
		}
		#endregion SERIALIZATION
	}
}
