using UserApp.Domain.OrderDetails;
using UserApp.Infrastructure.Persistence;

namespace UserApp.Infrastructure.Persistence.Repositories;

public class OrderDetailRepository : BaseRepository<OrderDetail>, IOrderDetailRepository
{
    public OrderDetailRepository(AppDbContext db) : base(db)
    {
    }
}
