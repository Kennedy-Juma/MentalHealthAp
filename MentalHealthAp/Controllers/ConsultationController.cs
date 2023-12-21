using MentalHealthAp.Data;
using MentalHealthAp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MentalHealthAp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConsultationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("consult-therapist")]
        public async Task<ActionResult> ConsultTherapist(Consultation consultation)
        {
            _context.Consultation.Add(consultation);
            await _context.SaveChangesAsync();
            return Ok(consultation);

        }
    }


}
