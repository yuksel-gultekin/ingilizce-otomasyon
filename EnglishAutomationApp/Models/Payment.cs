using System.ComponentModel.DataAnnotations;

namespace EnglishAutomationApp.Models
{
    public enum PaymentStatus
    {
        Pending = 1,
        Completed = 2,
        Failed = 3,
        Refunded = 4
    }

    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public PaymentStatus Status { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string? PaymentMethod { get; set; }

        [MaxLength(100)]
        public string? TransactionId { get; set; }

        public string? Notes { get; set; }

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Course Course { get; set; } = null!;

        // Computed Properties
        public string StatusText => Status.ToString();
        public string AmountText => $"${Amount:F2}";
    }
}
