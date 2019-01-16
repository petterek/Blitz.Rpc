using System.Threading.Tasks;

namespace Blitz.Rpc.Client.BaseClasses
{
    public interface IApiClient
    {
        Task<object> Invoke(RpcMethodInfo toCall, object[] param);
    }
}