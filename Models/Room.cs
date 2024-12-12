namespace PGManagementService.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string? RoomNumber { get; set; }
        public string? Type { get; set; } // e.g., Single, Double, Shared
        public int Capacity { get; set; } // Total number of beds
        public int Occupancy { get; set; } // Currently occupied beds
        public int AvailableBeds { get; set; } // Currently available beds
        public ICollection<Member>? Members { get; set; } // Members in this room
    }
}