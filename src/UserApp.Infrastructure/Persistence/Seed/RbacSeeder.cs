using Microsoft.EntityFrameworkCore;
using UserApp.Domain.Roles;

namespace UserApp.Infrastructure.Persistence.Seed;

public static class RbacSeeder
{
    public static async Task SeedRolesAsync(AppDbContext db)
    {
        if (await db.Roles.AnyAsync())
            return;

        var roles = new List<Role>
        {
            Role.Create("Admin"),
            Role.Create("User")
        };

        await db.Roles.AddRangeAsync(roles);
        await db.SaveChangesAsync();
    }
}