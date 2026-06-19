using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.Orders.Interfaces;
using UserApp.Domain.Orders;
using UserApp.Web.ViewModels;

namespace UserApp.Web.Controllers;

public class OrderController : BaseController<Order, OrderViewModel>
{
    public OrderController(IOrderService service, IMapper mapper) : base(service, mapper)
    {
        DisplayFieldForEntity["Customer"] = "Name";
    }
}
