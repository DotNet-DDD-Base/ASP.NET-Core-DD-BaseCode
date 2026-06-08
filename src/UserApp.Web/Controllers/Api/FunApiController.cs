using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.Funs.Interfaces;
using UserApp.Domain.Funs;
using UserApp.Web.ViewModels;

namespace UserApp.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FunApiController : BaseApiController<Fun, FunViewModel>
{
    public FunApiController(IFunService service, IMapper mapper) : base(service, mapper)
    {
    }
}
