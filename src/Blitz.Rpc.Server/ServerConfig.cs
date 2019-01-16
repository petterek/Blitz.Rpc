using Blitz.Rpc.Server.Events;
using Blitz.Rpc.Server.Internals;
using System;
using System.Collections.Generic;

namespace Blitz.Rpc.Server
{
    public class ServerConfig
    {
        private readonly List<RegistrationInfo> Services = new List<RegistrationInfo>();

        private WebRpcEvents eventHolder;

        public string MachineName()
        {
            return Environment.MachineName;
        }

        public HostInfo Info { get; }
        public ISerializer Serializer { get; }

        public ServerConfig(ISerializer serializer, HostInfo info)
        {
            Serializer = serializer;
            Info = info;
            this.eventHolder = new WebRpcEvents(this);
        }

        public string ResponseType => "text/json";

        public WebRpcEvents Events => eventHolder;

        public string BaseUrl { get; set; } = "/";

        public IEnumerable<RegistrationInfo> AllRegistered()
        {
            return Services;
        }

        public void RegisterService<TServiceInterface>()
        {
            Services.Add(new RegistrationInfo(typeof(TServiceInterface)));
            Events.OnEndpointRegistered(new RegistrationInfoEventArgs { Interface = typeof(TServiceInterface) });
        }
    }
}