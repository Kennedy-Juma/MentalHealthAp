using System.ComponentModel.DataAnnotations;

namespace MentalHealthAp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
