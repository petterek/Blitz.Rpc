using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Blitz.Rpc.HttpHelper.HttpHandlers
{
    public class ConsoleLogger : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine(request.RequestUri.ToString());
            var ret = base.SendAsync(request, cancellationToken);
            Console.WriteLine(ret.Result.StatusCode);

            return ret;
        }
    }
}