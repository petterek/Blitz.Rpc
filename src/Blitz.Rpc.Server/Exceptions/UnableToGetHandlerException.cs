using System;
using System.Runtime.Serialization;

namespace Blitz.Rpc.HttpServer.Exceptions
{
    [Serializable]
    internal class UnableToGetHandlerException : Exception
    {
        public UnableToGetHandlerException()
        {
        }

        public UnableToGetHandlerException(string message) : base(message)
        {
        }

        public UnableToGetHandlerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnableToGetHandlerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}