using System.ComponentModel.DataAnnotations;

namespace MentalHealthAp.Models
{
    public class Exercise
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }
}
