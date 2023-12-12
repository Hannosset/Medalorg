using System;

namespace xnet.Context
{
	/// <summary>
	/// Class StatusEventArgs. Prints a message on the status bar The message will be automatically set to empty after 10 secs
	/// </summary>
	public class StatusEventArgs : EventArgs
	{
		public enum StatusEventType { StatusBar, NetworkStatus, Notification };

		#region PUBLIC PROPERTIES
		public string ServerName { get; }
		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <value> The message. </value>
		public string Message { get; }
		/// <summary>
		/// Gets the color of the fore.
		/// </summary>
		/// <value> The color of the fore. </value>
		public System.Drawing.Color ForeColor { get; }
		public System.Windows.Forms.ToolTipIcon TipIcon { get; }
		public string Title { get; }
		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <value> The type. </value>
		public StatusEventType Type { get; set; } = StatusEventType.StatusBar;
		#endregion PUBLIC PROPERTIES

		#region CONSTRUCTOR
		/// <summary>
		/// Initializes a new instance of the <see cref="StatusEventArgs"/> class.
		/// </summary>
		/// <param name="message"> The message. </param>
		/// <param name="error">   if set to <c> true </c> [error]. </param>
		public StatusEventArgs( string message , bool error = false )
		{
			Message = message;
			if( error )
				ForeColor = System.Drawing.Color.Firebrick;
			else
				ForeColor = System.Drawing.SystemColors.ControlText;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="StatusEventArgs"/> class.
		/// </summary>
		/// <param name="message">   The message. </param>
		/// <param name="foreColor"> Color of the fore. </param>
		public StatusEventArgs( string message , System.Drawing.Color foreColor )
		{
			Message = message;
			ForeColor = foreColor;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="StatusEventArgs"/> class.
		/// </summary>
		/// <param name="tipIcon"> The tip icon. </param>
		/// <param name="title">   The title. </param>
		/// <param name="message"> The message. </param>
		public StatusEventArgs( System.Windows.Forms.ToolTipIcon tipIcon , string title , string message )
		{
			Type = StatusEventType.Notification;
			Message = message;
			Title = title;
			TipIcon = tipIcon;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StatusEventArgs"/> class.
		/// </summary>
		/// <param name="serverName">    Name of the server. </param>
		/// <param name="message">       The message. </param>
		/// <param name="foreColor">     Color of the fore. </param>
		/// <param name="networkStatus"> if set to <c> true </c> [network status]. </param>
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
