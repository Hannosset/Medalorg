﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using xnext.Context;
using xnext.Diagnostics;

namespace xnext.ui
{
	public partial class BaseMainWindow : Form
	{
		#region TYPES
		protected delegate void LogMessageMethodDelegate( StatusEventArgs e );
		#endregion TYPES

		#region LOCAL VARIABLE
		/// <summary>Gets the identifier.</summary>
		/// <value>The identifier.</value>
		public virtual string Identifier { get; } = string.Empty;
		/// <summary>Gets the protocol version.</summary>
		/// <value>The protocol version.</value>
		public virtual string ProtocolVersion { get; } = string.Empty;
		protected bool IsClosing { get; private set; }
		#endregion LOCAL VARIABLE

		#region PROPERTIES
		protected virtual string WindowTitle
		{
			get
			{
				StringBuilder AssemblyWindowTitle = new StringBuilder();

				_ = AssemblyWindowTitle.Append( Identifier );

				AssemblyFileVersionAttribute fileAttr = Attribute.GetCustomAttribute( Assembly.GetEntryAssembly() , typeof( AssemblyFileVersionAttribute ) ) as AssemblyFileVersionAttribute;
				_ = AssemblyWindowTitle.AppendFormat( CultureInfo.CurrentCulture , " - V{0}" , fileAttr.Version );

				if( Attribute.GetCustomAttribute( Assembly.GetEntryAssembly() , typeof( AssemblyConfigurationAttribute ) ) is AssemblyConfigurationAttribute ConfigAttr && !string.IsNullOrEmpty( ConfigAttr.Configuration ) )
					_ = AssemblyWindowTitle.AppendFormat( CultureInfo.CurrentCulture , " - {0}" , ConfigAttr.Configuration );

				if( !string.IsNullOrEmpty( ProtocolVersion ) )
					_ = AssemblyWindowTitle.AppendFormat( CultureInfo.CurrentCulture , " - [Protocol V{0}]" , ProtocolVersion );

				return AssemblyWindowTitle.ToString();
			}
		}
		#endregion PROPERTIES

		#region CONSTRUCTOR

		public BaseMainWindow()
		{
			if( LicenseManager.UsageMode == LicenseUsageMode.Runtime )
			{
				CltWinEnv.SetupEnvironment( Identifier );
			}

			InitializeComponent();

			if( LicenseManager.UsageMode == LicenseUsageMode.Runtime )
			{
				sbProgress.Visible = false;
				sbProgressLabel.Visible = false;
				sbStatus.Text = "Loading Configuration...";
#if DEBUG
				sbMode.Text = "Debug";
#else
				sbMode.Text = "Release";
#endif
			}
		}
		private void OnFormLoad( object sender , EventArgs e )
		{
			if( LicenseManager.UsageMode == LicenseUsageMode.Runtime )
			{
				try
				{
					Text = WindowTitle;

					CltWinEnv.UserReadSetting.Load( this );

					sbMode.ForeColor = SystemColors.WindowText;
					sbMode.Text = "";

					DateTimeTimer.Start();
					OnReady( sender , e );
				}
				catch( System.Exception ex )
				{
					Logger.TraceException( ex , $"The Control Panel will not display correctly" , "Close the client application, confirmed not running from task manager, restart client, collect data and report issue on Telegram or GitHub" );
				}
			}
		}
		private void OnFormClosing( object sender , FormClosingEventArgs e )
		{
			if( LicenseManager.UsageMode == LicenseUsageMode.Runtime )
			{
				DateTimeTimer.Stop();
				try
				{
					CltWinEnv.UserSetting.Save( this );
				}
				catch( System.Exception ex )
				{
					Logger.TraceException( ex , "Windows position x,y and w,h not saved" , "End-user will not have the GUI windows positioned the same way it was closed." );
				}
				IsClosing = true;
				ControlPanelClosingEvent?.Invoke( this , new ControlPanelClosingEventArgs { Identifier = Identifier } );
			}
		}
#endregion CONSTRUCTOR

		public event EventHandler<ControlPanelClosingEventArgs> ControlPanelClosingEvent;

		#region MESSAGE EVENTS
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Design" , "CA1051:Do not declare visible instance fields" , Justification = "In this case we should" )]
		protected LogMessageMethodDelegate DelegateLogMessage;

		/// <summary>
		///  Handles the event.
		///  What: logs a message on the status bar and in the log event
		///  Why: some messages are generated by thread, this functions automatically invoke the control created thread to display the information.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="StatusEventArgs"/> instance containing the event data.</param>
		/// <exception cref="ArgumentNullException">e</exception>
		protected void OnLogMessage( object sender , StatusEventArgs e )
		{
			if( e is null )
				throw new ArgumentNullException( nameof( e ) );
			if( LicenseManager.UsageMode == LicenseUsageMode.Runtime )
			{
				try
				{
					if( !IsClosing )
					{
						if( statusStrip1.InvokeRequired )
						{
							if( DelegateLogMessage == null )
								DelegateLogMessage = new LogMessageMethodDelegate( LogMessage );
							statusStrip1.Invoke( DelegateLogMessage , e );
						}
						else
							LogMessage( e );
					}
				}
				catch( System.Exception ex )
				{
					Logger.TraceWarning( $"exception when logging messages on the control Panel - {ex.Message}" , "User might not be informed of all events" , "Restart client" );
				}
			}
		}
		private void LogMessage( StatusEventArgs e )
		{
			sbStatus.ForeColor = e.ForeColor;

			if( e.Message.Contains( "\r" ) )
			{
				sbStatus.Text = e.Message.Substring( 0 , e.Message.IndexOf( '\r' ) - 1 );
				sbStatus.ToolTipText = e.Message;
			}
			else if( e.Type == StatusEventArgs.StatusEventType.StatusBar )
				sbStatus.ToolTipText = sbStatus.Text = e.Message;

			if( e.Type == StatusEventArgs.StatusEventType.Notification )
			{
				notifyIcon1.BalloonTipIcon = e.TipIcon;
				notifyIcon1.BalloonTipTitle = e.Title;
				notifyIcon1.BalloonTipText = e.Message;
				notifyIcon1.ShowBalloonTip( 60 * 1000 );
			}
			else if( !e.Message.Contains( "\n" ) )
			{
				sbStatus.ForeColor = e.ForeColor;
				if( e.ForeColor == Color.Red )
					Logger.TraceError( e.Message , null , null );
			}
		}
		#endregion MESSAGE EVENTS

		#region INTERNAL EVENTS
		/// <summary>
		///  What: every seconds, updates the time on the status bar
		///  Why: Offer the end-user with a clock - shuts the application every Saturday midnight. Reset the validate calendar every UTC new day and morning 6am
		///  local time.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnTimeTicks( object sender , EventArgs e )
		{
			if( sender is null )
				throw new ArgumentNullException( nameof( sender ) );
			if( LicenseManager.UsageMode == LicenseUsageMode.Runtime && !IsClosing && sender is Timer tm )
			{
				try
				{
					tm.Interval = 1000;

					sbDateTime.Text = DateTime.Now.ToLongDateString() + DateTime.Now.ToString( " HH:mm:ss" , CultureInfo.CurrentCulture );
				}
				catch( System.Exception ex )
				{
					Logger.TraceWarning( ex.Message );
				}
			}
		}
		/// <summary>
		///  What: Triggered by the SetReadyTimer when the status bar text has been changed
		///  Why: Restore the Ready text after a while
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnReady( object sender , EventArgs e )
		{
			if( LicenseManager.UsageMode == LicenseUsageMode.Runtime && !IsClosing )
			{
				try
				{
					SetReadyTimer.Stop();
					sbStatus.Text = "Ready...";
					sbStatus.ForeColor = SystemColors.WindowText;
				}
				catch( System.Exception ex )
				{
					Logger.TraceWarning( ex.Message );
				}
			}
		}
		/// <summary>
		///  When the status bar test is changed, we start the SetReadyTimer to write "ready... " on the status bar Start the timer if the test is not the
		///  "Ready..." one :)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnStatusChanged( object sender , EventArgs e )
		{
			if( LicenseManager.UsageMode == LicenseUsageMode.Runtime && !IsClosing )
			{
				try
				{
					SetReadyTimer.Stop();
					// sbStatus.Text = sbStatus.Text.Replace( "\n" , " " );
					if( sbStatus.Text != "Ready..." )
						SetReadyTimer.Start();
				}
				catch( System.Exception ex )
				{
					Logger.TraceWarning( ex.Message );
				}
			}
		}
		#endregion INTERNAL EVENTS

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Design" , "CA1034:Nested types should not be visible" , Justification = "Yes in this condition the message is only from the BaseMainWindow object" )]
		public class ControlPanelClosingEventArgs : EventArgs
		{
			public string Identifier { get; internal set; }
		}
	}
}
