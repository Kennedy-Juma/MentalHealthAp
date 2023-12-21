using System.ComponentModel.DataAnnotations;

namespace MentalHealthAp.Models
{
    public class ChangePassword
    {
            [Required(ErrorMessage = "Old Password is required")]
            public string? OldPassword { get; set; }
            [Required(ErrorMessage = "New Password is required")]
            public string? NewPassword { get; set; }
            [EmailAddress]
            public string? Email { get; set; }
        }
    }
