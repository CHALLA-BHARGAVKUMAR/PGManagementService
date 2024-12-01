using Microsoft.EntityFrameworkCore;

namespace PGManagementService.Models
{
    public class UtilityBill
    {
        public int Id { get; set; }
        public string? ProviderName { get; set; } // e.g., Electricity Board

        [Precision(18, 2)]
        public decimal Amount { get; set; }
        public DateTime BillDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public ICollection<Payment>? Payments { get; set; } // Payments linked to this bill
    }
}