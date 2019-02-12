using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Blitz.Rpc.HttpServer.Documentation
{
    public class ApiDescriptorCollectionProvider : IActionDescriptorCollectionProvider
    {
        readonly ServerInfoHolder serverInfo;

        public ApiDescriptorCollectionProvider(ServerInfoHolder serverInfo)
        {
            this.serverInfo = serverInfo;
        }

        public IReadOnlyList<ActionDescriptor> Items
        {
            get
            {
                var list = new List<ActionDescriptor>();
                foreach (var container in serverInfo)
                    foreach (var service in container.Services)
                        list.AddRange(FromRegistrationInfo( service));
                return list;
            }
        }

        public ActionDescriptorCollection ActionDescriptors
        {
            get
            {
                return new ActionDescriptorCollection(Items, 1);
            }
        }


        private IReadOnlyList<ActionDescriptor> FromRegistrationInfo(Internals.RegistrationInfo service)
        {
            var actions = new List<ActionDescriptor>();
            foreach (var action in service.MethodSignatures)
            {
                var path = action.Value.Signature;
                if (path.StartsWith("/"))
                    path = path.Substring(1);

                var desc = new ControllerActionDescriptor // Should really use ActionDescription, but then Swagger wont be able to find the method info
                {
                    DisplayName = action.Value.Method.Name,
                    Parameters = new List<ParameterDescriptor>(),
                    MethodInfo = action.Value.Method,
                    
                };

                foreach (var param in action.Value.Method.GetParameters())
                {
                    var bindingInfo = new BindingInfo();
                    bindingInfo.BinderModelName = param.ParameterType.FullName;
                    bindingInfo.BindingSource = BindingSource.Body;
                    desc.Parameters.Add(new ControllerParameterDescriptor
                    {
                        Name = param.Name,
                        ParameterType = param.ParameterType,
                        BindingInfo = bindingInfo,
                        ParameterInfo = param
                    });
                }
                
                desc.Properties["Path"] = path;
                desc.Properties["MethodInfo"] = action.Value;
                desc.RouteValues = new Dictionary<string, string>();
                desc.RouteValues["controller"] = service.Interface.FullName;
                actions.Add(desc);
            }
            return actions;
        }
    }
}
