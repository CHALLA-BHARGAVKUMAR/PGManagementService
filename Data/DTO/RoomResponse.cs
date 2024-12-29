using PGManagementService.Models;

namespace PGManagementService.Data.DTO
{
    public class RoomResponse
    {
        public int Id { get; set; }
        public string RoomNo { get; set; }
        public int Occupied { get; set; }
        public int BedsAvailable { get; set; }
        public string? Type { get; set; }
        public int Capacity { get; set; }

        public List<Member>? Members { get; set; }
    }
}
