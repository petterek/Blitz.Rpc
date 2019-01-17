using System;
using System.Runtime.Serialization;

namespace Blitz.Rpc.HttpServer.Exceptions
{
    [Serializable]
    internal class NoSerializerConfiguredException : Exception
    {
        public NoSerializerConfiguredException()
        {
        }

        public NoSerializerConfiguredException(string message) : base(message)
        {
        }

        public NoSerializerConfiguredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoSerializerConfiguredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}