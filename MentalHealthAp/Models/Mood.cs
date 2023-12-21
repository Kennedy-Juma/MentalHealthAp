using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MentalHealthAp.Models
{
    public class Mood
    {
        [Key]
        public int Id { get; set; }
        public string? Description { get; set; }
        public DateTime DateCaptured { get; set; }
        public int? Score { get; set; }

        //[ForeignKey("UserId")]
        public string? UserId { get; set; }
       // public AppUser User{get;set;}
    }
}
