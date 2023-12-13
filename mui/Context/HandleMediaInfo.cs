using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using System.Xml;
using System.Xml.Serialization;

using xnet.Diagnostics;

namespace mui.Context
{
	internal sealed class HandleMediaInfo
	{
		#region LOCAL VARIABLE
		/// <summary>The details of the security list for the streaming</summary>
		private List<MediaInfo> _Details = new List<MediaInfo>();
		#endregion LOCAL VARIABLE

		#region ACCESSORS
		internal MediaInfo[] Details => _Details.ToArray();
		/// <summary>Gets the <see cref="RateSecurityData"/> with the specified currency.</summary>
		/// <value>The <see cref="RateSecurityData"/>.</value>
		/// <param name="currency">The currency.</param>
		/// <returns></returns>
		internal MediaInfo this[string videoId]
		{
			get
			{
				lock( Info )
					foreach( MediaInfo sd in Details )
						if( sd.VideoId == videoId )
							return sd;
				return null;
			}
		}
		#endregion ACCESSORS

		#region SINGLETON
		/// <summary>Gets the information.</summary>
		/// <value>The information.</value>
		internal static HandleMediaInfo Info { get; private set; } = new HandleMediaInfo();
		#endregion SINGLETON

		#region PUBLIC METHODS
		public static void LoadFromFile()
		{
			if( !File.Exists( Filename ) )
				using( FileStream fs = new FileStream( Filename , FileMode.Create , FileAccess.Write , FileShare.Read ) )
					new XmlSerializer( typeof( MediaInfo[] ) ).Serialize( fs , Array.Empty<MediaInfo>() );

			using( FileStream fs = new FileStream( Filename , FileMode.Open , FileAccess.Read , FileShare.Read ) )
			using( XmlReader reader = XmlReader.Create( fs , new XmlReaderSettings() { XmlResolver = null } ) )
				Info._Details = new List<MediaInfo>( new XmlSerializer( typeof( MediaInfo[] ) ).Deserialize( reader ) as MediaInfo[] );
		}
		public static void Update( string[] fields )
		{
			if( fields.Length > 3 )
				lock( Info )
				{
					if( Info[fields[0]] == null )
						Info._Details.Add( new MediaInfo( fields[0] , fields[1] , fields[2] ) );
					else if( fields.Length > 6 )
						Info[fields[0]].Add( fields );
				}
		}
		#endregion PUBLIC METHODS

		#region SERIALIZATION
		/// <summary>The name</summary>
		internal const string Name = "MediaInfo";

		/// <summary>Gets the filename.</summary>
		/// <value>The filename associated with the object.</value>
		internal static string Filename
		{
			get
			{
				FileInfo fi = new FileInfo( Path.Combine( MemoryCache.Default["DataPath"] as string , Name + ".xml" ) );
				if( !fi.Directory.Exists )
					fi.Directory.Create();
				return fi.FullName;
			}
		}
		/// <summary>Serializes the specified filename.</summary>
		/// <param name="filename">The filename.</param>
		internal void Serialize( string filename = null )
		{
			if( filename == null )
				filename = Filename;
			lock( Info )
				try
				{
					using( FileStream fs = new FileStream( filename , FileMode.Create , FileAccess.Write , FileShare.None ) )
						new XmlSerializer( typeof( MediaInfo[] ) ).Serialize( fs , Details );
				}
				catch( System.Exception ex )
				{
					Logger.TraceException( ex , "The new media will not be saved" , $"Confirm the Data Path in the configuration file is correct and confirm read/write access to the path and the file ({filename} )" );
				}
		}
		/// <summary>Deserializes the specified filename <param name="filename">The filename.</param></summary>
		/// m&gt;
		/// <returns></returns>
		internal static MediaInfo[] Deserialize( string filename = null )
		{
			if( filename == null )
				filename = Filename;
			try
			{
				if( File.Exists( filename ) )
					using( FileStream fs = new FileStream( filename , FileMode.Open , FileAccess.Read , FileShare.Read ) )
					using( XmlReader reader = XmlReader.Create( fs , new XmlReaderSettings() { XmlResolver = null } ) )
						return new XmlSerializer( typeof( MediaInfo[] ) ).Deserialize( reader ) as MediaInfo[];
			}
			catch( System.Exception ex )
			{
				Logger.TraceException( ex , "The new media will not be loaded" , $"Confirm the Data Path in the configuration file is correct and confirm read access to the path and the file ({filename} )" );
			}
			return Array.Empty<MediaInfo>();
		}
		#endregion SERIALIZATION
	}
}
