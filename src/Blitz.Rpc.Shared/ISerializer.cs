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
    }
}