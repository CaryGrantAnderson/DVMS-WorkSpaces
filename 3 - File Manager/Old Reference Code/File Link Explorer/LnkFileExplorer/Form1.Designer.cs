namespace LnkFileExplorer
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.openFile = new System.Windows.Forms.ToolStripButton();
            this.saveReport = new System.Windows.Forms.ToolStripButton();
            this.editJustSavedReport = new System.Windows.Forms.ToolStripButton();
            this.reportEdit = new System.Windows.Forms.ToolStripButton();
            this.callHexEdit = new System.Windows.Forms.ToolStripButton();
            this.RunLink = new System.Windows.Forms.ToolStripButton();
            this.startInternet = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.pdfFile = new System.Windows.Forms.ToolStripButton();
            this.ExplorerInfo = new System.Windows.Forms.ToolStripButton();
            this.ExplorerExit = new System.Windows.Forms.ToolStripButton();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog3 = new System.Windows.Forms.OpenFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.desktopLinksButton = new System.Windows.Forms.ToolStripButton();
            this.startMenuButton = new System.Windows.Forms.ToolStripButton();
            this.userLinkButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.folderBrowserDialog2 = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DereferenceLinks = false;
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Link files (*.lnk)|*.lnk|All files (*.*)|*.*";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dataGridView1.Location = new System.Drawing.Point(201, 29);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 25;
            this.dataGridView1.Size = new System.Drawing.Size(548, 793);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView1_CellPainting);
            this.dataGridView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Tables";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 200;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Info";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 400;
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1});
            this.dataGridView2.Location = new System.Drawing.Point(755, 29);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.RowHeadersWidth = 25;
            this.dataGridView2.Size = new System.Drawing.Size(367, 793);
            this.dataGridView2.TabIndex = 2;
            this.dataGridView2.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView2_CellPainting);
            this.dataGridView2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Table Entries";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 400;
            // 
            // dataGridView3
            // 
            this.dataGridView3.AllowUserToDeleteRows = false;
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2});
            this.dataGridView3.Location = new System.Drawing.Point(1128, 29);
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.ReadOnly = true;
            this.dataGridView3.RowHeadersWidth = 25;
            this.dataGridView3.Size = new System.Drawing.Size(367, 793);
            this.dataGridView3.TabIndex = 3;
            this.dataGridView3.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView3_CellPainting);
            this.dataGridView3.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Extra Table Entries";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 400;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFile,
            this.saveReport,
            this.editJustSavedReport,
            this.reportEdit,
            this.callHexEdit,
            this.RunLink,
            this.startInternet,
            this.toolStripSeparator1,
            this.pdfFile,
            this.ExplorerInfo,
            this.ExplorerExit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1505, 25);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // openFile
            // 
            this.openFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openFile.Image = ((System.Drawing.Image)(resources.GetObject("openFile.Image")));
            this.openFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openFile.Name = "openFile";
            this.openFile.Size = new System.Drawing.Size(23, 22);
            this.openFile.Text = "Open Link File <CTRL-O> or <F2>";
            this.openFile.Click += new System.EventHandler(this.openFile_Click);
            // 
            // saveReport
            // 
            this.saveReport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveReport.Image = ((System.Drawing.Image)(resources.GetObject("saveReport.Image")));
            this.saveReport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveReport.Name = "saveReport";
            this.saveReport.Size = new System.Drawing.Size(23, 22);
            this.saveReport.Text = "Save Link Report <CTRL-S>";
            this.saveReport.Click += new System.EventHandler(this.saveReport_Click);
            // 
            // editJustSavedReport
            // 
            this.editJustSavedReport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.editJustSavedReport.Image = ((System.Drawing.Image)(resources.GetObject("editJustSavedReport.Image")));
            this.editJustSavedReport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editJustSavedReport.Name = "editJustSavedReport";
            this.editJustSavedReport.Size = new System.Drawing.Size(23, 22);
            this.editJustSavedReport.Text = "Edit just saved Report <CTRL-E>";
            this.editJustSavedReport.Click += new System.EventHandler(this.editJustSavedReport_Click);
            // 
            // reportEdit
            // 
            this.reportEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.reportEdit.Image = ((System.Drawing.Image)(resources.GetObject("reportEdit.Image")));
            this.reportEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.reportEdit.Name = "reportEdit";
            this.reportEdit.Size = new System.Drawing.Size(23, 22);
            this.reportEdit.Text = "Edit a Report  <CTRL-R>";
            this.reportEdit.Click += new System.EventHandler(this.reportEdit_Click);
            // 
            // callHexEdit
            // 
            this.callHexEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.callHexEdit.Image = ((System.Drawing.Image)(resources.GetObject("callHexEdit.Image")));
            this.callHexEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.callHexEdit.Name = "callHexEdit";
            this.callHexEdit.Size = new System.Drawing.Size(23, 22);
            this.callHexEdit.Text = "View in HexEditor <CTRL-H>";
            this.callHexEdit.Click += new System.EventHandler(this.runHexEdit_Click);
            // 
            // RunLink
            // 
            this.RunLink.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RunLink.Image = ((System.Drawing.Image)(resources.GetObject("RunLink.Image")));
            this.RunLink.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RunLink.Name = "RunLink";
            this.RunLink.Size = new System.Drawing.Size(23, 22);
            this.RunLink.Text = "Run the Link <CTRL-X>";
            this.RunLink.Click += new System.EventHandler(this.executeLink_Click);
            // 
            // startInternet
            // 
            this.startInternet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.startInternet.Image = ((System.Drawing.Image)(resources.GetObject("startInternet.Image")));
            this.startInternet.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.startInternet.Name = "startInternet";
            this.startInternet.Size = new System.Drawing.Size(23, 22);
            this.startInternet.Text = "Start Internet with Links Info <CTRL-I>";
            this.startInternet.Click += new System.EventHandler(this.startInternet_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // pdfFile
            // 
            this.pdfFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pdfFile.Image = ((System.Drawing.Image)(resources.GetObject("pdfFile.Image")));
            this.pdfFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pdfFile.Name = "pdfFile";
            this.pdfFile.Size = new System.Drawing.Size(23, 22);
            this.pdfFile.Text = "Open a PDF File";
            this.pdfFile.Click += new System.EventHandler(this.pdfFile_Click);
            // 
            // ExplorerInfo
            // 
            this.ExplorerInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ExplorerInfo.Image = ((System.Drawing.Image)(resources.GetObject("ExplorerInfo.Image")));
            this.ExplorerInfo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ExplorerInfo.Name = "ExplorerInfo";
            this.ExplorerInfo.Size = new System.Drawing.Size(23, 22);
            this.ExplorerInfo.Text = "About <F1>";
            this.ExplorerInfo.Click += new System.EventHandler(this.help_Click);
            // 
            // ExplorerExit
            // 
            this.ExplorerExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ExplorerExit.Image = ((System.Drawing.Image)(resources.GetObject("ExplorerExit.Image")));
            this.ExplorerExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ExplorerExit.Name = "ExplorerExit";
            this.ExplorerExit.Size = new System.Drawing.Size(23, 22);
            this.ExplorerExit.Text = "Exit Link File Explorer <CTRL-Q>";
            this.ExplorerExit.Click += new System.EventHandler(this.exit_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "txt";
            this.saveFileDialog1.Filter = "Text files (*.txt)|*.txt";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Window;
            this.textBox1.Location = new System.Drawing.Point(563, 29);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(460, 436);
            this.textBox1.TabIndex = 7;
            this.textBox1.Visible = false;
            this.textBox1.WordWrap = false;
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.DefaultExt = "txt";
            this.openFileDialog2.Filter = "Text files (*.txt)|*.txt";
            // 
            // openFileDialog3
            // 
            this.openFileDialog3.DefaultExt = "txt";
            this.openFileDialog3.Filter = "PDF files (*.pdf)|*.pdf|Word files (*.doc)|*.doc|All files (*.*)|*.*";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.treeView1);
            this.panel1.Controls.Add(this.toolStrip2);
            this.panel1.Location = new System.Drawing.Point(5, 29);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(190, 793);
            this.panel1.TabIndex = 10;
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.FloralWhite;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.FullRowSelect = true;
            this.treeView1.HotTracking = true;
            this.treeView1.Location = new System.Drawing.Point(0, 25);
            this.treeView1.Name = "treeView1";
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(188, 766);
            this.treeView1.TabIndex = 2;
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.desktopLinksButton,
            this.startMenuButton,
            this.userLinkButton,
            this.toolStripSeparator2,
            this.toolStripLabel1});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(188, 25);
            this.toolStrip2.TabIndex = 1;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // desktopLinksButton
            // 
            this.desktopLinksButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.desktopLinksButton.Image = ((System.Drawing.Image)(resources.GetObject("desktopLinksButton.Image")));
            this.desktopLinksButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.desktopLinksButton.Name = "desktopLinksButton";
            this.desktopLinksButton.Size = new System.Drawing.Size(23, 22);
            this.desktopLinksButton.Text = "Show Desktop Links";
            this.desktopLinksButton.Click += new System.EventHandler(this.LinkTreeDirectory_Click);
            // 
            // startMenuButton
            // 
            this.startMenuButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.startMenuButton.Image = ((System.Drawing.Image)(resources.GetObject("startMenuButton.Image")));
            this.startMenuButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.startMenuButton.Name = "startMenuButton";
            this.startMenuButton.Size = new System.Drawing.Size(23, 22);
            this.startMenuButton.Text = "Show StartMenu Links";
            this.startMenuButton.Click += new System.EventHandler(this.startMenuButton_Click);
            // 
            // userLinkButton
            // 
            this.userLinkButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.userLinkButton.Image = ((System.Drawing.Image)(resources.GetObject("userLinkButton.Image")));
            this.userLinkButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.userLinkButton.Name = "userLinkButton";
            this.userLinkButton.Size = new System.Drawing.Size(23, 22);
            this.userLinkButton.Text = "Show user selected Links";
            this.userLinkButton.Click += new System.EventHandler(this.userLinkButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.ForeColor = System.Drawing.Color.Blue;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(80, 22);
            this.toolStripLabel1.Text = "Desktop Links";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMargin = new System.Drawing.Size(10, 10);
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Moccasin;
            this.ClientSize = new System.Drawing.Size(1495, 849);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.dataGridView3);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1048, 38);
            this.Name = "Form1";
            this.Text = "Link File Explorer:  By Hubert Gut";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridView dataGridView3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton openFile;
        private System.Windows.Forms.ToolStripButton saveReport;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton ExplorerExit;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripButton callHexEdit;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStripButton ExplorerInfo;
        private System.Windows.Forms.ToolStripButton RunLink;
        private System.Windows.Forms.ToolStripButton reportEdit;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.ToolStripButton editJustSavedReport;
        private System.Windows.Forms.ToolStripButton startInternet;
        private System.Windows.Forms.ToolStripButton pdfFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton desktopLinksButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ToolStripButton startMenuButton;
        private System.Windows.Forms.ToolStripButton userLinkButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog2;
    }
}

