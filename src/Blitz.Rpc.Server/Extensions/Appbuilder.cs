using Blitz.Rpc.HttpServer.Adapters;
using Blitz.Rpc.HttpServer.Middleware;
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

            var serializers = builder.ApplicationServices.GetServices<ISerializer>();

            foreach (var container in config)
            {
                container.Serializers.AddRange(serializers);
                builder.MapWhen(
                    (ctx) => ctx.Request.Path.ToUriComponent().StartsWith(container.BasePath, StringComparison.OrdinalIgnoreCase),
                    (b) =>
                    {
                        b.UseMiddleware<ErrorHandlerMiddleWare>(container);
                        b.UseMiddleware<WebRpcMiddleware>(container);
                        b.UseMiddleware<ListSupportedInterfaces>(container);
                    });
            }
            return builder;
        }
    }
}