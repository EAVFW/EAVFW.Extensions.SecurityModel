using DotNetDevOps.Extensions.EAVFramework.Shared;
using System;

namespace EAVFW.Extensions.SecurityModel
{
    //[BaseEntity(EntityKey = "Permission")]
    //[Serializable]
    //[GenericTypeArgument(ArgumentName = "TIdentity", ManifestKey = "Identity")]
    //public class PermissionBase<TIdentity> : BaseOwnerEntity<TIdentity> where TIdentity : IdentityBase
    //{
    //    [DataMember(Name = "name")]
    //    [JsonProperty("name")]
    //    [JsonPropertyName("name")]
    //    [PrimaryField()]
    //    public String Name { get; set; }
    //}

    //[BaseEntity]
    //[Serializable]
    //[GenericTypeArgument(ArgumentName = "TIdentity", ManifestKey = "Identity")]
    //public class SecurityRolePermissionBase<TIdentity> : BaseOwnerEntity<TIdentity> where TIdentity : IdentityBase
    //{
    //    [DataMember(Name = "permissionid")]
    //    [JsonProperty("permissionid")]
    //    [JsonPropertyName("permissionid")]
    //    public Guid? PermissionId { get; set; }

    //    [DataMember(Name = "securityroleid")]
    //    [JsonProperty("securityroleid")]
    //    [JsonPropertyName("securityroleid")]
    //    public Guid? SecurityRoleId { get; set; }
    //}

    //[BaseEntity]
    //[Serializable]
    //[GenericTypeArgument(ArgumentName = "TSecurityGroup", ManifestKey = "Security Group")]
    //[GenericTypeArgument(ArgumentName = "TIdentity", ManifestKey = "Identity")]
    //public class SecurityGroupMemberBase<TSecurityGroup,TIdentity> : BaseOwnerEntity<TIdentity> 
    //    where TIdentity : IdentityBase
    //      where TSecurityGroup : SecurityGroupBase<TIdentity>
    //{
    //    [DataMember(Name = "securitygroupid")]
    //    [JsonProperty("securitygroupid")]
    //    [JsonPropertyName("securitygroupid")]
    //    public Guid? SecurityGroupId { get; set; }


    //    [DataMember(Name = "identityid")]
    //    [JsonProperty("identityid")]
    //    [JsonPropertyName("identityid")]
    //    public Guid? IdentityId { get; set; }


    //    [ForeignKey("SecurityGroupId")]
    //    [JsonProperty("securitygroup")]
    //    [JsonPropertyName("securitygroup")]
    //    [DataMember(Name = "securitygroup")]
    //    public TSecurityGroup SecurityGroup { get; set; }
    //}

    //[BaseEntity]
    //[Serializable]
    //[GenericTypeArgument(ArgumentName = "TIdentity", ManifestKey = "Identity")]
    //public class SecurityRoleAssignmentBase<TIdentity> : BaseOwnerEntity<TIdentity> where TIdentity : IdentityBase
    //{

    //    [DataMember(Name = "identityid")]
    //    [JsonProperty("identityid")]
    //    [JsonPropertyName("identityid")]
    //    public Guid? IdentityId { get; set; }

    //    [DataMember(Name = "securityroleid")]
    //    [JsonProperty("securityroleid")] 
    //    [JsonPropertyName("securityroleid")]
    //    public Guid? SecurityRoleId { get; set; }
    //}

    //[BaseEntity]
    //[Serializable]
    //[GenericTypeArgument(ArgumentName = "TIdentity", ManifestKey = "Identity")]
    //public class RecordShareBase<TIdentity> : BaseOwnerEntity<TIdentity> where TIdentity : IdentityBase
    //{

    //    [DataMember(Name = "permissionid")]
    //    [JsonProperty("permissionid")]
    //    [JsonPropertyName("permissionid")]
    //    public Guid? PermissionId { get; set; }

    //    [DataMember(Name = "entityname")]
    //    [JsonProperty("entityname")]
    //    [JsonPropertyName("entityname")]
    //    public String EntityName { get; set; }

    //    [DataMember(Name = "identity")]
    //    [JsonProperty("identity")]
    //    [JsonPropertyName("identity")]
    //    [PrimaryField()]
    //    public String Identity { get; set; }

    //    [DataMember(Name = "recordid")]
    //    [JsonProperty("recordid")]
    //    [JsonPropertyName("recordid")]
    //    public Guid? RecordId { get; set; }

    //}

    ////[BaseEntity]
    ////[Serializable]
    ////[GenericTypeArgument(ArgumentName = "TIdentity", ManifestKey = "Identity")]
    ////public class SecurityGroupBase<TIdentity> : IdentityBase
    ////{

    ////    [DataMember(Name = "isbusinessunit")]
    ////    [JsonProperty("isbusinessunit")]
    ////    [JsonPropertyName("isbusinessunit")]
    ////    public Boolean? IsBusinessUnit { get; set; }
    ////}


    //[BaseEntity(EntityKey = "Identity")]
    //[Serializable]

    //public class IdentityBase : BaseOwnerEntity<IdentityBase>
    //{


    //}

    [EntityInterface(EntityKey ="Permission")]
    public interface IPermission
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}