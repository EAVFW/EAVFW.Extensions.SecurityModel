using DotNetDevOps.Extensions.EAVFramework.Shared;
using System;

namespace EAVFW.Extensions.SecurityModel
{
    [EntityInterface(EntityKey = "Record Share")]
    public interface ITRecordShare
    {
        public Guid? PermissionId { get; set; }
        public string EntityName { get; set; }
        public string Identity { get; set; }
        public Guid? RecordId { get; set; }
    }
}