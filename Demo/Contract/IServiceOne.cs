using System.Threading.Tasks;

namespace Contract
{
    public interface IServiceOne
    {
        ResultData ServiceMethod1(ServiceMethod1Param param);
        ResultData ServiceMethod2(ServiceMethod2Param1 param1, ServiceMethod2Param2 param2);
        ResultData ServiceMethod2(int param1, int param2);
    }


    public interface IAsyncService
    {
        Task<ResultData> Method1(ServiceMethod1Param param);
        Task<ResultData> AsyncMethodThatThrowsKnownException(ExceptionParam param);
        Task<ResultData> AsyncMethodThatThrowsUnknownException(ExceptionParam param);
    }

    public class ExceptionParam
    {
        public string ExceptionMessage;
    }
    

    public class ServiceMethod2Param2
    {
        public int Value;
    }

    public class ServiceMethod2Param1
    {
        public int Value;
    }



}
