using UserApp.Domain.Common;

namespace UserApp.Domain.SidebarItems;

public class SidebarItem : Entity<Guid>
{
    public string ModuleName { get; set; } = string.Empty;
    public string ControllerName { get; set; } = string.Empty;
    public string? AreaName { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string? IconSvg { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
