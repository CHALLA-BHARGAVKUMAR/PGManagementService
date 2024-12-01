namespace PGManagementService.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; } // Ensure to hash passwords
        public string? Role { get; set; } // e.g., Owner, Manager, Accountant
    }
}