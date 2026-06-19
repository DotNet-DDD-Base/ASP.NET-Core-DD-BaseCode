using System.ComponentModel.DataAnnotations.Schema;
using UserApp.Domain.Common;

namespace UserApp.Domain.Customers;

public class Customer : Entity<Guid>, IHasMedia
{
    public string Name { get; set; } = string.Empty;
    public int Phone { get; set; }
    public string Address { get; set; } = string.Empty;

}