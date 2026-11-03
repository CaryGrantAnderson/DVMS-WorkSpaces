namespace DVM_Action_Tree.App_Dialogs
{
    partial class XML_File_Load_Dialog
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
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.textEdit_XML_File = new DevExpress.XtraEditors.TextEdit();
            this.simpleButton_Save = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_Cancel = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_Select = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_XML_File.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(12, 22);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(155, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Select the XML Tree file to Load.";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(24, 57);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(42, 13);
            this.labelControl2.TabIndex = 1;
            this.labelControl2.Text = "XML File:";
            // 
            // textEdit_XML_File
            // 
            this.textEdit_XML_File.Location = new System.Drawing.Point(82, 54);
            this.textEdit_XML_File.Name = "textEdit_XML_File";
            this.textEdit_XML_File.Size = new System.Drawing.Size(323, 20);
            this.textEdit_XML_File.TabIndex = 2;
            // 
            // simpleButton_Save
            // 
            this.simpleButton_Save.Location = new System.Drawing.Point(269, 97);
            this.simpleButton_Save.Name = "simpleButton_Save";
            this.simpleButton_Save.Size = new System.Drawing.Size(75, 23);
            this.simpleButton_Save.TabIndex = 3;
            this.simpleButton_Save.Text = "Load";
            this.simpleButton_Save.Click += new System.EventHandler(this.simpleButton_Save_Click);
            // 
            // simpleButton_Cancel
            // 
            this.simpleButton_Cancel.Location = new System.Drawing.Point(128, 97);
            this.simpleButton_Cancel.Name = "simpleButton_Cancel";
            this.simpleButton_Cancel.Size = new System.Drawing.Size(75, 23);
            this.simpleButton_Cancel.TabIndex = 4;
            this.simpleButton_Cancel.Text = "Cancel";
            this.simpleButton_Cancel.Click += new System.EventHandler(this.simpleButton_Cancel_Click);
            // 
            // simpleButton_Select
            // 
            this.simpleButton_Select.Location = new System.Drawing.Point(420, 51);
            this.simpleButton_Select.Name = "simpleButton_Select";
            this.simpleButton_Select.Size = new System.Drawing.Size(75, 23);
            this.simpleButton_Select.TabIndex = 5;
            this.simpleButton_Select.Text = "Select";
            this.simpleButton_Select.Click += new System.EventHandler(this.simpleButton_Select_Click);
            // 
            // XML_File_Load_Dialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 141);
            this.Controls.Add(this.simpleButton_Select);
            this.Controls.Add(this.simpleButton_Cancel);
            this.Controls.Add(this.simpleButton_Save);
            this.Controls.Add(this.textEdit_XML_File);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Name = "XML_File_Load_Dialog";
            this.Text = "XML File Load Dialog";
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_XML_File.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit textEdit_XML_File;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Save;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Cancel;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Select;
    }
}