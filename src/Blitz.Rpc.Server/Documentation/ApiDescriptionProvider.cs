using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static Blitz.Rpc.HttpServer.Internals.RegistrationInfo;

namespace Blitz.Rpc.HttpServer.Documentation
{
    public class ApiDescriptionProvider : IApiDescriptionProvider
    {
        public int Order => 0;

        public void OnProvidersExecuted(ApiDescriptionProviderContext context)
        {
        }

        public void OnProvidersExecuting(ApiDescriptionProviderContext context)
        {
            foreach (var action in context.Actions.Select(FromActionDescription).OfType<ApiDescription>())
                context.Results.Add(action);
        }

        private ApiDescription FromActionDescription(ActionDescriptor action)
        {
            if (!action.Properties.ContainsKey("MethodInfo") && !action.Properties.ContainsKey("Path"))
                return null; // Not a ServiceHost action

            var method =(MethodMap) action.Properties["MethodInfo"];
            var desc = new ApiDescription
            {
                RelativePath = (string)action.Properties["Path"],
                ActionDescriptor = action,
                HttpMethod = "POST",
            };
            desc.SupportedRequestFormats.Add(new ApiRequestFormat { MediaType = "application/json" });
            desc.SupportedResponseTypes.Add(new ApiResponseType { IsDefaultResponse=true, ApiResponseFormats = new List<ApiResponseFormat> { new ApiResponseFormat {MediaType="application/json" } }  , StatusCode=200, Type=method.Method.ReturnType });

            foreach (var param in action.Parameters)
            {
                desc.ParameterDescriptions.Add(FromParameterDescriptor(param));
            }
                                             
            return desc;
        }

        private ApiParameterDescription FromParameterDescriptor(ParameterDescriptor param)
        {
            return new ApiParameterDescription
            {
                Name = param.Name,
                Type = param.ParameterType,
                ParameterDescriptor = param,
                Source = param.BindingInfo?.BindingSource,
            };
        }
    }
}
