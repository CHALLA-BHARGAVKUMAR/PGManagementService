namespace PGManagementService.Models
{
    public class Query
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public Member? Member { get; set; } // Navigation property
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; } // e.g., Open, Resolved, Closed
        public DateTime CreatedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
    }
}