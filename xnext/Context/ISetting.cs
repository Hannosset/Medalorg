using System;
using System.Windows.Forms;

namespace xnext.Context
{
	public interface ISettingsReader : IDisposable
	{
		/// <summary>
		///  What: Access the ini configuration file associated with this object
		///  Why: the configuration is the ini file the application is reading or writing.
		/// </summary>
		string ConfigFile { get; }

		/// <summary>
		///  What: specify the path of the config file
		///  Why: in case of a local synchronization, we should be able to change the path of the same object.
		/// </summary>
		string PathName { get; set; }

		/// <summary>
		///  What: Access the data from the ini file
		///  Why: read and return the raw string associated to the key.
		/// </summary>
		/// <param name="section"> </param>
		/// <param name="key"> </param>
		/// <returns> </returns>
		string GetData( string section , string key );

		/// <summary> Gets the data value. </summary>
		/// <param name="section"> The section. </param>
		/// <param name="key"> The key. </param>
		/// <returns> </returns>
		int GetDataValue( string section , string key );

		/// <summary> Gets the double data. </summary>
		/// <param name="section"> The section. </param>
		/// <param name="key"> The key. </param>
		/// <returns> </returns>
		decimal GetDecimalData( string section , string key );

		/// <summary>
		///  What: Access the data from the ini file, if the key is not present, it is created with the default value
		///  Why: we might want to change manually the default recorded value.
		/// </summary>
		/// <param name="section"> </param>
		/// <param name="key"> </param>
		/// <param name="defaultValue"> </param>
		/// <returns> </returns>
		int GetData( string section , string key , int defaultValue );

		/// <summary> Gets the data. </summary>
		/// <param name="section"> The section. </param>
		/// <param name="key"> The key. </param>
		/// <param name="defaultValue"> The default value. </param>
		/// <returns> </returns>
		decimal GetData( string section , string key , decimal defaultValue );

		/// <summary> Gets the data. </summary>
		/// <param name="section"> The section. </param>
		/// <param name="key"> The key. </param>
		/// <param name="defaultValue"> if set to <c> true </c> [default value]. </param>
		/// <returns> </returns>
		bool GetData( string section , string key , bool defaultValue );

		/// <summary> Gets the data. </summary>
		/// <param name="section"> The section. </param>
		/// <param name="key"> The key. </param>
		/// <param name="defaultValue"> The default value. </param>
		/// <returns> </returns>
		string GetData( string section , string key , string defaultValue );
	}

	public interface ISettingsWriter : ISettingsReader
	{
		/// <summary>
		///  What: stores some configuration data in the ini file
		///  Why: allow to save configuration data.
		/// </summary>
		/// <param name="section"> </param>
		/// <param name="key"> </param>
		/// <param name="value"> </param>
		bool SetData( string section , string key , string value );

		/// <summary> Sets the data. </summary>
		/// <param name="section"> The section. </param>
		/// <param name="key"> The key. </param>
		/// <param name="value"> The value. </param>
		/// <returns> </returns>
		bool SetData( string section , string key , int value );

		/// <summary> Sets the data. </summary>
		/// <param name="section"> The section. </param>
		/// <param name="key"> The key. </param>
		/// <param name="value"> if set to <c> true </c> [value]. </param>
		/// <returns> </returns>
		bool SetData( string section , string key , bool value );

		/// <summary> Sets the data. </summary>
		/// <param name="section"> The section. </param>
		/// <param name="key"> The key. </param>
		/// <param name="value"> The value. </param>
		/// <returns> </returns>
		bool SetData( string section , string key , decimal value );
	}
	public interface IUserSettingsReader : ISettingsReader
	{
		bool Load( Control form );
		bool Load( Control form , bool location );
		System.Drawing.Rectangle Load( string section , string key , System.Drawing.Rectangle defaultValue );
	}
	public interface IUserSettingsWriter: ISettingsWriter
	{
		bool Save( Control form );
		
		bool Save( string section , string key , System.Drawing.Rectangle value );
	}
}
