using UserApp.Domain.Roles;

namespace UserApp.Domain.Roles;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name);
    Task<Role?> GetByIdAsync(Guid id);
    Task AddAsync(Role role);
    Task SaveAsync();
}