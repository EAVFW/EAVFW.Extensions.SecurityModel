using EAVFramework.Shared;
using System;

namespace EAVFW.Extensions.SecurityModel
{
    [EntityInterface(EntityKey = "Security Group")]
    public interface ISecurityGroup
    {
        public Guid Id { get; set; }
        public Boolean? IsBusinessUnit { get; set; }
    }
}