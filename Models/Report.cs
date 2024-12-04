namespace PGManagementService.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string? ReportType { get; set; } // e.g., Income, Occupancy
        public DateTime GeneratedDate { get; set; }
        public string? GeneratedBy { get; set; } // Admin who generated the report
        public string? ReportData { get; set; } // Serialized data (e.g., JSON)
    }
}