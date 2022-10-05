using EAVFramework;
using EAVFramework.Plugins;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EAVFW.Extensions.SecurityModel
{
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
