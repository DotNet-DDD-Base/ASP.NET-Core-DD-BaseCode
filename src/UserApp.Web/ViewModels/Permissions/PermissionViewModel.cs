using System.ComponentModel.DataAnnotations;

namespace UserApp.Web.ViewModels.Permissions;

public class PermissionViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Permission name is required")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}