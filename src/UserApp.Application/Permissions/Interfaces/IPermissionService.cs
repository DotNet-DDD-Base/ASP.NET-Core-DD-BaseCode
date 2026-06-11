using UserApp.Application.Common;
using UserApp.Application.Common.Interfaces;
using UserApp.Application.Permissions.DTOs;
using UserApp.Domain.Roles;

namespace UserApp.Application.Permissions.Interfaces;

public interface IPermissionService : IBaseService<Permission>
{
}