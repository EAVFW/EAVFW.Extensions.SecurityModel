using DotNetDevOps.Extensions.EAVFramework.Shared;
using System;

namespace EAVFW.Extensions.SecurityModel
{
    [EntityInterface(EntityKey = "Security Role Permission")]
    public interface ISecurityRolePermission
    {
        
        public Guid? SecurityRoleId { get; set; }
        public Guid? PermissionId { get; set; }
    }
}