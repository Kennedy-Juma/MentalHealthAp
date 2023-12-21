using System.ComponentModel.DataAnnotations;

namespace MentalHealthAp.Models
{
    public class Register
    {
        [Required(ErrorMessage = "Full Name is required")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        public string GenerateUserID()
        {

            Random rand = new Random();
            int randomNumber = rand.Next(1000, 10000);
            return $"meg_{randomNumber}";

        }

    }
}
