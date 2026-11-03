using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views.Tabbed;
using DevExpress.XtraEditors;
using DVMS_WorkSpaces.ComBus;
using DVMS_WorkSpaces.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DVMS_WorkSpaces
{
    public partial class DVMSWorkSpacesForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private readonly IComBus comBus;
        private readonly EnvironmentContext environmentContext;

        public DVMSWorkSpacesForm()
        {
            InitializeComponent();
            environmentContext = EnvironmentContext.CreateWorkSpacesHost();
            comBus = new LocalComBus();
            InitDocumentManager();
            FormClosed += delegate { comBus.Dispose(); };
        }

        private void InitDocumentManager()
        {
            documentManager1.MdiParent = this;
        }

        private void DVMSWorkSpacesForm_Load(object sender, EventArgs e)
        {
            
           

        }



        int documentCount = 0; // Counter to keep track of the number of documents
        private void barButtonItem_AddDocument_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            documentCount++;

            // 1. Create an instance of your custom control or form
            XtraUserControl myControl = new XtraUserControl();

            // 2. Add the control to your View (e.g., tabbedView1)
            DevExpress.XtraBars.Docking2010.Views.BaseDocument doc = tabbedView1.AddDocument(myControl);

            // 3. (Optional) Customize the appearance of the tab/document
            doc.Caption = "My New Document " + documentCount; 
        }

        private void barButtonItem_ClearAllDocuments_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            tabbedView1.Controller.CloseAll();

            documentCount = 0;
        }

        private void barButtonItem_ShowWorkSpacesForm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Forms.WorkSpace_XtraForm1 workSpaceForm = new Forms.WorkSpace_XtraForm1(comBus, environmentContext);
            workSpaceForm.Show();
        }


        int workSpaceFormCount = 0; // Counter to keep track of the number of WorkSpace forms.

        private void barButtonItem_AddWorkSpacesForm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            workSpaceFormCount++;

            Forms.WorkSpace_XtraForm1 workSpaceForm = new Forms.WorkSpace_XtraForm1(comBus, environmentContext);

            workSpaceForm.MdiParent = this;

            workSpaceForm.Text = "WorkSpace Form " + workSpaceFormCount;

            workSpaceForm.Show();
        }
    }
}
