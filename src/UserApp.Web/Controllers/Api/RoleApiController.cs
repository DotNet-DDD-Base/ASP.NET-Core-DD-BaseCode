using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.Ais.Interfaces;
using UserApp.Application.Roles.Interfaces;
using UserApp.Domain.Roles;
using UserApp.Web.ViewModels.Roles;

namespace UserApp.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoleApiController : BaseApiController<Role, RoleViewModel>
{
    public RoleApiController(IRoleService service, IMapper mapper) : base(service, mapper)
    {
    }
}
