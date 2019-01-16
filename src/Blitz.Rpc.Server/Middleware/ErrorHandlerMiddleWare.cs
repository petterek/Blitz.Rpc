using Blitz.Rpc.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Blitz.Rpc.Server.Middleware
{
    public class ErrorHandlerMiddleWare
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ErrorHandlerMiddleWare> logger;
        private readonly ServerConfig container;

        public ErrorHandlerMiddleWare(RequestDelegate next, ServerConfig container, ILogger<ErrorHandlerMiddleWare> logger)
        {
            this.logger = logger;
            this.container = container;
            this.next = next;
        }

        private string GetValueWithDefault(IHeaderDictionary headers, string name, string defaultValue)
        {
            if (headers.ContainsKey(name))
            {
                return headers[name].ToString();
            }
            return defaultValue;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (TargetInvocationException ex) when (ex.InnerException is WebRpcCallFailedException rpcException)
            {
                await WriteRpcExceptionToResponse(context, rpcException);
            }
            catch (TargetInvocationException ex)
            {
                await WriteExceptionToResponse(context, ex.InnerException);
            }
            catch (Exception ex)
            {
                await WriteExceptionToResponse(context, ex);
            }
        }

        private async Task WriteRpcExceptionToResponse(HttpContext context, WebRpcCallFailedException rpcException)
        {
            Guid guid = Guid.Empty;
            Guid.TryParse(context.Request.Headers[HeaderNames.Tracking], out guid);
            Guid.TryParse(context.Request.Headers[HeaderNames.Corrolation], out guid);

            var envelope = new RemoteExceptionInfo()
            {
                CorrelationId = guid,
                Message = rpcException.RemoteException.Message,
                Type = rpcException.RemoteException.Type,
                StackTrace = rpcException.RemoteException.StackTrace,
                Source = rpcException.RemoteException.Source,
                ExceptionId = rpcException.RemoteException.ExceptionId,
                MachineName = rpcException.RemoteException.MachineName,
                InnerException = rpcException.RemoteException.InnerException
            };

            await WriteExceptionEnvelopeToResponse(context, envelope);
            WriteToLog(rpcException);
        }

        private async Task WriteExceptionToResponse(HttpContext context, Exception ex)
        {
            Guid guid = Guid.Empty;
            Guid.TryParse(context.Request.Headers[HeaderNames.Tracking], out guid);
            Guid.TryParse(context.Request.Headers[HeaderNames.Corrolation], out guid);

            var envelope = new RemoteExceptionInfo
            {
                CorrelationId = guid,
                Message = ex.Message,
                Type = ex.GetType().FullName,
                StackTrace = ex.StackTrace,
                Source = ex.Source,
                ExceptionId = Guid.NewGuid(),
                MachineName = container.MachineName(),
                InnerException = ex.InnerException
            };

            await WriteExceptionEnvelopeToResponse(context, envelope);
            WriteToLog(ex);
        }

        private void WriteToLog(Exception ex)
        {
            logger.LogError(ex, "Exception occured in WebRpc handling");
        }

        private Task WriteExceptionEnvelopeToResponse(HttpContext context, RemoteExceptionInfo env)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            container.Serializer.ToStream(context.Response.Body, env);

            return context.Response.Body.FlushAsync();
        }

        private class RemoteExceptionWrapper : Exception
        {
            private readonly RemoteExceptionInfo remoteInfo;

            public RemoteExceptionWrapper(RemoteExceptionInfo remoteInfo) : base($"Exception occured on {remoteInfo.MachineName} - {remoteInfo.ExceptionId}")
            {
                this.remoteInfo = remoteInfo;
                this.Source = remoteInfo.Source;

                this.Data.Add("Id", remoteInfo.ExceptionId);
                this.Data.Add("InnerType", remoteInfo.Type);
            }

            public override string StackTrace
            {
                get
                {
                    return remoteInfo.StackTrace;
                }
            }
        }
    }
}