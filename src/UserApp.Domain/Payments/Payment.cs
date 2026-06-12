using UserApp.Domain.Common;

namespace UserApp.Domain.Payments;

public class Payment : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Gender  { get; set; } = string.Empty;
    public string SystemCode { get; set; } = "PAYMENT";
}