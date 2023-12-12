using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Timers;

namespace xnet.Files
{
	public enum MonitorChangeType
	{
		/// <summary>The created</summary>
		Created,

		/// <summary>The modified</summary>
		Modified
	};

	public enum NotificationType
	{
		/// <summary>The none</summary>
		None = 0x00,

		/// <summary>The last write</summary>
		LastWrite = 0x01
	};

	/// <summary></summary>

	public sealed class DirectoryMonitor : IDisposable
	{
		#region LOCAL VARIABLE

		/// <summary>What: specify if you want tot include subdirectories or keep flat</summary>
		private bool _IncludeSubdirectories;

		/// <summary>What: file filter</summary>
		private string _Filter = "*.*";

		/// <summary>What: pathname we are scanning.</summary>
		private DirectoryInfo _Path;

		/// <summary>What: Time to scan the directory for a change</summary>
		// private System.Windows.Forms.Timer timer1;
		private System.Timers.Timer timer1;

		/// <summary>Reference of the directory content used to compare to determine any changes</summary>
		private SortedList<string , long> RefList;

		#endregion LOCAL VARIABLE

		#region PUBLIC PROPERTIES
		public FileInfo[] Files => _Path.GetFiles( Filter , _IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly );
		/// <summary>
		///  What:Gets or sets the filter - *.ext
		///  Why: reduce the amount of file monitored
		/// </summary>
		/// <value>The filter.</value>
		[Category( "Data" ), Description( "Gets or sets the filter string used to determine what files are monitored in a directory." ), Browsable( true )]
		public string Filter
		{
			get => _Filter;
			set
			{
				if( _Filter != value )
				{
					_Filter = value;
					RefList = null;
				}
			}
		}

		/// <summary>Gets or sets the path of the directory to monitor.</summary>
		/// <value>The path.</value>
		[Category( "Data" ), Description( "Gets or sets the path of the directory to watch." ), Browsable( true )]
		public string Path
		{
			get
			{
				if( _Path != null )
					return _Path.FullName;
				return "";
			}
			set
			{
				if( Path != value )
				{
					_Path = new DirectoryInfo( value );
					if( !_Path.Exists )
						_Path = Directory.CreateDirectory( value );
					RefList = ScanTarget();
				}
			}
		}

		/// <summary>Gets or sets a value indicating whether [include subdirectories].</summary>
		/// <value><c>true</c> if [include subdirectories]; otherwise, <c>false</c>.</value>
		[Category( "Data" ), Description( "Gets or sets a value indicating whether subdirectories within the specified path should be monitored." ), Browsable( true )]
		public bool IncludeSubdirectories
		{
			get => _IncludeSubdirectories;
			set
			{
				if( IncludeSubdirectories != value )
				{
					_IncludeSubdirectories = value;
					RefList = ScanTarget();
				}
			}
		}

		/// <summary>Gets or sets a value indicating whether [enable raising events].</summary>
		/// <value><c>true</c> if [enable raising events]; otherwise, <c>false</c>.</value>
		[Category( "Data" ), Description( "Gets or sets a value indicating whether the component is enabled." ), Browsable( false )]
		public bool EnableRaisingEvents
		{
			get => timer1.Enabled;
			set
			{
				timer1.Enabled = value;
				if( value )
					timer1.Start();
				else
					timer1.Stop();
			}
		}

		/// <summary>What: Filter the event to wake the caller up</summary>
		[Category( "Data" ), Description( "Gets or sets the type of changes to watch for." ), Browsable( true )]
		public NotificationType NotifyFilter { get; set; }

		/// <summary>Gets or sets the interval.</summary>
		/// <value>The interval.</value>
		public double Interval
		{
			get => timer1.Interval;
			set => timer1.Interval = value;
		}

		#endregion PUBLIC PROPERTIES

		#region CONSTRUCTOR

		/// <summary>Initializes a new instance of the <see cref="DirectoryMonitor"/> class.</summary>
		public DirectoryMonitor()
		{
			// timer1 = new System.Windows.Forms.Timer();
			timer1 = new Timer( 2000.0 )
			{
				AutoReset = true ,
				Interval = 500 ,
				Enabled = true
			};
			// .Tick += OnScanDirectory;
			timer1.Elapsed += timer1_Elapsed;
		}

		/// <summary>Handles the Elapsed event of the timer1 control.</summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
		private void timer1_Elapsed( object sender , ElapsedEventArgs e )
		{
			timer1.Stop();

			try
			{
				if( RefList == null )
					RefList = ScanTarget();
				else
				{
					foreach( KeyValuePair<string , long> kvp in ScanTarget() )
					{
						if( RefList.ContainsKey( kvp.Key ) )
						{
							if( kvp.Value != RefList[kvp.Key] && FileMonitorEvent( MonitorChangeType.Modified , NotificationType.LastWrite , _Path.FullName + @"\" + kvp.Key , kvp.Value ) )
								RefList[kvp.Key] = kvp.Value;
						}
						else if( FileMonitorEvent( MonitorChangeType.Created , NotificationType.LastWrite , _Path.FullName + @"\" + kvp.Key , kvp.Value ) )
							RefList.Add( kvp.Key , kvp.Value );
					}
				}
			}
			catch( Exception )
			{
			}
			finally
			{
				if( timer1 != null )
					timer1.Start();
			}
		}

		#endregion CONSTRUCTOR

		#region WINDOWS EVENTS

		/// <summary>What: scan a directory target</summary>
		/// <returns></returns>
		private SortedList<string , long> ScanTarget()
		{
			SortedList<string , long> DirList = new SortedList<string , long>();
			if( _Path != null )
				try
				{
					foreach( FileSystemInfo fi in _Path.EnumerateFiles( Filter , _IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly ) )
						DirList.Add( fi.FullName.Replace( _Path.FullName + "\\" , "" ) , fi.LastWriteTime.ToFileTime() );
				}
				catch( Exception )
				{
				}
			return DirList;
		}

		#endregion WINDOWS EVENTS

		private bool FileMonitorEvent( MonitorChangeType changeType , NotificationType notifier , string fullFilename , long fileTime )
		{
			MonitorEventArgs MonirtorEvent = new MonitorEventArgs( changeType , notifier , fullFilename , fileTime );
			if( OnFileMonitorEvent != null )
			{
				OnFileMonitorEvent( this , MonirtorEvent );
				return MonirtorEvent.Success;
			}
			return true;
		}

		#region PUBLIC EVENT

		/// <summary>Occurs when [on file monitor].</summary>
		[Description( "Calls the function when an event is raised" ), Category( "Async" )]
		public event EventHandler<MonitorEventArgs> OnFileMonitorEvent;

		#endregion PUBLIC EVENT

		#region IDisposable Members

		/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		public void Dispose()
		{
			if( timer1 != null )
			{
				this.timer1.Stop();
				this.timer1.Enabled = false;
				timer1.Dispose();
				timer1 = null;
			}
			System.Threading.Thread.Sleep( 100 );
			GC.SuppressFinalize( this );
		}

		#endregion IDisposable Members
	}

	public sealed class MonitorEventArgs : EventArgs
	{
		#region PUBLIC VARIABLES

		/// <summary>Gets the last write time.</summary>
		/// <value>The last write time.</value>
		public DateTime LastWriteTime { get; }

		/// <summary>Gets the name of the file.</summary>
		/// <value>The name of the file.</value>
		public string FileName { get; }

		/// <summary>Gets the type of the change.</summary>
		/// <value>The type of the change.</value>
		public MonitorChangeType ChangeType { get; }

		/// <summary>Gets the event filter.</summary>
		/// <value>The event filter.</value>
		public NotificationType EventFilter { get; }

		/// <summary>
		///  What: inform the called process the file is currently been accessed and processed
		///  Why: if not accessible, the monitor will NOT record the file as new and on teh next scan the file will be seen as new. This may happen due to
		///  network latency when the file takes several seconds before being totally flushed on the hard disk.
		/// </summary>
		public bool Success { get; set; }

		#endregion PUBLIC VARIABLES

		#region CONSTRUCTOR

		/// <summary>What: Initializes a new instance of the FileMonitorEventArgs class</summary>
		/// <param name="changeType">One of the MonitorChangeTypes values which represents the kind of change detected in the file system.</param>
		/// <param name="eventFilter">The event filter.</param>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="fileTime">The file time.</param>
		internal MonitorEventArgs( MonitorChangeType changeType , NotificationType eventFilter , string fileName , long fileTime )
		{
			ChangeType = changeType;
			FileName = fileName;
			EventFilter = eventFilter;
			LastWriteTime = DateTime.FromFileTime( fileTime );
		}

		#endregion CONSTRUCTOR
	}
}
