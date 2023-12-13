using System;
using System.IO;
using System.Runtime.Caching;

using xnet.Context.Profile;
using xnet.Diagnostics;

namespace xnet.Context
{
	/// <summary>Class CltWinEnv.</summary>
	public class CltWinEnv
	{
		private string MainIdentifier = string.Empty;

		#region ACCESSORS
		/// <summary>Gets the information.</summary>
		/// <value>The information.</value>
		public static CltWinEnv Info { get; } = new CltWinEnv();

		/// <summary>Gets the <see cref="IApplicationSettingsWriter"/> with the specified identifier.</summary>
		/// <param name="identifier">The identifier.</param>
		/// <returns>IApplicationSettingsWriter.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance" , "CA1822:Mark members as static" , Justification = "this cannot be static lah!" )]
		public IApplicationSettingsWriter this[string identifier] => Manager.Instance.GetProfileWriter( Path.Combine( ConfigDir , $"{identifier}.ini" ) );

		/// <summary>Gets the working dir.</summary>
		/// <value>The working dir.</value>
		public static string WorkingDir => Manager.Instance.WorkingDirectory.FullName;
		/// <summary>Gets the configuration dir.</summary>
		/// <value>The configuration dir.</value>
		public static string ConfigDir => Path.Combine( Manager.Instance.WorkingDirectory.FullName , "Configuration" );
		#endregion ACCESSORS

		#region PUBLIC METHODS
		/// <summary>
		/// What: Setups the directory environment and the default trace level.
		/// Why: Creates the directory in the roaming directories and the local directory for the log and trace.
		/// </summary>
		/// <param name="identifier">The identifier.</param>
		public static void SetupEnvironment( string identifier )
		{
			Manager.Instance.Identifier = identifier;

			if( Manager.Instance.WorkingDirectory == null )
			{
				Info.MainIdentifier = identifier;

				Manager.Instance.WorkingDirectory = Directory.CreateDirectory( Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ) , identifier) );
				Manager.Instance.IsNetworked = false;
				Manager.Instance.AppSetting.SetData( "Configuration" , "Data Path" , Directory.CreateDirectory( Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ) , identifier , "Data" ) ).FullName );
				Manager.Instance.AppSetting.SetData( "Configuration" , "Log Path" , Directory.CreateDirectory( Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ) , identifier , "Log" ) ).FullName );
				
				Manager.Instance.Master = Manager.Instance.AppReadSetting;
				
				CacheItemPolicy Policy = new CacheItemPolicy()
				{
					AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration ,
					Priority = CacheItemPriority.NotRemovable
				};
				MemoryCache.Default.Add( "Configuration" , Manager.Instance.ApplicationSettingsDirectory.FullName , Policy );
				MemoryCache.Default.Add( "DataPath" , Manager.Instance.AppSetting.GetData( "Configuration" , "Data Path" ) , Policy );
				MemoryCache.Default.Add( "LogPath" , Manager.Instance.AppSetting.GetData( "Configuration" , "Log Path" ) , Policy );

				LogTrace.Begin();

#if DEBUG
				Logger.Instance.LogFile.TraceLevel = (int)Enum.Parse( typeof( System.Diagnostics.TraceEventType ) , Manager.Instance.AppReadSetting.GetData( "Log" , "Trace" , "Information" ) );
#else
				Logger.Instance.LogFile.TraceLevel = (int)Enum.Parse( typeof( System.Diagnostics.TraceEventType ) , Manager.Instance.AppReadSetting.GetData( "Log" , "Trace" , "Warning" ) );
#endif
			}
			else
			{
				LogTrace.Begin();
				FileInfo fi = new FileInfo( $@"{ConfigDir}\{identifier}.ini" );
				if( !fi.Exists )
					fi.CreateText().Close();
			}

			LogTrace.End( $"Environment for {identifier} configured" );
		}
		#endregion PUBLIC METHODS
	}
}
