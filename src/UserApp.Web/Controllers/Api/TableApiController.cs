using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.Tables.Interfaces;
using UserApp.Domain.Tables;
using UserApp.Web.ViewModels;

namespace UserApp.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TableApiController : BaseApiController<Table, TableViewModel>
{
    public TableApiController(ITableService service, IMapper mapper) : base(service, mapper)
    {
    }
}
