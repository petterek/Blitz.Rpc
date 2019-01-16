using Blitz.Rpc.Client.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Blitz.Rpc.Client
{
    public class ApiProxyFactory
    {
        private Type baseClass;
        private ConstructorInfo baseConstructor;
        private MethodInfo baseExecAsync;
        private MethodInfo baseExecSync;

        private PropertyInfo getValueFromTask;
        private MethodInfo configureAwait;
        private MethodInfo getMethodMethod = typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle) });
        private MethodInfo createRpcMethodInfo = typeof(ApiProxyFactory).GetMethod(nameof(CreateRpcMethodInfo), BindingFlags.Static | BindingFlags.Public);

        public static RpcMethodInfo CreateRpcMethodInfo(MethodInfo methodInfo, Type masterType)
        {
            return methodInfo.ToRpcMethodInfo(masterType);
        }

        public ApiProxyFactory()
        {
            baseClass = typeof(ProxyBase);
            baseConstructor = baseClass.GetConstructor(new Type[] { typeof(IApiClient) });
            getValueFromTask = typeof(Task<object>).GetProperty("Result", BindingFlags.Public | BindingFlags.Instance);
            configureAwait = typeof(Task<object>).GetMethod("ConfigureAwait");
            baseExecAsync = baseClass.GetMethod("Execute", BindingFlags.Instance | BindingFlags.NonPublic);
            baseExecSync = baseClass.GetMethod("ExecuteSync", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public Type CreateProxyTypeFor<TInterface>()
        {
            return CreateProxyTypeFor(typeof(TInterface), typeof(IApiClient));
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns></returns>
        public Type CreateProxyTypeFor(Type proxyFor, Type constructorParamType)
        {
            if (!typeof(IApiClient).IsAssignableFrom(constructorParamType)) throw new Exception("wrong type of constructor parameter. Must inherit IAPIClient");

            var aBuilder = GetAssemblyBuilder(proxyFor);

            var moduleBuilder = aBuilder.DefineDynamicModule("Proxy");
            var typeBuilder = moduleBuilder.DefineType($"{proxyFor.Name}_{constructorParamType.Name}_Proxy", TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed, baseClass);

            //Create the constructor of the class..
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { constructorParamType }).GetILGenerator();
            constructorBuilder.Emit(OpCodes.Ldarg_0);
            constructorBuilder.Emit(OpCodes.Ldarg_1);
            constructorBuilder.Emit(OpCodes.Call, baseConstructor);
            constructorBuilder.Emit(OpCodes.Ret);

            //Implement the interface..

            void AddSubInterface(Type parent, List<Type> list)
            {
                list.Add(parent);
                foreach (var t in parent.GetInterfaces())
                {
                    AddSubInterface(t, list);
                }
            }

            var toImplementList = new List<Type>();
            AddSubInterface(proxyFor, toImplementList);

            foreach (var toImpl in toImplementList)
            {
                typeBuilder.AddInterfaceImplementation(toImpl);

                foreach (MethodInfo mi in toImpl.GetMethods())
                {
                    ParameterInfo[] parameters = mi.GetParameters();

                    Type[] parameterTypes = parameters.Select(pi => pi.ParameterType).ToArray();
                    bool hasParams = parameters.Count() > 0;

                    Type returnType = mi.ReturnType == typeof(void) ? typeof(object) : mi.ReturnType;

                    var mBuilder = typeBuilder.DefineMethod(mi.Name,
                        MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual,
                        CallingConventions.Standard,
                        mi.ReturnType,
                        parameterTypes);

                    var ilGen = mBuilder.GetILGenerator();

                    Type paramType = hasParams ? parameterTypes[0] : typeof(object);
                    bool returnsTask = typeof(Task).IsAssignableFrom(mi.ReturnType);

                    ilGen.Emit(OpCodes.Ldarg_0);
                    ilGen.Emit(OpCodes.Ldtoken, mi);

                    //Get the MethodInfo
                    ilGen.Emit(OpCodes.Call, getMethodMethod);

                    //transform MethodInfo into RpcMethodInfo
                    ilGen.Emit(OpCodes.Ldtoken, proxyFor);
                    ilGen.Emit(OpCodes.Call, createRpcMethodInfo);

                    if (hasParams)
                    {
                        // object[] parameterList = new object[parameters.Length];
                        var parameterListLocal = ilGen.DeclareLocal(typeof(object[])); //declare local var
                        ilGen.Emit(OpCodes.Ldc_I4, parameters.Length); //set length
                        ilGen.Emit(OpCodes.Newarr, typeof(object)); //Create array
                        ilGen.Emit(OpCodes.Stloc, parameterListLocal);//Store to local

                        int i = 0;

                        foreach (var parameter in parameters)
                        {
                            // parameterList[i] = arguments[i+1];

                            ilGen.Emit(OpCodes.Ldloc, parameterListLocal);
                            ilGen.Emit(OpCodes.Ldc_I4, i);
                            ilGen.Emit(OpCodes.Ldarg, i + 1);
                            if (parameter.ParameterType.IsValueType)
                            {
                                ilGen.Emit(OpCodes.Box, parameter.ParameterType);
                            }
                            ilGen.Emit(OpCodes.Stelem, typeof(object));
                            i++;
                        }

                        ilGen.Emit(OpCodes.Ldloc, parameterListLocal);
                    }
                    else
                    {
                        ilGen.Emit(OpCodes.Ldnull);
                    }

                    if (returnsTask)
                    {
                        ilGen.Emit(OpCodes.Call, baseExecAsync);
                    }
                    else
                    {
                        ilGen.Emit(OpCodes.Call, baseExecSync);
                        if (mi.ReturnType == typeof(void))
                        {
                            ilGen.Emit(OpCodes.Pop);
                        }
                    }
                    ilGen.Emit(OpCodes.Ret);
                }
            }
            return typeBuilder.CreateTypeInfo();
        }

        private AssemblyBuilder GetAssemblyBuilder(Type proxyFor)
        {
            AssemblyName aname = new AssemblyName($"{proxyFor.Name}_Proxy");
            AppDomain currentDomain = AppDomain.CurrentDomain; // Thread.GetDomain();
            AssemblyBuilder builder = AssemblyBuilder.DefineDynamicAssembly(aname, AssemblyBuilderAccess.Run);
            return builder;
        }
    }
}