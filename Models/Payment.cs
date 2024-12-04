using Microsoft.EntityFrameworkCore;

namespace PGManagementService.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public Member? Member { get; set; } // Navigation property

        [Precision(18, 2)]
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? PaymentMode { get; set; } // e.g., UPI, Card, Cash
        public bool IsPaid { get; set; } // Indicates if the payment is successful
    }
}