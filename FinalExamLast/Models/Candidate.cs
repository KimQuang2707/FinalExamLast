using Microsoft.AspNetCore.Builder;

namespace FinalExamLast.Models
{
    public class Candidate
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public virtual ICollection<JobApplication>? JobApplications { get; set; }
    }
}
