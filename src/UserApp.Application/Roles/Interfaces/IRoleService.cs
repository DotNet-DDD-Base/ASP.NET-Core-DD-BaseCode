using UserApp.Application.Common;
using UserApp.Domain.Roles;

namespace UserApp.Application.Roles.Interfaces;

public interface IRoleService : IBaseService<Role>
{
    Task<Role?> GetByNameAsync(string name);
}