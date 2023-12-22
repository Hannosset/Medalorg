using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace xnext.Context
{
	/// <summary>
	/// What: 
	///  Why: 
	/// </summary>
	internal sealed class UserProfile : ProfileFile , IUserSettingsWriter , IUserSettingsReader
	{
		internal UserProfile( FileInfo iniFile ) : base( iniFile ) { }

		public bool Save( Control form )
		{
			if( form == null )
				throw new ArgumentNullException( nameof( form ) );

			return SetData( ValidName( form.Name ) , "Left" , form.Left )
				&& SetData( ValidName( form.Name ) , "Top" , form.Top )
				&& SetData( ValidName( form.Name ) , "Width" , form.Width )
				&& SetData( ValidName( form.Name ) , "Height" , form.Height );
		}

		public bool Load( Control form ) => Load( form , false );

		public bool Load( Control form , bool location )
		{
			if( form == null )
				throw new ArgumentNullException( nameof( form ) );

			try
			{
				form.Left = Math.Max( Screen.PrimaryScreen.WorkingArea.Left , GetData( ValidName( form.Name ) , "Left" , (Screen.PrimaryScreen.WorkingArea.Right - Screen.PrimaryScreen.WorkingArea.Left - (form.Right - form.Left)) / 2 ) );
				form.Top = Math.Max( Screen.PrimaryScreen.WorkingArea.Top , GetData( ValidName( form.Name ) , "Top" , Screen.PrimaryScreen.WorkingArea.Top ) );
				if( !location )
				{
					form.Width = Math.Max( 100 , GetData( ValidName( form.Name ) , "Width" , form.Width ) );
					form.Height = Math.Max( 100 , GetData( ValidName( form.Name ) , "Height" , form.Height ) );
				}

				int totWidth = 0, totHeight = 0;
				foreach( Screen scr in Screen.AllScreens )
				{
					totWidth += scr.Bounds.Width;
					totHeight += scr.Bounds.Height;
				}
				if( form.Top + form.Height > totHeight )
					form.Top = totHeight - form.Height;
				if( (form.Left + form.Width) > totWidth )
					form.Left = totWidth - form.Width;
				form.Left = Math.Max( 0 , form.Left );
				form.Top = Math.Max( 0 , form.Top );
			}
			catch { }
			return true;
		}
		public bool Save(  string section , string key , System.Drawing.Rectangle value ) => SetData( section , key , $"{value.X},{value.Y},{value.Width},{value.Height}" );

		public System.Drawing.Rectangle Load( string section , string key , System.Drawing.Rectangle defaultValue )
		{
			try
			{
				if( !string.IsNullOrEmpty( GetData( section , key , "" ) ) )
				{
					string[] str = GetData( section , key ).Replace( "\"" , "" ).Split( ',' );
					System.Drawing.Rectangle rect = new System.Drawing.Rectangle();
					try
					{
						rect.X = Math.Max( 0 , Convert.ToInt32( str[0] , CultureInfo.CurrentCulture ) );
						rect.Y = Math.Max( 0 , Convert.ToInt32( str[1] , CultureInfo.CurrentCulture ) );
						rect.Width = Math.Max( 100 , Convert.ToInt32( str[2] , CultureInfo.CurrentCulture ) );
						rect.Height = Math.Max( 100 , Convert.ToInt32( str[3] , CultureInfo.CurrentCulture ) );

						return rect;
					}
					catch( Exception )
					{
					}
				}
				else
					Save( section , key , defaultValue );
			}
			catch { }

			return defaultValue;
		}

		#region LOCAL METHODS
		private static string ValidName( string name )
		{
			if( name.IndexOf( ',' ) > 0 )
				return name.Substring( 0 , name.IndexOf( ',' ) ).Trim();
			return name.Trim();
		}
		#endregion LOCAL METHODS
	}
}
