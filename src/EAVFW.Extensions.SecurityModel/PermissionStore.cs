using DotNetDevOps.Extensions.EAVFramework;
using DotNetDevOps.Extensions.EAVFramework.Configuration;
using DotNetDevOps.Extensions.EAVFramework.Endpoints;
using DotNetDevOps.Extensions.EAVFramework.Plugins;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EAVFW.Extensions.SecurityModel
{

    public class PermissionStore<
       TContext, TIdentity, TPermission, TSecurityRole, TSecurityRolePermission, TSecurityRoleAssignment, TSecurityGroup, TSecurityGroupMember, TRecordShare> : IPermissionStore
        where TContext : DynamicContext
        where TPermission : DynamicEntity, IPermission
        where TIdentity : DynamicEntity, IIdentity
        where TSecurityRole : DynamicEntity, ISecurityRole
        where TSecurityRolePermission : DynamicEntity, ISecurityRolePermission
        where TSecurityRoleAssignment : DynamicEntity, ISecurityRoleAssignment
        where TSecurityGroup : DynamicEntity, ISecurityGroup
        where TSecurityGroupMember : DynamicEntity, ISecurityGroupMember
        where TRecordShare : DynamicEntity, ITRecordShare
    {
        private readonly TContext _dynamicContext;

        public PermissionStore(TContext dynamicContext)
        {
            _dynamicContext = dynamicContext;
        }
        public IQueryable<string> GetPermissions(ClaimsPrincipal user, EAVResource resource)
        {
            var permissions = _dynamicContext.Set<TPermission>();
            var securityRoles = _dynamicContext.Set<TSecurityRole>();
            var securityRolePermissions = _dynamicContext.Set<TSecurityRolePermission>();
            var securityRoleAssignments = _dynamicContext.Set<TSecurityRoleAssignment>();
            var securityGroups = _dynamicContext.Set<TSecurityGroup>();
            var securityGroupMembers = _dynamicContext.Set<TSecurityGroupMember>();
            var recordShares = _dynamicContext.Set<TRecordShare>();

            var identity = Guid.Parse(user.FindFirstValue("sub"));

            var securityGroupsForIdentity =
                                              from securityGroup in securityGroups
                                              join member in securityGroupMembers on securityGroup.Id equals member.SecurityGroupId
                                              where member.IdentityId == identity
                                              select securityGroup.Id;


            //Direct assignments of permissions by role
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



            //Business Units

            var securityGroupsForIdentityByBU =
                           from bu in securityGroups
                           join securityGroup in securityGroupMembers on bu.Id equals securityGroup.SecurityGroupId
                           join member in securityGroupMembers on securityGroup.Id equals member.SecurityGroupId

                           where member.IdentityId == identity
                           select securityGroup.Id;

            var permissionSetFromBU = from permision in permissions
                                      join rolepermission in securityRolePermissions on permision.Id equals rolepermission.PermissionId
                                      join roleassignment in securityRoleAssignments on rolepermission.SecurityRoleId equals roleassignment.SecurityRoleId
                                      join securitygroup in securityGroupsForIdentityByBU on roleassignment.IdentityId equals securitygroup
                                      select permision.Name;
            //Shares

            //var shares = from permision in permissions
            //             join share in recordShares on permision.Id equals share.PermissionId
            //             where (share.IdentityId == identity || securityGroupsForIdentity.Any(g => g == share.IdentityId)) && share.EntityName == type.GetCustomAttribute<EntityDTOAttribute>().LogicalName
            //             select new { permision.Name, share.RecordId };




            var allpermissions = permissionSetFromGroups.Union(permisionSet).Union(permissionSetFromBU);



            return allpermissions;



        }
    }
}
