
using Blitz.Rpc.HttpServer.Adapters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Blitz.Rpc.HttpServer.Extensions
{
    public static class ServiceCollection
    {


        public static IServiceCollection AddWebRpcServices(this IServiceCollection services, Action<ServerConfig> config)
        {
            var configHolder = new ServerInfoHolder();
            services.TryAddSingleton(configHolder);
            
            var configuration = new ServerConfig();
            config(configuration);



            RegisterServicesInContainer(services, configuration);

            //Create the container from the configuration.. 
            configHolder.Add( CreateServerInfo(configuration));


            return services;
        }

        private static ServerInfo CreateServerInfo(ServerConfig container)
        {
            var ret = new ServerInfo();

            foreach(var kv in container.ServiceList)
            {
                ret.AddService(kv.Key);
            }
            
            ret.BasePath = container.BasePath;
            
            return ret;
        }

        private static void RegisterServicesInContainer(IServiceCollection services, ServerConfig config)
        {
            foreach (var val in config.ServiceList)
            {
                if (val.Value != null) //Register all services that has been provided with an implementation in the container, the service can be registered manually if you want that. 
                {
                    services.AddTransient(val.Key, val.Value);
                }
            }
        }
                
    }
}