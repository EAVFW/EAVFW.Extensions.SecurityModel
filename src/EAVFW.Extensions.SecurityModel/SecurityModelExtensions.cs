using EAVFramework;
using EAVFramework.Configuration;
using EAVFramework.Endpoints;
using EAVFramework.Endpoints.Query;
using EAVFramework.Plugins;
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
                .AddPlugin<SetRowVersionIfNull<TDynamicContext, TIdentity>, TDynamicContext, BaseIdEntity<TIdentity>>(EntityPluginExecution.PreOperation, EntityPluginOperation.Update)
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
            builder.Services.AddScoped<IQueryExtender<TContext>, OwnerBasedAuthorizationQueryExtender<TContext,TIdentity, TPermission, TSecurityRole, TSecurityRolePermission, TSecurityRoleAssignment, TSecurityGroup, TSecurityGroupMember, TRecordShare>>();
            builder.Services.AddScoped<IPermissionStore<TContext>, PermissionStore<TContext, TIdentity, TPermission, TSecurityRole, TSecurityRolePermission, TSecurityRoleAssignment, TSecurityGroup, TSecurityGroupMember, TRecordShare>>();
            builder.Services.AddScoped<IAuthorizationHandler, PermissionBasedCreateRecordRequirementHandler<TContext>>();
            builder.Services.AddScoped<IAuthorizationHandler, PermissionBasedUpdateRecordRequirementHandler<TContext>>();
            return builder;
        }

        public static IEAVFrameworkBuilder WithPermissionBasedAuthorization<TContext>(this IEAVFrameworkBuilder builder)

        where TContext : DynamicContext
       
        {
            builder.Services.AddDynamicInterface<IPermission>();
            builder.Services.AddDynamicInterface<IIdentity>();
            builder.Services.AddDynamicInterface<ISecurityRole>();
            builder.Services.AddDynamicInterface<ISecurityRolePermission>();
            builder.Services.AddDynamicInterface<ISecurityRoleAssignment>();
            builder.Services.AddDynamicInterface<ISecurityGroup>();
            builder.Services.AddDynamicInterface<ISecurityGroupMember>();
            builder.Services.AddDynamicInterface<ITRecordShare>();

            builder.Services.AddDynamicScoped<TContext,IQueryExtender<TContext>>( typeof(OwnerBasedAuthorizationQueryExtender<,,,,,,,,>));
            builder.Services.AddDynamicScoped<TContext,IPermissionStore<TContext>>(typeof(PermissionStore<,,,,,,,,>));
            builder.Services.AddDynamicScoped<TContext,IAuthorizationHandler>(typeof( PermissionBasedCreateRecordRequirementHandler<>));
            builder.Services.AddDynamicScoped<TContext,IAuthorizationHandler>(typeof( PermissionBasedUpdateRecordRequirementHandler<>));
            return builder;
        }
    }
}
