using Blitz.Rpc.HttpServer.Internals;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Blitz.Rpc.HttpServer.Middleware
{
    public class WebRpcMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ServerInfo _container;

        private ApplicationState AppState;
        private readonly ILogger<WebRpcMiddleware> logger;

        public WebRpcMiddleware(RequestDelegate next, ServerInfo container, ILogger<WebRpcMiddleware> logger)
        {
            this.logger = logger;
            _next = next;
            _container = container;
            this.AppState = new ApplicationState(container);
        }

        public async Task Invoke(HttpContext context, IServiceProvider serviceProvider)
        {
            var t = new Internals.TimerFunc.InternalTimer();
            string Identifier = context.Request.Path.ToUriComponent().Replace(_container.BasePath, "");

            var hInfo = AppState.GetHandler(Identifier);

            if (hInfo == null)
            {
                await _next.Invoke(context);
                return;
            }

            AppState.ValidateRequest(hInfo); //Throw if not valid.

            object param = null;
            if (hInfo.ParamType != null)
            {
                param = hInfo.CreateParam(context.Request.Body);
                if (param == null)
                {
                    throw new ArgumentOutOfRangeException(hInfo.ParamType.Name, $"Not able to create param of '{hInfo.ParamType.Name}' from string: '{context.Request.Body}'");
                }
            }
            var sw = new Stopwatch();
            logger.LogTrace("Start handler {handler}", hInfo.HandlerType.FullName);

            var data = await hInfo.Execute(param, serviceProvider);
                                            

            logger.LogTrace("End handler {handler}", hInfo.HandlerType.FullName);

            var serializer = AppState.Container.Serializer;

            context.Response.Headers.Add("X-ExecutionTimeInNanoSecond", new Microsoft.Extensions.Primitives.StringValues(t.Elapsed().ToString()));
            context.Response.ContentType = serializer.ProduceMimeType;
            context.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            serializer.ToStream(context.Response.Body, data);
        }
    }
}