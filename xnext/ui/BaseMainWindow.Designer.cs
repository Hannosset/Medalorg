namespace xnet.ui
{
	partial class BaseMainWindow
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && (components != null) )
			{
				components.Dispose();
			}
			//	The Designer may throw an unhandled exception
			try
			{
				base.Dispose( disposing );
			}
			catch (System.Exception )
			{
				
			}
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.DateTimeTimer = new System.Windows.Forms.Timer(this.components);
			this.SetReadyTimer = new System.Windows.Forms.Timer(this.components);
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.sbDateTime = new System.Windows.Forms.ToolStripStatusLabel();
			this.sbStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.sbProgressLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.sbProgress = new System.Windows.Forms.ToolStripProgressBar();
			this.sbMode = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsNetworkStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.statusStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// DateTimeTimer
			// 
			this.DateTimeTimer.Enabled = true;
			this.DateTimeTimer.Interval = 1000;
			this.DateTimeTimer.Tick += new System.EventHandler(this.OnTimeTicks);
			// 
			// SetReadyTimer
			// 
			this.SetReadyTimer.Enabled = true;
			this.SetReadyTimer.Interval = 1000;
			this.SetReadyTimer.Tick += new System.EventHandler(this.OnReady);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sbDateTime,
            this.sbStatus,
            this.sbProgressLabel,
            this.sbProgress,
            this.sbMode,
            this.tsNetworkStatus});
			this.statusStrip1.Location = new System.Drawing.Point(0, 426);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.statusStrip1.ShowItemToolTips = true;
			this.statusStrip1.Size = new System.Drawing.Size(800, 24);
			this.statusStrip1.TabIndex = 23;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// sbDateTime
			// 
			this.sbDateTime.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.sbDateTime.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
			this.sbDateTime.Name = "sbDateTime";
			this.sbDateTime.Size = new System.Drawing.Size(122, 19);
			this.sbDateTime.Text = "toolStripStatusLabel1";
			// 
			// sbStatus
			// 
			this.sbStatus.Name = "sbStatus";
			this.sbStatus.Size = new System.Drawing.Size(470, 19);
			this.sbStatus.Spring = true;
			this.sbStatus.Text = "toolStripStatusLabel1";
			this.sbStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.sbStatus.TextChanged += new System.EventHandler(this.OnStatusChanged);
			// 
			// sbProgressLabel
			// 
			this.sbProgressLabel.Name = "sbProgressLabel";
			this.sbProgressLabel.Size = new System.Drawing.Size(10, 19);
			this.sbProgressLabel.Text = " ";
			// 
			// sbProgress
			// 
			this.sbProgress.ForeColor = System.Drawing.Color.Yellow;
			this.sbProgress.Name = "sbProgress";
			this.sbProgress.Size = new System.Drawing.Size(100, 18);
			this.sbProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			// 
			// sbMode
			// 
			this.sbMode.BackColor = System.Drawing.Color.White;
			this.sbMode.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.sbMode.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
			this.sbMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.sbMode.ForeColor = System.Drawing.SystemColors.WindowText;
			this.sbMode.Name = "sbMode";
			this.sbMode.Size = new System.Drawing.Size(50, 19);
			this.sbMode.Text = "Development";
			// 
			// tsNetworkStatus
			// 
			this.tsNetworkStatus.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.tsNetworkStatus.Name = "tsNetworkStatus";
			this.tsNetworkStatus.Size = new System.Drawing.Size(0, 19);
			this.tsNetworkStatus.ToolTipText = "Client connected";
			// 
			// notifyIcon1
			// 
			this.notifyIcon1.Text = "notifyIcon1";
			this.notifyIcon1.Visible = true;
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// BaseMainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.statusStrip1);
			this.Name = "BaseMainWindow";
			this.Text = "BaseMainWindow";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.ToolStripStatusLabel sbDateTime;
		private System.Windows.Forms.ToolStripStatusLabel tsNetworkStatus;
		protected System.Windows.Forms.Timer DateTimeTimer;
		protected System.Windows.Forms.Timer SetReadyTimer;
		protected System.Windows.Forms.StatusStrip statusStrip1;
		protected System.Windows.Forms.NotifyIcon notifyIcon1;
		protected System.Windows.Forms.ToolTip toolTip1;
		protected System.Windows.Forms.ErrorProvider errorProvider1;
		protected System.Windows.Forms.ToolStripStatusLabel sbStatus;
		protected System.Windows.Forms.ToolStripProgressBar sbProgress;
		protected System.Windows.Forms.ToolStripStatusLabel sbProgressLabel;
		protected System.Windows.Forms.ToolStripStatusLabel sbMode;
	}
}