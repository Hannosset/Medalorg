using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using xnet.Diagnostics;

namespace xnext.ui
{
	// Must inherit Control, not Component, in order to have Handle
	//	https://stackoverflow.com/questions/621577/how-do-i-monitor-clipboard-changes-in-c
	[DefaultEvent( "ClipboardChanged" )]
	public partial class ClipboardMonitor : Control
	{
		#region LOCAL VARIABLE
		IntPtr nextClipboardViewer;
		#endregion

		#region LOCAL VARIABLE
		public ClipboardMonitor()
		{
			nextClipboardViewer = (IntPtr)SetClipboardViewer( (int)Handle );
			Visible = false;
		}
		#endregion

		#region WINDOWS EVENTS
		/// <summary>
		/// Clipboard contents changed.
		/// </summary>
		public event EventHandler<ClipboardChangedEventArgs> ClipboardChanged;
		#endregion

		#region LOCAL METHODS
		[DllImport( "User32.dll" )]
		protected static extern int SetClipboardViewer( int hWndNewViewer );

		[DllImport( "User32.dll" , CharSet = CharSet.Auto )]
		public static extern bool ChangeClipboardChain( IntPtr hWndRemove , IntPtr hWndNewNext );

		[DllImport( "user32.dll" , CharSet = CharSet.Auto )]
		public static extern int SendMessage( IntPtr hwnd , int wMsg , IntPtr wParam , IntPtr lParam );

		protected override void WndProc( ref System.Windows.Forms.Message m )
		{
			// defined in winuser.h
			const int WM_DRAWCLIPBOARD = 0x308;
			const int WM_CHANGECBCHAIN = 0x030D;

			switch( m.Msg )
			{
				case WM_DRAWCLIPBOARD:
					OnClipboardChanged();
					SendMessage( nextClipboardViewer , m.Msg , m.WParam , m.LParam );
					break;

				case WM_CHANGECBCHAIN:
					if( m.WParam == nextClipboardViewer )
						nextClipboardViewer = m.LParam;
					else
						SendMessage( nextClipboardViewer , m.Msg , m.WParam , m.LParam );
					break;

				default:
					base.WndProc( ref m );
					break;
			}
		}

		void OnClipboardChanged()
		{
			try
			{
				ClipboardChanged?.Invoke( this , new ClipboardChangedEventArgs( Clipboard.GetDataObject() ) );

			}
			catch( Exception ex )
			{
				Logger.TraceException( ex , "the clipboard will not be correctly transfered to the monitoring application" , "copy/paste again." );
			}
		}
		#endregion

		#region DISPOSE
		protected override void Dispose( bool disposing )
		{
			ChangeClipboardChain( Handle , nextClipboardViewer );
		}
		#endregion
	}

	public class ClipboardChangedEventArgs : EventArgs
	{
		public readonly IDataObject DataObject;

		public string Text
		{
			get
			{
				if( DataObject.GetData( "System.String" , false ) != null )
					return DataObject.GetData( "System.String" , false ).ToString();
				else
					return DataObject.GetData( "System.String" , true ).ToString();
			}
		}

		public ClipboardChangedEventArgs( IDataObject dataObject )
		{
			DataObject = dataObject;
		}
	}
}
