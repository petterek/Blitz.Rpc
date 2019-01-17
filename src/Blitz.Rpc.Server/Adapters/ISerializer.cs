using System;
using System.Collections.Generic;
using System.IO;

namespace Blitz.Rpc.HttpServer.Adapters
{
    public interface ISerializer
    {
        void ToStream(Stream outstream, object v);
        object FromStream(Stream stream, Type returnType);

        List<string> AcceptMimeType { get; }
        string ProduceMimeType { get; set; }
    }
}