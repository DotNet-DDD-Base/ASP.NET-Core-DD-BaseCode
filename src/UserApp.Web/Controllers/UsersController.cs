using AutoMapper;
using UserApp.Application.Users.Interfaces;
using UserApp.Web.Controllers;
using UserApp.Web.ViewModels;
using UserApp.Domain.Users;
using Microsoft.AspNetCore.Authorization;


namespace UserApp.Web.Controllers;

    [Authorize(Roles = "Admin")]
public class UsersController : BaseController<User, UserViewModel>
{
    public UsersController(IUserService service, IMapper mapper)
        : base(service, mapper)
    {
    }

}