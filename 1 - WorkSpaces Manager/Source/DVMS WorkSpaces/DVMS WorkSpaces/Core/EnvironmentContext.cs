using System;
using System.Collections.Generic;

namespace DVMS_WorkSpaces.Core
{
    public enum HostKind
    {
        WorkSpaces,
        WindowsDesktop,
        WindowsService,
        Server,
        Web,
        Other
    }

    public sealed class EnvironmentContext
    {
        private readonly HashSet<string> capabilities;

        public EnvironmentContext(HostKind hostKind, string hostInstanceId, IEnumerable<string> capabilities)
        {
            HostKind = hostKind;
            HostInstanceId = hostInstanceId;
            OperatingSystem = Environment.OSVersion.ToString();
            this.capabilities = new HashSet<string>(capabilities ?? new string[0], StringComparer.OrdinalIgnoreCase);
        }

        public HostKind HostKind { get; private set; }
        public string HostInstanceId { get; private set; }
        public string OperatingSystem { get; private set; }
        public IEnumerable<string> Capabilities { get { return capabilities; } }

        public bool Supports(string capability)
        {
            return capabilities.Contains(capability);
        }

        public static EnvironmentContext CreateWorkSpacesHost()
        {
            return new EnvironmentContext(
                HostKind.WorkSpaces,
                Guid.NewGuid().ToString("N"),
                new[] { "Docking", "FreePlacement", "IntegratedComBus", "Persistence", "Notifications" });
        }
    }
}
