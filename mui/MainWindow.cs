using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

using xnet.Diagnostics;

namespace mui
{
	public partial class MainWindow : xnet.ui.BaseMainWindow
	{
		#region LOCAL VARIABLE
		private Font ItalicFont;
		#endregion LOCAL VARIABLE
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
				_ = typeof( Control ).InvokeMember( "DoubleBuffered" , BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic , null , listView1 , new object[] { true } , CultureInfo.CurrentCulture );
			}
		}

		private void OnFormLoad( object sender , System.EventArgs e )
		{
			if( LicenseManager.UsageMode == LicenseUsageMode.Runtime )
			{
				LogTrace.Label();
				ItalicFont = new Font( listView1.Font.FontFamily , 8.25f , FontStyle.Strikeout );

				Context.HandleMediaInfo.LoadFromFile();
				Context.HandleMediaGenre.LoadFromFile();

				OnRefresh( sender , e );
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


		#region CLIPBOARD & MID.EXE EVENTS
		private void OnClipboardChanged( object sender , xnext.ui.ClipboardChangedEventArgs e )
		{
			if( e.Text.Substring( 0 , 4 ).ToLower() == "http" )
			{
				LogTrace.Label(e.Text);
				textBox1.Text = e.Text;
				button1.PerformClick();
			}
		}
		private void OnAddWebVideo( object sender , EventArgs e )
		{
			if( textBox1.Text.Substring( 0 , 4 ).ToLower() == "http" )
			{
				LogTrace.Label( $"Execute mid.exe {textBox1.Text}" );
				Execute exec = new Execute();
				exec.ConsoleEvent += Exec_ConsoleEvent;
				exec.Launch( "mid.exe " , textBox1.Text );
			}
		}
		private void Exec_ConsoleEvent( object sender , ExecuteEventArgs e )
		{
			LogTrace.Label();
			if( !string.IsNullOrEmpty( e.Output ) )
			{
				Context.HandleMediaInfo.Update( e.Output.Split( new char[] { '\t' } , StringSplitOptions.None ) );
#if DEBUG
				Console.WriteLine( e.Output );
#endif
			}
			else if( e.ExitCode != int.MinValue )
			{
				if( e.ExitCode == 0 )
					Context.HandleMediaInfo.Info.Serialize();
				else
					Console.WriteLine( "ERROR: " + e.Error );
			}

			Invoke( (Action)delegate
			{
				OnRefresh( sender , e );
			} );
		}
		#endregion

		#region LISTVIEW EVENTS
		private void OnItemSelected( object sender , EventArgs e )
		{

		}
		#endregion

		#region TOOLBAR EVENTS
		private void OnConfigure( object sender , EventArgs e )
		{
			using( Configuration dlg = new Configuration() )
			{
				if( dlg.ShowDialog( this ) == DialogResult.OK )
				{
					//	Refresh right panel
				}
			}
		}

		private void OnRefresh( object sender , EventArgs e )
		{
			LogTrace.Label();
			Cursor crs = Cursor;
			Cursor = Cursors.WaitCursor;
			listView1.BeginUpdate();
			try
			{
				listView1.Items.Clear();
				foreach( Context.MediaInfo item in Context.HandleMediaInfo.Info.Details )
				{
					ListViewItem lvi = new ListViewItem( item.Title );
					lvi.SubItems.Add( item.VideoCount.ToString() );
					lvi.SubItems.Add( item.AudioCount.ToString() );
					if( item.Downloaded )
					{
						lvi.SubItems.Add(  "100 %"  );
						lvi.Font = ItalicFont;
					}
					else
						lvi.SubItems.Add( "0 %" );

					lvi.Tag = item;

					listView1.Items.Add( lvi );
				}
			}
			catch( Exception ex )
			{
				Logger.TraceException( ex , "List of media to download incomplete" , "verify the 'data/MediaInfo.xml' is not corrupted and after correcttion restart the application." );
			}
			finally
			{
				listView1.AutoResizeColumns( ColumnHeaderAutoResizeStyle.ColumnContent );
				listView1.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
				listView1.EndUpdate();
				Cursor = crs;
			}
		}
		#endregion
	}
}
