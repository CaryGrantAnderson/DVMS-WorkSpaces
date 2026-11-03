using DVMS_WorkSpaces.Core;
using System;
using System.Collections.Generic;

namespace DVMS_WorkSpaces.ComBus
{
    public sealed class ComBusMessageEventArgs : EventArgs
    {
        public ComBusMessageEventArgs(ComBusMessage message) { Message = message; }
        public ComBusMessage Message { get; private set; }
    }

    public interface IComBus : IDisposable
    {
        event EventHandler<ComBusMessageEventArgs> MessagePublished;
        void Register(ComponentRegistration registration, Action<ComBusMessage> receiver);
        void Unregister(string instanceId);
        void Heartbeat(string instanceId);
        IList<ComponentRegistration> GetRegistrations(string appSystemId = null);
        void Publish(ComBusMessage message);
    }
}
