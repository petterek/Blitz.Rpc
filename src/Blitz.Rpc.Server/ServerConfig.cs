

using Blitz.Rpc.Shared;
using System;
using System.Collections.Generic;

namespace Blitz.Rpc.HttpServer
{
    public class ServerConfig
    {
        internal Dictionary<Type, Type> ServiceList { get; } = new Dictionary<Type, Type>();

        public ISerializer Serializer { get; }  
        
        public string BasePath { get; set; } = "/";

        public void RegisterService<TService>() where TService:class
        {
            ServiceList.Add(typeof(TService),null);
        }

        public void RegisterService<TService,TImplementation>() where TService:class where TImplementation: class, TService  
        {
            ServiceList.Add(typeof(TService), typeof(TImplementation));
        }
                
    }
}