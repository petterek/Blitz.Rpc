using Blitz.Rpc.HttpHelper.UrlProvider;
using System.Net.Http;

namespace Blitz.Rpc.HttpHelper
{
    public class IntegratedHttpJsonApiClientConfig
    {
        public IUrlProvider urlProvider;
        public HttpMessageHandler LastHandler;
    }
}