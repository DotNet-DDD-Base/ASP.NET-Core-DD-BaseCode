using UserApp.Application.Common;
using UserApp.Domain.OrderDetails;
using UserApp.Application.OrderDetails.Interfaces;

namespace UserApp.Application.OrderDetails;

public class OrderDetailService : BaseService<OrderDetail>, IOrderDetailService
{
    public OrderDetailService(IOrderDetailRepository repo) : base(repo)
    {
    }
}
