using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.{{Name}}s.Interfaces;
using UserApp.Domain.{{Name}}s;
using UserApp.Web.ViewModels;

namespace UserApp.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class {{Name}}ApiController : BaseApiController<{{Name}}, {{Name}}ViewModel>
{
    public {{Name}}ApiController(I{{Name}}Service service, IMapper mapper) : base(service, mapper)
    {
    }
}
