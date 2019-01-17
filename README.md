# Blitz.Rpc

Small utility framework for  out of process call, in .Net using interfaces as contracts. 

## Setting up the client, look at the Demo/UsingClient/

```csharp

    
var serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton<ISerializer>(new MySerializer());

//more configuration options is avialable
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

```


## Setting up the server

```csharp

public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var appType = Guid.NewGuid();

            services.AddSingleton<ISerializer, MySerializer>();
            services.AddWebRpcServices((c) =>
                {
                    c.RegisterService<IServiceOne, ServiceOne>();
                });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebRpc();
        }
    }
```

The interface IServiceOne is declared in a seperate project, and the idea is to share this contract thorugh nuget. 
