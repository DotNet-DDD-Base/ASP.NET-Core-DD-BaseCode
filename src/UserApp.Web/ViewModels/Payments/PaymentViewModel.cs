using System.ComponentModel.DataAnnotations;
using UserApp.Application.Media;
namespace UserApp.Web.ViewModels;

public class PaymentViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "SystemCode is required")]
    public string SystemCode { get; set; } = string.Empty;

    [StringLength(225, MinimumLength = 0, ErrorMessage = "Name length must be between 0 and 225")] 
    public string Name { get; set; } = string.Empty;

    public string Gender  { get; set; } = string.Empty;


}
