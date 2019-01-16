using System;
using System.IO;

namespace Blitz.Rpc.Server
{
    public interface ISerializer
    {
        void ToStream(Stream outstream, object v);

        object FromStream(Stream stream, Type returnType);
    }
}