namespace File_Explorer_3
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
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraDialogs.NavigationBinding navigationBinding1 = new DevExpress.XtraDialogs.NavigationBinding();
            DevExpress.XtraDialogs.NavigationBinding navigationBinding2 = new DevExpress.XtraDialogs.NavigationBinding();
            DevExpress.XtraDialogs.NavigationBinding navigationBinding3 = new DevExpress.XtraDialogs.NavigationBinding();
            DevExpress.XtraDialogs.NavigationBinding navigationBinding4 = new DevExpress.XtraDialogs.NavigationBinding();
            DevExpress.XtraDialogs.FileExplorerExtensions.EnvironmentSpecialFolderNode environmentSpecialFolderNode1 = new DevExpress.XtraDialogs.FileExplorerExtensions.EnvironmentSpecialFolderNode();
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.fileExplorerAssistant1 = new DevExpress.XtraDialogs.FileExplorerAssistant(this.components);
            this.sidePanel1 = new DevExpress.XtraEditors.SidePanel();
            this.sidePanel2 = new DevExpress.XtraEditors.SidePanel();
            this.breadCrumbEdit1 = new DevExpress.XtraEditors.BreadCrumbEdit();
            this.breadCrumbExtension1 = new DevExpress.XtraDialogs.FileExplorerExtensions.BreadCrumbExtension();
            this.treeList1 = new DevExpress.XtraTreeList.TreeList();
            this.treeListExtension1 = new DevExpress.XtraDialogs.FileExplorerExtensions.TreeListExtension();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridControlExtension1 = new DevExpress.XtraDialogs.FileExplorerExtensions.GridControlExtension();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileExplorerAssistant1)).BeginInit();
            this.sidePanel1.SuspendLayout();
            this.sidePanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.breadCrumbEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.breadCrumbExtension1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeList1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeListExtension1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExtension1)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.ribbonControl1.SearchEditItem});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 1;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.Size = new System.Drawing.Size(1419, 150);
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "ribbonPageGroup1";
            // 
            // fileExplorerAssistant1
            // 
            this.fileExplorerAssistant1.Extensions.Add(this.breadCrumbExtension1);
            this.fileExplorerAssistant1.Extensions.Add(this.treeListExtension1);
            this.fileExplorerAssistant1.Extensions.Add(this.gridControlExtension1);
            this.fileExplorerAssistant1.Form = this;
            navigationBinding1.Source = this.treeListExtension1;
            navigationBinding1.Target = this.gridControlExtension1;
            navigationBinding2.Source = this.gridControlExtension1;
            navigationBinding2.Target = this.treeListExtension1;
            navigationBinding3.Source = this.breadCrumbExtension1;
            navigationBinding3.Target = this.gridControlExtension1;
            navigationBinding4.Source = this.gridControlExtension1;
            navigationBinding4.Target = this.breadCrumbExtension1;
            this.fileExplorerAssistant1.NavigationBindings.Add(navigationBinding1);
            this.fileExplorerAssistant1.NavigationBindings.Add(navigationBinding2);
            this.fileExplorerAssistant1.NavigationBindings.Add(navigationBinding3);
            this.fileExplorerAssistant1.NavigationBindings.Add(navigationBinding4);
            // 
            // sidePanel1
            // 
            this.sidePanel1.Controls.Add(this.treeList1);
            this.sidePanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidePanel1.Location = new System.Drawing.Point(0, 174);
            this.sidePanel1.Name = "sidePanel1";
            this.sidePanel1.Size = new System.Drawing.Size(250, 758);
            this.sidePanel1.TabIndex = 1;
            // 
            // sidePanel2
            // 
            this.sidePanel2.AllowResize = false;
            this.sidePanel2.Controls.Add(this.breadCrumbEdit1);
            this.sidePanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.sidePanel2.Location = new System.Drawing.Point(0, 150);
            this.sidePanel2.Name = "sidePanel2";
            this.sidePanel2.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.sidePanel2.Size = new System.Drawing.Size(1419, 24);
            this.sidePanel2.TabIndex = 2;
            // 
            // breadCrumbEdit1
            // 
            this.breadCrumbEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.breadCrumbEdit1.Location = new System.Drawing.Point(0, 3);
            this.breadCrumbEdit1.Name = "breadCrumbEdit1";
            this.breadCrumbEdit1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.breadCrumbEdit1.Size = new System.Drawing.Size(1419, 18);
            this.breadCrumbEdit1.TabIndex = 0;
            // 
            // breadCrumbExtension1
            // 
            this.breadCrumbExtension1.Control = this.breadCrumbEdit1;
            this.breadCrumbExtension1.CurrentPath = "C:\\Users\\Grant\\Documents";
            // 
            // treeList1
            // 
            this.treeList1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.treeList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeList1.Location = new System.Drawing.Point(0, 0);
            this.treeList1.Name = "treeList1";
            this.treeList1.Size = new System.Drawing.Size(249, 758);
            this.treeList1.TabIndex = 0;
            // 
            // treeListExtension1
            // 
            this.treeListExtension1.Control = this.treeList1;
            this.treeListExtension1.CurrentPath = "C:\\Users\\Grant\\Documents";
            this.treeListExtension1.IconSize = new System.Drawing.Size(16, 16);
            this.treeListExtension1.RootNodes.Add(environmentSpecialFolderNode1);
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(250, 174);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(1169, 758);
            this.gridControl1.TabIndex = 3;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            // 
            // gridControlExtension1
            // 
            this.gridControlExtension1.Control = this.gridControl1;
            this.gridControlExtension1.CurrentPath = "C:\\Users\\Grant\\Documents";
            this.gridControlExtension1.IconSize = new System.Drawing.Size(16, 16);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1419, 932);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.sidePanel1);
            this.Controls.Add(this.sidePanel2);
            this.Controls.Add(this.ribbonControl1);
            this.Name = "Form1";
            this.Ribbon = this.ribbonControl1;
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileExplorerAssistant1)).EndInit();
            this.sidePanel1.ResumeLayout(false);
            this.sidePanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.breadCrumbEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.breadCrumbExtension1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeList1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeListExtension1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExtension1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraDialogs.FileExplorerAssistant fileExplorerAssistant1;
        private DevExpress.XtraDialogs.FileExplorerExtensions.BreadCrumbExtension breadCrumbExtension1;
        private DevExpress.XtraEditors.BreadCrumbEdit breadCrumbEdit1;
        private DevExpress.XtraDialogs.FileExplorerExtensions.TreeListExtension treeListExtension1;
        private DevExpress.XtraTreeList.TreeList treeList1;
        private DevExpress.XtraDialogs.FileExplorerExtensions.GridControlExtension gridControlExtension1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.SidePanel sidePanel1;
        private DevExpress.XtraEditors.SidePanel sidePanel2;
    }
}

