using Contract;

namespace UsingServer
{
    public class ServiceOne : IServiceOne
    {
        public ResultData ServiceMethod1(ServiceMethod1Param param)
        {
            return new ResultData { Completed = param.NumberOfTasks - 1 };
        }
    }


}
