using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MentalHealthAp.Models
{
    public class AppUser:IdentityUser
    {
        [Required(ErrorMessage = "FullName is required")]
        public string FullName { get; set; }
    }
}
