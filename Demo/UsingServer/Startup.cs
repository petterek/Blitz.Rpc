using Blitz.Rpc.HttpServer.Extensions;
using Blitz.Rpc.Shared;
using Contract;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace UsingServer
{


    public class Startup
    {
        static Assembly entryAssembly = Assembly.GetEntryAssembly();
        static AssemblyName assemblyName = entryAssembly.GetName();
        static Version assemblyVersion = assemblyName.Version;
        static string version = $"v{assemblyVersion.Major}.{assemblyVersion.Minor}";

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var appType = Guid.NewGuid();

            services.AddSingleton<ISerializer, MySerializer>();
            services.AddWebRpcServices((c) =>
                {
                    c.RegisterService<IServiceOne, ServiceOne>();
                    c.RegisterService<IAsyncService, AsyncService>();
                });

            services.AddSwaggerGen(c => c.SwaggerDoc("api", new OpenApiInfo { Title = assemblyName.Name, Version = version }));
            services.AddMvcCore().AddJsonOptions(c => c.SerializerSettings.ContractResolver = new DefaultContractResolver());
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = "";
                    c.SwaggerEndpoint($"/swagger/api/swagger.json", $"{assemblyName.Name} {version}");
                });

            app.UseWebRpc();
        }
    }


}
