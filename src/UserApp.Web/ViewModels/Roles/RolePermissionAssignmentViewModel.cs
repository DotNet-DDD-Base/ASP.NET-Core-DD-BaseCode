namespace UserApp.Web.ViewModels.Roles;

public class RolePermissionAssignmentViewModel
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public List<PermissionCheckItem> Permissions { get; set; } = new();
}

public class PermissionCheckItem
{
    public Guid PermissionId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
    public bool IsAssigned { get; set; }
}
