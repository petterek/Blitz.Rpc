using System;

namespace Blitz.Rpc.Server
{
    public class HostInfo
    {
        public Guid AppTypeId { get; }
        public string DomainName { get; }
        public string Description { get; }
        public Guid AppInstanceId { get; }

        public HostInfo(Guid apptypeId, Guid appInstanceId, string domainName, string description)
        {
            AppInstanceId = appInstanceId;
            Description = description;
            DomainName = domainName;
            AppTypeId = apptypeId;
        }
    }
}