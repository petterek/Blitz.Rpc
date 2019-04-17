using Blitz.Rpc.Shared;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Blitz.Rpc.HttpServer.Internals
{
    class HandlerInfo
    {
        private MethodInfo CreateTypedParam;
        private MethodInfo CreateArrayOfTypedParam;
        private PropertyInfo GetValuefromTaskProp;
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

            if (typeof(Task).IsAssignableFrom(method.ReturnType))
            {
                if(method.ReturnType.IsGenericType)
                {
                    GetValuefromTaskProp = method.ReturnType.GetProperty("Result");
                }
            }
        }

        ParameterInfo[] parameterInfo;
        public Type HandlerType;
        public Type ParamType;
        public MethodInfo Method;
        public Type[] MultiParamTypes;

        public async Task<object> Execute(object param, IServiceProvider serviceProvider)
        {
            var handler = serviceProvider.GetRequiredService(HandlerType);
            if (handler == null) throw new ArgumentNullException(HandlerType.FullName);

            object result;

            switch (paramCount)
            {
                case 0:
                    result = Method.Invoke(handler, new object[0]);
                    break;
                case 1:
                    result = Method.Invoke(handler, new object[] { param });
                    break;
                default:
                    result = Method.Invoke(handler, (object[])param);
                    break;
            }

            if (typeof(Task).IsAssignableFrom(result.GetType()))
            {
                await (Task)result;
                return GetvalueFromTask((Task)result);
            }

            return result;

        }

        internal object GetvalueFromTask(Task theTask)
        {
            if (GetValuefromTaskProp != null)
            {
                return GetValuefromTaskProp.GetValue(theTask);
            }
            return null; //Task is a void.. 
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