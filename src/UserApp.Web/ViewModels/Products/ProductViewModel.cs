using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace UserApp.Web.ViewModels.Products;

public class ProductViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Status is required")]
    public string Status { get; set; } = string.Empty;

    public string StatusName { get; set; } = string.Empty;

    public List<SelectListItem> StatusOptions { get; set; } = [];

    public string Paymet { get; set; } = string.Empty;

    public string PaymetName { get; set; } = string.Empty;

    public List<SelectListItem> PaymetOptions { get; set; } = [];
}
