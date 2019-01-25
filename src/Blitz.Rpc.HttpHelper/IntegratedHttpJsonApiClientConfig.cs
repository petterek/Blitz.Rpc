using Blitz.Rpc.Client.Helper.UrlProvider;
using System;
using System.Net.Http;

namespace Blitz.Rpc.Client.Helper
{
    public class IntegratedHttpApiClientConfig
    {
        public IUrlProvider urlProvider;
        public HttpMessageHandler LastHandler;
        public TimeSpan TimeOut;
    }
}