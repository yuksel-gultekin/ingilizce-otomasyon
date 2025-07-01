using System;

namespace EnglishAutomationApp.Models
{
    public enum ProgressStatus
    {
        NotStarted = 0,
        InProgress = 1,
        Completed = 2,
        Paused = 3
    }

    public class UserProgress
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public ProgressStatus Status { get; set; }
        public int ProgressPercentage { get; set; }
        public int TimeSpentMinutes { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime? LastAccessDate { get; set; }

        public User? User { get; set; }
        public Course? Course { get; set; }
    }
}
