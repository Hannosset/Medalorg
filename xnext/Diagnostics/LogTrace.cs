using System;
using System.Diagnostics;

using xnext.Context;

namespace xnext.Diagnostics
{
	/// <summary>
	/// Class LogTrace. Implements the <see cref="xnet.Diagnostics.Logger"/> Log events in a file.
	/// <code>
	///The format is as follows:
	///Date, Time, TraceEventType, Context, message, consequence , function,line, details
	/// </code>
	/// </summary>
	/// <seealso cref="xnet.Diagnostics.Logger"/>
	public sealed class LogTrace : LogIssues
	{
		#region LOCAL VARIABLE
		private int IndentLevel;
		private readonly TimeZoneInfo TraceTimeZone = TimeZoneInfo.Local;
		#endregion LOCAL VARIABLE

		#region SINGLETON
		/// <summary>Gets the instance.</summary>
		/// <value>The instance.</value>
		public static LogTrace Instance { get; } = new LogTrace { Extension = "txt" };
		#endregion SINGLETON

		#region CONSTRUCTOR
		private LogTrace()
		{
			string TraceTimeZoneId = CltWinEnv.AppReadSetting.GetData( "Log" , "Time Zone ID" );
			if( !string.IsNullOrEmpty( TraceTimeZoneId ) )
				TraceTimeZone = TimeZoneInfo.FindSystemTimeZoneById( TraceTimeZoneId );
		}
		#endregion CONSTRUCTOR

		#region PUBLIC METHODS
		public static void Begin() => Begin( string.Empty );
		public static void Begin( int at ) => Begin( at , string.Empty );
		public static void Begin( string message )
		{
			if( !string.IsNullOrEmpty( message ) )
				Label( message );
			Instance.IndentLevel += 1;
		}
		public static void Begin( int at , string message )
		{
			if( !string.IsNullOrEmpty( message ) )
				Label( message );
			Instance.IndentLevel = at;
		}
		public static void Label() => Label( string.Empty );
		public static void Label( string message )
		{
			StackTrace stackTrace = new StackTrace( 1 , true );

			for( int i = 0 ; i < stackTrace.FrameCount ; i++ )
				if( !stackTrace.GetFrame( i ).GetMethod().ReflectedType.FullName.Contains( "Diagnostics" )
					&& !stackTrace.GetFrame( i ).GetMethod().ReflectedType.FullName.Contains( "LogTrace" ) )
				{
					string Method = stackTrace.GetFrame( i ).GetMethod().ReflectedType.FullName;

					foreach( string s in @"Application.,.Kernel.,.Net.Protocol.,.Net.,.Services.".Split( ',' ) )
						if( Method.IndexOf( s , StringComparison.OrdinalIgnoreCase ) >= 0 )
						{
							Method = Method.Substring( Method.IndexOf( s , StringComparison.OrdinalIgnoreCase ) + s.Length );
							break;
						}
					if( string.IsNullOrEmpty( message ) )
						message = $"{Method}[{stackTrace.GetFrame( i ).GetFileLineNumber()}].{stackTrace.GetFrame( i ).GetMethod().Name}";
					else
						message = $"{Method}[{stackTrace.GetFrame( i ).GetFileLineNumber()}].{stackTrace.GetFrame( i ).GetMethod().Name}: {message}";
					break;
				}
			lock( Instance )
			{
				Instance.WriteLine( $"{TimeZoneInfo.ConvertTimeFromUtc( DateTime.UtcNow , Instance.TraceTimeZone ):HH:mm:ss}>{new string( ' ' , Math.Max( 1 , Instance.IndentLevel * 2 ) )}{message}" );
				if( !IsService )
					Console.WriteLine( $"{TimeZoneInfo.ConvertTimeFromUtc( DateTime.UtcNow , Instance.TraceTimeZone ):HH:mm:ss}>{new string( ' ' , Math.Max( 1 , Instance.IndentLevel * 2 ) )}{message}" );
			}
		}
		public static void Tag( string message )
		{
			StackTrace stackTrace = new StackTrace( 1 , true );

			for( int i = 0 ; i < stackTrace.FrameCount ; i++ )
				if( !stackTrace.GetFrame( i ).GetMethod().ReflectedType.FullName.Contains( "Diagnostics" )
					&& !stackTrace.GetFrame( i ).GetMethod().ReflectedType.FullName.Contains( "LogTrace" ) )
				{
					string Method = stackTrace.GetFrame( i ).GetMethod().ReflectedType.FullName;

					foreach( string s in @"Application.,.Kernel.,.Net.Protocol.,.Net.,.Services.".Split( ',' ) )
						if( Method.IndexOf( s , StringComparison.OrdinalIgnoreCase ) >= 0 )
						{
							Method = Method.Substring( Method.IndexOf( s , StringComparison.OrdinalIgnoreCase ) + s.Length );
							break;
						}
					if( string.IsNullOrEmpty( message ) )
						message = $"{Method}[{stackTrace.GetFrame( i ).GetFileLineNumber()}].{stackTrace.GetFrame( i ).GetMethod().Name}";
					else
						message = $"{Method}[{stackTrace.GetFrame( i ).GetFileLineNumber()}].{stackTrace.GetFrame( i ).GetMethod().Name}: {message.Replace( "\n" , " " ).Replace( "\t" , "  " )}";
					break;
				}
			lock( Instance )
			{
				Instance.WriteLine( $"{TimeZoneInfo.ConvertTimeFromUtc( DateTime.UtcNow , Instance.TraceTimeZone ):HH:mm:ss}>{message}" );
				if( !IsService )
					Console.WriteLine( $"{TimeZoneInfo.ConvertTimeFromUtc( DateTime.UtcNow , Instance.TraceTimeZone ):HH:mm:ss}>{message}" );
			}
		}

		public static void End() => End( string.Empty );
		public static void End( string message )
		{
			Instance.IndentLevel -= 1;
			if( !string.IsNullOrEmpty( message ) )
				Label( message );
			Instance.TraceFile?.Flush();
		}
		#endregion PUBLIC METHODS

		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}
	}
}