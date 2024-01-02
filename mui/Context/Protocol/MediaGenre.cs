using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using xnext.Diagnostics;

namespace mui.Context.Protocol
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
		public class MediaStyle
		{
			[XmlAttribute] public string Label { get; set; }
			[XmlText] public string Description { get; set; }
		}
		#endregion

		#region LOCAL VARIABLE
		private List<MediaStyle> _Items { get; set; } = new List<MediaStyle>();
		#endregion


		#region PUBLIC PROPERTIES
		[XmlAttribute] public AdaptiveKind Type { get; set; }
		[XmlAttribute] public string Label { get; set; }
		[XmlText] public string Description { get; set; }

		public MediaStyle[] Details
		{
			get => _Items.ToArray();
			set => _Items.AddRange( value );
		}
		#endregion

		#region ACCESSORS
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
		#endregion

		#region PUBLIC METHODS
		/// <summary>
		/// What: Add a style to a specific Genre
		///  Why: Allow the end-user to add/modify a style and/or its description
		/// </summary>
		internal MediaStyle AddUpdate( string style , string description )
		{
			LogTrace.Label( $"{style},{description}" );
			foreach( MediaStyle ms in Details )
				if( ms.Label == style )
				{
					ms.Description = description;
					return ms;
				}
			_Items.Add( new MediaStyle() { Label = style , Description = description } );
			return this[style];
		}
		/// <summary>
		/// What: Remove a style from a genre
		///  Why: Allow the end-user to remove a specific style from a genre
		/// </summary>
		internal void Remove( MediaStyle mediaStyle )
		{
			LogTrace.Label( mediaStyle.Label );

			_Items.Remove( mediaStyle );
		}
		#endregion
	}
}
