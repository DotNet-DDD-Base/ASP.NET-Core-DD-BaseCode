using Microsoft.EntityFrameworkCore;
using UserApp.Domain.Roles;

namespace UserApp.Infrastructure.Persistence.Repositories;

public class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext db) : base(db)
    {
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _db.Set<Role>()
            .FirstOrDefaultAsync(x => x.Name == name);
    }
}