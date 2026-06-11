using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.Permissions.Interfaces;
using UserApp.Domain.Roles;
using UserApp.Web.ViewModels.Permissions;

namespace UserApp.Web.Controllers;

public class PermissionsController
    : BaseController<Permission, PermissionViewModel>
{
    public PermissionsController(
        IPermissionService service,
        IMapper mapper)
        : base(service, mapper)
    {
    }
}