using DVMS_WorkSpaces.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DVMS_WorkSpaces.ComBus
{
    // First local transport. The same IComBus boundary will be implemented by
    // the SignalR transport when the host moves out to a Windows Service.
    public sealed class LocalComBus : IComBus
    {
        private readonly object syncRoot = new object();
        private readonly Dictionary<string, ComponentRegistration> registrations = new Dictionary<string, ComponentRegistration>();
        private readonly Dictionary<string, Action<ComBusMessage>> receivers = new Dictionary<string, Action<ComBusMessage>>();

        public event EventHandler<ComBusMessageEventArgs> MessagePublished;

        public void Register(ComponentRegistration registration, Action<ComBusMessage> receiver)
        {
            if (registration == null || string.IsNullOrWhiteSpace(registration.InstanceId))
                throw new ArgumentException("A component registration requires an instance ID.", "registration");

            lock (syncRoot)
            {
                registration.RegisteredUtc = DateTime.UtcNow;
                registration.LastHeartbeatUtc = registration.RegisteredUtc;
                registrations[registration.InstanceId] = registration;
                receivers[registration.InstanceId] = receiver;
            }
        }

        public void Unregister(string instanceId)
        {
            lock (syncRoot)
            {
                registrations.Remove(instanceId);
                receivers.Remove(instanceId);
            }
        }

        public void Heartbeat(string instanceId)
        {
            lock (syncRoot)
            {
                ComponentRegistration registration;
                if (registrations.TryGetValue(instanceId, out registration))
                    registration.LastHeartbeatUtc = DateTime.UtcNow;
            }
        }

        public IList<ComponentRegistration> GetRegistrations(string appSystemId = null)
        {
            lock (syncRoot)
            {
                return registrations.Values
                    .Where(r => string.IsNullOrWhiteSpace(appSystemId) || string.Equals(r.AppSystemId, appSystemId, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }

        public void Publish(ComBusMessage message)
        {
            if (message == null || string.IsNullOrWhiteSpace(message.MessageType))
                throw new ArgumentException("A ComBus message requires a message type.", "message");

            List<Action<ComBusMessage>> targets;
            lock (syncRoot)
            {
                if (!string.IsNullOrWhiteSpace(message.DestinationInstanceId))
                {
                    Action<ComBusMessage> receiver;
                    targets = receivers.TryGetValue(message.DestinationInstanceId, out receiver)
                        ? new List<Action<ComBusMessage>> { receiver }
                        : new List<Action<ComBusMessage>>();
                }
                else
                {
                    targets = receivers
                        .Where(pair => !string.Equals(pair.Key, message.SourceInstanceId, StringComparison.OrdinalIgnoreCase))
                        .Select(pair => pair.Value)
                        .Where(receiver => receiver != null)
                        .ToList();
                }
            }

            foreach (Action<ComBusMessage> target in targets)
                target(message);

            EventHandler<ComBusMessageEventArgs> handler = MessagePublished;
            if (handler != null)
                handler(this, new ComBusMessageEventArgs(message));
        }

        public void Dispose()
        {
            lock (syncRoot)
            {
                registrations.Clear();
                receivers.Clear();
            }
        }
    }
}
