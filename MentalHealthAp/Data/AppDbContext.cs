using MentalHealthAp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MentalHealthAp.Data
{
  
        public class AppDbContext : DbContext
    {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
            {
            }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


        }
        public DbSet<Mood> Mood { get; set; }
            public DbSet<Category> Category { get; set; }
            public DbSet<Consultant> Consultant { get; set; }
            public DbSet<Exercise> Exercise { get; set; }
            public DbSet<Consultation> Consultation { get; set; }
            public DbSet<TermsAndConditions> TermsAndConditions { get; set; }
        }  
    
}
