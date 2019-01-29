using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Blitz.Rpc.HttpServer.Internals
{
    class ApplicationState
    {
        public ApplicationState(ServerInfo container)
        {
            this.Container = container;
        }

        public readonly ServerInfo Container;

        private Dictionary<String, HandlerInfo> handlerCache = new Dictionary<string, HandlerInfo>();



        
        internal void ValidateRequest(HandlerInfo hInfo)
        {
        }

        public HandlerInfo GetHandler(string identifier)
        {
            var key = identifier.ToLower();
            if (!handlerCache.ContainsKey(key))
            {
                lock (handlerCache)
                {
                    if (!handlerCache.ContainsKey(key))
                    {
                        createHandler(key);
                    }
                }
            }

            return handlerCache[key];
        }


        public string GetServiceName(string identifier)
        {
            return Regex.Match(identifier.Split('-')[0], @"(.*)\.+").Groups[1].Value;
        }



        private void createHandler(string key)
        {
            var regInfo = Container.Services[GetServiceName(key)];
            var method = regInfo.MethodSignatures[key];
            var handlerInfo = new HandlerInfo(Container.Serializer, method.Service, method.Method);
            handlerCache[key] = handlerInfo;
        }
    }
}