using System;
using System.Runtime.Serialization;

namespace Contract
{
    [Serializable]
    public class ExceptionInContractProjectExcption : Exception
    {
        public ExceptionInContractProjectExcption()
        {
        }

        public ExceptionInContractProjectExcption(string message) : base(message)
        {
        }

        public ExceptionInContractProjectExcption(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ExceptionInContractProjectExcption(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
