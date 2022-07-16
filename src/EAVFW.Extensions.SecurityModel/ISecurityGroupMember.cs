using DotNetDevOps.Extensions.EAVFramework.Shared;
using System;

namespace EAVFW.Extensions.SecurityModel
{
    [EntityInterface(EntityKey = "Security Group Member")]
    public interface ISecurityGroupMember
    {
        public Guid Id { get; set; }
        public Guid? SecurityGroupId { get; set; }
        public Guid? IdentityId { get; set; }
    
    }
}