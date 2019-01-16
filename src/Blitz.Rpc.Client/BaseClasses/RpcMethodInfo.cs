using System;

namespace Blitz.Rpc.Client.BaseClasses
{
    public class RpcMethodInfo
    {
        public string Name;
        public Type MasterType;
        public Type DefinedIn;
        public Type ReturnType;
        public Type ParamType;
        public string PackageName;
        public string ServiceId;
        public int Major;
        public int Minor;
    }
}