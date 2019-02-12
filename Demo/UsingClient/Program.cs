using Microsoft.Extensions.DependencyInjection;
using Blitz.Rpc.Client.Helper.Extensions;
using System;
using Blitz.Rpc.Shared;

namespace UsingClient
{
    class Program
    {
        static void Main(string[] args)
        {

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<ISerializer>(new MySerializer());

            serviceCollection.ConfigureHttpApiClient(conf => {
                
                conf.AddClientFor<Contract.IServiceOne>("http://localhost:5000/");
            });

            using (var sp = serviceCollection.BuildServiceProvider())
            {

                var client = sp.GetService<Contract.IServiceOne>();

                var res = client.ServiceMethod1(new Contract.ServiceMethod1Param { NumberOfTasks = 25 });
                //var res2 = client.ServiceMethod2(new Contract.ServiceMethod2Param1 { Value = 10 }, new Contract.ServiceMethod2Param2 { Value = 10 });
                //var res3 = client.ServiceMethod2(3, 4);

                Console.WriteLine($"Number1 = {res.Completed == 24}" );
                //Console.WriteLine($"Number2 = {res2.Completed == 20}");
                //Console.WriteLine($"Number3 = {res3.Completed == 12}");
            }


            Console.ReadKey();

        }
    }
}
