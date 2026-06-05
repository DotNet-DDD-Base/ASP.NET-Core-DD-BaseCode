using AutoMapper;
using UserApp.Domain.Users;
using UserApp.Domain.Common;
using UserApp.Web.ViewModels;

namespace UserApp.Web.Controllers;

public class UsersController : BaseController<User, UserViewModel>
{
    public UsersController(IBaseRepository<User> repo, IMapper mapper)
        : base(repo, mapper)
    {
    }
}