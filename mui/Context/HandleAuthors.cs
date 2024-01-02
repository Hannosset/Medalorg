using mui.Context.Protocol;

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
	/// What: List of the Downloaded video authors
	///  Why: helps recognizing the name of the author from the media caption
	/// </summary>
	internal sealed class HandleAuthors
	{
		#region LOCAL VARIABLE
		/// <summary>
		/// What: List of the available author of the singleton
		///  Why: easily adds a new author in the list
		/// </summary>
		private List<AuthorInfo> _Details = new List<AuthorInfo>();
		#endregion LOCAL VARIABLE

		#region ACCESSORS
		/// <summary>
		/// What: array of the author information
		///  Why: easily access a read-only array
		/// </summary>
		internal AuthorInfo[] Details => _Details.ToArray();
		/// <summary>
		/// What: Access the author data from the author name
		///  Why: simple method to determine if the author is already registered in the data set
		/// </summary>
		internal AuthorInfo this[string author]
		{
			get
			{
				lock( Info )
					foreach( AuthorInfo ai in Details )
						if( ai.Name.ToLower().CompareTo( author.ToLower() ) == 0 )
							return ai;
				return null;
			}
		}
		#endregion ACCESSORS

		#region SINGLETON
		/// <summary>
		/// What: Singleton instance of the object
		///  Why: Allow all application's methods to access the singleton
		/// </summary>
		internal static HandleAuthors Info { get; private set; } = new HandleAuthors();
		#endregion SINGLETON

		#region PUBLIC METHODS
		/// <summary>
		/// What: Load the author from the xml file and initialize the author if not existing.
		///  Why: Allow to specifically populate the singleton instance when initializing the application.
		/// </summary>
		internal static void LoadFromFile()
		{
				LogTrace.Label();
				Info._Details = new List<AuthorInfo>( Deserialize() );
		}
		/// <summary>
		/// What: Adds a new author in the singleton list - inversely sort the authors 
		///  Why: Enrich the singleton container with a new information if the author is not already registered
		///  Return: the object instance of the newly updated/added data
		/// </summary>
		internal static AuthorInfo Update( string author )
		{
			if( !string.IsNullOrEmpty( author ) )
			{
				lock( Info )
				{
					//	Add and sort the array in decreasing order
					if( Info[author] == null )
					{
						Info._Details.Add( new AuthorInfo { Name = author } );
						Info._Details.Sort( ( a , b ) => b.Name.CompareTo( a.Name ) );
					}
					return Info[author];
				}
			}
			return null;
		}
		/// <summary>
		/// What: Flushes the data set on the hard disk
		///  Why: allow the changes to be committed on the support preventing any loss of information in case of crash
		/// </summary>
		internal static void Flush()
		{
			lock( Info )
				Info.Serialize();
		}
		#endregion PUBLIC METHODS

		#region SERIALIZATION
		/// <summary>
		/// What: Name of the singleton
		///  Why: The name of the object is also identifying the object serialization on the support
		/// </summary>
		private const string Name = "Authors";
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
						new XmlSerializer( typeof( AuthorInfo[] ) ).Serialize( fs , Details );
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
		private static AuthorInfo[] Deserialize( string filename = null )
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
						return new XmlSerializer( typeof( AuthorInfo[] ) ).Deserialize( reader ) as AuthorInfo[];
				}
			}
			catch( System.Exception ex )
			{
				Logger.TraceException( ex , "The new media will not be loaded" , $"Confirm the Data Path in the configuration file is correct and confirm read access to the path and the file ({filename} )" );
			}
			return Array.Empty<AuthorInfo>();
		}
		#endregion SERIALIZATION
	}
}
