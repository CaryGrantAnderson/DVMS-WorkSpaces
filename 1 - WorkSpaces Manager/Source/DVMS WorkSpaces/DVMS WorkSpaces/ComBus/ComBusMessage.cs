using System;
using System.Collections.Generic;

namespace DVMS_WorkSpaces.ComBus
{
    public sealed class ComBusMessage
    {
        public ComBusMessage()
        {
            MessageId = Guid.NewGuid().ToString("N");
            TimestampUtc = DateTime.UtcNow;
            SchemaVersion = 1;
            Payload = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            Metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string MessageId { get; set; }
        public string CorrelationId { get; set; }
        public string SourceInstanceId { get; set; }
        public string DestinationInstanceId { get; set; }
        public string MessageType { get; set; }
        public DateTime TimestampUtc { get; set; }
        public int SchemaVersion { get; set; }
        public IDictionary<string, object> Payload { get; private set; }
        public IDictionary<string, string> Metadata { get; private set; }
    }
}
