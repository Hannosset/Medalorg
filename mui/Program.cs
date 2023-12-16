using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

using xnext.Native;

namespace mui
{
	internal static class Program
	{
		#region SINGLETON PROCESS
		private static bool SingleProcess()
		{
			foreach( Process process in Process.GetProcessesByName( Process.GetCurrentProcess().ProcessName ) )
				if( process.Id != Process.GetCurrentProcess().Id && Process.GetCurrentProcess().MainModule.FileName == process.MainModule.FileName )
				{
					if( new Version( process.MainModule.FileVersionInfo.FileVersion ) > new Version( Process.GetCurrentProcess().MainModule.FileVersionInfo.FileVersion ) )
					{
						NativeMethods.SetForegroundWindow( process.MainWindowHandle );
						return false;
					}
					else
						process.Kill();
				}
			return true;
		}
		#endregion SINGLETON PROCESS

		/// <summary>The main entry point for the application.</summary>
		[STAThread]
		static void Main()
		{
			Thread.CurrentThread.CurrentUICulture = new CultureInfo( "en-US" );
			Thread.CurrentThread.CurrentCulture = new CultureInfo( "en-US" );
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );

			if( SingleProcess() )
				try
				{
					using( MainWindow Main = new MainWindow() )
						Application.Run( Main );
				}
				catch( Exception ex )
				{
					_ = MessageBox.Show( ex.ToString() , "Application Level Exception" , MessageBoxButtons.OK , MessageBoxIcon.Error , MessageBoxDefaultButton.Button1 , MessageBoxOptions.DefaultDesktopOnly );
				}
		}
	}
}
