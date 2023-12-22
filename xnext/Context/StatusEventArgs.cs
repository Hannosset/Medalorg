using System;

namespace xnext.Context
{
	/// <summary>
	/// Class StatusEventArgs. Prints a message on the status bar The message will be automatically set to empty after 10 secs
	/// </summary>
	public class StatusEventArgs : EventArgs
	{
		public enum StatusEventType { StatusBar, NetworkStatus, Notification };

		#region PUBLIC PROPERTIES
		public string ServerName { get; }
		public string Message { get; }
		public System.Drawing.Color ForeColor { get; }
		public System.Windows.Forms.ToolTipIcon TipIcon { get; }
		public string Title { get; }
		public StatusEventType Type { get; set; } = StatusEventType.StatusBar;
		#endregion PUBLIC PROPERTIES

		#region CONSTRUCTOR
		public StatusEventArgs( string message , bool error = false )
		{
			Message = message;
			if( error )
				ForeColor = System.Drawing.Color.Firebrick;
			else
				ForeColor = System.Drawing.SystemColors.ControlText;
		}
		public StatusEventArgs( string message , System.Drawing.Color foreColor )
		{
			Message = message;
			ForeColor = foreColor;
		}
		public StatusEventArgs( System.Windows.Forms.ToolTipIcon tipIcon , string title , string message )
		{
			Type = StatusEventType.Notification;
			Message = message;
			Title = title;
			TipIcon = tipIcon;
		}
		public StatusEventArgs( string serverName , string message , System.Drawing.Color foreColor , bool networkStatus = false )
		{
			ServerName = serverName;
			Message = message;
			ForeColor = foreColor;
			Type = networkStatus == true ? StatusEventType.NetworkStatus : StatusEventType.StatusBar;
		}
		#endregion CONSTRUCTOR
	}
}
