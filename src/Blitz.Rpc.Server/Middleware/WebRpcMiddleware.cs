using Blitz.Rpc.Server.Exceptions;
using Blitz.Rpc.Server.Internals;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Blitz.Rpc.Server.Middleware
{
    public class WebRpcMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ServerConfig _container;

        private ApplicationState AppState;
        private readonly ILogger<WebRpcMiddleware> logger;

        public WebRpcMiddleware(RequestDelegate next, ServerConfig container, ILogger<WebRpcMiddleware> logger)
        {
            this.logger = logger;
            _next = next;
            _container = container;
            this.AppState = new ApplicationState(container);
        }

        public async Task Invoke(HttpContext context, IServiceProvider serviceProvider)
        {
            string Identifier = context.Request.Path.ToUriComponent().Replace(_container.BaseUrl, "");

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
                param = hInfo.CreateParam(context.Request.Body);
                if (param == null)
                {
                    throw new ArgumentOutOfRangeException(hInfo.ParamType.Name, $"Not able to create param of '{hInfo.ParamType.Name}' from string: '{context.Request.Body}'");
                }
            }

            logger.LogTrace("Start handler {handler}", hInfo.HandlerType.FullName);
            var data = hInfo.Execute(param, serviceProvider);
            logger.LogTrace("End handler {handler}", hInfo.HandlerType.FullName);

            context.Response.ContentType = AppState.Container.ResponseType;
            context.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            AppState.Container.Serializer.ToStream(context.Response.Body, data);
        }
    }
}