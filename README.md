# Blitz.Rpc

Small utility framework for  out of process call, in .Net using interfaces as contracts. 
Built to be used with a DI container. The Helper project is focused on extending for IServiceColletion from MS. 


All comments and feedback is welcome, I want to keep the framework small, and it is build with extension in mind. 



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
The serializer in both projects are declared as Interfaces to avoid dependencies against other nuget packages. 

### Implementation of serializer using Newtosoft Json could look like this. 
```csharp
using Blitz.Rpc.Client.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

public class MySerializer : ISerializer
{

    JsonSerializer serializer;
    public MySerializer()
    {
        serializer = new JsonSerializer();
    }
    public List<string> AcceptMimeType { get; } = new List<string> { "application/json", "text/json" };

    public string ProduceMimeType { get; set; } = "application/json";

    public object FromStream(Stream stream, Type returnType)
    {
        return serializer.Deserialize(new StreamReader(stream), returnType);
    }
    
    public object[] FromStream(Stream stream, Type[] returnType) //multiparam.. 
    {
           var data = JToken.Load(new JsonTextReader(new StreamReader(stream)));
            if (!(data is JArray))
            {
                throw new Exception();
            }
            var list = new List<object>();
            int i = 0;
            foreach (var item in data as JArray)
            {
                list.Add(item.ToObject(returnType[i++]));
            }
            return list.ToArray();
    }
    
    public void ToStream(Stream outstream, object v)
    {
        StreamWriter textWriter = new StreamWriter(outstream);
        serializer.Serialize(textWriter, v);
        textWriter.Flush();
    }
}
```
