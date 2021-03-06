using Blitz.Rpc.Shared;
using System;
using System.Threading.Tasks;

namespace Blitz.Rpc.Client.BaseClasses
{
    public class ProxyBase
    {
        private readonly IApiClient apiClient;

        public ProxyBase(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        protected object ExecuteSync(RpcMethodInfo methodInfo, object[] param)
        {
            try
            {
                return Execute(methodInfo, param).Result;
            }
            catch (AggregateException ex)
            {
                throw ex.InnerExceptions[0];
            }
        }

        protected async Task<object> Execute(RpcMethodInfo methodInfo, object[] param)
        {
            try
            {
                return await apiClient.Invoke(methodInfo, param);
            }
            catch (WebRpcCallFailedException ex)
            {
                var info = ex.RemoteException;
                try
                {
                    Exception exInstance;
                    if (ex.RemoteExceptionType != null)
                    {
                        exInstance = Activator.CreateInstance(ex.RemoteExceptionType, info.Message ) as Exception;
                    }
                    else
                    {
                        exInstance = new Exception(info.Message);
                    }
                    throw exInstance;
                }
                catch
                {
                    throw;
                }
            }
            catch
            {
                throw;
            }

        }
    }
}