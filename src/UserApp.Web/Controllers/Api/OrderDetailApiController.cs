using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.OrderDetails.Interfaces;
using UserApp.Domain.OrderDetails;
using UserApp.Web.ViewModels;

namespace UserApp.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderDetailApiController : BaseApiController<OrderDetail, OrderDetailViewModel>
{
    public OrderDetailApiController(IOrderDetailService service, IMapper mapper) : base(service, mapper)
    {
    }
}
