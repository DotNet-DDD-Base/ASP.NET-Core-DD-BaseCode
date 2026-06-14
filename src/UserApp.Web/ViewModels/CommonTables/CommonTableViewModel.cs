using System.ComponentModel.DataAnnotations;

namespace UserApp.Web.ViewModels.CommonTables;

public class CommonTableViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Type is required")]
    [StringLength(100, ErrorMessage = "Type must be at most 100 characters")]
    public string Type { get; set; } = string.Empty;

    [Required(ErrorMessage = "Code is required")]
    [StringLength(100, ErrorMessage = "Code must be at most 100 characters")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name must be at most 200 characters")]
    public string Name { get; set; } = string.Empty;
}
