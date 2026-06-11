using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.Permissions.Interfaces;
using UserApp.Application.Roles.Interfaces;
using UserApp.Domain.Roles;
using UserApp.Web.ViewModels.Permissions;
using UserApp.Web.ViewModels.Roles;
namespace UserApp.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionsApiController : BaseApiController<Permission, PermissionViewModel>
{
    public PermissionsApiController(IPermissionService service, IMapper mapper) : base(service, mapper)
    {
    }
}
