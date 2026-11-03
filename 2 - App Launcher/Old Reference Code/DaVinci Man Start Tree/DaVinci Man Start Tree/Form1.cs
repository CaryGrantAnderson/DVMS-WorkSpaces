using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.UserSkins;
using DevExpress.XtraBars.Helpers;
using DevExpress.XtraEditors;

using System.IO;
using System.Diagnostics;


namespace DaVinci_Man_Start_Tree
{
    public partial class Form_Main : XtraForm
    {
        public Form_Main()
        {
            InitializeComponent();

            InitSkinGallery();



        }
        void InitSkinGallery()
        {
            SkinHelper.InitSkinGallery(rgbiSkins, true);
        }



        private void Form_Main_Load(object sender, EventArgs e)
        {
            Load_ComboBox_Start_Directory();

            ribbonControl.Minimized = true;
            ribbonControl.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Below;

            treeView_Main.ImageList = imageList_Tree_Nodes;
        }


        private void Load_ComboBox_Start_Directory()
        {
            comboBoxEdit_Start_Directory.Properties.Items.Clear();

            comboBoxEdit_Start_Directory.Properties.Items.Add("My Start Directory");
            comboBoxEdit_Start_Directory.Properties.Items.Add("All Users Start Directory");
            comboBoxEdit_Start_Directory.Properties.Items.Add("Test Directory");
            comboBoxEdit_Start_Directory.Properties.Items.Add("Custom Start 1");
            comboBoxEdit_Start_Directory.Properties.Items.Add("Custom Start 2");

            comboBoxEdit_Start_Directory.SelectedIndex = 0;
        }


        //
        // ToolBox TreeView Code:
        //


        private void Populate_TreeView(string start_Directory)
        {
            treeView_Main.Nodes.Clear();

            if (start_Directory == null)
            {
                return;
            }

            if (start_Directory == string.Empty)
            {
                return;
            }

            try
            {
                TreeNode rootNode;

                //DirectoryInfo info = new DirectoryInfo(@"../..");

                DirectoryInfo info = new DirectoryInfo(start_Directory);

                if (info.Exists)
                {
                    rootNode = new TreeNode(info.Name);

                    rootNode.Tag = info;

                    //Get_Directories(info.GetDirectories(), rootNode);


                    // Get any files in the Root directory.

                    foreach (string full_File_Name in Directory.GetFiles(start_Directory, "*.*"))
                    {
                        string file_Name = Path.GetFileName(full_File_Name);

                        string file_Extension = Path.GetExtension(full_File_Name);

                        string file_Name_Without_Extension = Path.GetFileNameWithoutExtension(full_File_Name);

                        string tree_File_Name = string.Empty;

                        if (checkEdit_Show_File_Extension.Checked == true)
                        {
                            if (checkEdit_Show_Full_Paths.Checked == true)
                            {
                                tree_File_Name = full_File_Name;
                            }
                            else
                            {
                                tree_File_Name = file_Name;
                            }
                        }
                        else
                        {
                            if (checkEdit_Show_Full_Paths.Checked == true)
                            {
                                tree_File_Name = full_File_Name;
                            }
                            else
                            {
                                tree_File_Name = file_Name_Without_Extension;
                            }
                        }

                        TreeNode file_Node = new TreeNode(tree_File_Name);

                        // Full File Name with Path for Start Process.
                        file_Node.Tag = full_File_Name;

                        //file_Node.ImageKey = "File";

                        switch (file_Extension.ToUpper())
                        {
                            case "":
                                {
                                    file_Node.ImageKey = "Folder";

                                    break;
                                }

                            case ".LNK":
                                {
                                    file_Node.ImageKey = "Link";

                                    break;
                                }

                            case ".EXE":
                                {
                                    file_Node.ImageKey = "Program";

                                    break;
                                }

                            case ".DOC":
                            case ".DOCX":
                                {
                                    file_Node.ImageKey = "Document";

                                    break;
                                }

                            default:
                                {
                                    file_Node.ImageKey = "Other";

                                    break;
                                }
                        }

                        rootNode.Nodes.Add(file_Node);
                    }



                    Directory_Recurse(start_Directory, rootNode);


                    treeView_Main.Nodes.Add(rootNode);
                }

                treeView_Main.ExpandAll();
                simpleButton_Expand.Text = "Collapse";
            }
            catch (Exception ex)
            {
                string error_msg = ex.ToString();

                MessageBox.Show(error_msg, "Populate_TreeView Error!");
            }
        }


        private void Get_Directories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
        {
            TreeNode aNode;

            DirectoryInfo[] subSubDirs;

            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);

                aNode.Tag = subDir;

                aNode.ImageKey = "Folder";


                // Get and display the files here in this subdirectory.

                string[] file_Paths = Directory.GetFiles(@"c:\MyDir\", "*.bmp", SearchOption.TopDirectoryOnly);

                foreach (string file_Name in file_Paths)
                {
                    TreeNode file_Node = new TreeNode(file_Name);

                    aNode.Tag = subDir;

                    aNode.ImageKey = "Folder";
                }



                subSubDirs = subDir.GetDirectories();

                if (subSubDirs.Length != 0)
                {
                    Get_Directories(subSubDirs, aNode);
                }

                nodeToAddTo.Nodes.Add(aNode);
            }
        }


        void Directory_Recurse(string search_Directory, TreeNode nodeToAddTo)
        {
            TreeNode aNode;

            try
            {
                foreach (string dir in Directory.GetDirectories(search_Directory))
                {
                    string last_Folder_Name = Path.GetFileName(dir);

                    aNode = new TreeNode(last_Folder_Name, 0, 0);

                    aNode.Tag = dir;

                    aNode.ImageKey = "Folder";

                    nodeToAddTo.Nodes.Add(aNode);

                    foreach (string full_File_Name in Directory.GetFiles(dir, "*.*"))
                    {
                        string file_Name = Path.GetFileName(full_File_Name);

                        string file_Extension = Path.GetExtension(full_File_Name);

                        string file_Name_Without_Extension = Path.GetFileNameWithoutExtension(full_File_Name);

                        string tree_File_Name = string.Empty;

                        if (checkEdit_Show_File_Extension.Checked == true)
                        {
                            if (checkEdit_Show_Full_Paths.Checked == true)
                            {
                                tree_File_Name = full_File_Name;
                            }
                            else
                            {
                                tree_File_Name = file_Name;
                            }
                        }
                        else
                        {
                            if (checkEdit_Show_Full_Paths.Checked == true)
                            {
                                tree_File_Name = full_File_Name;
                            }
                            else
                            {
                                tree_File_Name = file_Name_Without_Extension;
                            }
                        }

                        TreeNode file_Node = new TreeNode(tree_File_Name);

                        // Full File Name with Path for Start Process.
                        file_Node.Tag = full_File_Name;

                        switch (file_Extension.ToUpper())
                        {
                            case "":
                                {
                                    file_Node.ImageKey = "Folder";

                                    break;
                                }

                            case ".LNK":
                                {
                                    file_Node.ImageKey = "Link";

                                    break;
                                }

                            case ".EXE":
                                {
                                    file_Node.ImageKey = "Program";

                                    break;
                                }

                            case ".DOC":
                            case ".DOCX":
                                {
                                    file_Node.ImageKey = "Document";

                                    break;
                                }

                            default:
                                {
                                    file_Node.ImageKey = "Other";

                                    break;
                                }
                        }

                        aNode.Nodes.Add(file_Node);

                        // lstFilesFound.Items.Add(file);
                    }

                    Directory_Recurse(dir, aNode);
                }
            }
            catch (Exception ex)
            {
                string error_msg = ex.ToString();

                MessageBox.Show(error_msg, "Directory_Search Error!");
            }
        }


        private void simpleButton_Select_Start_Directory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowDialog();

            comboBoxEdit_Start_Directory.Text = dialog.SelectedPath;

            dialog.Dispose();
        }


        private void simpleButton_Load_ToolBox_TreeView_Click(object sender, EventArgs e)
        {
            Do_Start_Directory();
        }


        private void comboBoxEdit_Start_Directory_SelectedIndexChanged(object sender, EventArgs e)
        {
            Do_Start_Directory();
        }


        private void Do_Start_Directory()
        {
            string start_Directory = comboBoxEdit_Start_Directory.Text.Trim();

            switch (start_Directory)
            {
                case "All Users Start Directory":
                    {
                        start_Directory = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs";

                        break;
                    }

                case "My Start Directory":
                    {
                        // %appdata%\Microsoft\Windows\Start Menu

                        // C:\Users\Grant_000\AppData\Roaming\Microsoft\Windows\Start Menu

                        start_Directory = Environment.ExpandEnvironmentVariables(@"%AppData%\Microsoft\Windows\Start Menu"); 

                        break;
                    }

                case "Test Directory":
                    {
                        start_Directory = @"D:\Grant - Dev - DaVinci Man Start Tree\Test Start Directory";

                        break;
                    }

                case "Custom Start 1":
                    {
                        start_Directory = @"D:\Grant - Dev - DaVinci Man Start Tree\Test Start Directory";

                        break;
                    }

                case "Custom Start 2":
                    {
                        start_Directory = @"D:\Grant - Dev - DaVinci Man Start Tree\Test Start Directory";

                        break;
                    }

                default:
                    {
                        //start_Directory = start_Directory;

                        break;
                    }
            }

            Populate_TreeView(start_Directory);
        }


        private void simpleButton_Expand_Click(object sender, EventArgs e)
        {
            if (simpleButton_Expand.Text == "Expand")
            {
                simpleButton_Expand.Text = "Collapse";

                treeView_Main.ExpandAll();
            }
            else
            {
                simpleButton_Expand.Text = "Expand";

                treeView_Main.CollapseAll();
            }
        }

        private void treeView_Main_DoubleClick(object sender, EventArgs e)
        {

        }

        private void treeView_Main_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode clicked_Node = e.Node;

            string clicked_Node_Name = clicked_Node.Text;

            string clicked_Node_Tag = clicked_Node.Tag.ToString();

            try
            {
                Process p = new Process();

                if (checkEdit_Run_As_Administrator.Checked == true)
                {
                    p.StartInfo.Verb = "runas";
                }


                if (checkEdit_Run_As_User_Prompt.Checked == true)
                {
                    // Prompt for Parameters.

                    p.StartInfo.UserName = "Administrator";
 //                   p.StartInfo.Password = "password";

                }


                if (checkEdit_Prompt_For_Parameters.Checked == true)
                {
                    // Prompt for Parameters.



                }


                // Process Options to Implement:

   //             p.StartInfo.FileName = "process.exe";
   //             p.StartInfo.Arguments = "-n";
   //             p.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
   //             p.StartInfo.CreateNoWindow = false;                           // Used when running command line programs.
   //             p.StartInfo.Domain = 
                //             p.StartInfo.EnvironmentVariables = true;
                //p.StartInfo.RedirectStandardError = true;
                //p.StartInfo.RedirectStandardInput = true;
                //p.StartInfo.RedirectStandardOutput = true;
                //p.StartInfo.UseShellExecute = true;
                //p.StartInfo.Verb = "adsf";
                //p.StartInfo.Verbs = 
                p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;

                string system_Root = Environment.GetEnvironmentVariable("SYSTEMROOT");
                p.StartInfo.WorkingDirectory = system_Root;


                Process.Start(clicked_Node_Tag);


                if (checkEdit_Wait_For_Program_Exit.Checked == true)
                {
                    p.WaitForExit();              // Waits here for the process to exit.
                }

            }
            catch (Exception ex)
            {
                string error_msg = ex.ToString();

                MessageBox.Show(error_msg, "Tree Item Start Error!");
            }


        }






    }
}