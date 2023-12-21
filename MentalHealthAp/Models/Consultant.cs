using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MentalHealthAp.Models
{
    public class Consultant
    {
        //[Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public int CategoryId { get; set; }
        public string UserId { get; set; }
       public bool IsActive { get; set; }
    }
}
