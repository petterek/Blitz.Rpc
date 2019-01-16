using System;
using System.Collections.Generic;

namespace Blitz.Rpc.Server.Internals
{
    internal class ApplicationState
    {
        public ApplicationState(ServerConfig container)
        {
            this.Container = container;
        }

        public readonly ServerConfig Container;

        private Dictionary<String, HandlerInfo> handlerCache = new Dictionary<string, HandlerInfo>();

        public class IdentifierParts
        {
            public string ClassName;
            public string FunctionName;
            public string ParamTypeName;
        }

        public IdentifierParts GetParts(string identifier)
        {
            var ret = new IdentifierParts();
            var p = identifier.Split('-');
            var className = p[0].Split('.');

            ret.FunctionName = className[className.Length - 1];
            ret.ClassName = p[0].Replace("." + ret.FunctionName, "");

            ret.ParamTypeName = p[1];
            return ret;
        }

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
                        var parts = GetParts(identifier);
                        var handlerInfo = new HandlerInfo(Container, parts.ClassName, parts.FunctionName, parts.ParamTypeName);
                        handlerCache[key] = handlerInfo;
                    }
                }
            }

            return handlerCache[key];
        }
    }
}