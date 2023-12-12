using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace xnet.Context.Profile
{
	/// <summary>
	///  Create an object that interface an IApplicationSettingWriter. The file will be located on the local drive instead of the network drive. A
	///  synchronization thread to and from the network will keep the files in sync This will greatly reduce the network traffic and improve speed of the
	///  networked application
	/// </summary>
	public sealed class LocalFileSettingProvider : IApplicationSettingsWriter
	{
		#region LOCAL VARIABLES

		private bool Flushing;

		/// <summary>
		///  What: monitor if a config has been modified
		///  Why: sync the modified file over the network after a specific timeout.
		/// </summary>
		private bool IsModified;

		/// <summary>
		///  What: static list of all the ini file we have referenced so far
		///  Why: To avoid to connect to the network as we already have the ini file locally.
		/// </summary>
		static internal SortedList<string , IApplicationSettingsWriter> ConfigFilesList = new SortedList<string , IApplicationSettingsWriter>();

		/// <summary>
		///  What: local path to store the working ini file
		///  Why: avoid numerous transfers over the network.
		/// </summary>
		private static string LocalPath = new DirectoryInfo( "." ).FullName;

		/// <summary> What: configuration file name </summary>
		private readonly string _ConfigFile;

		private readonly Thread SynThread;

		#endregion LOCAL VARIABLES

		#region CONSTRUCTOR

		/// <summary> Initializes a new instance of the <see cref="LocalFileSettingProvider" /> class. </summary>
		/// <param name="appSettings"> The application settings. </param>
		/// <exception cref="System.ArgumentNullException"> appSettings </exception>
		public LocalFileSettingProvider( IApplicationSettingsWriter appSettings )
		{
			if( appSettings == null )
				throw new ArgumentNullException( nameof( appSettings ) );
			FileInfo NetworkFI = new FileInfo( appSettings.ConfigFile );
			_ConfigFile = LocalPath + @"\" + NetworkFI.Name;

			SynThread = new Thread( new ParameterizedThreadStart( SyncIniFile ) );

			if( !ConfigFilesList.ContainsKey( ConfigFile ) )
			{
				FileInfo LocalFI = new FileInfo( ConfigFile );

				// If The source file and the local file are not the same then Sync the source to the local one.
				if( !NetworkFI.DirectoryName.Contains( LocalPath ) && !LocalFI.DirectoryName.Contains( NetworkFI.DirectoryName ) )
				{
					try
					{
						// Delete the local config file.
						if( LocalFI.Exists )
						{
							try
							{
								LocalFI.Attributes = FileAttributes.Normal;
								LocalFI.Delete();
							}
							catch( IOException )
							{
							}
							catch( NotSupportedException )
							{
							}
							catch( UnauthorizedAccessException )
							{
							}
						}

						// Copy the network file locally
						if( NetworkFI.Exists )
							File.Copy( NetworkFI.FullName , LocalFI.FullName , true );

						// Add the new instance
						ConfigFilesList.Add( ConfigFile , new PrivateProfileFile( LocalFI.DirectoryName , LocalFI.Name ) );

						// Activate the thread to synchronize
						SynThread.Name = ConfigFile;
						SynThread.Priority = ThreadPriority.Lowest;
						SynThread.Start( new KeyValuePair<FileInfo , FileInfo>( LocalFI , NetworkFI ) );
					}
					catch( DirectoryNotFoundException )
					{
						// Add the network file instead as we are unable to sync locally...
						ConfigFilesList.Add( ConfigFile , new PrivateProfileFile( NetworkFI.DirectoryName , NetworkFI.Name ) );
					}
					catch( FileNotFoundException )
					{
						// Add the network file instead as we are unable to sync locally...
						ConfigFilesList.Add( ConfigFile , new PrivateProfileFile( NetworkFI.DirectoryName , NetworkFI.Name ) );
					}
					catch( IOException )
					{
						// Add the network file instead as we are unable to sync locally...
						ConfigFilesList.Add( ConfigFile , new PrivateProfileFile( NetworkFI.DirectoryName , NetworkFI.Name ) );
					}
					catch( NotSupportedException )
					{
						// Add the network file instead as we are unable to sync locally...
						ConfigFilesList.Add( ConfigFile , new PrivateProfileFile( NetworkFI.DirectoryName , NetworkFI.Name ) );
					}
				}
				else
				{
					// Network and local are the same...
					ConfigFilesList.Add( ConfigFile , new PrivateProfileFile( NetworkFI.DirectoryName , NetworkFI.Name ) );
				}
			}
		}

		#endregion CONSTRUCTOR

		private void SyncIniFile( object fi )
		{
			FileInfo LocalFI = ((KeyValuePair<FileInfo , FileInfo>)fi).Key;
			FileInfo NetworkFI = ((KeyValuePair<FileInfo , FileInfo>)fi).Value;
			while( true )
			{
				try
				{
					if( LocalFI.LastWriteTimeUtc > NetworkFI.LastWriteTimeUtc || IsModified || Flushing )
					{
						lock( ConfigFilesList[ConfigFile] )
						{
							System.Diagnostics.Debug.WriteLine( "Copying " + LocalFI.Name + " To " + NetworkFI.FullName );
							try
							{
								File.Copy( LocalFI.FullName , NetworkFI.FullName , true );
							}
							catch( UnauthorizedAccessException )
							{
							}
							IsModified = false;
						}
					}
					else if( NetworkFI.LastWriteTimeUtc > LocalFI.LastWriteTimeUtc )
					{
						lock( ConfigFilesList[ConfigFile] )
						{
							try
							{
								File.Copy( NetworkFI.FullName , LocalFI.FullName , true );
							}
							catch( UnauthorizedAccessException )
							{
							}
							IsModified = false;
						}
					}
				}
				catch( UnauthorizedAccessException ex )
				{
					System.Diagnostics.Debug.WriteLine( ex.Message + "\n" + ex.StackTrace );
				}
				catch( DirectoryNotFoundException ex )
				{
					System.Diagnostics.Debug.WriteLine( ex.Message + "\n" + ex.StackTrace );
				}
				catch( FileNotFoundException ex )
				{
					System.Diagnostics.Debug.WriteLine( ex.Message + "\n" + ex.StackTrace );
				}
				catch( IOException ex )
				{
					System.Diagnostics.Debug.WriteLine( ex.Message + "\n" + ex.StackTrace );
				}
				catch( NotSupportedException ex )
				{
					System.Diagnostics.Debug.WriteLine( ex.Message + "\n" + ex.StackTrace );
				}

				if( !IsModified && Flushing )
					break;

				Thread.Sleep( 1000 );
				LocalFI.Refresh();
				NetworkFI.Refresh();
			}
		}

		#region IApplicationSettingsWriter Members

		/// <summary>
		///  What: propagate the modification on the ini file to the original network path if any
		///  Why: allow to copy the ini file locally and when we have worked save the changes on the network to improve performances
		/// </summary>
		public void Flush()
		{
			Flushing = true;

			while( SynThread.IsAlive )
				Thread.Sleep( 100 );
		}

		/// <summary>
		///  What: Reset the file container object
		///  Why: when we change the configuration directory, we have to flush the configuration files then empty our file list reference to start anew.
		/// </summary>
		public void Reset()
		{
			Flush();
			ConfigFilesList.Remove( ConfigFile );
		}

		/// <summary>
		///  What: stores some configuration data in the ini file
		///  Why: allow to save configuration data.
		/// </summary>
		/// <param name="section"> </param>
		/// <param name="key"> </param>
		/// <param name="value"> </param>
		/// <returns> </returns>
		public bool SetData( string section , string key , string value )
		{
			lock( ConfigFilesList[ConfigFile] )
			{
				IsModified = ConfigFilesList[ConfigFile].SetData( section , key , value );
			}
			return IsModified;
		}

		/// <summary> Sets the data. </summary>
		/// <param name="section"> The section. </param>
		/// <param name="key"> The key. </param>
		/// <param name="value"> The value. </param>
		/// <returns> </returns>
		public bool SetData( string section , string key , int value )
		{
			lock( ConfigFilesList[ConfigFile] )
			{
				IsModified = ConfigFilesList[ConfigFile].SetData( section , key , value );
			}
			return IsModified;
		}

		/// <summary> Sets the data. </summary>
		/// <param name="section"> The section. </param>
		/// <param name="key"> The key. </param>
		/// <param name="value"> if set to <c> true </c> [value]. </param>
		/// <returns> </returns>
		public bool SetData( string section , string key , bool value )
		{
			lock( ConfigFilesList[ConfigFile] )
			{
				IsModified = ConfigFilesList[ConfigFile].SetData( section , key , value );
			}
			return IsModified;
		}

		/// <summary> Sets the data. </summary>
		/// <param name="section"> The section. </param>
		/// <param name="key"> The key. </param>
		/// <param name="value"> The value. </param>
		/// <returns> </returns>
		public bool SetData( string section , string key , decimal value )
		{
			lock( ConfigFilesList[ConfigFile] )
			{
				IsModified = ConfigFilesList[ConfigFile].SetData( section , key , value );
			}
			return IsModified;
		}

		#endregion IApplicationSettingsWriter Members

		#region IApplicationSettingsReader Members

		/// <summary> The configuration file </summary>
		public string ConfigFile => _ConfigFile;

		/// <summary> Gets or sets the name of the path. </summary>
		/// <value> The name of the path. </value>
		public string PathName
		{
			get => LocalPath;
			set => throw new NotSupportedException();
		}

		/// <summary> Gets the data. </summary>
		/// <param name="section"> The section. </param>
		/// <param name="key"> The key. </param>
		/// <returns> </returns>
		public string GetData( string section , string key ) => ConfigFilesList[ConfigFile].GetData( section , key );

		/// <summary> Gets the data value. </summary>
		/// <param name="section"> The section. </param>
		/// <param name="key"> The key. </param>
		/// <returns> </returns>
		public int GetDataValue( string section , string key ) => ConfigFilesList[ConfigFile].GetDataValue( section , key );

		/// <summary> Gets the double data. </summary>
		/// <param name="section"> The section. </param>
		/// <param name="key"> The key. </param>
		/// <returns> </returns>
		public decimal GetDecimalData( string section , string key ) => ConfigFilesList[ConfigFile].GetDecimalData( section , key );

		/// <summary> Gets the data. </summary>
		/// <param name="section"> The section. </param>
		/// <param name="key"> The key. </param>
		/// <param name="defaultValue"> The default value. </param>
		/// <returns> </returns>
		public int GetData( string section , string key , int defaultValue ) => ConfigFilesList[ConfigFile].GetData( section , key , defaultValue );

		/// <summary> Gets the data. </summary>
		/// <param name="section"> The section. </param>
		/// <param name="key"> The key. </param>
		/// <param name="defaultValue"> The default value. </param>
		/// <returns> </returns>
		public decimal GetData( string section , string key , decimal defaultValue ) => ConfigFilesList[ConfigFile].GetData( section , key , defaultValue );

		/// <summary> Gets the data. </summary>
		/// <param name="section"> The section. </param>
		/// <param name="key"> The key. </param>
		/// <param name="defaultValue"> if set to <c> true </c> [default value]. </param>
		/// <returns> </returns>
		public bool GetData( string section , string key , bool defaultValue ) => ConfigFilesList[ConfigFile].GetData( section , key , defaultValue );

		/// <summary> Gets the data. </summary>
		/// <param name="section"> The section. </param>
		/// <param name="key"> The key. </param>
		/// <param name="defaultValue"> The default value. </param>
		/// <returns> </returns>
		public string GetData( string section , string key , string defaultValue ) => ConfigFilesList[ConfigFile].GetData( section , key , defaultValue );

		#endregion IApplicationSettingsReader Members

		#region IDisposable Members

		/// <summary> Releases unmanaged and - optionally - managed resources. </summary>
		public void Dispose()
		{
			ConfigFilesList[ConfigFile].Dispose();
			// This object will be cleaned up by the Dispose method. Therefore, you should call GC.SupressFinalize to take this object off the finalization
			// queue and prevent finalization code for this object from executing a second time.
			GC.SuppressFinalize( this );
		}

		#endregion IDisposable Members
	}
}
