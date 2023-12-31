﻿using System;
using System.Diagnostics;

namespace xnext.Diagnostics
{
	/// <summary>
	/// Log event to the OS event viewer, and/or Log file.
	/// </summary>
	public class Logger : IDisposable
	{
		#region PUBLIC PROPERTIES
		/// <summary>
		/// Gets a value indicating whether this instance is service.
		/// </summary>
		/// <value><c>true</c> if this instance is service; otherwise, <c>false</c>.</value>
		public static bool IsService => LogIssues.IsService;
		public LogIssues LogFile { get; private set; } = new LogIssues();
		public RecordIssue Record { get; private set; } = new RecordIssue();
		#endregion PUBLIC PROPERTIES

		#region SINGLETON
		public static Logger Instance { get; private set; } = new Logger();
		#endregion SINGLETON

		#region PUBLIC METHODS
		public static void TraceWarning( string message ) => TraceWarning( message , null , null );
		public static void TraceWarning( string message , string consequence , string reaction )
		{
			if( Instance.LogFile != null && Instance.LogFile.NewLogFile )
				Instance.LogFile?.WriteLine( "UTC Date|UTC Time (HH:mm)|local Time (HH:mm)|TraceEventType|Context|Message|Consequence|Reaction|Module(:line)|function|details" );
			Instance.LogFile?.WriteLine( Instance.Record.Step( TraceEventType.Warning , message , consequence , reaction , string.Empty ) );
			LogTrace.Label( message );
		}
		public static void TraceError( string message , string consequence , string reaction )
		{
			if( Instance.LogFile != null && Instance.LogFile.NewLogFile )
				Instance.LogFile?.WriteLine( "UTC Date|UTC Time (HH:mm)|local Time (HH:mm)|TraceEventType|Context|Message|Consequence|Reaction|Module(:line)|function|details" );
			Instance.LogFile?.WriteLine( Instance.Record.Step( TraceEventType.Error , message , consequence , reaction , string.Empty ) );
			LogTrace.Label( message );
		}
		/// <summary>
		///  What: Traces the event
		///  Why: if the caller is a service log the event in the log viewer, otherwise trace the information if the calling function is recording to be tracing data.
		/// </summary>
		public static void TraceException( Exception ex , string consequence , string reaction )
		{
			if( ex is null )
				throw new ArgumentNullException( nameof( ex ) );
			if( Instance.LogFile != null && Instance.LogFile.NewLogFile )
				Instance.LogFile?.WriteLine( "UTC Date|UTC Time (HH:mm)|local Time (HH:mm)|TraceEventType|Context|Message|Consequence|Reaction|Module(:line)|function|details" );
			Instance.LogFile?.WriteLine( Instance.Record.Step( ex , consequence , reaction ) );
			LogTrace.Label( ex.ToString() );
#if DEBUG
			Console.WriteLine( ex.ToString() );
#endif
		}
		#endregion PUBLIC METHODS

		#region IDisposable Support
		private bool disposedValue; // To detect redundant calls

		protected virtual void Dispose( bool disposing )
		{
			if( !disposedValue )
			{
				if( disposing )
				{
					LogFile?.Dispose();
					LogFile = null;
				}

				disposedValue = true;
			}
		}
		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose( true );
			// TODO: uncomment the following line if the finalizer is overridden above.
			GC.SuppressFinalize( this );
		}
		#endregion IDisposable Support
	}
}