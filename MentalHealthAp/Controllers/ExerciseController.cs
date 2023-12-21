using MentalHealthAp.Data;
using MentalHealthAp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MentalHealthAp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExerciseController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("share-blogs")]
        public async Task<ActionResult> AddExercise(Exercise exercise)
        {
            _context.Exercise.Add(exercise);
            await _context.SaveChangesAsync();
            return Ok(exercise);

        }

        [HttpGet]
        [Route("blogs")]
        public async Task<ActionResult> Blogs()
        {
            var response = await _context
                           .Exercise
                           .OrderByDescending(exercise => exercise.Id)
                           .ToListAsync();

            return Ok(response);

        }
    }
}
