using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using VideoLibrary;

namespace mid
{
	internal class MediaInfo
	{
		#region TYPES
		internal class InfoData
		{
			#region TYPES
			internal enum MediaType { Other, Audio, Video };
			internal enum MediaFormat { Other, acc, vorbis, opus, mp4, webm };
			internal enum AudioFormat { Any, acc, vorbis, opus };
			#endregion

			#region ACCESSORS
			internal MediaType Type { get; private set; } = MediaType.Other;
			internal MediaFormat Format { get; private set; } = MediaFormat.Other;
			internal decimal Resolution { get; private set; }
			internal AudioFormat VideoAudio { get; private set; } = AudioFormat.Any;
			internal long AudioBitrate { get; private set; } = -1;
			internal string Uri { get; private set; }
			#endregion

			#region CONSTRUCTOR
			internal InfoData( YouTubeVideo webvideo )
			{
				Uri = webvideo.Uri;
				if( webvideo.AdaptiveKind == AdaptiveKind.Audio )
				{
					Type = MediaType.Audio;
					switch( webvideo.AudioFormat )
					{
						case VideoLibrary.AudioFormat.Aac: VideoAudio = AudioFormat.acc; break;
						case VideoLibrary.AudioFormat.Vorbis: VideoAudio = AudioFormat.vorbis; break;
						case VideoLibrary.AudioFormat.Opus: VideoAudio = AudioFormat.opus; break;
					}
					AudioBitrate = webvideo.AudioBitrate;
				}
				else
				{
					Type = MediaType.Video;
					switch( webvideo.Format )
					{
						default:
						case VideoFormat.Mp4: Format = MediaFormat.mp4; break;
						case VideoFormat.WebM: Format = MediaFormat.webm; break;
					}
					Resolution = webvideo.Resolution;
					switch( webvideo.AudioFormat )
					{
						case VideoLibrary.AudioFormat.Aac: VideoAudio = AudioFormat.acc; break;
						case VideoLibrary.AudioFormat.Vorbis: VideoAudio = AudioFormat.vorbis; break;
						case VideoLibrary.AudioFormat.Opus: VideoAudio = AudioFormat.opus; break;
					}
					AudioBitrate = webvideo.AudioBitrate;
				}
			}
			#endregion

			#region PUBLIC METHODS
			public override string ToString()
			{
				if( Type == MediaType.Audio )
					return $"{Type}\t{VideoAudio}\t{AudioBitrate}\t{Uri}";
				else
					return $"{Type}\t{Format}\t{Resolution}\t{VideoAudio}\t{AudioBitrate}\t{Uri}";
			}
			#endregion
		}
		#endregion

		#region LOCAL VARIABLES
		List<InfoData> _infoDatas = new List<InfoData>();
		#endregion

		#region ACCESSORS
		internal string Title { get; private set; }
		internal string Author { get; private set; }
		internal string Url { get; private set; }
		#endregion

		#region CONSTRUCTOR
		public MediaInfo( string url )
		{
			Url = url;
			IEnumerable<YouTubeVideo> videos = YouTube.Default.GetAllVideos( url );

			if( videos.Count() > 0 )
			{
				Title = videos.First().Title;
				Author = videos.First().Info.Author;
				foreach( YouTubeVideo item in videos )
					_infoDatas.Add( new InfoData( item ) );
			}
		}
		#endregion

		#region PUBLIC METHODS
		public override string ToString() => $"\"{Title}\"\t\"{Author}\"\t{_infoDatas.Count()}\n" + string.Join( "\n" , _infoDatas );
		#endregion
	}
}
