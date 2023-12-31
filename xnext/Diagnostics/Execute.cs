using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace xnext.Diagnostics
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
		public string Output => executeAns.ToString();
		public string Error => ErrorAns.ToString();
		public Process ExecutedCommand => p;
		#endregion PROPERTIES

		#region PUBLIC EVENTS
		public event EventHandler<ExecuteEventArgs> ConsoleEvent;
		public event EventHandler Exit;
		private void OnLogEvent( object sender , string output , string error ) => ConsoleEvent?.Invoke( sender , new ExecuteEventArgs( output , error ) );
		#endregion PUBLIC EVENTS

		#region METHODS
		/// <summary>
		/// Launch an command and wait for the execution to terminates
		/// </summary>
		public string Run( string exeName , string args )
		{
			Launch( exeName , args );
			p.WaitForExit();
			return Output;
		}
		/// <summary>
		/// Launch a windows command. Redirect output and error streaming By default sends a 'Y' on the console input. This is required when the putty ask confirmation to register the id.
		/// </summary>
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
					RedirectStandardInput = true ,
					StandardOutputEncoding = Encoding.UTF8
				}
			};
			p.ErrorDataReceived += new DataReceivedEventHandler( ErrorDataReceived );
			if( Exit != null )
				p.Exited += new EventHandler( Exit );
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
			else if( sender is Process process )
			{
				try
				{
				if( process.HasExited )
				{
					ConsoleEvent?.Invoke( sender , new ExecuteEventArgs( process.ExitCode ) );
					process?.Close();
					process?.Dispose();
				}
				}
				catch( Exception ) { }
				finally { p = null; }
			}
		}

		private void ErrorDataReceived( object sender , DataReceivedEventArgs drea )
		{
			if( !string.IsNullOrEmpty( drea.Data ) )
			{
				ErrorAns.Append( drea.Data + "\n" );
				OnLogEvent( sender , "" , drea.Data );
			}
			else if( sender is Process process )
			{
				try
				{
					if( process.HasExited )
					{
						ConsoleEvent?.Invoke( sender , new ExecuteEventArgs( process.ExitCode ) );
						process?.Close();
						process?.Dispose();
					}
				}
				catch( Exception ) { }
				finally { p = null; }
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
				try { p.CancelErrorRead(); } catch( Exception ) { }
				try { p.CancelOutputRead(); } catch( Exception ) { }
				try { p.Kill(); } catch( Exception ) { }
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
		public string Output => _Output;
		/// <summary>
		/// Gets the error.
		/// </summary>
		public string Error => _Error;
		public int ExitCode { get; private set; } = int.MinValue;
		#endregion PUBLIC PROPERTIES

		#region CONSTRUCTOR
		/// <summary>
		/// Initializes a new instance of the <see cref="ExecuteEventArgs"/> class.
		/// </summary>
		internal ExecuteEventArgs( string output , string error )
		{
			_Output = output;
			_Error = error;
		}
		internal ExecuteEventArgs( int exitCode )
		{
			ExitCode = exitCode; ;
		}
		#endregion CONSTRUCTOR
	}
}
