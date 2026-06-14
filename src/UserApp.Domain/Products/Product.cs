using UserApp.Domain.Common;

namespace UserApp.Domain.Products;

public class Product : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Paymet { get; set; } = string.Empty;
}
