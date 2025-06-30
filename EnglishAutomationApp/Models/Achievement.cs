using System.ComponentModel.DataAnnotations;

namespace EnglishAutomationApp.Models
{
    public enum AchievementType
    {
        LessonCompletion = 1,
        CourseCompletion = 2,
        VocabularyMastery = 3,
        StudyStreak = 4,
        TimeSpent = 5,
        QuizScore = 6
    }

    public class Achievement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public AchievementType Type { get; set; }

        [MaxLength(10)]
        public string? Icon { get; set; }

        [MaxLength(20)]
        public string? Color { get; set; }

        [Required]
        public int RequiredValue { get; set; }

        [Required]
        public int Points { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();

        // Computed Properties
        public string TypeText => Type.ToString();
    }

    public class UserAchievement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int AchievementId { get; set; }

        [Required]
        public DateTime EarnedDate { get; set; } = DateTime.Now;

        public int CurrentValue { get; set; }

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Achievement Achievement { get; set; } = null!;
    }
}
