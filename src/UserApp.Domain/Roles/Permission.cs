using UserApp.Domain.Common;

namespace UserApp.Domain.Roles;

public class Permission : Entity<Guid>
{
    public string Name { get; private set; } = default!;

    private readonly List<RolePermission> _rolePermissions = new();
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions;

    private Permission() { }

    public static Permission Create(string name)
    {
        return new Permission
        {
            Id = Guid.NewGuid(),
            Name = name
        };
    }
}