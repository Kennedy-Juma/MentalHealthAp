using System.ComponentModel.DataAnnotations;

namespace MentalHealthAp.Models
{
    public class ResetPassword
    {

        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }
    }
}
