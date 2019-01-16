using System;

namespace Blitz.Rpc.Server.Events
{
    public class RegistrationInfoEventArgs
    {
        public Type Interface;
    }

    public class UniqueNamespaceEventArgs
    {
        public string Namespace;
    }
}