using System.Net.Http;

namespace Blitz.Rpc.HttpHelper.HttpHandlers
{
    internal class AvoidDisposeMessageHandler : DelegatingHandler
    {
        public AvoidDisposeMessageHandler(HttpMessageHandler handler) : base(handler)
        {
        }

        protected override void Dispose(bool disposing)
        {
            // Do nothing. The final handler is managed by the framework
        }
    }
}