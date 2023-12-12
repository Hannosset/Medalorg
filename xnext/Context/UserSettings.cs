using xnet.Context.Profile;

using System;
using System.Globalization;
using System.Windows.Forms;

namespace xnet.Context
{
	public static class UserSettings
	{
		#region GLOBAL METHODS
		public static bool Save( string identifier , Control form )
		{
			if( form == null )
				throw new ArgumentNullException( nameof( form ) );

			return CltWinEnv.Info[identifier].SetData( ValidName( form.Name ) , "Left" , form.Left )
				&& CltWinEnv.Info[identifier].SetData( ValidName( form.Name ) , "Top" , form.Top )
				&& CltWinEnv.Info[identifier].SetData( ValidName( form.Name ) , "Width" , form.Width )
				&& CltWinEnv.Info[identifier].SetData( ValidName( form.Name ) , "Height" , form.Height );
		}

		public static bool Load( string identifier , Control form ) => Load( identifier , form , false );

		public static bool Load( string identifier , Control form , bool location )
		{
			if( form == null )
				throw new ArgumentNullException( nameof( form ) );

			IApplicationSettingsReader master = Manager.Instance.Master;
			try
			{
				Manager.Instance.Master = null;
				form.Left = Math.Max( Screen.PrimaryScreen.WorkingArea.Left , CltWinEnv.Info[identifier].GetData( ValidName( form.Name ) , "Left" , (Screen.PrimaryScreen.WorkingArea.Right - Screen.PrimaryScreen.WorkingArea.Left - (form.Right - form.Left)) / 2 ) );
				form.Top = Math.Max( Screen.PrimaryScreen.WorkingArea.Top , CltWinEnv.Info[identifier].GetData( ValidName( form.Name ) , "Top" , Screen.PrimaryScreen.WorkingArea.Top ) );
				if( !location )
				{
					form.Width = Math.Max( 100 , CltWinEnv.Info[identifier].GetData( ValidName( form.Name ) , "Width" , form.Width ) );
					form.Height = Math.Max( 100 , CltWinEnv.Info[identifier].GetData( ValidName( form.Name ) , "Height" , form.Height ) );
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
			finally
			{
				Manager.Instance.Master = master;
			}
			return true;
		}

		public static bool Save( string identifier , string section , string key , System.Drawing.Rectangle value ) => CltWinEnv.Info[identifier].SetData( section , key , $"{value.X},{value.Y},{value.Width},{value.Height}" );

		public static System.Drawing.Rectangle Load( string identifier , string section , string key , System.Drawing.Rectangle defaultValue )
		{
			IApplicationSettingsReader master = Manager.Instance.Master;
			try
			{
				Manager.Instance.Master = null;
				if( !string.IsNullOrEmpty( CltWinEnv.Info[identifier].GetData( section , key , "" ) ) )
				{
					string[] str = CltWinEnv.Info[identifier].GetData( section , key ).Replace( "\"" , "" ).Split( ',' );
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
					Save( identifier , section , key , defaultValue );
			}
			catch { }
			finally
			{
				Manager.Instance.Master = master;
			}
			return defaultValue;
		}

		#endregion GLOBAL METHODS

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
