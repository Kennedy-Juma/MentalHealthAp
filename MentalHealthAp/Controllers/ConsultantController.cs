using MentalHealthAp.Data;
using MentalHealthAp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MentalHealthAp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultantController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConsultantController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("add-consultant")]
        public async Task<ActionResult> AddConsultant(Consultant consultant)
        {
            _context.Consultant.Add(consultant);
            await _context.SaveChangesAsync();
            return Ok(consultant);

        }

        [HttpGet]
        [Route("active-consultants")]
        public async Task<ActionResult> AvailableConsultants()
        {
            var response = await _context.Consultant.Where(consultant=>consultant.IsActive).ToListAsync();

            return Ok(response);

        }

        [HttpGet]
        [Route("relationship-consultants")]
        public async Task<ActionResult> RelationshipConsultants()
        {
            var response = await _context.Consultant.Where(consultant => consultant.CategoryId==2).ToListAsync();

            return Ok(response);

        }

        [HttpGet]
        [Route("career-consultants")]
        public async Task<ActionResult> CareerConsultants()
        {
            var response = await _context.Consultant.Where(consultant => consultant.CategoryId==3).ToListAsync();

            return Ok(response);

        }
    }
}
