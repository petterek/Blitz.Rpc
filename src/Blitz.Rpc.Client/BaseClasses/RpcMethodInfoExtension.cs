using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Blitz.Rpc.Client.BaseClasses
{
    public static class RpcMethodInfoExtension
    {
        public static RpcMethodInfo ToRpcMethodInfo(this MethodInfo info, Type masterType)
        {
            Type returnType = info.ReturnType;

            if (typeof(Task).IsAssignableFrom(returnType))
            {
                if (returnType.IsGenericType)
                {
                    returnType = returnType.GetGenericArguments()[0];
                }else
                {
                    returnType = typeof(void);
                }
            }

            return new RpcMethodInfo
            {
                Name = info.Name,
                MasterType = masterType,
                DefinedIn = info.DeclaringType,
                ServiceId = info.DeclaringType.FullName,
                PackageName = info.DeclaringType.Assembly.GetName().Name,
                ReturnType = returnType,
                ParamType = info.GetParameters().Select(p => p.ParameterType).ToArray(),
                Major = info.DeclaringType.Assembly.GetName().Version.Major,
                Minor = info.DeclaringType.Assembly.GetName().Version.Minor
            };
        }
    }
}