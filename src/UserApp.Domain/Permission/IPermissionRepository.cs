using UserApp.Domain.Common;

namespace UserApp.Domain.Roles;

public interface IPermissionRepository : IBaseRepository<Permission>
{
    Task<Permission?> GetByNameAsync(string name);
}