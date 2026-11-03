namespace DVMSVirtualFolders
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Docking.DockManager dockManager;
        private DevExpress.XtraBars.Docking.DockPanel previewPanel;
        private DevExpress.XtraBars.Docking.ControlContainer previewContainer;
        private DevExpress.XtraEditors.PanelControl previewHostPanel;
        private DevExpress.XtraTreeList.TreeList tree;
        //private DevExpress.XtraDialogs.FileExplorerExtensions.PreviewPanelExtension previewExtension;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.dockManager = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.previewPanel = new DevExpress.XtraBars.Docking.DockPanel();
            this.previewContainer = new DevExpress.XtraBars.Docking.ControlContainer();
            this.previewHostPanel = new DevExpress.XtraEditors.PanelControl();
            this.tree = new DevExpress.XtraTreeList.TreeList();
            this.fileExplorerAssistant1 = new DevExpress.XtraDialogs.FileExplorerAssistant(this.components);
            this.previewPanelExtension1 = new DevExpress.XtraDialogs.FileExplorerExtensions.PreviewPanelExtension();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager)).BeginInit();
            this.previewPanel.SuspendLayout();
            this.previewContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewHostPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tree)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileExplorerAssistant1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.previewPanelExtension1)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.EmptyAreaImageOptions.ImagePadding = new System.Windows.Forms.Padding(30, 32, 30, 32);
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 1;
            this.ribbon.Name = "ribbon";
            this.ribbon.Size = new System.Drawing.Size(1400, 71);
            // 
            // dockManager
            // 
            this.dockManager.Form = this;
            this.dockManager.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.previewPanel});
            this.dockManager.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.Ribbon.RibbonControl",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar"});
            // 
            // previewPanel
            // 
            this.previewPanel.Controls.Add(this.previewContainer);
            this.previewPanel.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
            this.previewPanel.ID = new System.Guid("86f13bb5-f480-43c3-87fd-f7387f912cc2");
            this.previewPanel.Location = new System.Drawing.Point(862, 71);
            this.previewPanel.Name = "previewPanel";
            this.previewPanel.OriginalSize = new System.Drawing.Size(430, 200);
            this.previewPanel.Size = new System.Drawing.Size(538, 782);
            this.previewPanel.Text = "Preview";
            // 
            // previewContainer
            // 
            this.previewContainer.Controls.Add(this.previewHostPanel);
            this.previewContainer.Location = new System.Drawing.Point(6, 32);
            this.previewContainer.Name = "previewContainer";
            this.previewContainer.Size = new System.Drawing.Size(528, 746);
            this.previewContainer.TabIndex = 0;
            // 
            // previewHostPanel
            // 
            this.previewHostPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.previewHostPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewHostPanel.Location = new System.Drawing.Point(0, 0);
            this.previewHostPanel.Name = "previewHostPanel";
            this.previewHostPanel.Size = new System.Drawing.Size(528, 746);
            this.previewHostPanel.TabIndex = 0;
            // 
            // tree
            // 
            this.tree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tree.Location = new System.Drawing.Point(0, 71);
            this.tree.Name = "tree";
            this.tree.Size = new System.Drawing.Size(862, 782);
            this.tree.TabIndex = 0;
            // 
            // fileExplorerAssistant1
            // 
            this.fileExplorerAssistant1.Extensions.Add(this.previewPanelExtension1);
            this.fileExplorerAssistant1.Form = this;
            // 
            // previewPanelExtension1
            // 
            this.previewPanelExtension1.Control = this.previewHostPanel;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 853);
            this.Controls.Add(this.tree);
            this.Controls.Add(this.previewPanel);
            this.Controls.Add(this.ribbon);
            this.MinimumSize = new System.Drawing.Size(900, 587);
            this.Name = "MainForm";
            this.Ribbon = this.ribbon;
            this.Text = "DVMS Virtual Folders";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager)).EndInit();
            this.previewPanel.ResumeLayout(false);
            this.previewContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.previewHostPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tree)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileExplorerAssistant1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.previewPanelExtension1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private DevExpress.XtraDialogs.FileExplorerAssistant fileExplorerAssistant1;
        private DevExpress.XtraDialogs.FileExplorerExtensions.PreviewPanelExtension previewPanelExtension1;
    }
}
