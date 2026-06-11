using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.Permissions.Interfaces;
using UserApp.Domain.Roles;
using UserApp.Infrastructure.Persistence;
using UserApp.Infrastructure.Persistence.Seed;
using UserApp.Web.ViewModels.Permissions;

namespace UserApp.Web.Controllers;

public class PermissionsController
    : BaseController<Permission, PermissionViewModel>
{
    private readonly IServiceProvider _serviceProvider;

    public PermissionsController(
        IPermissionService service,
        IMapper mapper,
        IServiceProvider serviceProvider)
        : base(service, mapper)
    {
        _serviceProvider = serviceProvider;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await RbacSeeder.SeedRolesAsync(db);
        await RbacSeeder.SeedPermissionsAsync(db);
        await RbacSeeder.SeedAdminRolePermissionsAsync(db);

        TempData["Success"] = "Permissions synced successfully from all controllers.";
        return RedirectToAction(nameof(Index));
    }
}