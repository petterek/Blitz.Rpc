
using Blitz.Rpc.HttpServer.Middleware;
using Blitz.Rpc.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Blitz.Rpc.HttpServer.Extensions
{
    public static class AppBuilder
    {
        public static IApplicationBuilder UseWebRpc(this IApplicationBuilder builder)
        {

            var config = builder.ApplicationServices.GetService<ServerInfoHolder>();

            if (config == null) throw new Exceptions.NoConfigurationFoundException();

            var serializer = builder.ApplicationServices.GetService<ISerializer>();

            foreach (var container in config)
            {
                container.Serializer = serializer;
                builder.MapWhen(
                    (ctx) => ctx.Request.Path.ToUriComponent().StartsWith(container.BasePath, StringComparison.OrdinalIgnoreCase),
                    (b) =>
                    {
                        b.UseMiddleware<ErrorHandlerMiddleWare>(container);
                        b.UseMiddleware<WebRpcMiddleware>(container);
                    });
            }
            return builder;
        }

        public static IApplicationBuilder UseListEndpoints(this IApplicationBuilder builder)
        {

            builder.MapWhen(ctx => ctx.Request.Method == "GET", app => app.UseMiddleware<ListSupportedInterfaces>());

            return builder;
        }
    }
}