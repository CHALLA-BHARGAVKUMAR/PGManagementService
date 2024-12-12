using PGManagementService.Data.enums;

namespace PGManagementService.Data.DTO
{
    public class HostlerDTO
    {
        public string Room {  get; set; }
        public int TotalHostlers { get; set; }
        public int PaymentsDue { get; set; }
        public int PaymentsCompleted { get; set; }
        public int OccupiedBeds { get; set; }
        public int AvailableBeds { get; set; }

       
        
    }   
}       
        
        