namespace DVM_Action_Tree
{
    partial class Form_DVM_Action_Tree
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
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.skinRibbonGalleryBarItem1 = new DevExpress.XtraBars.SkinRibbonGalleryBarItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.rpgSkins = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.treeList_NavTree = new DevExpress.XtraTreeList.TreeList();
            this.splitterControl1 = new DevExpress.XtraEditors.SplitterControl();
            this.simpleButton_SaveActionTree = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_LoadActionTree = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_ClearActionTree = new DevExpress.XtraEditors.SimpleButton();
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            this.simpleButton_Launch = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_New = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_Delete = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_Save = new DevExpress.XtraEditors.SimpleButton();
            this.memoEdit_Description = new DevExpress.XtraEditors.MemoEdit();
            this.textEdit_Action = new DevExpress.XtraEditors.TextEdit();
            this.textEdit_Object = new DevExpress.XtraEditors.TextEdit();
            this.textEdit_Name = new DevExpress.XtraEditors.TextEdit();
            this.textEdit_ParentID = new DevExpress.XtraEditors.TextEdit();
            this.textEdit_ID = new DevExpress.XtraEditors.TextEdit();
            this.labelControl_Action = new DevExpress.XtraEditors.LabelControl();
            this.labelControl_Object = new DevExpress.XtraEditors.LabelControl();
            this.labelControl_Description = new DevExpress.XtraEditors.LabelControl();
            this.labelControl_Name = new DevExpress.XtraEditors.LabelControl();
            this.labelControl_ParentID = new DevExpress.XtraEditors.LabelControl();
            this.labelControl_ID = new DevExpress.XtraEditors.LabelControl();
            this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
            this.checkEdit_ShowIDColumns = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeList_NavTree)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.xtraTabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit_Description.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_Action.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_Object.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_Name.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_ParentID.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_ID.Properties)).BeginInit();
            this.xtraTabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_ShowIDColumns.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(498, 323);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.skinRibbonGalleryBarItem1,
            this.ribbonControl1.SearchEditItem});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 2;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.Size = new System.Drawing.Size(1177, 158);
            // 
            // skinRibbonGalleryBarItem1
            // 
            this.skinRibbonGalleryBarItem1.Caption = "skinRibbonGalleryBarItem1";
            this.skinRibbonGalleryBarItem1.Id = 1;
            this.skinRibbonGalleryBarItem1.Name = "skinRibbonGalleryBarItem1";
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.rpgSkins});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "ribbonPage1";
            // 
            // rpgSkins
            // 
            this.rpgSkins.ItemLinks.Add(this.skinRibbonGalleryBarItem1);
            this.rpgSkins.Name = "rpgSkins";
            this.rpgSkins.Text = "Skins";
            // 
            // treeList_NavTree
            // 
            this.treeList_NavTree.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeList_NavTree.Location = new System.Drawing.Point(0, 158);
            this.treeList_NavTree.Name = "treeList_NavTree";
            this.treeList_NavTree.Size = new System.Drawing.Size(400, 556);
            this.treeList_NavTree.TabIndex = 2;
            this.treeList_NavTree.AfterFocusNode += new DevExpress.XtraTreeList.NodeEventHandler(this.treeList_NavTree_AfterFocusNode);
            this.treeList_NavTree.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.treeList_NavTree_MouseDoubleClick);
            // 
            // splitterControl1
            // 
            this.splitterControl1.Location = new System.Drawing.Point(400, 158);
            this.splitterControl1.Name = "splitterControl1";
            this.splitterControl1.Size = new System.Drawing.Size(10, 556);
            this.splitterControl1.TabIndex = 3;
            this.splitterControl1.TabStop = false;
            // 
            // simpleButton_SaveActionTree
            // 
            this.simpleButton_SaveActionTree.Location = new System.Drawing.Point(681, 67);
            this.simpleButton_SaveActionTree.Name = "simpleButton_SaveActionTree";
            this.simpleButton_SaveActionTree.Size = new System.Drawing.Size(115, 23);
            this.simpleButton_SaveActionTree.TabIndex = 5;
            this.simpleButton_SaveActionTree.Text = "Save Action Tree";
            this.simpleButton_SaveActionTree.Click += new System.EventHandler(this.simpleButton_SaveActionTree_Click);
            // 
            // simpleButton_LoadActionTree
            // 
            this.simpleButton_LoadActionTree.Location = new System.Drawing.Point(552, 67);
            this.simpleButton_LoadActionTree.Name = "simpleButton_LoadActionTree";
            this.simpleButton_LoadActionTree.Size = new System.Drawing.Size(114, 23);
            this.simpleButton_LoadActionTree.TabIndex = 6;
            this.simpleButton_LoadActionTree.Text = "Load Action Tree";
            this.simpleButton_LoadActionTree.Click += new System.EventHandler(this.simpleButton_LoadActionTree_Click);
            // 
            // simpleButton_ClearActionTree
            // 
            this.simpleButton_ClearActionTree.Location = new System.Drawing.Point(552, 96);
            this.simpleButton_ClearActionTree.Name = "simpleButton_ClearActionTree";
            this.simpleButton_ClearActionTree.Size = new System.Drawing.Size(114, 23);
            this.simpleButton_ClearActionTree.TabIndex = 8;
            this.simpleButton_ClearActionTree.Text = "Clear Action Tree";
            this.simpleButton_ClearActionTree.Click += new System.EventHandler(this.simpleButton_ClearActionTree_Click);
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl1.Location = new System.Drawing.Point(410, 158);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
            this.xtraTabControl1.Size = new System.Drawing.Size(767, 556);
            this.xtraTabControl1.TabIndex = 9;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPage1,
            this.xtraTabPage2});
            // 
            // xtraTabPage1
            // 
            this.xtraTabPage1.Controls.Add(this.simpleButton_Launch);
            this.xtraTabPage1.Controls.Add(this.simpleButton_New);
            this.xtraTabPage1.Controls.Add(this.simpleButton_Delete);
            this.xtraTabPage1.Controls.Add(this.simpleButton_Save);
            this.xtraTabPage1.Controls.Add(this.memoEdit_Description);
            this.xtraTabPage1.Controls.Add(this.textEdit_Action);
            this.xtraTabPage1.Controls.Add(this.textEdit_Object);
            this.xtraTabPage1.Controls.Add(this.textEdit_Name);
            this.xtraTabPage1.Controls.Add(this.textEdit_ParentID);
            this.xtraTabPage1.Controls.Add(this.textEdit_ID);
            this.xtraTabPage1.Controls.Add(this.labelControl_Action);
            this.xtraTabPage1.Controls.Add(this.labelControl_Object);
            this.xtraTabPage1.Controls.Add(this.labelControl_Description);
            this.xtraTabPage1.Controls.Add(this.labelControl_Name);
            this.xtraTabPage1.Controls.Add(this.labelControl_ParentID);
            this.xtraTabPage1.Controls.Add(this.labelControl_ID);
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Size = new System.Drawing.Size(765, 531);
            this.xtraTabPage1.Text = "Record";
            // 
            // simpleButton_Launch
            // 
            this.simpleButton_Launch.Location = new System.Drawing.Point(320, 25);
            this.simpleButton_Launch.Name = "simpleButton_Launch";
            this.simpleButton_Launch.Size = new System.Drawing.Size(115, 23);
            this.simpleButton_Launch.TabIndex = 16;
            this.simpleButton_Launch.Text = "Launch!";
            this.simpleButton_Launch.Click += new System.EventHandler(this.simpleButton_Launch_Click);
            // 
            // simpleButton_New
            // 
            this.simpleButton_New.Location = new System.Drawing.Point(311, 290);
            this.simpleButton_New.Name = "simpleButton_New";
            this.simpleButton_New.Size = new System.Drawing.Size(60, 23);
            this.simpleButton_New.TabIndex = 15;
            this.simpleButton_New.Text = "New";
            this.simpleButton_New.Click += new System.EventHandler(this.simpleButton_New_Click);
            // 
            // simpleButton_Delete
            // 
            this.simpleButton_Delete.Location = new System.Drawing.Point(226, 290);
            this.simpleButton_Delete.Name = "simpleButton_Delete";
            this.simpleButton_Delete.Size = new System.Drawing.Size(60, 23);
            this.simpleButton_Delete.TabIndex = 14;
            this.simpleButton_Delete.Text = "Delete";
            this.simpleButton_Delete.Click += new System.EventHandler(this.simpleButton_Delete_Click);
            // 
            // simpleButton_Save
            // 
            this.simpleButton_Save.Location = new System.Drawing.Point(390, 290);
            this.simpleButton_Save.Name = "simpleButton_Save";
            this.simpleButton_Save.Size = new System.Drawing.Size(60, 23);
            this.simpleButton_Save.TabIndex = 13;
            this.simpleButton_Save.Text = "Save";
            this.simpleButton_Save.Click += new System.EventHandler(this.simpleButton_Save_Click);
            // 
            // memoEdit_Description
            // 
            this.memoEdit_Description.Location = new System.Drawing.Point(113, 107);
            this.memoEdit_Description.MenuManager = this.ribbonControl1;
            this.memoEdit_Description.Name = "memoEdit_Description";
            this.memoEdit_Description.Size = new System.Drawing.Size(364, 120);
            this.memoEdit_Description.TabIndex = 12;
            // 
            // textEdit_Action
            // 
            this.textEdit_Action.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textEdit_Action.Location = new System.Drawing.Point(113, 259);
            this.textEdit_Action.MenuManager = this.ribbonControl1;
            this.textEdit_Action.Name = "textEdit_Action";
            this.textEdit_Action.Size = new System.Drawing.Size(631, 20);
            this.textEdit_Action.TabIndex = 11;
            // 
            // textEdit_Object
            // 
            this.textEdit_Object.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textEdit_Object.Location = new System.Drawing.Point(113, 233);
            this.textEdit_Object.MenuManager = this.ribbonControl1;
            this.textEdit_Object.Name = "textEdit_Object";
            this.textEdit_Object.Size = new System.Drawing.Size(631, 20);
            this.textEdit_Object.TabIndex = 10;
            // 
            // textEdit_Name
            // 
            this.textEdit_Name.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textEdit_Name.Location = new System.Drawing.Point(113, 79);
            this.textEdit_Name.MenuManager = this.ribbonControl1;
            this.textEdit_Name.Name = "textEdit_Name";
            this.textEdit_Name.Size = new System.Drawing.Size(631, 20);
            this.textEdit_Name.TabIndex = 8;
            // 
            // textEdit_ParentID
            // 
            this.textEdit_ParentID.Location = new System.Drawing.Point(113, 53);
            this.textEdit_ParentID.MenuManager = this.ribbonControl1;
            this.textEdit_ParentID.Name = "textEdit_ParentID";
            this.textEdit_ParentID.Size = new System.Drawing.Size(84, 20);
            this.textEdit_ParentID.TabIndex = 7;
            // 
            // textEdit_ID
            // 
            this.textEdit_ID.Location = new System.Drawing.Point(113, 27);
            this.textEdit_ID.MenuManager = this.ribbonControl1;
            this.textEdit_ID.Name = "textEdit_ID";
            this.textEdit_ID.Size = new System.Drawing.Size(84, 20);
            this.textEdit_ID.TabIndex = 6;
            // 
            // labelControl_Action
            // 
            this.labelControl_Action.Location = new System.Drawing.Point(26, 262);
            this.labelControl_Action.Name = "labelControl_Action";
            this.labelControl_Action.Size = new System.Drawing.Size(34, 13);
            this.labelControl_Action.TabIndex = 5;
            this.labelControl_Action.Text = "Action:";
            // 
            // labelControl_Object
            // 
            this.labelControl_Object.Location = new System.Drawing.Point(26, 236);
            this.labelControl_Object.Name = "labelControl_Object";
            this.labelControl_Object.Size = new System.Drawing.Size(36, 13);
            this.labelControl_Object.TabIndex = 4;
            this.labelControl_Object.Text = "Object:";
            // 
            // labelControl_Description
            // 
            this.labelControl_Description.Location = new System.Drawing.Point(26, 108);
            this.labelControl_Description.Name = "labelControl_Description";
            this.labelControl_Description.Size = new System.Drawing.Size(57, 13);
            this.labelControl_Description.TabIndex = 3;
            this.labelControl_Description.Text = "Description:";
            // 
            // labelControl_Name
            // 
            this.labelControl_Name.Location = new System.Drawing.Point(26, 82);
            this.labelControl_Name.Name = "labelControl_Name";
            this.labelControl_Name.Size = new System.Drawing.Size(31, 13);
            this.labelControl_Name.TabIndex = 2;
            this.labelControl_Name.Text = "Name:";
            // 
            // labelControl_ParentID
            // 
            this.labelControl_ParentID.Location = new System.Drawing.Point(26, 56);
            this.labelControl_ParentID.Name = "labelControl_ParentID";
            this.labelControl_ParentID.Size = new System.Drawing.Size(50, 13);
            this.labelControl_ParentID.TabIndex = 1;
            this.labelControl_ParentID.Text = "Parent ID:";
            // 
            // labelControl_ID
            // 
            this.labelControl_ID.Location = new System.Drawing.Point(26, 30);
            this.labelControl_ID.Name = "labelControl_ID";
            this.labelControl_ID.Size = new System.Drawing.Size(15, 13);
            this.labelControl_ID.TabIndex = 0;
            this.labelControl_ID.Text = "ID:";
            // 
            // xtraTabPage2
            // 
            this.xtraTabPage2.Controls.Add(this.gridControl1);
            this.xtraTabPage2.Name = "xtraTabPage2";
            this.xtraTabPage2.Size = new System.Drawing.Size(498, 323);
            this.xtraTabPage2.Text = "Grid";
            // 
            // checkEdit_ShowIDColumns
            // 
            this.checkEdit_ShowIDColumns.EditValue = true;
            this.checkEdit_ShowIDColumns.Location = new System.Drawing.Point(308, 98);
            this.checkEdit_ShowIDColumns.MenuManager = this.ribbonControl1;
            this.checkEdit_ShowIDColumns.Name = "checkEdit_ShowIDColumns";
            this.checkEdit_ShowIDColumns.Properties.Caption = "Show ID Columns";
            this.checkEdit_ShowIDColumns.Size = new System.Drawing.Size(129, 20);
            this.checkEdit_ShowIDColumns.TabIndex = 11;
            this.checkEdit_ShowIDColumns.CheckedChanged += new System.EventHandler(this.checkEdit_ShowIDColumns_CheckedChanged);
            // 
            // Form_DVM_Action_Tree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1177, 714);
            this.Controls.Add(this.checkEdit_ShowIDColumns);
            this.Controls.Add(this.xtraTabControl1);
            this.Controls.Add(this.simpleButton_ClearActionTree);
            this.Controls.Add(this.simpleButton_LoadActionTree);
            this.Controls.Add(this.simpleButton_SaveActionTree);
            this.Controls.Add(this.splitterControl1);
            this.Controls.Add(this.treeList_NavTree);
            this.Controls.Add(this.ribbonControl1);
            this.Name = "Form_DVM_Action_Tree";
            this.Ribbon = this.ribbonControl1;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DVM Action Tree";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeList_NavTree)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.xtraTabPage1.ResumeLayout(false);
            this.xtraTabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit_Description.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_Action.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_Object.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_Name.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_ParentID.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_ID.Properties)).EndInit();
            this.xtraTabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_ShowIDColumns.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgSkins;
        private DevExpress.XtraBars.SkinRibbonGalleryBarItem skinRibbonGalleryBarItem1;
        private DevExpress.XtraTreeList.TreeList treeList_NavTree;
        private DevExpress.XtraEditors.SplitterControl splitterControl1;
        private DevExpress.XtraEditors.SimpleButton simpleButton_SaveActionTree;
        private DevExpress.XtraEditors.SimpleButton simpleButton_LoadActionTree;
        private DevExpress.XtraEditors.SimpleButton simpleButton_ClearActionTree;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private DevExpress.XtraEditors.MemoEdit memoEdit_Description;
        private DevExpress.XtraEditors.TextEdit textEdit_Action;
        private DevExpress.XtraEditors.TextEdit textEdit_Object;
        private DevExpress.XtraEditors.TextEdit textEdit_Name;
        private DevExpress.XtraEditors.TextEdit textEdit_ParentID;
        private DevExpress.XtraEditors.TextEdit textEdit_ID;
        private DevExpress.XtraEditors.LabelControl labelControl_Action;
        private DevExpress.XtraEditors.LabelControl labelControl_Object;
        private DevExpress.XtraEditors.LabelControl labelControl_Description;
        private DevExpress.XtraEditors.LabelControl labelControl_Name;
        private DevExpress.XtraEditors.LabelControl labelControl_ParentID;
        private DevExpress.XtraEditors.LabelControl labelControl_ID;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
        private DevExpress.XtraEditors.SimpleButton simpleButton_New;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Delete;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Save;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Launch;
        private DevExpress.XtraEditors.CheckEdit checkEdit_ShowIDColumns;
    }
}
