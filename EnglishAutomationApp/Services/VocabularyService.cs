using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EnglishAutomationApp.Data;
using EnglishAutomationApp.Models;

namespace EnglishAutomationApp.Services
{
    public class VocabularyService
    {
        public static async Task<List<VocabularyWord>> GetAllWordsAsync()
        {
            using var context = new AppDbContext();
            return await context.VocabularyWords
                .Where(w => w.IsActive)
                .OrderBy(w => w.EnglishWord)
                .ToListAsync();
        }

        public static async Task<List<VocabularyWord>> GetWordsByCategoryAsync(string category)
        {
            using var context = new AppDbContext();
            return await context.VocabularyWords
                .Where(w => w.IsActive && w.Category == category)
                .OrderBy(w => w.EnglishWord)
                .ToListAsync();
        }

        public static async Task<List<VocabularyWord>> GetWordsByDifficultyAsync(WordDifficulty difficulty)
        {
            using var context = new AppDbContext();
            return await context.VocabularyWords
                .Where(w => w.IsActive && w.Difficulty == difficulty)
                .OrderBy(w => w.EnglishWord)
                .ToListAsync();
        }

        public static async Task<List<VocabularyWord>> SearchWordsAsync(string searchTerm)
        {
            using var context = new AppDbContext();
            return await context.VocabularyWords
                .Where(w => w.IsActive && 
                    (w.EnglishWord.Contains(searchTerm) || 
                     w.TurkishMeaning.Contains(searchTerm) ||
                     (w.ExampleSentence != null && w.ExampleSentence.Contains(searchTerm))))
                .OrderBy(w => w.EnglishWord)
                .ToListAsync();
        }

        public static async Task<VocabularyWord> AddWordAsync(VocabularyWord word)
        {
            using var context = new AppDbContext();
            context.VocabularyWords.Add(word);
            await context.SaveChangesAsync();
            return word;
        }

        public static async Task<VocabularyWord> UpdateWordAsync(VocabularyWord word)
        {
            using var context = new AppDbContext();
            context.VocabularyWords.Update(word);
            await context.SaveChangesAsync();
            return word;
        }

        public static async Task<bool> DeleteWordAsync(int wordId)
        {
            using var context = new AppDbContext();
            var word = await context.VocabularyWords.FindAsync(wordId);
            if (word != null)
            {
                word.IsActive = false;
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public static async Task<List<string>> GetCategoriesAsync()
        {
            using var context = new AppDbContext();
            return await context.VocabularyWords
                .Where(w => w.IsActive && !string.IsNullOrEmpty(w.Category))
                .Select(w => w.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public static async Task<UserVocabulary?> GetUserVocabularyAsync(int userId, int wordId)
        {
            using var context = new AppDbContext();
            return await context.UserVocabularies
                .Include(uv => uv.VocabularyWord)
                .FirstOrDefaultAsync(uv => uv.UserId == userId && uv.VocabularyWordId == wordId);
        }

        public static async Task<List<UserVocabulary>> GetUserVocabulariesAsync(int userId)
        {
            using var context = new AppDbContext();
            return await context.UserVocabularies
                .Include(uv => uv.VocabularyWord)
                .Where(uv => uv.UserId == userId)
                .OrderByDescending(uv => uv.LastReviewedDate)
                .ToListAsync();
        }

        public static async Task<UserVocabulary> AddOrUpdateUserVocabularyAsync(int userId, int wordId, bool isCorrect)
        {
            using var context = new AppDbContext();
            
            var userVocab = await context.UserVocabularies
                .FirstOrDefaultAsync(uv => uv.UserId == userId && uv.VocabularyWordId == wordId);

            if (userVocab == null)
            {
                userVocab = new UserVocabulary
                {
                    UserId = userId,
                    VocabularyWordId = wordId,
                    FirstLearnedDate = DateTime.Now,
                    MasteryLevel = 1
                };
                context.UserVocabularies.Add(userVocab);
            }

            userVocab.LastReviewedDate = DateTime.Now;
            userVocab.ReviewCount++;
            userVocab.TotalAttempts++;

            if (isCorrect)
            {
                userVocab.CorrectAnswers++;
                if (userVocab.MasteryLevel < 5)
                {
                    userVocab.MasteryLevel++;
                }
            }
            else
            {
                if (userVocab.MasteryLevel > 1)
                {
                    userVocab.MasteryLevel--;
                }
            }

            await context.SaveChangesAsync();
            return userVocab;
        }

        public static async Task<List<VocabularyWord>> GetWordsForReviewAsync(int userId, int maxWords = 10)
        {
            using var context = new AppDbContext();

            // Get words that need review based on spaced repetition algorithm
            var wordsForReview = await context.UserVocabularies
                .Include(uv => uv.VocabularyWord)
                .Where(uv => uv.UserId == userId &&
                    uv.VocabularyWord.IsActive &&
                    (uv.LastReviewedDate == null ||
                     uv.LastReviewedDate.Value.AddDays(GetReviewInterval(uv.MasteryLevel)) <= DateTime.Now))
                .OrderBy(uv => uv.LastReviewedDate ?? DateTime.MinValue)
                .Take(maxWords)
                .Select(uv => uv.VocabularyWord)
                .ToListAsync();

            // If not enough words for review, add some new words
            if (wordsForReview.Count < maxWords)
            {
                var learnedWordIds = await context.UserVocabularies
                    .Where(uv => uv.UserId == userId)
                    .Select(uv => uv.VocabularyWordId)
                    .ToListAsync();

                var newWords = await context.VocabularyWords
                    .Where(w => w.IsActive && !learnedWordIds.Contains(w.Id))
                    .OrderBy(w => w.Difficulty)
                    .Take(maxWords - wordsForReview.Count)
                    .ToListAsync();

                wordsForReview.AddRange(newWords);
            }

            return wordsForReview;
        }

        public static async Task<Dictionary<string, int>> GetUserStatsAsync(int userId)
        {
            using var context = new AppDbContext();

            var stats = new Dictionary<string, int>();

            var userVocabs = await context.UserVocabularies
                .Where(uv => uv.UserId == userId)
                .ToListAsync();

            stats["TotalWordsLearned"] = userVocabs.Count;
            stats["MasteredWords"] = userVocabs.Count(uv => uv.IsMastered);
            stats["WordsInProgress"] = userVocabs.Count(uv => !uv.IsMastered);
            stats["TotalReviews"] = userVocabs.Sum(uv => uv.ReviewCount);
            stats["AverageAccuracy"] = userVocabs.Count > 0 ?
                (int)userVocabs.Average(uv => uv.AccuracyRate) : 0;
            stats["CurrentStreak"] = await CalculateCurrentStreakAsync(userId);

            return stats;
        }

        public static async Task<List<VocabularyWord>> GetWeakWordsAsync(int userId, int count = 5)
        {
            using var context = new AppDbContext();

            return await context.UserVocabularies
                .Include(uv => uv.VocabularyWord)
                .Where(uv => uv.UserId == userId && uv.VocabularyWord.IsActive)
                .OrderBy(uv => uv.AccuracyRate)
                .ThenBy(uv => uv.MasteryLevel)
                .Take(count)
                .Select(uv => uv.VocabularyWord)
                .ToListAsync();
        }

        private static int GetReviewInterval(int masteryLevel)
        {
            // Spaced repetition intervals in days
            return masteryLevel switch
            {
                1 => 1,  // Review after 1 day
                2 => 3,  // Review after 3 days
                3 => 7,  // Review after 1 week
                4 => 14, // Review after 2 weeks
                5 => 30, // Review after 1 month
                _ => 1
            };
        }

        private static async Task<int> CalculateCurrentStreakAsync(int userId)
        {
            using var context = new AppDbContext();

            var recentReviews = await context.UserVocabularies
                .Where(uv => uv.UserId == userId && uv.LastReviewedDate.HasValue)
                .OrderByDescending(uv => uv.LastReviewedDate)
                .Take(30) // Check last 30 reviews
                .ToListAsync();

            int streak = 0;
            var currentDate = DateTime.Now.Date;

            foreach (var review in recentReviews)
            {
                var reviewDate = review.LastReviewedDate!.Value.Date;
                var daysDiff = (currentDate - reviewDate).Days;

                if (daysDiff == streak)
                {
                    streak++;
                }
                else if (daysDiff > streak)
                {
                    break;
                }
            }

            return streak;
        }

        public static async Task SeedSampleWordsAsync()
        {
            using var context = new AppDbContext();

            // Check if we already have words
            if (await context.VocabularyWords.AnyAsync())
                return;

            var sampleWords = new List<VocabularyWord>
            {
                // Greetings
                new VocabularyWord
                {
                    EnglishWord = "Hello",
                    TurkishMeaning = "Merhaba",
                    Pronunciation = "/həˈloʊ/",
                    ExampleSentence = "Hello, how are you?",
                    ExampleSentenceTurkish = "Merhaba, nasılsın?",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Interjection,
                    Category = "Greetings"
                },
                new VocabularyWord
                {
                    EnglishWord = "Goodbye",
                    TurkishMeaning = "Hoşçakal, güle güle",
                    Pronunciation = "/ɡʊdˈbaɪ/",
                    ExampleSentence = "Goodbye, see you tomorrow!",
                    ExampleSentenceTurkish = "Hoşçakal, yarın görüşürüz!",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Interjection,
                    Category = "Greetings"
                },
                new VocabularyWord
                {
                    EnglishWord = "Thank you",
                    TurkishMeaning = "Teşekkür ederim",
                    Pronunciation = "/θæŋk juː/",
                    ExampleSentence = "Thank you for your help.",
                    ExampleSentenceTurkish = "Yardımın için teşekkür ederim.",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Interjection,
                    Category = "Greetings"
                },

                // Family
                new VocabularyWord
                {
                    EnglishWord = "Family",
                    TurkishMeaning = "Aile",
                    Pronunciation = "/ˈfæməli/",
                    ExampleSentence = "I love spending time with my family.",
                    ExampleSentenceTurkish = "Ailemle vakit geçirmeyi seviyorum.",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Noun,
                    Category = "Family"
                },
                new VocabularyWord
                {
                    EnglishWord = "Mother",
                    TurkishMeaning = "Anne",
                    Pronunciation = "/ˈmʌðər/",
                    ExampleSentence = "My mother is a teacher.",
                    ExampleSentenceTurkish = "Annem bir öğretmen.",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Noun,
                    Category = "Family"
                },
                new VocabularyWord
                {
                    EnglishWord = "Father",
                    TurkishMeaning = "Baba",
                    Pronunciation = "/ˈfɑːðər/",
                    ExampleSentence = "My father works in a bank.",
                    ExampleSentenceTurkish = "Babam bir bankada çalışıyor.",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Noun,
                    Category = "Family"
                },

                // Adjectives
                new VocabularyWord
                {
                    EnglishWord = "Beautiful",
                    TurkishMeaning = "Güzel",
                    Pronunciation = "/ˈbjuːtɪfəl/",
                    ExampleSentence = "She has a beautiful smile.",
                    ExampleSentenceTurkish = "Onun güzel bir gülümsemesi var.",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Adjective,
                    Category = "Adjectives"
                },
                new VocabularyWord
                {
                    EnglishWord = "Intelligent",
                    TurkishMeaning = "Zeki, akıllı",
                    Pronunciation = "/ɪnˈtelɪdʒənt/",
                    ExampleSentence = "She is a very intelligent student.",
                    ExampleSentenceTurkish = "O çok zeki bir öğrenci.",
                    Difficulty = WordDifficulty.Intermediate,
                    PartOfSpeech = PartOfSpeech.Adjective,
                    Category = "Adjectives"
                },
                new VocabularyWord
                {
                    EnglishWord = "Sophisticated",
                    TurkishMeaning = "Sofistike, karmaşık",
                    Pronunciation = "/səˈfɪstɪkeɪtɪd/",
                    ExampleSentence = "The software has a sophisticated design.",
                    ExampleSentenceTurkish = "Yazılımın sofistike bir tasarımı var.",
                    Difficulty = WordDifficulty.Advanced,
                    PartOfSpeech = PartOfSpeech.Adjective,
                    Category = "Adjectives"
                },

                // Education
                new VocabularyWord
                {
                    EnglishWord = "Learn",
                    TurkishMeaning = "Öğrenmek",
                    Pronunciation = "/lɜːrn/",
                    ExampleSentence = "I want to learn English.",
                    ExampleSentenceTurkish = "İngilizce öğrenmek istiyorum.",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Verb,
                    Category = "Education"
                },
                new VocabularyWord
                {
                    EnglishWord = "Study",
                    TurkishMeaning = "Çalışmak, ders çalışmak",
                    Pronunciation = "/ˈstʌdi/",
                    ExampleSentence = "I study English every day.",
                    ExampleSentenceTurkish = "Her gün İngilizce çalışıyorum.",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Verb,
                    Category = "Education"
                },
                new VocabularyWord
                {
                    EnglishWord = "Knowledge",
                    TurkishMeaning = "Bilgi",
                    Pronunciation = "/ˈnɒlɪdʒ/",
                    ExampleSentence = "Knowledge is power.",
                    ExampleSentenceTurkish = "Bilgi güçtür.",
                    Difficulty = WordDifficulty.Intermediate,
                    PartOfSpeech = PartOfSpeech.Noun,
                    Category = "Education"
                },

                // Technology
                new VocabularyWord
                {
                    EnglishWord = "Computer",
                    TurkishMeaning = "Bilgisayar",
                    Pronunciation = "/kəmˈpjuːtər/",
                    ExampleSentence = "I use my computer every day.",
                    ExampleSentenceTurkish = "Bilgisayarımı her gün kullanırım.",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Noun,
                    Category = "Technology"
                },
                new VocabularyWord
                {
                    EnglishWord = "Internet",
                    TurkishMeaning = "İnternet",
                    Pronunciation = "/ˈɪntərnet/",
                    ExampleSentence = "The internet connects people worldwide.",
                    ExampleSentenceTurkish = "İnternet dünya çapında insanları bağlar.",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Noun,
                    Category = "Technology"
                },
                new VocabularyWord
                {
                    EnglishWord = "Artificial Intelligence",
                    TurkishMeaning = "Yapay Zeka",
                    Pronunciation = "/ˌɑːrtɪˈfɪʃəl ɪnˈtelɪdʒəns/",
                    ExampleSentence = "Artificial intelligence is changing our world.",
                    ExampleSentenceTurkish = "Yapay zeka dünyamızı değiştiriyor.",
                    Difficulty = WordDifficulty.Advanced,
                    PartOfSpeech = PartOfSpeech.Noun,
                    Category = "Technology"
                }
            };

            context.VocabularyWords.AddRange(sampleWords);
            await context.SaveChangesAsync();
        }
    }
}
