﻿using xnet.Files;

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Caching;

namespace xnet.Diagnostics
{
	/// <summary>
	/// Class LogRecTrace. This class cannot be inherited.
	/// </summary>
	public class LogIssues : IDisposable
	{
		/// <summary>
		/// RealTime Business Trace
		/// </summary>
		protected BufferedFile TraceFile { get; private set; } = new BufferedFile();

		private DateTime TraceStamp = DateTime.MinValue.Date;
		protected string Extension { get; set; } = "log";

		#region PUBLIC PROPERTIES
		public bool NewLogFile { get; private set; }
		/// <summary>
		/// Gets or sets the trace level.
		/// </summary>
		/// <value> The trace level. </value>
		public int TraceLevel { get; set; }
		/// <summary>
		/// Gets the identifier or production of the running application
		/// </summary>
		/// <value> The identifier. </value>
		public string Identifier { get; set; }
		#endregion PUBLIC PROPERTIES

		#region ACCESSORS
		/// <summary>
		/// Gets or sets the enabled flag.
		/// </summary>
		/// <value> The enabled flag. </value>
		public bool Enabled => TraceLevel > 0;
		/// <summary>
		/// Gets a value indicating whether this instance is service.
		/// </summary>
		/// <value> <c> true </c> if this instance is service; otherwise, <c> false </c>. </value>
		public static bool IsService { get; private set; } = (Native.NativeMethods.GetStandardHandle( -11 ) == IntPtr.Zero);
		#endregion ACCESSORS

		#region CONSTRUCTORS
		/// <summary>
		/// Initializes a new instance of the <see cref="RecordIssue"/> class.
		/// </summary>
		/// <param name="identifier"> The identifier. </param>
		public LogIssues( string identifier = "" )
		{
			Identifier = identifier;

			if( string.IsNullOrEmpty( Identifier ) )
			{
				Identifier = Process.GetCurrentProcess().MainModule.FileVersionInfo.ProductName;
				if( Identifier.Contains( "." ) )
					Identifier = Identifier.Substring( 0 , Identifier.LastIndexOf( '.' ) );
			}
		}
		#endregion CONSTRUCTORS

		/// <summary>
		/// Writes the line.
		/// </summary>
		/// <param name="fields"> The fields. </param>
		public void WriteLine( string[] fields )
		{
			if( fields != null )
			{
				if( DateTime.UtcNow.Day != TraceStamp.Day )
					ResetLogFile();

				TraceEventType type = TraceEventType.Verbose;
				foreach( string s in fields )
					if( Enum.TryParse( s , out type ) )
						break;

				if( Convert.ToInt32( type , CultureInfo.CurrentCulture ) <= TraceLevel )
				{
					TraceFile?.DumpLine( string.Join( "|" , fields.Select( x => x.Replace( "\n" , " " ).Replace( "\t" , "  " ) ) ) );
					if( Convert.ToInt32( type , CultureInfo.CurrentCulture ) < Convert.ToInt32( TraceEventType.Warning , CultureInfo.CurrentCulture ) )
						TraceFile?.Flush();
					NewLogFile = false;
				}
			}
		}
		/// <summary>
		/// Writes the line.
		/// </summary>
		/// <param name="line"> The log. </param>
		public void WriteLine( string line ) => WriteString( line.Replace( "\n" , " " ).Replace( "\t" , "  " ) );
		public void WriteString( string str )
		{
			if( !string.IsNullOrEmpty( str ) )
			{
				if( DateTime.UtcNow.Day != TraceStamp.Day || TraceStamp == DateTime.MinValue.Date )
					ResetLogFile();
				TraceFile?.DumpLine( str );
				NewLogFile = false;
			}
		}
		public void Flush()
		{
			try { TraceFile?.Flush(); } catch( System.Exception ) { }
		}
		protected virtual bool ResetLogFile()
		{
			TraceFile?.Flush();
			TraceFile?.Close();
			TraceStamp = DateTime.UtcNow;
			if( MemoryCache.Default["LogPath"] != null )
			{
				if( !File.Exists( $@"{MemoryCache.Default["LogPath"]}\{Identifier}.{TraceStamp:yyyy-MM-dd}.{Extension}" ) )
					NewLogFile = true;
				TraceFile.Open( $@"{MemoryCache.Default["LogPath"]}\{Identifier}.{TraceStamp:yyyy-MM-dd}.{Extension}" );
				return true;
			}
			else
				TraceFile = null;
			return false;
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"> <c> true </c> to release both managed and unmanaged resources; <c> false </c> to release only unmanaged resources. </param>
		protected virtual void Dispose( bool disposing )
		{
			if( disposing )
			{
				TraceFile?.Flush();
				TraceFile?.Close();
				TraceFile = null;
			}
		}
		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose( true ); GC.SuppressFinalize( this );
		}
	}
}