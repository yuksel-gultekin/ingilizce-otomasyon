using System.ComponentModel.DataAnnotations;

namespace EnglishAutomationApp.Models
{
    public enum LessonType
    {
        Text = 1,
        Video = 2,
        Audio = 3,
        Interactive = 4,
        Quiz = 5
    }

    public class Lesson
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public LessonType Type { get; set; }

        [Required]
        public int OrderIndex { get; set; }

        public int EstimatedDurationMinutes { get; set; }

        [MaxLength(500)]
        public string? VideoUrl { get; set; }

        [MaxLength(500)]
        public string? AudioUrl { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual Course Course { get; set; } = null!;
        public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();

        // Computed Properties
        public string TypeText => Type.ToString();
        public string DurationText => $"{EstimatedDurationMinutes} min";
    }
}
