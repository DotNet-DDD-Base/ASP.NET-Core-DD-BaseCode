using System.ComponentModel.DataAnnotations.Schema;
using UserApp.Domain.Common;
using UserApp.Domain.Customers;

namespace UserApp.Domain.Orders;

public class Order : Entity<Guid>, IHasMedia
{
    public int OrderNo { get; set; }
    public DateTime Date { get; set; }
    public int Total { get; set; }
    [Column("Customer_id")]
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }

}