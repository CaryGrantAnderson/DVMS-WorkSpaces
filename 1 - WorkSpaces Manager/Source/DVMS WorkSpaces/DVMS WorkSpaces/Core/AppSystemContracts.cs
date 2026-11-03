using System;
using System.Collections.Generic;
using DVMS_WorkSpaces.ComBus;

namespace DVMS_WorkSpaces.Core
{
    public enum PresentationKind
    {
        Button,
        Widget,
        MiniApp,
        FullApp
    }

    public enum ComponentHealth
    {
        Unknown,
        Healthy,
        Warning,
        Failed,
        Offline
    }

    public sealed class ComponentRegistration
    {
        public ComponentRegistration()
        {
            Capabilities = new List<string>();
            RegisteredUtc = DateTime.UtcNow;
            LastHeartbeatUtc = RegisteredUtc;
        }

        public string InstanceId { get; set; }
        public string AppSystemId { get; set; }
        public string DisplayName { get; set; }
        public string HostInstanceId { get; set; }
        public HostKind HostKind { get; set; }
        public IList<string> Capabilities { get; private set; }
        public DateTime RegisteredUtc { get; set; }
        public DateTime LastHeartbeatUtc { get; set; }
    }

    public static class CommonCommands
    {
        public const string Identify = "common.identify";
        public const string GetStatus = "common.get-status";
        public const string Activate = "common.activate";
        public const string Minimize = "common.minimize";
        public const string Expand = "common.expand";
        public const string OpenFullApp = "common.open-full-app";
        public const string Refresh = "common.refresh";
        public const string Shutdown = "common.shutdown";
        public const string Heartbeat = "common.heartbeat";
    }

    public interface IAppSystemComponent : IDisposable
    {
        ComponentRegistration Registration { get; }
        ComponentHealth Health { get; }
        void Start();
        void HandleMessage(ComBusMessage message);
    }
}
