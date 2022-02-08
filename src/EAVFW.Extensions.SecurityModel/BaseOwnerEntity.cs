using DotNetDevOps.Extensions.EAVFramework;
using DotNetDevOps.Extensions.EAVFramework.Shared;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace EAVFW.Extensions.SecurityModel
{
    [BaseEntity]
    [Serializable]
    [GenericTypeArgument(ArgumentName = "TIdentity", ManifestKey = "Identity")]
    public class BaseOwnerEntity<TIdentity> : BaseIdEntity<TIdentity> where TIdentity : DynamicEntity
    {
        [DataMember(Name = "ownerid")]
        [JsonProperty("ownerid")]
        [JsonPropertyName("ownerid")]
        public Guid? OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        [DataMember(Name = "owner")]
        [JsonProperty("owner")]
        [JsonPropertyName("owner")]
        public TIdentity Owner { get; set; }
    }

    [BaseEntity]
    [Serializable]
    [GenericTypeArgument(ArgumentName = "TIdentity", ManifestKey = "Identity")]
    public class Permission<TIdentity> : BaseOwnerEntity<TIdentity> where TIdentity : DynamicEntity
    {
        [DataMember(Name = "name")]
        [JsonProperty("name")]
        [JsonPropertyName("name")]
        [PrimaryField()]
        public String Name { get; set; }
    }

    [BaseEntity]
    [Serializable]
    [GenericTypeArgument(ArgumentName = "TIdentity", ManifestKey = "Identity")]
    public class SecurityRolePermission<TIdentity> : BaseOwnerEntity<TIdentity> where TIdentity : DynamicEntity
    {
        [DataMember(Name = "permissionid")]
        [JsonProperty("permissionid")]
        [JsonPropertyName("permissionid")]
        public Guid? PermissionId { get; set; }

        [DataMember(Name = "securityroleid")]
        [JsonProperty("securityroleid")]
        [JsonPropertyName("securityroleid")]
        public Guid? SecurityRoleId { get; set; }
    }

    [BaseEntity]
    [Serializable]
    [GenericTypeArgument(ArgumentName = "TIdentity", ManifestKey = "Identity")]
    public class SecurityGroupMember<TIdentity,TSecurityGroup> : BaseOwnerEntity<TIdentity>
        where TIdentity : DynamicEntity
        where TSecurityGroup : SecurityGroup<TIdentity>
    {
        [DataMember(Name = "securitygroupid")]
        [JsonProperty("securitygroupid")]
        [JsonPropertyName("securitygroupid")]
        public Guid? SecurityGroupId { get; set; }


        [DataMember(Name = "identityid")]
        [JsonProperty("identityid")]
        [JsonPropertyName("identityid")]
        public Guid? IdentityId { get; set; }


        [ForeignKey("SecurityGroupId")]
        [JsonProperty("securitygroup")]
        [JsonPropertyName("securitygroup")]
        [DataMember(Name = "securitygroup")]
        public TSecurityGroup SecurityGroup { get; set; }
    }

    [BaseEntity]
    [Serializable]
    [GenericTypeArgument(ArgumentName = "TIdentity", ManifestKey = "Identity")]
    public class SecurityRoleAssignment<TIdentity> : BaseOwnerEntity<TIdentity> where TIdentity : DynamicEntity
    {

        [DataMember(Name = "identityid")]
        [JsonProperty("identityid")]
        [JsonPropertyName("identityid")]
        public Guid? IdentityId { get; set; }

        [DataMember(Name = "securityroleid")]
        [JsonProperty("securityroleid")]
        [JsonPropertyName("securityroleid")]
        public Guid? SecurityRoleId { get; set; }
    }

    [BaseEntity]
    [Serializable]
    [GenericTypeArgument(ArgumentName = "TIdentity", ManifestKey = "Identity")]
    public class RecordShare<TIdentity> : BaseOwnerEntity<TIdentity> where TIdentity : DynamicEntity
    {

        [DataMember(Name = "permissionid")]
        [JsonProperty("permissionid")]
        [JsonPropertyName("permissionid")]
        public Guid? PermissionId { get; set; }

        [DataMember(Name = "entityname")]
        [JsonProperty("entityname")]
        [JsonPropertyName("entityname")]
        public String EntityName { get; set; }

        [DataMember(Name = "identity")]
        [JsonProperty("identity")]
        [JsonPropertyName("identity")]
        [PrimaryField()]
        public String Identity { get; set; }

        [DataMember(Name = "recordid")]
        [JsonProperty("recordid")]
        [JsonPropertyName("recordid")]
        public Guid? RecordId { get; set; }

    }

    [BaseEntity]
    [Serializable]
    [GenericTypeArgument(ArgumentName = "TIdentity", ManifestKey = "Identity")]
    public class SecurityGroup<TIdentity> : BaseOwnerEntity<TIdentity> where TIdentity : DynamicEntity
    {

        [DataMember(Name = "isbusinessunit")]
        [JsonProperty("isbusinessunit")]
        [JsonPropertyName("isbusinessunit")]
        public Boolean? IsBusinessUnit { get; set; }
    }









    public class OwnerBasedAuthorizationQueryExtender<
        TIdentity, TPermission, TSecurityRole, TSecurityRolePermission, TSecurityRoleAssignment, TSecurityGroup, TSecurityGroupMember, TRecordShare> : IQueryExtender 
        where TPermission : Permission<TIdentity>
        where TIdentity : DynamicEntity
        where TSecurityRole : DynamicEntity
        where TSecurityRolePermission : SecurityRolePermission<TIdentity>
        where TSecurityRoleAssignment: SecurityRoleAssignment<TIdentity>
        where TSecurityGroup: SecurityGroup<TIdentity>
        where TSecurityGroupMember: SecurityGroupMember<TIdentity, TSecurityGroup>
        where TRecordShare: RecordShare<TIdentity>
    {

        public IQueryable ApplyTo(IQueryable metadataQuerySet, QueryContext querycontext)
        {
            var context = querycontext.Context;
            var permissions = context.Set<TPermission>();
            var securityRoles = context.Set<TSecurityRole>();
            var securityRolePermissions = context.Set<TSecurityRolePermission>();
            var securityRoleAssignments = context.Set<TSecurityRoleAssignment>();
            var securityGroups = context.Set<TSecurityGroup>();
            var securityGroupMembers = context.Set<TSecurityGroupMember>();
            var recordShares = context.Set<TRecordShare>();

            if (metadataQuerySet is IQueryable<BaseOwnerEntity<TIdentity>> ownerKnown)
            {
                var entitySchemaName = querycontext.Type.GetCustomAttribute<EntityAttribute>()?.CollectionSchemaName;
                var logicalName = querycontext.Type.GetCustomAttribute<EntityDTOAttribute>()?.LogicalName;

                var sub = querycontext.Request.HttpContext.User.FindFirstValue("sub");

                if (string.IsNullOrEmpty(sub))
                    throw new UnauthorizedAccessException("User is not authorized");

                var identity = Guid.Parse(sub); //TODO: bind it with the ClaimsIdentity

                var securityGroupsForIdentity =
                                               from securityGroup in securityGroups
                                               join member in securityGroupMembers on securityGroup.Id equals member.SecurityGroupId
                                               where member.IdentityId == identity
                                               select securityGroup.Id;

                // var securitGroupForCVRNumbers=//from claims, find account and return owning security group;

                var permisionSet = from permision in permissions
                                   join rolepermission in securityRolePermissions on permision.Id equals rolepermission.PermissionId
                                   join roleassignment in securityRoleAssignments on rolepermission.SecurityRoleId equals roleassignment.SecurityRoleId
                                   where roleassignment.IdentityId == identity
                                   select permision.Name;

                var permissionSetFromGroups = from permision in permissions
                                              join rolepermission in securityRolePermissions on permision.Id equals rolepermission.PermissionId
                                              join roleassignment in securityRoleAssignments on rolepermission.SecurityRoleId equals roleassignment.SecurityRoleId
                                              join securitygroup in securityGroupsForIdentity on roleassignment.IdentityId equals securitygroup
                                              select permision.Name;

                var shares = from permision in permissions
                             join share in recordShares on permision.Id equals share.PermissionId
                             where share.Identity == identity.ToString() && share.EntityName ==logicalName
                             select new { permision.Name, share.RecordId };

                var allpermissions = permissionSetFromGroups.Union(permisionSet);




                var query = from record in ownerKnown
                            where
                                allpermissions.Any(
                                    permision => permision == $"{entitySchemaName}ReadGlobal" ||     //Either you have ReadGlobal                      
                                    (permision == $"{entitySchemaName}Read" && (record.OwnerId == identity || securityGroupsForIdentity.Any(g => g == record.OwnerId))) || //Or you have read and owns the record, or a group that you are part of owns the record.
                                    (permision == $"{entitySchemaName}ReadBU" && (
                                         securityGroupMembers.Include(s => s.SecurityGroup).Where(sgm => sgm.IdentityId == record.OwnerId && sgm.SecurityGroup.IsBusinessUnit == true)
                                             .Select(sgm => sgm.SecurityGroupId)
                                             .Any(sg => securityGroupMembers.Any(sgm => sgm.IdentityId == identity && sgm.SecurityGroupId == sg))))
                                     ) ||
                                shares.Any(s => s.RecordId == record.Id && s.Name == $"{entitySchemaName}Read")
                            select record;

                return query;

            }



            return metadataQuerySet;
        }
    }
}