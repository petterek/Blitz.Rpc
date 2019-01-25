using Blitz.Rpc.Client.BaseClasses;
using Blitz.Rpc.Shared;
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

        
        private HttpApiClient GetClient(RpcMethodInfo toCall)
        {
            HttpClient httpClient = new HttpClient(Build(), false);
            httpClient.Timeout = config.TimeOut;
            httpClient.BaseAddress = new System.Uri(config.urlProvider.GetEndpoint(toCall));
            return new HttpApiClient(httpClient, serializers);
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