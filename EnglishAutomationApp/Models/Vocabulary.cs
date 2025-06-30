using System.ComponentModel.DataAnnotations;

namespace EnglishAutomationApp.Models
{
    public enum WordDifficulty
    {
        Easy = 1,
        Medium = 2,
        Hard = 3
    }

    public enum PartOfSpeech
    {
        Noun = 1,
        Verb = 2,
        Adjective = 3,
        Adverb = 4,
        Pronoun = 5,
        Preposition = 6,
        Conjunction = 7,
        Interjection = 8
    }

    public class VocabularyWord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string EnglishWord { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string TurkishMeaning { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Pronunciation { get; set; }

        public string? ExampleSentence { get; set; }

        public string? ExampleSentenceTurkish { get; set; }

        [Required]
        public WordDifficulty Difficulty { get; set; }

        [Required]
        public PartOfSpeech PartOfSpeech { get; set; }

        [MaxLength(50)]
        public string? Category { get; set; }

        [MaxLength(500)]
        public string? AudioUrl { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<UserVocabulary> UserVocabularies { get; set; } = new List<UserVocabulary>();

        // Computed Properties
        public string DifficultyText => Difficulty.ToString();
        public string PartOfSpeechText => PartOfSpeech.ToString();
    }

    public class UserVocabulary
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int VocabularyWordId { get; set; }

        [Required]
        public int MasteryLevel { get; set; } = 1; // 1-5 scale

        [Required]
        public DateTime FirstLearnedDate { get; set; } = DateTime.Now;

        public DateTime? LastReviewedDate { get; set; }

        public int ReviewCount { get; set; } = 0;

        public int CorrectAnswers { get; set; } = 0;

        public int TotalAttempts { get; set; } = 0;

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual VocabularyWord VocabularyWord { get; set; } = null!;

        // Computed Properties
        public double AccuracyRate => TotalAttempts > 0 ? (double)CorrectAnswers / TotalAttempts * 100 : 0;
        public bool IsMastered => MasteryLevel >= 4;
    }
}
