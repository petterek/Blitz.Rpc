using Blitz.Rpc.HttpServer.Exceptions;
using Blitz.Rpc.HttpServer.Internals;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
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
            string Identifier = context.Request.Path.ToUriComponent().Replace(_container.BasePath, "");

            if (Identifier == "" | context.Request.Method == "GET")
            {
                await _next.Invoke(context);
                return;
            }

            HandlerInfo hInfo;
            try
            {
                hInfo = AppState.GetHandler(Identifier);
            }
            catch (Exception ex)
            {
                //Just swollow the body to avoid errors in server...
                new System.IO.StreamReader(context.Request.Body).ReadToEnd();
                throw new UnableToGetHandlerException(Identifier);
            }

            AppState.ValidateRequest(hInfo); //Throw if not valid.

            object param = null;
            if (hInfo.ParamType != null)
            {
                param = hInfo.CreateParam(context.Request.Body, hInfo.ParamType);
                if (param == null)
                {
                    throw new ArgumentOutOfRangeException(hInfo.ParamType.Name, $"Not able to create param of '{hInfo.ParamType.Name}' from string: '{context.Request.Body}'");
                }
            }

            logger.LogTrace("Start handler {handler}", hInfo.HandlerType.FullName);
            var data = hInfo.Execute(param, serviceProvider);
            logger.LogTrace("End handler {handler}", hInfo.HandlerType.FullName);

            var outStream = AppState.Container.Serializers.First();


            context.Response.ContentType = outStream.ProduceMimeType;
            context.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            outStream.ToStream(context.Response.Body, data);
        }
    }
}