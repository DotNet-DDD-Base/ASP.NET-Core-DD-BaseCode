using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using UserApp.Application.Media;
namespace UserApp.Web.ViewModels;

public class CustomerViewModel
{
    public Guid Id { get; set; }

    [StringLength(225, MinimumLength = 0, ErrorMessage = "Name length must be between 0 and 225")] 
    public string Name { get; set; } = string.Empty;

    public int Phone { get; set; }

    [StringLength(300, MinimumLength = 0, ErrorMessage = "Address length must be between 0 and 300")] 
    public string Address { get; set; } = string.Empty;


    public List<string> ImageUrls { get; set; } = [];
    public List<MediaDto> MediaList { get; set; } = [];

}
