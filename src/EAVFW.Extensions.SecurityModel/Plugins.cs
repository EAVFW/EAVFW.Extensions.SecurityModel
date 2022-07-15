using DotNetDevOps.Extensions.EAVFramework;
using DotNetDevOps.Extensions.EAVFramework.Configuration;
using DotNetDevOps.Extensions.EAVFramework.Plugins;
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
    }
    public class SetCreatedOnAuditFields<TDynamicContext, TIdentity> : IPlugin<TDynamicContext, BaseIdEntity<TIdentity>>
        where TDynamicContext : DynamicContext
        where TIdentity : DynamicEntity, IIdentity
    {
        public Task Execute(PluginContext<TDynamicContext, BaseIdEntity<TIdentity>> context)
        {
            context.Input.CreatedOn = context.Input.CreatedOn ?? DateTime.UtcNow;
            context.Input.ModifiedOn = context.Input.ModifiedOn ?? context.Input.CreatedOn;
            if (Guid.TryParse(context.User.FindFirstValue("sub"), out Guid identityId))
            {
                context.Input.CreatedById = identityId;
                context.Input.ModifiedById = identityId;
            }
            return Task.CompletedTask;
        }
    }
    public class SetModifiedOnAuditFields<TDynamicContext, TIdentity> : IPlugin<TDynamicContext, BaseIdEntity<TIdentity>>
        where TDynamicContext : DynamicContext
        where TIdentity : DynamicEntity, IIdentity
    {
        public Task Execute(PluginContext<TDynamicContext, BaseIdEntity<TIdentity>> context)
        {
            context.Input.ModifiedOn = DateTime.UtcNow;
            if (Guid.TryParse(context.User.FindFirstValue("sub"), out Guid identityId))
            {
                context.Input.ModifiedById = identityId;
            }
            return Task.CompletedTask;
        }
    }

    public class SetRowVersionIfNull<TDynamicContext, TIdentity> : IPlugin<TDynamicContext, BaseIdEntity<TIdentity>>
         where TDynamicContext : DynamicContext
        where TIdentity : DynamicEntity, IIdentity
    {
        public async Task Execute(PluginContext<TDynamicContext, BaseIdEntity<TIdentity>> context)
        {
            if (context.Input.RowVersion == null)
            {
                context.Input.RowVersion = await context.DB.Set(context.Input.GetType())
                    .Cast<BaseIdEntity<TIdentity>>()
                    .AsNoTracking()
                    .Where(c => c.Id == context.Input.Id)
                    .Select(c => c.RowVersion)
                    .FirstOrDefaultAsync();

                var prop = context.DB.Entry(context.Input).Property(x => x.RowVersion);
                prop.CurrentValue = prop.OriginalValue = context.Input.RowVersion;
            }
        }
    }

    public class SetOwnerOnCreated<TDynamicContext, TIdentity> : IPlugin<TDynamicContext, BaseOwnerEntity<TIdentity>>
         where TDynamicContext : DynamicContext
        where TIdentity : DynamicEntity, IIdentity
    {
        public Task Execute(PluginContext<TDynamicContext, BaseOwnerEntity<TIdentity>> context)
        {
            if (!context.Input.OwnerId.HasValue && Guid.TryParse(context.User.FindFirstValue("sub"), out Guid identityId))
            {
                context.Input.OwnerId = identityId;
            }

            return Task.CompletedTask;
        }
    }
}
