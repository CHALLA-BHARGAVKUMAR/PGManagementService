using PGManagementService.Data.enums;

namespace PGManagementService.Data.DTO
{
    public class MemberRequest
    {
        public string? FullName { get; set; }
        public string? Email { get; set;}
        public string PhoneNumber {  get; set; }
        public int RoomId { get; set; }
    
    }
}
