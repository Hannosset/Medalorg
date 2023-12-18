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
		private List<AuthorInfo> _Details = new List<AuthorInfo>();
		#endregion LOCAL VARIABLE

		#region ACCESSORS
		internal AuthorInfo[] Details => _Details.ToArray();
		internal AuthorInfo this[string author]
		{
			get
			{
				lock( Info )
					foreach( AuthorInfo ai in Details )
						if( ai.Name == author )
							return ai;
				return null;
			}
		}
		#endregion ACCESSORS

		#region SINGLETON
		/// <summary>Gets the information.</summary>
		/// <value>The information.</value>
		internal static HandleAuthors Info { get; private set; } = new HandleAuthors();
		#endregion SINGLETON

		#region PUBLIC METHODS
		public static void LoadFromFile()
		{
				LogTrace.Label();
				Info._Details = new List<AuthorInfo>( Deserialize() );
		}
		public static AuthorInfo Update( string author )
		{
			if( !string.IsNullOrEmpty( author ) )
			{
				lock( Info )
				{
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
		public static void Flush()
		{
			lock( Info )
				Info.Serialize();
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
						new XmlSerializer( typeof( AuthorInfo[] ) ).Serialize( fs , Details );
				}
				catch( System.Exception ex )
				{
					Logger.TraceException( ex , "The new media will not be saved" , $"Confirm the Data Path in the configuration file is correct and confirm read/write access to the path and the file ({filename} )" );
				}
		}
		/// <summary>Deserializes the specified filename <param name="filename">The filename.</param></summary>
		/// m&gt;
		/// <returns></returns>
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
