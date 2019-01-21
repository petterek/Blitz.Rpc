using Blitz.Rpc.Client.Helper.HttpHandlers;
using Blitz.Rpc.Client.Helper.UrlProvider;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;

namespace Blitz.Rpc.Client.Helper
{
    public class ClientConfig
    {
        internal readonly List<(bool register, Type type)> HttpHandlers = new List<(bool, Type)>();
        internal readonly List<Type> Clients = new List<Type>();
        
        internal Dictionary<Assembly, List<string>> AssemblyReg = new Dictionary<Assembly, List<string>>();
        internal Dictionary<Type, List<string>> TypeReg = new Dictionary<Type, List<string>>();
        internal HttpMessageHandler LastHandler = new AvoidDisposeMessageHandler(new HttpClientHandler());

        public IUrlProvider UrlProvider { get; set; }

        public void AddClientFor<T>()
        {
            Clients.Add(typeof(T));
        }

        public void AddClientFor<T>(string useUrl)
        {
            Clients.Add(typeof(T));
            AddUrlForType<T>(useUrl);
        }

        

        public void AddMessageHandler<THandler>(bool register = true) where THandler : DelegatingHandler
        {
            HttpHandlers.Add((register, typeof(THandler)));
        }

        public void AddUrlForAssembly<TTypeInAssembly>(string url)
        {
            if (!AssemblyReg.ContainsKey(typeof(TTypeInAssembly).Assembly))
            {
                AssemblyReg[typeof(TTypeInAssembly).Assembly] = new List<string>();
            }
            AssemblyReg[typeof(TTypeInAssembly).Assembly].Add(url);
        }

        public void AddUrlForType<TType>(string url)
        {
            if (!TypeReg.ContainsKey(typeof(TType)))
            {
                TypeReg[typeof(TType)] = new List<string>();
            }
            TypeReg[typeof(TType)].Add(url);
        }

        public void SetFinaleMessageHandler(HttpMessageHandler finale)
        {
            LastHandler = new AvoidDisposeMessageHandler(finale);
        }
    }
}