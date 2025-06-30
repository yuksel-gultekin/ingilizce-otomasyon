using System.ComponentModel.DataAnnotations;

namespace EnglishAutomationApp.Models
{
    public class LessonProgress
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int LessonId { get; set; }

        [Required]
        public ProgressStatus Status { get; set; } = ProgressStatus.NotStarted;

        [Required]
        public DateTime StartDate { get; set; } = DateTime.Now;

        public DateTime? CompletionDate { get; set; }

        public int TimeSpentMinutes { get; set; } = 0;

        public int Score { get; set; } = 0;

        public string? Notes { get; set; }

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Lesson Lesson { get; set; } = null!;

        // Computed Properties
        public string StatusText => Status.ToString();
        public bool IsCompleted => Status == ProgressStatus.Completed;
        public string TimeSpentText => $"{TimeSpentMinutes} min";
    }
}
