using System;
using System.Runtime.Serialization;

namespace Blitz.Rpc.HttpServer.Exceptions
{
    [Serializable]
    internal class NoConfigurationFoundException : Exception
    {
        public NoConfigurationFoundException()
        {
        }

        public NoConfigurationFoundException(string message) : base(message)
        {
        }

        public NoConfigurationFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoConfigurationFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}