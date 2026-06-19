using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.Orders.Interfaces;
using UserApp.Domain.Orders;
using UserApp.Web.ViewModels;

namespace UserApp.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderApiController : BaseApiController<Order, OrderViewModel>
{
    public OrderApiController(IOrderService service, IMapper mapper) : base(service, mapper)
    {
    }
}
