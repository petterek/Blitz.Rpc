using Blitz.Rpc.Client.BaseClasses;
using System;
using System.Runtime.Serialization;

namespace Blitz.Rpc.HttpHelper.UrlProvider
{
    [Serializable]
    internal class UrlNotConfiguredException : Exception
    {
        private RpcMethodInfo invokeInfo;

        public UrlNotConfiguredException()
        {
        }

        public UrlNotConfiguredException(RpcMethodInfo invokeInfo) : this(invokeInfo.ServiceId)
        {
            this.invokeInfo = invokeInfo;
        }

        public UrlNotConfiguredException(string message) : base(message)
        {
        }

        public UrlNotConfiguredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UrlNotConfiguredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}