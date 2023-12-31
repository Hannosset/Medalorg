using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using xnext.Context;
using xnext.Diagnostics;

namespace mui
{
	public partial class Configuration : Form
	{
		#region CONSTRUCTOR
		public Configuration()
		{
			InitializeComponent();
		}
		/// <summary>
		/// What: 
		///  Why: 
		/// </summary>
		private void OnFormLoad( object sender , EventArgs e )
		{
			if( !DesignMode && LicenseManager.UsageMode == LicenseUsageMode.Runtime )
			{
				LogTrace.Label();
				CltWinEnv.UserReadSetting.Load( this );
				if( CltWinEnv.AppReadSetting.GetData( Name , "Genre" , "Audio" ) == "Video" )
					radioButton4.Checked = true;
				else
					radioButton3.Checked = true;
				if( CltWinEnv.AppReadSetting.GetData( Name , "Use Default Pathname" , "True" ) == "False" )
					radioButton2.Checked = true;
				else
					radioButton1.Checked = true;

				textBox3.Text = CltWinEnv.AppReadSetting.GetData( Name , "User Pathname" , "{Root}" );
				textBox1.Text = CltWinEnv.AppReadSetting.GetData( Name , "Audio {Root}" , Environment.GetFolderPath( Environment.SpecialFolder.MyMusic ) );
				textBox2.Text = CltWinEnv.AppReadSetting.GetData( Name , "Video {Root}" , Environment.GetFolderPath( Environment.SpecialFolder.MyVideos ) );

				textBox4.Text = CltWinEnv.AppReadSetting.GetData( Name , "ffmpeg path" , new DirectoryInfo( "." ).FullName );
				textBox5.Text = CltWinEnv.AppReadSetting.GetData( Name , "ffmpeg arguments" , "-v 0 -y -max_error_rate 0.0 -i \"{audio-file}\" -i \"{video-file}\" -preset veryfast \"{media-file}\"" );

				InitSubtitles();
			}
		}
		/// <summary>
		/// What: 
		///  Why: 
		/// </summary>
		private void OnFormClosing( object sender , FormClosingEventArgs e )
		{
			if( !DesignMode && LicenseManager.UsageMode == LicenseUsageMode.Runtime )
			{
				LogTrace.Label();

				CltWinEnv.UserSetting.Save( this );

				if( radioButton3.Checked )
					CltWinEnv.AppSetting.SetData( Name , "Genre" , "Audio" );
				else
					CltWinEnv.AppSetting.SetData( Name , "Genre" , "Video" );
			}
		}
		#endregion CONSTRUCTOR

		/// <summary>
		/// What: 
		///  Why: 
		/// </summary>
		private void OnApply( object sender , EventArgs e )
		{
			LogTrace.Label();
			CltWinEnv.AppSetting.SetData( Name , "Audio {Root}" , textBox1.Text );
			CltWinEnv.AppSetting.SetData( Name , "Video {Root}" , textBox2.Text );

			CltWinEnv.AppSetting.SetData( Name , "Use Default Pathname" , radioButton1.Checked ? "True" : "False" );
			CltWinEnv.AppSetting.SetData( Name , "User Pathname" , textBox3.Text );
			CltWinEnv.AppSetting.SetData( Name , "Active Pathname" , radioButton1.Checked ? radioButton1.Text : textBox3.Text );

			string subtitles = "";
			foreach( ListViewItem lvi in listView3.CheckedItems )
				subtitles = subtitles + (lvi.Tag as Context.Protocol.CountryCode).Code + ",";

			CltWinEnv.AppSetting.SetData( Name , "Subtitles" , subtitles.Trim( ',' ) );

			if( string.IsNullOrEmpty( CltWinEnv.AppReadSetting.GetData( Name , "ffmpeg arguments" ) ) )
			{
				textBox5.Text = "-v 0 -y -max_error_rate 0.0 -i \"{audio-file}\" -i \"{video-file}\" -preset veryfast \"{media-file}\"";
				CltWinEnv.AppSetting.SetData( Name , "ffmpeg arguments" , textBox5.Text );
			}
			if( textBox4.Text != CltWinEnv.AppReadSetting.GetData( Name , "ffmpeg path" ) )
				CltWinEnv.AppSetting.SetData( Name , "ffmpeg path" , textBox4.Text );

			Context.HandleMediaGenre.Info.Serialize();
		}
		/// <summary>
		/// What: 
		///  Why: 
		/// </summary>
		private void OnCancel( object sender , EventArgs e )
		{
			Context.HandleMediaGenre.LoadFromFile();
		}

		#region TAB: TARGET DIRECTORY
		/// <summary>
		/// What: 
		///  Why: 
		/// </summary>
		private void OnSelectAudioRoot( object sender , EventArgs e )
		{
			LogTrace.Label();
			using( FolderBrowserDialog dlg = new FolderBrowserDialog() )
			{
				dlg.RootFolder = Environment.SpecialFolder.MyComputer;  //	Desktop;
				dlg.SelectedPath = textBox1.Text;
				dlg.ShowNewFolderButton = true;
				if( dlg.ShowDialog() == DialogResult.OK )
				{
					textBox1.Text = dlg.SelectedPath;
				}
			}
		}
		/// <summary>
		/// What: 
		///  Why: 
		/// </summary>
		private void OnSelectVideoRoot( object sender , EventArgs e )
		{
			LogTrace.Label();

			using( FolderBrowserDialog dlg = new FolderBrowserDialog() )
			{
				dlg.RootFolder = Environment.SpecialFolder.MyComputer;
				dlg.SelectedPath = textBox2.Text;
				dlg.ShowNewFolderButton = true;
				if( dlg.ShowDialog() == DialogResult.OK )
				{
					textBox2.Text = dlg.SelectedPath;
				}
			}
		}
		#endregion TAB: TARGET DIRECTORY

		#region TAB: MEDIA GENRES
		/// <summary>
		/// What: 
		///  Why: 
		/// </summary>
		private void OnAudioGenre( object sender , EventArgs e )
		{
			LogTrace.Label();
			Cursor crs = Cursor;
			Cursor = Cursors.WaitCursor;
			listView1.BeginUpdate();
			try
			{
				listView1.Items.Clear();
				listView2.Items.Clear();

				foreach( Context.Protocol.MediaGenre mg in Context.HandleMediaGenre.Info.Details )
					if( mg.Type == Context.Protocol.AdaptiveKind.Audio )
					{
						ListViewItem lvi = new ListViewItem( mg.Label );
						lvi.ToolTipText = mg.Description;
						lvi.Tag = mg;
						listView1.Items.Add( lvi );
					}
			}
			catch( Exception ex )
			{
				Logger.TraceException( ex , "List of Genre not updated correctly" , "verify the 'data/MediaGenre.xml' is not corrupted and restart the application." );
			}
			finally
			{
				listView1.AutoResizeColumns( ColumnHeaderAutoResizeStyle.ColumnContent );
				listView1.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
				listView1.EndUpdate();
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
			listView1.BeginUpdate();
			try
			{
				listView1.Items.Clear();
				listView2.Items.Clear();

				foreach( Context.Protocol.MediaGenre mg in Context.HandleMediaGenre.Info.Details )
					if( mg.Type == Context.Protocol.AdaptiveKind.Video )
					{
						ListViewItem lvi = new ListViewItem( mg.Label );
						lvi.ToolTipText = mg.Description;
						lvi.Tag = mg;
						listView1.Items.Add( lvi );
					}

			}
			catch( Exception ex )
			{
				Logger.TraceException( ex , "List of Genre not updated correctly" , "verify the 'data/MediaGenre.xml' is not corrupted and restart the application." );
			}
			finally
			{
				listView1.AutoResizeColumns( ColumnHeaderAutoResizeStyle.ColumnContent );
				listView1.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
				listView1.EndUpdate();
				Cursor = crs;
			}
		}
		/// <summary>
		/// What: 
		///  Why: 
		/// </summary>
		private void OnMediaSplitterMoved( object sender , SplitterEventArgs e ) => CltWinEnv.UserSetting.SetData( "Splitter" , "Genre" , splitContainer1.SplitterDistance );

		#region LISTVIEW GENRE
		/// <summary>
		/// What: 
		///  Why: 
		/// </summary>
		private void OnAddGenre( object sender , EventArgs e )
		{
			LogTrace.Label();
			using( AddItem dlg = new AddItem() { Text = "Add New Genre" } )
			{
				if( dlg.ShowDialog() == DialogResult.OK )
				{
					LogTrace.Label();
					Cursor crs = Cursor;
					Cursor = Cursors.WaitCursor;
					listView1.BeginUpdate();
					try
					{
						ListViewItem lvi = listView1.Items.Add( dlg.textBox1.Text );
						lvi.ToolTipText = dlg.textBox2.Text;
						lvi.Tag = Context.HandleMediaGenre.Add( dlg.textBox1.Text , dlg.textBox2.Text );
						listView1.Sort();
						lvi.Selected = true;
					}
					catch( Exception ex )
					{
						Logger.TraceException( ex , "List of Genre not updated correctly" , "verify the 'data/MediaGenre.xml' is not corrupted and restart the application." );
					}
					finally
					{
						listView1.AutoResizeColumns( ColumnHeaderAutoResizeStyle.ColumnContent );
						listView1.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
						listView1.EndUpdate();
						Cursor = crs;
					}
				}
			}
		}
		/// <summary>
		/// What: 
		///  Why: 
		/// </summary>
		private void OnEditGenre( object sender , EventArgs e )
		{
			LogTrace.Label();
			using( AddItem dlg = new AddItem() { Text = "Edit Genre Description" } )
			{
				dlg.textBox1.ReadOnly = true;
				dlg.textBox1.Text = listView1.SelectedItems[0].Text;
				dlg.textBox2.Text = (listView1.SelectedItems[0].Tag as Context.Protocol.MediaGenre).Description;

				if( dlg.ShowDialog() == DialogResult.OK )
				{
					LogTrace.Label();
					Cursor crs = Cursor;
					Cursor = Cursors.WaitCursor;
					listView1.BeginUpdate();
					try
					{
						ListViewItem lvi = listView1.SelectedItems[0];
						lvi.ToolTipText = dlg.textBox2.Text;
						lvi.Tag = Context.HandleMediaGenre.Add( dlg.textBox1.Text , dlg.textBox2.Text );
						listView1.Sort();
						lvi.Selected = true;
					}
					catch( Exception ex )
					{
						Logger.TraceException( ex , "List of Genre not updated correctly" , "verify the 'data/MediaGenre.xml' is not corrupted and restart the application." );
					}
					finally
					{
						listView1.AutoResizeColumns( ColumnHeaderAutoResizeStyle.ColumnContent );
						listView1.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
						listView1.EndUpdate();
						Cursor = crs;
					}
				}
			}
		}
		/// <summary>
		/// What: 
		///  Why: 
		/// </summary>
		private void OnDeleteGenre( object sender , KeyEventArgs e )
		{
			if( e.KeyCode == Keys.Delete )
			{
				LogTrace.Label();
				if( listView1.SelectedItems.Count > 0 )
				{
					ListViewItem lvi = listView1.SelectedItems[0];

					Cursor crs = Cursor;
					Cursor = Cursors.WaitCursor;
					listView1.BeginUpdate();
					try
					{
						//	Select the next best
						if( listView1.SelectedIndices[0] > 0 )
							listView1.Items[listView1.SelectedIndices[0] - 1].Selected = true;
						else if( listView1.Items.Count > 1 )
							listView1.Items[1].Selected = true;

						Context.HandleMediaGenre.Remove( lvi.Tag as Context.Protocol.MediaGenre );
						listView1.Items.Remove( lvi );
					}
					catch( Exception ex )
					{
						Logger.TraceException( ex , "List of styles not updated correctly" , "verify the 'data/MediaGenre.xml' is not corrupted and restart the application." );
					}
					finally
					{
						listView1.AutoResizeColumns( ColumnHeaderAutoResizeStyle.ColumnContent );
						listView1.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
						listView1.EndUpdate();
						Cursor = crs;
					}
				}
			}
		}
		/// <summary>
		/// What: 
		///  Why: 
		/// </summary>
		private void OnGenreSelected( object sender , EventArgs e )
		{
			LogTrace.Label();
			if( listView1.SelectedItems.Count > 0 )
			{
				Cursor crs = Cursor;
				Cursor = Cursors.WaitCursor;
				listView2.BeginUpdate();
				try
				{
					listView2.Items.Clear();

					label5.Text = (listView1.SelectedItems[0].Tag as Context.Protocol.MediaGenre).Description + "\n\n";

					foreach( Context.Protocol.MediaGenre.MediaStyle ms in (listView1.SelectedItems[0].Tag as Context.Protocol.MediaGenre).Details )
					{
						ListViewItem lvi = new ListViewItem( ms.Label );
						lvi.SubItems.Add( ms.Description );
						lvi.ToolTipText = ms.Description;
						lvi.Tag = ms;
						listView2.Items.Add( lvi );
					}
				}
				catch( Exception ex )
				{
					Logger.TraceException( ex , "List of Style not updated correctly" , "verify the 'data/MediaGenre.xml' is not corrupted and restart the application." );
				}
				finally
				{
					listView2.AutoResizeColumns( ColumnHeaderAutoResizeStyle.ColumnContent );
					listView2.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
					listView2.EndUpdate();
					Cursor = crs;
				}
			}
			else
				label5.Text = "";
		}
		#endregion LISTVIEW GENRE

		#region LISTVIEW STYLE
		/// <summary>
		/// What: 
		///  Why: 
		/// </summary>
		private void OnStyleSelected( object sender , EventArgs e )
		{
			string tmp = label5.Text;

			int at = tmp.IndexOf( "\n\n" );

			if( at > 0 )
				tmp = tmp.Substring( 0 , at );
			if( listView2.SelectedItems.Count > 0 )
				tmp = tmp + "\n\n" + (listView2.SelectedItems[0].Tag as Context.Protocol.MediaGenre.MediaStyle).Description;

			label5.Text = tmp;
		}
		/// <summary>
		/// What: 
		///  Why: 
		/// </summary>
		private void OnAddStyle( object sender , EventArgs e )
		{
			LogTrace.Label();
			if( listView1.SelectedItems.Count > 0 )
				using( AddItem dlg = new AddItem() { Text = "Add New Style" } )
				{
					if( dlg.ShowDialog() == DialogResult.OK )
					{
						LogTrace.Label();
						Cursor crs = Cursor;
						Cursor = Cursors.WaitCursor;
						listView2.BeginUpdate();
						try
						{
							ListViewItem lvi = listView2.Items.Add( dlg.textBox1.Text );
							lvi.ToolTipText = dlg.textBox2.Text;
							lvi.Tag = (listView1.SelectedItems[0].Tag as Context.Protocol.MediaGenre).Addpdate( dlg.textBox1.Text , dlg.textBox2.Text );
							listView2.Sort();
							lvi.Selected = true;
						}
						catch( Exception ex )
						{
							Logger.TraceException( ex , "List of styles not updated correctly" , "verify the 'data/MediaGenre.xml' is not corrupted and restart the application." );
						}
						finally
						{
							listView2.AutoResizeColumns( ColumnHeaderAutoResizeStyle.ColumnContent );
							listView2.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
							listView2.EndUpdate();
							Cursor = crs;
						}
					}
				}
		}
		/// <summary>
		/// What: 
		///  Why: 
		/// </summary>
		private void OnEditStyle( object sender , EventArgs e )
		{
			LogTrace.Label();
			using( AddItem dlg = new AddItem() { Text = "Edit Style Description" } )
			{
				dlg.textBox1.ReadOnly = true;
				dlg.textBox1.Text = listView2.SelectedItems[0].Text;
				dlg.textBox2.Text = (listView2.SelectedItems[0].Tag as Context.Protocol.MediaGenre.MediaStyle).Description;

				if( dlg.ShowDialog() == DialogResult.OK )
				{
					LogTrace.Label();
					Cursor crs = Cursor;
					Cursor = Cursors.WaitCursor;
					listView2.BeginUpdate();
					try
					{
						ListViewItem lvi = listView2.Items.Add( dlg.textBox1.Text );
						lvi.ToolTipText = dlg.textBox2.Text;
						lvi.Tag = Context.HandleMediaGenre.Add( dlg.textBox1.Text , dlg.textBox2.Text );
						listView2.Sort();
						lvi.Selected = true;
					}
					catch( Exception ex )
					{
						Logger.TraceException( ex , "List of styles not updated correctly" , "verify the 'data/MediaGenre.xml' is not corrupted and restart the application." );
					}
					finally
					{
						listView2.AutoResizeColumns( ColumnHeaderAutoResizeStyle.ColumnContent );
						listView2.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
						listView2.EndUpdate();
						Cursor = crs;
					}
				}
			}
		}
		/// <summary>
		/// What: 
		///  Why: 
		/// </summary>
		private void OnDeleteStyle( object sender , KeyEventArgs e )
		{
			if( e.KeyCode == Keys.Delete )
			{
				LogTrace.Label();
				if( listView1.SelectedItems.Count > 0 && listView2.SelectedItems.Count > 0 )
				{
					ListViewItem lvi = listView2.SelectedItems[0];

					Cursor crs = Cursor;
					Cursor = Cursors.WaitCursor;
					listView2.BeginUpdate();
					try
					{
						//	Select the next best
						if( listView2.SelectedIndices[0] > 0 )
							listView2.Items[listView2.SelectedIndices[0] - 1].Selected = true;
						else if( listView2.Items.Count > 1 )
							listView2.Items[1].Selected = true;

						Context.Protocol.MediaGenre mi = listView2.SelectedItems[0].Tag as Context.Protocol.MediaGenre;
						mi.Remove( lvi.Tag as Context.Protocol.MediaGenre.MediaStyle );
						listView2.Items.Remove( lvi );
					}
					catch( Exception ex )
					{
						Logger.TraceException( ex , "List of styles not updated correctly" , "verify the 'data/MediaGenre.xml' is not corrupted and restart the application." );
					}
					finally
					{
						listView2.AutoResizeColumns( ColumnHeaderAutoResizeStyle.ColumnContent );
						listView2.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
						listView2.EndUpdate();
						Cursor = crs;
					}
				}
			}
		}
		#endregion LISTVIEW STYLE
		#endregion TAB: MEDIA GENRES

		#region TAB: SUBTITLES
		/// <summary>
		/// What: 
		///  Why: 
		/// </summary>
		private void InitSubtitles()
		{
			string subtitles = CltWinEnv.AppReadSetting.GetData( Name , "Subtitles" , "en" );
			Cursor crs = Cursor;
			Cursor = Cursors.WaitCursor;
			listView3.BeginUpdate();
			try
			{
				listView3.Items.Clear();
				foreach( Context.Protocol.CountryCode cc in Context.HandleCountryCode.Info.Details )
				{
					ListViewItem lvi = new ListViewItem( cc.Code );
					lvi.SubItems.Add( cc.Label );
					lvi.ToolTipText = $"Country code is '{cc.Code}'";
					lvi.Checked = subtitles.Contains( cc.Code );
					lvi.Tag = cc;
					listView3.Items.Add( lvi );
				}
			}
			catch( Exception ex )
			{
				Logger.TraceException( ex , "List of Genre not updated correctly" , "verify the 'data/MediaGenre.xml' is not corrupted and restart the application." );
			}
			finally
			{
				listView3.AutoResizeColumns( ColumnHeaderAutoResizeStyle.ColumnContent );
				listView3.AutoResizeColumns( ColumnHeaderAutoResizeStyle.HeaderSize );
				listView3.EndUpdate();
				Cursor = crs;
			}
		}
		#endregion TAB: SUBTITLES

		#region TAB: FFMPEG
		/// <summary>
		/// What: 
		///  Why: 
		/// </summary>
		private void OnLinkClicked( object sender , LinkLabelLinkClickedEventArgs e )
		{
			Process.Start( "https://ffmpeg.org/" );
		}
		/// <summary>
		/// What: 
		///  Why: 
		/// </summary>
		private void OnLocateffmpeg( object sender , EventArgs e )
		{
			LogTrace.Label();
			using( OpenFileDialog dlg = new OpenFileDialog() )
			{
				dlg.InitialDirectory = textBox4.Text;
				dlg.Filter = "exe files (*.exe)|*.exe|All files (*.*)|*.*";
				dlg.FilterIndex = 1;
				dlg.RestoreDirectory = true;
				dlg.CheckFileExists = true;
				dlg.CheckPathExists = true;
				dlg.DefaultExt = ".exe";
				dlg.Multiselect = false;
				dlg.ValidateNames = true;
				if( dlg.ShowDialog() == DialogResult.OK )
				{
					textBox4.Text = dlg.FileName;
					CltWinEnv.AppSetting.SetData( Name , "ffmpeg path" , textBox4.Text );
				}
			}
		}
		#endregion TAB: FFMPEG
	}
}
