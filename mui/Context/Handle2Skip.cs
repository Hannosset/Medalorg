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
	/// What: List of the string to remove form the caption
	///  Why: helps recognizing the name of the author and the title of the media from the internet caption
	/// </summary>
	internal sealed class Handle2Skip
	{
		#region LOCAL VARIABLE
		/// <summary>
		/// What: List of the available author of the singleton
		///  Why: easily adds a new string to skip in the list
		/// </summary>
		private List<string> _Details = new List<string>();
		#endregion LOCAL VARIABLE

		#region ACCESSORS
		/// <summary>
		/// What: array of the strings to skip
		///  Why: easily access a read-only array
		/// </summary>
		internal string[] Details => _Details.ToArray();
		#endregion ACCESSORS

		#region SINGLETON
		/// <summary>
		/// What: Singleton instance of the object
		///  Why: Allow all application's methods to access the singleton
		/// </summary>
		internal static Handle2Skip Info { get; private set; } = new Handle2Skip();
		#endregion SINGLETON

		#region PUBLIC METHODS
		/// <summary>
		/// What: Load the author from the xml file and initialize the author if not existing.
		///  Why: Allow to specifically populate the singleton instance when initializing the application.
		/// </summary>
		internal static void LoadFromFile()
		{
			if( !File.Exists( Filename ) )
			{
				LogTrace.Label();
				Info._Details = new List<string>( Deserialize() );
			}
		}
		/// <summary>
		/// What: Adds a new string to skip in the singleton list.
		///  Why: Enrich the singleton container with a new information if the string to skip is not already registered
		/// </summary>
		internal static void Update( string str )
		{
			if( !string.IsNullOrEmpty( str ) )
			{
				lock( Info )
				{
					if( !Info._Details.Contains( str ) )
					{
						Info._Details.Add( str );
						Info._Details.Sort( ( a , b ) => b.CompareTo( a ) );
						Info.Serialize();
					}
				}
			}
		}
		#endregion PUBLIC METHODS

		#region SERIALIZATION
		/// <summary>
		/// What: Name of the singleton
		///  Why: The name of the object is also identifying the object serialization on the support
		/// </summary>
		private const string Name = "2skip";
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
		private void Serialize( string filename = null )
		{
			if( filename == null )
				filename = Filename;
			LogTrace.Label( filename );

			lock( Info )
				try
				{
					using( FileStream fs = new FileStream( filename , FileMode.Create , FileAccess.Write , FileShare.None ) )
						new XmlSerializer( typeof( string[] ) ).Serialize( fs , Details );
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
		private static string[] Deserialize( string filename = null )
		{
			if( filename == null )
				filename = Filename;
			try
			{
				if( File.Exists( filename ) )
				{
					LogTrace.Label( filename );
					using( FileStream fs = new FileStream( filename , FileMode.Open , FileAccess.Read , FileShare.Read ) )
					using( XmlReader reader = XmlReader.Create( fs , new XmlReaderSettings() { XmlResolver = null } ) )
						return new XmlSerializer( typeof( string[] ) ).Deserialize( reader ) as string[];
				}
			}
			catch( System.Exception ex )
			{
				Logger.TraceException( ex , "The new media will not be loaded" , $"Confirm the Data Path in the configuration file is correct and confirm read access to the path and the file ({filename} )" );
			}
			return Array.Empty<string>();
		}
		#endregion SERIALIZATION
	}
}
