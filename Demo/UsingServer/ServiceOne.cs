using System.Threading.Tasks;
using Contract;

namespace UsingServer
{
    public class ServiceOne : IServiceOne
    {
        public ResultData ServiceMethod1(ServiceMethod1Param param)
        {
            return new ResultData { Completed = param.NumberOfTasks - 1 };
        }

        public ResultData ServiceMethod2(ServiceMethod2Param1 param1, ServiceMethod2Param2 param2)
        {
            return new ResultData { Completed = param1.Value + param2.Value };
        }

        public ResultData ServiceMethod2(int param1, int param2)
        {
            return new ResultData { Completed = param2 * param1 };
        }
    }



    public class AsyncService : IAsyncService
    {
        public async Task<ResultData> Method1(ServiceMethod1Param param)
        {

            return await Task.FromResult(new ResultData {Completed=param.NumberOfTasks});

        }
    }

}
