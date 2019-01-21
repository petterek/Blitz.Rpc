using Blitz.Rpc.Client.Helper.UrlProvider;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Blitz.Rpc.Client.Helper.Extensions
{
    public static class ServiceCollectionExtension
    {
        private static int ConfigCounter = 1;

        private static int GetUniqueNumber()
        {
            return ConfigCounter++;
        }

        private static ApiProxyFactory ProxyFactory = new ApiProxyFactory();

        public static void ConfigureHttpApiClient(this IServiceCollection container, Action<ClientConfig> config)
        {
            ConfigureClientWorker(new ServiceCollectionWrapperImplementation( container), config);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="container"></param>
        /// <param name="config"></param>
        public static void ConfigureClientWorker(IServiceCollectionWrapper container, Action<ClientConfig> config)
        {
            string configName = $"Clientconfig_{GetUniqueNumber()}";
            var conf = new ClientConfig();

            config(conf);

            var (ConfigType, holder) = CreateConfigWithMarkerInterface<IntegratedHttpApiClientConfig>(configName);

            holder.LastHandler = conf.LastHandler;
            holder.urlProvider = conf.UrlProvider ?? new DefaultUrlProvider(conf.TypeReg, conf.AssemblyReg);

            container.AddSingleton(ConfigType, holder); //The config is now registered as a unique type in the container.

            conf.HttpHandlers.ForEach((h) => { if (h.register) container.AddTransient(h.type); }); //All HTTPHandlers is registerd.. (except those that should not :)

            var ProxyApiClientType = Utils.DynamicInherit<IntegratedHttpJsonApiClient>($"Client_{configName}", new Dictionary<Type, Type> { { typeof(IntegratedHttpApiClientConfig), ConfigType } }, typeof(ServiceCollectionExtension).GetMethod(nameof(FillTypes)), conf.HttpHandlers.Select(e => e.Item2).ToList());

            container.AddScoped(ProxyApiClientType);

            foreach (var registerdInterface in conf.Clients)
            {
                container.AddTransient(registerdInterface, ProxyFactory.CreateProxyTypeFor(registerdInterface, ProxyApiClientType)); //Create the type with the correct dependencies...
            }
        }

        internal static (Type, T) CreateConfigWithMarkerInterface<T>(string name)
        {
            var type = Utils.DynamicInherit<T>(name, null, null);
            return (type, (T)Activator.CreateInstance(type));
        }

        public static void FillTypes(IntegratedHttpJsonApiClient pipelineBase, List<Object> extraValues)
        {
            foreach (var o in extraValues)
            {
                pipelineBase.HttpHandlers.Add((DelegatingHandler)o);
            }
        }
    }
}