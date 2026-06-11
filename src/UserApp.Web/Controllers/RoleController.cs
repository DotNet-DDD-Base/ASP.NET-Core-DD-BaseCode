using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.Roles.Interfaces;
using UserApp.Domain.Roles;
using UserApp.Web.ViewModels.Roles;

namespace UserApp.Web.Controllers;

public class RolesController : BaseController<Role, RoleViewModel>
{
    public RolesController(IRoleService service, IMapper mapper) : base(service, mapper)
    {
    }
}
