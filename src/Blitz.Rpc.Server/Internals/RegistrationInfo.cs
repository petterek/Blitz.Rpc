using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Blitz.Rpc.Server.Internals
{
    public class RegistrationInfo
    {
        public RegistrationInfo(Type serviceInterface)
        {
            Interface = serviceInterface;
            HashCodes = new Dictionary<string, MethodInfo>();
            foreach (var mi in serviceInterface.GetMethods())
            {
                var paramTypeString = "";
                var param = mi.GetParameters();
                if (param.Length == 1) paramTypeString = param[0].ParameterType.FullName;

                var sigString = $"{mi.DeclaringType.FullName}.{mi.Name}-{paramTypeString}";
                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    var theHash = System.Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(sigString)));
                    HashCodes[theHash] = mi;
                }
            }
        }

        public readonly Type Interface;
        public Dictionary<string, MethodInfo> HashCodes;
    }
}