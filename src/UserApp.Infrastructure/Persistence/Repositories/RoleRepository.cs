using Microsoft.EntityFrameworkCore;
using UserApp.Domain.Roles;

namespace UserApp.Infrastructure.Persistence.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _db;

    public RoleRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _db.Set<Role>()
            .FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _db.Set<Role>()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Role role)
    {
        await _db.Set<Role>().AddAsync(role);
    }

    public async Task SaveAsync()
    {
        await _db.SaveChangesAsync();
    }
}