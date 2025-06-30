using System.ComponentModel.DataAnnotations;

namespace EnglishAutomationApp.Models
{
    public enum CourseLevel
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3
    }

    public enum CourseType
    {
        Grammar = 1,
        Vocabulary = 2,
        Speaking = 3,
        Listening = 4,
        Reading = 5,
        Writing = 6,
        Pronunciation = 7
    }

    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public CourseLevel Level { get; set; }

        [Required]
        public CourseType Type { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int OrderIndex { get; set; }

        public int EstimatedDurationMinutes { get; set; }

        [MaxLength(500)]
        public string? Prerequisites { get; set; }

        [MaxLength(20)]
        public string? Color { get; set; }

        // Navigation Properties
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<UserProgress> UserProgresses { get; set; } = new List<UserProgress>();
        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

        // Computed Properties
        public string LevelText => Level.ToString();
        public string TypeText => Type.ToString();
        public string PriceText => Price == 0 ? "FREE" : $"${Price:F2}";
    }
}
