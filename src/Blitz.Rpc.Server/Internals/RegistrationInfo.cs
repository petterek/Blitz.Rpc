using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blitz.Rpc.HttpServer.Internals
{
    public class RegistrationInfo
    {

        public class MethodMap
        {
            public string Signature { get; }
            public MethodInfo Method { get; }
            public Type Service { get; }

            public MethodMap(string signature,Type service, MethodInfo method)
            {
                Service = service;
                Method = method;
                Signature = signature;
            }
        }

        public RegistrationInfo(Type serviceInterface)
        {
            Interface = serviceInterface;

            foreach (var mi in serviceInterface.GetMethods())
            {
                var paramTypeString = "";
                var param = mi.GetParameters();
                paramTypeString = string.Join("-", param.Select(p => p.ParameterType.FullName).ToArray());
                string item = $"{mi.DeclaringType.FullName}.{mi.Name}-{paramTypeString}".ToLower();
                MethodSignatures.Add(item, new MethodMap(item, serviceInterface, mi));

            }
        }
        public string ServiceName { get => Interface.FullName; }
        public readonly Type Interface;
        public Dictionary<string, MethodMap> MethodSignatures { get; } = new Dictionary<string, MethodMap>();
        
    }
}