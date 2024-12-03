using EAVFramework.Endpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAVFW.Extensions.SecurityModel
{
    public class EAVAuthorizationHandlerProvider : IAuthorizationHandlerProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public EAVAuthorizationHandlerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public Task<IEnumerable<IAuthorizationHandler>> GetHandlersAsync(AuthorizationHandlerContext context)
        {
            var handlers = new List<IAuthorizationHandler>(_serviceProvider.GetService<IEnumerable<IAuthorizationHandler>>());

            foreach(var ctxgroup in context.PendingRequirements.OfType<CreateRecordRequirement>().GroupBy(c=>c.Context))
            {
                handlers.Add(_serviceProvider.GetRequiredService(typeof(PermissionBasedCreateRecordRequirementHandler<>).MakeGenericType(ctxgroup.Key)) as IAuthorizationHandler);
            }
            foreach (var ctxgroup in context.PendingRequirements.OfType<UpdateRecordRequirement>().GroupBy(c => c.Context))
            {
                handlers.Add(_serviceProvider.GetRequiredService(typeof(PermissionBasedUpdateRecordRequirementHandler<>).MakeGenericType(ctxgroup.Key)) as IAuthorizationHandler);
            }

            // builder.Services.AddDynamicScoped<TContext,IAuthorizationHandler>(typeof( PermissionBasedCreateRecordRequirementHandler<>));
            // builder.Services.AddDynamicScoped<TContext,IAuthorizationHandler>(typeof( PermissionBasedUpdateRecordRequirementHandler<>));

            return Task.FromResult< IEnumerable< IAuthorizationHandler>>(handlers);
        }
    }
}
