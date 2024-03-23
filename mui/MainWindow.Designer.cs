namespace mui
{
	partial class MainWindow
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
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
			this.label1 = new System.Windows.Forms.Label();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.clipboardMonitor1 = new xnext.ui.ClipboardMonitor();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.gotoAudioFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.gotoVideoFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.batchDownloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.downloadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.forgetCheckedVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.button2 = new System.Windows.Forms.Button();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.textBox4 = new System.Windows.Forms.TextBox();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.listView3 = new System.Windows.Forms.ListView();
			this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.imageList2 = new System.Windows.Forms.ImageList(this.components);
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.listView4 = new System.Windows.Forms.ListView();
			this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.listView5 = new System.Windows.Forms.ListView();
			this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.panel1 = new System.Windows.Forms.Panel();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.button3 = new System.Windows.Forms.Button();
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.button1 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.toolStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.contextMenuStrip1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.groupBox4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.toolStripButton2,
            this.toolStripButton3});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.toolStrip1.Size = new System.Drawing.Size(797, 25);
			this.toolStrip1.TabIndex = 24;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButton1
			// 
			this.toolStripButton1.Image = global::mui.Properties.Resources.Settings;
			this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton1.Name = "toolStripButton1";
			this.toolStripButton1.Size = new System.Drawing.Size(101, 22);
			this.toolStripButton1.Text = "Configuration";
			this.toolStripButton1.Click += new System.EventHandler(this.OnConfigure);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButton2
			// 
			this.toolStripButton2.Image = global::mui.Properties.Resources.RefreshDocViewHS;
			this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton2.Name = "toolStripButton2";
			this.toolStripButton2.Size = new System.Drawing.Size(66, 22);
			this.toolStripButton2.Text = "Refresh";
			this.toolStripButton2.Click += new System.EventHandler(this.OnRefreshMediaInfo);
			// 
			// toolStripButton3
			// 
			this.toolStripButton3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton3.Image = global::mui.Properties.Resources.help;
			this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton3.Name = "toolStripButton3";
			this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton3.Text = "toolStripButton3";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(12, 41);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(92, 16);
			this.label1.TabIndex = 25;
			this.label1.Text = "Web address:";
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "");
			this.imageList1.Images.SetKeyName(1, "DOWNLOAD_00.gif");
			this.imageList1.Images.SetKeyName(2, "page_swap_12968.png");
			this.imageList1.Images.SetKeyName(3, "swap_110974.png");
			this.imageList1.Images.SetKeyName(4, "SaveFormDesignHS.png");
			// 
			// clipboardMonitor1
			// 
			this.clipboardMonitor1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.clipboardMonitor1.Location = new System.Drawing.Point(110, 28);
			this.clipboardMonitor1.Name = "clipboardMonitor1";
			this.clipboardMonitor1.Size = new System.Drawing.Size(75, 23);
			this.clipboardMonitor1.TabIndex = 27;
			this.clipboardMonitor1.Text = "clipboardMonitor1";
			this.clipboardMonitor1.Visible = false;
			this.clipboardMonitor1.ClipboardChanged += new System.EventHandler<xnext.ui.ClipboardChangedEventArgs>(this.OnClipboardChanged);
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(110, 40);
			this.textBox1.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(643, 20);
			this.textBox1.TabIndex = 1;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(12, 66);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.listView1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel2);
			this.splitContainer1.Size = new System.Drawing.Size(773, 542);
			this.splitContainer1.SplitterDistance = 337;
			this.splitContainer1.TabIndex = 29;
			this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.OnMainSplitterMoved);
			// 
			// listView1
			// 
			this.listView1.CheckBoxes = true;
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader5,
            this.columnHeader2});
			this.listView1.ContextMenuStrip = this.contextMenuStrip1;
			this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView1.HideSelection = false;
			this.listView1.Location = new System.Drawing.Point(0, 0);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.ShowGroups = false;
			this.listView1.ShowItemToolTips = true;
			this.listView1.Size = new System.Drawing.Size(337, 542);
			this.listView1.TabIndex = 3;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.SelectedIndexChanged += new System.EventHandler(this.OnItemSelected);
			this.listView1.DoubleClick += new System.EventHandler(this.OnOpenVideoOnBrowser);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Title";
			this.columnHeader1.Width = 120;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Status";
			this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Communication";
			this.columnHeader2.Width = 130;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gotoAudioFileToolStripMenuItem,
            this.gotoVideoFileToolStripMenuItem,
            this.toolStripSeparator2,
            this.batchDownloadToolStripMenuItem,
            this.toolStripSeparator4,
            this.filterToolStripMenuItem,
            this.downloadingToolStripMenuItem,
            this.toolStripSeparator3,
            this.forgetCheckedVideoToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(194, 154);
			this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.OnOpenPopup);
			// 
			// gotoAudioFileToolStripMenuItem
			// 
			this.gotoAudioFileToolStripMenuItem.Image = global::mui.Properties.Resources.AudioFile;
			this.gotoAudioFileToolStripMenuItem.Name = "gotoAudioFileToolStripMenuItem";
			this.gotoAudioFileToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			this.gotoAudioFileToolStripMenuItem.Text = "Goto Audio File";
			this.gotoAudioFileToolStripMenuItem.ToolTipText = "Opens the file explorer to the audio directory \r\nwhere the media is located.";
			this.gotoAudioFileToolStripMenuItem.Click += new System.EventHandler(this.gotoAudioFile);
			// 
			// gotoVideoFileToolStripMenuItem
			// 
			this.gotoVideoFileToolStripMenuItem.Image = global::mui.Properties.Resources.VideoCamera;
			this.gotoVideoFileToolStripMenuItem.Name = "gotoVideoFileToolStripMenuItem";
			this.gotoVideoFileToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			this.gotoVideoFileToolStripMenuItem.Text = "Goto Video file";
			this.gotoVideoFileToolStripMenuItem.ToolTipText = "Opens the file explorer to the video directory \r\nwhere the media is located.\r\n";
			this.gotoVideoFileToolStripMenuItem.Click += new System.EventHandler(this.GotoVideoFile);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(190, 6);
			// 
			// batchDownloadToolStripMenuItem
			// 
			this.batchDownloadToolStripMenuItem.Image = global::mui.Properties.Resources.DOWNLOAD_00;
			this.batchDownloadToolStripMenuItem.Name = "batchDownloadToolStripMenuItem";
			this.batchDownloadToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			this.batchDownloadToolStripMenuItem.Text = "Batch Download";
			this.batchDownloadToolStripMenuItem.ToolTipText = "Download all the medi usig default \r\nSettings and using the target path as \r\ncurr" +
    "ently displayed (no author in the path)\r\n";
			this.batchDownloadToolStripMenuItem.Click += new System.EventHandler(this.OnBatchDownload);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(190, 6);
			// 
			// filterToolStripMenuItem
			// 
			this.filterToolStripMenuItem.Image = global::mui.Properties.Resources.Filter2HS;
			this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
			this.filterToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			this.filterToolStripMenuItem.Text = "Filter";
			this.filterToolStripMenuItem.ToolTipText = "Display only the media that haven\'t been downloaded.";
			this.filterToolStripMenuItem.Click += new System.EventHandler(this.OnFilterListview);
			// 
			// downloadingToolStripMenuItem
			// 
			this.downloadingToolStripMenuItem.Image = global::mui.Properties.Resources.Network_ConnectTo;
			this.downloadingToolStripMenuItem.Name = "downloadingToolStripMenuItem";
			this.downloadingToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			this.downloadingToolStripMenuItem.Text = "Downloading";
			this.downloadingToolStripMenuItem.ToolTipText = "Display the media that are currently being downloaded";
			this.downloadingToolStripMenuItem.Click += new System.EventHandler(this.OnDisplayDownloading);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(190, 6);
			// 
			// forgetCheckedVideoToolStripMenuItem
			// 
			this.forgetCheckedVideoToolStripMenuItem.Image = global::mui.Properties.Resources.delete;
			this.forgetCheckedVideoToolStripMenuItem.Name = "forgetCheckedVideoToolStripMenuItem";
			this.forgetCheckedVideoToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			this.forgetCheckedVideoToolStripMenuItem.Text = "Forget Checked Media";
			this.forgetCheckedVideoToolStripMenuItem.ToolTipText = "Remove the check items from the list.\r\nKeep the downloaded media.";
			this.forgetCheckedVideoToolStripMenuItem.Click += new System.EventHandler(this.OnForgetMedia);
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.button2, 0, 3);
			this.tableLayoutPanel2.Controls.Add(this.groupBox4, 0, 2);
			this.tableLayoutPanel2.Controls.Add(this.splitContainer2, 0, 1);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 4;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(432, 542);
			this.tableLayoutPanel2.TabIndex = 21;
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.ColumnCount = 2;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel3.Controls.Add(this.groupBox1, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this.groupBox3, 0, 0);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 1;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(432, 50);
			this.tableLayoutPanel3.TabIndex = 4;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textBox2);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox1.Location = new System.Drawing.Point(219, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this.groupBox1.Size = new System.Drawing.Size(210, 43);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Media Title";
			// 
			// textBox2
			// 
			this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox2.Location = new System.Drawing.Point(3, 16);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(210, 20);
			this.textBox2.TabIndex = 5;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.pictureBox1);
			this.groupBox3.Controls.Add(this.textBox3);
			this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox3.Location = new System.Drawing.Point(3, 3);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Padding = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this.groupBox3.Size = new System.Drawing.Size(210, 43);
			this.groupBox3.TabIndex = 4;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Author";
			this.groupBox3.UseCompatibleTextRendering = true;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox1.Image = global::mui.Properties.Resources.swap_110974;
			this.pictureBox1.Location = new System.Drawing.Point(186, 15);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(23, 22);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Click += new System.EventHandler(this.OnSwapAuthor_Title);
			// 
			// textBox3
			// 
			this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox3.Location = new System.Drawing.Point(3, 16);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(177, 20);
			this.textBox3.TabIndex = 4;
			this.textBox3.TextChanged += new System.EventHandler(this.OnAuthorLabelChanged);
			// 
			// button2
			// 
			this.button2.Dock = System.Windows.Forms.DockStyle.Right;
			this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.button2.ImageIndex = 1;
			this.button2.ImageList = this.imageList1;
			this.button2.Location = new System.Drawing.Point(348, 515);
			this.button2.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(84, 24);
			this.button2.TabIndex = 12;
			this.button2.Text = "Download";
			this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.OnDownload);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.textBox4);
			this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox4.Location = new System.Drawing.Point(3, 465);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Padding = new System.Windows.Forms.Padding(0);
			this.groupBox4.Size = new System.Drawing.Size(426, 44);
			this.groupBox4.TabIndex = 19;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Target Pathname";
			// 
			// textBox4
			// 
			this.textBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox4.Location = new System.Drawing.Point(3, 21);
			this.textBox4.Name = "textBox4";
			this.textBox4.Size = new System.Drawing.Size(420, 20);
			this.textBox4.TabIndex = 11;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 50);
			this.splitContainer2.Margin = new System.Windows.Forms.Padding(0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.listView3);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.tableLayoutPanel1);
			this.splitContainer2.Size = new System.Drawing.Size(432, 412);
			this.splitContainer2.SplitterDistance = 177;
			this.splitContainer2.TabIndex = 23;
			this.splitContainer2.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.OnPanelSplitterMoved);
			// 
			// listView3
			// 
			this.listView3.CheckBoxes = true;
			this.listView3.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8});
			this.listView3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView3.FullRowSelect = true;
			this.listView3.GridLines = true;
			this.listView3.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listView3.HideSelection = false;
			this.listView3.LargeImageList = this.imageList2;
			this.listView3.Location = new System.Drawing.Point(0, 0);
			this.listView3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.listView3.MultiSelect = false;
			this.listView3.Name = "listView3";
			this.listView3.ShowGroups = false;
			this.listView3.ShowItemToolTips = true;
			this.listView3.Size = new System.Drawing.Size(432, 177);
			this.listView3.TabIndex = 6;
			this.listView3.UseCompatibleStateImageBehavior = false;
			this.listView3.SelectedIndexChanged += new System.EventHandler(this.OnUriSelected);
			// 
			// columnHeader7
			// 
			this.columnHeader7.Text = "Video Type";
			this.columnHeader7.Width = 80;
			// 
			// columnHeader8
			// 
			this.columnHeader8.Text = "Resolution";
			this.columnHeader8.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.columnHeader8.Width = 80;
			// 
			// imageList2
			// 
			this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
			this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList2.Images.SetKeyName(0, "Video");
			this.imageList2.Images.SetKeyName(1, "Audio");
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.listView4, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.listView5, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.button3, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(432, 231);
			this.tableLayoutPanel1.TabIndex = 20;
			// 
			// listView4
			// 
			this.listView4.CheckBoxes = true;
			this.listView4.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader9});
			this.listView4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView4.FullRowSelect = true;
			this.listView4.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView4.HideSelection = false;
			this.listView4.Location = new System.Drawing.Point(0, 33);
			this.listView4.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
			this.listView4.MultiSelect = false;
			this.listView4.Name = "listView4";
			this.listView4.ShowItemToolTips = true;
			this.listView4.Size = new System.Drawing.Size(213, 198);
			this.listView4.TabIndex = 9;
			this.listView4.UseCompatibleStateImageBehavior = false;
			this.listView4.View = System.Windows.Forms.View.Details;
			this.listView4.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.OnGenreCheck);
			this.listView4.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.OnGenreChecked);
			this.listView4.SelectedIndexChanged += new System.EventHandler(this.OnGenreSelected);
			// 
			// columnHeader9
			// 
			this.columnHeader9.Text = "Genre";
			this.columnHeader9.Width = 116;
			// 
			// listView5
			// 
			this.listView5.CheckBoxes = true;
			this.listView5.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10});
			this.listView5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView5.FullRowSelect = true;
			this.listView5.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView5.HideSelection = false;
			this.listView5.Location = new System.Drawing.Point(219, 33);
			this.listView5.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
			this.listView5.MultiSelect = false;
			this.listView5.Name = "listView5";
			this.listView5.ShowItemToolTips = true;
			this.listView5.Size = new System.Drawing.Size(213, 198);
			this.listView5.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listView5.TabIndex = 10;
			this.listView5.UseCompatibleStateImageBehavior = false;
			this.listView5.View = System.Windows.Forms.View.Details;
			this.listView5.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.OnStyleCheck);
			this.listView5.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.OnStyleChecked);
			// 
			// columnHeader10
			// 
			this.columnHeader10.Text = "Style";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.radioButton2);
			this.panel1.Controls.Add(this.radioButton1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(3, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(210, 24);
			this.panel1.TabIndex = 7;
			this.panel1.TabStop = true;
			// 
			// radioButton2
			// 
			this.radioButton2.AutoSize = true;
			this.radioButton2.Location = new System.Drawing.Point(61, 3);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(59, 17);
			this.radioButton2.TabIndex = 8;
			this.radioButton2.Text = "Movies";
			this.radioButton2.UseVisualStyleBackColor = true;
			this.radioButton2.CheckedChanged += new System.EventHandler(this.OnVideoGenre);
			// 
			// radioButton1
			// 
			this.radioButton1.AutoSize = true;
			this.radioButton1.Location = new System.Drawing.Point(3, 3);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(53, 17);
			this.radioButton1.TabIndex = 7;
			this.radioButton1.TabStop = true;
			this.radioButton1.Text = "Music";
			this.radioButton1.UseVisualStyleBackColor = true;
			this.radioButton1.CheckedChanged += new System.EventHandler(this.OnAudioGenre);
			// 
			// button3
			// 
			this.button3.Dock = System.Windows.Forms.DockStyle.Right;
			this.button3.ImageIndex = 4;
			this.button3.ImageList = this.imageList1;
			this.button3.Location = new System.Drawing.Point(389, 3);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(40, 24);
			this.button3.TabIndex = 11;
			this.toolTip1.SetToolTip(this.button3, "Save the author, title, genre and style selection.\r\n");
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.OnSaveMediaParam);
			// 
			// backgroundWorker1
			// 
			this.backgroundWorker1.WorkerReportsProgress = true;
			this.backgroundWorker1.WorkerSupportsCancellation = true;
			this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.OnProcessBatch);
			this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.OnBatchProgress);
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.button1.ImageIndex = 0;
			this.button1.ImageList = this.imageList1;
			this.button1.Location = new System.Drawing.Point(753, 39);
			this.button1.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(32, 21);
			this.button1.TabIndex = 2;
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.OnAddWebVideo);
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(797, 635);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.clipboardMonitor1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.toolStrip1);
			this.DoubleBuffered = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(600, 400);
			this.Name = "MainWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Medalorg GUI";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.Controls.SetChildIndex(this.toolStrip1, 0);
			this.Controls.SetChildIndex(this.label1, 0);
			this.Controls.SetChildIndex(this.button1, 0);
			this.Controls.SetChildIndex(this.clipboardMonitor1, 0);
			this.Controls.SetChildIndex(this.textBox1, 0);
			this.Controls.SetChildIndex(this.splitContainer1, 0);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.contextMenuStrip1.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel3.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton toolStripButton2;
		private System.Windows.Forms.ToolStripButton toolStripButton3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ImageList imageList1;
		private xnext.ui.ClipboardMonitor clipboardMonitor1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ListView listView3;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.ColumnHeader columnHeader8;
		private System.Windows.Forms.ListView listView4;
		private System.Windows.Forms.ColumnHeader columnHeader9;
		private System.Windows.Forms.ListView listView5;
		private System.Windows.Forms.ColumnHeader columnHeader10;
		private System.Windows.Forms.ImageList imageList2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.TextBox textBox4;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem gotoAudioFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem gotoVideoFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem filterToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem downloadingToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem forgetCheckedVideoToolStripMenuItem;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.ToolStripMenuItem batchDownloadToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
		private System.Windows.Forms.Button button3;
	}
}

