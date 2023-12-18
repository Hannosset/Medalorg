using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

using xnext.Context;
using xnext.Diagnostics;

namespace mui
{
	public partial class MainWindow : xnext.ui.BaseMainWindow
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
				Context.HandleCountryCode.LoadFromFile();
				Context.HandleAuthors.LoadFromFile();
				Context.Handle2Skip.LoadFromFile();

				splitContainer1.SplitterDistance = CltWinEnv.UserSetting.GetData( "Splitter" , "Main" , splitContainer2.SplitterDistance );
				splitContainer2.SplitterDistance = CltWinEnv.UserSetting.GetData( "Splitter" , "Panel" , splitContainer2.SplitterDistance );

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
				LogTrace.Label( e.Text );
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
					ListViewItem lvi = new ListViewItem( item.Caption );
					lvi.SubItems.Add( item.VideoCount.ToString() );
					lvi.SubItems.Add( item.AudioCount.ToString() );
					if( item.Downloaded )
					{
						lvi.SubItems.Add( "100 %" );
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

		#region LISTVIEW EVENTS
		private void OnItemSelected( object sender , EventArgs e )
		{
			if( listView1.SelectedItems.Count > 0 )
			{
				LogTrace.Label();
				Cursor crs = Cursor;
				Cursor = Cursors.WaitCursor;
				listView3.BeginUpdate();
				try
				{
					radioButton1.Checked = radioButton2.Checked = false;
					listView3.Items.Clear();
					Context.MediaInfo mi = listView1.SelectedItems[0].Tag as Context.MediaInfo;
					foreach( Context.MediaInfo.MediaData md in mi.Details )
					{
						ListViewItem lvi = new ListViewItem( md.ToString() );
						lvi.ImageIndex = md.Type == Context.MediaType.Video ? 0 : 1;
						lvi.Tag = md;
						listView3.Items.Add( lvi );
					}
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
		}
		private void OnMainSplitterMoved( object sender , SplitterEventArgs e ) => CltWinEnv.UserSetting.SetData( "Splitter" , "Main" , splitContainer1.SplitterDistance );
		#endregion

		#region RIGHT PANEL EVENTS
		private void OnPanelSplitterMoved( object sender , SplitterEventArgs e ) => CltWinEnv.UserSetting.SetData( "Splitter" , "Panel" , splitContainer2.SplitterDistance );

		private void OnAuthorLabelChanged( object sender , EventArgs e )
		{
			textBox4.Text = string.Empty;
			Context.AuthorInfo ai = Context.HandleAuthors.Info[textBox3.Text];
			if( ai != null )
			{
				if( ai.Type == Context.MediaType.Unkown )
				{
					radioButton1.Checked = radioButton2.Checked = false;
					listView4.Items.Clear();
					listView5.Items.Clear();
				}
				else
				{
					if( ai.Type == Context.MediaType.Audio )
						radioButton1.Checked = true;
					else if( ai.Type == Context.MediaType.Video )
						radioButton2.Checked = true;

					if( !string.IsNullOrEmpty( ai.Genre ) )
					{
						listView4.Items[ai.Genre].Checked = true;
						if( !string.IsNullOrEmpty( ai.Style ) )
							listView5.Items[ai.Style].Checked = true;
						else
							listView5.Items.Clear();
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

		private void OnUriSelected( object sender , EventArgs e )
		{
			LogTrace.Label();
			UpdateTargetPath();
		}

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

				foreach( Context.MediaGenre mg in Context.HandleMediaGenre.Info.Details )
					if( mg.Type == Context.MediaType.Audio )
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

				foreach( Context.MediaGenre mg in Context.HandleMediaGenre.Info.Details )
					if( mg.Type == Context.MediaType.Video )
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

					foreach( Context.MediaGenre.MediaStyle ms in (listView4.SelectedItems[0].Tag as Context.MediaGenre).Details )
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
		private void OnGenreChecked( object sender , ItemCheckedEventArgs e )
		{
			e.Item.Selected = true;
			UpdateTargetPath();
		}

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
		private void OnStyleChecked( object sender , ItemCheckedEventArgs e )
		{
			e.Item.Selected = true;
			UpdateTargetPath();
		}

		private void UpdateTargetPath()
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

			if( listView3.SelectedItems.Count > 0 )
				if( (listView3.SelectedItems[0].Tag as Context.MediaInfo.MediaData).Type == Context.MediaType.Audio )
					tmp = tmp.Replace( "{ROOT}" , CltWinEnv.AppReadSetting.GetData( Name , "Audio {Root}" , Environment.GetFolderPath( Environment.SpecialFolder.MyMusic ) ) );
				else
					tmp = tmp.Replace( "{ROOT}" , CltWinEnv.AppReadSetting.GetData( Name , "Video {Root}" , Environment.GetFolderPath( Environment.SpecialFolder.MyVideos ) ) );

			textBox4.Text = tmp.Replace( @"\\" , @"\" );
		}
		#endregion

		private void OnDownload( object sender , EventArgs e )
		{
			LogTrace.Label();
			Cursor crs = Cursor;
			Cursor = Cursors.WaitCursor;
			try
			{
				Context.MediaInfo mi = textBox2.Tag as Context.MediaInfo;
				Context.Handle2Skip.Update( mi.Title.Replace( textBox2.Text , "" ).Trim() );

				Context.AuthorInfo ai = Context.HandleAuthors.Update( textBox3.Text.Trim() );

				if( listView4.CheckedItems.Count > 0 )
					ai.Genre = listView4.CheckedItems[0].Text;
				if( listView5.CheckedItems.Count > 0 )
					ai.Style = listView5.CheckedItems[0].Text;
				if( radioButton1.Checked )
					ai.Type = Context.MediaType.Audio;
				else if( radioButton2.Checked )
					ai.Type = Context.MediaType.Video;

				Context.HandleAuthors.Flush();

				Context.Handle2Skip.Update( mi.Author.Replace( textBox3.Text , "" ).Trim() );
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
	}
}
