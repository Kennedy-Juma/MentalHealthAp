namespace MentalHealthAp.Models
{
    public class Consultation
    {
        public int Id { get; set; }
        public string Therapist { get; set; }
        public string UserId { get; set; }
        public DateTime DateConsulted { get; set; }
    }
}
