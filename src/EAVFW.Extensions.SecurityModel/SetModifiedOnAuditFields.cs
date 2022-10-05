using EAVFramework;
using EAVFramework.Plugins;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EAVFW.Extensions.SecurityModel
{
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
}
