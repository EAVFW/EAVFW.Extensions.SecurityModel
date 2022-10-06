using EAVFramework;
using EAVFramework.Endpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EAVFW.Extensions.SecurityModel
{
    public class PermissionBasedUpdateRecordRequirementHandler<TContext> :
  AuthorizationHandler<UpdateRecordRequirement, EAVResource>
  where TContext : DynamicContext
    {
        private readonly IPermissionStore<TContext> _permissionStore;

        public PermissionBasedUpdateRecordRequirementHandler(IPermissionStore<TContext> permissionStore)
        {
            _permissionStore = permissionStore;
        }


        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UpdateRecordRequirement requirement, EAVResource resource)
        {
            var entitySchemaName = resource.EntityCollectionSchemaName;

            var hasPemission = await _permissionStore.GetPermissions(context.User, resource).AnyAsync(permision => permision == $"{entitySchemaName}UpdateGlobal" || (permision == $"{entitySchemaName}Update") || (permision == $"{entitySchemaName}UpdateBU"));


            if (hasPemission)
                context.Succeed(requirement);

        }
    }
}
