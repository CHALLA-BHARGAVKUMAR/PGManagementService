namespace PGManagementService.Data.DTO
{
    public class AdminDashboardViewResponse
	{
		public int TotalHostlers { get; set; }
		public int PaymentsDue { get; set; }
		public int PaymentsCompleted { get; set; }
		public int OccupiedBeds { get; set; }
		public int AvailableBeds { get; set; }
	}
}
