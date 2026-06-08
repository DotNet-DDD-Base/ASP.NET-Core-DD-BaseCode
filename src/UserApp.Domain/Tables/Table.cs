using UserApp.Domain.Common;

namespace UserApp.Domain.Tables;

public class Table : Entity<Guid>
{
    public int Name { get; set; }
    public int price { get; set; }

}
