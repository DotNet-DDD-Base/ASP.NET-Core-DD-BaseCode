using UserApp.Domain.Common;

namespace UserApp.Domain.Funs;

public class Fun : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public int Price { get; set; }

}
