using EAVFramework;
using EAVFramework.Endpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace EAVFW.Extensions.SecurityModel
{
    public class PermissionBasedUpdateRecordRequirementHandler<TContext> :
  AuthorizationHandler<UpdateRecordRequirement, EAVResource>
  where TContext : DynamicContext
    {
        private readonly IServiceProvider _serviceProvider;

        public PermissionBasedUpdateRecordRequirementHandler(IServiceProvider serviceProvider)
        {
            
            _serviceProvider = serviceProvider;
        }


        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UpdateRecordRequirement requirement, EAVResource resource)
        {
            var entitySchemaName = resource.EntityCollectionSchemaName;
            var permissionStore = _serviceProvider.GetRequiredService<IPermissionStore<TContext>>();
            var hasPemission = await permissionStore.GetPermissions(context.User, resource).AnyAsync(permision => permision == $"{entitySchemaName}UpdateGlobal" || (permision == $"{entitySchemaName}Update") || (permision == $"{entitySchemaName}UpdateBU"));


            if (hasPemission)
                context.Succeed(requirement);

        }
    }
}
