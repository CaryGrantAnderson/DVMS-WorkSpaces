using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DVMS_WorkSpaces.Controls
{
    public class WidgetContainerControl : XtraUserControl
    {
        private readonly LabelControl titleLabel;
        private readonly SimpleButton collapseButton;
        private readonly PanelControl contentPanel;

        public WidgetContainerControl()
        {
            Size = new Size(320, 210);
            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            PanelControl header = new PanelControl { Dock = DockStyle.Top, Height = 34 };
            titleLabel = new LabelControl
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(8, 0, 0, 0),
                Appearance = { Font = new Font("Segoe UI", 10F, FontStyle.Bold) },
                Text = "Widget"
            };
            collapseButton = new SimpleButton { Dock = DockStyle.Right, Width = 36, Text = "—" };
            collapseButton.Click += delegate
            {
                EventHandler handler = CollapseRequested;
                if (handler != null) handler(this, EventArgs.Empty);
            };
            header.Controls.Add(titleLabel);
            header.Controls.Add(collapseButton);

            contentPanel = new PanelControl { Dock = DockStyle.Fill, BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder };
            Controls.Add(contentPanel);
            Controls.Add(header);
        }

        public event EventHandler CollapseRequested;

        public string Title
        {
            get { return titleLabel.Text; }
            set { titleLabel.Text = value; }
        }

        public Control.ControlCollection ContentControls { get { return contentPanel.Controls; } }
    }
}
