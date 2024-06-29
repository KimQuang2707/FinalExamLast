namespace FinalExamLast.Models
{
    public class JobApplication
    {
        public int Id { get; set; }
        public string? name { get; set; }
        public string? RequiredQualifications { get; set; }
        public int? JobListingId { get; set; }
        public virtual JobListing? JobListing { get; set; }
        public int? CandidateId { get; set; }
        public virtual Candidate? Candidate { get; set; }
    }
}
