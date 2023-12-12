using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace xnet.Diagnostics
{
	/// <summary>
	/// Object executing a executable and redirecting the I/O to events
	/// </summary>

	public sealed class Execute : IDisposable
	{
		#region PRIVATE VARIABLES
		private StringBuilder ErrorAns = new StringBuilder();
		private StringBuilder executeAns = new StringBuilder();
		private Process p;
		#endregion PRIVATE VARIABLES

		#region PROPERTIES
		/// <summary>
		/// Gets the output.
		/// </summary>
		/// <value> The output. </value>
		public string Output => executeAns.ToString();
		/// <summary>
		/// Gets the error.
		/// </summary>
		/// <value> The error. </value>
		public string Error => ErrorAns.ToString();
		/// <summary>
		/// Gets the executed command.
		/// </summary>
		/// <value> The executed command. </value>
		public Process ExecutedCommand => p;
		#endregion PROPERTIES

		#region PUBLIC EVENTS
		/// <summary>
		/// Occurs when [log event].
		/// </summary>
		public event EventHandler<ExecuteEventArgs> ConsoleEvent;
		/// <summary>
		/// Called when [log event].
		/// </summary>
		/// <param name="sender"> The sender. </param>
		/// <param name="output"> The output. </param>
		/// <param name="error">  The error. </param>
		private void OnLogEvent( object sender , string output , string error ) => ConsoleEvent?.Invoke( sender , new ExecuteEventArgs( output , error ) );
		#endregion PUBLIC EVENTS

		#region METHODS
		/// <summary>
		/// Launch an command and wait for the execution to terminates
		/// </summary>
		/// <param name="exeName"> Name of the executable. </param>
		/// <param name="args">    The arguments. </param>
		/// <returns> console output string </returns>
		public string Run( string exeName , string args )
		{
			Launch( exeName , args );
			p.WaitForExit();
			return Output;
		}
		/// <summary>
		/// Launch a windows command. Redirect output and error streaming By default sends a 'Y' on the console input. This is required when the putty ask confirmation to register the id.
		/// </summary>
		/// <param name="exeName"> Name of the executable. </param>
		/// <param name="args">    The arguments. </param>
		public void Launch( string exeName , string args )
		{
			FileInfo fi = new FileInfo( exeName );

			executeAns.Clear();
			ErrorAns.Clear();

			p = new Process
			{
				StartInfo = new ProcessStartInfo( fi.FullName )
				{
					WorkingDirectory = fi.DirectoryName ,
					UseShellExecute = false ,
					RedirectStandardOutput = true ,
					RedirectStandardError = true ,
					CreateNoWindow = true ,
					Arguments = args ,
					RedirectStandardInput = true
				}
			};
			p.ErrorDataReceived += new DataReceivedEventHandler( ErrorDataReceived );
			p.OutputDataReceived += new DataReceivedEventHandler( OutputDataReceived );
			try
			{
				p.Start();
				p.StandardInput.WriteLine( "y" );
				p.BeginOutputReadLine();
				p.BeginErrorReadLine();
			}
			catch( Exception ex )
			{
				Logger.TraceException( ex , $"The execution '{exeName} {args}' threw an exception" , "collect the data in the log event and report on GitHub" );
			}
		}
		#endregion METHODS

		#region LOCAL METHODS
		/// <summary>
		/// Outputs the data received.
		/// </summary>
		/// <param name="sender"> The sender. </param>
		/// <param name="drea">   The <see cref="System.Diagnostics.DataReceivedEventArgs"/> instance containing the event data. </param>
		private void OutputDataReceived( object sender , DataReceivedEventArgs drea )
		{
			if( !string.IsNullOrEmpty( drea.Data ) )
			{
				if( drea.Data.ToUpper( CultureInfo.InvariantCulture ).Contains( " ERROR" ) || drea.Data.ToUpper( CultureInfo.InvariantCulture ).Contains( "ERROR " ) )
				{
					ErrorAns.Append( drea.Data + "\n" );
					OnLogEvent( sender , "" , drea.Data );
				}
				else
				{
					executeAns.Append( drea.Data + "\n" );
					OnLogEvent( sender , drea.Data , "" );
				}
			}
			else if( p != null && p.HasExited && p.ExitCode == 0 )
			{
				OnLogEvent( sender , "0" , "" );
				p.Close();
				p.Dispose();
				p = null;
			}
		}

		private void ErrorDataReceived( object sender , DataReceivedEventArgs drea )
		{
			if( !string.IsNullOrEmpty( drea.Data ) )
			{
				ErrorAns.Append( drea.Data + "\n" );
				OnLogEvent( sender , "" , drea.Data );
			}
			else if( p != null && p.HasExited && p.ExitCode != 0 )
			{
				OnLogEvent( sender , null , $"{p.ExitCode}" );
				p.Close();
				p.Dispose();
				p = null;
			}
		}

		#endregion LOCAL METHODS

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if( p != null )
			{
				p.Close();
				p.Dispose();
			}
			// This object will be cleaned up by the Dispose method. Therefore, you should call GC.SupressFinalize to take this object off the finalization queue and prevent
			// finalization code for this object from executing a second time.
			GC.SuppressFinalize( this );
		}

		#endregion IDisposable Members
	}

	/// <summary>
	/// Event of an output redirected
	/// </summary>
	public class ExecuteEventArgs : EventArgs
	{
		#region LOCAL VARIABLES
		private readonly string _Output;
		private readonly string _Error;
		#endregion LOCAL VARIABLES

		#region PUBLIC PROPERTIES
		/// <summary>
		/// Gets the output.
		/// </summary>
		/// <value> The output. </value>
		public string Output => _Output;
		/// <summary>
		/// Gets the error.
		/// </summary>
		/// <value> The error. </value>
		public string Error => _Error;
		#endregion PUBLIC PROPERTIES

		#region CONSTRUCTOR
		/// <summary>
		/// Initializes a new instance of the <see cref="ExecuteEventArgs"/> class.
		/// </summary>
		/// <param name="output"> The output. </param>
		/// <param name="error">  The error. </param>
		internal ExecuteEventArgs( string output , string error )
		{
			_Output = output;
			_Error = error;
		}
		#endregion CONSTRUCTOR
	}
}
