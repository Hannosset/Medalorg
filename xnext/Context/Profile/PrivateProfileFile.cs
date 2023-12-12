using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace xnet.Context.Profile
{
	internal sealed class PrivateProfileFile : IApplicationSettingsWriter
	{
		#region LOCAL VARIABLES
		private const int InputBufferLen = 20 * 1024;
		private FileInfo IniFile;

		#endregion LOCAL VARIABLES

		#region CONSTRUCTOR

		internal PrivateProfileFile( string path , string iniFile ) => IniFile = new FileInfo( path + @"\" + iniFile );

		#endregion CONSTRUCTOR

		#region IApplicationSettingsWriter Members
		public void Flush()
		{ }
		public void Reset()
		{ }
		/// <summary>Sets the data.</summary>
		/// <param name="section">The section.</param>
		/// <param name="key">The key.</param>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">value</exception>
		public bool SetData( string section , string key , string data )
		{
			if( data == null )
				throw new ArgumentNullException( nameof( data ) );
			if( GetData( section , key , "" ) != data )
			{
				return Native.NativeMethods.WritePrivateProfileString( section , key , data.ToString( CultureInfo.CurrentCulture ) , ConfigFile );
			}
			return false;
		}

		/// <summary>Sets the data.</summary>
		/// <param name="section">The section.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public bool SetData( string section , string key , int value ) => Native.NativeMethods.WritePrivateProfileString( section , key , value.ToString( CultureInfo.CurrentCulture ) , ConfigFile );
		public bool SetData( string section , string key , bool value ) => Native.NativeMethods.WritePrivateProfileString( section , key , value.ToString( CultureInfo.CurrentCulture ) , ConfigFile );
		public bool SetData( string section , string key , decimal value ) => Native.NativeMethods.WritePrivateProfileString( section , key , value.ToString( CultureInfo.CurrentCulture ) , ConfigFile );
		#endregion IApplicationSettingsWriter Members

		#region IApplicationSettingsReader Members
		public string ConfigFile => IniFile.FullName;
		public string PathName
		{
			get => IniFile.DirectoryName;
			set => IniFile = new FileInfo( value + @"\" + IniFile.Name );
		}

		/// <summary>
		///  What: Access the data from the ini file
		///  Why: read and return the raw string associated to the key.
		/// </summary>
		/// <param name="section">The section.</param>
		/// <param name="key">The key.</param>
		/// <returns>System.String.</returns>
		public string GetData( string section , string key )
		{
			StringBuilder Ans = new StringBuilder( InputBufferLen );

			int rc = Native.NativeMethods.GetPrivateProfileString( section , key , "" , Ans , InputBufferLen , ConfigFile );
			if( rc == 0 )
			{
				if( Manager.Instance.Master != null )
					rc = Native.NativeMethods.GetPrivateProfileString( section , key , "" , Ans , InputBufferLen , Manager.Instance.Master.ConfigFile );
#if MACRO_COMMENT
				if( rc == 0 )
					Native.NativeMethods.WritePrivateProfileString( section , key , "" , ConfigFile );
#endif
			}
			return Ans.ToString();
		}

		public int GetDataValue( string section , string key ) => GetData( section , key , 0 );

		public decimal GetDecimalData( string section , string key )
		{
			try
			{
				if( decimal.TryParse( GetData( section , key ) , out decimal result ) )
					return result;
			}
			catch { }
			return 0M;
		}

		public int GetData( string section , string key , int defaultValue )
		{
			string value = GetData( section , key , defaultValue.ToString( CultureInfo.CurrentCulture ) );
			try
			{
				return Convert.ToInt32( value , CultureInfo.CurrentCulture );
			}
			catch( FormatException )
			{
				return defaultValue;
			}
			catch( OverflowException )
			{
				return defaultValue;
			}
		}

		public decimal GetData( string section , string key , decimal defaultValue )
		{
			string value = GetData( section , key , defaultValue.ToString( CultureInfo.CurrentCulture ) );
			try
			{
				return Convert.ToDecimal( value , CultureInfo.CurrentCulture );
			}
			catch( FormatException )
			{
				return defaultValue;
			}
			catch( OverflowException )
			{
				return defaultValue;
			}
		}

		public bool GetData( string section , string key , bool defaultValue )
		{
			string value = GetData( section , key , defaultValue.ToString( CultureInfo.CurrentCulture ) );
			try
			{
				return Convert.ToBoolean( value , CultureInfo.CurrentCulture );
			}
			catch( FormatException )
			{
				return defaultValue;
			}
		}

		public string GetData( string section , string key , string @default )
		{
			if( @default == null )
				throw new ArgumentNullException( nameof( @default ) );
			StringBuilder Ans = new StringBuilder( InputBufferLen );
			int rc = Native.NativeMethods.GetPrivateProfileString( section , key , "" , Ans , InputBufferLen , ConfigFile );
			if( rc == 0 )
			{
				if( Manager.Instance.Master != null )
					rc = Native.NativeMethods.GetPrivateProfileString( section , key , "" , Ans , InputBufferLen , Manager.Instance.Master.ConfigFile );
				if( rc == 0 )
				{
					if( Ans.ToString() != @default && !string.IsNullOrEmpty( @default ) )
						Native.NativeMethods.WritePrivateProfileString( section , key , @default , ConfigFile );
					Ans.Clear();
					Ans.Append( @default );
				}
			}

			return Ans.ToString();
		}

		#endregion IApplicationSettingsReader Members

		#region IDisposable Members

		public void Dispose() =>
			// This object will be cleaned up by the Dispose method. Therefore, you should call GC.SupressFinalize to take this object off the finalization
			// queue and prevent finalization code for this object from executing a second time.
			GC.SuppressFinalize( this );

		#endregion IDisposable Members
	}
}
