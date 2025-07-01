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
        public int ProgressPercentage { get; set; } = 0;

        [Required]
        public int TimeSpentMinutes { get; set; } = 0;

        [Required]
        public DateTime StartDate { get; set; } = DateTime.Now;

        public DateTime? CompletionDate { get; set; }

        public DateTime? LastAccessDate { get; set; }

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Lesson Lesson { get; set; } = null!;

        // Computed Properties
        public string StatusText => Status.ToString();
        public string ProgressText => $"{ProgressPercentage}%";
        public bool IsCompleted => Status == ProgressStatus.Completed;
        public string TimeSpentText => $"{TimeSpentMinutes} min";
    }
}
