using System.ComponentModel.DataAnnotations;

namespace EnglishAutomationApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "User"; // "User" or "Admin"

        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastLoginDate { get; set; }

        // Navigation Properties
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<UserProgress> UserProgresses { get; set; } = new List<UserProgress>();

        // Computed Properties
        public string FullName => $"{FirstName} {LastName}".Trim();
        public bool IsAdmin => Role == "Admin";
    }
}
