using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using xnet.Context.Profile;
using xnet.Diagnostics;

namespace mui
{
	public partial class Configuration : Form
	{
		#region CONSTRUCTOR
		public Configuration()
		{
			InitializeComponent();
		}

		private void OnFormLoad( object sender , EventArgs e )
		{
			if( !DesignMode && LicenseManager.UsageMode == LicenseUsageMode.Runtime )
			{
				LogTrace.Label();
				if( Manager.Instance.AppReadSetting.GetData( Name , "Genre" , "Audio" ) == "Video" )
					radioButton3.Checked = true;
				else
					radioButton4.Checked = true;
				if( Manager.Instance.AppReadSetting.GetData( Name , "Use Default Pathname" , "True" ) == "False" )
					radioButton2.Checked = true;
				else
					radioButton1.Checked = true;

				textBox3.Text = Manager.Instance.AppReadSetting.GetData( Name , "User Pathname" , "{Root}" );
				textBox1.Text = Manager.Instance.AppReadSetting.GetData( Name , "Audio {Root}" , Environment.GetFolderPath( Environment.SpecialFolder.MyMusic ) );
				textBox2.Text = Manager.Instance.AppReadSetting.GetData( Name , "Video {Root}" , Environment.GetFolderPath( Environment.SpecialFolder.MyVideos ) );
			}
		}

		private void OnFormClosing( object sender , FormClosingEventArgs e )
		{
			if( !DesignMode && LicenseManager.UsageMode == LicenseUsageMode.Runtime )
			{
				LogTrace.Label();
				if( radioButton3.Checked )
					Manager.Instance.AppSetting.SetData( Name , "Genre" , "Audio" );
				else
					Manager.Instance.AppSetting.SetData( Name , "Genre" , "Video" );
			}
		}
		#endregion CONSTRUCTOR

		#region PATHNAMES
		private void OnSelectAudioRoot( object sender , EventArgs e )
		{
			LogTrace.Label();
			using( FolderBrowserDialog dlg = new FolderBrowserDialog() )
			{
				dlg.RootFolder = Environment.SpecialFolder.MyComputer;	//	Desktop;
				dlg.SelectedPath = textBox1.Text;
				dlg.ShowNewFolderButton = true;
				if( dlg.ShowDialog() == DialogResult.OK )
				{
					textBox1.Text = dlg.SelectedPath;
				}
			}
		}

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
		#endregion CONSTRUCTOR

		#region GENRE & STYLE EVENTS
		private void OnAudioGenre( object sender , EventArgs e )
		{
			LogTrace.Label();
			Cursor crs = Cursor;
			Cursor = Cursors.WaitCursor;
			listView1.BeginUpdate();
			try
			{
				listView1.Items.Clear();

				foreach( Context.MediaGenre mg in Context.HandleMediaGenre.Info.Details )
					if( mg.Type == Context.MediaGenre.MediaType.Audio )
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

		private void OnVideoGenre( object sender , EventArgs e )
		{
			LogTrace.Label();
			Cursor crs = Cursor;
			Cursor = Cursors.WaitCursor;
			listView1.BeginUpdate();
			try
			{
				listView1.Items.Clear();

				foreach( Context.MediaGenre mg in Context.HandleMediaGenre.Info.Details )
					if( mg.Type == Context.MediaGenre.MediaType.Video )
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

		private void OnEditGenre( object sender , EventArgs e )
		{
			LogTrace.Label();
			using( AddItem dlg = new AddItem() { Text = "Edit Genre Description" } )
			{
				dlg.textBox1.ReadOnly = true;
				dlg.textBox1.Text = listView1.SelectedItems[0].Text;
				dlg.textBox2.Text = (listView1.SelectedItems[0].Tag as Context.MediaGenre).Description;

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

						Context.HandleMediaGenre.Remove( lvi.Tag as Context.MediaGenre );
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

					foreach( Context.MediaGenre.MediaStyle ms in (listView1.SelectedItems[0].Tag as Context.MediaGenre).Details )
					{
						ListViewItem lvi = new ListViewItem( ms.Label );
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
		}

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
							lvi.Tag = (listView1.SelectedItems[0].Tag as Context.MediaGenre).Addpdate( dlg.textBox1.Text , dlg.textBox2.Text );
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

		private void OnEditStyle( object sender , EventArgs e )
		{
			LogTrace.Label();
			using( AddItem dlg = new AddItem() { Text = "Edit Style Description" } )
			{
				dlg.textBox1.ReadOnly = true;
				dlg.textBox1.Text = listView2.SelectedItems[0].Text;
				dlg.textBox2.Text = (listView2.SelectedItems[0].Tag as Context.MediaGenre.MediaStyle).Description;

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

						Context.MediaGenre mi = listView2.SelectedItems[0].Tag as Context.MediaGenre;
						mi.Remove( lvi.Tag as Context.MediaGenre.MediaStyle );
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
		#endregion

		private void OnApply( object sender , EventArgs e )
		{
			LogTrace.Label();
			Manager.Instance.AppSetting.SetData( Name , "Audio {Root}" , textBox1.Text );
			Manager.Instance.AppSetting.SetData( Name , "Video {Root}" , textBox2.Text );

			Manager.Instance.AppSetting.SetData( Name , "Use Default Pathname" , radioButton1.Checked ? "True" : "False" );
			Manager.Instance.AppSetting.SetData( Name , "User Pathname" , textBox3.Text );

			Context.HandleMediaGenre.Info.Serialize();
		}

		private void OnCancel( object sender , EventArgs e )
		{
			Context.HandleMediaGenre.LoadFromFile();
		}
	}
}
