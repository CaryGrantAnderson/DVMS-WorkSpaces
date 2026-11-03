using DevExpress.XtraEditors;
using DVMS_WorkSpaces.Core;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DVMS_WorkSpaces.Controls
{
    public class WorkspaceButtonControl : XtraUserControl
    {
        private readonly SimpleButton actionButton;
        private readonly LabelControl statusLabel;

        public WorkspaceButtonControl()
        {
            Size = new Size(112, 104);
            BackColor = Color.Transparent;

            actionButton = new SimpleButton
            {
                Dock = DockStyle.Fill,
                Appearance = { Font = new Font("Segoe UI", 10F, FontStyle.Bold) },
                Text = "App"
            };
            actionButton.Click += delegate { OnExpandRequested(); };

            statusLabel = new LabelControl
            {
                Dock = DockStyle.Bottom,
                Height = 22,
                AutoSizeMode = LabelAutoSizeMode.None,
                Appearance = { TextOptions = { HAlignment = DevExpress.Utils.HorzAlignment.Center } },
                Text = "Unknown"
            };

            Controls.Add(actionButton);
            Controls.Add(statusLabel);
        }

        public event EventHandler ExpandRequested;
        public string ComponentInstanceId { get; set; }

        public string Caption
        {
            get { return actionButton.Text; }
            set { actionButton.Text = value; }
        }

        public void SetHealth(ComponentHealth health, string statusText)
        {
            statusLabel.Text = statusText;
            switch (health)
            {
                case ComponentHealth.Healthy:
                    actionButton.Appearance.BackColor = Color.FromArgb(111, 207, 151);
                    break;
                case ComponentHealth.Warning:
                    actionButton.Appearance.BackColor = Color.FromArgb(242, 201, 76);
                    break;
                case ComponentHealth.Failed:
                    actionButton.Appearance.BackColor = Color.FromArgb(235, 87, 87);
                    break;
                default:
                    actionButton.Appearance.BackColor = Color.LightGray;
                    break;
            }
            actionButton.Appearance.Options.UseBackColor = true;
        }

        protected virtual void OnExpandRequested()
        {
            EventHandler handler = ExpandRequested;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
