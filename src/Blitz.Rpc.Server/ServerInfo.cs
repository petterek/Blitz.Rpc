﻿using Blitz.Rpc.HttpServer.Internals;
using Blitz.Rpc.Shared;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Test.Server")]

namespace Blitz.Rpc.HttpServer
{



    public class ServerInfo {

        public ServerInfo(ISerializer serializer)
        {
            Serializer = serializer;
        }

        public string MachineName { get; } = Environment.MachineName;
        public string DomainName { get; set; }
        public string DomainDescription { get; set; }



        public string BasePath { get; set; }
        public ISerializer Serializer { get; set; } 
        public List<RegistrationInfo> Services { get; } = new List< RegistrationInfo>();

        public List<Type> PreMiddleware { get; } = new List<Type>();

        public void AddPreMiddleware<TMiddelwareType>()
        {
            PreMiddleware.Add(typeof(TMiddelwareType));
        }

        public void AddService<TServiceType>()
        {
            AddService(typeof(TServiceType));
        }
        public void AddService(Type service)
        {
            Services.Add(new RegistrationInfo(service));
        }
    }
}