using DevExpress.XtraEditors;
using DVMS_WorkSpaces.Controls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DVMS_WorkSpaces.AppSystems.Ping
{
    public sealed class PingWidgetControl : WidgetContainerControl
    {
        private readonly PingComponent component;
        private readonly LabelControl stateLabel;

        public PingWidgetControl(PingComponent component)
        {
            this.component = component;
            Title = "Ping";

            stateLabel = new LabelControl
            {
                Dock = DockStyle.Top,
                Height = 70,
                AutoSizeMode = LabelAutoSizeMode.None,
                Padding = new Padding(12),
                Appearance = { Font = new Font("Segoe UI", 12F), TextOptions = { WordWrap = DevExpress.Utils.WordWrap.Wrap } }
            };
            SimpleButton simulateButton = new SimpleButton { Dock = DockStyle.Top, Height = 34, Text = "Simulate failure / recovery" };
            SimpleButton miniAppButton = new SimpleButton { Dock = DockStyle.Top, Height = 34, Text = "Open Ping Mini App" };
            simulateButton.Click += delegate { component.ToggleSimulatedFailure(); };
            miniAppButton.Click += delegate
            {
                EventHandler handler = OpenMiniAppRequested;
                if (handler != null) handler(this, EventArgs.Empty);
            };

            ContentControls.Add(miniAppButton);
            ContentControls.Add(simulateButton);
            ContentControls.Add(stateLabel);
            component.StatusChanged += Component_StatusChanged;
            RefreshState();
        }

        public event EventHandler OpenMiniAppRequested;

        private void Component_StatusChanged(object sender, EventArgs e)
        {
            if (IsHandleCreated && InvokeRequired)
                BeginInvoke(new Action(RefreshState));
            else
                RefreshState();
        }

        private void RefreshState()
        {
            stateLabel.Text = component.StatusText + Environment.NewLine + "Instance: " + component.Registration.InstanceId.Substring(0, 8);
            stateLabel.Appearance.ForeColor = component.Health == Core.ComponentHealth.Failed ? Color.Firebrick : Color.SeaGreen;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) component.StatusChanged -= Component_StatusChanged;
            base.Dispose(disposing);
        }
    }
}
