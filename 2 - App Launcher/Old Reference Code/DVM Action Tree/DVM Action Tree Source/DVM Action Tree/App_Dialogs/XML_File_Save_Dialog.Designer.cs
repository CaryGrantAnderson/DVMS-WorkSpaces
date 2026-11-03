namespace DVM_Action_Tree.App_Dialogs
{
    partial class XML_File_Save_Dialog
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
            this.simpleButton_Select = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_Cancel = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_Save = new DevExpress.XtraEditors.SimpleButton();
            this.textEdit_XML_File = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_XML_File.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // simpleButton_Select
            // 
            this.simpleButton_Select.Location = new System.Drawing.Point(420, 53);
            this.simpleButton_Select.Name = "simpleButton_Select";
            this.simpleButton_Select.Size = new System.Drawing.Size(75, 23);
            this.simpleButton_Select.TabIndex = 11;
            this.simpleButton_Select.Text = "Select";
            this.simpleButton_Select.Click += new System.EventHandler(this.simpleButton_Select_Click);
            // 
            // simpleButton_Cancel
            // 
            this.simpleButton_Cancel.Location = new System.Drawing.Point(128, 99);
            this.simpleButton_Cancel.Name = "simpleButton_Cancel";
            this.simpleButton_Cancel.Size = new System.Drawing.Size(75, 23);
            this.simpleButton_Cancel.TabIndex = 10;
            this.simpleButton_Cancel.Text = "Cancel";
            this.simpleButton_Cancel.Click += new System.EventHandler(this.simpleButton_Cancel_Click);
            // 
            // simpleButton_Save
            // 
            this.simpleButton_Save.Location = new System.Drawing.Point(269, 99);
            this.simpleButton_Save.Name = "simpleButton_Save";
            this.simpleButton_Save.Size = new System.Drawing.Size(75, 23);
            this.simpleButton_Save.TabIndex = 9;
            this.simpleButton_Save.Text = "Save";
            this.simpleButton_Save.Click += new System.EventHandler(this.simpleButton_Save_Click);
            // 
            // textEdit_XML_File
            // 
            this.textEdit_XML_File.Location = new System.Drawing.Point(82, 56);
            this.textEdit_XML_File.Name = "textEdit_XML_File";
            this.textEdit_XML_File.Size = new System.Drawing.Size(323, 20);
            this.textEdit_XML_File.TabIndex = 8;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(24, 59);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(42, 13);
            this.labelControl2.TabIndex = 7;
            this.labelControl2.Text = "XML File:";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(12, 24);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(282, 13);
            this.labelControl1.TabIndex = 6;
            this.labelControl1.Text = "Select the XML Tree file to Save to or enter XML File Name.";
            // 
            // XML_File_Save_Dialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 133);
            this.Controls.Add(this.simpleButton_Select);
            this.Controls.Add(this.simpleButton_Cancel);
            this.Controls.Add(this.simpleButton_Save);
            this.Controls.Add(this.textEdit_XML_File);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Name = "XML_File_Save_Dialog";
            this.Text = "XML File Save Dialog";
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_XML_File.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton simpleButton_Select;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Cancel;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Save;
        private DevExpress.XtraEditors.TextEdit textEdit_XML_File;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;

    }
}