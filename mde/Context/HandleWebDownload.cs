using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using xnext.Diagnostics;

namespace mde.Context
{
	internal class HandleWebDownload
	{

		#region ACCESSORS
		/// <summary>
		/// What: array of the strings to skip
		///  Why: easily access a read-only array
		/// </WebDownload>
		internal WebDownload[] Details { get; private set; }
		#endregion ACCESSORS

		#region SINGLETON
		/// <summary>
		/// What: Singleton instance of the object
		///  Why: Allow all application's methods to access the singleton
		/// </summary>
		internal static HandleWebDownload Info { get; private set; } = new HandleWebDownload();
		#endregion SINGLETON

		#region PUBLIC METHODS
		/// <summary>
		/// What: Load the author from the xml file and initialize the author if not existing.
		///  Why: Allow to specifically populate the singleton instance when initializing the application.
		/// </summary>
		public static WebDownload[] LoadFromFile( string filename )
		{
			if( File.Exists( filename ) )
			{
				LogTrace.Label();
				Info.Details = Deserialize(filename);
				return Info.Details;
			}
			return Array.Empty<WebDownload>();
		}
		#endregion PUBLIC METHODS

		#region SERIALIZATION
		/// <summary>
		/// What: Initialize the singleton with the data set from the hard disk
		///  Why: initialize the singleton object with the updated data from the support.
		/// </summary>
		private static WebDownload[] Deserialize( string filename )
		{
			try
			{
				LogTrace.Label( filename );
				using( FileStream fs = new FileStream( filename , FileMode.Open , FileAccess.Read , FileShare.Read ) )
				using( XmlReader reader = XmlReader.Create( fs , new XmlReaderSettings() { XmlResolver = null } ) )
					return new XmlSerializer( typeof( WebDownload[] ) ).Deserialize( reader ) as WebDownload[];
			}
			catch( System.Exception ex )
			{
				Logger.TraceException( ex , "The Web download command is corrupt" , $"verify you have the latest version of the package - contact support with the log files." );
			}
			return Array.Empty<WebDownload>();
		}
		#endregion SERIALIZATION
	}
}
