using EAVFramework;
using EAVFramework.Endpoints;
using EAVFramework.Shared.V2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAVFW.Extensions.SecurityModel
{
    public class PermissionBasedCreateRecordRequirementHandler<TContext> :
    AuthorizationHandler<CreateRecordRequirement, EAVResource>
    where TContext : DynamicContext
    {
        
        private readonly IServiceProvider _serviceProvider;

        public PermissionBasedCreateRecordRequirementHandler(IServiceProvider serviceProvider)
        {
             
            _serviceProvider = serviceProvider;
        }


        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CreateRecordRequirement requirement, EAVResource resource)
        {
            var entitySchemaName = resource.EntityCollectionSchemaName;
            var permissionStore = _serviceProvider.GetRequiredService<IPermissionStore<TContext>>();
            var hasPemission = await permissionStore.GetPermissions(context.User, resource).AnyAsync(permision => permision == $"{entitySchemaName}CreateGlobal" || (permision == $"{entitySchemaName}Create") || (permision == $"{entitySchemaName}CreateBU"));

            if (hasPemission)
                context.Succeed(requirement);

        }
    }
}
