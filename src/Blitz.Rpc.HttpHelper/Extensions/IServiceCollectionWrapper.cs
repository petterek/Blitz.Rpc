
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Blitz.Rpc.Client.Helper.Extensions
{
    public interface IServiceCollectionWrapper
    {
        void AddSingleton(Type type, object value);
        void AddTransient(Type sig, Type impl);
        void AddTransient(Type type);
        void AddScoped(Type type);
    }


    internal class ServiceCollectionWrapperImplementation : IServiceCollectionWrapper
    {
        readonly IServiceCollection serviceCollection;

        public ServiceCollectionWrapperImplementation(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
        }
        public void AddScoped(Type type)
        {
            serviceCollection.AddScoped(type);
        }

        public void AddSingleton(Type type, object value)
        {
            serviceCollection.AddSingleton(type ,value);
        }

        public void AddTransient(Type sig, Type impl)
        {
            serviceCollection.AddTransient(sig, impl);
        }

        public void AddTransient(Type type)
        {
            serviceCollection.AddTransient(type);
        }
    }
}