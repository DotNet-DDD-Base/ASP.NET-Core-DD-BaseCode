using Microsoft.EntityFrameworkCore;
using System.Reflection;
using UserApp.Domain.Roles;
using UserApp.Infrastructure.Security;

namespace UserApp.Infrastructure.Persistence.Seed;

public static class RbacSeeder
{
    public static async Task SeedRolesAsync(AppDbContext db)
    {
        if (!await db.Roles.AnyAsync())
        {
            var roles = new List<Role>
            {
                Role.Create("Admin"),
                Role.Create("User")
            };

            await db.Roles.AddRangeAsync(roles);
            await db.SaveChangesAsync();
        }
    }

    public static async Task SeedPermissionsAsync(AppDbContext db)
    {
        var assembly = Assembly.Load("UserApp.Web");

        var permissionNames =
            PermissionScanner.GetAllPermissions(assembly);

        foreach (var permissionName in permissionNames)
        {
            var exists = await db.Permissions
                .AnyAsync(x => x.Name == permissionName);

            if (exists)
                continue;

            db.Permissions.Add(
                Permission.Create(permissionName));
        }

        await db.SaveChangesAsync();
    }

    public static async Task SeedAdminRolePermissionsAsync(AppDbContext db)
    {
        var adminRole = await db.Roles
            .FirstOrDefaultAsync(x => x.Name == "Admin");

        if (adminRole == null)
            return;

        var permissions = await db.Permissions.ToListAsync();

        foreach (var permission in permissions)
        {
            bool exists = await db.RolePermissions.AnyAsync(x =>
                x.RoleId == adminRole.Id &&
                x.PermissionId == permission.Id);

            if (exists)
                continue;

            db.RolePermissions.Add(new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = permission.Id
            });
        }

        await db.SaveChangesAsync();
    }
}