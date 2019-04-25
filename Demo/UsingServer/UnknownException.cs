using System;
using System.Runtime.Serialization;

namespace UsingServer
{
    [Serializable]
    internal class UnknownException : Exception
    {
        public UnknownException()
        {
        }

        public UnknownException(string message) : base(message)
        {
        }

        public UnknownException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnknownException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}