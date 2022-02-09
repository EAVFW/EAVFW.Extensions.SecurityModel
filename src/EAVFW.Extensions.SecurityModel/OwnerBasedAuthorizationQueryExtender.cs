using DotNetDevOps.Extensions.EAVFramework;
using DotNetDevOps.Extensions.EAVFramework.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;

namespace EAVFW.Extensions.SecurityModel
{
    public class OwnerBasedAuthorizationQueryExtender<
        TIdentity, TPermission, TSecurityRole, TSecurityRolePermission, TSecurityRoleAssignment, TSecurityGroup, TSecurityGroupMember, TRecordShare> : IQueryExtender 
        where TPermission  : DynamicEntity, IPermission
        where TIdentity : DynamicEntity, IIdentity
        where TSecurityRole: DynamicEntity, ISecurityRole
        where TSecurityRolePermission : DynamicEntity, ISecurityRolePermission
        where TSecurityRoleAssignment: DynamicEntity, ISecurityRoleAssignment
        where TSecurityGroup: DynamicEntity, ISecurityGroup
        where TSecurityGroupMember: DynamicEntity, ISecurityGroupMember
        where TRecordShare: DynamicEntity, ITRecordShare
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
                                         (from sgm in securityGroupMembers
                                         join sg in securityGroups on sgm.SecurityGroupId equals sg.Id
                                         where sgm.IdentityId == record.OwnerId && sg.IsBusinessUnit == true
                                         select sg.Id)//.Include("SecurityGroup")
                                           // .Where(sgm => sgm.IdentityId == record.OwnerId && sgm.SecurityGroup.IsBusinessUnit == true)
                                           //  .Select(sgm => sgm.SecurityGroupId)
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