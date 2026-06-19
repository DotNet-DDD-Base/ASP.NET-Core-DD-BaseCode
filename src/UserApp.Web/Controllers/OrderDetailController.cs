using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.OrderDetails.Interfaces;
using UserApp.Domain.OrderDetails;
using UserApp.Web.ViewModels;

namespace UserApp.Web.Controllers;

public class OrderDetailController : BaseController<OrderDetail, OrderDetailViewModel>
{
    public OrderDetailController(IOrderDetailService service, IMapper mapper) : base(service, mapper)
    {
        DisplayFieldForEntity["Order"] = "OrderNo";
    }
}
