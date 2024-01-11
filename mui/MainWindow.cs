using mde.Context;

using mui.Context.Protocol;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Caching;
using System.Threading;
using System.Windows.Forms;

using xnext.Context;
using xnext.Diagnostics;

namespace mui
{
	public partial class MainWindow : xnext.ui.BaseMainWindow
	{
		#region LOCAL VARIABLE
		private Font ItalicFont;
		private Font StrikeoutFont;
		private bool FilterToDownload = false;
		private bool FilterDownloading = false;
		private int NrParallelDownload;
		private Semaphore ParallelDownload;
		AutoResetEvent OnCloseEvent = new AutoResetEvent( false );
		#endregion LOCAL VARIABLE

		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		public override string Identifier { get; } = "Medalorg";

		#region CONSTRUCTOR
		public MainWindow()
		{
			InitializeComponent();

			if( LicenseManager.UsageMode == LicenseUsageMode.Runtime )
			{
				LogTrace.Label();
				_ = typeof( Control ).InvokeMember( "DoubleBuffered" , BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic , null , listView1 , new object[] { true } , CultureInfo.CurrentCulture );
				_ = typeof( Control ).InvokeMember( "DoubleBuffered" , BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic , null , listView3 , new object[] { true } , CultureInfo.CurrentCulture );
				_ = typeof( Control ).InvokeMember( "DoubleBuffered" , BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic , null , listView4 , new object[] { true } , CultureInfo.CurrentCulture );
				_ = typeof( Control ).InvokeMember( "DoubleBuffered" , BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic , null , listView5 , new object[] { true } , CultureInfo.CurrentCulture );

				//	Create default parameter values
				CltWinEnv.AppReadSetting.GetData( "Configuration" , "User Pathname" , "{Root}" );
				CltWinEnv.AppReadSetting.GetData( "Configuration" , "Audio {Root}" , Environment.GetFolderPath( Environment.SpecialFolder.MyMusic ) );
				CltWinEnv.AppReadSetting.GetData( "Configuration" , "Video {Root}" , Environment.GetFolderPath( Environment.SpecialFolder.MyVideos ) );
				CltWinEnv.AppReadSetting.GetData( "Configuration" , "ffmpeg path" , new DirectoryInfo( "." ).FullName );
				CltWinEnv.AppReadSetting.GetData( "Configuration" , "ffmpeg arguments" , "-v 0 -y -max_error_rate 0.0 -i \"{audio-file}\" -i \"{video-file}\" -preset veryfast \"{media-file}\"" );
				CltWinEnv.AppReadSetting.GetData( "Configuration" , "Use Default Pathname" , "True" );
				CltWinEnv.AppReadSetting.GetData( "Configuration" , "Default Audio" , true );
				CltWinEnv.AppReadSetting.GetData( "Configuration" , "Default Best Video" , false );
				CltWinEnv.AppReadSetting.GetData( "Configuration" , "Default Good Video" , true );
				NrParallelDownload = CltWinEnv.AppReadSetting.GetData( "Configuration" , "# parallel download" , 5 );
			}
		}
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void OnFormLoad( object sender , System.EventArgs e )
		{
			if( LicenseManager.UsageMode == LicenseUsageMode.Runtime )
			{
				LogTrace.Label();
				ItalicFont = new Font( listView1.Font.FontFamily , 8.25f , FontStyle.Italic );
				StrikeoutFont = new Font( listView1.Font.FontFamily , 8.25f , FontStyle.Strikeout );

				Context.HandleMediaInfo.LoadFromFile();
				Context.HandleMediaGenre.LoadFromFile();
				Context.HandleCountryCode.LoadFromFile();
				Context.HandleAuthors.LoadFromFile();
				Context.Handle2Skip.LoadFromFile();

				splitContainer1.SplitterDistance = CltWinEnv.UserSetting.GetData( "Splitter" , "Main" , splitContainer2.SplitterDistance );
				splitContainer2.SplitterDistance = CltWinEnv.UserSetting.GetData( "Splitter" , "Panel" , splitContainer2.SplitterDistance );

				OnRefreshMediaInfo( sender , e );
				ParallelDownload = new Semaphore( NrParallelDownload , NrParallelDownload );

				//	Initiate auto load of the incomplete sessions 
				backgroundWorker1.RunWorkerAsync();
			}
		}
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void OnFormClosing( object sender , FormClosingEventArgs e )
		{
			if( LicenseManager.UsageMode == LicenseUsageMode.Runtime )
			{
				LogTrace.Label();
				Cursor crs = Cursor;
				Cursor = Cursors.WaitCursor;

				if( backgroundWorker1.WorkerSupportsCancellation )
					backgroundWorker1.CancelAsync();

				OnCloseEvent.Set();
				Thread.Sleep( 1000 );
				try
				{
					foreach( ListViewItem lvi in listView1.Items )
					{
						if( lvi.SubItems[2].Tag is Execute )
						{
							try
							{
								(lvi.SubItems[2].Tag as Execute).Dispose();
							}
							catch( Exception )
							{
							}
						}
					}
				}
				catch( Exception ex )
				{
					Logger.TraceException( ex , "Severe error" , "Restart the application." );
				}
				finally
				{
					Cursor = crs;
				}
				OnCloseEvent.WaitOne();
			}
		}
		#endregion CONSTRUCTOR

		#region CLIPBOARD & MID.EXE EVENTS
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void OnClipboardChanged( object sender , xnext.ui.ClipboardChangedEventArgs e )
		{
			if( e.Text.Substring( 0 , 4 ).ToLower() == "http" )
			{
				LogTrace.Label( e.Text );
				textBox1.Text = e.Text;
				button1.PerformClick();
			}
		}
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void OnAddWebVideo( object sender , EventArgs e )
		{
			if( textBox1.Text.Substring( 0 , 4 ).ToLower() == "http" )
			{
				LogTrace.Label( $"Execute mid.exe {textBox1.Text}" );
				Execute exec = new Execute();
				exec.ConsoleEvent += ClipboardConsoleEvent;
				exec.Launch( "mid.exe " , textBox1.Text );
			}
		}
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void ClipboardConsoleEvent( object sender , ExecuteEventArgs e )
		{
#if DEBUG
			Console.WriteLine( e.Output );
#endif
			LogTrace.Label();
			if( !string.IsNullOrEmpty( e.Output ) )
			{
				MediaInfo mi = Context.HandleMediaInfo.Update( e.Output.Split( new char[] { '\t' } , StringSplitOptions.None ) );
				if( mi != null )
					Invoke( (Action)delegate { RefreshView( mi.VideoId ); } );
			}
			else if( e.ExitCode != int.MinValue )
			{
				if( e.ExitCode == 0 )
					Context.HandleMediaInfo.Info.Serialize();
				else
					Console.WriteLine( "ERROR: " + e.Error );
			}

		}
		#endregion CLIPBOARD & MID.EXE EVENTS

		#region TOOLBAR EVENTS
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void OnConfigure( object sender , EventArgs e )
		{
			using( Configuration dlg = new Configuration() )
			{
				//	Prevent changing the parallelism is currently busy.
				foreach( MediaInfo mi in Context.HandleMediaInfo.Info.Details )
					if( mi.ListItem.PDownloading != null )
					{
						dlg.numericUpDown1.Enabled = false;
						break;
					}

				if( dlg.ShowDialog( this ) == DialogResult.OK )
				{
					if( CltWinEnv.AppReadSetting.GetData( "Configuration" , "# parallel download" , 5 ) != NrParallelDownload )
					{
						ParallelDownload.Close();
						ParallelDownload = new Semaphore( NrParallelDownload , NrParallelDownload );
					}
					//	Refresh right panel
					OnRefreshMediaInfo( sender , e );
				}
			}
		}
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void OnRefreshMediaInfo( object sender , EventArgs e )
		{
			LogTrace.Label();
			Cursor crs = Cursor;
			Cursor = Cursors.WaitCursor;
			listView1.BeginUpdate();
			try
			{
				string selectedItem = listView1.SelectedItems.Count > 0 ? listView1.SelectedItems[0].Name : string.Empty;
				string topItem = listView1.TopItem != null ? listView1.TopItem.Name : string.Empty;

				listView1.Items.Clear();

				foreach( MediaInfo mi in Context.HandleMediaInfo.Info.Details )
				{
					//	Filter here the item to add
					if( FilterDownloading )
					{
						if( mi.ListItem.PDownloading != null )
							listView1.Items.Add( mi );
					}
					else if( FilterToDownload )
					{
						if( !mi.Downloaded )
							listView1.Items.Add( mi ).Font = ItalicFont;
					}
					else
						listView1.Items.Add( mi );
				}

				if( !string.IsNullOrEmpty( selectedItem ) && listView1.Items.ContainsKey( selectedItem ) )
					listView1.Items[selectedItem].Selected = true;
				else if( listView1.Items.Count > 0 )
					listView1.Items[0].Selected = true;
				else
					listView3.Items.Clear();

				if( !string.IsNullOrEmpty( topItem ) && listView1.Items.ContainsKey( topItem ) )
					listView1.TopItem = listView1.Items[topItem];
			}
			catch( Exception ex )
			{
				Logger.TraceException( ex , "List of media to download incomplete" , "verify the 'data/MediaInfo.xml' is not corrupted and after correction restart the application." );
			}
			finally
			{
				listView1.AutoResizeColumns( ColumnHeaderAutoResizeStyle.ColumnContent );
				listView1.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
				listView1.EndUpdate();
				Cursor = crs;
			}
		}
		private void RefreshView( string videoid )
		{
			LogTrace.Label();
			listView1.BeginUpdate();

			if( listView1.Items.ContainsKey( videoid ) )
			{
				ListViewItem lvi = listView1.Items[videoid];
				(lvi.Tag as MediaInfo).UpdateListItem( lvi );
			}
			else if( Context.HandleMediaInfo.Info[videoid] != null )
			{
				if( !FilterDownloading )
					listView1.Items.Add( Context.HandleMediaInfo.Info[videoid] );
			}

			listView1.AutoResizeColumns( ColumnHeaderAutoResizeStyle.ColumnContent );
			listView1.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
			listView1.EndUpdate();
		}
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void OnRefreshMediaData()
		{
			Cursor crs = Cursor;
			Cursor = Cursors.WaitCursor;
			listView3.BeginUpdate();
			try
			{
				listView3.Items.Clear();

				MediaInfo mi = listView1.SelectedItems[0].Tag as MediaInfo;

				foreach( MediaInfo.MediaData md in mi.Details )
				{
					ListViewItem lvi = new ListViewItem( md.ToString() );
					lvi.Name = md.ToString();
					lvi.ImageIndex = md.Type == AdaptiveKind.Video ? 0 : 1;
					lvi.Tag = md;
					if( md.Downloaded )
					{
						lvi.ForeColor = Color.Gray;
						lvi.Font = StrikeoutFont;
					}

					if( mi.ListItem.webdownload != null && mi.ListItem.webdownload[md.DataLength] != null )
						lvi.Checked = true;

					listView3.Items.Add( lvi );
				}
				if( textBox3.Text.CompareTo( mi.Author ) == 0 )
					UpdateTargetPath();
				else
					textBox3.Text = mi.Author;
				textBox2.Text = mi.Title;
				textBox2.Tag = mi;
			}
			catch( Exception ex )
			{
				Logger.TraceException( ex , "List of Genre not updated correctly" , "verify the 'data/MediaGenre.xml' is not corrupted and restart the application." );
			}
			finally
			{
				listView3.EndUpdate();
				Cursor = crs;
			}
		}
		#endregion TOOLBAR EVENTS

		#region LISTVIEW EVENTS
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void OnItemSelected( object sender , EventArgs e )
		{
			if( listView1.SelectedItems.Count > 0 )
			{
				LogTrace.Label();
				OnRefreshMediaData();
			}
		}
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void OnMainSplitterMoved( object sender , SplitterEventArgs e ) => CltWinEnv.UserSetting.SetData( "Splitter" , "Main" , splitContainer1.SplitterDistance );
		#endregion LISTVIEW EVENTS

		#region RIGHT PANEL EVENTS
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void OnPanelSplitterMoved( object sender , SplitterEventArgs e ) => CltWinEnv.UserSetting.SetData( "Splitter" , "Panel" , splitContainer2.SplitterDistance );
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void OnAuthorLabelChanged( object sender , EventArgs e )
		{
			textBox4.Text = string.Empty;
			AuthorInfo ai = Context.HandleAuthors.Info[textBox3.Text];
			if( ai != null )
			{
				if( ai.Type == AdaptiveKind.Unkown )
				{
					radioButton1.Checked = radioButton2.Checked = false;
					listView4.Items.Clear();
					listView5.Items.Clear();
				}
				else
				{
					if( ai.Type == AdaptiveKind.Audio )
						radioButton1.Checked = true;
					else if( ai.Type == AdaptiveKind.Video )
						radioButton2.Checked = true;

					if( !string.IsNullOrEmpty( ai.Genre ) )
					{
						if( listView4.Items.ContainsKey( ai.Genre ) )
						{
							listView4.Items[ai.Genre].Selected = true;
							listView4.Items[ai.Genre].Checked = true;
							if( !string.IsNullOrEmpty( ai.Style ) && listView5.Items.ContainsKey( ai.Style ) )
								listView5.Items[ai.Style].Checked = true;
						}
						else if( listView4.CheckedItems.Count > 0 )
							listView4.CheckedItems[0].Checked = false;
					}
					else
					{
						listView4.Items.Clear();
						listView5.Items.Clear();
					}
				}
				UpdateTargetPath();
			}
			else
			{
				radioButton1.Checked = radioButton2.Checked = false;
				listView4.Items.Clear();
				listView5.Items.Clear();
			}
		}
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void OnSwapAuthor_Title( object sender , EventArgs e )
		{
			if( listView1.SelectedItems.Count > 0 )
			{
				MediaInfo mi = listView1.SelectedItems[0].Tag as MediaInfo;
				mi.Caption = $"{mi.Title} - {mi.Author}";
				Context.HandleMediaInfo.Info.Serialize();
				textBox2.Text = mi.Title;
				textBox3.Text = mi.Author;
				RefreshView( mi.VideoId );
			}
		}
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void OnUriSelected( object sender , EventArgs e )
		{
			LogTrace.Label();
			if( listView3.SelectedItems.Count > 0 )
				if( listView3.SelectedItems[0].Selected )
					listView3.SelectedItems[0].Checked = !listView3.SelectedItems[0].Checked;
		}
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void OnAudioGenre( object sender , EventArgs e )
		{
			LogTrace.Label();
			Cursor crs = Cursor;
			Cursor = Cursors.WaitCursor;
			listView4.BeginUpdate();
			try
			{
				listView4.Items.Clear();
				listView5.Items.Clear();

				foreach( MediaGenre mg in Context.HandleMediaGenre.Info.Details )
					if( mg.Type == AdaptiveKind.Audio )
					{
						ListViewItem lvi = new ListViewItem( mg.Label );
						lvi.Name = mg.Label;
						lvi.ToolTipText = mg.Description;
						lvi.Tag = mg;
						listView4.Items.Add( lvi );
					}
			}
			catch( Exception ex )
			{
				Logger.TraceException( ex , "List of Genre not updated correctly" , "verify the 'data/MediaGenre.xml' is not corrupted and restart the application." );
			}
			finally
			{
				listView4.AutoResizeColumns( ColumnHeaderAutoResizeStyle.ColumnContent );
				listView4.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
				listView4.EndUpdate();
				Cursor = crs;
			}
		}
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void OnVideoGenre( object sender , EventArgs e )
		{
			LogTrace.Label();
			Cursor crs = Cursor;
			Cursor = Cursors.WaitCursor;
			listView4.BeginUpdate();
			try
			{
				listView4.Items.Clear();
				listView5.Items.Clear();

				foreach( MediaGenre mg in Context.HandleMediaGenre.Info.Details )
					if( mg.Type == AdaptiveKind.Video )
					{
						ListViewItem lvi = new ListViewItem( mg.Label );
						lvi.Name = mg.Label;
						lvi.ToolTipText = mg.Description;
						lvi.Tag = mg;
						listView4.Items.Add( lvi );
					}
			}
			catch( Exception ex )
			{
				Logger.TraceException( ex , "List of Genre not updated correctly" , "verify the 'data/MediaGenre.xml' is not corrupted and restart the application." );
			}
			finally
			{
				listView4.AutoResizeColumns( ColumnHeaderAutoResizeStyle.ColumnContent );
				listView4.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
				listView4.EndUpdate();
				Cursor = crs;
			}
		}
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void OnGenreSelected( object sender , EventArgs e )
		{
			LogTrace.Label();
			if( listView4.SelectedItems.Count > 0 )
			{
				Cursor crs = Cursor;
				Cursor = Cursors.WaitCursor;
				listView5.BeginUpdate();
				try
				{
					listView5.Items.Clear();

					foreach( MediaGenre.MediaStyle ms in (listView4.SelectedItems[0].Tag as MediaGenre).Details )
					{
						ListViewItem lvi = new ListViewItem( ms.Label );
						lvi.Name = ms.Label;
						lvi.ToolTipText = ms.Description;
						lvi.Tag = ms;
						listView5.Items.Add( lvi );
					}
				}
				catch( Exception ex )
				{
					Logger.TraceException( ex , "List of Style not updated correctly" , "verify the 'data/MediaGenre.xml' is not corrupted and restart the application." );
				}
				finally
				{
					listView5.AutoResizeColumns( ColumnHeaderAutoResizeStyle.ColumnContent );
					listView5.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
					listView5.EndUpdate();
					Cursor = crs;
				}
			}
		}
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void OnGenreCheck( object sender , ItemCheckEventArgs e )
		{
			LogTrace.Label();
			Cursor crs = Cursor;
			Cursor = Cursors.WaitCursor;
			listView4.BeginUpdate();
			listView4.ItemCheck -= new ItemCheckEventHandler( OnGenreCheck );
			try
			{
				while( listView4.CheckedItems.Count > 0 )
					listView4.CheckedItems[0].Checked = false;
			}
			catch( Exception ex )
			{
				Logger.TraceException( ex , "Severe error" , "Restart the application." );
			}
			finally
			{
				listView4.AutoResizeColumns( ColumnHeaderAutoResizeStyle.ColumnContent );
				listView4.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
				listView4.EndUpdate();
				listView4.ItemCheck += new ItemCheckEventHandler( OnGenreCheck );
				Cursor = crs;
			}
		}
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void OnGenreChecked( object sender , ItemCheckedEventArgs e )
		{
			e.Item.Selected = true;
			UpdateTargetPath();
		}
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void OnStyleCheck( object sender , ItemCheckEventArgs e )
		{
			LogTrace.Label();
			Cursor crs = Cursor;
			Cursor = Cursors.WaitCursor;
			listView5.BeginUpdate();
			listView5.ItemCheck -= new ItemCheckEventHandler( OnStyleCheck );
			try
			{
				while( listView5.CheckedItems.Count > 0 )
					listView5.CheckedItems[0].Checked = false;
			}
			catch( Exception ex )
			{
				Logger.TraceException( ex , "Severe error" , "Restart the application." );
			}
			finally
			{
				listView5.AutoResizeColumns( ColumnHeaderAutoResizeStyle.ColumnContent );
				listView5.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
				listView5.EndUpdate();
				listView5.ItemCheck += new ItemCheckEventHandler( OnStyleCheck );
				Cursor = crs;
				UpdateTargetPath();
			}
		}
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void OnStyleChecked( object sender , ItemCheckedEventArgs e )
		{
			e.Item.Selected = true;
			UpdateTargetPath();
		}
		/// <summary>
		/// What: Assemble a target filename base on the configuration parameter and the genre, style and author of the media to download
		///  Why: Organize the media as they are being downloaded.
		/// </summary>
		private string RelativeTargetPath()
		{
			string tmp = CltWinEnv.AppReadSetting.GetData( "Configuration" , "Active Pathname" , "{Root}\\{Genre}\\{Style}\\{Author}" ).ToUpper();

			if( listView4.CheckedItems.Count > 0 )
				tmp = tmp.Replace( "{GENRE}" , listView4.CheckedItems[0].Text );
			else
				tmp = tmp.Replace( "{GENRE}" , "" ).Replace( @"\\" , @"\" );

			if( listView5.CheckedItems.Count > 0 )
				tmp = tmp.Replace( "{STYLE}" , listView5.CheckedItems[0].Text );
			else
				tmp = tmp.Replace( "{STYLE}" , "" ).Replace( @"\\" , @"\" );

			if( !string.IsNullOrEmpty( textBox3.Text ) )
				tmp = tmp.Replace( "{AUTHOR}" , textBox3.Text );
			else
				tmp = tmp.Replace( "{AUTHOR}" , "" ).Replace( @"\\" , @"\" );

			return tmp.Replace( @"\\" , @"\" );
		}
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void UpdateTargetPath()
		{
			string tmp = RelativeTargetPath();

			if( listView3.SelectedItems.Count > 0 )
				if( (listView3.SelectedItems[0].ImageIndex & 0x01) == 0x01 )
					tmp = tmp.Replace( "{ROOT}" , CltWinEnv.AppReadSetting.GetData( Name , "Audio {Root}" , Environment.GetFolderPath( Environment.SpecialFolder.MyMusic ) ) );
				else
					tmp = tmp.Replace( "{ROOT}" , CltWinEnv.AppReadSetting.GetData( Name , "Video {Root}" , Environment.GetFolderPath( Environment.SpecialFolder.MyVideos ) ) );

			textBox4.Text = tmp.Replace( @"\\" , @"\" );
		}

		#endregion RIGHT PANEL EVENTS

		#region Download
		/// <summary>
		/// What: Starts to download the media from the selected uri
		///  Why: Create a file in the published directory that will instruct the engine to download. Once fully downloaded, the file will be moved to the history directory.
		///		This mean that upon start up, the UI will continue any interrupted or failed download.
		/// </summary>
		private void OnDownload( object sender , EventArgs e )
		{
			if( listView1.SelectedItems.Count == 0 || listView3.CheckedItems.Count == 0 )
				return;

			LogTrace.Label();
			Cursor crs = Cursor;
			Cursor = Cursors.WaitCursor;

			MediaInfo mi = listView1.SelectedItems[0].Tag as MediaInfo;

			try
			{
				//	Set the author's name the best we can
				AuthorInfo ai = Context.HandleAuthors.Update( textBox3.Text.Trim() );
				if( listView4.CheckedItems.Count > 0 )
					ai.Genre = listView4.CheckedItems[0].Text;
				if( listView5.CheckedItems.Count > 0 )
					ai.Style = listView5.CheckedItems[0].Text;
				if( radioButton1.Checked )
					ai.Type = AdaptiveKind.Audio;
				else if( radioButton2.Checked )
					ai.Type = AdaptiveKind.Video;
				Context.HandleAuthors.Flush();

				//	All the word in the interface title and author that are not in our record are to be recorded as to skip
				Context.Handle2Skip.Update( mi.Title.Replace( textBox2.Text , "" ).Trim() );
				Context.Handle2Skip.Update( mi.Author.Replace( textBox3.Text , "" ).Trim() );

				//	Create a handle web download object to attach to the left panel list view item
				Context.HandleWebDownload HandleDownload = new Context.HandleWebDownload();
				//	set the relative path for the genre/style/author
				string filename = RelativeTargetPath();
				foreach( ListViewItem lvi in listView3.CheckedItems )
				{
					if( lvi.Tag is MediaInfo.MediaData md && md.Type == AdaptiveKind.Audio )
					{
						string audiofname = (filename.Replace( "{ROOT}" , CltWinEnv.AppReadSetting.GetData( Name , "Audio {Root}" ) ) + $@"\{mi.Title}{md.Extension}").Replace( @"\\" , @"\" );
						if( !CltWinEnv.AppReadSetting.GetData( Name , "Audio {Root}" ).ToLower().Contains( "{Author}" ) )
							audiofname = (filename.Replace( "{ROOT}" , CltWinEnv.AppReadSetting.GetData( Name , "Audio {Root}" ) ) + $@"\{mi.Author} - {mi.Title}{md.Extension}").Replace( @"\\" , @"\" );
						
						HandleDownload.UpdateWith( new Context.Protocol.DownloadAudio
						{
							VideoId = mi.VideoId ,
							DataLength = md.DataLength ,
							Filename = audiofname ,
							BitRate = (md as MediaInfo.AudioData).BitRate ,
							Format = (md as MediaInfo.AudioData).Model ,
							Downloaded = File.Exists( audiofname ) ,
							MediaData = md
						} );
					}
					else if( lvi.Tag is MediaInfo.VideoData vd )
					{
						string videofname = (filename.Replace( "{ROOT}" , CltWinEnv.AppReadSetting.GetData( Name , "Video {Root}" ) ) + $@"\{mi.Title}{vd.Extension}").Replace( @"\\" , @"\" );
						if( !CltWinEnv.AppReadSetting.GetData( Name , "Audio {Root}" ).ToLower().Contains( "{Author}" ) )
							videofname = (filename.Replace( "{ROOT}" , CltWinEnv.AppReadSetting.GetData( Name , "Video {Root}" ) ) + $@"\{mi.Author} - {mi.Title}{vd.Extension}").Replace( @"\\" , @"\" );

						HandleDownload.UpdateWith( new Context.Protocol.DownloadVideo
						{
							VideoId = mi.VideoId ,
							DataLength = vd.DataLength ,
							Filename = videofname ,
							Format = vd.Format ,
							Resolution = vd.Resolution ,
							Downloaded = File.Exists( videofname ) ,
							MediaData = vd
						} );
					}
				}
				if( HandleDownload.VideoNeedsAudio && !HandleDownload.HasAudio )
				{
					MessageBox.Show( "Some or all the video you selected are mpeg without audio.\nYou did not download any audio file.\nPlease select at least one audio file (vorbis and opus have the best quality)" , "STOP" , MessageBoxButtons.OK , MessageBoxIcon.Stop );
					return;
				}

				HandleDownload.Serialize();
			}
			catch( Exception ex )
			{
				Logger.TraceException( ex , "Severe error" , "Restart the application." );
			}
			finally
			{
				Cursor = crs;
			}
		}
		private bool LaunchDownload( BackgroundWorker bgnd , MediaInfo mi , Context.HandleWebDownload webDownload )
		{
			bool ans = false;

			//	Have we reached the number of allowed parallel download?
			if( ParallelDownload.WaitOne( 100 ) )
			{
				bgnd.ReportProgress( 0 , $"Execute mde.exe {webDownload.Filename}" );
				Execute exec = new Execute();

				mi.ListItem.webdownload = webDownload;
				mi.ListItem.PDownloading = exec;
				mi.ListItem.Communication = "Launching mde to download";
				exec.ConsoleEvent += MDEConsoleEvent;
				exec.Exit += ( s , ev ) => MDEConsoleExited( exec , mi );
				exec.Launch( "mde.exe " , webDownload.Filename );
				bgnd.ReportProgress( 0 , mi );
				ans = true;
			}
			return ans;
		}
		/// <summary>
		/// What: Shows the progress and communicate the status of the progress on the UI
		///  Why: the end user keeps informed of the progress of the download,
		/// </summary>
		private void MDEConsoleEvent( object sender , ExecuteEventArgs e )
		{
			LogTrace.Label( e.Output );
			try
			{
				if( !string.IsNullOrEmpty( e.Output ) )
				{
					string[] str = e.Output.Split( new char[] { '\t' } );
					if( str.Length > 3 )
					{
						MediaInfo mi = Context.HandleMediaInfo.Info[str[0]];
						if( mi != null )
						{
							if( decimal.TryParse( str[2] , out decimal downloaded ) && int.TryParse( str[1] , out int id ) )
							{
								mi.ListItem.webdownload.Update( id , downloaded );
								if( mi.ListItem.webdownload.Attempt( id ) > 1 )
								{
									if( mi.ListItem.webdownload.Attempt( id ) > 3 )
										mi.ListItem.Communication = $"{mi.ListItem.webdownload.Attempt( id )}th attempt - {str[3]} packet of {downloaded:#,###,###} bytes";
									else if( mi.ListItem.webdownload.Attempt( id ) > 2 )
										mi.ListItem.Communication = $"{mi.ListItem.webdownload.Attempt( id )}rd attempt - {str[3]} packet of {downloaded:#,###,###} bytes";
									else
										mi.ListItem.Communication = $"{mi.ListItem.webdownload.Attempt( id )}nd attempt - {str[3]} packet of {downloaded:#,###,###} bytes";
								}
								else
									mi.ListItem.Communication = $"{str[3]} packet of {downloaded:#,###,###} bytes";
							}
							else
								mi.ListItem.Communication = str[3];
							Invoke( (Action)delegate { RefreshView( str[0] ); } );
						}
					}
				}
			}
			catch( Exception ) { }
		}
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void MDEConsoleExited( object sender , EventArgs e )
		{
			LogTrace.Label();

			try
			{
				ParallelDownload.Release();

				MediaInfo mi = e as MediaInfo;

				mi.ListItem.PDownloading = null;

				//	Record the downloaded data.
				foreach( Context.Protocol.WebDownload wd in mi.ListItem.webdownload.Details )
					if( File.Exists( wd.Filename ) )
					{
						wd.MediaData.Filename = wd.Filename;
						wd.Downloaded = true;
					}

				Context.HandleMediaInfo.Info.Serialize();

				//	Update the UI: the list of audio and video of the selected media
				Invoke( (Action)delegate { RefreshView( mi.VideoId ); } );

				bool needserialization = false;
				//	Check if merge needs to be invoked
				if( mi.ListItem.webdownload.VideoNeedsAudio && mi.ListItem.webdownload.HasAudio )
				{
					if( !string.IsNullOrEmpty( CltWinEnv.AppReadSetting.GetData( "Configuration" , "ffmpeg path" ) ) )
					{
						foreach( Context.Protocol.WebDownload wd in mi.ListItem.webdownload.Details )
						{
							if( File.Exists( wd.Filename ) && wd is Context.Protocol.DownloadVideo downloadvideo && (wd.MediaData as MediaInfo.VideoData).BitRate < 1 )
							{
								MediaInfo.VideoData vd = wd.MediaData as MediaInfo.VideoData;
								downloadvideo.TargetFilename = downloadvideo.TargetFilename.Replace( "@-1." , $"@{mi.ListItem.webdownload.BestAudio.BitRate}." ).Replace( ".mpeg" , "" );

								if( !File.Exists( downloadvideo.TargetFilename ) )
								{
									string args = CltWinEnv.AppReadSetting.GetData( "Configuration" , "ffmpeg arguments" , "-v 0 -y -max_error_rate 0.0 -i \"{audio-file}\" -i \"{video-file}\" -preset veryfast \"{media-file}\"" );
									args = args.Replace( "{audio-file}" , mi.ListItem.webdownload.BestAudio.Filename );
									args = args.Replace( "{video-file}" , wd.Filename );
									args = args.Replace( "{media-file}" , downloadvideo.TargetFilename );
									
									mi.ListItem.Communication = "Merging audio and video";
									Invoke( (Action)delegate { RefreshView( mi.VideoId ); } );

									Execute exec = new Execute();
									exec.Run( CltWinEnv.AppReadSetting.GetData( "Configuration" , "ffmpeg path" ) , args );
								}
								if( File.Exists( downloadvideo.TargetFilename ) )
								{
									mi.Add( downloadvideo.TargetFilename );
									mi.ListItem.Communication = "Merging Successful";
									needserialization = true;
								}
								else
									mi.ListItem.Communication = "Error: Merging failed";
								Invoke( (Action)delegate { RefreshView( mi.VideoId ); } );
							}
						}
					}
				}
				else
					needserialization = true;

				if( needserialization )
				{
					try
					{
						if( File.Exists( Path.Combine( MemoryCache.Default["PublishPath"] as string , mi.VideoId + ".xml" ) ) )
							File.Delete( Path.Combine( MemoryCache.Default["PublishPath"] as string , mi.VideoId + ".xml" ) );
					}
					catch( Exception ) { }
					Context.HandleMediaInfo.Info.Serialize();
				}
			}
			catch( Exception ) { }
		}
		#endregion

		#region  CONTEXT MENU
		private void OnOpenVideoOnBrowser( object sender , EventArgs e )
		{
			if( listView1.SelectedItems.Count > 0 )
				Process.Start( $"https://www.youtube.com/watch?v={listView1.SelectedItems[0].Name}" );
		}
		private void OnOpenPopup( object sender , CancelEventArgs e )
		{
			filterToolStripMenuItem.Checked = FilterToDownload;
			downloadingToolStripMenuItem.Checked = FilterDownloading;
		}
		private void gotoAudioFile( object sender , EventArgs e )
		{
			if( listView1.SelectedItems.Count > 0 && Directory.Exists( RelativeTargetPath().Replace( "{ROOT}" , CltWinEnv.AppReadSetting.GetData( Name , "Audio {Root}" ) ) ) )
				Process.Start( RelativeTargetPath().Replace( "{ROOT}" , CltWinEnv.AppReadSetting.GetData( Name , "Audio {Root}" ) ) );
		}
		private void GotoVideoFile( object sender , EventArgs e )
		{
			if( listView1.SelectedItems.Count > 0 && Directory.Exists( RelativeTargetPath().Replace( "{ROOT}" , CltWinEnv.AppReadSetting.GetData( Name , "Video {Root}" ) ) ) )
				Process.Start( RelativeTargetPath().Replace( "{ROOT}" , CltWinEnv.AppReadSetting.GetData( Name , "Video {Root}" ) ) );
		}
		private void OnBatchDownload( object sender , EventArgs e )
		{
			SetupBatchDownload();
		}
		private void OnFilterListview( object sender , EventArgs e )
		{
			FilterToDownload = !FilterToDownload;
			if( FilterToDownload )
				FilterDownloading = false;
			OnRefreshMediaInfo( sender , e );
		}
		private void OnDisplayDownloading( object sender , EventArgs e )
		{
			FilterDownloading = !FilterDownloading;
			if( FilterDownloading )
				FilterToDownload = false;
			OnRefreshMediaInfo( sender , e );
		}
		private void OnForgetMedia( object sender , EventArgs e )
		{
			LogTrace.Label();
			Cursor crs = Cursor;
			Cursor = Cursors.WaitCursor;
			listView1.BeginUpdate();
			try
			{
				string selectedItem = string.Empty;
				for( int i = listView1.SelectedIndices[0] ; i >= 0 && string.IsNullOrEmpty( selectedItem ) ; i-- )
					if( !listView1.Items[i].Checked )
						selectedItem = listView1.SelectedItems[0].Name;
				for( int i = listView1.SelectedIndices[0] ; i < listView1.Items.Count && string.IsNullOrEmpty( selectedItem ) ; i++ )
					if( !listView1.Items[i].Checked )
						selectedItem = listView1.SelectedItems[0].Name;

				string topItem = listView1.TopItem != null ? listView1.TopItem.Name : string.Empty;
				for( int i = listView1.SelectedIndices[0] ; i >= 0 && string.IsNullOrEmpty( selectedItem ) ; i-- )
					if( !listView1.Items[i].Checked )
						topItem = listView1.SelectedItems[0].Name;
				for( int i = listView1.SelectedIndices[0] ; i < listView1.Items.Count && string.IsNullOrEmpty( selectedItem ) ; i++ )
					if( !listView1.Items[i].Checked )
						topItem = listView1.SelectedItems[0].Name;

				while( listView1.CheckedItems.Count > 0 )
				{
					Context.HandleMediaInfo.Info.Remove( listView1.CheckedItems[0].Name );
					listView1.Items.Remove( listView1.CheckedItems[0] );
				}
				Context.HandleMediaInfo.Info.Serialize();
			}
			catch( Exception ex )
			{
				Logger.TraceException( ex , "Error removing a media from the list" , "Kindly try again, if the issue persist contact support." );
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

		#region  BATCH PROCESS
		/// <summary>
		/// What: Prepare the checked item to be batch processed generating the download parameter file in the published directory
		///  Why: the end user run a set of media to download, the background thread will manage the download execution 
		/// </summary>
		private void SetupBatchDownload()
		{
			LogTrace.Label();
			Cursor crs = Cursor;
			Cursor = Cursors.WaitCursor;
			try
			{
				string filename = RelativeTargetPath();

				string[] range = CltWinEnv.AppSetting.GetData( Name , "Range Good Video" ).Split( ',' );
				int maxres = CltWinEnv.AppReadSetting.GetData( "Configuration" , "Default Best Video" , false ) ? 10000 : 0;
				int minres = 0;
				if( CltWinEnv.AppReadSetting.GetData( "Configuration" , "Default Good Video" , true ) )
				{
					if( range.Length > 0 )
						int.TryParse( range[0] , out maxres );
					if( range.Length > 1 )
						int.TryParse( range[1] , out minres );
				}
				List<Context.HandleWebDownload> batchList = new List<Context.HandleWebDownload>();

				foreach( ListViewItem lvi in listView1.CheckedItems )
				{
					MediaInfo mi = lvi.Tag as MediaInfo;
					if( mi.ListItem.PDownloading == null )
					{
						Context.HandleWebDownload HandleDownload = new Context.HandleWebDownload();

						MediaInfo.AudioData BestAudio = mi.BestAudio( CltWinEnv.AppReadSetting.GetData( "Configuration" , "Default Audio" , true ) );
						if( BestAudio != null )
						{
							string audiofname = (filename.Replace( "{ROOT}" , CltWinEnv.AppReadSetting.GetData( Name , "Audio {Root}" ) ) + $@"\{mi.Title}{BestAudio.Extension}").Replace( @"\\" , @"\" );
							if( !CltWinEnv.AppReadSetting.GetData( Name , "Audio {Root}" ).ToLower().Contains( "{Author}" ) )
								audiofname = (filename.Replace( "{ROOT}" , CltWinEnv.AppReadSetting.GetData( Name , "Audio {Root}" ) ) + $@"\{mi.Author} - {mi.Title}{BestAudio.Extension}").Replace( @"\\" , @"\" );

							HandleDownload.UpdateWith( new Context.Protocol.DownloadAudio
							{
								VideoId = mi.VideoId ,
								DataLength = BestAudio.DataLength ,
								Filename = audiofname ,
								BitRate = BestAudio.BitRate ,
								Format = BestAudio.Model ,
								Downloaded = File.Exists( audiofname ) ,
								MediaData = BestAudio
							} );
						}

						MediaInfo.VideoData BestVideo = mi.BestVideo( maxres , minres );
						if( BestVideo != null )
						{
							string videofname = (filename.Replace( "{ROOT}" , CltWinEnv.AppReadSetting.GetData( Name , "Video {Root}" ) ) + $@"\{mi.Title}{BestVideo.Extension}").Replace( @"\\" , @"\" );
							if( !CltWinEnv.AppReadSetting.GetData( Name , "Audio {Root}" ).ToLower().Contains( "{Author}" ) )
								videofname = (filename.Replace( "{ROOT}" , CltWinEnv.AppReadSetting.GetData( Name , "Video {Root}" ) ) + $@"\{mi.Author} - {mi.Title}{BestVideo.Extension}").Replace( @"\\" , @"\" );

							HandleDownload.UpdateWith( new Context.Protocol.DownloadVideo
							{
								VideoId = mi.VideoId ,
								DataLength = BestVideo.DataLength ,
								Filename = videofname ,
								Format = BestVideo.Format ,
								Resolution = BestVideo.Resolution ,
								Downloaded = File.Exists( videofname ) ,
								MediaData = BestVideo
							} );
						}
						if( HandleDownload.VideoNeedsAudio && !HandleDownload.HasAudio )
							continue;

						HandleDownload.Serialize();
						batchList.Add( HandleDownload );
					}
				}
			}
			catch( Exception ex )
			{
				Logger.TraceException( ex , "Error producing the batch files" , "verify the 'data/MediaInfo.xml' is not corrupted and after correction restart the application." );
			}
			finally
			{
				Cursor = crs;
			}
		}
		/// <summary>
		/// What:
		///  Why:
		/// </summary>
		private void OnProcessBatch( object sender , DoWorkEventArgs e )
		{
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;

			List<Context.HandleWebDownload> downloading = new List<Context.HandleWebDownload>();
			DirectoryInfo PublishedDir = new DirectoryInfo( MemoryCache.Default["PublishPath"] as string );

			bool keepon = OnCloseEvent.WaitOne( 1 ) == false;

			//	Loop as long as we are not closing the application.
			while( keepon )
			{
				//	Every time scans the published directory and process each download request
				foreach( FileInfo fi in PublishedDir.GetFiles( "*.xml" , SearchOption.TopDirectoryOnly ) )
				{
					Context.HandleWebDownload webDownload = Context.HandleWebDownload.Deserialize( fi.FullName );
					MediaInfo mi = Context.HandleMediaInfo.Info[webDownload.VideoId];
					//	The video id is registered in our database
					if( mi != null && fi.Exists )
					{
						webDownload.Bind( mi );
						//	The item is not currently downloading
						if( mi.ListItem.PDownloading == null )
							if( !LaunchDownload( sender as BackgroundWorker , mi , webDownload ) )
								for( int i = 0 ; keepon && i < 100 ; i++ )
								{
									Thread.Sleep( 100 );
									keepon = OnCloseEvent.WaitOne( 1 ) == false;
								}
					}
					else
						try { fi.Delete(); } catch( Exception ) { }

					if( !keepon )
						break;
				}
			}
			e.Cancel = true;
			OnCloseEvent.Set();
		}
		#endregion

		private void OnBatchProgress( object sender , ProgressChangedEventArgs e )
		{
			if( e.UserState is MediaInfo mi )
				RefreshView( mi.VideoId );
			else
			{
				LogTrace.Label( e.UserState as string );
				OnLogMessage( this , new StatusEventArgs( e.UserState as string ) );
			}
		}
	}
}