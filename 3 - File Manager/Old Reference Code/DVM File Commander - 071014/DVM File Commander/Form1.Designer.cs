namespace DVM_File_Commander
{
    partial class Form_File_Commander
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_File_Commander));
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.dockPanel_Nav_Tree = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.panelControl_Nav_Tree_Main = new DevExpress.XtraEditors.PanelControl();
            this.treeList_Nav_Tree = new DevExpress.XtraTreeList.TreeList();
            this.panelControl_Nav_Tree_Footer = new DevExpress.XtraEditors.PanelControl();
            this.panelControl_Nav_Tree_Header = new DevExpress.XtraEditors.PanelControl();
            this.checkEdit_Show_CheckBoxes = new DevExpress.XtraEditors.CheckEdit();
            this.simpleButton_Show_Checked_Items = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_Show_Drives = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_Alpha_Blending = new DevExpress.XtraEditors.SimpleButton();
            this.checkEdit_Show_Preview = new DevExpress.XtraEditors.CheckEdit();
            this.checkEdit_Show_Lines = new DevExpress.XtraEditors.CheckEdit();
            this.checkEdit_Show_Footer = new DevExpress.XtraEditors.CheckEdit();
            this.checkEdit_Show_Alternating_Rows_Highlighting = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl_Nav_Tree_Selected_Node = new DevExpress.XtraEditors.LabelControl();
            this.imageList_Nav_Tree_Select = new System.Windows.Forms.ImageList(this.components);
            this.imageList_Nav_Tree_State = new System.Windows.Forms.ImageList(this.components);
            this.colFullName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colType = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colSize = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colAttributes = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colCheck = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            this.dockPanel_Nav_Tree.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_Nav_Tree_Main)).BeginInit();
            this.panelControl_Nav_Tree_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeList_Nav_Tree)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_Nav_Tree_Footer)).BeginInit();
            this.panelControl_Nav_Tree_Footer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_Nav_Tree_Header)).BeginInit();
            this.panelControl_Nav_Tree_Header.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_Show_CheckBoxes.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_Show_Preview.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_Show_Lines.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_Show_Footer.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_Show_Alternating_Rows_Highlighting.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 1;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2010;
            this.ribbonControl1.Size = new System.Drawing.Size(931, 144);
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
            // dockManager1
            // 
            this.dockManager1.Form = this;
            this.dockManager1.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.dockPanel_Nav_Tree});
            this.dockManager1.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "DevExpress.XtraBars.StandaloneBarDockControl",
            "System.Windows.Forms.StatusBar",
            "System.Windows.Forms.MenuStrip",
            "System.Windows.Forms.StatusStrip",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl"});
            // 
            // dockPanel_Nav_Tree
            // 
            this.dockPanel_Nav_Tree.Controls.Add(this.dockPanel1_Container);
            this.dockPanel_Nav_Tree.Dock = DevExpress.XtraBars.Docking.DockingStyle.Left;
            this.dockPanel_Nav_Tree.ID = new System.Guid("fa82d92d-74dc-4cab-ac2c-7c4e2d00ee79");
            this.dockPanel_Nav_Tree.Location = new System.Drawing.Point(0, 144);
            this.dockPanel_Nav_Tree.Name = "dockPanel_Nav_Tree";
            this.dockPanel_Nav_Tree.OriginalSize = new System.Drawing.Size(437, 200);
            this.dockPanel_Nav_Tree.Size = new System.Drawing.Size(437, 470);
            this.dockPanel_Nav_Tree.Text = "Nav Tree";
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add(this.panelControl_Nav_Tree_Main);
            this.dockPanel1_Container.Controls.Add(this.panelControl_Nav_Tree_Footer);
            this.dockPanel1_Container.Controls.Add(this.panelControl_Nav_Tree_Header);
            this.dockPanel1_Container.Location = new System.Drawing.Point(4, 23);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(429, 443);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // panelControl_Nav_Tree_Main
            // 
            this.panelControl_Nav_Tree_Main.Controls.Add(this.treeList_Nav_Tree);
            this.panelControl_Nav_Tree_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl_Nav_Tree_Main.Location = new System.Drawing.Point(0, 66);
            this.panelControl_Nav_Tree_Main.Name = "panelControl_Nav_Tree_Main";
            this.panelControl_Nav_Tree_Main.Padding = new System.Windows.Forms.Padding(5);
            this.panelControl_Nav_Tree_Main.Size = new System.Drawing.Size(429, 358);
            this.panelControl_Nav_Tree_Main.TabIndex = 1;
            // 
            // treeList_Nav_Tree
            // 
            this.treeList_Nav_Tree.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colFullName,
            this.colName,
            this.colType,
            this.colSize,
            this.colAttributes,
            this.colCheck});
            this.treeList_Nav_Tree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeList_Nav_Tree.Location = new System.Drawing.Point(7, 7);
            this.treeList_Nav_Tree.Name = "treeList_Nav_Tree";
            this.treeList_Nav_Tree.Size = new System.Drawing.Size(415, 344);
            this.treeList_Nav_Tree.TabIndex = 0;
            this.treeList_Nav_Tree.GetStateImage += new DevExpress.XtraTreeList.GetStateImageEventHandler(this.treeList_Nav_Tree_GetStateImage);
            this.treeList_Nav_Tree.GetSelectImage += new DevExpress.XtraTreeList.GetSelectImageEventHandler(this.treeList_Nav_Tree_GetSelectImage);
            this.treeList_Nav_Tree.GetPreviewText += new DevExpress.XtraTreeList.GetPreviewTextEventHandler(this.treeList_Nav_Tree_GetPreviewText);
            this.treeList_Nav_Tree.BeforeExpand += new DevExpress.XtraTreeList.BeforeExpandEventHandler(this.treeList_Nav_Tree_BeforeExpand);
            this.treeList_Nav_Tree.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.treeList_Nav_Tree_FocusedNodeChanged);
            this.treeList_Nav_Tree.CompareNodeValues += new DevExpress.XtraTreeList.CompareNodeValuesEventHandler(this.treeList_Nav_Tree_CompareNodeValues);
            this.treeList_Nav_Tree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeList_Nav_Tree_KeyDown);
            this.treeList_Nav_Tree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeList_Nav_Tree_MouseDown);
            // 
            // panelControl_Nav_Tree_Footer
            // 
            this.panelControl_Nav_Tree_Footer.Controls.Add(this.labelControl_Nav_Tree_Selected_Node);
            this.panelControl_Nav_Tree_Footer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl_Nav_Tree_Footer.Location = new System.Drawing.Point(0, 424);
            this.panelControl_Nav_Tree_Footer.Name = "panelControl_Nav_Tree_Footer";
            this.panelControl_Nav_Tree_Footer.Size = new System.Drawing.Size(429, 19);
            this.panelControl_Nav_Tree_Footer.TabIndex = 2;
            // 
            // panelControl_Nav_Tree_Header
            // 
            this.panelControl_Nav_Tree_Header.Controls.Add(this.checkEdit_Show_Alternating_Rows_Highlighting);
            this.panelControl_Nav_Tree_Header.Controls.Add(this.checkEdit_Show_Footer);
            this.panelControl_Nav_Tree_Header.Controls.Add(this.checkEdit_Show_Lines);
            this.panelControl_Nav_Tree_Header.Controls.Add(this.checkEdit_Show_Preview);
            this.panelControl_Nav_Tree_Header.Controls.Add(this.checkEdit_Show_CheckBoxes);
            this.panelControl_Nav_Tree_Header.Controls.Add(this.simpleButton_Show_Drives);
            this.panelControl_Nav_Tree_Header.Controls.Add(this.simpleButton_Alpha_Blending);
            this.panelControl_Nav_Tree_Header.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl_Nav_Tree_Header.Location = new System.Drawing.Point(0, 0);
            this.panelControl_Nav_Tree_Header.Name = "panelControl_Nav_Tree_Header";
            this.panelControl_Nav_Tree_Header.Size = new System.Drawing.Size(429, 66);
            this.panelControl_Nav_Tree_Header.TabIndex = 0;
            // 
            // checkEdit_Show_CheckBoxes
            // 
            this.checkEdit_Show_CheckBoxes.Location = new System.Drawing.Point(99, 15);
            this.checkEdit_Show_CheckBoxes.MenuManager = this.ribbonControl1;
            this.checkEdit_Show_CheckBoxes.Name = "checkEdit_Show_CheckBoxes";
            this.checkEdit_Show_CheckBoxes.Properties.Caption = "Show Checkboxes";
            this.checkEdit_Show_CheckBoxes.Size = new System.Drawing.Size(115, 15);
            this.checkEdit_Show_CheckBoxes.TabIndex = 8;
            this.checkEdit_Show_CheckBoxes.CheckedChanged += new System.EventHandler(this.checkEdit_Show_CheckBoxes_CheckedChanged);
            // 
            // simpleButton_Show_Checked_Items
            // 
            this.simpleButton_Show_Checked_Items.Location = new System.Drawing.Point(484, 167);
            this.simpleButton_Show_Checked_Items.Name = "simpleButton_Show_Checked_Items";
            this.simpleButton_Show_Checked_Items.Size = new System.Drawing.Size(84, 23);
            this.simpleButton_Show_Checked_Items.TabIndex = 6;
            this.simpleButton_Show_Checked_Items.Text = "Show Checked";
            this.simpleButton_Show_Checked_Items.Click += new System.EventHandler(this.simpleButton_Show_Checked_Items_Click);
            // 
            // simpleButton_Show_Drives
            // 
            this.simpleButton_Show_Drives.Location = new System.Drawing.Point(8, 8);
            this.simpleButton_Show_Drives.Name = "simpleButton_Show_Drives";
            this.simpleButton_Show_Drives.Size = new System.Drawing.Size(84, 23);
            this.simpleButton_Show_Drives.TabIndex = 1;
            this.simpleButton_Show_Drives.Text = "Show Drives";
            this.simpleButton_Show_Drives.Click += new System.EventHandler(this.simpleButton_Show_Drives_Click);
            // 
            // simpleButton_Alpha_Blending
            // 
            this.simpleButton_Alpha_Blending.Location = new System.Drawing.Point(8, 37);
            this.simpleButton_Alpha_Blending.Name = "simpleButton_Alpha_Blending";
            this.simpleButton_Alpha_Blending.Size = new System.Drawing.Size(84, 23);
            this.simpleButton_Alpha_Blending.TabIndex = 0;
            this.simpleButton_Alpha_Blending.Text = "Alpha Blending";
            this.simpleButton_Alpha_Blending.Click += new System.EventHandler(this.simpleButton_Alpha_Blending_Click);
            // 
            // checkEdit_Show_Preview
            // 
            this.checkEdit_Show_Preview.Location = new System.Drawing.Point(99, 41);
            this.checkEdit_Show_Preview.MenuManager = this.ribbonControl1;
            this.checkEdit_Show_Preview.Name = "checkEdit_Show_Preview";
            this.checkEdit_Show_Preview.Properties.Caption = "Show Preview";
            this.checkEdit_Show_Preview.Size = new System.Drawing.Size(94, 15);
            this.checkEdit_Show_Preview.TabIndex = 9;
            this.checkEdit_Show_Preview.CheckedChanged += new System.EventHandler(this.checkEdit_Show_Preview_CheckedChanged);
            // 
            // checkEdit_Show_Lines
            // 
            this.checkEdit_Show_Lines.Location = new System.Drawing.Point(230, 15);
            this.checkEdit_Show_Lines.MenuManager = this.ribbonControl1;
            this.checkEdit_Show_Lines.Name = "checkEdit_Show_Lines";
            this.checkEdit_Show_Lines.Properties.Caption = "Show Lines";
            this.checkEdit_Show_Lines.Size = new System.Drawing.Size(87, 15);
            this.checkEdit_Show_Lines.TabIndex = 10;
            this.checkEdit_Show_Lines.CheckedChanged += new System.EventHandler(this.checkEdit_Show_Lines_CheckedChanged);
            // 
            // checkEdit_Show_Footer
            // 
            this.checkEdit_Show_Footer.Location = new System.Drawing.Point(230, 41);
            this.checkEdit_Show_Footer.MenuManager = this.ribbonControl1;
            this.checkEdit_Show_Footer.Name = "checkEdit_Show_Footer";
            this.checkEdit_Show_Footer.Properties.Caption = "Show Footer";
            this.checkEdit_Show_Footer.Size = new System.Drawing.Size(87, 15);
            this.checkEdit_Show_Footer.TabIndex = 11;
            this.checkEdit_Show_Footer.CheckedChanged += new System.EventHandler(this.checkEdit_Show_Footer_CheckedChanged);
            // 
            // checkEdit_Show_Alternating_Rows_Highlighting
            // 
            this.checkEdit_Show_Alternating_Rows_Highlighting.Location = new System.Drawing.Point(323, 41);
            this.checkEdit_Show_Alternating_Rows_Highlighting.MenuManager = this.ribbonControl1;
            this.checkEdit_Show_Alternating_Rows_Highlighting.Name = "checkEdit_Show_Alternating_Rows_Highlighting";
            this.checkEdit_Show_Alternating_Rows_Highlighting.Properties.Caption = "Show Alt Rows";
            this.checkEdit_Show_Alternating_Rows_Highlighting.Size = new System.Drawing.Size(99, 15);
            this.checkEdit_Show_Alternating_Rows_Highlighting.TabIndex = 12;
            this.checkEdit_Show_Alternating_Rows_Highlighting.CheckedChanged += new System.EventHandler(this.checkEdit_Show_Alternating_Rows_Highlighting_CheckedChanged);
            // 
            // labelControl_Nav_Tree_Selected_Node
            // 
            this.labelControl_Nav_Tree_Selected_Node.Location = new System.Drawing.Point(5, 5);
            this.labelControl_Nav_Tree_Selected_Node.Name = "labelControl_Nav_Tree_Selected_Node";
            this.labelControl_Nav_Tree_Selected_Node.Size = new System.Drawing.Size(79, 13);
            this.labelControl_Nav_Tree_Selected_Node.TabIndex = 0;
            this.labelControl_Nav_Tree_Selected_Node.Text = "Selected Node:  ";
            // 
            // imageList_Nav_Tree_Select
            // 
            this.imageList_Nav_Tree_Select.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_Nav_Tree_Select.ImageStream")));
            this.imageList_Nav_Tree_Select.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_Nav_Tree_Select.Images.SetKeyName(0, "folder_closed.png");
            this.imageList_Nav_Tree_Select.Images.SetKeyName(1, "folder.png");
            this.imageList_Nav_Tree_Select.Images.SetKeyName(2, "document.png");
            this.imageList_Nav_Tree_Select.Images.SetKeyName(3, "hard_drive.png");
            this.imageList_Nav_Tree_Select.Images.SetKeyName(4, "folder_blue.png");
            this.imageList_Nav_Tree_Select.Images.SetKeyName(5, "folder_green.png");
            this.imageList_Nav_Tree_Select.Images.SetKeyName(6, "folder_red.png");
            this.imageList_Nav_Tree_Select.Images.SetKeyName(7, "floppy_drive.png");
            // 
            // imageList_Nav_Tree_State
            // 
            this.imageList_Nav_Tree_State.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_Nav_Tree_State.ImageStream")));
            this.imageList_Nav_Tree_State.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_Nav_Tree_State.Images.SetKeyName(0, "checkbox_unchecked.png");
            this.imageList_Nav_Tree_State.Images.SetKeyName(1, "checkbox.png");
            this.imageList_Nav_Tree_State.Images.SetKeyName(2, "checks.png");
            // 
            // colFullName
            // 
            this.colFullName.Caption = "Name";
            this.colFullName.FieldName = "FullName";
            this.colFullName.Name = "colFullName";
            // 
            // colName
            // 
            this.colName.AllNodesSummary = true;
            this.colName.Caption = "Name";
            this.colName.FieldName = "Name";
            this.colName.MinWidth = 50;
            this.colName.Name = "colName";
            this.colName.SummaryFooter = DevExpress.XtraTreeList.SummaryItemType.Count;
            this.colName.SummaryFooterStrFormat = "Files and Folders ({0})";
            this.colName.Visible = true;
            this.colName.VisibleIndex = 0;
            this.colName.Width = 286;
            // 
            // colType
            // 
            this.colType.Caption = "Type";
            this.colType.FieldName = "Type";
            this.colType.Name = "colType";
            this.colType.Visible = true;
            this.colType.VisibleIndex = 1;
            // 
            // colSize
            // 
            this.colSize.AppearanceCell.Options.UseTextOptions = true;
            this.colSize.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.colSize.Caption = "Size(Bytes)";
            this.colSize.FieldName = "Size";
            this.colSize.Name = "colSize";
            this.colSize.Visible = true;
            this.colSize.VisibleIndex = 2;
            // 
            // colAttributes
            // 
            this.colAttributes.Caption = "Attributes";
            this.colAttributes.FieldName = "Attributes";
            this.colAttributes.Name = "colAttributes";
            // 
            // colCheck
            // 
            this.colCheck.Caption = "Check";
            this.colCheck.FieldName = "Check";
            this.colCheck.Name = "colCheck";
            // 
            // Form_File_Commander
            // 
            this.AllowFormGlass = DevExpress.Utils.DefaultBoolean.False;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(931, 614);
            this.Controls.Add(this.dockPanel_Nav_Tree);
            this.Controls.Add(this.simpleButton_Show_Checked_Items);
            this.Controls.Add(this.ribbonControl1);
            this.Name = "Form_File_Commander";
            this.Ribbon = this.ribbonControl1;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DVM File Commander";
            this.Load += new System.EventHandler(this.Form_File_Commander_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            this.dockPanel_Nav_Tree.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_Nav_Tree_Main)).EndInit();
            this.panelControl_Nav_Tree_Main.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeList_Nav_Tree)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_Nav_Tree_Footer)).EndInit();
            this.panelControl_Nav_Tree_Footer.ResumeLayout(false);
            this.panelControl_Nav_Tree_Footer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_Nav_Tree_Header)).EndInit();
            this.panelControl_Nav_Tree_Header.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_Show_CheckBoxes.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_Show_Preview.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_Show_Lines.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_Show_Footer.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_Show_Alternating_Rows_Highlighting.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel_Nav_Tree;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        private DevExpress.XtraEditors.PanelControl panelControl_Nav_Tree_Main;
        private DevExpress.XtraTreeList.TreeList treeList_Nav_Tree;
        private DevExpress.XtraEditors.PanelControl panelControl_Nav_Tree_Footer;
        private DevExpress.XtraEditors.PanelControl panelControl_Nav_Tree_Header;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Alpha_Blending;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Show_Drives;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Show_Checked_Items;
        private DevExpress.XtraEditors.CheckEdit checkEdit_Show_CheckBoxes;
        private DevExpress.XtraEditors.CheckEdit checkEdit_Show_Footer;
        private DevExpress.XtraEditors.CheckEdit checkEdit_Show_Lines;
        private DevExpress.XtraEditors.CheckEdit checkEdit_Show_Preview;
        private DevExpress.XtraEditors.CheckEdit checkEdit_Show_Alternating_Rows_Highlighting;
        private DevExpress.XtraEditors.LabelControl labelControl_Nav_Tree_Selected_Node;
        private System.Windows.Forms.ImageList imageList_Nav_Tree_Select;
        private System.Windows.Forms.ImageList imageList_Nav_Tree_State;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colFullName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colType;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colSize;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colAttributes;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colCheck;
    }
}

