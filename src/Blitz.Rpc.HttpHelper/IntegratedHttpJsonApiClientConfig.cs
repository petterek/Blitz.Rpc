using Blitz.Rpc.Client.Helper.UrlProvider;
using System.Net.Http;

namespace Blitz.Rpc.Client.Helper
{
    public class IntegratedHttpApiClientConfig
    {
        public IUrlProvider urlProvider;
        public HttpMessageHandler LastHandler;

    }
}