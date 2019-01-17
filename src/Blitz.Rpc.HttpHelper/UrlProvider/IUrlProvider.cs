using Blitz.Rpc.Client.BaseClasses;

namespace Blitz.Rpc.Client.Helper.UrlProvider
{
    public interface IUrlProvider
    {
        string GetEndpoint(RpcMethodInfo invokeInfo);
    }
}