using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace xnet.Context.Profile
{
	/// <summary> Manage the configuration file and the working directory </summary>

	public sealed class Manager : IDisposable
	{
		#region SINGLETON
		private readonly string CurrentPath;

		/// <summary> Prevents a default instance of the <see cref="Manager" /> class from being created. </summary>
		private Manager()
		{
			CurrentPath = new DirectoryInfo( "." ).FullName;
			IsNetworked = false;
		}

		/// <summary> The instance </summary>
		static public Manager Instance { get; } = new Manager();

		#endregion SINGLETON

		/// <summary>
		///  Gets or sets the master. Reading set If the value does not exist in the child, we check if it exist in the master, if it does, we return the one of
		///  the master Otherwise we add in in the child with the default value.
		/// </summary>
		/// <value> The master. </value>
		public IApplicationSettingsReader Master { get; set; }

		#region	WORKING DIRECTORY
		/// <summary> Gets or sets the identifier. </summary>
		/// <value> The identifier. </value>
		public string Identifier { get; set; } = Process.GetCurrentProcess().ProcessName;

		/// <summary>
		///  See if we have the working directory as out application directory All files are seen as part of a network and a local copy will be updated and then
		///  later on updated asynchronously on the network.
		/// </summary>
		public bool IsNetworked { get; set; }

		/// <summary>
		///  What: the working directory of the application
		///  Why: the working directory can be different from the execution directory
		/// </summary>
		private DirectoryInfo _WorkingDirectory;

		/// <summary> Gets or sets the working directory. </summary>
		/// <value> The working directory. </value>
		/// <exception cref="ArgumentNullException"> value </exception>
		public DirectoryInfo WorkingDirectory
		{
			get => _WorkingDirectory;
			set
			{
				if( value == null )
					throw new ArgumentNullException( nameof( value ) );
				_WorkingDirectory = value;
				ApplicationSettingsDirectory = new DirectoryInfo( value.FullName + @"\Configuration" );
				if( !ApplicationSettingsDirectory.Exists )
				{
					try
					{
						ApplicationSettingsDirectory = new DirectoryInfo( ApplicationSettingsDirectory.FullName );
						if( !ApplicationSettingsDirectory.Exists )
							ApplicationSettingsDirectory = Directory.CreateDirectory( ApplicationSettingsDirectory.FullName );
					}
					catch( Exception ex )
					{
						System.Diagnostics.Debug.WriteLine( "Creating Directory: " + ApplicationSettingsDirectory.FullName + "\n" + ex.ToString() );
					}
				}
				if( ChangeWorkingDirectoryEvent != null && ApplicationSettingsDirectory.Exists )
					ChangeWorkingDirectoryEvent( null , new SetDirectoryEventArgs { DirectoryType = DirectoryType.WorkingDirectory , NewDirectory = value } );
			}
		}

		/// <summary> Occurs when [directory changes]. </summary>
		public event EventHandler<SetDirectoryEventArgs> ChangeWorkingDirectoryEvent;

		#endregion

		#region CONFIGURATION DIRECTORY

		/// <summary>
		///  What: Directory of the Configuration file location
		///  Why: The configuration directory is where the configuration and user config files are stored
		/// </summary>
		private DirectoryInfo _ApplicationSettingsDirectory;

		/// <summary> Gets or sets the application settings directory. </summary>
		/// <value> The application settings directory. </value>
		public DirectoryInfo ApplicationSettingsDirectory
		{
			get => _ApplicationSettingsDirectory;
			set
			{
				Reset();
				_ApplicationSettingsDirectory = value;
				ChangeConfigDirectoryEvent?.Invoke( null , new SetDirectoryEventArgs { DirectoryType = DirectoryType.UserConfigDirectory , NewDirectory = value } );
			}
		}

		#region WINDOWS EVENTS

		/// <summary> Occurs when [the config directory changes]. </summary>
		public event EventHandler<SetDirectoryEventArgs> ChangeConfigDirectoryEvent;

		#endregion

		#endregion

		#region APPLICATION SETTINGS
		private SortedList<string , IApplicationSettingsWriter> SettingList = new SortedList<string , IApplicationSettingsWriter>();

		/// <summary>
		///  What: copy the local configuration back to the configuration directory
		///  Why: for performance improvement the configuration are copied locally
		/// </summary>
		public void Reset()
		{
			foreach( KeyValuePair<string , IApplicationSettingsWriter> kvp in SettingList )
				kvp.Value.Reset();
			SettingList.Clear();
		}

		#endregion

		#region PUBLIC PROPERTIES

		/// <summary> Gets or sets the application setting. </summary>
		/// <value> The application setting. </value>
		/// <exception cref="System.ArgumentNullException"> value </exception>
		public IApplicationSettingsWriter AppSetting
		{
			get
			{
				if( _WorkingDirectory != null )
				{
					if( !SettingList.ContainsKey( ApplicationSettingsDirectory.FullName + "\\" + Identifier + ".ini" ) )
						AppSetting = new PrivateProfileFile( ApplicationSettingsDirectory.FullName , Identifier + ".ini" );

					return SettingList[ApplicationSettingsDirectory.FullName + "\\" + Identifier + ".ini"];
				}
				return new PrivateProfileFile( CurrentPath , Identifier + ".ini" );
			}
			set
			{
				if( value == null )
					throw new ArgumentNullException( nameof( value ) );
				if( IsNetworked )
					SettingList.Add( value.ConfigFile , new LocalFileSettingProvider( value ) );
				else
					SettingList.Add( value.ConfigFile , value );
			}
		}

		/// <summary> Gets the application read setting. </summary>
		/// <value> The application read setting. </value>
		public IApplicationSettingsReader AppReadSetting
		{
			get
			{
				if( _WorkingDirectory != null )
				{
					if( !SettingList.ContainsKey( ApplicationSettingsDirectory.FullName + "\\" + Identifier + ".ini" ) )
						AppSetting = new PrivateProfileFile( ApplicationSettingsDirectory.FullName , Identifier + ".ini" );

					return SettingList[ApplicationSettingsDirectory.FullName + "\\" + Identifier + ".ini"];
				}
				return new PrivateProfileFile( CurrentPath , Identifier + ".ini" );
			}
		}
		#endregion

		/// <summary>
		///  What: Access for writing a specific configuration file
		///  Why: an application can share a config file with another application
		/// </summary>
		/// <param name="fileName"> </param>
		/// <returns> </returns>
		public IApplicationSettingsWriter GetProfileWriter( string fileName )
		{
			FileInfo fi = new FileInfo( fileName );

			if( SettingList.ContainsKey( ApplicationSettingsDirectory.FullName + "\\" + fi.Name ) )
				return SettingList[ApplicationSettingsDirectory.FullName + "\\" + fi.Name];
			else if( SettingList.ContainsKey( fi.FullName ) )
				return SettingList[fi.FullName];
			else if( SettingList.ContainsKey( WorkingDirectory.FullName + "\\" + fi.Name ) )
				return SettingList[WorkingDirectory.FullName + "\\" + fi.Name];
			else if( new FileInfo( ApplicationSettingsDirectory.FullName + "\\" + fi.Name ).Exists )
			{
				AppSetting = new PrivateProfileFile( ApplicationSettingsDirectory.FullName , fi.Name );
				return SettingList[ApplicationSettingsDirectory.FullName + "\\" + fi.Name];
			}
			else if( fi.Exists )
			{
				AppSetting = new PrivateProfileFile( fi.DirectoryName , fi.Name );
				return SettingList[fi.FullName];
			}
			else
			{
				AppSetting = new PrivateProfileFile( WorkingDirectory.FullName , fi.Name );
				return SettingList[WorkingDirectory.FullName + "\\" + fi.Name];
			}
		}

		/// <summary>
		///  What: Access for writing a specific configuration file
		///  Why: an application can share a config file with another application
		/// </summary>
		/// <param name="fileName"> </param>
		/// <returns> </returns>
		public IApplicationSettingsWriter GetNetworkProfileWriter( string fileName )
		{
			FileInfo fi = new FileInfo( fileName );

			if( SettingList.ContainsKey( ApplicationSettingsDirectory.FullName + "\\" + fi.Name ) )
				return SettingList[ApplicationSettingsDirectory.FullName + "\\" + fi.Name];
			else if( SettingList.ContainsKey( fi.FullName ) )
				return SettingList[fi.FullName];
			else if( SettingList.ContainsKey( WorkingDirectory.FullName + "\\" + fi.Name ) )
				return SettingList[WorkingDirectory.FullName + "\\" + fi.Name];
			else if( fi.Exists )
			{
				SettingList.Add( fi.FullName , new PrivateProfileFile( fi.DirectoryName , fi.Name ) );
				return SettingList[fi.FullName];
			}
			else if( new FileInfo( ApplicationSettingsDirectory.FullName + "\\" + fi.Name ).Exists )
			{
				SettingList.Add( fileName , new PrivateProfileFile( ApplicationSettingsDirectory.FullName , fi.Name ) );
				return SettingList[ApplicationSettingsDirectory.FullName + "\\" + fi.Name];
			}
			else
			{
				SettingList.Add( WorkingDirectory.FullName + "\\" + fi.Name , new PrivateProfileFile( WorkingDirectory.FullName , fi.Name ) );
				return SettingList[WorkingDirectory.FullName + "\\" + fi.Name];
			}
		}

		/// <summary>
		///  What: Access for reading a specific configuration file
		///  Why: an application can share a config file with another application
		/// </summary>
		/// <param name="fileName"> </param>
		/// <returns> </returns>
		public IApplicationSettingsReader GetProfileReader( string fileName )
		{
			FileInfo fi = new FileInfo( fileName );

			if( SettingList.ContainsKey( ApplicationSettingsDirectory.FullName + "\\" + fi.Name ) )
				return SettingList[ApplicationSettingsDirectory.FullName + "\\" + fi.Name];
			else if( SettingList.ContainsKey( fi.FullName ) )
				return SettingList[fi.FullName];
			else if( SettingList.ContainsKey( WorkingDirectory.FullName + "\\" + fi.Name ) )
				return SettingList[WorkingDirectory.FullName + "\\" + fi.Name];
			else if( new FileInfo( ApplicationSettingsDirectory.FullName + "\\" + fi.Name ).Exists )
			{
				AppSetting = new PrivateProfileFile( ApplicationSettingsDirectory.FullName , fi.Name );
				return SettingList[ApplicationSettingsDirectory.FullName + "\\" + fi.Name];
			}
			else if( fi.Exists )
			{
				AppSetting = new PrivateProfileFile( fi.DirectoryName , fi.Name );
				return SettingList[fi.FullName];
			}
			else
			{
				AppSetting = new PrivateProfileFile( WorkingDirectory.FullName , fi.Name );
				return SettingList[WorkingDirectory.FullName + "\\" + fi.Name];
			}
		}

		/// <summary>
		///  What: Access for writing a specific configuration file
		///  Why: an application can share a config file with another application
		/// </summary>
		/// <param name="fileName"> </param>
		/// <returns> </returns>
		public IApplicationSettingsReader GetNetworkProfileReader( string fileName )
		{
			FileInfo fi = new FileInfo( fileName );

			if( SettingList.ContainsKey( ApplicationSettingsDirectory.FullName + "\\" + fi.Name ) )
				return SettingList[ApplicationSettingsDirectory.FullName + "\\" + fi.Name];
			else if( SettingList.ContainsKey( fi.FullName ) )
				return SettingList[fi.FullName];
			else if( SettingList.ContainsKey( WorkingDirectory.FullName + "\\" + fi.Name ) )
				return SettingList[WorkingDirectory.FullName + "\\" + fi.Name];
			else if( fi.Exists )
			{
				SettingList.Add( fi.FullName , new PrivateProfileFile( fi.DirectoryName , fi.Name ) );
				return SettingList[fi.FullName];
			}
			else if( new FileInfo( ApplicationSettingsDirectory.FullName + "\\" + fi.Name ).Exists )
			{
				SettingList.Add( fileName , new PrivateProfileFile( ApplicationSettingsDirectory.FullName , fi.Name ) );
				return SettingList[ApplicationSettingsDirectory.FullName + "\\" + fi.Name];
			}
			else
			{
				SettingList.Add( WorkingDirectory.FullName + "\\" + fi.Name , new PrivateProfileFile( WorkingDirectory.FullName , fi.Name ) );
				return SettingList[WorkingDirectory.FullName + "\\" + fi.Name];
			}
		}

		#region IDisposable Members

		/// <summary> Releases unmanaged and - optionally - managed resources. </summary>
		public void Dispose()
		{
			Reset();

			foreach( KeyValuePair<string , IApplicationSettingsWriter> kvp in SettingList )
				kvp.Value.Dispose();

			// This object will be cleaned up by the Dispose method. Therefore, you should call GC.SupressFinalize to take this object off the finalization
			// queue and prevent finalization code for this object from executing a second time.
			GC.SuppressFinalize( this );
		}

		#endregion
	}

	#region TYPES

	public enum DirectoryType
	{
		/// <summary> The working directory </summary>
		WorkingDirectory,

		/// <summary> The user configuration directory </summary>
		UserConfigDirectory
	};

	/// <summary> </summary>

	public sealed class SetDirectoryEventArgs : EventArgs
	{
		#region PUBLIC PROPERTIES

		/// <summary> Gets the new directory. </summary>
		/// <value> The new directory. </value>
		public DirectoryInfo NewDirectory { get; internal set; }

		/// <summary> Gets the type of the directory. </summary>
		/// <value> The type of the directory. </value>
		public DirectoryType DirectoryType { get; internal set; }

		#endregion
	}

	#endregion
}
