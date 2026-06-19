using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using UserApp.Application.Media;
namespace UserApp.Web.ViewModels;

public class OrderDetailViewModel
{
    public Guid Id { get; set; }

    [StringLength(225, MinimumLength = 0, ErrorMessage = "ItemName length must be between 0 and 225")] 
    public string ItemName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public int Price { get; set; }

    [Required(ErrorMessage = "Order is required")]
    public Guid OrderId { get; set; }
    public List<SelectListItem> OrderOptions { get; set; } = [];
    public string OrderName { get; set; } = string.Empty;


    public List<string> ImageUrls { get; set; } = [];
    public List<MediaDto> MediaList { get; set; } = [];

}
