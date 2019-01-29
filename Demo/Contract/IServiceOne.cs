namespace Contract
{
    public interface IServiceOne
    {
        ResultData ServiceMethod1(ServiceMethod1Param param);
        ResultData ServiceMethod2(ServiceMethod2Param1 param1, ServiceMethod2Param2 param2);
        ResultData ServiceMethod2(int param1, int param2);
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
