using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserApp.Application.Roles.Interfaces;
using UserApp.Domain.Common;
using UserApp.Domain.Roles;
using UserApp.Web.ViewModels.Roles;

namespace UserApp.Web.Controllers;

public class RolesController : BaseController<Role, RoleViewModel>
{
    private readonly IBaseRepository<Permission> _permRepo;
    private readonly IBaseRepository<RolePermission> _rpRepo;

    public RolesController(
        IRoleService service,
        IMapper mapper,
        IBaseRepository<Permission> permRepo,
        IBaseRepository<RolePermission> rpRepo) : base(service, mapper)
    {
        _permRepo = permRepo;
        _rpRepo = rpRepo;
    }

    public async Task<IActionResult> ManagePermissions(Guid roleId)
    {
        var role = await _service.GetByIdAsync(roleId);
        if (role == null) return NotFound();

        var allPermissions = await _permRepo.ListAsync(0, 10000);
        var assigned = await _rpRepo.ListAsync(0, 10000);
        var assignedIds = assigned
            .Where(x => x.RoleId == roleId)
            .Select(x => x.PermissionId)
            .ToHashSet();

        var vm = new RolePermissionAssignmentViewModel
        {
            RoleId = role.Id,
            RoleName = role.Name,
            Permissions = allPermissions.Select(p => new PermissionCheckItem
            {
                PermissionId = p.Id,
                PermissionName = p.Name,
                IsAssigned = assignedIds.Contains(p.Id)
            }).ToList()
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> ManagePermissions(Guid roleId, List<Guid> permissionIds)
    {
        var role = await _service.GetByIdAsync(roleId);
        if (role == null) return NotFound();

        permissionIds ??= new List<Guid>();

        var existing = await _rpRepo.ListAsync(0, 10000);
        var toRemove = existing.Where(x => x.RoleId == roleId).ToList();

        foreach (var rp in toRemove)
            _rpRepo.Remove(rp);

        foreach (var pid in permissionIds)
            await _rpRepo.AddAsync(new RolePermission { RoleId = roleId, PermissionId = pid });

        await _rpRepo.SaveChangesAsync();

        TempData["Success"] = "Permissions updated successfully.";
        return RedirectToAction(nameof(ManagePermissions), new { roleId });
    }
}
