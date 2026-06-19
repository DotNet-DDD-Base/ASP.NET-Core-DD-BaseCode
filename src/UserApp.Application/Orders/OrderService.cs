using UserApp.Application.Common;
using UserApp.Domain.Orders;
using UserApp.Application.Orders.Interfaces;

namespace UserApp.Application.Orders;

public class OrderService : BaseService<Order>, IOrderService
{
    public OrderService(IOrderRepository repo) : base(repo)
    {
    }
}
