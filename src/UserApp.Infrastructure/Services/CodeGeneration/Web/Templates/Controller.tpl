using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.{{Name}}s.Interfaces;
using UserApp.Domain.{{Name}}s;
using UserApp.Web.ViewModels;

namespace UserApp.Web.Controllers;

public class {{Name}}Controller : BaseController<{{Name}}, {{Name}}ViewModel>
{
    public {{Name}}Controller(I{{Name}}Service service, IMapper mapper) : base(service, mapper)
    {
{{DisplayFieldCode}}    }
}
