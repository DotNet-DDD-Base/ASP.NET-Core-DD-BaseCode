using Microsoft.EntityFrameworkCore;
using UserApp.Domain.Roles;

namespace UserApp.Infrastructure.Persistence.Repositories;

public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
{
    public PermissionRepository(AppDbContext db) : base(db)
    {
    }

    public async Task<Permission?> GetByNameAsync(string name)
    {
        return await _db.Set<Permission>()
            .FirstOrDefaultAsync(x => x.Name == name);
    }
}