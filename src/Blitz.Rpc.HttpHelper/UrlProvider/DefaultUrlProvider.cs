using Blitz.Rpc.Client.BaseClasses;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Blitz.Rpc.Client.Helper.UrlProvider
{
    /// <summary>
    /// Rudimentary implementation of URL provider..
    /// </summary>
    public class DefaultUrlProvider : IUrlProvider
    {
        internal Dictionary<Type, List<string>> TypeReg = new Dictionary<Type, List<string>>();
        internal Dictionary<Assembly, List<string>> AssemblyReg = new Dictionary<Assembly, List<string>>();

        public DefaultUrlProvider(Dictionary<Type, List<string>> typeReg, Dictionary<Assembly, List<string>> assemblyReg)
        {
            TypeReg = typeReg;
            AssemblyReg = assemblyReg;
        }

        public IUrlProvider Next { get; set; }

        public string GetEndpoint(RpcMethodInfo invokeInfo)
        {
            if (TypeReg.ContainsKey(invokeInfo.MasterType))
            {
                return TypeReg[invokeInfo.MasterType][0];
            }
            if (AssemblyReg.ContainsKey(invokeInfo.MasterType.Assembly))
            {
                return AssemblyReg[invokeInfo.MasterType.Assembly][0];
            }

            if(Next == null)
            {
                throw new UrlNotConfiguredException(invokeInfo);
            }
            else
            {
                return Next.GetEndpoint(invokeInfo);
            }
            
        }
    }
}