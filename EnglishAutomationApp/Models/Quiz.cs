using System.ComponentModel.DataAnnotations;

namespace EnglishAutomationApp.Models
{
    public enum QuestionType
    {
        MultipleChoice = 1,
        TrueFalse = 2,
        FillInTheBlank = 3,
        Essay = 4,
        Matching = 5
    }

    public class Quiz
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LessonId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int PassingScore { get; set; }

        public int TimeLimit { get; set; } // in minutes

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual Lesson Lesson { get; set; } = null!;
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
        public virtual ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
    }

    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int QuizId { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        public QuestionType Type { get; set; }

        [Required]
        public int OrderIndex { get; set; }

        [Required]
        public int Points { get; set; }

        public string? Explanation { get; set; }

        // Navigation Properties
        public virtual Quiz Quiz { get; set; } = null!;
        public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }

    public class Answer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [Required]
        public string AnswerText { get; set; } = string.Empty;

        [Required]
        public bool IsCorrect { get; set; }

        [Required]
        public int OrderIndex { get; set; }

        // Navigation Properties
        public virtual Question Question { get; set; } = null!;
    }

    public class QuizAttempt
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int QuizId { get; set; }

        [Required]
        public DateTime StartTime { get; set; } = DateTime.Now;

        public DateTime? EndTime { get; set; }

        public int Score { get; set; }

        public bool IsPassed { get; set; }

        public bool IsCompleted { get; set; }

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Quiz Quiz { get; set; } = null!;
        public virtual ICollection<QuizAnswer> QuizAnswers { get; set; } = new List<QuizAnswer>();
    }

    public class QuizAnswer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int QuizAttemptId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        public string? SelectedAnswer { get; set; }

        public bool IsCorrect { get; set; }

        public int PointsEarned { get; set; }

        // Navigation Properties
        public virtual QuizAttempt QuizAttempt { get; set; } = null!;
        public virtual Question Question { get; set; } = null!;
    }
}
