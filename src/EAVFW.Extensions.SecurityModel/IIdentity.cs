using EAVFramework.Shared;
using System;

namespace EAVFW.Extensions.SecurityModel
{
    [EntityInterface(EntityKey = "Identity")]
    public interface IIdentity
    {
        public Guid Id { get; set; }
    }
}