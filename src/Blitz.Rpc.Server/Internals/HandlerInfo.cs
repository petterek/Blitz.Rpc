using Blitz.Rpc.HttpServer.Exceptions;
using Blitz.Rpc.Shared;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Blitz.Rpc.HttpServer.Internals
{
    internal class HandlerInfo
    {
        private readonly ServerInfo container;

        private MethodInfo CreateTypedParam;

        public HandlerInfo(ServerInfo container, string handlerTypeName, string functionName, string paramTypeName)
        {
            this.container = container;
            string typeNameInvariant = handlerTypeName.ToLower();
            string funcNameInvariant = functionName.ToLower();

            HandlerType = container.Services.FirstOrDefault(t => t.Interface.FullName.ToLower() == typeNameInvariant)?.Interface;
            if (HandlerType == null) throw new UnableToGetHandlerException(handlerTypeName);

            foreach (var mi in HandlerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                if (mi.Name.ToLower() == funcNameInvariant)
                {
                    var funcParams = mi.GetParameters();
                    if (funcParams.Length == 1 && funcParams[0].ParameterType.FullName.ToLower() == paramTypeName.ToLower())
                    {
                        Method = mi;
                        ParamType = funcParams[0].ParameterType;
                        CreateTypedParam = typeof(ISerializer).GetMethod("FromStream", new Type[] { typeof(Stream), typeof(Type) });
                        break;
                    }
                    if (funcParams.Length == 0)
                    {
                        Method = mi;
                        ParamType = null;
                        break;
                    }
                }
            }

            if (Method == null) { throw new NotSupportedException(functionName); }
        }

        public Type HandlerType;
        public Type ParamType;
        public MethodInfo Method;

        public object Execute(object param, IServiceProvider serviceProvider)
        {
            var handler = serviceProvider.GetRequiredService(HandlerType);
            if (handler == null) throw new ArgumentNullException(HandlerType.FullName);
            if (param != null)
            {
                return Method.Invoke(handler, new object[] { param });
            }
            else
            {
                return Method.Invoke(handler, new object[0]);
            }
        }

        internal object CreateParam(Stream param, Type paramType)
        {
            //Here we must do something smarter.. To match mimetype -> serializer
            var ret = CreateTypedParam.Invoke(container.Serializer, new object[] { param,paramType  });
            return ret;
        }
    }
}