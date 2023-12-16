using System;
using System.IO;
using System.Runtime.Caching;

using xnext.Diagnostics;

namespace xnext.Context
{
	/// <summary>Class CltWinEnv.</summary>
	public class CltWinEnv
	{
		public enum SettingType {  Local , Roaming , UserDefined };

		#region LOCAL VARIABLES
		private string _mainIdentifier = string.Empty;
		private FileInfo _appSetting = new FileInfo( "Config.ini" );
		private FileInfo _userSetting = new FileInfo( "Config.ini" );
		#endregion LOCAL VARIABLES

		#region ACCESSORS
		/// <summary>Gets the information.</summary>
		/// <value>The information.</value>
		public static CltWinEnv Info { get; } = new CltWinEnv();

		public static ISettingsReader AppReadSetting => new ProfileFile( Info._appSetting );
		public static ISettingsWriter AppSetting =>new ProfileFile( Info._appSetting );
		public static IUserSettingsReader UserReadSetting => new UserProfile( Info._userSetting );
		public static IUserSettingsWriter UserSetting => new UserProfile( Info._userSetting );
		
		public ISettingsWriter this[string fname] => new ProfileFile(new FileInfo( fname == null ? $@".\{_mainIdentifier}.ini" : fname ) );
		#endregion ACCESSORS

		#region PUBLIC METHODS
		/// <summary>
		/// What: Setups the directory environment and the default trace level.
		/// Why: Creates the directory in the roaming directories and the local directory for the log and trace.
		/// </summary>
		/// <param name="identifier">The identifier.</param>
		public static void SetupEnvironment( string identifier )
		{
			if( string.IsNullOrEmpty( Info._mainIdentifier  ) )
				lock( Info )
				{
					Info._mainIdentifier = identifier;

					Info._appSetting = new FileInfo( Path.Combine( Directory.CreateDirectory( Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ) , identifier ) ).FullName , "Config.ini" ) );
					Info._userSetting = new FileInfo( Path.Combine( Directory.CreateDirectory( Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ) , identifier ) ).FullName , "Config.ini" ) );

				UserSetting.SetData( "Configuration" , "Data Path" , Directory.CreateDirectory( Path.Combine( Info._appSetting.DirectoryName , "Data" ) ).FullName );
					UserSetting.SetData( "Configuration" , "Log Path" , Directory.CreateDirectory( Path.Combine( Info._userSetting.DirectoryName , "Log" ) ).FullName );
				
		
				CacheItemPolicy Policy = new CacheItemPolicy()
				{
					AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration ,
					Priority = CacheItemPriority.NotRemovable
				};
				MemoryCache.Default.Add( "DataPath" , UserSetting.GetData( "Configuration" , "Data Path" ) , Policy );
				MemoryCache.Default.Add( "LogPath" , UserSetting.GetData( "Configuration" , "Log Path" ) , Policy );


#if DEBUG
				Logger.Instance.LogFile.TraceLevel = (int)Enum.Parse( typeof( System.Diagnostics.TraceEventType ) , UserSetting.GetData( "Log" , "Trace" , "Information" ) );
#else
				Logger.Instance.LogFile.TraceLevel = (int)Enum.Parse( typeof( System.Diagnostics.TraceEventType ) , UserSetting.GetData( "Log" , "Trace" , "Warning" ) );
#endif
				}

			LogTrace.Label( $"Environment for {identifier} configured" );
		}
		#endregion PUBLIC METHODS
	}
}
