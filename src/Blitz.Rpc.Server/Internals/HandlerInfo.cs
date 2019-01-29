using Blitz.Rpc.Shared;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Blitz.Rpc.HttpServer.Internals
{
    class HandlerInfo
    {
        private MethodInfo CreateTypedParam;
        private MethodInfo CreateArrayOfTypedParam;
        readonly ISerializer serializer;
        private int paramCount = 0;

        public HandlerInfo(ISerializer serializer, Type service, MethodInfo method)
        {
            this.serializer = serializer;
            
            parameterInfo = method.GetParameters();
            paramCount = parameterInfo.Count();
            HandlerType = service;
            Method = method;

            switch (paramCount)
            {
                case 0:

                    break;
                case 1:
                    CreateTypedParam = typeof(ISerializer).GetMethod("FromStream", new Type[] { typeof(Stream), typeof(Type) });
                    ParamType = method.GetParameters()[0].ParameterType;
                    break;

                default:
                    CreateArrayOfTypedParam = typeof(ISerializer).GetMethod("FromStream", new Type[] { typeof(Stream), typeof(Type[]) });
                    ParamType = typeof(Type[]);
                    MultiParamTypes = parameterInfo.Select(el => el.ParameterType).ToArray();
                    break;
            }
        }

        ParameterInfo[] parameterInfo;
        public Type HandlerType;
        public Type ParamType;
        public MethodInfo Method;
        public Type[] MultiParamTypes;

        public object Execute(object param, IServiceProvider serviceProvider)
        {
            var handler = serviceProvider.GetRequiredService(HandlerType);
            if (handler == null) throw new ArgumentNullException(HandlerType.FullName);

            switch(paramCount)
            {
                case 0:
                    return Method.Invoke(handler, new object[0]);
                case 1:
                    return Method.Invoke(handler, new object[] { param });
                default:
                    return Method.Invoke(handler, (object[])param);
            }
        }

        internal object CreateParam(Stream param)
        {
            switch (paramCount)
            {
                case 0:
                    return null;
                case 1:
                    return CreateTypedParam.Invoke(serializer, new object[] { param, ParamType });
                default:
                    return CreateArrayOfTypedParam.Invoke(serializer, new object[] { param, MultiParamTypes });
            }
        }
    }
}