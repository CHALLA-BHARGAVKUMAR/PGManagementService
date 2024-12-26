using PGManagementService.Data.enums;

namespace PGManagementService.Data.DTO
{
    public class RoomRequest
    {
        public string? RoomNo { get; set; }
        public SharingType RoomType { get; set;}

    }
}
