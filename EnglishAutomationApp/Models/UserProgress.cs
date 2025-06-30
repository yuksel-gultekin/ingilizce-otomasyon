using System.ComponentModel.DataAnnotations;

namespace EnglishAutomationApp.Models
{
    public enum ProgressStatus
    {
        NotStarted = 1,
        InProgress = 2,
        Completed = 3,
        Paused = 4
    }

    public class UserProgress
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int CourseId { get; set; }

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
        public virtual Course Course { get; set; } = null!;

        // Computed Properties
        public string StatusText => Status.ToString();
        public string ProgressText => $"{ProgressPercentage}%";
        public string TimeSpentText => $"{TimeSpentMinutes / 60}h {TimeSpentMinutes % 60}m";
        public bool IsCompleted => Status == ProgressStatus.Completed;
    }
}
