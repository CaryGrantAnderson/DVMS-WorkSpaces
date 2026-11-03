using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using DVMS_WorkSpaces.AppSystems.Ping;
using DVMS_WorkSpaces.ComBus;
using DVMS_WorkSpaces.Controls;
using DVMS_WorkSpaces.Core;
using DVMS_WorkSpaces.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVMS_WorkSpaces.Forms
{
    public partial class WorkSpace_XtraForm1 : DevExpress.XtraEditors.XtraForm
    {
        private bool ShowGrid;
        private readonly IComBus comBus;
        private readonly EnvironmentContext environmentContext;
        private readonly bool ownsComBus;
        private readonly WorkspaceDocument workspaceDocument;
        private PingComponent pingComponent;
        private WorkspaceButtonControl pingButton;
        private PingWidgetControl pingWidget;

        private GridType gridType = GridType.Lines;
        public int GridSize { get; set; } = 20;

        enum GridType
        {
            Lines,
            Dots
        }


        public WorkSpace_XtraForm1()
            : this(new LocalComBus(), EnvironmentContext.CreateWorkSpacesHost(), true)
        {
        }

        public WorkSpace_XtraForm1(IComBus comBus, EnvironmentContext environmentContext)
            : this(comBus, environmentContext, false)
        {
        }

        private WorkSpace_XtraForm1(IComBus comBus, EnvironmentContext environmentContext, bool ownsComBus)
        {
            this.comBus = comBus;
            this.environmentContext = environmentContext;
            this.ownsComBus = ownsComBus;
            workspaceDocument = new WorkspaceDocument { Title = "Personal WorkSpace" };
            InitializeComponent();

            // Prevents screen flicker.
            this.DoubleBuffered = true;
            FormClosed += WorkSpace_XtraForm1_FormClosed;
        }

        private void WorkSpace_XtraForm1cs_Load(object sender, EventArgs e)
        {
            ShowGrid = true;
            GridSize = 20;

            checkEdit_ShowGrid.Checked = ShowGrid;

            comboBoxEdit_GridType.Properties.Items.Add("Lines");
            comboBoxEdit_GridType.Properties.Items.Add("Dots");
            comboBoxEdit_GridType.SelectedIndex = 0;    // Default to Lines.

            CreatePingPrototype();
        }

        private void CreatePingPrototype()
        {
            pingComponent = new PingComponent(comBus, environmentContext);
            pingComponent.Start();

            pingButton = new WorkspaceButtonControl
            {
                Caption = "PING",
                ComponentInstanceId = pingComponent.Registration.InstanceId,
                Location = new Point(60, 90)
            };
            pingButton.SetHealth(pingComponent.Health, "Healthy");
            pingButton.ExpandRequested += delegate { ShowPingWidget(); };
            pingComponent.StatusChanged += delegate
            {
                if (!IsDisposed)
                    BeginInvoke(new Action(delegate { pingButton.SetHealth(pingComponent.Health, pingComponent.Health.ToString()); }));
            };
            Controls.Add(pingButton);
            pingButton.BringToFront();
            EnableDragging(pingButton);

            workspaceDocument.Items.Add(new WorkspaceItemPlacement
            {
                ComponentInstanceId = pingComponent.Registration.InstanceId,
                AppSystemId = PingComponent.AppSystemId,
                Presentation = PresentationKind.Button,
                HostKind = environmentContext.HostKind,
                X = pingButton.Left,
                Y = pingButton.Top,
                Width = pingButton.Width,
                Height = pingButton.Height
            });
            labelControl_Status.Text = "ComBus online • 1 App System registered";
        }

        private void ShowPingWidget()
        {
            if (pingWidget == null || pingWidget.IsDisposed)
            {
                pingWidget = new PingWidgetControl(pingComponent) { Location = pingButton.Location };
                pingWidget.CollapseRequested += delegate
                {
                    pingWidget.Hide();
                    pingButton.Show();
                };
                pingWidget.OpenMiniAppRequested += delegate
                {
                    using (PingMiniAppForm form = new PingMiniAppForm(pingComponent))
                        form.ShowDialog(this);
                };
                Controls.Add(pingWidget);
                EnableDragging(pingWidget);
            }

            pingButton.Hide();
            pingWidget.Show();
            pingWidget.BringToFront();
        }

        private void EnableDragging(Control control)
        {
            Point mouseDown = Point.Empty;
            control.MouseDown += delegate(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left) mouseDown = e.Location;
            };
            control.MouseMove += delegate(object sender, MouseEventArgs e)
            {
                if (e.Button != MouseButtons.Left) return;
                int x = control.Left + e.X - mouseDown.X;
                int y = control.Top + e.Y - mouseDown.Y;
                if (workspaceDocument.Grid.SnapToGrid)
                {
                    int spacing = workspaceDocument.Grid.Spacing;
                    x = (x / spacing) * spacing;
                    y = (y / spacing) * spacing;
                }
                control.Location = new Point(Math.Max(0, x), Math.Max(panelControl_Top.Bottom, y));
                workspaceDocument.IsDirty = true;
            };
        }

        private void WorkSpace_XtraForm1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (pingComponent != null) pingComponent.Dispose();
            if (ownsComBus) comBus.Dispose();
        }

        private void WorkSpace_XtraForm1cs_Paint(object sender, PaintEventArgs e)
        {
            if (!ShowGrid)
            {
                return;
            }

            switch (gridType)
            {
                case GridType.Lines:
                    DrawGridLines(e);
                    break;

                case GridType.Dots:
                    DrawGridDots(e);
                    break;
            }
        }

        private void DrawGridLines(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int cellSize = GridSize;

            using (Pen gridPen = new Pen(Color.LightGray, 1))
            {
                for (int x = 0; x < this.ClientSize.Width; x += cellSize)
                {
                    g.DrawLine(gridPen, x, 0, x, this.ClientSize.Height);
                }
                for (int y = 0; y < this.ClientSize.Height; y += cellSize)
                {
                    g.DrawLine(gridPen, 0, y, this.ClientSize.Width, y);
                }
            }
        }

        private void DrawGridDots(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int cellSize = GridSize;

            using (Brush gridBrush = new SolidBrush(Color.LightGray))
            {
                for (int x = 0; x < this.ClientSize.Width; x += cellSize)
                {
                    for (int y = 0; y < this.ClientSize.Height; y += cellSize)
                    {
                        g.FillEllipse(gridBrush, x - 1, y - 1, 2, 2); // Draw a small dot
                    }
                }
            }
        }

        private void comboBoxEdit_GridType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(comboBoxEdit_GridType.SelectedIndex)
            {
                case 0:
                    gridType = GridType.Lines;
                    break;
                case 1:
                    gridType = GridType.Dots;
                    break;
            }   

            this.Refresh(); 
        }

        private void numericUpDown_GridSpacing_ValueChanged(object sender, EventArgs e)
        {
            GridSize = (int)numericUpDown_GridSpacing.Value;

            this.Refresh(); 
        }

        private void checkEdit_ShowGrid_CheckedChanged(object sender, EventArgs e)
        {
            ShowGrid = checkEdit_ShowGrid.Checked;

            this.Refresh(); 
        }

        private void simpleButton_Setting_Click(object sender, EventArgs e)
        {
            Forms.GridSettingsXtraForm settingsXtraForm = new Forms.GridSettingsXtraForm();
            settingsXtraForm.ShowDialog();
        }

        private void panelControl_Top_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
