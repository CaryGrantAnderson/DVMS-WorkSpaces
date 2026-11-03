namespace DVMS_WorkSpaces
{
    partial class DVMSWorkSpacesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DVMSWorkSpacesForm));
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.barButtonItem_AddDocument = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem_ClearAllDocuments = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem_ShowWorkSpacesForm = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem_AddWorkSpacesForm = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonStatusBar1 = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.panelContainer1 = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel_WorkSpacesTree = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.panelControl_WorkSpacesTree_Main = new DevExpress.XtraEditors.PanelControl();
            this.treeList_WorkSpaces = new DevExpress.XtraTreeList.TreeList();
            this.panelControl_WorkSpacesTree_Bottom = new DevExpress.XtraEditors.PanelControl();
            this.panelControl_WorkSpacesTree_Top = new DevExpress.XtraEditors.PanelControl();
            this.dockPanel_ToolBox = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel2_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.treeList_ToolBox = new DevExpress.XtraTreeList.TreeList();
            this.panelControl_ToolBox_Bottom = new DevExpress.XtraEditors.PanelControl();
            this.panelControl_ToolBox_Top = new DevExpress.XtraEditors.PanelControl();
            this.documentManager1 = new DevExpress.XtraBars.Docking2010.DocumentManager(this.components);
            this.tabbedView1 = new DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            this.panelContainer1.SuspendLayout();
            this.dockPanel_WorkSpacesTree.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_WorkSpacesTree_Main)).BeginInit();
            this.panelControl_WorkSpacesTree_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeList_WorkSpaces)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_WorkSpacesTree_Bottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_WorkSpacesTree_Top)).BeginInit();
            this.dockPanel_ToolBox.SuspendLayout();
            this.dockPanel2_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeList_ToolBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_ToolBox_Bottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_ToolBox_Top)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.documentManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabbedView1)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.barButtonItem_AddDocument,
            this.barButtonItem_ClearAllDocuments,
            this.barButtonItem_ShowWorkSpacesForm,
            this.barButtonItem_AddWorkSpacesForm});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 5;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.Size = new System.Drawing.Size(1186, 158);
            this.ribbonControl1.StatusBar = this.ribbonStatusBar1;
            // 
            // barButtonItem_AddDocument
            // 
            this.barButtonItem_AddDocument.Caption = "Add Document";
            this.barButtonItem_AddDocument.Id = 1;
            this.barButtonItem_AddDocument.Name = "barButtonItem_AddDocument";
            this.barButtonItem_AddDocument.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_AddDocument_ItemClick);
            // 
            // barButtonItem_ClearAllDocuments
            // 
            this.barButtonItem_ClearAllDocuments.Caption = "Clear All Documents";
            this.barButtonItem_ClearAllDocuments.Id = 2;
            this.barButtonItem_ClearAllDocuments.Name = "barButtonItem_ClearAllDocuments";
            this.barButtonItem_ClearAllDocuments.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_ClearAllDocuments_ItemClick);
            // 
            // barButtonItem_ShowWorkSpacesForm
            // 
            this.barButtonItem_ShowWorkSpacesForm.Caption = "Show WorkSpaces Form";
            this.barButtonItem_ShowWorkSpacesForm.Id = 3;
            this.barButtonItem_ShowWorkSpacesForm.Name = "barButtonItem_ShowWorkSpacesForm";
            this.barButtonItem_ShowWorkSpacesForm.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_ShowWorkSpacesForm_ItemClick);
            // 
            // barButtonItem_AddWorkSpacesForm
            // 
            this.barButtonItem_AddWorkSpacesForm.Caption = "Add WorkSpaces Form";
            this.barButtonItem_AddWorkSpacesForm.Id = 4;
            this.barButtonItem_AddWorkSpacesForm.Name = "barButtonItem_AddWorkSpacesForm";
            this.barButtonItem_AddWorkSpacesForm.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_AddWorkSpacesForm_ItemClick);
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1,
            this.ribbonPageGroup2});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.barButtonItem_AddDocument);
            this.ribbonPageGroup1.ItemLinks.Add(this.barButtonItem_ClearAllDocuments);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "ribbonPageGroup1";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.barButtonItem_ShowWorkSpacesForm);
            this.ribbonPageGroup2.ItemLinks.Add(this.barButtonItem_AddWorkSpacesForm);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "ribbonPageGroup2";
            // 
            // ribbonStatusBar1
            // 
            this.ribbonStatusBar1.Location = new System.Drawing.Point(0, 720);
            this.ribbonStatusBar1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ribbonStatusBar1.Name = "ribbonStatusBar1";
            this.ribbonStatusBar1.Ribbon = this.ribbonControl1;
            this.ribbonStatusBar1.Size = new System.Drawing.Size(1186, 24);
            // 
            // dockManager1
            // 
            this.dockManager1.Form = this;
            this.dockManager1.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.panelContainer1});
            this.dockManager1.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "DevExpress.XtraBars.StandaloneBarDockControl",
            "System.Windows.Forms.MenuStrip",
            "System.Windows.Forms.StatusStrip",
            "System.Windows.Forms.StatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl",
            "DevExpress.XtraBars.Navigation.OfficeNavigationBar",
            "DevExpress.XtraBars.Navigation.TileNavPane",
            "DevExpress.XtraBars.TabFormControl",
            "DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormControl",
            "DevExpress.XtraBars.ToolbarForm.ToolbarFormControl"});
            // 
            // panelContainer1
            // 
            this.panelContainer1.ActiveChild = this.dockPanel_WorkSpacesTree;
            this.panelContainer1.Controls.Add(this.dockPanel_WorkSpacesTree);
            this.panelContainer1.Controls.Add(this.dockPanel_ToolBox);
            this.panelContainer1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Left;
            this.panelContainer1.ID = new System.Guid("13f91ea8-9f2b-4499-9c4e-5413c5e9c067");
            this.panelContainer1.Location = new System.Drawing.Point(0, 158);
            this.panelContainer1.Name = "panelContainer1";
            this.panelContainer1.OriginalSize = new System.Drawing.Size(341, 200);
            this.panelContainer1.Size = new System.Drawing.Size(341, 562);
            this.panelContainer1.Tabbed = true;
            this.panelContainer1.TabsPosition = DevExpress.XtraBars.Docking.TabsPosition.Top;
            this.panelContainer1.Text = "panelContainer1";
            // 
            // dockPanel_WorkSpacesTree
            // 
            this.dockPanel_WorkSpacesTree.Controls.Add(this.dockPanel1_Container);
            this.dockPanel_WorkSpacesTree.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
            this.dockPanel_WorkSpacesTree.ID = new System.Guid("af41ec46-dc0f-4264-b0f1-506075b8104b");
            this.dockPanel_WorkSpacesTree.Location = new System.Drawing.Point(3, 52);
            this.dockPanel_WorkSpacesTree.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dockPanel_WorkSpacesTree.Name = "dockPanel_WorkSpacesTree";
            this.dockPanel_WorkSpacesTree.OriginalSize = new System.Drawing.Size(341, 200);
            this.dockPanel_WorkSpacesTree.Size = new System.Drawing.Size(334, 507);
            this.dockPanel_WorkSpacesTree.TabsPosition = DevExpress.XtraBars.Docking.TabsPosition.Top;
            this.dockPanel_WorkSpacesTree.Text = "WorkSpaces Tree";
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add(this.panelControl_WorkSpacesTree_Main);
            this.dockPanel1_Container.Controls.Add(this.panelControl_WorkSpacesTree_Bottom);
            this.dockPanel1_Container.Controls.Add(this.panelControl_WorkSpacesTree_Top);
            this.dockPanel1_Container.Location = new System.Drawing.Point(0, 0);
            this.dockPanel1_Container.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(334, 507);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // panelControl_WorkSpacesTree_Main
            // 
            this.panelControl_WorkSpacesTree_Main.Controls.Add(this.treeList_WorkSpaces);
            this.panelControl_WorkSpacesTree_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl_WorkSpacesTree_Main.Location = new System.Drawing.Point(0, 35);
            this.panelControl_WorkSpacesTree_Main.Name = "panelControl_WorkSpacesTree_Main";
            this.panelControl_WorkSpacesTree_Main.Size = new System.Drawing.Size(334, 441);
            this.panelControl_WorkSpacesTree_Main.TabIndex = 2;
            // 
            // treeList_WorkSpaces
            // 
            this.treeList_WorkSpaces.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeList_WorkSpaces.Location = new System.Drawing.Point(2, 2);
            this.treeList_WorkSpaces.MenuManager = this.ribbonControl1;
            this.treeList_WorkSpaces.Name = "treeList_WorkSpaces";
            this.treeList_WorkSpaces.Size = new System.Drawing.Size(330, 437);
            this.treeList_WorkSpaces.TabIndex = 0;
            // 
            // panelControl_WorkSpacesTree_Bottom
            // 
            this.panelControl_WorkSpacesTree_Bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl_WorkSpacesTree_Bottom.Location = new System.Drawing.Point(0, 476);
            this.panelControl_WorkSpacesTree_Bottom.Name = "panelControl_WorkSpacesTree_Bottom";
            this.panelControl_WorkSpacesTree_Bottom.Size = new System.Drawing.Size(334, 31);
            this.panelControl_WorkSpacesTree_Bottom.TabIndex = 1;
            // 
            // panelControl_WorkSpacesTree_Top
            // 
            this.panelControl_WorkSpacesTree_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl_WorkSpacesTree_Top.Location = new System.Drawing.Point(0, 0);
            this.panelControl_WorkSpacesTree_Top.Name = "panelControl_WorkSpacesTree_Top";
            this.panelControl_WorkSpacesTree_Top.Size = new System.Drawing.Size(334, 35);
            this.panelControl_WorkSpacesTree_Top.TabIndex = 0;
            // 
            // dockPanel_ToolBox
            // 
            this.dockPanel_ToolBox.Controls.Add(this.dockPanel2_Container);
            this.dockPanel_ToolBox.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
            this.dockPanel_ToolBox.ID = new System.Guid("8041fb6a-7cf9-4ece-be35-7c1db00b13bf");
            this.dockPanel_ToolBox.Location = new System.Drawing.Point(3, 52);
            this.dockPanel_ToolBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dockPanel_ToolBox.Name = "dockPanel_ToolBox";
            this.dockPanel_ToolBox.OriginalSize = new System.Drawing.Size(200, 200);
            this.dockPanel_ToolBox.Size = new System.Drawing.Size(334, 507);
            this.dockPanel_ToolBox.TabsPosition = DevExpress.XtraBars.Docking.TabsPosition.Top;
            this.dockPanel_ToolBox.Text = "Toolbox";
            // 
            // dockPanel2_Container
            // 
            this.dockPanel2_Container.Controls.Add(this.treeList_ToolBox);
            this.dockPanel2_Container.Controls.Add(this.panelControl_ToolBox_Bottom);
            this.dockPanel2_Container.Controls.Add(this.panelControl_ToolBox_Top);
            this.dockPanel2_Container.Location = new System.Drawing.Point(0, 0);
            this.dockPanel2_Container.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dockPanel2_Container.Name = "dockPanel2_Container";
            this.dockPanel2_Container.Size = new System.Drawing.Size(334, 507);
            this.dockPanel2_Container.TabIndex = 0;
            // 
            // treeList_ToolBox
            // 
            this.treeList_ToolBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeList_ToolBox.Location = new System.Drawing.Point(0, 35);
            this.treeList_ToolBox.MenuManager = this.ribbonControl1;
            this.treeList_ToolBox.Name = "treeList_ToolBox";
            this.treeList_ToolBox.Size = new System.Drawing.Size(334, 441);
            this.treeList_ToolBox.TabIndex = 2;
            // 
            // panelControl_ToolBox_Bottom
            // 
            this.panelControl_ToolBox_Bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl_ToolBox_Bottom.Location = new System.Drawing.Point(0, 476);
            this.panelControl_ToolBox_Bottom.Name = "panelControl_ToolBox_Bottom";
            this.panelControl_ToolBox_Bottom.Size = new System.Drawing.Size(334, 31);
            this.panelControl_ToolBox_Bottom.TabIndex = 1;
            // 
            // panelControl_ToolBox_Top
            // 
            this.panelControl_ToolBox_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl_ToolBox_Top.Location = new System.Drawing.Point(0, 0);
            this.panelControl_ToolBox_Top.Name = "panelControl_ToolBox_Top";
            this.panelControl_ToolBox_Top.Size = new System.Drawing.Size(334, 35);
            this.panelControl_ToolBox_Top.TabIndex = 0;
            // 
            // documentManager1
            // 
            this.documentManager1.ContainerControl = this;
            this.documentManager1.MenuManager = this.ribbonControl1;
            this.documentManager1.View = this.tabbedView1;
            this.documentManager1.ViewCollection.AddRange(new DevExpress.XtraBars.Docking2010.Views.BaseView[] {
            this.tabbedView1});
            // 
            // DVMSWorkSpacesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1186, 744);
            this.Controls.Add(this.panelContainer1);
            this.Controls.Add(this.ribbonStatusBar1);
            this.Controls.Add(this.ribbonControl1);
            this.IconOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("DVMSWorkSpacesForm.IconOptions.LargeImage")));
            this.Name = "DVMSWorkSpacesForm";
            this.Ribbon = this.ribbonControl1;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.StatusBar = this.ribbonStatusBar1;
            this.Text = "DVMS WorkSpaces";
            this.Load += new System.EventHandler(this.DVMSWorkSpacesForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            this.panelContainer1.ResumeLayout(false);
            this.dockPanel_WorkSpacesTree.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_WorkSpacesTree_Main)).EndInit();
            this.panelControl_WorkSpacesTree_Main.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeList_WorkSpaces)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_WorkSpacesTree_Bottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_WorkSpacesTree_Top)).EndInit();
            this.dockPanel_ToolBox.ResumeLayout(false);
            this.dockPanel2_Container.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeList_ToolBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_ToolBox_Bottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_ToolBox_Top)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.documentManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabbedView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar1;
        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel_ToolBox;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel2_Container;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel_WorkSpacesTree;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        private DevExpress.XtraBars.Docking2010.DocumentManager documentManager1;
        private DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView tabbedView1;
        private DevExpress.XtraEditors.PanelControl panelControl_WorkSpacesTree_Main;
        private DevExpress.XtraTreeList.TreeList treeList_WorkSpaces;
        private DevExpress.XtraEditors.PanelControl panelControl_WorkSpacesTree_Bottom;
        private DevExpress.XtraEditors.PanelControl panelControl_WorkSpacesTree_Top;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_AddDocument;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_ClearAllDocuments;
        private DevExpress.XtraTreeList.TreeList treeList_ToolBox;
        private DevExpress.XtraEditors.PanelControl panelControl_ToolBox_Bottom;
        private DevExpress.XtraEditors.PanelControl panelControl_ToolBox_Top;
        private DevExpress.XtraBars.Docking.DockPanel panelContainer1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_ShowWorkSpacesForm;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_AddWorkSpacesForm;
    }
}

