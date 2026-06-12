namespace UserApp.Web.ViewModels;

public class UserRoleAssignmentViewModel
{
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public List<RoleCheckItem> Roles { get; set; } = new();
}

public class RoleCheckItem
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool IsAssigned { get; set; }
}
