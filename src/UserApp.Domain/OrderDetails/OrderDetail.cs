using System.ComponentModel.DataAnnotations.Schema;
using UserApp.Domain.Common;
using UserApp.Domain.Orders;

namespace UserApp.Domain.OrderDetails;

public class OrderDetail : Entity<Guid>, IHasMedia
{
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int Price { get; set; }
    [Column("Order_id")]
    public Guid OrderId { get; set; }
    public Order Order { get; set; }

}