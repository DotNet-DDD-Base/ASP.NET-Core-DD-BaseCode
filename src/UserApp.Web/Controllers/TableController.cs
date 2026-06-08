using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.Tables.Interfaces;
using UserApp.Domain.Tables;
using UserApp.Web.ViewModels;

namespace UserApp.Web.Controllers;

public class TableController : BaseController<Table, TableViewModel>
{
    public TableController(ITableService service, IMapper mapper) : base(service, mapper)
    {
    }
}
