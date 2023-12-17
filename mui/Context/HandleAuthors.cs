using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using System.Xml;
using System.Xml.Serialization;

using xnext.Diagnostics;

namespace mui.Context
{
	internal class HandleAuthors
	{
		#region LOCAL VARIABLE
		/// <summary>The details of the security list for the streaming</summary>
		private List<string> _Details = new List<string>();
		#endregion LOCAL VARIABLE

		#region ACCESSORS
		internal string[] Details => _Details.ToArray();
		#endregion ACCESSORS

		#region SINGLETON
		/// <summary>Gets the information.</summary>
		/// <value>The information.</value>
		internal static HandleAuthors Info { get; private set; } = new HandleAuthors();
		#endregion SINGLETON

		#region PUBLIC METHODS
		public static void LoadFromFile()
		{
			if( !File.Exists( Filename ) )
			{
				LogTrace.Label();
				Info._Details = new List<string>( Deserialize() );
			}
		}
		public static void Update( string author )
		{
			if( !string.IsNullOrEmpty( author ) )
			{
				lock( Info )
				{
					if( !Info._Details.Contains( author ) )
					{
						Info._Details.Add( author );
						Info._Details.Sort( ( a , b ) => b.CompareTo( a ) );
						Info.Serialize();
					}
				}
			}
		}
		#endregion PUBLIC METHODS

		#region SERIALIZATION
		/// <summary>The name</summary>
		internal const string Name = "Authors";
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
		/// <summary>Deserializes the specified filename <param name="filename">The filename.</param></summary>
		/// m&gt;
		/// <returns></returns>
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
