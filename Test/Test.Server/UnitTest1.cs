using NUnit.Framework;
using System;
using System.IO;

namespace Test.Server
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void MethodSiganturesLooksOk()
        {
            var serverInfo = new Blitz.Rpc.HttpServer.ServerInfo(new MySerializer());
            serverInfo.AddService(typeof(IServiceInterface));

            Assert.IsNotNull(serverInfo.Services[0].MethodSignatures["Test.Server.IServiceInterface.Method1-Test.Server.Method1Param".ToLower()]);
            Assert.IsNotNull(serverInfo.Services[0].MethodSignatures["Test.Server.IServiceInterface.Method2-Test.Server.Method2Param1-Test.Server.Method2Param2".ToLower()]);
            Assert.IsNotNull(serverInfo.Services[0].MethodSignatures["Test.Server.IServiceInterface.Method3-System.String-System.Int32".ToLower()]);
            Assert.IsNotNull(serverInfo.Services[0].MethodSignatures["Test.Server.IServiceInterface.Method4-".ToLower()]);

        }


        [Test]
        public void MethodIsFound()
        {
            var serverInfo = new Blitz.Rpc.HttpServer.ServerInfo(new MySerializer());

            serverInfo.AddService(typeof(IServiceInterface));
            var AppState = new Blitz.Rpc.HttpServer.Internals.ApplicationState(serverInfo);

            var handler = AppState.GetHandler("Test.Server.IServiceInterface.Method1-Test.Server.Method1Param");

            Assert.IsNotNull(handler);


            //var mStream = new MemoryStream();
            //StreamWriter streamWriter = new StreamWriter(mStream);
            //streamWriter.WriteLine(@"{""Value"": 1}");
            //streamWriter.Flush();


            //mStream.Position = 0;
            //var param =(Method1Param) handler.CreateParam(mStream);



            //Assert.IsNotNull(param);
            //Assert.AreEqual(1,param.Value);


        }
        [Test]
        public void MethodArgumentTypeIsSetForSingleParamenter()
        {
            var serverInfo = new Blitz.Rpc.HttpServer.ServerInfo(new MySerializer());

            serverInfo.AddService(typeof(IServiceInterface));
            var AppState = new Blitz.Rpc.HttpServer.Internals.ApplicationState(serverInfo);

            var handler = AppState.GetHandler("Test.Server.IServiceInterface.Method1-Test.Server.Method1Param");

            Assert.IsNotNull(handler.ParamType);

        }
        [Test]
        public void MethodArgumentTypeIsSetForMultiParamenter()
        {
            var serverInfo = new Blitz.Rpc.HttpServer.ServerInfo(new MySerializer());

            serverInfo.AddService(typeof(IServiceInterface));
            var AppState = new Blitz.Rpc.HttpServer.Internals.ApplicationState(serverInfo);

            var handler = AppState.GetHandler("Test.Server.IServiceInterface.Method2-Test.Server.Method2Param1-Test.Server.Method2Param2");

            Assert.IsNotNull(handler.ParamType);

        }

        [Test]
        public void DeserializingOfSingleParamIsWorking()
        {
            var serverInfo = new Blitz.Rpc.HttpServer.ServerInfo(new MySerializer());

            serverInfo.AddService(typeof(IServiceInterface));
            var AppState = new Blitz.Rpc.HttpServer.Internals.ApplicationState(serverInfo);

            var handler = AppState.GetHandler("Test.Server.IServiceInterface.Method1-Test.Server.Method1Param");

            Assert.IsNotNull(handler);


            var mStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(mStream);
            streamWriter.WriteLine(@"{""Value"": 1}");
            streamWriter.Flush();


            mStream.Position = 0;
            var param = (Method1Param)handler.CreateParam(mStream);



            Assert.IsNotNull(param);
            Assert.AreEqual(1, param.Value);


        }
        [Test]
        public void DeserializingOfMultiParamIsWorking()
        {
            var serverInfo = new Blitz.Rpc.HttpServer.ServerInfo(new MySerializer());

            serverInfo.AddService(typeof(IServiceInterface));
            var AppState = new Blitz.Rpc.HttpServer.Internals.ApplicationState(serverInfo);

            var handler = AppState.GetHandler("Test.Server.IServiceInterface.Method2-Test.Server.Method2Param1-Test.Server.Method2Param2");

            Assert.IsNotNull(handler);


            var mStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(mStream);
            streamWriter.WriteLine(@"[{},{}]");
            streamWriter.Flush();


            mStream.Position = 0;
            var param = (object[])handler.CreateParam(mStream);
            Assert.IsNotNull(param);

            var p1 = param[0];
            var p2 = param[1];

            Assert.IsNotNull(p1);
            Assert.IsNotNull(p2);

        }

        [Test]
        public void CallingServiceWwith1ParamWorks()
        {
            var serverInfo = new Blitz.Rpc.HttpServer.ServerInfo(new MySerializer());

            serverInfo.AddService(typeof(IServiceInterface));
            var AppState = new Blitz.Rpc.HttpServer.Internals.ApplicationState(serverInfo);

            var handler = AppState.GetHandler("Test.Server.IServiceInterface.Method1-Test.Server.Method1Param");

            var mStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(mStream);
            streamWriter.WriteLine(@"{""Value"": 1}");
            streamWriter.Flush();
            mStream.Position = 0;

            var param = (Method1Param)handler.CreateParam(mStream);
            var result = (ResultObject)handler.Execute(param, new ServiceProviderMock<ServiceImplementation>());

            Assert.AreEqual(1, result.Result);
        }

        [Test]
        public void CallingServiceWwithMultiParamWorks()
        {
            var serverInfo = new Blitz.Rpc.HttpServer.ServerInfo(new MySerializer());

            serverInfo.AddService(typeof(IServiceInterface));
            var AppState = new Blitz.Rpc.HttpServer.Internals.ApplicationState(serverInfo);

            var handler = AppState.GetHandler("Test.Server.IServiceInterface.Method2-Test.Server.Method2Param1-Test.Server.Method2Param2");

            Assert.IsNotNull(handler);


            var mStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(mStream);
            streamWriter.WriteLine(@"[{},{}]");
            streamWriter.Flush();


            mStream.Position = 0;
            var param = (object[])handler.CreateParam(mStream);
                       
            var result = (ResultObject)handler.Execute(param, new ServiceProviderMock<ServiceImplementation>());

            Assert.AreEqual(1, result.Result);
        }

    }


    public class ServiceProviderMock<T> : IServiceProvider where T: new()
    {
        
        public object GetService(Type serviceType)
        {
            return new T();
        }
    }

    public class ServiceImplementation : IServiceInterface
    {
        public ResultObject Method1(Method1Param param)
        {
            return new ResultObject() { Result = param.Value };
        }

        public ResultObject Method2(Method2Param1 param1, Method2Param2 param2)
        {
            return new ResultObject { Result = 1 };
        }

        public ResultObject Method3(string param1, int param2)
        {
            throw new System.NotImplementedException();
        }

        public ResultObject Method4()
        {
            throw new System.NotImplementedException();
        }
    }



    public interface IServiceInterface
    {
        ResultObject Method1(Method1Param param);
        ResultObject Method2(Method2Param1 param1, Method2Param2 param2);

        ResultObject Method3(string param1, int param2);
        ResultObject Method4();
    }

    public class Method2Param1
    {
    }

    public class Method2Param2
    {
    }

    public class ResultObject
    {
        public int Result = 1;
    }

    public class Method1Param
    {
        public int Value;
    }
}