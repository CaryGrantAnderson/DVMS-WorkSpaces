using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DVM_Action_Tree.App_Dialogs
{
    public partial class XML_File_Save_Dialog : Form
    {

        private string file_Name = string.Empty;

        public string File_Name
        {
            get
            {
                return file_Name;
            }

            set
            {
                file_Name = value;
            }
        }


        public XML_File_Save_Dialog()
        {
            InitializeComponent();
        }

        private void simpleButton_Select_Click(object sender, EventArgs e)
        {
            SaveFileDialog save_Dialog = new SaveFileDialog();

            save_Dialog.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
            save_Dialog.FilterIndex = 1;
            save_Dialog.RestoreDirectory = true;

            if (save_Dialog.ShowDialog() == DialogResult.OK)
            {
                textEdit_XML_File.Text = save_Dialog.FileName;
            }
            else
            {
                textEdit_XML_File.Text = string.Empty;
            }
        }

        private void simpleButton_Cancel_Click(object sender, EventArgs e)
        {
            file_Name = string.Empty;

            this.DialogResult = DialogResult.Cancel;

            this.Close();
        }

        private void simpleButton_Save_Click(object sender, EventArgs e)
        {
            file_Name = textEdit_XML_File.Text.Trim();

            this.DialogResult = DialogResult.OK;

            this.Close();
        }
    }
}
