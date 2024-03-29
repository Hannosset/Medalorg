﻿using mui.Context.Protocol;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using System.Xml;
using System.Xml.Serialization;

using xnext.Diagnostics;

namespace mui.Context
{
	/// <summary>
	/// What:Record all the media that have been made available for download
	///  Why: To ensure uniqueness and allow specific download of format and resolution 
	/// </summary>
	internal sealed class HandleMediaInfo
	{
		#region LOCAL VARIABLE
		/// <summary>
		/// What: List of the available media of the singleton
		///  Why: easily adds a new media in the list
		/// </summary>
		private List<MediaInfo> _Details = new List<MediaInfo>();
		#endregion LOCAL VARIABLE

		#region ACCESSORS
		/// <summary>
		/// What: array of the media information
		///  Why: easily access a read-only array
		/// </summary>
		internal MediaInfo[] Details => _Details.ToArray();
		/// <summary>
		/// What: Access the Media data from the internet video Id
		///  Why: simple method to determine if the video is already registered in the data set
		/// </summary>
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
		/// <summary>
		/// What: Singleton instance of the object
		///  Why: Allow all application's methods to access the singleton
		/// </summary>
		internal static HandleMediaInfo Info { get; private set; } = new HandleMediaInfo();
		#endregion SINGLETON

		#region PUBLIC METHODS
		/// <summary>
		/// What: Load the author from the xml file.
		///  Why: Allow to specifically populate the singleton instance when initializing the application.
		/// </summary>
		internal static void LoadFromFile()
		{
			if( !File.Exists( Filename ) )
				using( FileStream fs = new FileStream( Filename , FileMode.Create , FileAccess.Write , FileShare.Read ) )
					new XmlSerializer( typeof( MediaInfo[] ) ).Serialize( fs , Array.Empty<MediaInfo>() );

			MediaInfo[] mil = Deserialize();
			for( int i = 0 ; i < mil.Length ; i++ )
			{
				int at = mil[i].VideoId.IndexOf( "\\u0026list" );
				if( at > 0 )
					mil[i].VideoId = mil[i].VideoId.Substring( 0 , at );
				if( mil[i].Details.Length > 0 )
					Info._Details.Add( mil[i] );
			}
		}
		/// <summary>
		/// What: Adds a new media in the singleton list 
		///  Why: Enrich the singleton container with a new information if the media is not already registered
		/// </summary>
		internal static MediaInfo Update( string[] fields )
		{
			LogTrace.Label( string.Join( "," , fields ) );

			if( fields.Length > 3 )
			{
				lock( Info )
				{
					if( Info[fields[0]] == null )
						Info._Details.Add( new MediaInfo( fields[0] , fields[1] , fields[2] ) );
					else if( fields.Length > 6 && Info[fields[0]].Add( fields ) == null )
						return null;
					return Info[fields[0]];
				}
			}
			return null;
		}
		internal void Remove( string name )
		{
			MediaInfo mi = this[name];

			if( mi != null )
			{
				_Details.Remove( mi );
			}
		}
		#endregion PUBLIC METHODS

		#region SERIALIZATION
		/// <summary>
		/// What: Name of the singleton
		///  Why: The name of the object is also identifying the object serialization on the support
		/// </summary>
		private const string Name = "MediaInfo";
		/// <summary>
		/// What: Filename used to load/save the content of the singleton
		///  Why: one filename access allowing a centralization of the filename management
		/// </summary>
		private static string Filename
		{
			get
			{
				FileInfo fi = new FileInfo( Path.Combine( MemoryCache.Default["DataPath"] as string , Name + ".xml" ) );
				if( !fi.Directory.Exists )
					fi.Directory.Create();
				return fi.FullName;
			}
		}
		/// <summary>
		/// What: Serialization of the singleton
		///  Why: allow to save on the file system the content of the singleton - allowing manual alteration or simple recovery by deleting the file.
		/// </summary>
		internal void Serialize( string filename = null )
		{
			if( string.IsNullOrEmpty( filename ) )
				filename = Filename;
			lock( Info )
				try
				{
					LogTrace.Label( filename );
					try
					{
						if( File.Exists( filename.Replace( ".xml" , ".bak" ) ) )
							File.Delete( filename.Replace( ".xml" , ".bak" ) );
					}
					catch( Exception ) { }
					try { File.Move( filename , filename.Replace( ".xml" , ".bak" ) ); } catch( Exception ) { }

					using( FileStream fs = new FileStream( filename , FileMode.Create , FileAccess.Write , FileShare.None ) )
						new XmlSerializer( typeof( MediaInfo[] ) ).Serialize( fs , Details );
				}
				catch( System.Exception ex )
				{
					Logger.TraceException( ex , "The new media will not be saved" , $"Confirm the Data Path in the configuration file is correct and confirm read/write access to the path and the file ({filename} )" );
				}
		}
		/// <summary>
		/// What: Initialize the singleton with the data set from the hard disk
		///  Why: initialize the singleton object with the updated data from the support.
		/// </summary>
		private static MediaInfo[] Deserialize( string filename = null )
		{
			if( string.IsNullOrEmpty( filename ) )
				filename = Filename;
			try
			{
				LogTrace.Label( filename );
				if( !File.Exists( filename ) )
					filename = filename.Replace( ".xml" , ".bak" );

				if( File.Exists( filename ) )
					using( FileStream fs = new FileStream( filename , FileMode.Open , FileAccess.Read , FileShare.Read ) )
					using( XmlReader reader = XmlReader.Create( fs , new XmlReaderSettings() { XmlResolver = null } ) )
						return new XmlSerializer( typeof( MediaInfo[] ) ).Deserialize( reader ) as MediaInfo[];
			}
			catch( System.Exception ex )
			{
				Logger.TraceException( ex , "The new media will not be loaded" , $"Confirm the Data Path in the configuration file is correct and confirm read access to the path and the file ({filename} )" );
				return Deserialize( filename.Replace( ".xml" , ".bak" ) );
			}
			return Array.Empty<MediaInfo>();
		}
		#endregion SERIALIZATION
	}
}
