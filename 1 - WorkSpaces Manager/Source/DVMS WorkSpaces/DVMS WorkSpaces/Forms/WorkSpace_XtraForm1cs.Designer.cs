namespace DVMS_WorkSpaces.Forms
{
    partial class WorkSpace_XtraForm1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkSpace_XtraForm1));
            this.panelControl_Top = new DevExpress.XtraEditors.PanelControl();
            this.comboBoxEdit_GridType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl_GridType = new DevExpress.XtraEditors.LabelControl();
            this.checkEdit_ShowGrid = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl_GridSpacing = new DevExpress.XtraEditors.LabelControl();
            this.numericUpDown_GridSpacing = new System.Windows.Forms.NumericUpDown();
            this.checkEdit_SnapToGrid = new DevExpress.XtraEditors.CheckEdit();
            this.checkEdit_LockGrid = new DevExpress.XtraEditors.CheckEdit();
            this.simpleButton_Setting = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl_Status = new DevExpress.XtraEditors.PanelControl();
            this.labelControl_StatusLabel = new DevExpress.XtraEditors.LabelControl();
            this.labelControl_Status = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_Top)).BeginInit();
            this.panelControl_Top.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit_GridType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_ShowGrid.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_GridSpacing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_SnapToGrid.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_LockGrid.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_Status)).BeginInit();
            this.panelControl_Status.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl_Top
            // 
            this.panelControl_Top.Controls.Add(this.simpleButton_Setting);
            this.panelControl_Top.Controls.Add(this.checkEdit_LockGrid);
            this.panelControl_Top.Controls.Add(this.checkEdit_SnapToGrid);
            this.panelControl_Top.Controls.Add(this.numericUpDown_GridSpacing);
            this.panelControl_Top.Controls.Add(this.labelControl_GridSpacing);
            this.panelControl_Top.Controls.Add(this.checkEdit_ShowGrid);
            this.panelControl_Top.Controls.Add(this.labelControl_GridType);
            this.panelControl_Top.Controls.Add(this.comboBoxEdit_GridType);
            this.panelControl_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl_Top.Location = new System.Drawing.Point(0, 0);
            this.panelControl_Top.Name = "panelControl_Top";
            this.panelControl_Top.Size = new System.Drawing.Size(1148, 32);
            this.panelControl_Top.TabIndex = 0;
            this.panelControl_Top.Paint += new System.Windows.Forms.PaintEventHandler(this.panelControl_Top_Paint);
            // 
            // comboBoxEdit_GridType
            // 
            this.comboBoxEdit_GridType.Location = new System.Drawing.Point(152, 6);
            this.comboBoxEdit_GridType.Name = "comboBoxEdit_GridType";
            this.comboBoxEdit_GridType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.comboBoxEdit_GridType.Size = new System.Drawing.Size(64, 20);
            this.comboBoxEdit_GridType.TabIndex = 0;
            this.comboBoxEdit_GridType.SelectedIndexChanged += new System.EventHandler(this.comboBoxEdit_GridType_SelectedIndexChanged);
            // 
            // labelControl_GridType
            // 
            this.labelControl_GridType.Location = new System.Drawing.Point(96, 10);
            this.labelControl_GridType.Name = "labelControl_GridType";
            this.labelControl_GridType.Size = new System.Drawing.Size(50, 13);
            this.labelControl_GridType.TabIndex = 1;
            this.labelControl_GridType.Text = "Grid Type:";
            // 
            // checkEdit_ShowGrid
            // 
            this.checkEdit_ShowGrid.Location = new System.Drawing.Point(12, 7);
            this.checkEdit_ShowGrid.Name = "checkEdit_ShowGrid";
            this.checkEdit_ShowGrid.Properties.Caption = "Show Grid";
            this.checkEdit_ShowGrid.Size = new System.Drawing.Size(75, 20);
            this.checkEdit_ShowGrid.TabIndex = 2;
            this.checkEdit_ShowGrid.CheckedChanged += new System.EventHandler(this.checkEdit_ShowGrid_CheckedChanged);
            // 
            // labelControl_GridSpacing
            // 
            this.labelControl_GridSpacing.Location = new System.Drawing.Point(233, 9);
            this.labelControl_GridSpacing.Name = "labelControl_GridSpacing";
            this.labelControl_GridSpacing.Size = new System.Drawing.Size(63, 13);
            this.labelControl_GridSpacing.TabIndex = 3;
            this.labelControl_GridSpacing.Text = "Grid Spacing:";
            // 
            // numericUpDown_GridSpacing
            // 
            this.numericUpDown_GridSpacing.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDown_GridSpacing.Location = new System.Drawing.Point(302, 8);
            this.numericUpDown_GridSpacing.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDown_GridSpacing.Name = "numericUpDown_GridSpacing";
            this.numericUpDown_GridSpacing.Size = new System.Drawing.Size(53, 21);
            this.numericUpDown_GridSpacing.TabIndex = 4;
            this.numericUpDown_GridSpacing.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown_GridSpacing.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDown_GridSpacing.ValueChanged += new System.EventHandler(this.numericUpDown_GridSpacing_ValueChanged);
            // 
            // checkEdit_SnapToGrid
            // 
            this.checkEdit_SnapToGrid.Location = new System.Drawing.Point(370, 7);
            this.checkEdit_SnapToGrid.Name = "checkEdit_SnapToGrid";
            this.checkEdit_SnapToGrid.Properties.Caption = "Snap To Grid";
            this.checkEdit_SnapToGrid.Size = new System.Drawing.Size(87, 20);
            this.checkEdit_SnapToGrid.TabIndex = 5;
            // 
            // checkEdit_LockGrid
            // 
            this.checkEdit_LockGrid.Location = new System.Drawing.Point(463, 7);
            this.checkEdit_LockGrid.Name = "checkEdit_LockGrid";
            this.checkEdit_LockGrid.Properties.Caption = "Lock Grid";
            this.checkEdit_LockGrid.Size = new System.Drawing.Size(75, 20);
            this.checkEdit_LockGrid.TabIndex = 6;
            // 
            // simpleButton_Setting
            // 
            this.simpleButton_Setting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButton_Setting.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton_Setting.ImageOptions.Image")));
            this.simpleButton_Setting.Location = new System.Drawing.Point(1067, 5);
            this.simpleButton_Setting.Name = "simpleButton_Setting";
            this.simpleButton_Setting.Size = new System.Drawing.Size(69, 23);
            this.simpleButton_Setting.TabIndex = 7;
            this.simpleButton_Setting.Text = "Settings";
            this.simpleButton_Setting.Click += new System.EventHandler(this.simpleButton_Setting_Click);
            // 
            // panelControl_Status
            // 
            this.panelControl_Status.Controls.Add(this.labelControl_Status);
            this.panelControl_Status.Controls.Add(this.labelControl_StatusLabel);
            this.panelControl_Status.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl_Status.Location = new System.Drawing.Point(0, 573);
            this.panelControl_Status.Name = "panelControl_Status";
            this.panelControl_Status.Size = new System.Drawing.Size(1148, 25);
            this.panelControl_Status.TabIndex = 1;
            // 
            // labelControl_StatusLabel
            // 
            this.labelControl_StatusLabel.Location = new System.Drawing.Point(13, 6);
            this.labelControl_StatusLabel.Name = "labelControl_StatusLabel";
            this.labelControl_StatusLabel.Size = new System.Drawing.Size(35, 13);
            this.labelControl_StatusLabel.TabIndex = 0;
            this.labelControl_StatusLabel.Text = "Status:";
            // 
            // labelControl_Status
            // 
            this.labelControl_Status.Location = new System.Drawing.Point(65, 6);
            this.labelControl_Status.Name = "labelControl_Status";
            this.labelControl_Status.Size = new System.Drawing.Size(43, 13);
            this.labelControl_Status.TabIndex = 1;
            this.labelControl_Status.Text = "Status...";
            // 
            // WorkSpace_XtraForm1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1148, 598);
            this.Controls.Add(this.panelControl_Status);
            this.Controls.Add(this.panelControl_Top);
            this.IconOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("WorkSpace_XtraForm1cs.IconOptions.LargeImage")));
            this.Name = "WorkSpace_XtraForm1";
            this.Text = "WorkSpace";
            this.Load += new System.EventHandler(this.WorkSpace_XtraForm1cs_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.WorkSpace_XtraForm1cs_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_Top)).EndInit();
            this.panelControl_Top.ResumeLayout(false);
            this.panelControl_Top.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit_GridType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_ShowGrid.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_GridSpacing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_SnapToGrid.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit_LockGrid.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_Status)).EndInit();
            this.panelControl_Status.ResumeLayout(false);
            this.panelControl_Status.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl_Top;
        private DevExpress.XtraEditors.LabelControl labelControl_GridType;
        private DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit_GridType;
        private System.Windows.Forms.NumericUpDown numericUpDown_GridSpacing;
        private DevExpress.XtraEditors.LabelControl labelControl_GridSpacing;
        private DevExpress.XtraEditors.CheckEdit checkEdit_ShowGrid;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Setting;
        private DevExpress.XtraEditors.CheckEdit checkEdit_LockGrid;
        private DevExpress.XtraEditors.CheckEdit checkEdit_SnapToGrid;
        private DevExpress.XtraEditors.PanelControl panelControl_Status;
        private DevExpress.XtraEditors.LabelControl labelControl_Status;
        private DevExpress.XtraEditors.LabelControl labelControl_StatusLabel;
    }
}