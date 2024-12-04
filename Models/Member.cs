namespace PGManagementService.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public int RoomId { get; set; }
        public Room? Room { get; set; } // Navigation property
        public ICollection<Payment>? Payments { get; set; } // Payment history
        public ICollection<Query>? Queries { get; set; } // Queries by this member
    }
}