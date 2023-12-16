using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using xnet.Diagnostics;

namespace mui.Context
{
	/// <summary>
	/// Movies: https://www.studiobinder.com/blog/movie-genres-list/
	/// Musi: 
	///		https://www.musicgenreslist.com/
	///		https://www.musicgrotto.com/music-genres/
	///		https://en.wikipedia.org/wiki/List_of_music_genres_and_styles
	/// </summary>
	public class MediaGenre
	{
		#region TYPES
		public enum MediaType { Audio, Video };

		public class MediaStyle
		{
			[XmlAttribute]
			public string Label { get; set; }

			[XmlText]
			public string Description { get; set; }
		}
		#endregion

		#region LOCAL VARIABLE
		private List<MediaStyle> _Items { get; set; } = new List<MediaStyle>();
		#endregion


		#region PUBLIC PROPERTIES
		[XmlAttribute]
		public MediaType Type { get; set; }

		[XmlAttribute]
		public string Label { get; set; }

		[XmlText]
		public string Description { get; set; }

		public MediaStyle[] Details
		{
			get => _Items.ToArray();
			set => _Items.AddRange( value );
		}
		#endregion

		public MediaStyle this[string label]
		{
			get
			{
					foreach( MediaStyle ms in Details )
						if( ms.Label == label )
							return ms;
				return null;
			}
		}
		#region PUBLIC METHODS
		internal MediaStyle Addpdate( string style , string description )
		{
			LogTrace.Label($"{style},{description}");
			foreach( MediaStyle ms in Details )
				if( ms.Label == style )
				{
					ms.Description = description;
					return ms;
				}
			_Items.Add( new MediaStyle() { Label = style , Description = description } );
			return this[style];
		}

		internal void Remove( MediaStyle mediaStyle )
		{
			LogTrace.Label(mediaStyle.Label);

			_Items.Remove( mediaStyle );
		}
		#endregion
	}
}
