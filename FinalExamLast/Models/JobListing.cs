using Microsoft.AspNetCore.Builder;

namespace FinalExamLast.Models
{
    public class JobListing
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<JobApplication>? JobApplication { get; set; }
        public int? EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }
    }
}
