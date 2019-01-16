using System;
using System.Collections;

namespace Blitz.Rpc.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class RemoteExceptionInfo
    {
        /// <summary>
        /// This is copied from the incomming ClientContext.
        /// </summary>
        public Guid CorrelationId;
        /// <summary>
        /// 
        /// </summary>
        public string MachineName;
        /// <summary>
        /// 
        /// </summary>
        public Guid ExceptionId;
        /// <summary>
        /// 
        /// </summary>
        public string Type;
        /// <summary>
        /// 
        /// </summary>
        public string Message;
        /// <summary>
        /// 
        /// </summary>
        public string StackTrace;
        /// <summary>
        /// 
        /// </summary>
        public string Source;
        /// <summary>
        /// 
        /// </summary>
        public IDictionary Data;

        /// <summary>
        /// Holding on to the inner exception
        /// </summary>
        public Exception InnerException;
    }


}