using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.Roles.Interfaces;
using UserApp.Domain.Roles;
using UserApp.Web.ViewModels.Roles;

namespace UserApp.Web.Controllers.Api;

[ApiController]
[Route("api/roles")]
[Authorize]
public class RolesApiController : BaseApiController<Role, RoleViewModel>
{
    public RolesApiController(IRoleService service, IMapper mapper) : base(service, mapper)
    {
    }
}
