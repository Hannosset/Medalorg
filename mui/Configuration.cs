using mui.Context.Protocol;

using System;
using System.ComponentModel;
using System.Diagnostics;
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
		/// What: Initialize the configuration dialog
		///  Why: Initialize the default or saved parameter values.
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
				if( CltWinEnv.AppReadSetting.GetData( Name , "Use Default Pathname" ) == "False" )
					radioButton2.Checked = true;
				else
					radioButton1.Checked = true;

				textBox3.Text = CltWinEnv.AppReadSetting.GetData( Name , "User Pathname" );
				textBox1.Text = CltWinEnv.AppReadSetting.GetData( Name , "Audio {Root}" );
				textBox2.Text = CltWinEnv.AppReadSetting.GetData( Name , "Video {Root}" );

				textBox4.Text = CltWinEnv.AppReadSetting.GetData( Name , "ffmpeg path" );
				textBox5.Text = CltWinEnv.AppReadSetting.GetData( Name , "ffmpeg arguments" );

				InitSubtitles();
			}
		}
		/// <summary>
		/// What: Saves the dialog position and size and the genre radio button
		///  Why: restore the screen and its selection as it was when closing
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
		/// What: Saves all parameters in the .ini file
		///  Why: Allow these parameters to impacts the results of the application.
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
				subtitles = subtitles + (lvi.Tag as CountryCode).Code + ",";

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
		/// What: Do not save all changes and restore the genre data set
		///  Why: Restore the genre data set from the hard disk
		/// </summary>
		private void OnCancel( object sender , EventArgs e )
		{
			Context.HandleMediaGenre.LoadFromFile();
		}

		#region TAB: TARGET DIRECTORY
		/// <summary>
		/// What: pops the Folder browsing dialog
		///  Why: Specify the root folder for the audio
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
		/// What: pops the Folder browsing dialog
		///  Why: Specify the root folder for the video
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
		private void OnGoToLink( object sender , LinkLabelLinkClickedEventArgs e )
		{
			if( radioButton4.Checked )
				Process.Start( "https://www.studiobinder.com/blog/movie-genres-list" );
			else if( listView1.SelectedItems.Count > 0 && listView1.SelectedItems[0].Name == "Blues" )
				Process.Start( "https://jazzfuel.com/types-of-blues-styles/" );
			else
				Process.Start( "https://www.musicianwave.com" );
		}
		/// <summary>
		/// What: Select a genre for the the audio display
		///  Why: Fills the genre listview with the audio genres
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

				foreach( MediaGenre mg in Context.HandleMediaGenre.Info.Details )
					if( mg.Type == AdaptiveKind.Audio )
					{
						ListViewItem lvi = new ListViewItem( mg.Label );
						lvi.Name = mg.Label;
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
				if( listView1.Items.Count > 0 )
					listView1.Items[0].Selected = true;
				listView1.EndUpdate();
				Cursor = crs;
			}
		}
		/// <summary>
		/// What: Select a genre for the video display
		///  Why: Fills the genre list view with the video genres
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

				foreach( MediaGenre mg in Context.HandleMediaGenre.Info.Details )
					if( mg.Type == AdaptiveKind.Video )
					{
						ListViewItem lvi = new ListViewItem( mg.Label );
						lvi.Name = mg.Label;
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
				if( listView1.Items.Count > 0 )
					listView1.Items[0].Selected = true;
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
		/// What: Add a new genre int he list - maintain the list sorted
		///  Why: Allow the end-user to manage its genres according to its choices
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
						lvi.Name = dlg.textBox1.Text;
						lvi.ToolTipText = dlg.textBox2.Text;
						lvi.Tag = Context.HandleMediaGenre.Add( radioButton3.Checked ? AdaptiveKind.Audio : AdaptiveKind.Video , dlg.textBox1.Text , dlg.textBox2.Text );
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
		/// What: Modify the description of the genre
		///  Why: All the end-user to specify his/her own description and comment
		/// </summary>
		private void OnEditGenre( object sender , EventArgs e )
		{
			LogTrace.Label();
			using( AddItem dlg = new AddItem() { Text = "Edit Genre Description" } )
			{
				dlg.textBox1.ReadOnly = true;
				dlg.textBox1.Text = listView1.SelectedItems[0].Text;
				dlg.textBox2.Text = (listView1.SelectedItems[0].Tag as MediaGenre).Description;

				if( dlg.ShowDialog() == DialogResult.OK )
				{
					LogTrace.Label();
					Cursor crs = Cursor;
					Cursor = Cursors.WaitCursor;
					listView1.BeginUpdate();
					try
					{
						ListViewItem lvi = listView1.SelectedItems[0];
						lvi.Name = dlg.textBox1.Text;
						lvi.ToolTipText = dlg.textBox2.Text;
						lvi.Tag = Context.HandleMediaGenre.Add( radioButton3.Checked ? AdaptiveKind.Audio : AdaptiveKind.Video , dlg.textBox1.Text , dlg.textBox2.Text );
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
		/// What: Removes a genre from the genre list and select either the next or the previous one.
		///  Why: If the user can add, the user should be allowed to delete
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

						Context.HandleMediaGenre.Remove( lvi.Tag as MediaGenre );
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
		/// What: Handles the selection of a genre
		///  Why: Populate the styles of the selected genre
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

					foreach( MediaGenre.MediaStyle ms in (listView1.SelectedItems[0].Tag as MediaGenre).Details )
					{
						ListViewItem lvi = new ListViewItem( ms.Label );
						lvi.Name = ms.Label;
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
					if( listView2.Items.Count > 0 )
						listView2.Items[0].Selected = true;

					label5.Text = (listView1.SelectedItems[0].Tag as MediaGenre).Description + "\n\n";

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
		/// What: A genre and a style are selected
		///  Why: Display the combined description of bot the genre and the style
		/// </summary>
		private void OnStyleSelected( object sender , EventArgs e )
		{
			try { label5.Text = $"{(listView1.SelectedItems[0].Tag as MediaGenre).Description}\n\n{(listView2.SelectedItems[0].Tag as MediaGenre.MediaStyle).Description}"; }
			catch( Exception ) { }
		}
		/// <summary>
		/// What: Add a style to the currently selected genre
		///  Why: Gives the end-user the possibility to specify styles of the genre
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
							lvi.Tag = (listView1.SelectedItems[0].Tag as MediaGenre).AddUpdate( dlg.textBox1.Text , dlg.textBox2.Text );
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
		/// What: changes the style description of the currently selected genre
		///  Why: Gives the end-user the possibility to specify styles of the genre
		/// </summary>
		private void OnEditStyle( object sender , EventArgs e )
		{
			LogTrace.Label();
			using( AddItem dlg = new AddItem() { Text = "Edit Style Description" } )
			{
				dlg.textBox1.ReadOnly = true;
				dlg.textBox1.Text = listView2.SelectedItems[0].Text;
				dlg.textBox2.Text = (listView2.SelectedItems[0].Tag as MediaGenre.MediaStyle).Description;

				if( dlg.ShowDialog() == DialogResult.OK )
				{
					LogTrace.Label();
					Cursor crs = Cursor;
					Cursor = Cursors.WaitCursor;
					listView2.BeginUpdate();
					try
					{
						ListViewItem lvi = listView2.SelectedItems[0];
						lvi.ToolTipText = dlg.textBox2.Text;
						(listView2.SelectedItems[0].Tag as MediaGenre.MediaStyle).Description = dlg.textBox2.Text;
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
						
						OnStyleSelected( sender , e );
					}
				}
			}
		}
		/// <summary>
		/// What: Delete a style
		///  Why: Gives full control on the genre-style to the end-user
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

						MediaGenre mi = listView1.SelectedItems[0].Tag as MediaGenre;
						mi.Remove( lvi.Tag as MediaGenre.MediaStyle );
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
		/// What: Initialize the language subtitles tab page
		///  Why: Lighten up the OnFormLoad() function
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
				foreach( CountryCode cc in Context.HandleCountryCode.Info.Details )
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
		/// What: Hyper link to the merging tool page
		///  Why: Provide the end user with a quick way to access the merge tool
		/// </summary>
		private void OnLinkClicked( object sender , LinkLabelLinkClickedEventArgs e )
		{
			Process.Start( "https://ffmpeg.org/" );
		}
		/// <summary>
		/// What: Locate the ffmpeg tool
		///  Why: The end-user may have installed the tool in a specific directory and this will prevent use to scan the drives to find the tool
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
