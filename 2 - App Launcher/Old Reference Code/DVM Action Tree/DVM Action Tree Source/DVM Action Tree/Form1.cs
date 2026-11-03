using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraTreeList;
using System.Diagnostics;
using DevExpress.XtraTreeList.Nodes;

namespace DVM_Action_Tree
{
    public partial class Form_DVM_Action_Tree : RibbonForm
    {
        DataTable dt_Tree;

        public Form_DVM_Action_Tree()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Set_TreeList_Options();

            //Set_Tree_View_Mode();
            Set_Tree_List_Mode();

            Create_dt_Tree_Data_Table();

            Load_Test_Data();

            BindTree();

            treeList_NavTree.ExpandAll();

        }


        private void Set_TreeList_Options()
        {
            // Base Options.

            //treeList_NavTree.BackColor
            //treeList_NavTree.BackgroundImage
            //treeList_NavTree.BackgroundImageLayout
            treeList_NavTree.BestFitColumns(true);
            //treeList_NavTree.BestFitVisibleOnly = true;
            treeList_NavTree.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            //treeList_NavTree.CheckAll();
            //treeList_NavTree.CollapseAll();
            //treeList_NavTree.ForeColor = Color.Aqua;
            treeList_NavTree.IndicatorWidth = 100;             // Need to calculate and set this dynamically
                                                                // in the CustomDrawNodeIndicator event.
                                                                // treeList_NavTree.KeyFieldName
                                                                //treeList_NavTree.ParentFieldName
                                                                //treeList_NavTree.PreviewFieldName

            treeList_NavTree.SetDefaultBehaviorOptions();
            treeList_NavTree.SetDefaultOptionsView();
            treeList_NavTree.SetDefaultPrintOptions();
            treeList_NavTree.SetDefaultRowHeight();
            treeList_NavTree.SetDefaultSelectionOptions();

            treeList_NavTree.ShowButtonMode = ShowButtonModeEnum.Default;
            treeList_NavTree.TreeLevelWidth = 10;
            treeList_NavTree.TreeLineStyle = LineStyle.Dark;



            // OptionsBehavior Options.

            treeList_NavTree.OptionsBehavior.AllowCopyToClipboard = true;
            treeList_NavTree.OptionsBehavior.AllowExpandOnDblClick = true;
            treeList_NavTree.OptionsBehavior.AllowIncrementalSearch = true;
            treeList_NavTree.OptionsBehavior.AllowIndeterminateCheckState = false;
            treeList_NavTree.OptionsBehavior.AllowQuickHideColumns = true;
            treeList_NavTree.OptionsBehavior.AllowRecursiveNodeChecking = true;
            treeList_NavTree.OptionsBehavior.AutoFocusNewNode = true;
            treeList_NavTree.OptionsBehavior.AutoMoveRowFocus = true;
            treeList_NavTree.OptionsBehavior.AutoNodeHeight = true;
            treeList_NavTree.OptionsBehavior.AutoPopulateColumns = true;
            treeList_NavTree.OptionsBehavior.AutoSelectAllInEditor = true;
            treeList_NavTree.OptionsBehavior.CanCloneNodesOnDrop = true;
            treeList_NavTree.OptionsBehavior.CloseEditorOnLostFocus = true;
            treeList_NavTree.OptionsBehavior.CopyToClipboardWithColumnHeaders = true;
            treeList_NavTree.OptionsBehavior.CopyToClipboardWithNodeHierarchy = true;
            treeList_NavTree.OptionsBehavior.DragNodes = true;
            treeList_NavTree.OptionsBehavior.Editable = false;
            treeList_NavTree.OptionsBehavior.EnableFiltering = true;
            treeList_NavTree.OptionsBehavior.EnterMovesNextColumn = true;
            treeList_NavTree.OptionsBehavior.ExpandNodeOnDrag = true;
            treeList_NavTree.OptionsBehavior.ExpandNodesOnIncrementalSearch = true;
            treeList_NavTree.OptionsBehavior.ImmediateEditor = true;
            treeList_NavTree.OptionsBehavior.KeepSelectedOnClick = true;
            treeList_NavTree.OptionsBehavior.MoveOnEdit = true;
            treeList_NavTree.OptionsBehavior.PopulateServiceColumns = false;           // Show the "service" columns.
            treeList_NavTree.OptionsBehavior.ResizeNodes = true;
            treeList_NavTree.OptionsBehavior.ShowEditorOnMouseUp = false;
            treeList_NavTree.OptionsBehavior.ShowToolTips = true;
            treeList_NavTree.OptionsBehavior.SmartMouseHover = true;
            treeList_NavTree.OptionsBehavior.UseTabKey = true;


            // OptionsFilter Options.

            treeList_NavTree.OptionsFilter.AllowColumnMRUFilterList = true;
            treeList_NavTree.OptionsFilter.AllowFilterEditor = true;
            treeList_NavTree.OptionsFilter.AllowMRUFilterList = true;
            treeList_NavTree.OptionsFilter.FilterMode = FilterMode.Smart;
            treeList_NavTree.OptionsFilter.ShowAllValuesInCheckedFilterPopup = true;
            treeList_NavTree.OptionsFilter.ShowAllValuesInFilterPopup = true;

            // Gridview has it but Tree List does NOT!!!
            //gridView_Settings_Tree_Grid.OptionsFilter.AllowMultiSelectInCheckedFilterPopup = true;


            // OptionsFind Options.

            treeList_NavTree.OptionsFind.AllowFindPanel = true;
            treeList_NavTree.OptionsFind.AlwaysVisible = false;
            treeList_NavTree.OptionsFind.ClearFindOnClose = true;
            // treeList_NavTree.OptionsFind.FindDelay = 100;
            // treeList_NavTree.OptionsFind.FindFilterColumns = 
            treeList_NavTree.OptionsFind.FindMode = DevExpress.XtraTreeList.FindMode.Always;
            treeList_NavTree.OptionsFind.HighlightFindResults = true;
            treeList_NavTree.OptionsFind.ShowClearButton = true;
            treeList_NavTree.OptionsFind.ShowCloseButton = true;
            treeList_NavTree.OptionsFind.ShowFindButton = true;


            // OptionsLayout Options.

            treeList_NavTree.OptionsLayout.AddNewColumns = false;
            treeList_NavTree.OptionsLayout.RemoveOldColumns = false;
            treeList_NavTree.OptionsLayout.StoreAppearance = true;


            // OptionsMenu Options.

            treeList_NavTree.OptionsMenu.EnableColumnMenu = true;
            treeList_NavTree.OptionsMenu.EnableFooterMenu = true;
            treeList_NavTree.OptionsMenu.ShowAutoFilterRowItem = false;


            // OptionsPrint Options.

            treeList_NavTree.OptionsPrint.AutoRowHeight = true;
            treeList_NavTree.OptionsPrint.AutoWidth = false;
            treeList_NavTree.OptionsPrint.PrintAllNodes = true;
            treeList_NavTree.OptionsPrint.PrintFilledTreeIndent = false;
            treeList_NavTree.OptionsPrint.PrintHorzLines = true;
            treeList_NavTree.OptionsPrint.PrintImages = true;
            treeList_NavTree.OptionsPrint.PrintPageHeader = true;
            treeList_NavTree.OptionsPrint.PrintPreview = true;
            treeList_NavTree.OptionsPrint.PrintReportFooter = true;
            treeList_NavTree.OptionsPrint.PrintRowFooterSummary = true;
            treeList_NavTree.OptionsPrint.PrintTree = true;
            treeList_NavTree.OptionsPrint.PrintTreeButtons = true;
            treeList_NavTree.OptionsPrint.PrintVertLines = true;
            treeList_NavTree.OptionsPrint.UsePrintStyles = false;


            // OptionsSelection Options.

            treeList_NavTree.OptionsSelection.EnableAppearanceFocusedCell = false;
            treeList_NavTree.OptionsSelection.EnableAppearanceFocusedRow = true;
            treeList_NavTree.OptionsSelection.InvertSelection = false;
            treeList_NavTree.OptionsSelection.MultiSelect = false;
            treeList_NavTree.OptionsSelection.UseIndicatorForSelection = true;


            // OptionsView Options.

            treeList_NavTree.OptionsView.AllowGlyphSkinning = false;           // MUST be FALSE or else Tree images will be black!!!
            treeList_NavTree.OptionsView.AllowHtmlDrawHeaders = true;
            treeList_NavTree.OptionsView.AutoWidth = false;

            // Alternate Row Colorization/Highlighting:
            treeList_NavTree.OptionsView.EnableAppearanceEvenRow = false;
            treeList_NavTree.OptionsView.EnableAppearanceOddRow = false;
            //treeList_NavTree.Appearance.EvenRow.BackColor = Color.White;
            //treeList_NavTree.Appearance.OddRow.BackColor = Color.LightSkyBlue;

            treeList_NavTree.OptionsView.ExpandButtonCentered = true;
            treeList_NavTree.OptionsView.HeaderFilterButtonShowMode = DevExpress.XtraEditors.Controls.FilterButtonShowMode.SmartTag;
            treeList_NavTree.OptionsView.ShowAutoFilterRow = false;
            treeList_NavTree.OptionsView.ShowButtons = true;
            treeList_NavTree.OptionsView.ShowCheckBoxes = false;
            treeList_NavTree.OptionsView.ShowColumns = true;
            treeList_NavTree.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Default;
            treeList_NavTree.OptionsView.ShowFocusedFrame = true;
            treeList_NavTree.OptionsView.ShowHorzLines = true;
            treeList_NavTree.OptionsView.ShowIndentAsRowStyle = true;
            treeList_NavTree.OptionsView.ShowIndicator = true;
            treeList_NavTree.IndicatorWidth = 35;                      // Need to calculate and set this dynamically
            // in the CustomDrawNodeIndicator event.
            treeList_NavTree.OptionsView.ShowPreview = false;


            //
            // Debug:  If ShowRoot = true then filtering and sorting doesn't work because the root is blank.
            //
            //treeList_NavTree.OptionsView.ShowRoot = true;
            //treeList_NavTree.OptionsView.ShowRoot = false;


            treeList_NavTree.OptionsView.ShowRowFooterSummary = false;
            treeList_NavTree.OptionsView.ShowSummaryFooter = false;
            treeList_NavTree.OptionsView.ShowVertLines = true;


            // Misc Options/Settings:


            //
            // Debug:  If = 0 then filtering and sorting doesn't work because the root is blank.
            //
            //treeList_NavTree.RootValue = 0;




        }

        public void Set_Tree_List_Mode()
        {
            //show_As_Tree_View = false;
            //show_As_Tree_List = true;

            treeList_NavTree.OptionsBehavior.DragNodes = true;

            //treeList1.DragNodesMode = TreeListDragNodesMode.Default;
            //treeList1.DragNodesMode = TreeListDragNodesMode.Standard;
            treeList_NavTree.DragNodesMode = TreeListDragNodesMode.Advanced;

            // Make it look like regular TreeView.
            treeList_NavTree.OptionsView.ShowColumns = true;
            treeList_NavTree.OptionsView.ShowIndicator = true;
            treeList_NavTree.OptionsView.ShowHorzLines = true;
            treeList_NavTree.OptionsView.ShowVertLines = true;

            // This hides the ID and Parent_D columns.
            treeList_NavTree.OptionsBehavior.PopulateServiceColumns = true;

            // See E1153 - How to make the TreeList look like TreeView.
            // Also see the treeList1_CustomDrawNodeCell event.
        }

        public void Set_Tree_View_Mode()
        {
            //show_As_Tree_View = true;
            //show_As_Tree_List = false;

            treeList_NavTree.OptionsBehavior.DragNodes = true;

            //treeList1.DragNodesMode = TreeListDragNodesMode.Default;
            //treeList1.DragNodesMode = TreeListDragNodesMode.Standard;
            treeList_NavTree.DragNodesMode = TreeListDragNodesMode.Advanced;

            // Make it look like regular TreeView.
            treeList_NavTree.OptionsView.ShowColumns = false;
            treeList_NavTree.OptionsView.ShowIndicator = false;
            treeList_NavTree.OptionsView.ShowHorzLines = false;
            treeList_NavTree.OptionsView.ShowVertLines = false;

            // This hides the ID and ParentID columns.
            treeList_NavTree.OptionsBehavior.PopulateServiceColumns = false;

            // See E1153 - How to make the TreeList look like TreeView.
            // Also see the treeList1_CustomDrawNodeCell event.
        }


        public void Create_dt_Tree_Data_Table()
        {
            dt_Tree = new DataTable();

            dt_Tree.Columns.Add("ID", typeof(int));
            dt_Tree.Columns.Add("ParentID", typeof(int));

            dt_Tree.Columns.Add("Name", typeof(string));
            dt_Tree.Columns.Add("Description", typeof(string));

            dt_Tree.Columns.Add("Type", typeof(string));
            dt_Tree.Columns.Add("Object", typeof(string));
            dt_Tree.Columns.Add("Action", typeof(string));
        }


        public void Load_Test_Data()
        {
            dt_Tree.Rows.Add(1, 0, "Action Tree", "Action Tree Description", "Type", "Object", "Action");

            dt_Tree.Rows.Add(2, 1, "Project 1", "Description", "Project", @"D:\OneDrive\Grant - 2019\", "");
            dt_Tree.Rows.Add(3, 2, "Task", "Description", "Task List", "Task List 1", "Load");
            dt_Tree.Rows.Add(4, 2, "Document", "Description", "Document", @"D:\OneDrive\Grant - 2019\SiteGround Website Backups - 020918\DaVinci Minds University\DaVinciMinds.University - Plugins List - 020919.docx", "Load");
            dt_Tree.Rows.Add(5, 2, "Website", "Description", "Web Page", "www.Bing.com", "Load");
            dt_Tree.Rows.Add(6, 2, "Run Program", "Description", "App", "Notepad.exe", "Run");
            dt_Tree.Rows.Add(7, 2, "OneDrive Directory", "Description", "File Directory", @"D:\OneDrive", "Run");
            dt_Tree.Rows.Add(8, 2, "Reminder 1", "Description", "Reminder", "Notepad.exe", "Run");

            dt_Tree.Rows.Add(9, 1, "Folder 2019", "Description", "Folder", "", "");
            dt_Tree.Rows.Add(10, 9, "Note 2019", "Description", "Note", "Notepad.exe", @"D:\OneDrive\Grant - 2019\Note 2019.txt");

            dt_Tree.Rows.Add(11, 1, "Folder 2018", "Description", "Folder", "", "");
            dt_Tree.Rows.Add(12, 11, "Note 2018", "Description", "Note", "Notepad.exe", @"D:\OneDrive\Grant - 2018\Note 2018.txt");
        }


        public void BindTree()
        { 
            treeList_NavTree.DataSource = null;
            treeList_NavTree.DataSource = dt_Tree;

            treeList_NavTree.KeyFieldName = "ID";
            treeList_NavTree.ParentFieldName = "ParentID";

            gridControl1.DataSource = null;
            gridControl1.DataSource = dt_Tree;
        }


        private void treeList_NavTree_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TreeListHitInfo hi = treeList_NavTree.CalcHitInfo(new Point(e.X, e.Y));

            string node_Type = hi.HitInfoType.ToString();

            string node_ID = hi.Node.GetValue("ID").ToString();
            string node_ParentID = hi.Node.GetValue("ParentID").ToString();
            string node_Name = hi.Node.GetValue("Name").ToString();

            string node_Description = hi.Node.GetValue("Description").ToString();
            string node_Object = hi.Node.GetValue("Object").ToString();
            string node_Action = hi.Node.GetValue("Action").ToString();

            if (treeList_NavTree.FocusedNode != null)
            {
                switch(node_Action.ToUpper())
                {
                    case "RUN":
                    case "LOAD":
                        {
                            Process.Start(node_Object);

                            break;
                        }
                }
            }
        }


        private void simpleButton_ClearActionTree_Click(object sender, EventArgs e)
        {
            ClearTree();
        }

        public void ClearTree()
        {
            if (dt_Tree == null)
            {
                Create_dt_Tree_Data_Table();
            }

            if (dt_Tree.Rows.Count > 0)
            {
                dt_Tree.Clear();
            }
        }


        int HighestID = 0;

        string FileName = string.Empty;

        private void simpleButton_LoadActionTree_Click(object sender, EventArgs e)
        {
            ClearTree();

            App_Dialogs.XML_File_Load_Dialog dialog = new App_Dialogs.XML_File_Load_Dialog();

            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FileName = dialog.File_Name;
            }
            else
            {
                return;
            }

            dt_Tree.ReadXml(FileName);

            HighestID = GetHighestID();

            BindTree();
        }


        public int GetHighestID()
        {
            int returnInt = 0;

            DataRow[] maxID = dt_Tree.Select("ID = MAX(ID)");

            returnInt = (int)maxID[0]["ID"];

            return returnInt;
        }



        private void simpleButton_SaveActionTree_Click(object sender, EventArgs e)
        {
            if (dt_Tree == null)
            {
                return;
            }

            if (dt_Tree.Rows.Count <= 0)
            {
                return;
            }

            if ((FileName == null) || (FileName == string.Empty))
            {
                App_Dialogs.XML_File_Save_Dialog dialog = new App_Dialogs.XML_File_Save_Dialog();

                dialog.StartPosition = FormStartPosition.CenterParent;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    FileName = dialog.File_Name;
                }
                else
                {
                    return;
                }
            }

            dt_Tree.TableName = "dt_Tree";

            dt_Tree.WriteXml(FileName, XmlWriteMode.WriteSchema);
        }

        private void treeList_NavTree_AfterFocusNode(object sender, NodeEventArgs e)
        {
            if (e.Node != null)
            {
                string node_ID = e.Node.GetValue("ID").ToString();
                string node_ParentID = e.Node.GetValue("ParentID").ToString();
                string node_Name = e.Node.GetValue("Name").ToString();

                string node_Description = e.Node.GetValue("Description").ToString();
                string node_Object = e.Node.GetValue("Object").ToString();
                string node_Action = e.Node.GetValue("Action").ToString();

                textEdit_ID.Text = node_ID;
                textEdit_ParentID.Text = node_ParentID;
                textEdit_Name.Text = node_Name;

                memoEdit_Description.Text = node_Description;
                textEdit_Object.Text = node_Object;
                textEdit_Action.Text = node_Action;
            }
        }

        private void simpleButton_Delete_Click(object sender, EventArgs e)
        {
            TreeListNode node = treeList_NavTree.FocusedNode;
            string node_ID = node.GetValue("ID").ToString();

            string ID = string.Empty;

            for (int i = dt_Tree.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dr = dt_Tree.Rows[i];

                if (dr["ID"].ToString() == node_ID)
                {
                    dr.Delete();
                }

                dt_Tree.AcceptChanges();

                BlankFields();

               treeList_NavTree.Refresh();
            }
        }

        private void simpleButton_New_Click(object sender, EventArgs e)
        {
            BlankFields();

            HighestID = GetHighestID();

            HighestID++;

            textEdit_ID.Text = HighestID.ToString();

            TreeListNode node = treeList_NavTree.FocusedNode;
            string node_ParentID = node.GetValue("ParentID").ToString();

            textEdit_ParentID.Text = node_ParentID;
        }


        public void BlankFields()
        {
            textEdit_ID.Text = string.Empty;
            textEdit_ParentID.Text = string.Empty;
            textEdit_Name.Text = string.Empty;

            memoEdit_Description.Text = string.Empty;
            textEdit_Object.Text = string.Empty;
            textEdit_Action.Text = string.Empty;
        }


        private void simpleButton_Save_Click(object sender, EventArgs e)
        {
            DataRow newRow = dt_Tree.NewRow();

            newRow["ID"] = textEdit_ID.Text;
            newRow["ParentID"] = textEdit_ParentID.Text;
            newRow["Name"] = textEdit_Name.Text;

            newRow["Description"] = memoEdit_Description.Text;
            newRow["Object"] = textEdit_Object.Text;
            newRow["Action"] = textEdit_Action.Text;

            dt_Tree.Rows.Add(newRow);

            treeList_NavTree.Refresh();
        }

        private void simpleButton_Launch_Click(object sender, EventArgs e)
        {
            if (treeList_NavTree.FocusedNode != null)
            {
                TreeListNode node = treeList_NavTree.FocusedNode;
                string node_Object = node.GetValue("Object").ToString();
                string node_Action = node.GetValue("Action").ToString();

                switch (node_Action.ToUpper())
                {
                    case "RUN":
                    case "LOAD":
                        {
                            try
                            {
                                Process.Start(node_Object);
                            }
                            catch(Exception ex)
                            {
                                MessageBox.Show("Could not Launch object " + node_Object, "Launch Error!");
                            }

                            break;
                        }
                }
            }
        }



        private void checkEdit_ShowIDColumns_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit_ShowIDColumns.Checked == true)
            {
                //treeList_NavTree.OptionsBehavior.PopulateServiceColumns = true;

                treeList_NavTree.Columns[0].Visible = true;
                treeList_NavTree.Columns[1].Visible = true;

                treeList_NavTree.Refresh();
            }
            else
            {
                //treeList_NavTree.OptionsBehavior.PopulateServiceColumns = false;

                treeList_NavTree.Columns[0].Visible = false;
                treeList_NavTree.Columns[1].Visible = false;

                treeList_NavTree.Refresh();
            }
        }





    }
}
