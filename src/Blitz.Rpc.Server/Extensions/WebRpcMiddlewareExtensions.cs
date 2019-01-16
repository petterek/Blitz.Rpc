using Blitz.Rpc.Server.Middleware;
using Microsoft.AspNetCore.Builder;
using System;

namespace Blitz.Rpc.Server.Extensions
{
    public static class WebRpcMiddlewareExtensions
    {
        public static IApplicationBuilder ConfigureWebRpc(this IApplicationBuilder builder, HostInfo info, ISerializer serializer, Action<ServerConfig> config)
        {
            var container = new ServerConfig(serializer, info);
            config(container);
            
            builder.MapWhen(
                (ctx) => ctx.Request.Path.ToUriComponent().StartsWith(container.BaseUrl, StringComparison.OrdinalIgnoreCase),
                (b) =>
                {
                    b.UseMiddleware<ErrorHandlerMiddleWare>(container);
                    b.UseMiddleware<WebRpcMiddleware>(container);
                    b.UseMiddleware<ListSupportedInterfaces>(container);
                });

            return builder;
        }
    }
}