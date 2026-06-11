using UserApp.Application.Common;
using UserApp.Domain.Roles;
using UserApp.Domain.Common; // Add this using statement
using UserApp.Application.Roles.Interfaces;

namespace UserApp.Application.Roles;

public class RoleService : BaseService<Role>, IRoleService
{
    private readonly IRoleRepository _roleRepository;

    // Cast 'repo' to IBaseRepository<Role> when passing to base()
    public RoleService(IRoleRepository repo) : base((IBaseRepository<Role>)repo)
    {
        _roleRepository = repo;
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _roleRepository.GetByNameAsync(name);
    }
}