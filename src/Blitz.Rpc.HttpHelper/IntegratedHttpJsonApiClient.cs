using Blitz.Rpc.Client.BaseClasses;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Blitz.Rpc.Client.Helper
{
    public class IntegratedHttpJsonApiClient : IApiClient
    {
        public List<DelegatingHandler> HttpHandlers = new List<DelegatingHandler>();
        private readonly IntegratedHttpApiClientConfig config;
        readonly IList<ISerializer> serializers;

        public IntegratedHttpJsonApiClient(IntegratedHttpApiClientConfig config, IEnumerable<ISerializer> serializers)
        {
            this.serializers = serializers.ToList();
            this.config = config;
        }

        public Task<object> Invoke(RpcMethodInfo toCall, object[] param)
        {
            return GetClient(toCall).Invoke(toCall, param);
        }

        private HttpApiClient _getClient = null;

        private HttpApiClient GetClient(RpcMethodInfo toCall)
        {
            if (_getClient == null)
            {
                HttpClient httpClient = new HttpClient(Build(), false);
                httpClient.BaseAddress = new System.Uri(config.urlProvider.GetEndpoint(toCall));
                _getClient = new HttpApiClient(httpClient,serializers);
            }
            return _getClient;
        }

        private HttpMessageHandler Build()
        {
            DelegatingHandler lastHandler = null;
            DelegatingHandler firstHandler = null;

            foreach (var handler in HttpHandlers)
            {
                var thisHandler = handler;
                if (lastHandler != null)
                {
                    lastHandler.InnerHandler = thisHandler;
                }
                else
                {
                    firstHandler = thisHandler;
                }

                lastHandler = thisHandler;
            }

            if (firstHandler == null) return config.LastHandler;

            lastHandler.InnerHandler = config.LastHandler;

            return firstHandler;
        }
    }
}