using System.ComponentModel.DataAnnotations;

namespace MentalHealthAp.Models
{
    public class EditProfile
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
    }
}
