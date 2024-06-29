using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinalExamLast.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<FinalExamLast.Models.Employee> Employee { get; set; } = default!;
        public DbSet<FinalExamLast.Models.Candidate> Candidate { get; set; } = default!;
        public DbSet<FinalExamLast.Models.JobListing> JobListing { get; set; } = default!;
        public DbSet<FinalExamLast.Models.JobApplication> JobApplication { get; set; } = default!;
      

    }
}
