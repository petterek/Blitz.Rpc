using Blitz.Rpc.HttpServer.Internals;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitz.Rpc.HttpServer.Middleware
{
    public class ListSupportedInterfaces
    {
        private readonly RequestDelegate _next;
        private readonly ServerInfo _container;
        private readonly ApplicationState _appState;

        public ListSupportedInterfaces(RequestDelegate next, ServerInfo container)
        {
            _next = next;
            _container = container;
            _appState = new ApplicationState(container);
        }

        public async Task Invoke(HttpContext context)
        {
            string v = context.Request.Path.ToUriComponent();
            var Identifier = v.Replace(_container.BasePath, "");

            var bodyContent = new StringBuilder();

            if (Identifier == "")
            {
                var filter = context.Request.Query["filter"].FirstOrDefault();
                bodyContent.AppendLine($"<h1>{_container.DomainName}</h1><h2></h2>    <h2>Supported interfaces:</h2>");
                if (!string.IsNullOrEmpty(filter))
                {
                    bodyContent.AppendLine($"<h3>Filtered by: <i>{filter}</i></h3>");
                }
                
                _container.Services.ToList().ForEach(e =>
                {
                    bodyContent.AppendLine($"<h2>{e.Value.ServiceName}</h2>");
                    bodyContent.AppendLine("<ul>");
                    e.Value.MethodSignatures.ToList().ForEach(val => {
                        bodyContent.AppendLine($"<li><a href='{val.Key}'>{val.Key }</a></li>");
                    });
                    bodyContent.AppendLine("</ul>");
                });

                
            }
            else
            {
                HandlerInfo info = null;
                try
                {

                    info = _appState.GetHandler(Identifier);
                }
                catch
                {
                    return;
                }

                var stream = new System.IO.MemoryStream();

                List<(string name, string typeName)> propertiesAndFields = new List<(string name, string typeName)>();
                string paramInstanceSerialized = "{}";
                string paramName = "Empty";

                if (info.ParamType != null)
                {
                    propertiesAndFields.AddRange(info.ParamType.GetFields().Select(item => (item.Name, item.FieldType.Name)).ToList());
                    propertiesAndFields.AddRange(info.ParamType.GetProperties().Select(item => (item.Name, item.PropertyType.Name)));
                    var paramInstance = Activator.CreateInstance(info.ParamType);
                    _container.Serializer.ToStream(stream, paramInstance);
                    stream.Position = 0;
                    paramInstanceSerialized = new System.IO.StreamReader(stream).ReadToEnd();
                    paramName = info.ParamType.FullName;
                }

                bodyContent.AppendLine($@"
                            <h3>{paramName}</h3>
                            <code style='white-space: pre-wrap;'>
&#123;
{string.Join(Environment.NewLine, propertiesAndFields.Select(item => $"   {item.name} : {item.typeName}"))}
&#125;
                            </code>
                            <hr/>
                            <h4>Example request</h4>
                            <code cols='60' rows='30' style='background-color: lightgoldenrodyellow; width: 640px; height: 480px;'>
                            {paramInstanceSerialized}
                            </code>");
            }
            context.Response.Headers["content-type"] = "text/html";
            await context.Response.WriteAsync($"<html><head></head><body>{bodyContent}</body></html>");
        }
    }
}