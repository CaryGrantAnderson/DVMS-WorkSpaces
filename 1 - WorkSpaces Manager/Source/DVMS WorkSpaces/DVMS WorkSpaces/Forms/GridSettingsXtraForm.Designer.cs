namespace DVMS_WorkSpaces.Forms
{
    partial class GridSettingsXtraForm
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
            this.checkEdit_ShowGrid = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl_GridType = new DevExpress.XtraEditors.LabelControl();
            this.comboBoxEdit_GridType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.checkEdit_LockGrid = new DevExpress.XtraEditors.CheckEdit();
            this.checkEdit_SnapToGrid = new DevExpress.XtraEditors.CheckEdit();
            this.numericUpDown_GridSpacing = new System.Windows.Forms.NumericUpDown();
            this.labelControl_GridSpacing = new DevExpress.XtraEditors.LabelControl();
            this.simpleButton_Save = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_Cancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_ShowGrid.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit_GridType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_LockGrid.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_SnapToGrid.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_GridSpacing)).BeginInit();
            this.SuspendLayout();
            // 
            // checkEdit_ShowGrid
            // 
            this.checkEdit_ShowGrid.Location = new System.Drawing.Point(28, 12);
            this.checkEdit_ShowGrid.Name = "checkEdit_ShowGrid";
            this.checkEdit_ShowGrid.Properties.Caption = "Show Grid";
            this.checkEdit_ShowGrid.Size = new System.Drawing.Size(75, 20);
            this.checkEdit_ShowGrid.TabIndex = 5;
            // 
            // labelControl_GridType
            // 
            this.labelControl_GridType.Location = new System.Drawing.Point(28, 101);
            this.labelControl_GridType.Name = "labelControl_GridType";
            this.labelControl_GridType.Size = new System.Drawing.Size(50, 13);
            this.labelControl_GridType.TabIndex = 4;
            this.labelControl_GridType.Text = "Grid Type:";
            this.labelControl_GridType.Click += new System.EventHandler(this.labelControl_GridType_Click);
            // 
            // comboBoxEdit_GridType
            // 
            this.comboBoxEdit_GridType.Location = new System.Drawing.Point(108, 98);
            this.comboBoxEdit_GridType.Name = "comboBoxEdit_GridType";
            this.comboBoxEdit_GridType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.comboBoxEdit_GridType.Size = new System.Drawing.Size(64, 20);
            this.comboBoxEdit_GridType.TabIndex = 3;
            this.comboBoxEdit_GridType.SelectedIndexChanged += new System.EventHandler(this.comboBoxEdit_GridType_SelectedIndexChanged);
            // 
            // checkEdit_LockGrid
            // 
            this.checkEdit_LockGrid.Location = new System.Drawing.Point(28, 64);
            this.checkEdit_LockGrid.Name = "checkEdit_LockGrid";
            this.checkEdit_LockGrid.Properties.Caption = "Lock Grid";
            this.checkEdit_LockGrid.Size = new System.Drawing.Size(75, 20);
            this.checkEdit_LockGrid.TabIndex = 10;
            // 
            // checkEdit_SnapToGrid
            // 
            this.checkEdit_SnapToGrid.Location = new System.Drawing.Point(28, 38);
            this.checkEdit_SnapToGrid.Name = "checkEdit_SnapToGrid";
            this.checkEdit_SnapToGrid.Properties.Caption = "Snap To Grid";
            this.checkEdit_SnapToGrid.Size = new System.Drawing.Size(87, 20);
            this.checkEdit_SnapToGrid.TabIndex = 9;
            // 
            // numericUpDown_GridSpacing
            // 
            this.numericUpDown_GridSpacing.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDown_GridSpacing.Location = new System.Drawing.Point(108, 130);
            this.numericUpDown_GridSpacing.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDown_GridSpacing.Name = "numericUpDown_GridSpacing";
            this.numericUpDown_GridSpacing.Size = new System.Drawing.Size(64, 21);
            this.numericUpDown_GridSpacing.TabIndex = 8;
            this.numericUpDown_GridSpacing.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown_GridSpacing.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDown_GridSpacing.ValueChanged += new System.EventHandler(this.numericUpDown_GridSpacing_ValueChanged);
            // 
            // labelControl_GridSpacing
            // 
            this.labelControl_GridSpacing.Location = new System.Drawing.Point(28, 132);
            this.labelControl_GridSpacing.Name = "labelControl_GridSpacing";
            this.labelControl_GridSpacing.Size = new System.Drawing.Size(63, 13);
            this.labelControl_GridSpacing.TabIndex = 7;
            this.labelControl_GridSpacing.Text = "Grid Spacing:";
            this.labelControl_GridSpacing.Click += new System.EventHandler(this.labelControl_GridSpacing_Click);
            // 
            // simpleButton_Save
            // 
            this.simpleButton_Save.Location = new System.Drawing.Point(126, 181);
            this.simpleButton_Save.Name = "simpleButton_Save";
            this.simpleButton_Save.Size = new System.Drawing.Size(75, 23);
            this.simpleButton_Save.TabIndex = 11;
            this.simpleButton_Save.Text = "Save";
            this.simpleButton_Save.Click += new System.EventHandler(this.simpleButton_Save_Click);
            // 
            // simpleButton_Cancel
            // 
            this.simpleButton_Cancel.Location = new System.Drawing.Point(28, 181);
            this.simpleButton_Cancel.Name = "simpleButton_Cancel";
            this.simpleButton_Cancel.Size = new System.Drawing.Size(75, 23);
            this.simpleButton_Cancel.TabIndex = 12;
            this.simpleButton_Cancel.Text = "Cancel";
            this.simpleButton_Cancel.Click += new System.EventHandler(this.simpleButton_Cancel_Click);
            // 
            // GridSettingsXtraForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(213, 222);
            this.ControlBox = false;
            this.Controls.Add(this.simpleButton_Cancel);
            this.Controls.Add(this.simpleButton_Save);
            this.Controls.Add(this.checkEdit_LockGrid);
            this.Controls.Add(this.checkEdit_SnapToGrid);
            this.Controls.Add(this.numericUpDown_GridSpacing);
            this.Controls.Add(this.labelControl_GridSpacing);
            this.Controls.Add(this.checkEdit_ShowGrid);
            this.Controls.Add(this.labelControl_GridType);
            this.Controls.Add(this.comboBoxEdit_GridType);
            this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Shadow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GridSettingsXtraForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Grid Settings";
            this.Load += new System.EventHandler(this.GridSettingsXtraForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_ShowGrid.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit_GridType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_LockGrid.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_SnapToGrid.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_GridSpacing)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.CheckEdit checkEdit_ShowGrid;
        private DevExpress.XtraEditors.LabelControl labelControl_GridType;
        private DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit_GridType;
        private DevExpress.XtraEditors.CheckEdit checkEdit_LockGrid;
        private DevExpress.XtraEditors.CheckEdit checkEdit_SnapToGrid;
        private System.Windows.Forms.NumericUpDown numericUpDown_GridSpacing;
        private DevExpress.XtraEditors.LabelControl labelControl_GridSpacing;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Save;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Cancel;
    }
}