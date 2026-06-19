using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.Customers.Interfaces;
using UserApp.Domain.Customers;
using UserApp.Web.ViewModels;

namespace UserApp.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomerApiController : BaseApiController<Customer, CustomerViewModel>
{
    public CustomerApiController(ICustomerService service, IMapper mapper) : base(service, mapper)
    {
    }
}
