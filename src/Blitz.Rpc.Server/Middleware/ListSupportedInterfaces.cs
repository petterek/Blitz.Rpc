using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Blitz.Rpc.HttpServer.Middleware
{
    public class ListSupportedInterfaces
    {
        private readonly RequestDelegate next;
        private readonly ServerInfoHolder serverInfos;


        public ListSupportedInterfaces(RequestDelegate next, ServerInfoHolder serverInfos)
        {
            this.next = next;
            this.serverInfos = serverInfos;

        }

        public async Task Invoke(HttpContext context)
        {

            var bodyContent = new StringBuilder();

            foreach (var container in serverInfos)
            {
                bodyContent.AppendLine($@"
                                <h1>{container.DomainName}</h1>
                                <h2>{container.BasePath}</h2>
                                <h2>{container.DomainDescription}</h2>
                                <h2>Supported interfaces:</h2>");

                container.Services.ToList().ForEach(e =>
                {
                    bodyContent.AppendLine($"<h2>{e.ServiceName}</h2>");
                    bodyContent.AppendLine("<ul>");
                    e.MethodSignatures.ToList().ForEach(val =>
                    {
                       bodyContent.AppendLine( @"<li>{ShowMethodInfo(val.Key,val.Value.Method)}</li>");
                    });
                    bodyContent.AppendLine("</ul>");
                });

            }
            context.Response.Headers["content-type"] = "text/html";
            await context.Response.WriteAsync($"<html><head></head><body>{bodyContent}</body></html>");
        }

        private string ShowMethodInfo(string key, MethodInfo method)
        {
            return 
                $@"
                <p><h3>{method.Name}</h3>
                <div>Url:{key}</div>
                <div>Parameters:<ul>
                {string.Join("",method.GetParameters().ToList().Select(el=>$"<li>{el.Name}-{el.ParameterType.FullName}<ul>{CreateParamJson(el.ParameterType)}</ul></li>" ).ToArray())}
                </ul></div>
                <div>
                Returns:
                    {method.ReturnType.FullName}
                    <code><pre>{CreateParamJson(method.ReturnType)}</pre></code>
                </div>
                </p>";
            
        }

        private string CreateParamJson(Type el)
        {
            var instance = Activator.CreateInstance(el);
            var stream = new System.IO.MemoryStream();

            serverInfos[0].Serializer.ToStream(stream, instance);
            stream.Flush();
            stream.Position = 0;

            return new System.IO.StreamReader(stream).ReadToEnd();
        }
    }
}