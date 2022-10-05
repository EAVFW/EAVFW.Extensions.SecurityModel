using EAVFramework;
using EAVFramework.Plugins;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EAVFW.Extensions.SecurityModel
{
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
}
