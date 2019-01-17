using Microsoft.Extensions.DependencyInjection;
using Blitz.Rpc.Client.Helper.Extensions;
using System;
using Blitz.Rpc.Client.Helper;

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


                Console.WriteLine(res.Completed);
            }


            Console.ReadKey();

        }
    }
}
