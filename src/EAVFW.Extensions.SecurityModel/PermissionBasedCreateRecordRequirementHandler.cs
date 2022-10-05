using EAVFramework;
using EAVFramework.Endpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EAVFW.Extensions.SecurityModel
{
    public class PermissionBasedCreateRecordRequirementHandler<TContext> :
    AuthorizationHandler<CreateRecordRequirement, EAVResource>
    where TContext : DynamicContext
    {
        private readonly IPermissionStore _permissionStore;

        public PermissionBasedCreateRecordRequirementHandler(IPermissionStore permissionStore)
        {
            _permissionStore = permissionStore;
        }


        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CreateRecordRequirement requirement, EAVResource resource)
        {
            var entitySchemaName = resource.EntityCollectionSchemaName;

            var hasPemission = await _permissionStore.GetPermissions(context.User, resource).AnyAsync(permision => permision == $"{entitySchemaName}CreateGlobal" || (permision == $"{entitySchemaName}Create") || (permision == $"{entitySchemaName}CreateBU"));

            if (hasPemission)
                context.Succeed(requirement);

        }
    }
}
