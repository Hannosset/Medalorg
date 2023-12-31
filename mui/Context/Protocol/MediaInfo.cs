using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

using xnext.Diagnostics;

namespace mui.Context.Protocol
{
	public enum AdaptiveKind { Unkown, Audio, Video };
	public enum AudioFormat { Unknown, Aac, Vorbis, Opus };
	public enum VideoFormat { Mp4, WebM };

	public class MediaInfo
	{
		#region TYPES
		public class MediaData
		{
			[XmlAttribute] public AdaptiveKind Type { get; set; }
			[XmlAttribute] public long DataLength { get; set; }
			[XmlText] public string Filename { get; set; } = "";
			[XmlIgnore] public MediaInfo Parent { get; set; }

			/// <summary>
			/// What: extension of the media file
			///  Why: the extension must be regulated and formatted as follows . [media parameter] . [media file type] as this format will be used to insert automatically lyrics and subtitles files
			/// </summary>
			internal virtual string Extension { get; } = string.Empty;

			internal bool Downloaded => string.IsNullOrEmpty( Filename ) ? false : File.Exists( Filename );
		}
		public class AudioData : MediaData
		{
			[XmlAttribute] public AudioFormat Model { get; set; } = AudioFormat.Unknown;
			[XmlAttribute] public int BitRate { get; set; } = -1;

			internal override string Extension => $".{BitRate}.{Model.ToString().ToLower()}";
			public override string ToString() => $"{Model} @ {BitRate} [{DataLength / 1024:#,###,###} Kb.]";
		}
		public class VideoData : AudioData
		{
			[XmlAttribute] public VideoFormat Format { get; set; } = VideoFormat.Mp4;
			[XmlAttribute] public int Resolution { get; set; }

			internal override string Extension => $".{Resolution}@{BitRate}.{Format.ToString().ToLower()}";

			public override string ToString()
				=> Model == AudioFormat.Unknown
				? $"{Format} - {Resolution} [{DataLength / 1024:#,###,###} Kb.]"
				: $"{Format} - {Resolution} with {Model} @{BitRate} [{DataLength / 1024:#,###,###} Kb.]";
		}
		#endregion

		#region LOCAL VARIABLE
		private List<MediaData> _Items = new List<MediaData>();
		private List<string> _MovieFilenames = new List<string>();
		#endregion

		#region PUBLIC PROPERTIES
		[XmlAttribute] public string VideoId { get; set; }
		[XmlAttribute] public string Caption { get; set; }
		[XmlAttribute] public string Publisher { get; set; }
		public string[] MovieFilenames
		{
			get => _MovieFilenames.ToArray();
			set => _MovieFilenames.AddRange( value );
		}
		[
			XmlElement( Type = typeof( MediaData ) , IsNullable = true ),
			XmlElement( Type = typeof( AudioData ) , IsNullable = true ),
			XmlElement( Type = typeof( VideoData ) , IsNullable = true )
		]
		public MediaData[] Details
		{
			get => _Items.ToArray();
			set
			{
				_Items.AddRange( value );
				foreach( MediaData md in _Items )
					md.Parent = this;
			}
		}
		#endregion

		#region PREDICATE
		/// <summary>
		/// What: Access the author of the media
		///  Why: Extract from the caption the author of the media who is not necessarily the publisher of the media
		/// </summary>
		public string Author => SplitAuthorFromTitle( Caption )[0];
		/// <summary>
		/// What: Access the title of the media
		///  Why: Extract from the internet caption the title and convert it into a valid filename
		/// </summary>
		public string Title => HumanString( SplitAuthorFromTitle( Caption )[1] );
		#endregion

		#region ACESSORS
		public MediaData this[long l ]
		{
			get
			{
				foreach( MediaData md in Details)
					if( md.DataLength== l )
						return md;
				return null;
			}
		}
		public int DownloadedVideo => Details.Where( x => x.Downloaded && (x.Type == AdaptiveKind.Audio || (x.Type == AdaptiveKind.Video && !x.Extension.Contains( "@-1" ))) ).Count() + MovieFilenames.Length;
		public bool Downloaded => MovieFilenames.Length > 0 || Details.Where( x => x.Downloaded && ( x.Type == AdaptiveKind.Audio || (x.Type == AdaptiveKind.Video && !x.Extension.Contains( "@-1" )))).Any();
		/// <summary>
		/// What: Count the number of audio files
		///  Why: Display that information in the main list view - purely informative
		/// </summary>
		public int AudioCount => Details.Where( x => x.Type == AdaptiveKind.Audio ).Count();
		/// <summary>
		/// What: Count the number of video files
		///  Why: Display that information in the main list view - purely informative
		/// </summary>
		public int VideoCount => Details.Where( x => x.Type == AdaptiveKind.Video ).Count();
		#endregion

		#region CONSTRUCTOR
		public MediaInfo() { }
		public MediaInfo( string videoId , string title , string publisher )
		{
			VideoId = videoId;
			Caption = title.Trim( new char[] { '\"' , ' ' , '\'' , '?' , '.' , '-' } );
			Publisher = publisher;
		}
		#endregion

		#region PUBLIC METHODS
		internal void Add( string v ) => _MovieFilenames.Add( v );
		/// <summary>
		/// What: Adds a video or audio stream of the media to download
		/// Why: need to identify the stream to allow the end-user to chose what to download
		/// Note: fields
		/// 0 -> video Id
		/// 1 -> MediaType
		/// 2 -> AudioModel
		/// 3 -> bit rate
		/// 4 -> VideoFormat
		/// 5 -> resolution
		/// 6 -> Data length
		/// </summary>
		/// <param name="fields"></param>
		public MediaData Add( string[] fields )
		{
			LogTrace.Label();

			long datalength = 0;
			if( fields.Length < 6 || long.TryParse( fields[6] , out datalength ) == false || this[datalength] != null )
				return this[datalength];

			AdaptiveKind type;
			if( Enum.TryParse( fields[1] , true , out type ) )
			{
				AudioFormat model;

				int bitrate;
				int.TryParse( fields[3] , out bitrate );

				if( datalength > 0 )
				{
					MediaData md = null;
					if( Enum.TryParse( fields[2] , true , out model ) && type == AdaptiveKind.Audio )
					{
						md = new AudioData
						{
							Parent = this,
							Type = AdaptiveKind.Audio ,
							Model = model ,
							BitRate = bitrate ,
							DataLength = datalength
						};
					}
					else if( type == AdaptiveKind.Video )
					{
						VideoFormat format;
						int resolution;
						int.TryParse( fields[5] , out resolution );
						if( Enum.TryParse( fields[4] , true , out format ) && resolution > 0 )
						{
							md = new VideoData
							{
								Parent = this,
								Type = AdaptiveKind.Video ,
								Model = model ,
								BitRate = bitrate ,
								Format = format ,
								Resolution = resolution ,
								DataLength = datalength
							};
						}
					}
					if( md != null )
					{
						//	Do not add if already recorded.
						foreach( MediaData item in Details )
							if( item.ToString().CompareTo( md.ToString() ) == 0 )
								return null;
						_Items.Add( md );
						return md;
					}
				}
			}
			return null;
		}
		#endregion

		#region LOCAL METHODS
		/// <summary>
		/// What: Convert a string into a human filename
		///  Why: strip the string from all unwanted characters to allow a decent and easy to read filename
		/// </summary>
		private string HumanString( string str )
		{
			string fname = str.Replace( "\t" , " " ).Replace( "\f" , "" ).Replace( "\n" , "" ).Replace( "\r" , "" )
							.Replace( "\"" , "'" )
							.Replace( "|" , "-" ).Replace( "\\" , "-" ).Replace( "/" , "-" ).Replace( "~" , "-" ).Replace( ":" , "-" )
							.Replace( "*" , "" ).Replace( "?" , "" ).Replace( "!" , "" ).Replace( "<" , "" ).Replace( ">" , "" )
							.Replace( "HQ Audio" , "" ).Replace( "_" , " " );

			int at = fname.IndexOf( '(' );
			if( at != -1 )
			{
				int to = fname.IndexOf( ')' );
				if( to != -1 && to > at + 2 )
					fname = fname.Replace( fname.Substring( at - 1 , to - at + 2 ) , "" );
				else
					fname = fname.Substring( at - 1 );
			}

			foreach( string s in Handle2Skip.Info.Details )
				if( fname.Contains( s ) )
					fname = fname.Replace( s , "" ).Trim();

			return fname.Trim();
		}
		/// <summary>
		/// What: Extract the author and the title of the media from a media caption
		///  Why: The author's name may be used in the pathname and the title is used to generate the filename media
		/// </summary>
		private string[] SplitAuthorFromTitle( string astr )
		{
			string str = HumanString( astr );

			foreach( AuthorInfo auth in HandleAuthors.Info.Details )
				if( (auth.Name.Length == str.Length && auth.Name.CompareTo( str ) == 0) || (str.Length > auth.Name.Length && str.Substring( 0 , auth.Name.Length ) == auth.Name) )
					return new string[] { auth.Name , HumanString( str.Replace( auth.Name , "" ) ).Replace( "-" , "" ).Trim() };

			foreach( string cond in new string[] { ";" } )
			{
				int at = str.IndexOf( cond );
				if( at != -1 )
					str = str.Substring( 0 , at ).Trim();
			}
			foreach( string cond in new string[] { " - " , " : " , ":" } )
			{
				int at = str.IndexOf( cond );
				if( at != -1 )
					return new string[] { str.Substring( 0 , at ).Trim() , str.Substring( at + cond.Length ).Trim() };
			}
			string author = HumanString( Publisher ).Replace( "'" , "" );
			foreach( string cond in new string[] { "-" , ":" , ":" , "-" } )
			{
				int at = author.IndexOf( cond );
				if( at != -1 )
					author = author.Substring( 0 , at ).Trim();
			}
			return new string[] { author , str };
		}
		#endregion
	}
}
