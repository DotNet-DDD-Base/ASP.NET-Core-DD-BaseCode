using UserApp.Application.Common;
using UserApp.Application.Permissions.Interfaces;
using UserApp.Domain.Roles;

namespace UserApp.Application.Permissions;

public class PermissionService : BaseService<Permission>, IPermissionService
{
    public PermissionService(IPermissionRepository repo)
        : base(repo)
    {
    }
}