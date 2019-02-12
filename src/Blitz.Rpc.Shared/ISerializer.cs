using System;
using System.Collections.Generic;
using System.IO;

namespace Blitz.Rpc.Shared
{
    public interface ISerializer
    {
        string ProduceMimeType { get; set; }
        List<String> AcceptMimeType { get; }

        void ToStream(Stream outstream, object v);
        object FromStream(Stream stream, Type returnType);
        object[]FromStream(Stream stream, Type[] returnType);
    }

    public interface IPingPong
    {
        Pong Ping();
    }

    public class Pong
    {
        public DateTime ServerTime;
    }

    public class PingPong : IPingPong
    {
        public Pong Ping()
        {
            return new Pong { ServerTime = DateTime.Now };
        }
    }
}