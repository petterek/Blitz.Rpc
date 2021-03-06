using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Blitz.Rpc.HttpServer.Documentation
{
    public class ApiDescriptorGroupCollectionProvider : IApiDescriptionGroupCollectionProvider
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly IApiDescriptionProvider[] _apiDescriptionProviders;

        private ApiDescriptionGroupCollection _apiDescriptionGroups;

        public ApiDescriptorGroupCollectionProvider(
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            IEnumerable<IApiDescriptionProvider> apiDescriptionProviders)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _apiDescriptionProviders = apiDescriptionProviders.OrderBy(item => item.Order).ToArray();
        }

        public ApiDescriptionGroupCollection ApiDescriptionGroups
        {
            get
            {
                var actionDescriptors = _actionDescriptorCollectionProvider.ActionDescriptors;
                if (_apiDescriptionGroups == null || _apiDescriptionGroups.Version != actionDescriptors.Version)
                {
                    _apiDescriptionGroups = GetCollection(actionDescriptors);
                }

                return _apiDescriptionGroups;
            }
        }

        private ApiDescriptionGroupCollection GetCollection(ActionDescriptorCollection actionDescriptors)
        {
            var context = new ApiDescriptionProviderContext(actionDescriptors.Items);

            foreach (var provider in _apiDescriptionProviders)
            {
                provider.OnProvidersExecuting(context);
            }

            for (var i = _apiDescriptionProviders.Length - 1; i >= 0; i--)
            {
                _apiDescriptionProviders[i].OnProvidersExecuted(context);
            }

            var groups = context.Results
                .GroupBy(d => d.GroupName)
                .Select(g => new ApiDescriptionGroup(g.Key, g.ToArray()))
                .ToArray();

            return new ApiDescriptionGroupCollection(groups, actionDescriptors.Version);
        }   
    }
}
