using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DVMS_WorkSpaces.AppSystems.Ping
{
    public sealed class PingMiniAppForm : XtraForm
    {
        private readonly PingComponent component;
        private readonly MemoEdit details;

        public PingMiniAppForm(PingComponent component)
        {
            this.component = component;
            Text = "Ping Mini App";
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(480, 300);
            Size = new Size(640, 400);

            details = new MemoEdit { Dock = DockStyle.Fill, Properties = { ReadOnly = true } };
            SimpleButton simulateButton = new SimpleButton { Dock = DockStyle.Bottom, Height = 38, Text = "Simulate failure / recovery" };
            simulateButton.Click += delegate { component.ToggleSimulatedFailure(); };
            Controls.Add(details);
            Controls.Add(simulateButton);
            component.StatusChanged += Component_StatusChanged;
            RefreshDetails();
        }

        private void Component_StatusChanged(object sender, EventArgs e) { RefreshDetails(); }

        private void RefreshDetails()
        {
            details.Text = "Ping App System" + Environment.NewLine + Environment.NewLine +
                "Instance ID: " + component.Registration.InstanceId + Environment.NewLine +
                "Health: " + component.Health + Environment.NewLine +
                "Status: " + component.StatusText + Environment.NewLine +
                "Last heartbeat: " + component.Registration.LastHeartbeatUtc.ToLocalTime().ToString("G") + Environment.NewLine + Environment.NewLine +
                "This is the first Mini App presentation. Target management and real ICMP checks will follow in the Ping App System project.";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) component.StatusChanged -= Component_StatusChanged;
            base.Dispose(disposing);
        }
    }
}
