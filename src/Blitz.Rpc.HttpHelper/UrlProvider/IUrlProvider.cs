using Blitz.Rpc.Client.BaseClasses;

namespace Blitz.Rpc.HttpHelper.UrlProvider
{
    public interface IUrlProvider
    {
        string GetEndpoint(RpcMethodInfo invokeInfo);
    }
}