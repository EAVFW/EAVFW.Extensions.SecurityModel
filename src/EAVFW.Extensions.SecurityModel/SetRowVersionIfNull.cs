using EAVFramework;
using EAVFramework.Plugins;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EAVFW.Extensions.SecurityModel
{
    public class SetRowVersionIfNull<TDynamicContext, TIdentity> : IPlugin<TDynamicContext, BaseIdEntity<TIdentity>>
         where TDynamicContext : DynamicContext
        where TIdentity : DynamicEntity, IIdentity
    {
        public async Task Execute(PluginContext<TDynamicContext, BaseIdEntity<TIdentity>> context)
        {
            if (context.Input.RowVersion == null)
            {
                context.Input.RowVersion = await context.DB.Context.Set(context.Input.GetType())
                    .Cast<BaseIdEntity<TIdentity>>()
                    .AsNoTracking()
                    .Where(c => c.Id == context.Input.Id)
                    .Select(c => c.RowVersion)
                    .FirstOrDefaultAsync();

                var prop = context.DB.Context.Entry(context.Input).Property(x => x.RowVersion);
                prop.CurrentValue = prop.OriginalValue = context.Input.RowVersion;
            }
        }
    }
}
