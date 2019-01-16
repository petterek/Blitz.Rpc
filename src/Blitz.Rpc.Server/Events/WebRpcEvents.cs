using System.Collections.Generic;

namespace Blitz.Rpc.Server.Events
{
    public delegate void EndpointRegisteredEventHandler(object sender, RegistrationInfoEventArgs e);

    public delegate void UniqueNamespaceRegisteredEventHandler(object sender, UniqueNamespaceEventArgs e);

    public class WebRpcEvents
    {
        private readonly ServerConfig owner;

        public WebRpcEvents(ServerConfig owner)
        {
            this.owner = owner;
        }

        private List<string> RegisteredNamspace = new List<string>();

        internal void OnEndpointRegistered(RegistrationInfoEventArgs eventArgs)
        {
            EndpointRegistered?.Invoke(owner, eventArgs);

            string @namespace = eventArgs.Interface.Namespace;
            if (RegisteredNamspace.Contains(@namespace)) return;

            RegisteredNamspace.Add(@namespace);
            UniqueNamespaceRegistered?.Invoke(owner, new UniqueNamespaceEventArgs { Namespace = @namespace });
        }

        public event EndpointRegisteredEventHandler EndpointRegistered;

        public event UniqueNamespaceRegisteredEventHandler UniqueNamespaceRegistered;
    }
}