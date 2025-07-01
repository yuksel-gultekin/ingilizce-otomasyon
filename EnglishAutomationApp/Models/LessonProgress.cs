using System;

namespace EnglishAutomationApp.Models
{
    public class LessonProgress
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int LessonId { get; set; }
        public ProgressStatus Status { get; set; }
        public int ProgressPercentage { get; set; }
        public int TimeSpentMinutes { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime? LastAccessDate { get; set; }

        public User? User { get; set; }
        public Lesson? Lesson { get; set; }
    }
}
