using DotNetDevOps.Extensions.EAVFramework;
using DotNetDevOps.Extensions.EAVFramework.Configuration;
using DotNetDevOps.Extensions.EAVFramework.Endpoints;
using DotNetDevOps.Extensions.EAVFramework.Plugins;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace EAVFW.Extensions.SecurityModel
{
    public static class SecurityModelExtensions
    {
        public static IEAVFrameworkBuilder WithAuditFieldsPlugins<TDynamicContext, TIdentity>(this IEAVFrameworkBuilder builder)
             where TDynamicContext : DynamicContext
            where TIdentity : DynamicEntity, IIdentity
        {
            builder
                .AddPlugin<SetCreatedOnAuditFields<TDynamicContext, TIdentity>, TDynamicContext, BaseIdEntity<TIdentity>>(EntityPluginExecution.PreValidate, EntityPluginOperation.Create)
                .AddPlugin<SetModifiedOnAuditFields<TDynamicContext, TIdentity>, TDynamicContext, BaseIdEntity<TIdentity>>(EntityPluginExecution.PreValidate, EntityPluginOperation.Update)
                .AddPlugin<SetRowVersionIfNull<TDynamicContext, TIdentity>, TDynamicContext, BaseIdEntity<TIdentity>>(EntityPluginExecution.PreValidate, EntityPluginOperation.Update)
                .AddPlugin<SetOwnerOnCreated<TDynamicContext, TIdentity>, TDynamicContext, BaseOwnerEntity<TIdentity>>(EntityPluginExecution.PreValidate, EntityPluginOperation.Create);


            return builder;

        }

        public static IEAVFrameworkBuilder WithPermissionBasedAuthorization<TContext, TIdentity, TPermission, TSecurityRole, TSecurityRolePermission, TSecurityRoleAssignment, TSecurityGroup, TSecurityGroupMember, TRecordShare>(this IEAVFrameworkBuilder builder)
            
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
            builder.Services.AddScoped<IQueryExtender, OwnerBasedAuthorizationQueryExtender<TIdentity, TPermission, TSecurityRole, TSecurityRolePermission, TSecurityRoleAssignment, TSecurityGroup, TSecurityGroupMember, TRecordShare>>();
            builder.Services.AddScoped<IPermissionStore, PermissionStore<TContext, TIdentity, TPermission, TSecurityRole, TSecurityRolePermission, TSecurityRoleAssignment, TSecurityGroup, TSecurityGroupMember, TRecordShare>>();
            builder.Services.AddScoped<IAuthorizationHandler, PermissionBasedCreateRecordRequirementHandler<DynamicContext>>();
            builder.Services.AddScoped<IAuthorizationHandler, PermissionBasedUpdateRecordRequirementHandler<DynamicContext>>();
            return builder;
        }
    }
}
