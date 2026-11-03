using DevExpress.XtraBars;
using DevExpress.XtraDialogs.FileExplorerExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace DevExpress.XtraDialogs.Demos
{
    public partial class RibbonFormMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {

        readonly GridControlExtension gridControlExtension;
        readonly BreadCrumbExtension breadCrumb;



        public RibbonFormMain()
        {
            InitializeComponent();

            simpleButton_OpenRunFile.Appearance.Options.UseBackColor = true;

            if (!fileExplorerAssistant1.IsDesignMode)
            {
                //<gridControl>
                gridControlExtension = fileExplorerAssistant1.Attach(gridControl1, x => {
                    x.FilterString = "Text files (*.txt)|*.txt";                                                // To Do!!!
                });

                gridControlExtension.FocusedLinkChanged += OnListViewFocusedLinkChanged;
                gridControlExtension.CurrentItemChanged += OnListViewCurrentItemChanged;

                //</gridControl>
                gridControlExtension.AllowGoBackChanged += OnUpdateNavigationButtons;
                gridControlExtension.AllowGoForwardChanged += OnUpdateNavigationButtons;
                gridControlExtension.AllowGoUpChanged += OnUpdateNavigationButtons;

                //<currentPathEdit>
                breadCrumb = fileExplorerAssistant1.Attach(breadCrumbEdit_Path, x => {
                    x.CurrentItemChanged += OnCurrentPathEditCurrentItemChanged;
                });


                // TreeList - Current Path 
                fileExplorerAssistant1.Attach(treeList_NavTree, x => {
                    x.RootNodes.Add(new EnvironmentSpecialFolderNode(Environment.SpecialFolder.MyComputer));            // To Do!!!
                    x.CurrentItemChanged += OnTreeCurrentItemChanged;
                });


                simpleButton_OpenRunFile.Enabled = false;
            }
        }

        private void RibbonFormMain_Load(object sender, EventArgs e)
        {


        }


        void OnUpdateNavigationButtons(object sender, EventArgs e)
        {
            simpleButton_Up.Enabled = gridControlExtension.CanGoUp;
            simpleButton_Back.Enabled = gridControlExtension.CanGoBack;
            simpleButton_Forward.Enabled = gridControlExtension.CanGoForward;
        }


        void OnListViewFocusedLinkChanged(object sender, FocusedLinkChangedEventArgs e)
        {
            if (e.FocusedLink != null && e.FocusedLink.Extension == ".txt")
            {
                textEdit_SelectedFile.Tag = e.FocusedLink.FullPath;
                textEdit_SelectedFile.Text = e.FocusedLink.Identifier;

                simpleButton_OpenRunFile.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Success;
            }
            else
            {
                textEdit_SelectedFile.Tag = null;
                textEdit_SelectedFile.Text = null;

                simpleButton_OpenRunFile.Appearance.Reset();
            }

            if (textEdit_SelectedFile.Tag != null)
            {
                if (string.IsNullOrEmpty(textEdit_SelectedFile.Tag.ToString()))
                {
                    simpleButton_OpenRunFile.Enabled = false;
                }
                else
                {
                    simpleButton_OpenRunFile.Enabled = true;
                }
            }
        }


        void OnListViewCurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            breadCrumb.SetCurrentItem(e.CurrentItem);
        }


        //</gridControl>
        //<treeList>
        void OnTreeCurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            gridControlExtension.SetCurrentItem(e.CurrentItem);
        }


        //</treeList>
        //<currentPathEdit>
        void OnCurrentPathEditCurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            gridControlExtension.SetCurrentItem(e.CurrentItem);
        }


        //</currentPathEdit>
        void btnBack_Click(object sender, EventArgs e)
        {
            gridControlExtension.GoBack();
        }


        void btnForward_Click(object sender, EventArgs e)
        {
            gridControlExtension.GoForward();
        }


        void btnUp_Click(object sender, EventArgs e)
        {
            gridControlExtension.GoUp();
        }


        void btnOpenFile_Click(object sender, EventArgs e)
        {
            string path = textEdit_SelectedFile.Text;

            if (System.IO.File.Exists(path))
            {
                DevExpress.Data.Utils.SafeProcess.Open(path);
            }
        }


        private void breadCrumbEdit_Path_PathChanged(object sender, XtraEditors.BreadCrumbPathChangedEventArgs e)
        {

        }


        private void simpleButton_OpenDocument_Click(object sender, EventArgs e)
        {

        }

        private void simpleButton_OpenRunFile_Click(object sender, EventArgs e)
        {

        }
    }
}