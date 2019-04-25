using Microsoft.Extensions.DependencyInjection;
using Blitz.Rpc.Client.Helper.Extensions;
using System;
using Blitz.Rpc.Shared;
using System.Threading.Tasks;

namespace UsingClient
{
    class Program
    {
        private const string V = "Should be visisble";

        static async Task Main(string[] args)
        {

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<ISerializer>(new MySerializer());

            serviceCollection.ConfigureHttpApiClient(conf => {
                
                conf.AddClientFor<Contract.IServiceOne>("http://localhost:5000/");
                conf.AddClientFor<Contract.IAsyncService>("http://localhost:5000/");
            });

            using (var sp = serviceCollection.BuildServiceProvider())
            {

                var client = sp.GetService<Contract.IServiceOne>();

                var res = client.ServiceMethod1(new Contract.ServiceMethod1Param { NumberOfTasks = 25 });
                Console.WriteLine($"Number1 = {res.Completed == 24}" );


                Contract.IAsyncService asyncService = sp.GetRequiredService<Contract.IAsyncService>();

                var res2 = await asyncService.Method1(new Contract.ServiceMethod1Param { NumberOfTasks = 10 });

                Console.WriteLine($"Result2 = {res2.Completed == 10}");


                try
                {
                    var res3 = await asyncService.AsyncMethodThatThrowsKnownException(new Contract.ExceptionParam { ExceptionMessage = V });
                }

                catch(Contract.ExceptionInContractProjectExcption ex)
                {
                    Console.WriteLine($"Correct exception is recieved: {ex.GetType().FullName}");
                }

                catch(Exception ex)
                {
                    Console.WriteLine(V == ex.Message);
                    Console.WriteLine(ex.Message);
                }


                try
                {
                    var res4 = await asyncService.AsyncMethodThatThrowsUnknownException(new Contract.ExceptionParam { ExceptionMessage = V });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(V == ex.Message);
                    Console.WriteLine(ex.Message);
                }


            }


            Console.ReadKey();

        }
    }
}
