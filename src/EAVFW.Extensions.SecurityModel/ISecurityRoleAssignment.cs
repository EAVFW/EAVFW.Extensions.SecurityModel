using EAVFramework.Shared;
using System;

namespace EAVFW.Extensions.SecurityModel
{
    [EntityInterface(EntityKey = "Security Role Assignment")]
    public interface ISecurityRoleAssignment
    {
        public Guid? SecurityRoleId { get; set; }
        public Guid? IdentityId { get; set; }


    }
}