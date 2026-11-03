using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;

using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.Utils;
using DevExpress.XtraBars;



namespace DVM_File_Commander
{
    public partial class Form_File_Commander : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Globals:
        ///////////////////////////////////////////////////////////////////////////////////////////////

        DevExpress.XtraTreeList.Blending.XtraTreeListBlending xtraTreeListBlending1;




        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Main Code:
        ///////////////////////////////////////////////////////////////////////////////////////////////

        public Form_File_Commander()
        {
            InitializeComponent();
        }

        private void Form_File_Commander_Load(object sender, EventArgs e)
        {
            ribbonControl1.Minimized = true;

            //this.colSize.StyleName = "Style1";

            xtraTreeListBlending1 = new DevExpress.XtraTreeList.Blending.XtraTreeListBlending();
            xtraTreeListBlending1.TreeListControl = treeList_Nav_Tree;

            treeList_Nav_Tree.SelectImageList = imageList_Nav_Tree_Select;
            treeList_Nav_Tree.StateImageList = imageList_Nav_Tree_State;

            Initialize_Root();

            

            // Options:

            treeList_Nav_Tree.OptionsBehavior.AllowIndeterminateCheckState = true;
            treeList_Nav_Tree.OptionsBehavior.AllowRecursiveNodeChecking = true;

        }




        #region Virtual Tree Code

        private void Initialize_Root()
        {
            string current_Directory = Directory.GetCurrentDirectory();

            Initialize_Folders(Directory.GetDirectoryRoot(Directory.GetCurrentDirectory()), null, CheckState.Unchecked);

            //if (Directory.GetCurrentDirectory() == null)            // i.e. we're at the Drives level...
            //{
            //    Initialize_Folders(Directory.GetDirectoryRoot(Directory.GetCurrentDirectory()), null, CheckState.Unchecked);
            //}
            //else
            //{
            //    Initialize_Drives();
            //}
        }


        private void Initialize_Drives()
        {
            treeList_Nav_Tree.BeginUnboundLoad();

            TreeListNode node;

            try
            {
                string[] root = Directory.GetLogicalDrives();

                foreach (string s in root)
                {
                    node = treeList_Nav_Tree.AppendNode(new object[] { s, s, "Logical Driver", null, null, CheckState.Unchecked }, null);

                    node.HasChildren = true;

                    node.Tag = true;
                }
            }
            catch(Exception ex)
            {

            }

            treeList_Nav_Tree.EndUnboundLoad();
        }

        private void Initialize_Folders(string path, TreeListNode pNode, CheckState check)
        {
            treeList_Nav_Tree.BeginUnboundLoad();

            TreeListNode node;
            DirectoryInfo di;

            try
            {
                string[] root = Directory.GetDirectories(path);

                foreach (string s in root)
                {
                    try
                    {
                        di = new DirectoryInfo(s);

                        node = treeList_Nav_Tree.AppendNode(new object[] { s, di.Name, "Folder", null, di.Attributes, check }, pNode);

                        node.HasChildren = Has_Files(s);

                        if (node.HasChildren)
                        {
                            node.Tag = true;
                        }
                    }
                    catch { }
                }
            }
            catch
            {

            }

            Initialize_Files(path, pNode, check);

            treeList_Nav_Tree.EndUnboundLoad();
        }


        private void Initialize_Files(string path, TreeListNode pNode, CheckState check)
        {
            TreeListNode node;
            FileInfo fi;

            try
            {
                string[] root = Directory.GetFiles(path);

                foreach (string s in root)
                {
                    fi = new FileInfo(s);

                    node = treeList_Nav_Tree.AppendNode(new object[] { s, fi.Name, "File", fi.Length, fi.Attributes, check }, pNode);

                    node.HasChildren = false;
                }
            }
            catch
            {

            }
        }

        private bool Has_Files(string path)
        {
            string[] root = Directory.GetFiles(path);

            if (root.Length > 0)
            {
                return true;
            }

            root = Directory.GetDirectories(path);

            if (root.Length > 0)
            {
                return true;
            }

            return false;
        }

        #endregion Virtual Tree Code


        #region Button Handling


        private void simpleButton_Alpha_Blending_Click(object sender, EventArgs e)
        {
            xtraTreeListBlending1.Enabled = true;

            xtraTreeListBlending1.ShowDialog();
        }

        private void simpleButton_Show_Drives_Click(object sender, EventArgs e)
        {
            treeList_Nav_Tree.ClearNodes();

            Initialize_Root();
        }

        private void checkEdit_Show_CheckBoxes_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit_Show_CheckBoxes.Checked == true)
            {
                treeList_Nav_Tree.OptionsView.ShowCheckBoxes = true;
            }
            else
            {
                treeList_Nav_Tree.OptionsView.ShowCheckBoxes = false;
            }
        }

        private void checkEdit_Show_Preview_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit_Show_Preview.Checked == true)
            {
                treeList_Nav_Tree.OptionsView.ShowPreview = true;
            }
            else
            {
                treeList_Nav_Tree.OptionsView.ShowPreview = false;
            }
        }

        private void checkEdit_Show_Lines_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit_Show_Lines.Checked == true)
            {
                treeList_Nav_Tree.OptionsView.ShowHorzLines = true;
                treeList_Nav_Tree.OptionsView.ShowVertLines = true;
            }
            else
            {
                treeList_Nav_Tree.OptionsView.ShowHorzLines = false;
                treeList_Nav_Tree.OptionsView.ShowVertLines = false;
            }
        }

        private void checkEdit_Show_Footer_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit_Show_Footer.Checked == true)
            {
                treeList_Nav_Tree.OptionsView.ShowSummaryFooter = true;
            }
            else
            {
                treeList_Nav_Tree.OptionsView.ShowSummaryFooter = false;
            }
        }


        private void checkEdit_Show_Alternating_Rows_Highlighting_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit_Show_Alternating_Rows_Highlighting.Checked == true)
            {
                treeList_Nav_Tree.OptionsView.EnableAppearanceEvenRow = true;
            }
            else
            {
                treeList_Nav_Tree.OptionsView.EnableAppearanceEvenRow = false;
            }
        }


        private void simpleButton_Show_Checked_Items_Click(object sender, EventArgs e)
        {

        }



#endregion Button Handling


        #region Tree List Event Handling

        private void treeList_Nav_Tree_CompareNodeValues(object sender, CompareNodeValuesEventArgs e)
        {
            string type1 = e.Node1.GetDisplayText("Type");

            string type2 = e.Node2.GetDisplayText("Type");

            if (type1 != type2)
            {
                if (type1 == "Folder")
                {
                    e.Result = (e.SortOrder == SortOrder.Ascending) ? -1 : 1;
                }
                else
                {
                    e.Result = (e.SortOrder == SortOrder.Ascending) ? 1 : -1;
                }
            }
        }

        private void treeList_Nav_Tree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Space)
            {
                Set_Checked_Node(treeList_Nav_Tree.FocusedNode);
            }

            if (e.KeyData == Keys.Enter)
            {
                if (treeList_Nav_Tree.FocusedNode.GetDisplayText("Type") == "File")
                {
                    try
                    {
                        System.Diagnostics.Process process = new System.Diagnostics.Process();

                        process.StartInfo.FileName = treeList_Nav_Tree.FocusedNode.GetDisplayText("FullName");

                        process.StartInfo.Verb = "Open";

                        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

                        process.Start();
                    }
                    catch
                    {

                    }
                }
                else
                {
                    if (treeList_Nav_Tree.FocusedNode.HasChildren)
                    {
                        treeList_Nav_Tree.FocusedNode.Expanded = !treeList_Nav_Tree.FocusedNode.Expanded;
                    }
                }
            }
        }

        private void Set_Checked_Node(TreeListNode node)
        {
            CheckState check = (CheckState)node.GetValue("Check");

            if (check == CheckState.Indeterminate || check == CheckState.Unchecked)
            {
                check = CheckState.Checked;
            }
            else
            {
                check = CheckState.Unchecked;
            }

            treeList_Nav_Tree.FocusedNode = node;

            treeList_Nav_Tree.BeginUpdate();

            node["Check"] = check;

            labelControl_Nav_Tree_Selected_Node.Text = "Selected Node: " + treeList_Nav_Tree.FocusedNode.GetDisplayText("FullName");

            Set_Checked_Child_Nodes(node, check);

            Set_Checked_Parent_Nodes(node, check);

            treeList_Nav_Tree.EndUpdate();
        }


        private void Set_Checked_Child_Nodes(TreeListNode node, CheckState check)
        {
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                node.Nodes[i]["Check"] = check;

                Set_Checked_Child_Nodes(node.Nodes[i], check);
            }
        }

        private void Set_Checked_Parent_Nodes(TreeListNode node, CheckState check)
        {
            if (node.ParentNode != null)
            {
                bool b = false;

                for (int i = 0; i < node.ParentNode.Nodes.Count; i++)
                {
                    if (!check.Equals(node.ParentNode.Nodes[i]["Check"]))
                    {
                        b = !b;
                        break;
                    }
                }

                node.ParentNode["Check"] = b ? CheckState.Indeterminate : check;

                Set_Checked_Parent_Nodes(node.ParentNode, check);
            }
        }

        private string Checked_Items_Info_Main()
        {
            string s = "";

            Checked_Items_Info(null, ref s);

            if (s == "")
            {
                s = "No checked files...";
            }

            return s;
        }

        private void Checked_Items_Info(TreeListNode pnode, ref string s)
        {
            TreeListNodes nodes = pnode != null ? pnode.Nodes : treeList_Nav_Tree.Nodes;

            foreach (TreeListNode node in nodes)
            {
                CheckState nodeChecked = (CheckState)node.GetValue("Check");

                if (nodeChecked != CheckState.Unchecked)
                {
                    if (nodeChecked == CheckState.Checked && node.GetDisplayText("Type") == "File")
                    {
                        s += node.GetDisplayText("FullName") + "\r\n";
                    }

                    Checked_Items_Info(node, ref s);
                }
            }
        }

        private void treeList_Nav_Tree_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                TreeListHitInfo hInfo = treeList_Nav_Tree.CalcHitInfo(new Point(e.X, e.Y));

                if (hInfo.HitInfoType == HitInfoType.StateImage)
                {
                    Set_Checked_Node(hInfo.Node);
                }
            }
        }

        private void treeList_Nav_Tree_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            if (treeList_Nav_Tree.FocusedNode == null)
            {
                labelControl_Nav_Tree_Selected_Node.Text = "Selected Node: ";
            }
            else
            {
                labelControl_Nav_Tree_Selected_Node.Text = "Selected Node: " + treeList_Nav_Tree.FocusedNode.GetDisplayText("FullName");
            }
        }

        private void treeList_Nav_Tree_BeforeExpand(object sender, BeforeExpandEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                Cursor currentCursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;

                Initialize_Folders(e.Node.GetDisplayText("FullName"), e.Node, (CheckState)e.Node.GetValue("Check"));

                e.Node.Tag = null;

                Cursor.Current = currentCursor;
            }
        }

        private void treeList_Nav_Tree_GetPreviewText(object sender, GetPreviewTextEventArgs e)
        {
            e.PreviewText = e.Node.GetDisplayText("FullName");
        }

        private void treeList_Nav_Tree_GetSelectImage(object sender, GetSelectImageEventArgs e)
        {
            if (e.Node.GetDisplayText("Type") == "Folder")
            {
                e.NodeImageIndex = e.Node.Expanded ? 1 : 0;
            }
            else if (e.Node.GetDisplayText("Type") == "File")
            {
                e.NodeImageIndex = 2;
            }
            else
            {
                e.NodeImageIndex = 3;
            }
        }

        private void treeList_Nav_Tree_GetStateImage(object sender, GetStateImageEventArgs e)
        {
            CheckState check = (CheckState)e.Node.GetValue("Check");

            if (check == CheckState.Unchecked)
            {
                e.NodeImageIndex = 0;
            }
            else if (check == CheckState.Checked)
            {
                e.NodeImageIndex = 1;
            }
            else
            {
                e.NodeImageIndex = 2;
            }
        }

        #endregion Tree List Event Handling


        #region Misc Code

        void ListClick(object sender, ItemClickEventArgs e)
        {
            DevExpress.XtraEditors.XtraForm f = new DevExpress.XtraEditors.XtraForm();

            TextBox tb = new TextBox();
            tb.Multiline = true;
            tb.Dock = DockStyle.Fill;
            tb.ScrollBars = ScrollBars.Vertical;
            tb.Text = Checked_Items_Info_Main();
            tb.SelectionLength = 0;

            f.Controls.Add(tb);
            f.Text = "CheckedItems Info";
            f.StartPosition = FormStartPosition.Manual;
            f.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            f.Location = DevExpress.Utils.ControlUtils.CalcLocation(Control.MousePosition, Control.MousePosition, f.Size);
            f.ShowDialog();
        }

        #endregion Misc Code






    }
}
