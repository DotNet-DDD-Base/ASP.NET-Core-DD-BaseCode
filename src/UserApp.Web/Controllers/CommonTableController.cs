using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.CommonTables.Interfaces;
using UserApp.Domain.CommonTables;
using UserApp.Web.ViewModels.CommonTables;

namespace UserApp.Web.Controllers;

public class CommonTableController : BaseController<CommonTable, CommonTableViewModel>
{
    public CommonTableController(ICommonTableService service, IMapper mapper) : base(service, mapper)
    {
    }
}
