using DVMS_WorkSpaces.ComBus;
using DVMS_WorkSpaces.Core;
using System;
using System.Threading;

namespace DVMS_WorkSpaces.AppSystems.Ping
{
    public sealed class PingComponent : IAppSystemComponent
    {
        public const string AppSystemId = "dvms.ping";
        public const string StatusChangedMessage = "ping.status-changed";

        private readonly IComBus comBus;
        private Timer heartbeatTimer;

        public PingComponent(IComBus comBus, EnvironmentContext environment)
        {
            this.comBus = comBus;
            Registration = new ComponentRegistration
            {
                InstanceId = Guid.NewGuid().ToString("N"),
                AppSystemId = AppSystemId,
                DisplayName = "Local Ping",
                HostInstanceId = environment.HostInstanceId,
                HostKind = environment.HostKind
            };
            Registration.Capabilities.Add(CommonCommands.GetStatus);
            Registration.Capabilities.Add(CommonCommands.Expand);
            Registration.Capabilities.Add(CommonCommands.OpenFullApp);
            Health = ComponentHealth.Healthy;
        }

        public ComponentRegistration Registration { get; private set; }
        public ComponentHealth Health { get; private set; }
        public string StatusText { get; private set; } = "All systems reachable";
        public event EventHandler StatusChanged;

        public void Start()
        {
            comBus.Register(Registration, HandleMessage);
            heartbeatTimer = new Timer(delegate { comBus.Heartbeat(Registration.InstanceId); }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            PublishStatus();
        }

        public void ToggleSimulatedFailure()
        {
            Health = Health == ComponentHealth.Failed ? ComponentHealth.Healthy : ComponentHealth.Failed;
            StatusText = Health == ComponentHealth.Healthy ? "All systems reachable" : "1 ping target failed";
            PublishStatus();
            EventHandler handler = StatusChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void HandleMessage(ComBusMessage message)
        {
            if (message == null) return;
            if (message.MessageType == CommonCommands.GetStatus || message.MessageType == CommonCommands.Refresh)
                PublishStatus(message.SourceInstanceId, message.MessageId);
        }

        private void PublishStatus(string destination = null, string correlationId = null)
        {
            ComBusMessage message = new ComBusMessage
            {
                SourceInstanceId = Registration.InstanceId,
                DestinationInstanceId = destination,
                CorrelationId = correlationId,
                MessageType = StatusChangedMessage
            };
            message.Payload["health"] = Health.ToString();
            message.Payload["status"] = StatusText;
            comBus.Publish(message);
        }

        public void Dispose()
        {
            if (heartbeatTimer != null) heartbeatTimer.Dispose();
            comBus.Unregister(Registration.InstanceId);
        }
    }
}
