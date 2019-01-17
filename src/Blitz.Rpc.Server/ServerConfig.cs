using Blitz.Rpc.HttpServer.Adapters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Blitz.Rpc.HttpServer
{
    public class ServerConfig
    {
        internal Dictionary<Type, Type> ServiceList { get; } = new Dictionary<Type, Type>();

        public List<ISerializer> Serializer { get; } = new List<ISerializer>(); 
        
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