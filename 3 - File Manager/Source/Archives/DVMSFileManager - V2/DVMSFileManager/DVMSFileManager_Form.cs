using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Windows.Forms;

namespace DVMSFileManager
{
    public partial class DVMSFileManagerForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public DVMSFileManagerForm()
        {
            InitializeComponent();
        }


        private void DVMSFileManagerForm_Load(object sender, EventArgs e)
        {
            ////InitBreadCrum();
        }

        #region Breadcrumb Control

        //// See:  https://docs.devexpress.com/WindowsForms/16973/controls-and-libraries/editors-and-simple-controls/breadcrumb-edit-control
        //// See:  https://docs.devexpress.com/WindowsForms/17004/controls-and-libraries/editors-and-simple-controls/breadcrumb-edit-control/how-to-create-a-file-explorer
        //// See:  https://github.com/DevExpress-Examples/winforms-file-explorer-breadcrumb-folder-nav

        //public static readonly int MaxEntitiesCount = 80;

        //private void InitBreadCrum()
        //{
        //    // Initialize the breadcrumb control here
        //    // For example, you can set the initial path or items
        //    //BreadCrumbEdit1.Path = "C:\\";

        //    breadCrumbEditxxx.Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        //    foreach (DriveInfo driveInfo in GetFixedDrives())
        //    {
        //        breadCrumbEditxxx.Properties.History.Add(new BreadCrumbHistoryItem(driveInfo.RootDirectory.ToString()));
        //    }
        //}


        //private void BreadCrumbEdit1_Properties_NewNodeAdding(object sender, DevExpress.XtraEditors.BreadCrumbNewNodeAddingEventArgs e)
        //{
        //    e.Node.PopulateOnDemand = true;
        //}

        ////Check whether or not the target path exists
        //private void BreadCrumbEdit1_Properties_ValidatePath(object sender, DevExpress.XtraEditors.BreadCrumbValidatePathEventArgs e)
        //{
        //    if (!Directory.Exists(e.Path))
        //    {
        //        e.ValidationResult = DevExpress.XtraEditors.BreadCrumbValidatePathResult.Cancel;
        //        return;
        //    }
        //    e.ValidationResult = DevExpress.XtraEditors.BreadCrumbValidatePathResult.CreateNodes;
        //}

        //private void BreadCrumbEdit1_Properties_QueryChildNodes(object sender, DevExpress.XtraEditors.BreadCrumbQueryChildNodesEventArgs e)
        //{
        //    //Add custom shortcuts to the 'Root' node
        //    if (string.Equals(e.Node.Caption, "Root", StringComparison.Ordinal))
        //    {
        //        InitBreadCrumbRootNode(e.Node);
        //        return;
        //    }
        //    //Add local discs shortcuts to the 'Root' node
        //    if (string.Equals(e.Node.Caption, "Computer", StringComparison.Ordinal))
        //    {
        //        InitBreadCrumbComputerNode(e.Node);
        //        return;
        //    }
        //    //Populate dynamic nodes
        //    string dir = e.Node.Path;
        //    if (!Directory.Exists(dir))
        //        return;
        //    string[] subDirs = GetSubFolders(dir);
        //    for (int i = 0; i < subDirs.Length; i++)
        //    {
        //        e.Node.ChildNodes.Add(CreateNode(subDirs[i]));
        //    }
        //}

        //void InitBreadCrumbRootNode(BreadCrumbNode node)
        //{
        //    node.ChildNodes.Add(new BreadCrumbNode("Desktop", Environment.GetFolderPath(Environment.SpecialFolder.Desktop)));
        //    node.ChildNodes.Add(new BreadCrumbNode("Windows", Environment.GetFolderPath(Environment.SpecialFolder.Windows)));
        //    node.ChildNodes.Add(new BreadCrumbNode("Program Files", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)));
        //}
        //void InitBreadCrumbComputerNode(BreadCrumbNode node)
        //{
        //    foreach (DriveInfo driveInfo in GetFixedDrives())
        //    {
        //        node.ChildNodes.Add(new BreadCrumbNode(driveInfo.Name, driveInfo.RootDirectory));
        //    }
        //}

        //protected BreadCrumbNode CreateNode(string path)
        //{
        //    string folderName = new DirectoryInfo(path).Name;
        //    return new BreadCrumbNode(folderName, folderName, true);
        //}

        ////Get the local drives list
        //public static IEnumerable<DriveInfo> GetFixedDrives()
        //{
        //    foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
        //    {
        //        if (driveInfo.DriveType != DriveType.Fixed) continue;
        //        yield return driveInfo;
        //    }
        //}

        ////Get all subfolders contained within the target directory
        //public static string[] GetSubFolders(string rootDir)
        //{
        //    string[] subDirs = GetSubDirs(rootDir);
        //    if (subDirs == null)
        //        return new string[0];
        //    if (subDirs.Length <= MaxEntitiesCount)
        //        return subDirs;
        //    string[] res = new string[MaxEntitiesCount];
        //    Array.Copy(subDirs, res, res.Length);
        //    return res;
        //}

        ////Get the names of the subdirectories
        //public static string[] GetSubDirs(string dir)
        //{
        //    string[] subDirs = null;
        //    try
        //    {
        //        subDirs = Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly);
        //    }
        //    catch { }
        //    return subDirs;
        //}

        //private void BreadCrumbEdit_PathChanged(object sender, BreadCrumbPathChangedEventArgs e)
        //{
        //    labelControl_CurrentPath.Text = breadCrumbEditxxx.Path;
        //}




        #endregion Breadcrumb Control

        private void treeList1_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {

        }
    }
}
