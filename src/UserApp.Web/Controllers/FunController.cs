using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.Funs.Interfaces;
using UserApp.Domain.Funs;
using UserApp.Web.ViewModels;

namespace UserApp.Web.Controllers;

public class FunController : BaseController<Fun, FunViewModel>
{
    public FunController(IFunService service, IMapper mapper) : base(service, mapper)
    {
    }
}
