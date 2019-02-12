using System;
using System.Collections.Generic;
using System.Linq;
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
                        if (!createHandler(key)) return null;
                    }
                }
            }

            return handlerCache[key];
        }


        public string GetServiceName(string identifier)
        {
            return Regex.Match(identifier.Split('-')[0], @"(.*)\.+").Groups[1].Value;
        }



        private bool createHandler(string key)
        {
            try
            {
                string serviceName = GetServiceName(key);
                var regInfo = Container.Services.FirstOrDefault(s => s.ServiceName.ToLower() == serviceName.ToLower());
                var method = regInfo.MethodSignatures[key];
                var handlerInfo = new HandlerInfo(Container.Serializer, method.Service, method.Method);
                handlerCache[key] = handlerInfo;
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}