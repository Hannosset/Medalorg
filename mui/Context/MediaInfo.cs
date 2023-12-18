using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

using xnext.Diagnostics;

namespace mui.Context
{
	public enum MediaType { Unkown , Audio, Video };

	public class MediaInfo
	{
		#region TYPES
		public enum AudioModel { Any, aac, vorbis, opus };
		public enum VideoFormat { mp4, webm };

		public class MediaData
		{
			[XmlAttribute]
			public MediaType Type { get; set; }

			[XmlAttribute]
			public bool Downloaded { get; set; } = false;

			[XmlIgnore]
			public bool Selected { get; set; } = false;

			[XmlText]
			public string Uri { get; set; }
		}
		public class AudioData : MediaData
		{
			[XmlAttribute]
			public AudioModel Model { get; set; } = AudioModel.Any;

			[XmlAttribute]
			public int BitRate { get; set; } = -1;

			public override string ToString() => $"{Model} @ {BitRate}";
		}
		public class VideoData : AudioData
		{
			[XmlAttribute]
			public VideoFormat Format { get; set; } = VideoFormat.mp4;

			[XmlAttribute]
			public int Resolution { get; set; }

			public override string ToString()
				=> Model == AudioModel.Any
				? $"{Format} - {Resolution}"
				: $"{Format} - {Resolution} with {Model} @ {BitRate}";
		}
		#endregion

		#region LOCAL VARIABLE
		private List<MediaData> _Items { get; set; } = new List<MediaData>();
		#endregion

		#region PUBLIC PROPERTIES
		[XmlAttribute]
		public string VideoId { get; set; }

		[XmlAttribute]
		public string Caption { get; set; }

		[XmlAttribute]
		public string Publisher { get; set; }

		[XmlAttribute]
		public DateTime LastDownload { get; set; } = DateTime.MinValue;

		[
			XmlElement( Type = typeof( MediaData ) , IsNullable = true ),
			XmlElement( Type = typeof( AudioData ) , IsNullable = true ),
			XmlElement( Type = typeof( VideoData ) , IsNullable = true )
		]
		public MediaData[] Details
		{
			get => _Items.ToArray();
			set => _Items.AddRange( value );
		}
		#endregion

		#region PREDICATE
		public string Author => SplitAuthorFromTitle( Caption )[0];
		public string Title => HumanString(SplitAuthorFromTitle( Caption )[1] );
		#endregion

		#region PREDICATE
		public bool Downloaded => Details.Where( x => x.Downloaded ).Any();
		public int AudioCount => Details.Where( x => x.Type == MediaType.Audio ).Count();
		public int VideoCount => Details.Where( x => x.Type == MediaType.Video ).Count();
		#endregion

		#region CONSTRUCTOR
		public MediaInfo() { }
		public MediaInfo( string videoId , string title , string publisher )
		{
			VideoId = videoId;
			Caption = title.Trim( new char[] { '\"' , ' ' , '\'' , '?' , '.' } );
			Publisher = publisher;
		}
		#endregion

		#region PUBLIC METHODS
		/// <summary>
		/// What: Adds a video or audio stream of the media to download
		/// Why: need to identify the stream to allow the end-user to chose what to doanload
		/// Note: fields
		/// 0 -> video Id
		/// 1 -> MediaType
		/// 2 -> AudioModel
		/// 3 -> bitrate
		/// 4 -> VideoFormat
		/// 5 -> resolution
		/// 6 -> uri
		/// </summary>
		/// <param name="fields"></param>
		public void Add( string[] fields )
		{
			LogTrace.Label();
			MediaType type;
			if( Enum.TryParse( fields[1] , true , out type ) )
			{
				AudioModel model = AudioModel.Any;
				int bitrate = -1;

				int.TryParse( fields[3] , out bitrate );

				if( Enum.TryParse( fields[2] , true , out model ) && type == MediaType.Audio )
					_Items.Add( new AudioData
					{
						Type = MediaType.Audio ,
						Model = model ,
						BitRate = bitrate ,
						Uri = fields[6]
					} );
				else if( type == MediaType.Video )
				{
					VideoFormat format;
					int resolution = 0;
					int.TryParse( fields[5] , out resolution );
					if( Enum.TryParse( fields[4] , true , out format ) && resolution > 0 )
						_Items.Add( new VideoData
						{
							Type = MediaType.Video ,
							Model = model ,
							BitRate = bitrate ,
							Format = format ,
							Resolution = resolution ,
							Uri = fields[6]
						} );
				}
			}
		}
		#endregion

		private string HumanString( string str )
		{
			string fname = str.Replace( "\t" , " " ).Replace( "\f" , "" ).Replace( "\n" , "" ).Replace( "\r" , "" )
							.Replace( "\"" , "'" ).Replace( "|" , "-" ).Replace( "\\" , "-" ).Replace( "/" , "-" ).Replace( ":" , "-" ).Replace( "*" , "" ).Replace( "?" , "" )/*.Replace( "!" , "" )*/.Replace( "<" , "" ).Replace( ">" , "" )
							.Replace( "HQ Audio" , "" ).Replace( "_" , " " );

			int at = fname.IndexOf( '(' );
			if( at != -1 )
				fname = fname.Replace( fname.Substring( at - 1 , fname.IndexOf( ')' ) - at + 2 ) , "" );
			
			foreach( string s in Handle2Skip.Info.Details )
				if( fname.Contains( s ) )
					fname = fname.Replace( s , "" ).Trim();
			
			return fname.Trim();
		}
		private string[] SplitAuthorFromTitle( string astr )
		{
			string str = HumanString( astr );

			foreach( AuthorInfo auth in HandleAuthors.Info.Details )
				if( (auth.Name.Length == str.Length && auth.Name.CompareTo( str ) == 0) || (str.Length > auth.Name.Length && str.Substring( 0 , auth.Name.Length ) == auth.Name) )
					return new string[] { auth.Name , HumanString( str.Replace( auth.Name , "" ) ).Replace( "-" , "" ).Trim() };

			foreach( string cond in new string[] { " - " , " : " , ":" } )
			{
				int at = str.IndexOf( cond );
				if( at != -1 )
					return new string[] { str.Substring( 0 , at ).Trim() , str.Substring( at + cond.Length ).Trim() };
			}
			return new string[] { HumanString( Publisher ).Replace( "'" , "" ) , astr };
		}
	}
}
