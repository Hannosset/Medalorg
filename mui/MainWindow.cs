using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using xnet.Diagnostics;

namespace mui
{
	public partial class MainWindow : xnet.ui.BaseMainWindow
	{
		/// <summary>Gets the identifier.</summary>
		/// <value>The identifier.</value>
		public override string Identifier { get; } = "Medalorg";

		#region CONSTRUCTOR
		public MainWindow()
		{
			InitializeComponent();
			if( LicenseManager.UsageMode == LicenseUsageMode.Runtime )
			{
				LogTrace.Label();
			}
		}

		private void OnFormLoad( object sender , System.EventArgs e )
		{
			if( LicenseManager.UsageMode == LicenseUsageMode.Runtime )
			{
				LogTrace.Label();
			}
		}

		private void OnFornClosing( object sender , FormClosingEventArgs e )
		{
			if( LicenseManager.UsageMode == LicenseUsageMode.Runtime )
			{
				LogTrace.Label();
			}
		}
		#endregion


		#region WINDOWS EVENTS
		private void OnClipboardChanged( object sender , xnext.ui.ClipboardChangedEventArgs e )
		{
			if( e.Text.Substring( 0 , 4 ).ToLower() == "http" )
			{
				textBox1.Text = e.Text;
				button1.PerformClick();
			}
		}
		private void OnAddWebVideo( object sender , EventArgs e )
		{
			if( textBox1.Text.Substring( 0 , 4 ).ToLower() == "http" )
			{
				Execute exec = new Execute();
				exec.ConsoleEvent += Exec_ConsoleEvent;
				exec.Launch( "mid.exe " , textBox1.Text );
			}
		}

		private void Exec_ConsoleEvent( object sender , ExecuteEventArgs e )
		{
			if( !string.IsNullOrEmpty( e.Output ) )
				Console.WriteLine( e.Output );
			else
				Console.WriteLine( "ERROR: " + e.Error );
		}
		#endregion

	}
}
