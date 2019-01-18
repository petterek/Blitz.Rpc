using Blitz.Rpc.Client.Helper.Extensions;
using Blitz.Rpc.Shared;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Tests.Contract;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShowSetup()
        {
            IServiceCollection services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

            services.AddSingleton<ISerializer>(new Serializer()); //This need to be implemented..  To avoid dependencies to other packages.. :)

            services.ConfigureHttpApiClient(conf =>
            {
                conf.AddClientFor<IRemoteServiceContract>("http://your.url.com");
            });

            var sp = services.BuildServiceProvider();
            IRemoteServiceContract instance = null;
            Assert.DoesNotThrow(() => instance = sp.GetService<IRemoteServiceContract>());
            Assert.IsNotNull(instance);
        }

        public class Serializer : ISerializer
        {
            public string ProduceMimeType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public List<string> AcceptMimeType => throw new NotImplementedException();

            public object FromStream(Stream stream, Type returnType)
            {
                throw new NotImplementedException();
            }

            public void ToStream(Stream outstream, object v)
            {
                throw new NotImplementedException();
            }
        }
    }

    namespace Contract
    {
        public interface IRemoteServiceContract
        {
            ResultData ServiceMethod(ServiceMethodParam param);
        }

        public class ServiceMethodParam
        {
        }

        public class ResultData
        {
        }
    }
}