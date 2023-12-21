using MentalHealthAp.Data;
using MentalHealthAp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MentalHealthAp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoodController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MoodController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult> PostMood(Mood mood)
        {
            mood.DateCaptured = DateTime.Now;

            if (mood.Description == "Badly")
            {
                mood.Score = 1;
            }
            else if (mood.Description == "Fine")
            {
                mood.Score = 2;
            }
            else if(mood.Description=="Well")
            { 
                mood.Score = 3;
            }
            else if (mood.Description == "Excellent")
            {
                mood.Score = 4;
            }
            else
            {
                mood.Score = 0;
            }

            _context.Mood.Add(mood);
            await _context.SaveChangesAsync();
            return Ok(mood);

        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> MyMoods()
        {
            var response = await _context
                           .Mood
                           .OrderByDescending(mood => mood.DateCaptured)
                           .ToListAsync();

            return Ok(response);

        }
    }
}
