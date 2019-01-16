using System;
using System.Collections.Generic;
using System.Text;

namespace Blitz.Rpc.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class WebRpcCallFailedException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid RemoteExceptionId = Guid.NewGuid();
        
        /// <summary>
        /// 
        /// </summary>
        public RemoteExceptionInfo RemoteException;

        /// <summary>
        /// 
        /// </summary>
        public Type RemoteExceptionType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public WebRpcCallFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteException"></param>
        public WebRpcCallFailedException(RemoteExceptionInfo remoteException)
        {
            RemoteException = remoteException;
            try
            {
                RemoteExceptionId = remoteException.ExceptionId;
                RemoteExceptionType = System.Type.GetType(remoteException.Type);
            }
            catch
            {
            }
        }
    }
}