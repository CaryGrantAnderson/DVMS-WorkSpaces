namespace DVMSFileManager
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.propertyGridControl_Settings = new DevExpress.XtraVerticalGrid.PropertyGridControl();
            this.panelControl_Bottom = new DevExpress.XtraEditors.PanelControl();
            this.simpleButton_Defaults = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_BrowseHome = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_DefaultTheme = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_Cancel = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_Apply = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_OK = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl_Database = new DevExpress.XtraEditors.PanelControl();
            this.labelControl_DatabaseCaption = new DevExpress.XtraEditors.LabelControl();
            this.labelControl_DatabasePath = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.propertyGridControl_Settings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_Bottom)).BeginInit();
            this.panelControl_Bottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_Database)).BeginInit();
            this.panelControl_Database.SuspendLayout();
            this.SuspendLayout();
            // propertyGridControl_Settings
            this.propertyGridControl_Settings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridControl_Settings.Location = new System.Drawing.Point(0, 42);
            this.propertyGridControl_Settings.Name = "propertyGridControl_Settings";
            this.propertyGridControl_Settings.Size = new System.Drawing.Size(760, 470);
            this.propertyGridControl_Settings.TabIndex = 0;
            // panelControl_Database
            this.panelControl_Database.Controls.Add(this.labelControl_DatabasePath);
            this.panelControl_Database.Controls.Add(this.labelControl_DatabaseCaption);
            this.panelControl_Database.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl_Database.Location = new System.Drawing.Point(0, 0);
            this.panelControl_Database.Name = "panelControl_Database";
            this.panelControl_Database.Size = new System.Drawing.Size(760, 42);
            this.panelControl_Database.TabIndex = 1;
            this.labelControl_DatabaseCaption.Location = new System.Drawing.Point(12, 13);
            this.labelControl_DatabaseCaption.Name = "labelControl_DatabaseCaption";
            this.labelControl_DatabaseCaption.Text = "Settings database:";
            this.labelControl_DatabasePath.AutoEllipsis = true;
            this.labelControl_DatabasePath.Location = new System.Drawing.Point(132, 13);
            this.labelControl_DatabasePath.Name = "labelControl_DatabasePath";
            this.labelControl_DatabasePath.Size = new System.Drawing.Size(614, 16);
            // panelControl_Bottom
            this.panelControl_Bottom.Controls.Add(this.simpleButton_Defaults);
            this.panelControl_Bottom.Controls.Add(this.simpleButton_BrowseHome);
            this.panelControl_Bottom.Controls.Add(this.simpleButton_DefaultTheme);
            this.panelControl_Bottom.Controls.Add(this.simpleButton_Cancel);
            this.panelControl_Bottom.Controls.Add(this.simpleButton_Apply);
            this.panelControl_Bottom.Controls.Add(this.simpleButton_OK);
            this.panelControl_Bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl_Bottom.Location = new System.Drawing.Point(0, 512);
            this.panelControl_Bottom.Name = "panelControl_Bottom";
            this.panelControl_Bottom.Size = new System.Drawing.Size(760, 54);
            this.panelControl_Bottom.TabIndex = 2;
            this.simpleButton_Defaults.Location = new System.Drawing.Point(12, 13);
            this.simpleButton_Defaults.Name = "simpleButton_Defaults";
            this.simpleButton_Defaults.Size = new System.Drawing.Size(110, 28);
            this.simpleButton_Defaults.Text = "Restore Defaults";
            this.simpleButton_Defaults.Click += new System.EventHandler(this.simpleButton_Defaults_Click);
            this.simpleButton_BrowseHome.Location = new System.Drawing.Point(132, 13);
            this.simpleButton_BrowseHome.Name = "simpleButton_BrowseHome";
            this.simpleButton_BrowseHome.Size = new System.Drawing.Size(120, 28);
            this.simpleButton_BrowseHome.Text = "Browse Home...";
            this.simpleButton_BrowseHome.Click += new System.EventHandler(this.simpleButton_BrowseHome_Click);
            this.simpleButton_DefaultTheme.Location = new System.Drawing.Point(262, 13);
            this.simpleButton_DefaultTheme.Name = "simpleButton_DefaultTheme";
            this.simpleButton_DefaultTheme.Size = new System.Drawing.Size(112, 28);
            this.simpleButton_DefaultTheme.Text = "Default Theme";
            this.simpleButton_DefaultTheme.Click += new System.EventHandler(this.simpleButton_DefaultTheme_Click);
            this.simpleButton_OK.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.simpleButton_OK.Location = new System.Drawing.Point(476, 13);
            this.simpleButton_OK.Name = "simpleButton_OK";
            this.simpleButton_OK.Size = new System.Drawing.Size(82, 28);
            this.simpleButton_OK.Text = "OK";
            this.simpleButton_OK.Click += new System.EventHandler(this.simpleButton_OK_Click);
            this.simpleButton_Apply.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.simpleButton_Apply.Location = new System.Drawing.Point(570, 13);
            this.simpleButton_Apply.Name = "simpleButton_Apply";
            this.simpleButton_Apply.Size = new System.Drawing.Size(82, 28);
            this.simpleButton_Apply.Text = "Apply";
            this.simpleButton_Apply.Click += new System.EventHandler(this.simpleButton_Apply_Click);
            this.simpleButton_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.simpleButton_Cancel.Location = new System.Drawing.Point(664, 13);
            this.simpleButton_Cancel.Name = "simpleButton_Cancel";
            this.simpleButton_Cancel.Size = new System.Drawing.Size(82, 28);
            this.simpleButton_Cancel.Text = "Cancel";
            this.simpleButton_Cancel.Click += new System.EventHandler(this.simpleButton_Cancel_Click);
            // SettingsForm
            this.AcceptButton = this.simpleButton_OK;
            this.CancelButton = this.simpleButton_Cancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 566);
            this.Controls.Add(this.propertyGridControl_Settings);
            this.Controls.Add(this.panelControl_Database);
            this.Controls.Add(this.panelControl_Bottom);
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DVMS File Manager Settings";
            ((System.ComponentModel.ISupportInitialize)(this.propertyGridControl_Settings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_Bottom)).EndInit();
            this.panelControl_Bottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_Database)).EndInit();
            this.panelControl_Database.ResumeLayout(false);
            this.panelControl_Database.PerformLayout();
            this.ResumeLayout(false);
        }

        private DevExpress.XtraVerticalGrid.PropertyGridControl propertyGridControl_Settings;
        private DevExpress.XtraEditors.PanelControl panelControl_Bottom;
        private DevExpress.XtraEditors.PanelControl panelControl_Database;
        private DevExpress.XtraEditors.LabelControl labelControl_DatabaseCaption;
        private DevExpress.XtraEditors.LabelControl labelControl_DatabasePath;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Defaults;
        private DevExpress.XtraEditors.SimpleButton simpleButton_BrowseHome;
        private DevExpress.XtraEditors.SimpleButton simpleButton_DefaultTheme;
        private DevExpress.XtraEditors.SimpleButton simpleButton_OK;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Apply;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Cancel;
    }
}
