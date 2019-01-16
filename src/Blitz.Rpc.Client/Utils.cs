using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Blitz.Rpc.Client
{
    public static class Utils
    {
        private static int Counter = 0;

        /// <summary>
        /// Will create a proxy for the instance, warapping it in a unique class type..
        /// </summary>
        /// <typeparam name="TWrapper"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static TWrapper CreateWrapperForInstance<TWrapper>(object instance)
        {
            if (!typeof(TWrapper).IsAssignableFrom(instance.GetType())) throw new NotSupportedException("Instance must implement " + typeof(TWrapper).FullName);

            var typeBuilder = AssemblyBuilder.DefineDynamicAssembly(
                            new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run).
                            DefineDynamicModule("Wrapper").
                            DefineType(typeof(TWrapper).Name + "_Proxy_" + Counter++, TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed, null, new Type[] { typeof(TWrapper) });

            var nextField = typeBuilder.DefineField("_next", typeof(TWrapper), FieldAttributes.Private);

            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(TWrapper) }).GetILGenerator();
            constructor.Emit(OpCodes.Ldarg_0);
            constructor.Emit(OpCodes.Ldarg_1);
            constructor.Emit(OpCodes.Stfld, nextField);
            constructor.Emit(OpCodes.Ret);

            foreach (MethodInfo mi in typeof(TWrapper).GetMethods())
            {
                Type[] parameterTypes = mi.GetParameters().Select(pi => pi.ParameterType).ToArray();
                var Params = mi.GetParameters();

                var mBuilder = typeBuilder.DefineMethod(mi.Name,
                    MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual,
                    CallingConventions.Standard,
                    mi.ReturnType,
                    parameterTypes).GetILGenerator();

                int x = 1;
                mBuilder.Emit(OpCodes.Ldarg_0);
                mBuilder.Emit(OpCodes.Ldfld, nextField);

                foreach (var p in Params)
                {
                    mBuilder.Emit(OpCodes.Ldarg, x);
                    x++;
                }
                mBuilder.Emit(OpCodes.Call, mi);
                mBuilder.Emit(OpCodes.Ret);
            }

            return (TWrapper)Activator.CreateInstance(typeBuilder.CreateTypeInfo(), instance);
        }

        public static Type DynamicInherit<TToMark>(string name, Dictionary<Type, Type> replaceTypes, MethodInfo constructor, List<Type> extraConstructorParams = null)
        {
            if (replaceTypes == null) replaceTypes = new Dictionary<Type, Type>();

            var constructors = typeof(TToMark).GetConstructors();

            if (constructors.Length > 1) throw new ArgumentOutOfRangeException(nameof(TToMark), "Only 1 constructor on type allowed");

            ConstructorInfo constructorInfo = constructors[0];
            var baseConstructorParams = constructorInfo.GetParameters();

            var typeBuilder = AssemblyBuilder.DefineDynamicAssembly(
                            new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run).
                            DefineDynamicModule($"MarkerFor{typeof(TToMark).Name}").
                            DefineType($"{typeof(TToMark).Name}_{name}", TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed, typeof(TToMark));

            var allParams = new List<Type>();
            foreach (var p in baseConstructorParams)
            {
                if (replaceTypes.ContainsKey(p.ParameterType))
                {
                    if (!p.ParameterType.IsAssignableFrom(replaceTypes[p.ParameterType])) throw new TypeAccessException();

                    allParams.Add(replaceTypes[p.ParameterType]);
                }
                else
                {
                    allParams.Add(p.ParameterType);
                }
            }
            if (extraConstructorParams != null)
            {
                allParams.AddRange(extraConstructorParams);
            }

            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, allParams.ToArray()).GetILGenerator();

            constructorBuilder.Emit(OpCodes.Ldarg_0);

            int x = 1;
            foreach (var p in baseConstructorParams)
            {
                constructorBuilder.Emit(OpCodes.Ldarg, x);
                x++;
            }
            constructorBuilder.Emit(OpCodes.Call, constructorInfo);

            constructorBuilder.DeclareLocal(typeof(List<object>));

            var addToList = typeof(List<Type>).GetMethod("Add");

            if (extraConstructorParams != null)
            {
                constructorBuilder.Emit(OpCodes.Newobj, typeof(List<object>).GetConstructor(new Type[] { }));
                constructorBuilder.Emit(OpCodes.Stloc_0);

                foreach (var p in extraConstructorParams) //Load all the extra params into the List.
                {
                    constructorBuilder.Emit(OpCodes.Ldloc_0);
                    constructorBuilder.Emit(OpCodes.Ldarg, x);
                    constructorBuilder.Emit(OpCodes.Call, addToList);
                    x++;
                }

                //var mi = constructor.GetType().GetMethod("Invoke");
                var mi = constructor;

                constructorBuilder.Emit(OpCodes.Ldarg_0);
                constructorBuilder.Emit(OpCodes.Ldloc_0);
                constructorBuilder.Emit(OpCodes.Call, mi);
            }

            constructorBuilder.Emit(OpCodes.Ret);
            return typeBuilder.CreateTypeInfo();
        }

        public static Type DynamicConstructor<TInherit>(string name, List<Type> mappings)
        {
            var constructors = typeof(TInherit).GetConstructors();
            ConstructorInfo theConstructor = null;

            foreach (var con in constructors)
            {
                if (con.GetParameters().Count() == mappings.Count)
                {
                    theConstructor = con;
                    break;
                }
            }

            if (theConstructor == null) throw new AmbiguousMatchException("Could not find matching constructor");

            var baseConstructorParams = theConstructor.GetParameters();

            var typeBuilder = AssemblyBuilder.DefineDynamicAssembly(
                            new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run).
                            DefineDynamicModule($"MarkerFor{typeof(TInherit).Name}").
                            DefineType($"{typeof(TInherit).Name}_DynamicExtend_For_{name}", TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed, typeof(TInherit));

            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, mappings.ToArray()).GetILGenerator();

            constructorBuilder.Emit(OpCodes.Ldarg_0);

            int x = 1;
            foreach (var p in baseConstructorParams)
            {
                if (!p.ParameterType.IsAssignableFrom(mappings[x - 1]))
                {
                    throw new AmbiguousMatchException($"{p.ParameterType.FullName} does not match with {mappings[x - 1].FullName}");
                }
                constructorBuilder.Emit(OpCodes.Ldarg, x);
                x++;
            }
            constructorBuilder.Emit(OpCodes.Call, theConstructor);
            constructorBuilder.Emit(OpCodes.Ret);

            return typeBuilder.CreateTypeInfo();
        }
    }
}