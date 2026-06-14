using UserApp.Domain.Common;

namespace UserApp.Domain.CommonTables;

public class CommonTable : Entity<Guid>
{
    public string Type { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
