using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using UserApp.Application.Media;
namespace UserApp.Web.ViewModels;

public class OrderViewModel
{
    public Guid Id { get; set; }

    public int OrderNo { get; set; }

    public DateTime Date { get; set; }

    public int Total { get; set; }

    [Required(ErrorMessage = "Customer is required")]
    public Guid CustomerId { get; set; }
    public List<SelectListItem> CustomerOptions { get; set; } = [];
    public string CustomerName { get; set; } = string.Empty;


    public List<string> ImageUrls { get; set; } = [];
    public List<MediaDto> MediaList { get; set; } = [];

}
