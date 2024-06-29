namespace FinalExamLast.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? Name { get; set; }
        public string? Department { get; set; }
        public string? CompanyName { get; set; }

        public string? CompanyAddress { get; set; }

        public string? contact { get; set; }


        // Mối quan hệ cha-con với JobListings
        public virtual ICollection<JobListing>? JobListings { get; set; }
    }
}
